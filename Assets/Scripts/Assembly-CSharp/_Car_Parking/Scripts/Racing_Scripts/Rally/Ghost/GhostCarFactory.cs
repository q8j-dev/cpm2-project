using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Ghost;
using CPM.Ghost.Core;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using _Car_Parking.Scripts.EngineTuning;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Ghost
{
	public sealed class GhostCarFactory : IGhostCarFactory
	{
		private readonly IGhostLogger _log;

		private readonly Dictionary<GameObject, AsyncOperationHandle<GameObject>> _handles = new Dictionary<GameObject, AsyncOperationHandle<GameObject>>();

		[Inject]
		public GhostCarFactory(IGhostLogger log)
		{
			_log = log;
		}

		public async UniTask<GameObject> CreateAsync(int carId, GhostMaterialPreset preset, int[] lodWheelPool, LayerMask groundLayers, Transform observer, CancellationToken ct)
		{
			GameObject gameObject = Resources.Load<GameObject>($"car{carId}");
			if (!gameObject)
			{
				_log.Error($"[GhostCarFactory] car prefab not found: car{carId}");
				return null;
			}
			VehicleData component = gameObject.GetComponent<VehicleData>();
			Vector3 bodyOffset = ((component != null) ? component.MainModelPosition : Vector3.zero);
			(string Name, bool IsLeft)[] wheelDefs = new (string, bool)[4]
			{
				("WheelFL", true),
				("WheelFR", false),
				("WheelRL", true),
				("WheelRR", false)
			};
			Dictionary<string, Vector3> wheelPoses = new Dictionary<string, Vector3>();
			Transform transform = gameObject.transform.Find("Wheels");
			(string, bool)[] array;
			if (transform == null)
			{
				_log.Warn($"[GhostCarFactory] 'Wheels' node not found on prefab car{carId}");
			}
			else
			{
				array = wheelDefs;
				for (int i = 0; i < array.Length; i++)
				{
					string item = array[i].Item1;
					Transform transform2 = transform.Find(item);
					if (transform2 == null)
					{
						_log.Warn($"[GhostCarFactory] Wheel '{item}' not found on prefab car{carId}");
					}
					else
					{
						wheelPoses[item] = gameObject.transform.InverseTransformPoint(transform2.position);
					}
				}
			}
			int frontRimId = 0;
			int rearRimId = 0;
			VehicleStockTuningData vehicleStockTuningData = Resources.Load<VehicleStockTuningData>($"Configs/CarTuningData/car{carId}");
			if (vehicleStockTuningData != null)
			{
				frontRimId = (vehicleStockTuningData.Tuning?.FrontWheelData?.RimModelId).GetValueOrDefault();
				rearRimId = (vehicleStockTuningData.Tuning?.RearWheelData?.RimModelId).GetValueOrDefault();
			}
			else
			{
				_log.Warn($"[GhostCarFactory] VehicleStockTuningData not found for car{carId}, using rim id 0");
			}
			float frontWheelDrop = SuspensionRestDrop(vehicleStockTuningData?.Tuning?.FrontWheelData?.SuspensionData);
			float rearWheelDrop = SuspensionRestDrop(vehicleStockTuningData?.Tuning?.RearWheelData?.SuspensionData);
			GameObject ghost = new GameObject($"Ghost_{carId}");
			ghost.SetActive(value: false);
			AsyncOperationHandle<GameObject> lodHandle = Addressables.LoadAssetAsync<GameObject>($"body/_LOD/b{carId}_LOD.prefab");
			try
			{
				await lodHandle.ToUniTask(null, PlayerLoopTiming.Update, ct);
			}
			catch (OperationCanceledException)
			{
				if (lodHandle.IsValid())
				{
					Addressables.Release(lodHandle);
				}
				UnityEngine.Object.Destroy(ghost);
				throw;
			}
			catch (Exception ex2)
			{
				_log.Error($"[GhostCarFactory] LOD body load failed for car{carId}: {ex2.Message}");
				if (lodHandle.IsValid())
				{
					Addressables.Release(lodHandle);
				}
				UnityEngine.Object.Destroy(ghost);
				return null;
			}
			if (lodHandle.Status != AsyncOperationStatus.Succeeded || lodHandle.Result == null)
			{
				_log.Error($"[GhostCarFactory] LOD body handle failed for car{carId}");
				if (lodHandle.IsValid())
				{
					Addressables.Release(lodHandle);
				}
				UnityEngine.Object.Destroy(ghost);
				return null;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(lodHandle.Result, ghost.transform);
			gameObject2.transform.localPosition = bodyOffset;
			gameObject2.transform.localRotation = Quaternion.identity;
			array = wheelDefs;
			for (int i = 0; i < array.Length; i++)
			{
				var (text, flag) = array[i];
				if (wheelPoses.TryGetValue(text, out var value))
				{
					bool flag2 = text == "WheelFL" || text == "WheelFR";
					int num = (flag2 ? frontRimId : rearRimId);
					value.y -= (flag2 ? frontWheelDrop : rearWheelDrop);
					GameObject gameObject3 = Resources.Load<GameObject>($"WheelLow/wheelLow {num}");
					if (!gameObject3)
					{
						_log.Warn($"[GhostCarFactory] LOD wheel prefab not found: WheelLow/wheelLow {num} (wheel={text}, car={carId})");
						continue;
					}
					GameObject gameObject4 = UnityEngine.Object.Instantiate(gameObject3, ghost.transform);
					gameObject4.name = text;
					gameObject4.transform.localPosition = value;
					gameObject4.transform.localRotation = Quaternion.identity;
					gameObject4.transform.localScale = (flag ? Vector3.one : new Vector3(-1f, 1f, 1f));
				}
			}
			ghost.SetActive(value: true);
			ghost.AddComponent<GhostInstance>().Setup(observer, preset);
			_handles[ghost] = lodHandle;
			return ghost;
		}

		private static float SuspensionRestDrop(SuspensionData suspension)
		{
			if (suspension == null)
			{
				return 0f;
			}
			return Mathf.Clamp(suspension.SpringLength * 0.0254f, 0.0254f, 0.635f) / 3f;
		}

		public void Release(GameObject ghost)
		{
			if (!ghost)
			{
				return;
			}
			if (_handles.TryGetValue(ghost, out var value))
			{
				if (value.IsValid())
				{
					Addressables.Release(value);
				}
				_handles.Remove(ghost);
			}
			UnityEngine.Object.Destroy(ghost);
		}
	}
}
