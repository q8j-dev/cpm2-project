using System;
using System.Collections.Generic;
using System.Threading;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Connection;
using CPM.Database;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using _Car_Parking;
using _Car_Parking.Configs;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.AssetBundles;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Interior;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class InteriorsComposition
	{
		public CarInteriorReferences InitialInterier;

		public CarInteriorReferences Interior;

		private static readonly int RainPowerProperty = Shader.PropertyToID("_RainPower");

		private static readonly int StencilRefProperty = Shader.PropertyToID("_StencilRef");

		private const int InteriorStencilRef = 1;

		private readonly VehicleData _vehicle;

		private readonly IConnectionManager _connectionManager;

		private readonly DefaultMaterialsConfig _defaultMaterialsConfig;

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly CameraManager _cameraManager;

		private readonly SoundBankLoadService _soundBankService;

		private bool _dashboardBankHeld;

		public int CurrentInteriorId { get; private set; }

		public bool InteriorLoading { get; set; }

		public CancellationTokenSource SetInteriorCancellationToken { get; private set; } = new CancellationTokenSource();

		public InteriorInfoConfig InteriorInfoConfig { get; private set; }

		public GameObject SteeringWheelParent
		{
			get
			{
				if (!Interior || !Interior.gameObject.activeSelf || !Interior.SteeringWheel)
				{
					return null;
				}
				return Interior.SteeringWheel.gameObject;
			}
		}

		public InteriorsComposition(VehicleData vehicle, IConnectionManager connectionManager, DefaultMaterialsConfig defaultMaterialsConfig, FreeDriveProvider freeDriveProvider, CameraManager cameraManager, SoundBankLoadService soundBankService)
		{
			_vehicle = vehicle;
			_connectionManager = connectionManager;
			_defaultMaterialsConfig = defaultMaterialsConfig;
			_freeDriveProvider = freeDriveProvider;
			_cameraManager = cameraManager;
			_soundBankService = soundBankService;
		}

		private void LoadDashboardBank()
		{
			if (!_dashboardBankHeld && _soundBankService != null)
			{
				_soundBankService.TryLoadBank(SoundBankType.CarDashboard);
				_dashboardBankHeld = true;
			}
		}

		private void UnloadDashboardBank()
		{
			if (_dashboardBankHeld && _soundBankService != null)
			{
				_soundBankService.TryUnloadBank(SoundBankType.CarDashboard);
				_dashboardBankHeld = false;
			}
		}

		public void Init(Transform bodyTransform)
		{
			if (bodyTransform.TryGetComponent<CarBodyReferences>(out var component) && (bool)component.OldInterior)
			{
				InitialInterier = component.OldInterior;
				CarExtension.DisableRainOnInteriorMaterials(InitialInterier);
				if ((bool)_vehicle.CarAnimationReferences)
				{
					_vehicle.CarAnimationReferences.SetInteriorParents(_vehicle.gameObject, InitialInterier);
				}
				GlobalEventSystem.OnGraphicsChanged += OnGraphicsChanged;
				TryGetInteriorInfosConfig(string.Format("{0}{1}", "InteriorsInfo/", _vehicle.CarId)).Forget();
			}
		}

		private async UniTask TryGetInteriorInfosConfig(string key)
		{
			AsyncOperationHandle<IList<IResourceLocation>> loc = Addressables.LoadResourceLocationsAsync(key);
			await loc.Task;
			if (loc.Status != AsyncOperationStatus.Succeeded || loc.Result == null || loc.Result.Count == 0)
			{
				Addressables.Release(loc);
				return;
			}
			Addressables.Release(loc);
			AsyncOperationHandle<InteriorInfoConfig> handle = Addressables.LoadAssetAsync<InteriorInfoConfig>(key);
			await handle.Task;
			if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
			{
				Addressables.Release(handle);
				return;
			}
			if (!FlagsStorage.IsMultiFree || ((bool)_vehicle && _vehicle.gameObject == V.Car))
			{
				InteriorInfoConfig = handle.Result;
				InteriorInfoConfig.Init();
			}
			Addressables.Release(handle);
		}

		public void ShowHighInterior()
		{
			SwitchInterior(high: true);
		}

		public void ShowLowInterior()
		{
			SwitchInterior(high: false);
		}

		public void SetHandsToCurrentInteriorWheel()
		{
			if ((bool)Interior && Interior.gameObject.activeSelf && (bool)Interior.SteeringWheel)
			{
				SetHandsToHighInteriorWheel();
			}
			else
			{
				SetHandsToDefaultInteriorWheel();
			}
		}

		public void SetHandsToHighInteriorWheel()
		{
			if ((bool)_vehicle && (bool)_vehicle.Driver && (bool)Interior && (bool)Interior.SteeringWheel && _vehicle.TryGetComponent<DriverController>(out var component))
			{
				component.SetHandWheelToSteeringWheel(_vehicle.Driver);
			}
		}

		public void SetHandsToDefaultInteriorWheel()
		{
			if ((bool)_vehicle && (bool)_vehicle.Driver && _vehicle.TryGetComponent<DriverController>(out var component))
			{
				component.RestoreHandsToSeatPoints();
			}
		}

		private void SwitchInterior(bool high)
		{
			if ((bool)Interior && (bool)InitialInterier)
			{
				SetActiveInteriorParts(Interior, high);
				SetActiveInteriorParts(InitialInterier, !high);
				_vehicle.SwitchOnOffAdditionalObjects(high);
				if ((bool)_vehicle.CarAnimationReferences)
				{
					_vehicle.CarAnimationReferences.SetInteriorParents(_vehicle.gameObject, high ? Interior : InitialInterier);
				}
				if (high)
				{
					SetHandsToHighInteriorWheel();
				}
				else
				{
					SetHandsToDefaultInteriorWheel();
				}
			}
		}

		private void SetActiveInteriorParts(CarInteriorReferences interior, bool active)
		{
			if ((bool)interior)
			{
				SetActiveIfExists(interior.FrontLeftDoorInteriorPart, active);
				SetActiveIfExists(interior.FrontRightDoorInteriorPart, active);
				SetActiveIfExists(interior.BackLeftDoorInteriorPart, active);
				SetActiveIfExists(interior.BackRightDoorInteriorPart, active);
				SetActiveIfExists(interior.TrunkFirstDoorInteriorPart, active);
				SetActiveIfExists(interior.TrunkSecondDoorInteriorPart, active);
				interior.gameObject.SetActive(active);
			}
		}

		private static void SetActiveIfExists(Component c, bool active)
		{
			if ((bool)c)
			{
				c.gameObject.SetActive(active);
			}
		}

		public void ResetInterior()
		{
			UnloadDashboardBank();
			ShowLowInterior();
			if ((bool)Interior)
			{
				DestroyIfExists(Interior.FrontLeftDoorInteriorPart);
				DestroyIfExists(Interior.FrontRightDoorInteriorPart);
				DestroyIfExists(Interior.BackLeftDoorInteriorPart);
				DestroyIfExists(Interior.BackRightDoorInteriorPart);
				DestroyIfExists(Interior.TrunkFirstDoorInteriorPart);
				DestroyIfExists(Interior.TrunkSecondDoorInteriorPart);
				UnityEngine.Object.Destroy(Interior.gameObject);
			}
			CurrentInteriorId = 0;
			SetHandsToDefaultInteriorWheel();
		}

		private static void DestroyIfExists(Component c)
		{
			if ((bool)c)
			{
				UnityEngine.Object.Destroy(c.gameObject);
			}
		}

		private async UniTaskVoid UnloadUnused()
		{
			await UniTask.Yield();
			PlayerDatas.UnloadUnusedAssets("InteriorsComposition");
		}

		private void TryCancelLoadInterior()
		{
			SetInteriorCancellationToken?.Cancel();
			SetInteriorCancellationToken?.Dispose();
			SetInteriorCancellationToken = new CancellationTokenSource();
		}

		public void TryRestoreSavedInterior()
		{
			if ((bool)_vehicle && !_vehicle.IsBeingDestroyed && CanShowHighQualityInterior() && ((int)_connectionManager.H == 1 || (int)_connectionManager.H == 2))
			{
				int num = _vehicle.carInfo.InstalledBodyKits.InteriorId;
				if (num == 0 && CarIdStorage.LoadsBuiltInInteriorViaAddressables(_vehicle.CarId))
				{
					num = 1;
				}
				TrySetInterior(num, save: false).Forget();
			}
		}

		public void TryLoadRemoteInterior()
		{
			if ((bool)_vehicle && !_vehicle.IsBeingDestroyed && !(_vehicle.gameObject == V.Car) && _vehicle.TryGetComponent<MultiPlayerPosControl>(out var component) && component.isOnlineCar && CarIdStorage.LoadsBuiltInInteriorViaAddressables(_vehicle.CarId) && _vehicle.carInfo != null && _vehicle.carInfo.InstalledBodyKits != null)
			{
				int num = _vehicle.carInfo.InstalledBodyKits.InteriorId;
				if (num == 0)
				{
					num = 1;
				}
				TrySetInterior(num, save: false).Forget();
			}
		}

		private bool CanShowHighQualityInterior()
		{
			if (CPMGraphicsSettingsManager.Instance.AdditionalEffects != CPMGraphicsSettings.AdditionalEffects.Low)
			{
				return true;
			}
			if (ScenesHelper.IsMenuSceneActive() && (bool)_cameraManager && _cameraManager.CurrentCameraType == RacingCameraType.InCar)
			{
				return true;
			}
			if (ScenesHelper.IsFreeDriveSceneActive() && V.CurrentCamIndex == 0)
			{
				return true;
			}
			if (ScenesHelper.IsDragRacingSceneActive() && _vehicle.RacingProvider != null && (bool)_vehicle.RacingProvider.DragRacingController && (bool)_vehicle.RacingProvider.DragRacingController.dragRacingCamera && _vehicle.RacingProvider.DragRacingController.dragRacingCamera.currentCamera == DragRacingCamera.CameraType.driverView)
			{
				return true;
			}
			if (ScenesHelper.IsLapRacingSceneActive() && _vehicle.RacingProvider != null && (bool)_vehicle.RacingProvider.RacingCameraController && _vehicle.RacingProvider.RacingCameraController.currentCamera == DragRacingCamera.CameraType.driverView)
			{
				return true;
			}
			if (ScenesHelper.IsParkingLevelActive() && V.CurrentCamIndex == 0)
			{
				return true;
			}
			return false;
		}

		public async UniTask ForceSetSavedInterior(CancellationToken cancellationToken = default(CancellationToken))
		{
			if ((bool)_vehicle)
			{
				int interiorId = _vehicle.carInfo.InstalledBodyKits.InteriorId;
				int interiorId2 = ((interiorId == 0) ? 1 : interiorId);
				string name = ((interiorId2 > 1) ? $"{_vehicle.CarId}i_{interiorId2}" : $"{_vehicle.CarId}i");
				int version = InteriorVersionStorage.GetVersionOfInterior(_vehicle.CarId) + 6;
				AssetBundleManager assetBundleManager = V.CoreGameManager?.AssetBundleManager;
				if (assetBundleManager != null && !assetBundleManager.IsCachedAssetBundle(name, version) && !CarIdStorage.LoadsBuiltInInteriorViaAddressables(_vehicle.CarId, interiorId2))
				{
					await assetBundleManager.DownloadInteriorAssetBundle(name, version, cancellationToken);
				}
				if ((bool)_vehicle)
				{
					TrySetInterior(interiorId2, save: false).Forget();
				}
			}
		}

		public async UniTask<bool> TrySetInterior(int interior, bool save)
		{
			if (interior == 0 || !_vehicle || _vehicle.IsBeingDestroyed || InteriorLoading)
			{
				return false;
			}
			if ((bool)Interior && CurrentInteriorId == interior)
			{
				if (!Interior.gameObject.activeSelf)
				{
					ShowHighInterior();
				}
				return true;
			}
			InteriorLoading = true;
			try
			{
				TryCancelLoadInterior();
				CancellationTokenSource cts = SetInteriorCancellationToken;
				string name = ((interior > 1) ? $"{_vehicle.CarId}i_{interior}" : $"{_vehicle.CarId}i");
				bool viaAddressables = CarIdStorage.LoadsBuiltInInteriorViaAddressables(_vehicle.CarId, interior);
				GameObject gameObject = ((!viaAddressables) ? (await LoadInteriorPrefabViaAssetBundle(name, cts.Token)) : (await LoadInteriorPrefabViaAddressables(name, cts.Token)));
				GameObject gameObject2 = gameObject;
				if (cts.IsCancellationRequested || !gameObject2)
				{
					return false;
				}
				if (!_vehicle.CarBodyReferences || !_vehicle.CarBodyReferences.Body)
				{
					return false;
				}
				await SpawnLoadedInterior(gameObject2, cts.Token);
				if (cts.IsCancellationRequested)
				{
					return false;
				}
				if (!Interior)
				{
					if (!viaAddressables)
					{
						int version = InteriorVersionStorage.GetVersionOfInterior(_vehicle.CarId) + 6;
						await V.CoreGameManager.AssetBundleManager.RemoveCachedAssetBundle(name, version);
					}
					return false;
				}
				CurrentInteriorId = interior;
				if (save)
				{
					_vehicle.carInfo.InstalledBodyKits.InteriorId = interior;
					_vehicle.carInfo.Save();
				}
				return true;
			}
			finally
			{
				InteriorLoading = false;
			}
		}

		private async UniTask<GameObject> LoadInteriorPrefabViaAddressables(string key, CancellationToken cancellationToken)
		{
			AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key).BindTo(_vehicle.gameObject);
			await handle.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (cancellationToken.IsCancellationRequested)
			{
				return null;
			}
			if (handle.Status != AsyncOperationStatus.Succeeded || handle.Result == null)
			{
				Debug.LogWarning($"[InteriorsComposition] Failed to load interior '{key}' for car {_vehicle.CarId} via Addressables.");
				return null;
			}
			return handle.Result;
		}

		private async UniTask<GameObject> LoadInteriorPrefabViaAssetBundle(string name, CancellationToken cancellationToken)
		{
			int version = InteriorVersionStorage.GetVersionOfInterior(_vehicle.CarId) + 6;
			AssetBundle assetBundle = await V.CoreGameManager.AssetBundleManager.GetInteriorAssetBundle(name, version, cancellationToken);
			if (cancellationToken.IsCancellationRequested || !assetBundle)
			{
				return null;
			}
			AssetBundleRequest req = assetBundle.LoadAssetAsync<GameObject>(name);
			await req.ToUniTask(null, PlayerLoopTiming.Update, cancellationToken).SuppressCancellationThrow();
			if (cancellationToken.IsCancellationRequested)
			{
				return null;
			}
			GameObject gameObject = (req.asset as GameObject) ?? assetBundle.LoadAsset<GameObject>($"i{_vehicle.CarId}");
			if (!gameObject || !gameObject.GetComponent<CarInteriorReferences>())
			{
				V.CoreGameManager.AssetBundleManager.RemoveLoadedCorruptedAssetBundle(name, version);
				return null;
			}
			return gameObject;
		}

		private async UniTask SpawnLoadedInterior(GameObject objBundle, CancellationToken cancellationToken)
		{
			ClusterOsd clusterOsd = _vehicle.GetComponent<ClusterOsd>();
			await UniTask.Yield(PlayerLoopTiming.LastPostLateUpdate, cancellationToken).SuppressCancellationThrow();
			if (cancellationToken.IsCancellationRequested || !_vehicle || _vehicle.IsBeingDestroyed || !_vehicle.CarBodyReferences || !_vehicle.CarBodyReferences.Body)
			{
				return;
			}
			GameObject body = _vehicle.CarBodyReferences.Body;
			ResetInterior();
			CarInteriorReferences carInteriorReferences = SpawnInterior(objBundle, body);
			if ((bool)carInteriorReferences)
			{
				Interior = carInteriorReferences;
				CarExtension.UpdateInteriorMaterialSmooth0(carInteriorReferences);
				SetupSteeringWheel(carInteriorReferences);
				SetupClusterOsd(carInteriorReferences, clusterOsd);
				SetupInteriorMirror(carInteriorReferences);
				SetupRetractableDisplay(carInteriorReferences);
				TryAddMiniMapLaptop(carInteriorReferences);
				TrySetupTachometerHolder(carInteriorReferences);
				TrySetupSpeedometerController(carInteriorReferences);
				carInteriorReferences.transform.parent = _vehicle.CarBodyReferences.Body.transform;
				SetActiveInteriorParts(InitialInterier, active: false);
				TrySetMainMenuLayer(carInteriorReferences);
				if (!CanShowHighQualityInterior())
				{
					ShowLowInterior();
				}
				if ((bool)carInteriorReferences && (bool)carInteriorReferences.TachometerHolderGradient)
				{
					carInteriorReferences.TachometerHolderGradient.TryInit(carInteriorReferences, _vehicle.GetPowertrain);
				}
				if ((bool)clusterOsd)
				{
					clusterOsd.FixDigitalTextMat();
				}
				if ((bool)_vehicle.CarAnimationReferences)
				{
					_vehicle.CarAnimationReferences.SetInteriorParents(_vehicle.gameObject, carInteriorReferences);
				}
			}
		}

		private void TrySetupTachometerHolder(CarInteriorReferences interior)
		{
			if ((bool)interior && (bool)interior.TachometerHolder && (bool)_vehicle)
			{
				interior.TachometerHolder.Init(_vehicle.GetPowertrain, _vehicle);
			}
		}

		private void TrySetupSpeedometerController(CarInteriorReferences interior)
		{
			if ((bool)interior && (bool)interior.SpeedometerController && (bool)_vehicle)
			{
				interior.SpeedometerController.Init(_vehicle.GetPowertrain);
			}
		}

		private void SetupSteeringWheel(CarInteriorReferences interior)
		{
			if ((bool)interior && (bool)interior.SteeringWheel && interior.SteeringWheel.TryGetComponent<SteeringWheel>(out var component))
			{
				component.Init(_vehicle);
				component.TryInitPanelStabilization(interior.Gimbal);
				if (_vehicle.TryGetComponent<DriverController>(out var component2))
				{
					component2.SetHandWheelToSteeringWheel(_vehicle.Driver);
				}
			}
		}

		private void SetupClusterOsd(CarInteriorReferences interior, ClusterOsd clusterOsd)
		{
			if ((bool)clusterOsd)
			{
				if ((bool)interior.DigitalSPEEDO)
				{
					clusterOsd.TextMeshSpeedo = interior.DigitalSPEEDO.GetComponent<TextMesh>();
				}
				if ((bool)interior.DigitalTACHO)
				{
					clusterOsd.TextMeshTachometer = interior.DigitalTACHO.GetComponent<TextMesh>();
				}
				if ((bool)interior.DigitalGEAR)
				{
					clusterOsd.SetDigitalGear(interior.DigitalGEAR.GetComponent<TextMesh>());
				}
				if ((bool)interior.Tacho)
				{
					clusterOsd.SetTachometerNeedle(interior.Tacho.gameObject);
				}
				if ((bool)interior.Speedo)
				{
					clusterOsd.SetSpeedometerNeedle(interior.Speedo.gameObject);
				}
			}
		}

		private void SetupInteriorMirror(CarInteriorReferences interior)
		{
			if ((bool)interior && (bool)interior.Mmirror && !(_vehicle.gameObject != V.Car))
			{
				if ((bool)_vehicle.CarMirrorController.mMirror)
				{
					UnityEngine.Object.Destroy(_vehicle.CarMirrorController.mMirror);
				}
				_vehicle.CarMirrorController.mMirror = interior.Mmirror.gameObject;
				if (interior.Mmirror.TryGetComponent<MeshRenderer>(out var component))
				{
					component.material.shader = Shader.Find("Universal Render Pipeline/Unlit");
				}
				_vehicle.CarMirrorController.SetMirror(FlagsStorage.ShowCarMirror);
			}
		}

		private void TrySetMainMenuLayer(CarInteriorReferences interior)
		{
			if (FlagsStorage.IsMainMenu && (bool)interior)
			{
				SetLayerRecursively(interior.transform, LayerViaName.CarBody);
			}
		}

		private void SetLayerRecursively(Transform root, int layer)
		{
			root.gameObject.layer = layer;
			foreach (Transform item in root)
			{
				SetLayerRecursively(item, layer);
			}
		}

		private CarInteriorReferences SpawnInterior(UnityEngine.Object objBundle, GameObject body)
		{
			Shader urpShader = GlobalEventSystem.RequireShader("CPM/URP/LitLite");
			Shader autoSimple = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveSimple");
			Shader autoLights = GlobalEventSystem.RequireShader("CPM/URP/Automotive/AutomotiveLights");
			GameObject gameObject = UnityEngine.Object.Instantiate(objBundle, body.transform, instantiateInWorldSpace: true) as GameObject;
			if (!gameObject)
			{
				return null;
			}
			CarInteriorReferences component = gameObject.GetComponent<CarInteriorReferences>();
			if (!component)
			{
				UnityEngine.Object.Destroy(gameObject);
				return null;
			}
			gameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
			Dictionary<Material, Material> dictionary = new Dictionary<Material, Material>();
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(includeInactive: true);
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.gameObject.layer = LayerViaName.CarBody;
				Material[] sharedMaterials = renderer.sharedMaterials;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					if (sharedMaterials[j] == null)
					{
						Debug.LogWarning($"[InteriorsComposition] Empty material slot {j} on '{renderer.name}' (carId={_vehicle.CarId}). Fix the interior prefab.", renderer);
						continue;
					}
					if (!dictionary.TryGetValue(sharedMaterials[j], out var value))
					{
						value = RemapMaterial(sharedMaterials[j], urpShader, autoSimple, autoLights);
						if (value != sharedMaterials[j])
						{
							if (value.HasProperty(RainPowerProperty))
							{
								value.SetFloat(RainPowerProperty, 0f);
							}
							if (value.HasProperty(StencilRefProperty))
							{
								value.SetInt(StencilRefProperty, 1);
							}
						}
						dictionary.Add(sharedMaterials[j], value);
					}
					sharedMaterials[j] = value;
				}
				renderer.sharedMaterials = sharedMaterials;
			}
			return component;
		}

		private Material RemapMaterial(Material src, Shader urpShader, Shader autoSimple, Shader autoLights)
		{
			if (src.shader.name == "Transparent/VertexLit")
			{
				Material material = new Material(_defaultMaterialsConfig.SimpleGlass);
				material.name = src.name + "_ResetSmooth";
				material.SetTexture("_BaseMap", src.mainTexture);
				if (src.HasProperty("_Color") || src.HasProperty("_BaseColor"))
				{
					material.color = src.color;
				}
				return material;
			}
			if (src.shader.name == "Mobile/Bumped Diffuse")
			{
				Material material2 = new Material(urpShader);
				material2.name = src.name + "_ResetSmooth";
				material2.SetTexture("_BaseMap", src.mainTexture);
				material2.color = Color.white;
				return material2;
			}
			if (src.shader.name == "CPM/URP/Automotive/AutomotiveSimple")
			{
				return new Material(src)
				{
					shader = autoSimple
				};
			}
			if (src.shader.name == "CPM/URP/Automotive/AutomotiveLights")
			{
				return new Material(src)
				{
					shader = autoLights
				};
			}
			if (src.shader.name != urpShader.name)
			{
				Material material3 = new Material(urpShader);
				material3.name = src.name + "_ResetSmooth";
				material3.SetTexture("_BaseMap", src.mainTexture);
				if (src.HasProperty("_Color") || src.HasProperty("_BaseColor"))
				{
					material3.color = src.color;
				}
				return material3;
			}
			return src;
		}

		private void SetupRetractableDisplay(CarInteriorReferences interior)
		{
			try
			{
				if ((bool)interior.RetractableDisplay)
				{
					CarRetractableDisplay carRetractableDisplay = interior.RetractableDisplay.gameObject.AddComponent<CarRetractableDisplay>();
					_vehicle.BodyLoadingComposition.CarRetractableDisplay = carRetractableDisplay;
					LoadDashboardBank();
					if ((bool)_vehicle.BodyLoadingComposition.CarDisplayIndicators)
					{
						_vehicle.BodyLoadingComposition.CarDisplayIndicators.transform.SetParent(_vehicle.BodyLoadingComposition.CarRetractableDisplay.transform);
					}
					if (carRetractableDisplay.TryGetComponent<Animator>(out var component))
					{
						_vehicle.BodyLoadingComposition.CarRetractableDisplay.DisplayAnimator = component;
						_vehicle.BodyLoadingComposition.CarRetractableDisplay.ShowHideDisplay(isActive: true);
						_vehicle.BodyLoadingComposition.CarRetractableDisplay.SwitchOnOffCarDisplayAnimation(isActive: true);
					}
				}
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		private void TryAddMiniMapLaptop(CarInteriorReferences interior)
		{
			if ((bool)_vehicle && (bool)interior && (bool)interior.MiniMap)
			{
				_vehicle.ExtraModuleController.RemoveExtraModule<ExtraModuleMiniMapLaptop>();
				_vehicle.ExtraModuleController.AddExtraModule(new ExtraModuleMiniMapLaptop(interior, _vehicle, _freeDriveProvider));
			}
		}

		private void OnGraphicsChanged(CPMGraphicsSettings.AdditionalEffects additionalEffects)
		{
			if (!CanShowHighQualityInterior())
			{
				ShowLowInterior();
			}
			else
			{
				if (!_vehicle || _vehicle.gameObject != V.Car)
				{
					return;
				}
				if ((bool)Interior)
				{
					ShowHighInterior();
					return;
				}
				int valueOrDefault = (_vehicle.carInfo?.InstalledBodyKits?.InteriorId).GetValueOrDefault();
				if (valueOrDefault > 0)
				{
					TrySetInterior(valueOrDefault, save: false).Forget();
				}
			}
		}

		public void Dispose()
		{
			UnloadDashboardBank();
			SetInteriorCancellationToken?.Cancel();
			SetInteriorCancellationToken?.Dispose();
			SetInteriorCancellationToken = null;
			GlobalEventSystem.OnGraphicsChanged -= OnGraphicsChanged;
		}
	}
}
