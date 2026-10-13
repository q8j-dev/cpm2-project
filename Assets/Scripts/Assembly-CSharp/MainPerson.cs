using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using MTAssets.SkinnedMeshCombiner;
using Photon.Pun;
using Racing.CameraManagement;
using RootMotion.FinalIK;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

public class MainPerson : MonoBehaviour
{
	public delegate void SetClientAnimDelegate(GameObject gameObject);

	[NonSerialized]
	public bool IsDragRacingCrowd;

	public SkinnedMeshRenderer SkinnedMeshRender;

	public ObscuredInt Gender;

	public GameObject lastCar;

	public Transform middle;

	public Transform head;

	public Transform foot;

	public GameObject[] allKits;

	public Material mainMat;

	public bool EditPerson;

	[NonSerialized]
	public bool HideSkinUntilSeated;

	public bool IsRacingPerson;

	[HideInInspector]
	public bool IsMCUnitySkin;

	public FullBodyBipedIK ik;

	public PersonAnimationsController PersonAnimationsController;

	public Animator Animator;

	public GameObject skin;

	public SkinnedMeshRenderer mainBodySkin;

	public SkinnedMeshRenderer CombinedBodySkin;

	public bool canHideBody;

	public SkinnedMeshCombiner skinnedMeshCombiner;

	public bool randomSkin;

	public bool LoadSkinWhenClose;

	public Equipment.Role equipmentRole;

	private CharactersService _charactersService;

	private IForcedOutfitProvider _forcedOutfit;

	public bool racingPerson;

	private PersonKitConfig _malePersonKitConfig;

	private PersonKitConfig _femalePersonKitConfig;

	public bool DestroyMat;

	public List<int> HairsToIgnore = new List<int> { 6, 10, 17 };

	[HideInInspector]
	public bool HairHiddenByHat;

	private CancellationTokenSource _cancellationTokenSourceTrySetToSeatPersonPoints;

	private AnimationShop _animationShop;

	private OptionalDependency<AnimationShop> _animationShopDep;

	private FreeDriveProvider _freeDriveProvider;

	private IDisposable _setMultiplayerEquipmentDisposable;

	private IDisposable _loadMultiplayerMaskDisposable;

	private IDisposable _clearUnUsedMeshDisposable;

	private IDisposable _checkDistanceCamTick;

	private bool _headHiddenBeforePhotoMode;

	private bool _photoModeHeadForced;

	private CameraManager _cameraManager;

	private OptionalDependency<CameraManager> _cameraManagerDep;

	private Transform _headCheckCameraTransform;

	private CarCameras _headCheckCarCameras;

	public SwitchHeadController SwitchHeadController { get; set; }

	public int EquipmentIndexToLoadInMultiplayer { get; set; }

	public bool EquipmentLoaded { get; private set; }

	private event SetClientAnimDelegate OnSetClientAnimIn;

	public event SetClientAnimDelegate OnSetClientAnim
	{
		add
		{
			OnSetClientAnimIn = null;
			OnSetClientAnimIn += value;
		}
		remove
		{
			OnSetClientAnimIn -= value;
		}
	}

	[Inject]
	public void Construct(CharactersService charactersService, FreeDriveProvider freeDriveProvider, PersonKitConfigs personKitConfigs, OptionalDependency<AnimationShop> animationShop, OptionalDependency<CameraManager> cameraManager, IForcedOutfitProvider forcedOutfit)
	{
		_freeDriveProvider = freeDriveProvider;
		_animationShopDep = animationShop;
		_charactersService = charactersService;
		_malePersonKitConfig = personKitConfigs.Male;
		_femalePersonKitConfig = personKitConfigs.Female;
		_cameraManagerDep = cameraManager;
		_forcedOutfit = forcedOutfit;
	}

	private void Awake()
	{
		_animationShop = _animationShopDep?.Value;
		_cameraManager = _cameraManagerDep?.Value;
		allKits = new GameObject[11];
		HairHiddenByHat = false;
		InitPushUpCount();
		if ((bool)V.CoreGameManager)
		{
			TrySwitchVisibleHeadPerson(V.CoreGameManager.transform);
		}
		Observable.Interval(TimeSpan.FromSeconds(2.0)).Subscribe(SweepDestroyedKits).AddTo(this);
	}

	private void SweepDestroyedKits(long _)
	{
		if (allKits != null)
		{
			for (int i = 0; i < allKits.Length; i++)
			{
				CommonExtensions.NullIfDestroyed(ref allKits[i]);
			}
		}
	}

	private void OnDestroy()
	{
		if (DestroyMat)
		{
			CommonExtensions.DestroyMat(ref mainMat);
		}
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceTrySetToSeatPersonPoints);
		if ((bool)ik)
		{
			ik.solver.OnPostUpdate = null;
		}
		if ((bool)middle && !middle.parent)
		{
			UnityEngine.Object.Destroy(middle.gameObject);
		}
		if (allKits != null)
		{
			for (int i = 0; i < allKits.Length; i++)
			{
				allKits[i] = null;
			}
			allKits = null;
		}
		OnSetClientAnimIn = null;
	}

	public void ChangeToIdle()
	{
		if (PersonAnimationsController != null)
		{
			PersonAnimationsController.StopSpecialAnimation();
		}
	}

	private void InitPushUpCount()
	{
		if (!FlagsStorage.IsMainMenu && (bool)_animationShop && (bool)_animationShop.animationDataSo)
		{
			AnimationDataSO.AnimationDataItem byY = _animationShop.animationDataSo.GetByY(29);
			if (byY != null && !(byY.animationClip == null))
			{
				AnimationClip animationClip = byY.animationClip;
				animationClip.events = Array.Empty<AnimationEvent>();
				AnimationEvent animationEvent = new AnimationEvent();
				animationEvent.time = animationClip.length * 0.9f;
				animationEvent.functionName = "CountPushUp";
				animationClip.AddEvent(animationEvent);
			}
		}
	}

	private void CountPushUp()
	{
		if (!(V.DriverPerson != base.gameObject) && FlagsStorage.IsFreeDriveMode)
		{
			++_freeDriveProvider.FreeDriveDB.push_ups;
		}
	}

	public void ReSkin()
	{
		if (!SkinnedMeshRender && base.gameObject == V.DriverPerson)
		{
			skin = _charactersService.LoadPersonSkin(base.gameObject, Gender);
			SetSkinnedMeshCombine();
		}
	}

	private void Start()
	{
		if ((FlagsStorage.InGarageFree || FlagsStorage.InGarageMenu) && !base.gameObject.GetComponent<AkGameObj>())
		{
			base.gameObject.AddComponent<AkGameObj>();
		}
		if (racingPerson)
		{
			Gender = UnityEngine.Random.Range(0, 2);
			LoadRacingSkinAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}
		else if (randomSkin || IsRacingPerson)
		{
			Gender = UnityEngine.Random.Range(0, 2);
			if (!LoadSkinWhenClose)
			{
				if (skin == null)
				{
					LoadRandomSkinWarmAsync(useHelmet: false, this.GetCancellationTokenOnDestroy()).Forget();
				}
			}
			else
			{
				_checkDistanceCamTick?.Dispose();
				_checkDistanceCamTick = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612), TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					CheckDistanceCam();
				}).AddTo(this);
			}
		}
		else
		{
			LoadDriverSkinAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}
		if (FlagsStorage.IsMultiFree && (bool)V.LastUsedCar && base.gameObject.TryGetComponent<PhotonView>(out var component) && !component.IsMine && V.LastUsedCar.TryGetComponent<CarAnimationsController>(out var component2))
		{
			component2.SendDataOpenDoors(component.Owner);
		}
	}

	private void CheckDistanceCam()
	{
		if (Vector3.Distance(V.CoreGameManager.transform.position, base.transform.position) < 100f)
		{
			_checkDistanceCamTick?.Dispose();
			_checkDistanceCamTick = null;
			if (skin == null)
			{
				LoadRandomSkinWarmAsync(useHelmet: false, this.GetCancellationTokenOnDestroy()).Forget();
			}
		}
	}

	public void LoadRandomSkin(bool useHelmet = false)
	{
		skin = _charactersService.LoadPersonSkin(base.gameObject, Gender, randomSkin: true);
		LoadRandomEquipment(useHelmet);
		OnSetClientAnimIn?.Invoke(base.gameObject);
	}

	private async UniTaskVoid LoadDriverSkinAsync(CancellationToken ct)
	{
		if (base.gameObject != V.DriverPerson && !EditPerson)
		{
			return;
		}
		await _charactersService.PrefetchPersonAssetsAsync(Gender, driverOutfit: true, ct);
		if (ct.IsCancellationRequested || !this)
		{
			return;
		}
		if (!SkinnedMeshRender && base.gameObject == V.DriverPerson)
		{
			skin = _charactersService.LoadPersonSkin(base.gameObject, Gender);
			SetSkinnedMeshCombine();
		}
		if ((base.gameObject == V.DriverPerson || EditPerson) && !IsMCUnitySkin && (bool)SkinnedMeshRender)
		{
			LoadLocalEquipment();
			if (!EditPerson)
			{
				Observable.Timer(TimeSpan.FromSeconds(UnityEngine.Random.Range(3, 6))).Subscribe((long _) =>
				{
					CombineAfter();
				}).AddTo(this);
			}
		}
		if (HideSkinUntilSeated && base.gameObject == V.DriverPerson)
		{
			SmoothLoadCarInMenu.SkinPersonRendererEnabler(enable: false);
		}
	}

	private async UniTaskVoid LoadRacingSkinAsync(CancellationToken ct)
	{
		await _charactersService.PrefetchPersonAssetsAsync(Gender, driverOutfit: false, ct);
		if (!ct.IsCancellationRequested && (bool)this)
		{
			if (skin == null)
			{
				LoadRandomSkin(ScenesHelper.IsLapRacingSceneActive());
			}
			SetSkinnedMeshCombine();
			Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
			{
				CombineAfter();
			}).AddTo(this);
		}
	}

	private async UniTaskVoid LoadRandomSkinWarmAsync(bool useHelmet, CancellationToken ct)
	{
		await _charactersService.PrefetchPersonAssetsAsync(Gender, driverOutfit: false, ct);
		if (!ct.IsCancellationRequested && (bool)this && skin == null)
		{
			LoadRandomSkin(useHelmet);
		}
	}

	public void TryUseHelmet()
	{
		EquipmentsData equipmentsData = _charactersService.GetEquipmentsData(Gender).Clone();
		equipmentsData.SelectedEquipments[3] = 8;
		ApplyEquipmentsData(equipmentsData);
	}

	private void LoadLocalEquipment()
	{
		ApplyEquipmentsData(_charactersService.GetEquipmentsData(Gender));
	}

	private void ApplyEquipmentsData(EquipmentsData equipmentsData)
	{
		int num = 0;
		List<int> list = new List<int>(new int[5] { 0, 1, 5, 8, 9 });
		GameObject[] array = allKits;
		for (int num2 = 0; num2 < array.Length; num2++)
		{
			_ = array[num2];
			if (num >= equipmentsData.SelectedEquipments.Count)
			{
				break;
			}
			int num3 = equipmentsData.SelectedEquipments[num];
			if (list.Contains(num) && num3 == -2)
			{
				num3 = num switch
				{
					9 => 0, 
					1 => UnityEngine.Random.Range(0, 3), 
					_ => UnityEngine.Random.Range(0, 4), 
				};
				equipmentsData.BuyItem(num, num3);
				equipmentsData.SetSelectedItem(num, num3);
			}
			if (num3 == -2)
			{
				num3 = -1;
			}
			int num4 = num;
			if (num4 != 0)
			{
				if (num4 != 1)
				{
					if (num4 == 8 && num3 < 0)
					{
						goto IL_00cd;
					}
				}
				else if (num3 < 0)
				{
					goto IL_00cd;
				}
			}
			else if (num3 < 0 && (int)Gender == 0)
			{
				goto IL_00cd;
			}
			goto IL_00d0;
			IL_00cd:
			num3 = 0;
			goto IL_00d0;
			IL_00d0:
			if (_forcedOutfit != null && _forcedOutfit.TryGetForcedKit(Gender, num, out var kitId))
			{
				num3 = kitId;
			}
			if (num3 != -1)
			{
				_charactersService.LoadPersonEquipment(base.gameObject, num, num3);
			}
			num++;
		}
		_charactersService.Save();
		_charactersService.LoadAllMask(base.gameObject);
		EquipmentLoaded = true;
	}

	private void LoadRandomEquipment(bool useHelmet = false)
	{
		int num = 0;
		GameObject[] array = allKits;
		for (int i = 0; i < array.Length; i++)
		{
			_ = array[i];
			int num2 = UnityEngine.Random.Range(0, 3);
			if (num2 == -2)
			{
				num2 = -1;
			}
			switch (num)
			{
			case 1:
				if (num2 >= 0)
				{
					break;
				}
				goto IL_005b;
			case 0:
				if (num2 >= 0 || (int)Gender != 0)
				{
					break;
				}
				goto IL_005b;
			case 3:
				if (useHelmet)
				{
					num2 = 9;
				}
				break;
			case 7:
				{
					if (racingPerson)
					{
						num2 = -1;
					}
					break;
				}
				IL_005b:
				num2 = 0;
				break;
			}
			if (num2 != -1)
			{
				_charactersService.LoadPersonEquipment(base.gameObject, num, num2);
			}
			num++;
		}
		_charactersService.LoadAllMask(base.gameObject);
	}

	private async UniTask LoadRandomEquipmentAsync()
	{
		foreach (CollectionsExtensions.EnumerationEntry<GameObject> item in allKits.Enumerate())
		{
			PersonKitConfig personKitConfig = (((int)Gender == 0) ? _femalePersonKitConfig : _malePersonKitConfig);
			bool flag = false;
			int index = item.Index;
			int a = personKitConfig.allKits[index].Length;
			switch (index)
			{
			case 2:
				if ((int)Gender == 0)
				{
					flag = true;
				}
				break;
			case 3:
				if (UnityEngine.Random.value < 0.7f)
				{
					a = 3;
					flag = true;
				}
				break;
			case 4:
				if (UnityEngine.Random.value < 0.9f)
				{
					a = 3;
					flag = true;
				}
				break;
			case 5:
				a = 16;
				break;
			case 6:
				a = (((int)Gender == 0) ? 1 : 3);
				flag = UnityEngine.Random.value < 0.7f;
				break;
			case 7:
				a = 3;
				flag = UnityEngine.Random.value < 0.5f;
				break;
			case 8:
				a = 6;
				break;
			case 9:
				a = (((int)Gender == 0) ? 1 : 10);
				flag = UnityEngine.Random.value < 0.03f;
				break;
			case 10:
				flag = UnityEngine.Random.value < 0.7f;
				a = 10;
				break;
			}
			if (!flag)
			{
				int kitID = UnityEngine.Random.Range(0, Mathf.Min(a, personKitConfig.allKits[index].Length));
				await _charactersService.LoadPersonEquipmentAsync(base.gameObject, index, kitID);
			}
		}
		await _charactersService.LoadAllMaskAsync(base.gameObject);
	}

	public void LoadMultiPlayerPersonSkin()
	{
		MultiPersonControl component = GetComponent<MultiPersonControl>();
		Gender = component.Gender;
		skin = _charactersService.LoadPersonSkin(base.gameObject, Gender);
		SetSkinnedMeshCombine();
		EquipmentIndexToLoadInMultiplayer = 0;
		_setMultiplayerEquipmentDisposable?.Dispose();
		_setMultiplayerEquipmentDisposable = Observable.Timer(TimeSpan.FromSeconds(0.20000000298023224)).Subscribe((long _) =>
		{
			SetMultiplayerEquipmentOneByOne();
		}).AddTo(this);
		PersonAnimationsController.SetFloatOffset(UnityEngine.Random.Range(0f, 1f));
	}

	public void SetMultiplayerEquipmentOneByOne()
	{
		MultiPersonControl component = GetComponent<MultiPersonControl>();
		for (int i = EquipmentIndexToLoadInMultiplayer; i < component.equipments.Length; i++)
		{
			if (component.equipments[i] != -1)
			{
				_charactersService.LoadPersonEquipment(base.gameObject, i, component.equipments[i]);
				EquipmentIndexToLoadInMultiplayer++;
				_setMultiplayerEquipmentDisposable?.Dispose();
				_setMultiplayerEquipmentDisposable = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
				{
					SetMultiplayerEquipmentOneByOne();
				}).AddTo(this);
				return;
			}
		}
		ApplyInCarOutfitIfSeated(component);
		Observable.Timer(TimeSpan.FromSeconds(UnityEngine.Random.Range(3, 6))).Subscribe((long _) =>
		{
			CombineAfter();
		}).AddTo(this);
		_loadMultiplayerMaskDisposable?.Dispose();
		_loadMultiplayerMaskDisposable = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
		{
			LoadMultuPlayerMask();
		}).AddTo(this);
	}

	private void ApplyInCarOutfitIfSeated(MultiPersonControl multiPersonControl)
	{
		if ((bool)multiPersonControl && (bool)multiPersonControl.currentCar && multiPersonControl.currentCar.TryGetComponent<DriverController>(out var component))
		{
			component.ApplyInCarOutfit(this);
		}
	}

	private void LoadMultuPlayerMask()
	{
		_charactersService.LoadAllMask(base.gameObject);
	}

	public void SetSkinnedMeshCombine()
	{
		if ((bool)skin)
		{
			skinnedMeshCombiner = base.gameObject.AddComponent<SkinnedMeshCombiner>();
			skinnedMeshCombiner.mergeMethod = SkinnedMeshCombiner.MergeMethod.OneMeshPerMaterial;
		}
	}

	public void InitRacingDriver(DriverController driverController)
	{
		CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceTrySetToSeatPersonPoints);
		_cancellationTokenSourceTrySetToSeatPersonPoints = new CancellationTokenSource();
		InitRacingDriverAsync(driverController, _cancellationTokenSourceTrySetToSeatPersonPoints.Token).Forget();
	}

	public void CombineAfter()
	{
		if (IsMCUnitySkin)
		{
			return;
		}
		if (!base.gameObject.activeSelf)
		{
			Observable.Timer(TimeSpan.FromSeconds(UnityEngine.Random.Range(3, 6))).Subscribe((long _) =>
			{
				CombineAfter();
			}).AddTo(this);
			return;
		}
		skinnedMeshCombiner.gameObjectsToIgnore = new List<GameObject>();
		mainBodySkin = skin.transform.Find("body").gameObject.GetComponent<SkinnedMeshRenderer>();
		mainBodySkin.gameObject.layer = LayerViaName.PersonLayer;
		if ((bool)mainBodySkin)
		{
			skinnedMeshCombiner.gameObjectsToIgnore.Add(mainBodySkin.gameObject);
			Bounds localBounds = mainBodySkin.localBounds;
			localBounds.extents = Vector3.one;
			mainBodySkin.localBounds = localBounds;
		}
		if ((bool)allKits[7])
		{
			skinnedMeshCombiner.gameObjectsToIgnore.Add(allKits[7]);
		}
		if ((bool)allKits[2])
		{
			skinnedMeshCombiner.gameObjectsToIgnore.Add(allKits[2]);
		}
		if ((bool)allKits[3] && PersonElements.CapNeedHideInCar(Gender, allKits[3].GetComponent<Equipment>().id))
		{
			skinnedMeshCombiner.gameObjectsToIgnore.Add(allKits[3]);
			if (HairHiddenByHat && (bool)allKits[0])
			{
				skinnedMeshCombiner.gameObjectsToIgnore.Add(allKits[0]);
			}
		}
		if ((int)Gender == 1 && (bool)allKits[0])
		{
			int id = allKits[0].GetComponent<Equipment>().id;
			if (HairsToIgnore.Contains(id))
			{
				skinnedMeshCombiner.gameObjectsToIgnore.Add(allKits[0]);
			}
		}
		GameObject[] array = allKits;
		foreach (GameObject gameObject in array)
		{
			if (!gameObject || !(gameObject != allKits[7]) || (gameObject == allKits[3] && PersonElements.CapNeedHideInCar(Gender, allKits[3].GetComponent<Equipment>().id)) || (gameObject == allKits[0] && HairHiddenByHat))
			{
				continue;
			}
			if (gameObject == allKits[2])
			{
				bool num2 = (bool)allKits[3] && allKits[3].activeSelf && allKits[3].GetComponent<Equipment>().HideBeard;
				bool flag = (bool)allKits[4] && allKits[4].activeSelf && allKits[4].GetComponent<Equipment>().HideBeard;
				if (num2 | flag)
				{
					continue;
				}
			}
			gameObject.gameObject.SetActive(value: true);
		}
		if ((bool)skinnedMeshCombiner)
		{
			skinnedMeshCombiner.CombineMeshes();
		}
		int[] obj = new int[4] { 5, 6, 8, 9 };
		canHideBody = true;
		int[] array2 = obj;
		foreach (int num3 in array2)
		{
			GameObject gameObject2 = allKits[num3];
			if (gameObject2 == null)
			{
				canHideBody = false;
				break;
			}
			if (!gameObject2.GetComponent<Equipment>().HidesBodyCompletely)
			{
				canHideBody = false;
				break;
			}
		}
		if (!canHideBody && (bool)allKits[5])
		{
			Equipment component = allKits[5].GetComponent<Equipment>();
			if (component.HidesBodyCompletely && component.pack != PersonElements.Pack.no)
			{
				canHideBody = true;
			}
		}
		if (canHideBody && (bool)mainBodySkin)
		{
			mainBodySkin.enabled = false;
		}
		CombinedBodySkin = skinnedMeshCombiner.GetCombinedMeshSkinnedMeshRenderer();
		CombinedBodySkin.gameObject.layer = LayerMask.NameToLayer("DriverPerson");
		float num4 = (racingPerson ? 0.5f : UnityEngine.Random.Range(0.1f, 2f));
		_clearUnUsedMeshDisposable?.Dispose();
		_clearUnUsedMeshDisposable = Observable.Timer(TimeSpan.FromSeconds(num4)).Subscribe((long _) =>
		{
			clearUnUsedMesh();
		}).AddTo(this);
		MultiPersonControl component2 = GetComponent<MultiPersonControl>();
		Bounds localBounds2 = CombinedBodySkin.localBounds;
		localBounds2.extents = Vector3.one;
		localBounds2.center = Vector3.zero;
		CombinedBodySkin.localBounds = localBounds2;
		if ((bool)component2)
		{
			if ((bool)component2.currentCar)
			{
				if ((bool)mainBodySkin)
				{
					mainBodySkin.shadowCastingMode = ShadowCastingMode.Off;
				}
				if ((bool)CombinedBodySkin)
				{
					CombinedBodySkin.shadowCastingMode = ShadowCastingMode.Off;
				}
			}
			else
			{
				if ((bool)mainBodySkin)
				{
					mainBodySkin.shadowCastingMode = ShadowCastingMode.On;
				}
				if ((bool)CombinedBodySkin)
				{
					CombinedBodySkin.shadowCastingMode = ShadowCastingMode.On;
				}
			}
		}
		if (racingPerson && !IsDragRacingCrowd)
		{
			if ((bool)mainBodySkin)
			{
				mainBodySkin.shadowCastingMode = ShadowCastingMode.Off;
			}
			if ((bool)CombinedBodySkin)
			{
				CombinedBodySkin.shadowCastingMode = ShadowCastingMode.Off;
			}
		}
	}

	private void clearUnUsedMesh()
	{
		if ((bool)skinnedMeshCombiner && skinnedMeshCombiner.isMeshesCombined())
		{
			SkinnedMeshRender = skinnedMeshCombiner.GetCombinedMeshSkinnedMeshRenderer();
			if (skinnedMeshCombiner.isMeshesCombined())
			{
				GameObject[] resultMergeOriginalGameObjects = skinnedMeshCombiner.resultMergeOriginalGameObjects;
				foreach (GameObject gameObject in resultMergeOriginalGameObjects)
				{
					if ((bool)gameObject && !gameObject.activeSelf && gameObject != allKits[7])
					{
						UnityEngine.Object.Destroy(gameObject);
					}
				}
				skinnedMeshCombiner.resultMergeOriginalGameObjects = null;
				if ((bool)SkinnedMeshRender && SkinnedMeshRender.TryGetComponent<CombinedMeshesManager>(out var component))
				{
					component.ClearSourceMeshes();
				}
			}
		}
		PlayerDatas.TryUnloadResource();
	}

	private async UniTaskVoid InitRacingDriverAsync(DriverController driverController, CancellationToken cancellationToken)
	{
		while (!ik)
		{
			await UniTask.Yield();
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
		}
		if ((bool)driverController)
		{
			driverController.SetToSeatPersonPoints(base.gameObject, 0);
			ik.solver.Update();
			Animator.enabled = false;
			UnityEngine.Object.Destroy(ik);
		}
	}

	private void OnEnable()
	{
		MoveElements.GlobalPointMoved += GPSTracker.RecalcPath;
	}

	private void OnDisable()
	{
		MoveElements.GlobalPointMoved -= GPSTracker.RecalcPath;
	}

	public void SwitchHeadVisible(bool visible)
	{
		if ((bool)SwitchHeadController)
		{
			SwitchHeadController.SwitchHead(visible);
		}
	}

	public void EnterPhotoModeHeadVisible()
	{
		if ((bool)SwitchHeadController)
		{
			_headHiddenBeforePhotoMode = SwitchHeadController.IsHeadHidden;
			_photoModeHeadForced = true;
			SwitchHeadVisible(visible: true);
		}
	}

	public void RestoreHeadAfterPhotoMode()
	{
		_photoModeHeadForced = false;
		if ((bool)SwitchHeadController)
		{
			SwitchHeadVisible(!_headHiddenBeforePhotoMode);
		}
	}

	public void TrySwitchVisibleHeadPerson(Transform camera)
	{
		if ((bool)SwitchHeadController && !_photoModeHeadForced)
		{
			camera = ResolveFpvHeadCheckAnchor(camera);
			if ((bool)_cameraManager && _cameraManager.CurrentCameraType == RacingCameraType.InCar)
			{
				SwitchHeadVisible(visible: false);
			}
			else
			{
				SwitchHeadByCameraDistance(camera);
			}
		}
	}

	private void SwitchHeadByCameraDistance(Transform camera)
	{
		Vector3 position = SwitchHeadController.Head.position;
		if ((camera.position - position).sqrMagnitude > 2.25f)
		{
			SwitchHeadVisible(visible: true);
			return;
		}
		if (!V.Car || !V.Car.TryGetComponent<DriverController>(out var component))
		{
			SwitchHeadVisible(visible: true);
			return;
		}
		float sqrMagnitude = (component.camPosInitialInsideView - component.camPosDefaultInsideView).sqrMagnitude;
		SwitchHeadVisible(sqrMagnitude > 0.040000003f);
	}

	private Transform ResolveFpvHeadCheckAnchor(Transform camera)
	{
		if (!camera || !V.Car || !V.Car.TryGetComponent<DriverController>(out var component) || !component.camInsideView)
		{
			return camera;
		}
		if (_headCheckCameraTransform != camera)
		{
			_headCheckCameraTransform = camera;
			camera.TryGetComponent<CarCameras>(out _headCheckCarCameras);
		}
		Transform transform = component.camInsideView.transform;
		if (!_headCheckCarCameras || _headCheckCarCameras.mycamera != CarCameras.Cameras.FixedTo || !(_headCheckCarCameras.Target == transform))
		{
			return camera;
		}
		return transform;
	}
}
