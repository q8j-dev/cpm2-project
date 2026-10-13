using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceCarBodyKitsUpdater : MonoBehaviour
	{
		private LoadAndSpawnPrefab _loadAndSpawnPrefab;

		private readonly Dictionary<PoliceBodyKitType, CancellationTokenSource> _cancellations = new Dictionary<PoliceBodyKitType, CancellationTokenSource>();

		private List<PoliceCarBodyKit> _policeCarBodyKits = new List<PoliceCarBodyKit>();

		private PoliceSystemConfig _policeConfig;

		private VehicleData _vehicleData;

		private Array AllBodyKitTypes => Enum.GetValues(typeof(PoliceBodyKitType));

		public List<PoliceCarBodyKit> PoliceCarBodyKits => _policeCarBodyKits;

		public event Action<PoliceCarBodyKitsUpdater, PoliceCarBodyKit> OnNewBodyKit;

		[Inject]
		public void Construct(PoliceSystemConfig policeConfig, IPrefabFactory prefabFactory)
		{
			_policeConfig = policeConfig;
			_loadAndSpawnPrefab = new LoadAndSpawnPrefab(prefabFactory);
		}

		public PoliceCarBodyKit GetBodyKit(PoliceBodyKitType policeBodyKitType)
		{
			for (int i = 0; i < _policeCarBodyKits.Count; i++)
			{
				if ((bool)_policeCarBodyKits[i] && _policeCarBodyKits[i].BodyKitType == policeBodyKitType)
				{
					return _policeCarBodyKits[i];
				}
			}
			return null;
		}

		public void Init(VehicleData vehicleData)
		{
			_vehicleData = vehicleData;
		}

		public void SetBodyKit(PoliceBodyKitType policeBodyKitType, int index)
		{
			if (index < 0)
			{
				RemoveBodyKit(policeBodyKitType);
				return;
			}
			CancelSetAsync(policeBodyKitType);
			PoliceCarBodyKit bodyKit = GetBodyKit(policeBodyKitType);
			if (!bodyKit || bodyKit.Index != index)
			{
				RemoveBodyKit(policeBodyKitType);
				CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
				_cancellations[policeBodyKitType] = cancellationTokenSource;
				SetAsync(policeBodyKitType, index, cancellationTokenSource.Token).Forget();
			}
		}

		public void RemoveBodyKit(PoliceBodyKitType policeBodyKitType)
		{
			for (int i = 0; i < _policeCarBodyKits.Count; i++)
			{
				if ((bool)_policeCarBodyKits[i] && _policeCarBodyKits[i].BodyKitType == policeBodyKitType)
				{
					CancelSetAsync(policeBodyKitType);
					UnityEngine.Object.Destroy(_policeCarBodyKits[i].gameObject);
					_policeCarBodyKits.RemoveAt(i);
				}
			}
		}

		public void Clear()
		{
			foreach (PoliceBodyKitType allBodyKitType in AllBodyKitTypes)
			{
				RemoveBodyKit(allBodyKitType);
			}
			_policeCarBodyKits.Clear();
		}

		private void OnDestroy()
		{
			foreach (KeyValuePair<PoliceBodyKitType, CancellationTokenSource> cancellation in _cancellations)
			{
				cancellation.Value?.Cancel();
				cancellation.Value?.Dispose();
			}
			_cancellations.Clear();
			_loadAndSpawnPrefab.Dispose();
		}

		private void CancelSetAsync(PoliceBodyKitType policeBodyKitType)
		{
			if (_cancellations.TryGetValue(policeBodyKitType, out var value))
			{
				value?.Cancel();
				value?.Dispose();
			}
			_cancellations.Remove(policeBodyKitType);
		}

		private async UniTaskVoid SetAsync(PoliceBodyKitType policeBodyKitType, int index, CancellationToken cancellationToken)
		{
			if (!_vehicleData)
			{
				return;
			}
			PoliceCarBodyKitsConfig config = _policeConfig.PoliceCarConfigs.GetConfig(_vehicleData.CarId);
			if (!config)
			{
				return;
			}
			PoliceBodyKitSettingsConfig bodyKitsSettingsConfig = _policeConfig.GetBodyKitsSettingsConfig(policeBodyKitType);
			PoliceCarBodyKitsConfig.BodyKitData bodyKit = config.GetBodyKit(policeBodyKitType, index);
			if (bodyKit != null)
			{
				GameObject gameObject = await _loadAndSpawnPrefab.Spawn(policeBodyKitType.ToString(), bodyKitsSettingsConfig.GetAddressablePath(index), bodyKit.TransformData, base.transform, cancellationToken);
				if (!cancellationToken.IsCancellationRequested && (bool)gameObject && (bool)_vehicleData)
				{
					PoliceCarBodyKit component = gameObject.GetComponent<PoliceCarBodyKit>();
					component.Index = index;
					_policeCarBodyKits.Add(component);
					OnNewBodyKit?.Invoke(this, component);
				}
			}
		}

		public void ToggleTopLights(bool enable)
		{
			foreach (PoliceCarBodyKit policeCarBodyKit in _policeCarBodyKits)
			{
				if (policeCarBodyKit.BodyKitType == PoliceBodyKitType.TopLight)
				{
					policeCarBodyKit.SetActive(enable);
				}
			}
		}
	}
}
