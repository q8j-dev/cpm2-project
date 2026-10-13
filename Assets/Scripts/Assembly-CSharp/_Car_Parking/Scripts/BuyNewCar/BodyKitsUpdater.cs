using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.BuyNewCar
{
	public class BodyKitsUpdater : MonoBehaviour
	{
		public struct BodyKitsData
		{
			public int CarID;

			public BodyKitType Type;

			public int Index;

			public BodyKitsData(int carID, BodyKitType type, int index)
			{
				CarID = carID;
				Type = type;
				Index = index;
			}
		}

		private LoadAndSpawnPrefab _loadAndSpawnPrefab;

		private readonly List<bodyKitValues> _bodyKitValues = new List<bodyKitValues>();

		private CancellationTokenSource _cancellationTokenSource;

		private readonly Dictionary<BodyKitType, int> _requestGeneration = new Dictionary<BodyKitType, int>();

		private int _pendingLoads;

		private const float WaitForKitsTimeoutSeconds = 5f;

		public bool InstantShow { get; set; }

		public bool HasPendingLoads => _pendingLoads > 0;

		public async UniTask WaitForPendingAsync(CancellationToken cancellationToken)
		{
			float deadline = Time.realtimeSinceStartup + 5f;
			while ((_pendingLoads > 0 || IsAnyKitFading()) && !(Time.realtimeSinceStartup >= deadline) && !(await UniTask.Yield(PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow()))
			{
			}
		}

		private bool IsAnyKitFading()
		{
			for (int i = 0; i < _bodyKitValues.Count; i++)
			{
				if ((bool)_bodyKitValues[i] && _bodyKitValues[i].IsFading)
				{
					return true;
				}
			}
			return false;
		}

		[Inject]
		public void Construct(IPrefabFactory prefabFactory)
		{
			_loadAndSpawnPrefab = new LoadAndSpawnPrefab(prefabFactory);
		}

		public void AddBodyKit(bodyKitValues bodyKit)
		{
			_bodyKitValues.Add(bodyKit);
		}

		public void SetBodyKits(BodyKitsData bodyKitsData, TransformData localTransformData)
		{
			if (_cancellationTokenSource == null)
			{
				_cancellationTokenSource = new CancellationTokenSource();
			}
			int num = ((!_requestGeneration.TryGetValue(bodyKitsData.Type, out var value)) ? 1 : (value + 1));
			_requestGeneration[bodyKitsData.Type] = num;
			string pathToPrefab = GetPathToPrefab(bodyKitsData.CarID, bodyKitsData.Type, bodyKitsData.Index);
			_pendingLoads++;
			TrackPendingLoad(LoadBodyKit(bodyKitsData, localTransformData, pathToPrefab, num, _cancellationTokenSource.Token)).Forget();
		}

		private async UniTaskVoid TrackPendingLoad(UniTask load)
		{
			try
			{
				await load;
			}
			finally
			{
				_pendingLoads--;
			}
		}

		private bool IsSuperseded(BodyKitType type, int generation)
		{
			if (_requestGeneration.TryGetValue(type, out var value))
			{
				return value != generation;
			}
			return true;
		}

		private static bool IsEmptyDefaultType(BodyKitType type)
		{
			if (type != BodyKitType.Spoiler && type != BodyKitType.AirIntakeHood)
			{
				return type == BodyKitType.AirIntakeRoof;
			}
			return true;
		}

		private void OnDestroy()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSource);
			_loadAndSpawnPrefab.Dispose();
		}

		private async UniTask LoadBodyKit(BodyKitsData bodyKitsData, TransformData localTransformData, string path, int generation, CancellationToken cancellationToken)
		{
			for (int attempt = 0; attempt < 8; attempt++)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					break;
				}
				if (IsSuperseded(bodyKitsData.Type, generation))
				{
					break;
				}
				GameObject gameObject = await _loadAndSpawnPrefab.Spawn(bodyKitsData.Type.ToString(), path, localTransformData, base.transform, cancellationToken);
				if (cancellationToken.IsCancellationRequested || IsSuperseded(bodyKitsData.Type, generation))
				{
					if ((bool)gameObject)
					{
						Object.Destroy(gameObject);
					}
					break;
				}
				if ((bool)gameObject && !gameObject.GetComponentInChildren<Renderer>(includeInactive: true))
				{
					if (bodyKitsData.Index == 0 && IsEmptyDefaultType(bodyKitsData.Type))
					{
						DeleteBodyKit(bodyKitsData.Type);
						Object.Destroy(gameObject);
						break;
					}
					Object.Destroy(gameObject);
					await UniTask.Delay(300, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
					continue;
				}
				if ((bool)gameObject)
				{
					bodyKitValues bodyKitValues2 = gameObject.GetComponent<bodyKitValues>();
					if ((bool)bodyKitValues2 && bodyKitValues2.localPosition != Vector3.zero)
					{
						gameObject.transform.localPosition = bodyKitValues2.localPosition;
					}
					if ((bool)bodyKitValues2 && bodyKitValues2.localEuler != Vector3.zero)
					{
						gameObject.transform.localEulerAngles = bodyKitValues2.localEuler;
					}
					if (!bodyKitValues2)
					{
						bodyKitValues2 = gameObject.AddComponent<bodyKitValues>();
					}
					bodyKitValues2.BodyKitType = bodyKitsData.Type;
					DeleteBodyKit(bodyKitsData.Type);
					_bodyKitValues.Add(bodyKitValues2);
					if (InstantShow)
					{
						bodyKitValues2.ShowInstant();
					}
					else
					{
						bodyKitValues2.StartShowing();
					}
					break;
				}
				await UniTask.Delay(300, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			}
		}

		private string GetPathToPrefab(int carID, BodyKitType bodyKitType, int index)
		{
			string text = "";
			switch (bodyKitType)
			{
			case BodyKitType.Spoiler:
				text = "bodyKits/spoiler";
				break;
			case BodyKitType.FrontBumper:
				text = $"bodyKits/kit{carID}/fbumper";
				break;
			case BodyKitType.RearBumper:
				text = $"bodyKits/kit{carID}/rbumper";
				break;
			case BodyKitType.Skirt:
				text = $"bodyKits/kit{carID}/skirts";
				break;
			case BodyKitType.Hood:
				text = $"bodyKits/kit{carID}/hood";
				break;
			case BodyKitType.AirIntakeHood:
			case BodyKitType.AirIntakeRoof:
				text = "bodyKits/airintake";
				break;
			case BodyKitType.Fender:
				text = $"bodyKits/kit{carID}/fender";
				break;
			}
			if (!string.IsNullOrEmpty(text))
			{
				text += index;
				text += ".prefab";
			}
			return text;
		}

		private void DeleteBodyKit(BodyKitType type)
		{
			for (int i = 0; i < _bodyKitValues.Count; i++)
			{
				if (_bodyKitValues[i].BodyKitType == type)
				{
					if (InstantShow)
					{
						Object.Destroy(_bodyKitValues[i].gameObject);
					}
					else
					{
						_bodyKitValues[i].StartFade();
					}
					_bodyKitValues.RemoveAt(i);
					break;
				}
			}
		}
	}
}
