using System;
using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.AsyncOperations;
using _Car_Parking;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.CommonUtility.Contracts;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class BodyLoadingComposition
	{
		private GameObject _rrLODBody;

		private GameObject _rrMainBody;

		private AsyncOperationHandle<GameObject> _asyncOperationLoadMainBodyHandle;

		private AsyncOperationHandle<GameObject> _asyncOperationLoadMainBodyOnCreateHandleBody;

		private AsyncOperationHandle<GameObject> _asyncOperationLoadLODBodyHandle;

		private CancellationTokenSource _lodLoadCts;

		private readonly VehicleData _vehicle;

		private bool _showMainModelOnly;

		private static int _lastBodyInstantiateFrame = -1000;

		private static int _activeLodLoads;

		private const int MaxConcurrentLodLoads = 2;

		private bool _lodLoadQueuedOrLoading;

		public GameObject ShadowBody { get; set; }

		public bodyLOD BodyLod { get; set; }

		public bool HasAirRoof { get; set; }

		public bool HasAirHood { get; set; }

		public GameObject LODModel { get; set; }

		public CarRetractableDisplay CarRetractableDisplay { get; set; }

		public GameObject CarDisplayIndicators { get; set; }

		public SpoilerSystem SpoilerSystem { get; set; }

		public List<bodyKitValues> DisableSpoilerKits { get; set; } = new List<bodyKitValues>();

		public BodyMarker BodyMarker { get; private set; }

		public bool IsLocalPlayerCar { get; set; }

		private bool IsOwnCar
		{
			get
			{
				if (!IsLocalPlayerCar)
				{
					return V.Car == _vehicle.gameObject;
				}
				return true;
			}
		}

		public BodyLoadingComposition(VehicleData vehicle)
		{
			_vehicle = vehicle;
			_vehicle.GetCancellationTokenOnDestroy().Register(ReleaseHandles);
		}

		private void ReleaseHandles()
		{
			ReleaseLODHandle();
			ReleaseHandle(ref _asyncOperationLoadMainBodyHandle);
			ReleaseHandle(ref _asyncOperationLoadMainBodyOnCreateHandleBody);
		}

		private void ReleaseLODHandle()
		{
			ReleaseHandle(ref _asyncOperationLoadLODBodyHandle);
		}

		private static void ReleaseHandle(ref AsyncOperationHandle<GameObject> handle)
		{
			if (handle.IsValid())
			{
				Addressables.Release(handle);
				handle = default;
			}
		}

		public void SetActiveMainModel(bool isOn)
		{
			_vehicle.MainModel.SetActive(isOn);
		}

		private async UniTask<bool> WaitForBodyInstantiateSlot()
		{
			if (!IsOwnCar)
			{
				while (Time.frameCount == _lastBodyInstantiateFrame)
				{
					await UniTask.NextFrame();
					if (!_vehicle || _vehicle.IsBeingDestroyed)
					{
						return false;
					}
				}
			}
			_lastBodyInstantiateFrame = Time.frameCount;
			return true;
		}

		public async UniTask LoadFromResource(bool immediately)
		{
			if (!immediately)
			{
				StartLODLoad().Forget();
				return;
			}
			_asyncOperationLoadMainBodyOnCreateHandleBody = Addressables.LoadAssetAsync<GameObject>($"body/b{_vehicle.CarId}.prefab");
			await _asyncOperationLoadMainBodyOnCreateHandleBody.Task;
			if (_vehicle.IsBeingDestroyed || _asyncOperationLoadMainBodyOnCreateHandleBody.Status != AsyncOperationStatus.Succeeded)
			{
				return;
			}
			GameObject prefab = _asyncOperationLoadMainBodyOnCreateHandleBody.Result;
			if (!prefab || !(await WaitForBodyInstantiateSlot()))
			{
				return;
			}
			_vehicle.MainModel = UnityEngine.Object.Instantiate(prefab, _vehicle.MainModelPosition, Quaternion.identity);
			BodyMarker = (_vehicle.MainModel ? _vehicle.MainModel.GetComponent<BodyMarker>() : null);
			if ((bool)BodyMarker)
			{
				UpdateMaterial();
				GameObject[] additionalElementsWithoutVinyls = BodyMarker.AdditionalElementsWithoutVinyls;
				foreach (GameObject gameObject in additionalElementsWithoutVinyls)
				{
					if ((bool)gameObject && gameObject.TryGetComponent<MeshRenderer>(out var component))
					{
						component.sharedMaterial = _vehicle.CarMat;
					}
				}
				additionalElementsWithoutVinyls = BodyMarker.AdditionalWindowElements;
				foreach (GameObject gameObject2 in additionalElementsWithoutVinyls)
				{
					if ((bool)gameObject2 && gameObject2.TryGetComponent<MeshRenderer>(out var component2))
					{
						component2.sharedMaterial = _vehicle.WindowComposition.WindowMat;
					}
				}
				Utils.SetLpAnchor(_vehicle.transform, _vehicle.MainModel);
			}
			_vehicle.CarAnimationReferences = _vehicle.MainModel.GetComponent<CarAnimationReferences>();
			_vehicle.CarAnimationsController = _vehicle.gameObject.GetComponent<CarAnimationsController>();
			if ((bool)_vehicle.CarAnimationReferences && (bool)_vehicle.CarAnimationsController)
			{
				_vehicle.CarAnimationsController.SetCarAnimationReference(_vehicle.CarAnimationReferences);
			}
			OnMainLoaded();
		}

		public void ShowMainModelOnly()
		{
			_showMainModelOnly = true;
			if ((bool)_vehicle.MainModel)
			{
				SetActiveMainModel(isOn: true);
			}
			if ((bool)LODModel)
			{
				LODModel.SetActive(value: false);
			}
			SetLodWheel(showLODWheel: false);
		}

		public void ShowLod(bool val)
		{
			_showMainModelOnly = !val;
			if ((bool)_vehicle.MainModel)
			{
				SetActiveMainModel(!val);
			}
			if ((bool)LODModel)
			{
				LODModel.SetActive(val);
			}
		}

		public void SetLodWheel(bool showLODWheel)
		{
			if ((showLODWheel && _vehicle.racingState == RacingState.OnVisualTuning) || !_vehicle.Axles || _vehicle.Axles.AllWheels.Count <= 0)
			{
				return;
			}
			foreach (WheelProvider allWheel in _vehicle.Axles.AllWheels)
			{
				if ((bool)allWheel.WheelModel)
				{
					allWheel.WheelModel.ShowLod(showLODWheel);
				}
			}
		}

		public async UniTask StartLODLoad(bool immediately = false, bool showLod = true)
		{
			if ((bool)_rrLODBody || _vehicle.IsBeingDestroyed)
			{
				return;
			}
			if (_lodLoadCts == null)
			{
				_lodLoadCts = CancellationTokenSource.CreateLinkedTokenSource(_vehicle.GetCancellationTokenOnDestroy());
			}
			CancellationToken ct = _lodLoadCts.Token;
			bool queued = !immediately && !_vehicle.IsRacingCar;
			if (queued)
			{
				if (_lodLoadQueuedOrLoading)
				{
					return;
				}
				_lodLoadQueuedOrLoading = true;
			}
			try
			{
				if (queued)
				{
					while (_activeLodLoads >= 2)
					{
						await UniTask.Yield();
						if (!_vehicle || _vehicle.IsBeingDestroyed || ct.IsCancellationRequested || (bool)_rrLODBody)
						{
							return;
						}
					}
				}
				_activeLodLoads++;
				try
				{
					ReleaseLODHandle();
					_asyncOperationLoadLODBodyHandle = Addressables.LoadAssetAsync<GameObject>($"body/_LOD/b{_vehicle.CarId}_LOD.prefab");
					try
					{
						await _asyncOperationLoadLODBodyHandle.ToUniTask(null, PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
					}
					catch (Exception)
					{
						return;
					}
					if (!_vehicle || _vehicle.IsBeingDestroyed || ct.IsCancellationRequested)
					{
						return;
					}
					_rrLODBody = _asyncOperationLoadLODBodyHandle.Result;
					await WaitLODAsync(ct);
					if (!_vehicle || ct.IsCancellationRequested)
					{
						return;
					}
					if (!showLod)
					{
						ShowLod(val: false);
					}
				}
				finally
				{
					_activeLodLoads--;
				}
			}
			finally
			{
				if (queued)
				{
					_lodLoadQueuedOrLoading = false;
				}
			}
		}

		public void ReloadLOD()
		{
			CommonExtensions.CancelAndDisposeToken(ref _lodLoadCts);
			if ((bool)LODModel)
			{
				UnityEngine.Object.Destroy(LODModel);
			}
			if ((bool)ShadowBody)
			{
				UnityEngine.Object.Destroy(ShadowBody);
			}
			LODModel = null;
			ShadowBody = null;
			BodyLod = null;
			_rrLODBody = null;
			StartLODLoad(immediately: true).Forget();
		}

		public void Unload()
		{
			if ((bool)_vehicle.MainModel)
			{
				UnityEngine.Object.Destroy(_vehicle.MainModel);
				ReleaseHandles();
				_rrMainBody = null;
				if (_vehicle.TryGetComponent<CarInfoContainer>(out var component))
				{
					component.CarBodyWasLoaded = false;
				}
			}
		}

		public void UpdateMaterial()
		{
			if (!_vehicle.MainModel || !_vehicle.MainModel.TryGetComponent<BodyMarker>(out var component))
			{
				return;
			}
			if ((bool)component.frontLightRight && component.frontLightRight.TryGetComponent<MeshRenderer>(out var component2))
			{
				component2.sharedMaterial = _vehicle.CarMat;
			}
			if ((bool)component.frontLightLeft && component.frontLightLeft.TryGetComponent<MeshRenderer>(out component2))
			{
				component2.sharedMaterial = _vehicle.CarMat;
			}
			if ((bool)component.hood && component.hood.TryGetComponent<MeshRenderer>(out component2))
			{
				component2.sharedMaterial = _vehicle.CarMat;
			}
			GameObject[] additionalElements = component.AdditionalElements;
			foreach (GameObject gameObject in additionalElements)
			{
				if (!gameObject)
				{
					continue;
				}
				if (gameObject.TryGetComponent<MeshRenderer>(out var component3))
				{
					component3.sharedMaterial = _vehicle.CarMat;
				}
				if (gameObject.transform.childCount <= 0)
				{
					continue;
				}
				foreach (Transform item in gameObject.transform)
				{
					if (item.name.Contains("window") && item.TryGetComponent<MeshRenderer>(out var component4))
					{
						component4.sharedMaterial = _vehicle.WindowComposition.WindowMat;
					}
				}
			}
			if ((bool)component.spoiler)
			{
				SetSpoiler(component.spoiler);
			}
		}

		public void SetSpoiler(SpoilerSystem spoilerSystem)
		{
			bool num = SpoilerSystem != spoilerSystem;
			SpoilerSystem = spoilerSystem;
			if (spoilerSystem.TryGetComponent<MeshRenderer>(out var component))
			{
				component.material = _vehicle.CarMat;
			}
			if (num)
			{
				spoilerSystem.Init(_vehicle.Axles, _vehicle.GetPowertrain);
			}
			spoilerSystem.enabled = DisableSpoilerKits.IsNullOrEmpty();
		}

		public void AddDisableSpoilerKit(bodyKitValues kit)
		{
			if ((bool)kit)
			{
				if (!DisableSpoilerKits.Contains(kit))
				{
					DisableSpoilerKits.Add(kit);
				}
				if ((bool)SpoilerSystem)
				{
					SpoilerSystem.enabled = false;
				}
			}
		}

		public void RemoveDisableSpoilerKit(bodyKitValues kit)
		{
			if ((bool)kit && DisableSpoilerKits.Contains(kit))
			{
				DisableSpoilerKits.Remove(kit);
				if ((bool)SpoilerSystem)
				{
					SpoilerSystem.enabled = DisableSpoilerKits.IsNullOrEmpty();
				}
			}
		}

		public void SetSoundSpoiler(bool isSet)
		{
			if ((bool)SpoilerSystem)
			{
				SpoilerSystem.SetSound(isSet);
			}
		}

		public void Dispose()
		{
			ReleaseLODHandle();
			CommonExtensions.CancelAndDisposeToken(ref _lodLoadCts);
			_rrMainBody = null;
			_rrLODBody = null;
		}

		private async UniTask CheckBodyLoadAsync(CancellationToken cancellationToken)
		{
			if ((bool)_rrMainBody)
			{
				if (!_vehicle.MainModel)
				{
					if (!(await WaitForBodyInstantiateSlot()))
					{
						return;
					}
					_vehicle.MainModel = UnityEngine.Object.Instantiate(_rrMainBody);
				}
				if ((bool)_vehicle.CarMat)
				{
					UpdateMaterial();
				}
				if (!_vehicle.MainModel)
				{
					Debug.LogError("MainModel is null " + _vehicle.name, _vehicle.gameObject);
				}
				if (_vehicle.TryGetComponent<MultiPlayerPosControl>(out var multiPlayerPosControl))
				{
					await multiPlayerPosControl.ApplyDataAsync(cancellationToken);
				}
				if ((bool)_vehicle && !cancellationToken.IsCancellationRequested)
				{
					OnMainLoaded();
					if ((bool)multiPlayerPosControl)
					{
						multiPlayerPosControl.SetAllMaterialColor();
					}
					if ((bool)_vehicle.BodyKits)
					{
						_vehicle.BodyKits.enabled = true;
						LoadAllBodyTypes();
					}
				}
			}
			else
			{
				Debug.LogError("Main body is null " + _vehicle.name, _vehicle.gameObject);
			}
		}

		private async UniTask WaitLODAsync(CancellationToken ct)
		{
			if (!_rrLODBody)
			{
				return;
			}
			if (_vehicle.TryGetComponent<MultiPlayerPosControl>(out var multiPlayerPosControl) && multiPlayerPosControl.isOnlineCar)
			{
				await multiPlayerPosControl.ApplyDataAsync(ct);
			}
			if (!_vehicle || ct.IsCancellationRequested)
			{
				return;
			}
			if ((bool)_rrLODBody)
			{
				if (!LODModel)
				{
					LODModel = UnityEngine.Object.Instantiate(_rrLODBody);
				}
				OnLODLoaded();
				if ((bool)multiPlayerPosControl && _vehicle.gameObject != _vehicle.FreeDriveProvider.FirstCarMultiPlayer)
				{
					multiPlayerPosControl.SetAllMaterialColor();
				}
			}
			if (!_vehicle.IsRacingCar && (bool)multiPlayerPosControl && multiPlayerPosControl.isOnlineCar && multiPlayerPosControl.TryGetComponent<CarInfoContainer>(out var component) && component.CarInfo != null)
			{
				MultiplayerDataSending.ApplyVinyls(multiPlayerPosControl, component.VinylsData, component.CarInfo.Colors);
			}
		}

		private async UniTaskVoid StartLoadMainBody()
		{
			Debug.Log("StartLoadMainBody " + _vehicle.IsBeingDestroyed);
			if ((bool)_rrMainBody || _vehicle.IsBeingDestroyed)
			{
				return;
			}
			_asyncOperationLoadMainBodyHandle = Addressables.LoadAssetAsync<GameObject>($"body/b{_vehicle.CarId}.prefab");
			await _asyncOperationLoadMainBodyHandle.Task;
			if (_vehicle.IsBeingDestroyed)
			{
				ReleaseHandle(ref _asyncOperationLoadMainBodyHandle);
				Debug.LogError("Car is being destroyed");
				Debug.LogError("Car is being destroyed " + _vehicle.name);
				return;
			}
			_rrMainBody = _asyncOperationLoadMainBodyHandle.Result;
			await CheckBodyLoadAsync(_vehicle.GetCancellationTokenOnDestroy());
			if ((bool)_vehicle && ScenesHelper.IsParkingLevelActive())
			{
				ShowLod(val: false);
			}
		}

		private void OnMainLoaded()
		{
			_vehicle.CarBodyReferences = _vehicle.MainModel.GetComponent<CarBodyReferences>();
			_vehicle.WindowComposition?.RefreshWiperSources();
			_vehicle.ColorsService.CreateFirstMat(_vehicle);
			_vehicle.MainModel.transform.parent = _vehicle.transform;
			_vehicle.MainModel.transform.SetLocalPositionAndRotation(_vehicle.MainModelPosition, Quaternion.identity);
			InitBodyChilds(_vehicle.MainModel.transform);
			_vehicle.CarMirrorController.Init(_vehicle.MainModel.transform);
			if (V.Car == _vehicle.gameObject)
			{
				_vehicle.CarMirrorController.SetMirror(FlagsStorage.ShowCarMirror);
			}
			_vehicle.LightsComposition.Init(_vehicle.MainModel.transform);
			_vehicle.InteriorsComposition.Init(_vehicle.MainModel.transform);
			_vehicle.InteriorsComposition.TryLoadRemoteInterior();
			if ((bool)_vehicle.renderBody)
			{
				_vehicle.renderBody.material = _vehicle.CarMat;
			}
			if ((bool)_vehicle.WindowComposition.WindowMat && (bool)_vehicle.WindowComposition.WindowBody && _vehicle.WindowComposition.WindowBody.TryGetComponent<Renderer>(out var component))
			{
				component.material = _vehicle.WindowComposition.WindowMat;
			}
			if (_vehicle.TryGetComponent<VehicleDamage>(out var component2))
			{
				component2.Init();
			}
			_vehicle.IsCarGraphicLoaded = true;
			_vehicle.InvokeOnCarGraphicLoaded();
		}

		private void InitBodyChilds(Transform inspectTransform)
		{
			if (!inspectTransform.TryGetComponent<CarBodyReferences>(out var component))
			{
				return;
			}
			if ((bool)ShadowBody)
			{
				MeshRenderer[] meshRenderers = component.MeshRenderers;
				foreach (MeshRenderer meshRenderer in meshRenderers)
				{
					if ((bool)meshRenderer)
					{
						meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
					}
				}
			}
			if ((bool)component.Body)
			{
				_vehicle.renderBody = component.Body.GetComponent<MeshRenderer>();
			}
			if ((bool)component.GlassWindow)
			{
				_vehicle.WindowComposition.WindowBody = component.GlassWindow.gameObject;
			}
		}

		private void OnLODLoaded()
		{
			LODModel.transform.parent = _vehicle.transform;
			LODModel.transform.SetLocalPositionAndRotation(_vehicle.MainModelPosition, Quaternion.identity);
			if (_showMainModelOnly && (bool)_vehicle.MainModel)
			{
				SetActiveMainModel(isOn: true);
				LODModel.SetActive(value: false);
			}
			else
			{
				if ((bool)_vehicle.MainModel)
				{
					SetActiveMainModel(isOn: false);
				}
				LODModel.SetActive(value: true);
			}
			BodyLod = LODModel.GetComponent<bodyLOD>();
			if ((bool)BodyLod)
			{
				if ((bool)BodyLod.shadowBody)
				{
					ShadowBody = BodyLod.shadowBody.gameObject;
					ShadowBody.transform.parent = _vehicle.transform;
					BodyLod.shadowBody.shadowCastingMode = ShadowCastingMode.ShadowsOnly;
					if ((bool)_vehicle.MainModel)
					{
						foreach (Transform item in _vehicle.MainModel.transform)
						{
							MeshRenderer component = item.GetComponent<MeshRenderer>();
							if ((bool)component)
							{
								component.shadowCastingMode = ShadowCastingMode.Off;
							}
						}
					}
				}
				if ((bool)BodyLod.mainLODBody)
				{
					BodyLod.mainLODBody.sharedMaterial = _vehicle.CarMat;
				}
				if ((bool)BodyLod.windowLODBody)
				{
					if (_vehicle.WindowComposition.WindowMat == null)
					{
						_vehicle.WindowComposition.CreateWindowMat(BodyLod.windowLODBody.gameObject);
					}
					else
					{
						BodyLod.windowLODBody.material = _vehicle.WindowComposition.WindowMat;
					}
				}
			}
			if (_vehicle.IsRacingCar)
			{
				StartLoadMainBody();
			}
		}

		private void LoadAllBodyTypes()
		{
			if (_vehicle.BodyKits.TypesToIntall.Length == 0)
			{
				_vehicle.BodyKits.TypesToIntall = new int[8];
			}
			_vehicle.BodyKits.LoadAllKitsAsync().Forget();
			if (_vehicle.BodyKits.newExhaustFoundCount == 1)
			{
				_vehicle.ExhaustComposition.CreateStrongShotParticles();
				_vehicle.ExhaustComposition.CreateWeakShotParticles();
			}
			if (_vehicle.gameObject == _vehicle.FreeDriveProvider.FirstCarMultiPlayer && _vehicle.TryGetComponent<VehicleDamage>(out var component))
			{
				component.meshFilters = Array.Empty<MeshFilter>();
			}
		}
	}
}
