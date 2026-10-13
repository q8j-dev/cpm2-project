using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database.Constructors;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Sirenix.Utilities;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.CommonUtility.Contracts;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.CarLegitService;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;
using _Car_Parking.Scripts.Other.FsoStrobeLights;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.VinylsAi;

namespace CPM.Database
{
	public class CarFactoryService : IDisposable
	{
		private ICarTuningFactory _carTuningFactory;

		private IPrefabFactory _prefabFactory;

		private DrawQueue _drawQueue;

		private IPlayerStorageService _playerStorageService;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private IProgressBackend _progressBackend;

		private ICarLegitService _carLegitService;

		private readonly CancellationTokenSource _scopeCts = new CancellationTokenSource();

		private bool _disposed;

		private bool _wheelMismatchPopupShownThisSession;

		public const float BoostCustomThreshold = 20000f;

		private const float ChargerStaminaOverheadMultiplier = 1.15f;

		[Inject]
		public CarFactoryService(ICarTuningFactory carTuningFactory, OnlineCollectionsProvider onlineCollectionsProvider, IPrefabFactory prefabFactory, DrawQueue drawQueue, IPlayerStorageService playerStorageService, IProgressBackend progressBackend, ICarLegitService carLegitService)
		{
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_playerStorageService = playerStorageService;
			_drawQueue = drawQueue;
			_carTuningFactory = carTuningFactory;
			_prefabFactory = prefabFactory;
			_progressBackend = progressBackend;
			_carLegitService = carLegitService;
		}

		public async UniTask<GameObject> AssemblyMyCar(CarInfo carInfo, Vector3 pos, Quaternion rot, int slotId, bool waitBodyLoad = true, bool isMultiplayer = false, bool isPlayerCar = true)
		{
			if (_disposed)
			{
				return null;
			}
			CancellationToken scopeToken = _scopeCts.Token;
			if (!CarIdStorage.IsValidCarId(carInfo.CarID))
			{
				Debug.LogWarning(carInfo.CarID + " is not a valid car id (out of range)");
				return null;
			}
			CarInfo carInfo2 = carInfo;
			if (carInfo2.AdditionalData == null)
			{
				carInfo2.AdditionalData = new AdditionalData();
			}
			carInfo2 = carInfo;
			if (carInfo2.Colors == null)
			{
				carInfo2.Colors = CarTuningFactory.GetDefaultCarTuningDataConfig(carInfo.CarID).CarInfoDefault.Colors.Clone() as Colors;
			}
			ClampWheelDataToLimits(carInfo, isLocalCar: true);
			ResourceRequest carPrefabRequest = Resources.LoadAsync<GameObject>("car" + carInfo.CarID);
			CarEntity carEntity = _carTuningFactory.AssemblyCar(carInfo, checkNulls: true);
			VehicleDataResult vehicleDataResult = default;
			bool carResultWasCalculated = false;
			GameObject car;
			if (isMultiplayer)
			{
				carResultWasCalculated = true;
				vehicleDataResult = carEntity.GetValue();
				Debug.Log($"CreateMultiplayerCar:{carInfo.CarID}");
				object[] data = MultiplayerDataSending.SetDataForSend(carInfo.CarID, carInfo, vehicleDataResult, _playerStorageService.AccountID);
				if (carInfo.GetHaveAdultVinyls())
				{
					((object[])data[6])[2] = null;
					((object[])data[6])[5] = null;
				}
				bool flag;
				try
				{
					flag = await _carLegitService.IsCheckEnabledAsync(scopeToken);
				}
				catch (OperationCanceledException)
				{
					return null;
				}
				if (!flag)
				{
					_progressBackend.RegisterCar(carInfo.CarInstanceId, carInfo.CarID, scopeToken).Forget();
				}
				else
				{
					bool needsBackgroundRetry = false;
					try
					{
						Result<bool> result = await _progressBackend.RegisterCar(carInfo.CarInstanceId, carInfo.CarID, scopeToken).Timeout(TimeSpan.FromMilliseconds(3000.0));
						if (result.IsFail)
						{
							Debug.LogWarning($"[CarFactory] RegisterCar IsFail for {carInfo.CarInstanceId} carId={carInfo.CarID} - spawning anyway, will retry in background");
							needsBackgroundRetry = true;
						}
						else if (!result.Value)
						{
							Debug.LogWarning($"[CarFactory] RegisterCar returned false (server rejected) for {carInfo.CarInstanceId} carId={carInfo.CarID} - spawning anyway, NOT retrying");
						}
					}
					catch (OperationCanceledException)
					{
						return null;
					}
					catch (TimeoutException)
					{
						Debug.LogWarning($"[CarFactory] RegisterCar timed out (>{3000}ms) for {carInfo.CarInstanceId} - spawning anyway, will retry in background");
						needsBackgroundRetry = true;
					}
					catch (Exception ex4)
					{
						Debug.LogWarning("[CarFactory] RegisterCar threw for " + carInfo.CarInstanceId + ": " + ex4.Message + " - spawning anyway, will retry in background");
						needsBackgroundRetry = true;
					}
					if (needsBackgroundRetry)
					{
						_progressBackend.RegisterCarWithBackoff(carInfo.CarInstanceId, carInfo.CarID, scopeToken).Forget();
					}
				}
				await carPrefabRequest;
				try
				{
					car = PhotonNetwork.Instantiate("car" + carInfo.CarID, pos, rot, 0, data);
					_onlineCollectionsProvider.RegisterCar(car.GetComponent<MultiPlayerPosControl>());
					car.GetComponent<MultiPlayerPosControl>().InterestGroup = InterestGroupManager.CarInterestGroup;
					car.GetComponent<PhotonView>().Owner.NickName = _playerStorageService.Name;
					PhotonNetwork.NickName = _playerStorageService.Name;
				}
				catch (Exception message)
				{
					Debug.LogError(message);
					return null;
				}
			}
			else
			{
				GameObject gameObject = (GameObject)(await carPrefabRequest);
				if (!gameObject)
				{
					Debug.LogWarning($"[CarFactory] Car prefab 'car{carInfo.CarID}' not found in Resources");
					return null;
				}
				car = _prefabFactory.Instantiate(gameObject, pos, rot);
			}
			if (car.TryGetComponent<Rigidbody>(out var rigidbody))
			{
				rigidbody.isKinematic = true;
			}
			car.GetComponents<MonoBehaviour>().ForEach((MonoBehaviour comp) =>
			{
				if ((bool)comp)
				{
					comp.enabled = true;
				}
			});
			VehicleData component = car.GetComponent<VehicleData>();
			if (FlagsStorage.IsMainMenu || ScenesHelper.IsParkingLevelActive())
			{
				car.AddComponent<CarLODControl>();
			}
			component.VehicleMembersController.SetDriver(null);
			component.slotId = slotId;
			PlayerDatas.CarLoadingUtility.CreateMainCarValues(car);
			if (car.TryGetComponent<VehicleEntityProvider>(out var component2))
			{
				component2.CarEntity = carEntity;
				if (carResultWasCalculated)
				{
					component2.InitMultiplayerCar(vehicleDataResult);
				}
				else
				{
					component2.Init(initSuspension: true);
				}
			}
			await SetVisual(carInfo, waitBodyLoad, car, component, isLocalCar: true, isPlayerCar);
			if ((bool)rigidbody)
			{
				rigidbody.isKinematic = false;
			}
			return car;
		}

		public async UniTask SetVisual(CarInfo carInfo, bool waitBodyLoad, GameObject car, VehicleData vehicleData, bool isLocalCar = true, bool isPlayerCar = true)
		{
			vehicleData.ColorsService.CreateFirstMat(vehicleData);
			vehicleData.WindowComposition.CreateWindowMat();
			bool num = ClampWheelDataToLimits(carInfo, isLocalCar);
			WheelConstructor.SetWheel(vehicleData, carInfo.TuningData, (!isLocalCar) ? 16 : 0);
			if ((num & isLocalCar) && (bool)car && car.TryGetComponent<Powertrain>(out var component))
			{
				component.SetWheelParams(AxleType.Front, carInfo.TuningData.FrontWheelData);
				component.SetWheelParams(AxleType.Rear, carInfo.TuningData.RearWheelData);
			}
			vehicleData.DirtComposition?.SetDirtMatProps(updateWheelManually: true);
			vehicleData.BodyLoadingComposition.IsLocalPlayerCar = isLocalCar & isPlayerCar;
			UniTask task = vehicleData.BodyLoadingComposition.LoadFromResource(immediately: true);
			if (waitBodyLoad)
			{
				await task;
				await InstallVisualParts();
			}
			else
			{
				task.ContinueWith((Func<UniTask>)InstallVisualParts).Forget();
			}
			async UniTask InstallVisualParts()
			{
				if ((bool)car && car.TryGetComponent<BodyKits>(out var component2))
				{
					component2.TypesToIntall = carInfo.InstalledBodyKits.GetInstalledKitIds();
					component2.TrimsToIntall = carInfo.InstalledBodyKits.GetInstalledTrims();
					if (carInfo.Colors.BodyKits == null)
					{
						component2.colors = Array.Empty<Color>();
					}
					if (waitBodyLoad)
					{
						await component2.LoadAllKitsAsync();
						if (!vehicleData)
						{
							return;
						}
					}
					else
					{
						component2.LoadAllKits();
					}
				}
				if (!FlagsStorage.IsLevelScene && (bool)vehicleData)
				{
					vehicleData.FlagConstructor.SetFlag(vehicleData, carInfo.AdditionalData.FlagId);
					if (vehicleData.TryGetComponentInChildren<FsoStrobeLights>(out var component3, includeInactive: true))
					{
						component3.SpawnFromPrefs(carInfo);
					}
				}
				if (isLocalCar && (bool)vehicleData)
				{
					PlateConstructor.SetPlates(vehicleData, carInfo);
					_drawQueue.Enqueue(vehicleData.VinylsComposition);
				}
			}
		}

		public void Dispose()
		{
			if (!_disposed)
			{
				_disposed = true;
				_scopeCts.Cancel();
				_scopeCts.Dispose();
			}
		}

		private bool ClampWheelDataToLimits(CarInfo carInfo, bool isLocalCar)
		{
			if (carInfo?.TuningData == null)
			{
				return false;
			}
			VehicleStockTuningData vehicleStockTuningData = _carTuningFactory.CreateDefault(carInfo.CarID);
			if (vehicleStockTuningData?.Tuning == null)
			{
				return false;
			}
			bool isF1Car = CarIdStorage.F1Cars.Contains(carInfo.CarID);
			List<string> list = new List<string>();
			WheelData frontWheelData = carInfo.TuningData.FrontWheelData;
			if (frontWheelData != null)
			{
				float maxRadius = vehicleStockTuningData.Tuning.FrontWheelDataLimits?.MaxWheelRadius ?? 0f;
				int brakeDiscMinInch = frontWheelData.BrakesData?.GetMinWheelInch() ?? 0;
				List<string> list2 = WheelData.ClampToLimits(frontWheelData, maxRadius, brakeDiscMinInch, isF1Car);
				if (list2.Count > 0)
				{
					list.Add("front[" + string.Join("; ", list2) + "]");
				}
			}
			WheelData rearWheelData = carInfo.TuningData.RearWheelData;
			if (rearWheelData != null)
			{
				float maxRadius2 = vehicleStockTuningData.Tuning.RearWheelDataLimits?.MaxWheelRadius ?? 0f;
				int brakeDiscMinInch2 = rearWheelData.BrakesData?.GetMinWheelInch() ?? 0;
				List<string> list3 = WheelData.ClampToLimits(rearWheelData, maxRadius2, brakeDiscMinInch2, isF1Car);
				if (list3.Count > 0)
				{
					list.Add("rear[" + string.Join("; ", list3) + "]");
				}
			}
			AllParts instance = AllParts.Instance;
			ECUPart ecu = ((instance != null && carInfo.TuningData.ECU != null) ? instance.GetPart<ECUPart>(carInfo.TuningData.ECU) : null);
			ChargerPart charger = ((instance != null && carInfo.TuningData.EngineKitData?.Charger != null) ? instance.GetPart<ChargerPart>(carInfo.TuningData.EngineKitData.Charger) : null);
			List<string> list4 = SanitizeDynamicTuning(carInfo.TuningData.DynamicTuningSettings, ecu, charger);
			if (list.Count == 0 && list4.Count == 0)
			{
				return false;
			}
			if (!isLocalCar)
			{
				return list.Count > 0;
			}
			List<string> list5 = new List<string>(list);
			if (list4.Count > 0)
			{
				list5.Add("engine[" + string.Join("; ", list4) + "]");
			}
			Debug.LogWarning($"[CarFactory] Car {carInfo.CarID} corrected: " + string.Join(" ", list5));
			if (!string.IsNullOrEmpty(carInfo.CarInstanceId))
			{
				CarInfoDeferredSaver.RequestSave(carInfo);
			}
			else
			{
				Debug.LogWarning($"[CarFactory] Skipped clamp save for car {carInfo.CarID}: empty CarInstanceId");
			}
			if (list.Count == 0)
			{
				return false;
			}
			if (_wheelMismatchPopupShownThisSession)
			{
				return true;
			}
			_wheelMismatchPopupShownThisSession = true;
			ManagerUI managerUI = V.CoreGameManager?.ManagerUI;
			if (managerUI != null)
			{
				managerUI.ShowNotificationWindow(new LocalizedString("DragRacingCarSettingsUI.WheelError"), 3f).Forget();
			}
			return true;
		}

		private static List<string> SanitizeDynamicTuning(DynamicTuningSettings s, ECUPart ecu, ChargerPart charger)
		{
			List<string> list = new List<string>();
			if (s == null)
			{
				return list;
			}
			s.MixtureRatio = SanitizeCustomFloat(s.MixtureRatio, 8f, 20f, "MixtureRatio", list);
			if (ecu != null)
			{
				ECUValues getEcuValues = ecu.GetEcuValues;
				s.IdleRpm = SanitizeCustomFloat(s.IdleRpm, getEcuValues.RpmIdleMin, getEcuValues.RpmIdleMax, "IdleRpm", list);
				s.RedlineRpm = SanitizeCustomFloat(s.RedlineRpm, getEcuValues.RpmLimit * 0.7f, getEcuValues.RpmLimitMax, "RedlineRpm", list);
			}
			else
			{
				s.IdleRpm = ScrubNonFiniteFloat(s.IdleRpm, "IdleRpm", list);
				s.RedlineRpm = ScrubNonFiniteFloat(s.RedlineRpm, "RedlineRpm", list);
			}
			if (charger != null)
			{
				int maxPa = Mathf.RoundToInt((float)(int)charger.GetMaxPressure * 1.15f);
				s.Boost = SanitizeBoost(s.Boost, maxPa, list);
			}
			else
			{
				s.Boost = ScrubNonFiniteFloat(s.Boost, "Boost", list);
			}
			return list;
		}

		private static float SanitizeCustomFloat(float value, float min, float max, string fieldName, List<string> changes)
		{
			if (!float.IsFinite(value))
			{
				changes.Add($"{fieldName}: {value} → -1 (non-finite)");
				return -1f;
			}
			if (value <= 0f)
			{
				return value;
			}
			if (value < min)
			{
				changes.Add($"{fieldName}: {value:F0} → {min:F0} (< part min)");
				return min;
			}
			if (value > max)
			{
				changes.Add($"{fieldName}: {value:F0} → {max:F0} (> part max)");
				return max;
			}
			return value;
		}

		private static float SanitizeBoost(float value, int maxPa, List<string> changes)
		{
			if (!float.IsFinite(value))
			{
				changes.Add($"Boost: {value} → -1 (non-finite)");
				return -1f;
			}
			if (value <= 20000f)
			{
				return value;
			}
			if (value > (float)maxPa)
			{
				changes.Add($"Boost: {value:F0} → {maxPa} (> charger max)");
				return maxPa;
			}
			return value;
		}

		private static float ScrubNonFiniteFloat(float value, string fieldName, List<string> changes)
		{
			if (!float.IsFinite(value))
			{
				changes.Add($"{fieldName}: {value} → -1 (non-finite)");
				return -1f;
			}
			return value;
		}
	}
}
