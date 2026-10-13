using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using CarAnimationScripts;
using CarInteractionAnimationsScripts;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using _Car_Parking;
using _Car_Parking.Configs;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Car.car_animations_scripts;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;

public class BodyKits : MonoBehaviour
{
	public bool randomBodyKit;

	public int BumperCountRear;

	public int BumperCountFront;

	public int SkirtsCount;

	public int HoodCount;

	public int FenderCount;

	public Transform[] spoilers;

	public Transform[] airIntakesHood;

	public Transform[] airIntakeRoof;

	public Transform[] trims;

	public GameObject InstalledSpoiler;

	public GameObject InstalledRearBumper;

	public GameObject installedSkirts;

	public GameObject installedHood;

	public GameObject installedAirIntakeHood;

	public GameObject installedAirIntakeRoof;

	public GameObject installedFender;

	public Transform parent;

	public bool CanNaked = true;

	public int[] TypesToIntall;

	public int[] TrimsToIntall;

	public Color[] colors;

	public int newExhaustFoundCount;

	public BodyKitsTrim BodyKitsTrim;

	public PaidExteriorPart[] PaidSpoilers;

	public PaidExteriorPart[] PaidFrontBumpers;

	public PaidExteriorPart[] PaidRearBumpers;

	public PaidExteriorPart[] PaidSkirts;

	public PaidExteriorPart[] PaidHoods;

	public PaidExteriorPart[] PaidIntakeHood;

	public PaidExteriorPart[] PaidIntakeRoof;

	public PaidExteriorPart[] PaidFenders;

	public PaidExteriorPart[] PaidTrims;

	private static int delay = 8;

	private CancellationTokenSource _token;

	private VehicleData _vehicleData;

	public ReactiveProperty<bool> AllBodyKitsWasLoaded;

	private Transform _frontPlatePoint;

	private Transform _rearPlatePoint;

	private GameObject _installedFrontBumper;

	private List<BodyKitsLightRule> _lightRules;

	private LightController _lightController;

	private VinylDrawer _vinylDrawer;

	private bool _isBeingDestroyed;

	private DefaultMaterialsConfig _defaultMaterialsConfig;

	private HashSet<BodyKitType> loadedTypes;

	private LevelsProvider _levelsProvider;

	private static BodyKitsPartRulesConfig _cachedPartRulesConfig;

	private int CarId
	{
		get
		{
			if (!_vehicleData)
			{
				return 132;
			}
			return _vehicleData.CarId;
		}
	}

	public GameObject InstalledFrontBumper
	{
		get
		{
			return _installedFrontBumper;
		}
		set
		{
			_installedFrontBumper = value;
			OnInstalledBodyKitUpdated?.Invoke(this, BodyKitType.FrontBumper, _installedFrontBumper);
		}
	}

	public List<GameObject> AllInstalledBodyKits
	{
		get
		{
			List<GameObject> list = new List<GameObject>();
			if ((bool)InstalledSpoiler)
			{
				list.Add(InstalledSpoiler);
			}
			if ((bool)InstalledRearBumper)
			{
				list.Add(InstalledRearBumper);
			}
			if ((bool)installedSkirts)
			{
				list.Add(installedSkirts);
			}
			if ((bool)installedHood)
			{
				list.Add(installedHood);
			}
			if ((bool)installedAirIntakeHood)
			{
				list.Add(installedAirIntakeHood);
			}
			if ((bool)installedAirIntakeRoof)
			{
				list.Add(installedAirIntakeRoof);
			}
			if ((bool)installedFender)
			{
				list.Add(installedFender);
			}
			list.AddRange(BodyKitsTrim.GetAllTrims());
			return list;
		}
	}

	public event Action<BodyKits, BodyKitType, GameObject> OnInstalledBodyKitUpdated;

	public static string BodyKitTypeToGOName(BodyKitType type)
	{
		switch (type)
		{
		case BodyKitType.Spoiler:
			return "spoiler";
		case BodyKitType.FrontBumper:
			return "fbumper";
		case BodyKitType.RearBumper:
			return "rbumper";
		case BodyKitType.Skirt:
			return "skirt";
		case BodyKitType.Hood:
			return "hood";
		case BodyKitType.AirIntakeHood:
			return "hair";
		case BodyKitType.AirIntakeRoof:
			return "rair";
		case BodyKitType.Fender:
			return "fender";
		case BodyKitType.Trim:
			return "trim";
		default:
			Debug.LogError($"Body Kit type {type} is not supported");
			return "None";
		}
	}

	public void CollectPhysicalWiperSources(List<IPhysicalWiperSource> results)
	{
		if (results != null)
		{
			AddWiperSource(results, InstalledSpoiler);
			AddWiperSource(results, InstalledFrontBumper);
			AddWiperSource(results, InstalledRearBumper);
			AddWiperSource(results, installedSkirts);
			AddWiperSource(results, installedHood);
			AddWiperSource(results, installedAirIntakeHood);
			AddWiperSource(results, installedAirIntakeRoof);
			AddWiperSource(results, installedFender);
		}
	}

	private static void AddWiperSource(List<IPhysicalWiperSource> results, GameObject kit)
	{
		if ((bool)kit && kit.TryGetComponent<BodyKitReferences>(out var component) && component.HasPhysicalWipers)
		{
			results.Add(component);
		}
	}

	public void CollectCustomWindowKits(List<BodyKitReferences> results)
	{
		if (results != null)
		{
			AddCustomWindowKit(results, InstalledSpoiler);
			AddCustomWindowKit(results, InstalledFrontBumper);
			AddCustomWindowKit(results, InstalledRearBumper);
			AddCustomWindowKit(results, installedSkirts);
			AddCustomWindowKit(results, installedHood);
			AddCustomWindowKit(results, installedAirIntakeHood);
			AddCustomWindowKit(results, installedAirIntakeRoof);
			AddCustomWindowKit(results, installedFender);
		}
	}

	private static void AddCustomWindowKit(List<BodyKitReferences> results, GameObject kit)
	{
		if ((bool)kit && kit.TryGetComponent<BodyKitReferences>(out var component) && component.HasCustomWindow)
		{
			results.Add(component);
		}
	}

	[Inject]
	public void Construct(VinylDrawer vinylDrawer, DefaultMaterialsConfig defaultMaterialsConfig, LevelsProvider levelsProvider)
	{
		_levelsProvider = levelsProvider;
		_defaultMaterialsConfig = defaultMaterialsConfig;
		_vinylDrawer = vinylDrawer;
		BodyKitsTrim = new BodyKitsTrim(this);
	}

	public void Awake()
	{
		_token = new CancellationTokenSource();
		AllBodyKitsWasLoaded = new ReactiveProperty<bool>(initialValue: false);
		TryGetComponent<VehicleData>(out _vehicleData);
		loadedTypes = new HashSet<BodyKitType>();
		if ((object)_cachedPartRulesConfig == null)
		{
			_cachedPartRulesConfig = Resources.Load<BodyKitsPartRulesConfig>("Configs/BodyKitsPartRules");
		}
		_lightRules = _cachedPartRulesConfig.LightRules;
		_lightController = GetComponent<LightController>();
	}

	public void Start()
	{
		if (!parent)
		{
			CreateBodyKitsParent();
		}
		if ((bool)_vehicleData.MainModel)
		{
			parent.transform.parent = _vehicleData.MainModel.transform;
			parent.transform.localScale = Vector3.one;
		}
	}

	private void OnDestroy()
	{
		_token.Cancel();
		_token.Dispose();
		_isBeingDestroyed = true;
		BodyKitsTrim.Dispose();
	}

	public void LoadAllKits()
	{
		if (!parent)
		{
			CreateBodyKitsParent();
		}
		InstallHood().Forget();
		InstallRearBumper().Forget();
		LoadInstalledSpoiler().Forget();
		InstallFrontBumper().Forget();
		InstallSkirts().Forget();
		InstallRoofAirIntake().Forget();
		InstallHoodAirIntake().Forget();
		InstallFender().Forget();
		BodyKitsTrim.SetTrims((int[])TrimsToIntall.Clone());
		if ((bool)_vehicleData.MainModel)
		{
			ResetParentOfBodyKits();
		}
	}

	public async UniTask LoadAllKitsAsync()
	{
		if (!parent)
		{
			CreateBodyKitsParent();
		}
		await UniTask.WhenAll(LoadInstalledSpoiler(), InstallFrontBumper(), InstallRearBumper(), InstallSkirts(), InstallHood(), InstallRoofAirIntake(), InstallHoodAirIntake(), InstallFender());
		if (!_isBeingDestroyed)
		{
			BodyKitsTrim.SetTrims((int[])TrimsToIntall.Clone());
			if ((bool)_vehicleData.MainModel)
			{
				ResetParentOfBodyKits();
			}
			await UniTask.DelayFrame(1);
			InitPlatePoints();
		}
	}

	private void ResetParentOfBodyKits()
	{
		parent.transform.parent = _vehicleData.MainModel.transform;
		parent.transform.localPosition = Vector3.zero;
		parent.transform.localEulerAngles = Vector3.zero;
		parent.transform.localScale = Vector3.one;
	}

	private void CreateBodyKitsParent()
	{
		GameObject gameObject = new GameObject("body kits");
		parent = gameObject.transform;
		parent.transform.parent = base.transform;
		parent.SetLocalPositionAndRotation(_vehicleData.MainModelPosition, Quaternion.identity);
	}

	public void UnloadPrevKit(BodyKitType bodyKitType, bool fadeOut)
	{
		GameObject installedBodyKit = GetInstalledBodyKit(bodyKitType);
		if ((bool)installedBodyKit)
		{
			TryRevertSpoilerOwnedBy(installedBodyKit);
			if (fadeOut && installedBodyKit.TryGetComponent<bodyKitValues>(out var component))
			{
				component.StartFade();
			}
			else
			{
				UnityEngine.Object.Destroy(installedBodyKit);
			}
			SetInstalledBodyKit(bodyKitType, null);
		}
	}

	private void TryRevertSpoilerOwnedBy(GameObject prevBodyKit)
	{
		if (!_vehicleData || !prevBodyKit)
		{
			return;
		}
		SpoilerSystem spoilerSystem = _vehicleData.BodyLoadingComposition.SpoilerSystem;
		if ((bool)spoilerSystem)
		{
			SpoilerSystem componentInChildren = prevBodyKit.GetComponentInChildren<SpoilerSystem>(includeInactive: true);
			if ((bool)componentInChildren && !(componentInChildren != spoilerSystem))
			{
				SetDefaultSpoilerFromBodyMarker();
			}
		}
	}

	private void SetDefaultSpoilerFromBodyMarker()
	{
		if ((bool)_vehicleData && (bool)_vehicleData.BodyLoadingComposition.BodyMarker && (bool)_vehicleData.BodyLoadingComposition.BodyMarker.spoiler)
		{
			if (!_vehicleData.BodyLoadingComposition.BodyMarker.spoiler.gameObject.activeSelf)
			{
				_vehicleData.BodyLoadingComposition.BodyMarker.spoiler.gameObject.SetActive(value: true);
			}
			_vehicleData.BodyLoadingComposition.SetSpoiler(_vehicleData.BodyLoadingComposition.BodyMarker.spoiler);
		}
	}

	public async UniTask<GameObject> LoadInstalledSpoiler(bool fadeOut = false)
	{
		UnloadPrevKit(BodyKitType.Spoiler, fadeOut);
		if (TypesToIntall[0] == -2)
		{
			if (FlagsStorage.IsLevelScene)
			{
				List<int> list = new List<int>();
				int num = 0;
				Transform[] array = spoilers;
				for (int i = 0; i < array.Length; i++)
				{
					if ((bool)array[i])
					{
						list.Add(num);
					}
					num++;
				}
				int num2 = UnityEngine.Random.Range(0, list.Count);
				if (num2 != 0)
				{
					await LoadBody(BodyKitType.Spoiler, list[num2]);
				}
			}
		}
		else
		{
			int num3 = TypesToIntall[0];
			if (num3 != -1 && num3 != 0)
			{
				await LoadBody(BodyKitType.Spoiler, num3);
			}
		}
		BodyKitLoaded(BodyKitType.Spoiler);
		return null;
	}

	public async UniTask<GameObject> InstallHoodAirIntake(bool fadeOut = false)
	{
		UnloadPrevKit(BodyKitType.AirIntakeHood, fadeOut);
		if (airIntakesHood.Length == 0)
		{
			BodyKitLoaded(BodyKitType.AirIntakeHood);
			return null;
		}
		if (TypesToIntall[5] == -2)
		{
			if (FlagsStorage.IsLevelScene)
			{
				List<int> list = new List<int>();
				int num = 0;
				Transform[] array = airIntakesHood;
				for (int i = 0; i < array.Length; i++)
				{
					if ((bool)array[i])
					{
						list.Add(num);
					}
					num++;
				}
				int num2 = UnityEngine.Random.Range(0, list.Count);
				if (num2 != 0)
				{
					await LoadBody(BodyKitType.AirIntakeHood, list[num2]);
				}
			}
		}
		else
		{
			int num3 = TypesToIntall[5];
			if (num3 != 0 && num3 > -1)
			{
				await LoadBody(BodyKitType.AirIntakeHood, num3);
			}
		}
		BodyKitLoaded(BodyKitType.AirIntakeHood);
		return null;
	}

	public async UniTask<GameObject> InstallRoofAirIntake(bool fadeOut = false)
	{
		UnloadPrevKit(BodyKitType.AirIntakeRoof, fadeOut);
		if (airIntakeRoof.Length == 0)
		{
			BodyKitLoaded(BodyKitType.AirIntakeRoof);
			return null;
		}
		int num = TypesToIntall[6];
		if (num != 0 && num > -1)
		{
			await LoadBody(BodyKitType.AirIntakeRoof, num);
		}
		BodyKitLoaded(BodyKitType.AirIntakeRoof);
		return null;
	}

	public async UniTask<GameObject> InstallFrontBumper(bool fadeOut = false)
	{
		if (BumperCountFront == 0)
		{
			BodyKitLoaded(BodyKitType.FrontBumper);
			return null;
		}
		UnloadPrevKit(BodyKitType.FrontBumper, fadeOut);
		if (TypesToIntall[1] == -2)
		{
			if (!FlagsStorage.IsLevelScene)
			{
				await LoadBody(BodyKitType.FrontBumper, 0);
			}
			else
			{
				int num = UnityEngine.Random.Range(0, BumperCountFront);
				await LoadBody(BodyKitType.FrontBumper, num);
			}
		}
		else
		{
			int num2 = TypesToIntall[1];
			await LoadBody(BodyKitType.FrontBumper, num2);
		}
		BodyKitLoaded(BodyKitType.FrontBumper);
		return null;
	}

	public async UniTask<GameObject> InstallRearBumper(bool fadeOut = false)
	{
		if (BumperCountRear == 0)
		{
			BodyKitLoaded(BodyKitType.RearBumper);
			return null;
		}
		UnloadPrevKit(BodyKitType.RearBumper, fadeOut);
		if (TypesToIntall[2] == -2)
		{
			if (!randomBodyKit)
			{
				await LoadBody(BodyKitType.RearBumper, 0);
			}
			else
			{
				int num = UnityEngine.Random.Range(0, BumperCountRear);
				await LoadBody(BodyKitType.RearBumper, num);
			}
		}
		else
		{
			int num2 = TypesToIntall[2];
			await LoadBody(BodyKitType.RearBumper, num2);
		}
		BodyKitLoaded(BodyKitType.RearBumper);
		return null;
	}

	public async UniTask<GameObject> InstallSkirts(bool fadeOut = false)
	{
		if (SkirtsCount == 0)
		{
			BodyKitLoaded(BodyKitType.Skirt);
			return null;
		}
		UnloadPrevKit(BodyKitType.Skirt, fadeOut);
		if (TypesToIntall[3] == -2)
		{
			if (!randomBodyKit)
			{
				await LoadBody(BodyKitType.Skirt, 0);
			}
			else
			{
				int num = UnityEngine.Random.Range(0, SkirtsCount);
				await LoadBody(BodyKitType.Skirt, num);
			}
		}
		else
		{
			int num2 = TypesToIntall[3];
			if (num2 > -1)
			{
				await LoadBody(BodyKitType.Skirt, num2);
			}
		}
		BodyKitLoaded(BodyKitType.Skirt);
		return null;
	}

	public async UniTask<GameObject> InstallHood(bool fadeOut = false)
	{
		if (HoodCount == 0)
		{
			BodyKitLoaded(BodyKitType.Hood);
			return null;
		}
		UnloadPrevKit(BodyKitType.Hood, fadeOut);
		if (TypesToIntall[4] == -2)
		{
			if (!randomBodyKit)
			{
				await LoadBody(BodyKitType.Hood, 0);
			}
			else
			{
				int num = UnityEngine.Random.Range(0, HoodCount);
				await LoadBody(BodyKitType.Hood, num);
			}
		}
		else
		{
			int num2 = TypesToIntall[4];
			await LoadBody(BodyKitType.Hood, num2);
		}
		BodyKitLoaded(BodyKitType.Hood);
		return null;
	}

	public async UniTask<GameObject> InstallFender(bool fadeOut = false)
	{
		UnloadPrevKit(BodyKitType.Fender, fadeOut);
		if (FenderCount == 0 || TypesToIntall.Length <= 7)
		{
			BodyKitLoaded(BodyKitType.Fender);
			return null;
		}
		if (TypesToIntall[7] == -2)
		{
			if (!randomBodyKit)
			{
				await LoadBody(BodyKitType.Fender, 0);
			}
			else
			{
				int num = UnityEngine.Random.Range(0, FenderCount);
				await LoadBody(BodyKitType.Fender, num);
			}
		}
		else
		{
			int num2 = TypesToIntall[7];
			if (num2 > -1)
			{
				await LoadBody(BodyKitType.Fender, num2);
			}
		}
		BodyKitLoaded(BodyKitType.Fender);
		return null;
	}

	private async UniTask<GameObject> LoadBody(BodyKitType type, ObscuredInt bodyKitId)
	{
		return await LoadBodyKit(type, bodyKitId, unloadPrevKitFadeOut: false, CancellationToken.None);
	}

	public async UniTask<GameObject> LoadBodyKit(int type, int id, CancellationToken cancellationToken = default(CancellationToken))
	{
		return await LoadBodyKit((BodyKitType)type, id, unloadPrevKitFadeOut: true, cancellationToken);
	}

	private async UniTask<GameObject> LoadBodyKit(BodyKitType type, int id, bool unloadPrevKitFadeOut, CancellationToken cancellationToken)
	{
		cancellationToken = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, base.destroyCancellationToken).Token;
		try
		{
			BodyMarker bodyMarker = null;
			if ((bool)_vehicleData.MainModel)
			{
				_vehicleData.MainModel.TryGetComponent<BodyMarker>(out bodyMarker);
			}
			UnloadPrevKit(type, unloadPrevKitFadeOut);
			bodyKitValues bodyKitValues2 = null;
			GameObject bodyKitResult = null;
			Transform platePoint = null;
			switch (type)
			{
			case BodyKitType.Spoiler:
			{
				if (id <= 0)
				{
					SetDefaultSpoilerFromBodyMarker();
					return null;
				}
				Transform transformSpoiler = GetTransformByIDName(spoilers, id);
				if (!transformSpoiler)
				{
					return null;
				}
				bodyKitResult = await LoadAndInstantiateAsset($"bodyKits/spoiler{id}", cancellationToken);
				if (cancellationToken.IsCancellationRequested || _isBeingDestroyed || !bodyKitResult)
				{
					return null;
				}
				bodyKitValues2 = InitNewBodyKitValues(bodyKitResult, type, id);
				bodyKitResult.transform.SetLocalData(transformSpoiler);
				_vehicleData.DirtComposition.SpoilerMat = bodyKitValues2.MainMaterial;
				break;
			}
			case BodyKitType.FrontBumper:
			{
				id = Mathf.Clamp(id, 0, BumperCountFront - 1);
				if (id < 0)
				{
					return null;
				}
				bodyKitResult = await LoadAndInstantiateAsset($"bodyKits/kit{CarId}/fbumper{id}", cancellationToken);
				if (cancellationToken.IsCancellationRequested || _isBeingDestroyed || !bodyKitResult)
				{
					return null;
				}
				bodyKitResult.transform.parent = parent;
				bodyKitValues2 = InitNewBodyKitValues(bodyKitResult, type, id);
				BodyKitReferences bodyKitReferences = bodyKitResult.GetComponent<BodyKitReferences>();
				bodyKitResult.transform.SetLocalData(bodyKitValues2.localPosition, Quaternion.Euler(bodyKitValues2.localEuler), Vector3.one);
				SetForVinylPlace(bodyKitResult);
				await UniTask.DelayFrame(1);
				if (cancellationToken.IsCancellationRequested)
				{
					UnityEngine.Object.Destroy(bodyKitResult);
					return null;
				}
				if ((bool)bodyKitReferences)
				{
					platePoint = bodyKitReferences.PlatePos;
				}
				break;
			}
			case BodyKitType.RearBumper:
			{
				id = ((id == 667) ? BumperCountRear : Mathf.Clamp(id, 0, BumperCountRear - 1));
				if (id < 0)
				{
					return null;
				}
				bodyKitResult = await LoadAndInstantiateAsset($"bodyKits/kit{CarId}/rbumper{id}", cancellationToken);
				if (cancellationToken.IsCancellationRequested || _isBeingDestroyed || !bodyKitResult)
				{
					return null;
				}
				bodyKitResult.transform.parent = parent;
				bodyKitValues2 = InitNewBodyKitValues(bodyKitResult, type, id);
				BodyKitReferences bodyKitReferences = bodyKitResult.GetComponent<BodyKitReferences>();
				bodyKitResult.transform.SetLocalData(bodyKitValues2.localPosition, Quaternion.Euler(bodyKitValues2.localEuler), Vector3.one);
				SearchForExhausts(bodyKitReferences);
				SetForVinylPlace(bodyKitResult);
				await UniTask.DelayFrame(1);
				if (cancellationToken.IsCancellationRequested || _isBeingDestroyed || !bodyKitResult)
				{
					UnityEngine.Object.Destroy(bodyKitResult);
					return null;
				}
				if ((bool)bodyKitReferences)
				{
					platePoint = bodyKitReferences.PlatePos;
				}
				if ((bool)_vehicleData.CarAnimationReferences)
				{
					_vehicleData.CarAnimationReferences.SetParentBodyKit(bodyKitValues2, BodyKitType.Skirt);
				}
				break;
			}
			case BodyKitType.Skirt:
			{
				id = Mathf.Clamp(id, 0, SkirtsCount - 1);
				if (id < 0)
				{
					return null;
				}
				bodyKitResult = await LoadAndInstantiateAsset($"bodyKits/kit{CarId}/skirts{id}", cancellationToken);
				if (cancellationToken.IsCancellationRequested || _isBeingDestroyed || !bodyKitResult)
				{
					return null;
				}
				bodyKitResult.transform.parent = parent;
				bodyKitValues2 = InitNewBodyKitValues(bodyKitResult, type, id);
				BodyKitReferences bodyKitReferences = bodyKitResult.GetComponent<BodyKitReferences>();
				bodyKitResult.transform.SetLocalData(bodyKitValues2.localPosition, Quaternion.Euler(bodyKitValues2.localEuler), Vector3.one);
				SearchForExhausts(bodyKitReferences);
				SetForVinylPlace(bodyKitResult);
				break;
			}
			case BodyKitType.Hood:
				id = Mathf.Clamp(id, 0, HoodCount - 1);
				if (id < 0)
				{
					return null;
				}
				bodyKitResult = await LoadAndInstantiateAsset($"bodyKits/kit{CarId}/hood{id}", cancellationToken);
				if (cancellationToken.IsCancellationRequested || _isBeingDestroyed || !bodyKitResult)
				{
					return null;
				}
				bodyKitResult.transform.parent = parent;
				bodyKitValues2 = InitNewBodyKitValues(bodyKitResult, type, id);
				if ((bool)bodyKitValues2.animator && (bool)bodyMarker)
				{
					bodyMarker.hood = bodyKitResult;
					bodyMarker.SetHoodAnimator();
				}
				bodyKitResult.transform.SetLocalData(bodyKitValues2.localPosition, Quaternion.Euler(bodyKitValues2.localEuler), Vector3.one);
				SetForVinylPlace(bodyKitResult);
				break;
			case BodyKitType.AirIntakeHood:
			{
				if (id <= 0)
				{
					return null;
				}
				Transform transformAirHood = GetTransformByIDName(airIntakesHood, id);
				if (!transformAirHood)
				{
					return null;
				}
				bodyKitResult = await LoadAndInstantiateAsset($"bodyKits/airintake{id}", cancellationToken);
				if (cancellationToken.IsCancellationRequested || _isBeingDestroyed || !bodyKitResult)
				{
					if ((bool)_vehicleData)
					{
						_vehicleData.BodyLoadingComposition.HasAirHood = false;
					}
					return null;
				}
				bodyKitResult.transform.parent = parent;
				bodyKitValues2 = InitNewBodyKitValues(bodyKitResult, type, id);
				bodyKitResult.transform.SetLocalData(transformAirHood.localPosition, transformAirHood.localRotation, transformAirHood.localScale);
				_vehicleData.BodyLoadingComposition.HasAirHood = true;
				SetForVinylPlace(bodyKitResult);
				_vehicleData.DirtComposition.AirIntakeHoodMat = bodyKitValues2.MainMaterial;
				break;
			}
			case BodyKitType.AirIntakeRoof:
			{
				if (id <= 0)
				{
					_vehicleData.BodyLoadingComposition.HasAirRoof = false;
					return null;
				}
				Transform transformAirRoof = GetTransformByIDName(airIntakeRoof, id);
				if (!transformAirRoof)
				{
					return null;
				}
				bodyKitResult = await LoadAndInstantiateAsset($"bodyKits/airintake{id}", cancellationToken);
				if (cancellationToken.IsCancellationRequested || _isBeingDestroyed || !bodyKitResult)
				{
					return null;
				}
				bodyKitResult.transform.parent = parent;
				bodyKitValues2 = InitNewBodyKitValues(bodyKitResult, type, id);
				bodyKitResult.transform.SetLocalData(transformAirRoof.localPosition, transformAirRoof.localRotation, transformAirRoof.localScale);
				_vehicleData.BodyLoadingComposition.HasAirRoof = true;
				SetForVinylPlace(bodyKitResult);
				_vehicleData.DirtComposition.AirIntakeRoofMat = bodyKitValues2.MainMaterial;
				break;
			}
			case BodyKitType.Fender:
				id = Mathf.Clamp(id, 0, FenderCount - 1);
				if (id < 0)
				{
					return null;
				}
				bodyKitResult = await LoadAndInstantiateAsset($"bodyKits/kit{CarId}/fender{id}", cancellationToken);
				if (cancellationToken.IsCancellationRequested || _isBeingDestroyed || !bodyKitResult)
				{
					return null;
				}
				bodyKitResult.transform.parent = parent;
				bodyKitValues2 = InitNewBodyKitValues(bodyKitResult, type, id);
				bodyKitResult.transform.SetLocalData(bodyKitValues2.localPosition, Quaternion.Euler(bodyKitValues2.localEuler), Vector3.one);
				SetForVinylPlace(bodyKitResult);
				break;
			}
			SetInstalledBodyKit(type, bodyKitResult);
			if ((bool)_vehicleData.CarAnimationReferences)
			{
				if ((bool)bodyKitResult && bodyKitResult.TryGetComponent<BodyKitSkirtsAnimators>(out var component))
				{
					component.Init(_vehicleData.CarAnimationReferences);
				}
				if ((uint)(type - 2) <= 1u || type == BodyKitType.Fender)
				{
					TryInitTrunk(bodyKitValues2);
					if (bodyKitResult.TryGetComponent<CarWithoutDoorsBodyKit>(out var component2))
					{
						component2.Init(_vehicleData.CarAnimationReferences);
						_vehicleData.CarAnimationReferences.CarWithoutDoorsBodyKit = component2;
					}
				}
				if ((bool)bodyKitValues2)
				{
					_vehicleData.CarAnimationReferences.SetParentBodyKit(bodyKitValues2, type);
					if (bodyKitValues2.CarDoorType != CarDoorType.None)
					{
						_vehicleData.CarAnimationReferences.CarDoorAnimators[bodyKitValues2.CarDoorType] = bodyKitValues2.animator;
					}
					if (bodyKitValues2.CarDoorType == CarDoorType.Roof)
					{
						List<CarPartAnimationSequence> openSequence = null;
						List<CarPartAnimationSequence> closeSequence = null;
						CarAnimationReferences roofReferences = _vehicleData.CarAnimationReferences;
						if (bodyKitValues2.TryGetComponent<SetupCarPartAnimationBodyKitValues>(out var component3))
						{
							openSequence = component3.OpenSequence;
							closeSequence = component3.CloseSequence;
							bodyKitValues2.OnBodyKitDestroed += () =>
							{
								if ((bool)roofReferences)
								{
									roofReferences.SwitchRoofAnimation(enable: false);
								}
							};
						}
						HoldRoofBanksForKit(bodyKitValues2);
						roofReferences.SetupRoof(openSequence, closeSequence);
					}
				}
			}
			if ((bool)platePoint && (bool)_vehicleData.PlatesComposition.FrontPlatePoint && type != BodyKitType.RearBumper)
			{
				_vehicleData.PlatesComposition.FrontPlatePoint.SetPositionAndRotation(platePoint.position, platePoint.rotation);
				_frontPlatePoint = platePoint;
			}
			else if ((bool)platePoint && (bool)_vehicleData.PlatesComposition.RearPlatePoint && type == BodyKitType.RearBumper)
			{
				_vehicleData.PlatesComposition.RearPlatePoint.SetPositionAndRotation(platePoint.position, platePoint.rotation);
				_rearPlatePoint = platePoint;
			}
			if ((bool)bodyKitValues2 && bodyKitValues2.IsMirrorsThisBodyKit)
			{
				if ((bool)bodyKitValues2.MMirror)
				{
					_vehicleData.CarMirrorController.mMirror = bodyKitValues2.MMirror;
				}
				if ((bool)bodyKitValues2.LMirror)
				{
					_vehicleData.CarMirrorController.lMirror = bodyKitValues2.LMirror;
				}
				if ((bool)bodyKitValues2.RMirror)
				{
					_vehicleData.CarMirrorController.rMirror = bodyKitValues2.RMirror;
				}
				if ((bool)V.Car && V.Car.gameObject == _vehicleData.gameObject)
				{
					_vehicleData.CarMirrorController.SetMirror(FlagsStorage.ShowCarMirror);
				}
			}
			TryAttachSpoilerSystem(bodyKitResult, bodyKitValues2, type);
			TryDisableSpoilerSystem(_vehicleData, bodyKitValues2);
			return bodyKitResult;
		}
		catch (Exception message)
		{
			Debug.LogError(message);
			return null;
		}
	}

	private void HoldRoofBanksForKit(bodyKitValues kit)
	{
		SoundBankLoadService soundBankService = (_vehicleData ? _vehicleData.SoundBankService : null);
		if (soundBankService != null)
		{
			soundBankService.TryLoadBank(SoundBankType.CarRooftop).Forget();
			soundBankService.TryLoadBank(SoundBankType.CarWindow).Forget();
			kit.OnBodyKitDestroed += () =>
			{
				soundBankService.TryUnloadBank(SoundBankType.CarRooftop).Forget();
				soundBankService.TryUnloadBank(SoundBankType.CarWindow).Forget();
			};
		}
	}

	private void TryAttachSpoilerSystem(GameObject bodyKitResult, bodyKitValues kit, BodyKitType kitType)
	{
		if ((bool)_vehicleData && (bool)bodyKitResult)
		{
			SpoilerSystem spoilerSystem = (((bool)kit && (bool)kit.SpoilerSystem) ? kit.SpoilerSystem : bodyKitResult.GetComponentInChildren<SpoilerSystem>(includeInactive: true));
			if ((bool)spoilerSystem)
			{
				_vehicleData.BodyLoadingComposition.SetSpoiler(spoilerSystem);
			}
			else if (kitType == BodyKitType.Spoiler && (bool)_vehicleData.BodyLoadingComposition.SpoilerSystem)
			{
				_vehicleData.BodyLoadingComposition.SpoilerSystem.enabled = false;
			}
		}
	}

	private void TryDisableSpoilerSystem(VehicleData vehicleData, bodyKitValues bodyKitValues)
	{
		if ((bool)vehicleData && (bool)bodyKitValues && bodyKitValues.DisableDefaultSpoilerSystem)
		{
			vehicleData.BodyLoadingComposition.AddDisableSpoilerKit(bodyKitValues);
		}
	}

	private void InitPlatePoints()
	{
		if ((bool)_frontPlatePoint && (bool)_vehicleData.PlatesComposition.FrontPlatePoint)
		{
			_vehicleData.PlatesComposition.FrontPlatePoint.SetPositionAndRotation(_frontPlatePoint.position, _frontPlatePoint.rotation);
		}
		if ((bool)_rearPlatePoint && (bool)_vehicleData.PlatesComposition.RearPlatePoint)
		{
			_vehicleData.PlatesComposition.RearPlatePoint.SetPositionAndRotation(_rearPlatePoint.position, _rearPlatePoint.rotation);
		}
	}

	public bodyKitValues InitNewBodyKitValues(GameObject bodyKit, BodyKitType type, int id)
	{
		if (!bodyKit.TryGetComponent<bodyKitValues>(out var component))
		{
			component = bodyKit.AddComponent<bodyKitValues>();
		}
		component.VinylDrawer = _vinylDrawer;
		component.Type = (int)type;
		component.Index = id;
		bool flag = false;
		bool flag2 = false;
		foreach (BodyKitsLightRule lightRule in _lightRules)
		{
			if (lightRule.KitType == component.Type)
			{
				flag2 = true;
				if (lightRule.KitId == component.Index)
				{
					_lightController.RemoveLightsSide(lightRule.LightSideForRemove);
					flag = true;
				}
			}
		}
		if (flag2 && !flag && _lightController.LightSide != LightSideType.Both)
		{
			_lightController.SetBothLightSide();
			_lightController.SetupHeadLightsRenderers();
		}
		return component;
	}

	public Transform GetTransformByIDName(Transform[] transforms, int id)
	{
		Transform result = null;
		foreach (Transform transform in transforms)
		{
			if (Utils.ExtractIntFromName(transform.name) == id)
			{
				result = transform;
			}
		}
		return result;
	}

	public async UniTask<GameObject> LoadAndInstantiateAsset(string path, CancellationToken cancellationToken)
	{
		GameObject gameObject = await LoadAndInstantiateAsset(path);
		if (cancellationToken.IsCancellationRequested || _isBeingDestroyed || !gameObject)
		{
			if ((bool)gameObject)
			{
				UnityEngine.Object.Destroy(gameObject);
			}
			return null;
		}
		return gameObject;
	}

	private async UniTask<GameObject> LoadAndInstantiateAsset(string path)
	{
		GameObject gameObject = null;
		if (AddressableExtensions.AddressableResourceExists(path + ".prefab", typeof(GameObject)))
		{
			gameObject = await Addressables.LoadAssetAsync<GameObject>(path + ".prefab").BindTo(base.gameObject).ToUniTask();
			if (_isBeingDestroyed)
			{
				return null;
			}
		}
		if (!gameObject)
		{
			Debug.LogWarning(path + " asset is null");
			path += 0;
			if (AddressableExtensions.AddressableResourceExists(path + ".prefab", typeof(GameObject)))
			{
				gameObject = await Addressables.LoadAssetAsync<GameObject>(path + ".prefab").BindTo(base.gameObject).ToUniTask();
				if (_isBeingDestroyed)
				{
					return null;
				}
				if (!gameObject)
				{
					Debug.Log(path + " NOW asset is NOT null");
				}
			}
		}
		if (!gameObject)
		{
			return null;
		}
		GameObject instance = UnityEngine.Object.Instantiate(gameObject, parent);
		if (_vehicleData.IsCarGraphicLoaded)
		{
			EnableMeshRenderer();
		}
		else
		{
			_vehicleData.OnCarGraphicLoaded += EnableMeshRendererAndUnsubscribe;
		}
		return instance;
		void EnableMeshRenderer()
		{
			SmoothLoadCarInMenu.MeshRendererInChildrenEnabler(instance, enable: true);
		}
		void EnableMeshRendererAndUnsubscribe()
		{
			_vehicleData.OnCarGraphicLoaded -= EnableMeshRendererAndUnsubscribe;
			EnableMeshRenderer();
		}
	}

	public void SetForVinylPlace(GameObject part)
	{
		if (FlagsStorage.OnExteriorMenu && (bool)part)
		{
			part.transform.parent = _vehicleData.CarBodyReferences.Body.transform;
			if (!part.GetComponent<MeshCollider>())
			{
				part.gameObject.AddComponent<MeshCollider>();
			}
			part.GetComponent<MeshCollider>().enabled = true;
		}
		if ((bool)part)
		{
			part.gameObject.layer = 27;
		}
	}

	private void SearchForExhausts(BodyKitReferences references)
	{
		if ((bool)references && !references.Exhaust.IsNullOrEmpty())
		{
			_vehicleData.CarChildsReferences.UpdateExhaustsReferences(references.Exhaust);
			newExhaustFoundCount++;
			_vehicleData.ExhaustComposition.Install();
		}
	}

	public PaidExteriorPart GetPaidExteriorPart(BodyKitType bodyKitType, int id)
	{
		switch (bodyKitType)
		{
		case BodyKitType.Spoiler:
			return PaidSpoilers.FirstOrDefault((PaidExteriorPart x) => (int)x.Id == id);
		case BodyKitType.FrontBumper:
			return PaidFrontBumpers.FirstOrDefault((PaidExteriorPart x) => (int)x.Id == id);
		case BodyKitType.RearBumper:
			return PaidRearBumpers.FirstOrDefault((PaidExteriorPart x) => (int)x.Id == id);
		case BodyKitType.Skirt:
			return PaidSkirts.FirstOrDefault((PaidExteriorPart x) => (int)x.Id == id);
		case BodyKitType.Hood:
			return PaidHoods.FirstOrDefault((PaidExteriorPart x) => (int)x.Id == id);
		case BodyKitType.AirIntakeHood:
			return PaidIntakeHood.FirstOrDefault((PaidExteriorPart x) => (int)x.Id == id);
		case BodyKitType.AirIntakeRoof:
			return PaidIntakeRoof.FirstOrDefault((PaidExteriorPart x) => (int)x.Id == id);
		case BodyKitType.Fender:
			return PaidFenders.FirstOrDefault((PaidExteriorPart x) => (int)x.Id == id);
		case BodyKitType.Trim:
			return PaidTrims.FirstOrDefault((PaidExteriorPart x) => (int)x.Id == id);
		case BodyKitType.Interior:
		{
			InteriorInfoConfig interiorInfoConfig = ((_vehicleData != null && _vehicleData.InteriorsComposition != null) ? _vehicleData.InteriorsComposition.InteriorInfoConfig : null);
			InteriorInfo interiorInfo = (interiorInfoConfig ? interiorInfoConfig.InteriorsInfos.FirstOrDefault((InteriorInfo x) => x.Number == id) : null);
			return new PaidExteriorPart
			{
				PartType = ((interiorInfo != null) ? BodyKitType.Interior : BodyKitType.Spoiler),
				Id = (interiorInfo?.Number ?? 0),
				Cost = (interiorInfo?.Price ?? default(ObscuredInt))
			};
		}
		default:
			return default;
		}
	}

	public void GetPriceForKit(BodyKitType bodyKitType, int id, IPricesService pricesService, out int price, out CostType priceType)
	{
		PaidExteriorPart paidExteriorPart = GetPaidExteriorPart(bodyKitType, id);
		price = paidExteriorPart.Cost;
		priceType = (((int)paidExteriorPart.Cost != 0) ? CostType.Coins : CostType.Money);
		price = ((price == 0) ? ((int)pricesService.BodyKitsMoneyPrice) : price);
	}

	public bodyKitValues GetInstalledBodyKitValues(BodyKitType bodyKitType)
	{
		GameObject installedBodyKit = GetInstalledBodyKit(bodyKitType);
		if (!(installedBodyKit != null))
		{
			return null;
		}
		return installedBodyKit.GetComponent<bodyKitValues>();
	}

	public GameObject GetInstalledBodyKit(BodyKitType bodyKitType)
	{
		return bodyKitType switch
		{
			BodyKitType.Spoiler => InstalledSpoiler, 
			BodyKitType.FrontBumper => InstalledFrontBumper, 
			BodyKitType.RearBumper => InstalledRearBumper, 
			BodyKitType.Skirt => installedSkirts, 
			BodyKitType.Hood => installedHood, 
			BodyKitType.AirIntakeHood => installedAirIntakeHood, 
			BodyKitType.AirIntakeRoof => installedAirIntakeRoof, 
			BodyKitType.Fender => installedFender, 
			_ => null, 
		};
	}

	public void SetInstalledBodyKit(BodyKitType bodyKitType, GameObject value)
	{
		switch (bodyKitType)
		{
		case BodyKitType.Spoiler:
			InstalledSpoiler = value;
			break;
		case BodyKitType.FrontBumper:
			InstalledFrontBumper = value;
			break;
		case BodyKitType.RearBumper:
			InstalledRearBumper = value;
			break;
		case BodyKitType.Skirt:
			installedSkirts = value;
			break;
		case BodyKitType.Hood:
			installedHood = value;
			break;
		case BodyKitType.AirIntakeHood:
			installedAirIntakeHood = value;
			break;
		case BodyKitType.AirIntakeRoof:
			installedAirIntakeRoof = value;
			break;
		case BodyKitType.Fender:
			installedFender = value;
			break;
		}
		if ((bool)_vehicleData)
		{
			_vehicleData.WindowComposition?.RefreshWiperSources();
		}
	}

	private void BodyKitLoaded(BodyKitType type)
	{
		loadedTypes.Add(type);
		if (loadedTypes.Contains(BodyKitType.Spoiler) && loadedTypes.Contains(BodyKitType.FrontBumper) && loadedTypes.Contains(BodyKitType.RearBumper) && loadedTypes.Contains(BodyKitType.Skirt) && loadedTypes.Contains(BodyKitType.Hood) && loadedTypes.Contains(BodyKitType.AirIntakeHood) && loadedTypes.Contains(BodyKitType.AirIntakeRoof) && loadedTypes.Contains(BodyKitType.Fender))
		{
			AllBodyKitsWasLoaded.Value = true;
		}
	}

	private void TryInitTrunk(bodyKitValues bodyKitValues)
	{
		if ((bool)bodyKitValues)
		{
			if ((bool)bodyKitValues.Trunk)
			{
				_vehicleData.CarAnimationReferences.SetTrunkParent(bodyKitValues.Trunk, bodyKitValues.MoveableObjectsParent, bodyKitValues.PlateTrunkParent);
			}
			UpdatePlatePos(bodyKitValues.PlatePoint);
		}
	}

	private void UpdatePlatePos(Transform platePoint)
	{
		if ((bool)platePoint)
		{
			_vehicleData.PlatesComposition.RearPlatePos = platePoint;
			if (!_vehicleData.PlatesComposition.Plates.IsNullOrEmpty() && (bool)_vehicleData.PlatesComposition.Plates[1])
			{
				_vehicleData.PlatesComposition.Plates[1].transform.position = platePoint.position;
			}
		}
	}
}
