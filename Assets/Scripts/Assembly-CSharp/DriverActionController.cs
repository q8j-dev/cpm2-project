using System;
using Addler.Runtime.Core.LifetimeBinding;
using CPM.Database;
using CPM.Rating.Entities;
using CPMEngine;
using CarInteractionAnimationsScripts;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Views;
using Photon.Pun;
using RootMotion.FinalIK;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService.Homes;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Other.ProceduralDoors;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem;
using _Car_Parking.Scripts.WorldInteraction;

public class DriverActionController : MonoBehaviour
{
	private const float TimeMoveToShifter = 0.25f;

	private const float TimeMoveToWheel = 0.15f;

	private const string UI_MAP_ICON = "UI_mapiconperson";

	private const string UI_PERSON_LABEL = "UI_PersonLabel";

	private const int PersonLabelOwnerRetries = 10;

	public Transform Shifter;

	public bool interrupt;

	public FullBodyBipedIK ik;

	private DriverController driverControll;

	private float smoothTime = 0.1f;

	public GameObject Car;

	public HandPoser LeftHandPoser;

	public HandPoser RightHandPoser;

	private VehicleData _vehicleData;

	public bool TouchShifter = true;

	public bool RightHanded;

	private int state = -1;

	private int oldstate;

	public PersonAnimationsController PersonAnimationsController;

	public Animator Animator;

	public int current_tar_index;

	public Transform[] targets;

	public int carDirection;

	public Collider[] allColliders;

	public bool isPassenger;

	private GameObject PersonLabel;

	public IconMapView MapPersonIcon;

	public GameObject speakingLabel;

	public MultiPersonControl multiPersonControl;

	private MainPerson mainPerson;

	public GameObject capToHide;

	public int skipFrame = 3;

	public float zRotCar;

	public bool resetIK;

	public Vector3 initRot;

	private WorldInteractionController _worldInteraction;

	public float steering;

	private IDisposable _enableCollidersDisposable;

	private IDisposable _setPoserFingerDisposable;

	private CurrencyManagerController _currencyManagerController;

	private HomesConfig _homesConfig;

	private bool animationUpdated;

	private GlobalCollectionsProvider _collectionsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private LevelsProvider _levelsProvider;

	private PhotonView _photonView;

	private PhotoModeObserver _photoModeObserver;

	private bool _isDestroyed;

	public RagdollCPM ragdollCpm;

	public float carSpeed;

	public PersonHeadMove PersonHeadMove;

	private CarInputManager _carInputManager;

	public Transform aimTarget;

	private bool _personIconLoading;

	public Rigidbody carRb;

	private float mulZ = -0.01f;

	private float smoothZ = 0.02f;

	public float maxAccelZ = 10f;

	public float minAccelZ = 10f;

	private float maxZOffset = 0.3f;

	private float mulX = 0.008f;

	public float smoothX = 0.06f;

	public float maxAccelX = 8f;

	public float minAccelX = 8f;

	private float maxXOffset = 0.2f;

	private Vector3 lastVelocity;

	public Vector3 localAcceleration;

	private float zOff;

	private float xOff;

	public float baseL_Z;

	public float baseR_Z;

	public float baseL_X;

	public float baseR_X;

	public bool initDone;

	public Transform RightUpperArm;

	public Transform LeftUpperArm;

	[Header("Crash Shake")]
	public float shakeReturnSpeed = 1f;

	public float shakeDuration = 1f;

	public float currentShakeX;

	public float currentShakeZ;

	public float shakeTimer;

	public float random = 1f;

	private float deadRange = 3f;

	public int skipFrames = 1;

	private int frameCounter;

	public float time;

	private float desiredZLerp = 0.1f;

	public float desiredZ;

	public bool insideHeadCam;

	public bool IsExitCarBlocked { get; set; }

	private HandIKTarget ShifterHandIKTarget
	{
		get
		{
			if (!driverControll)
			{
				return null;
			}
			if (!RightHanded)
			{
				return driverControll.RightHandIKTarget;
			}
			return driverControll.LeftHandIKTarget;
		}
	}

	public bool PersonInCar { get; set; }

	[Inject]
	public void Construct(CurrencyManagerController currencyManagerController, HomesConfig homesConfig, GlobalCollectionsProvider collectionsProvider, FreeDriveProvider freeDriveProvider, LevelsProvider levelsProvider, OptionalDependency<PhotoModeObserver> photoModeObserver, CarInputManager carInputManager)
	{
		_levelsProvider = levelsProvider;
		_freeDriveProvider = freeDriveProvider;
		_collectionsProvider = collectionsProvider;
		_homesConfig = homesConfig;
		_currencyManagerController = currencyManagerController;
		_photoModeObserver = photoModeObserver.Value;
		_carInputManager = carInputManager;
	}

	private void Awake()
	{
		SetColliders();
	}

	public void Init(GameObject car, VehicleData vh, DriverController dc)
	{
		Car = car;
		driverControll = dc;
		_vehicleData = vh;
		initDone = false;
		carRb = car.GetComponent<Rigidbody>();
	}

	private void Start()
	{
		multiPersonControl = GetComponent<MultiPersonControl>();
		mainPerson = GetComponent<MainPerson>();
		_worldInteraction = GetComponent<WorldInteractionController>();
		if (base.gameObject == V.DriverPerson)
		{
			if ((bool)RightHandPoser)
			{
				RightHandPoser.enabled = true;
				LeftHandPoser.enabled = true;
			}
			AddHeadMove();
			_ = AddRagdoll();
		}
		else
		{
			LoadPersonLabel().Forget();
		}
		CreatePersonIcon();
		_photonView = GetComponent<PhotonView>();
	}

	public void AddHeadMove()
	{
		if (!(PersonHeadMove != null))
		{
			PersonHeadMove = base.gameObject.AddComponent<PersonHeadMove>();
			PersonHeadMove.driverActionController = this;
			PersonHeadMove.multiPersonControl = multiPersonControl;
		}
	}

	public async UniTask AddRagdoll()
	{
		while (ik == null)
		{
			await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
		}
		ragdollCpm = ik.GetComponent<RagdollCPM>();
		if (ragdollCpm != null)
		{
			ragdollCpm.createTime = Time.time;
			ragdollCpm.Init(this, _photoModeObserver, _carInputManager);
		}
	}

	private void OnDestroy()
	{
		if ((bool)V.CoreGameManager && V.CoreGameManager.MapIconsModel != null && (bool)MapPersonIcon)
		{
			V.CoreGameManager.MapIconsModel.RemoveMapIconModel(MapPersonIcon);
		}
	}

	private void LateUpdate()
	{
		if (Time.frameCount % skipFrame == 0)
		{
			checkDistanceIkEnable();
		}
	}

	public void CreatePersonIcon()
	{
		CreatePersonIconAsync().Forget();
	}

	private async UniTaskVoid CreatePersonIconAsync()
	{
		if (!_freeDriveProvider.MapControl)
		{
			return;
		}
		if (!MapPersonIcon)
		{
			if (_personIconLoading)
			{
				return;
			}
			_personIconLoading = true;
			try
			{
				GameObject original = await Addressables.LoadAssetAsync<GameObject>("UI_mapiconperson").BindTo(base.gameObject);
				if (!this || !base.gameObject)
				{
					return;
				}
				if (UnityEngine.Object.Instantiate(original).TryGetComponent<IconMapView>(out MapPersonIcon))
				{
					_collectionsProvider.RegisterMapIcon(MapPersonIcon.transform);
				}
			}
			finally
			{
				_personIconLoading = false;
			}
		}
		MapPersonIcon.transform.parent = base.gameObject.transform;
		MapPersonIcon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(90f, 0f, 0f));
		if (FlagsStorage.IsMultiFree)
		{
			(from _ in Observable.EveryUpdate()
				where _photonView
				select _).First().Subscribe((long _) =>
			{
				if (_photonView.IsMine && (bool)V.CoreGameManager && V.CoreGameManager.MapIconsModel != null)
				{
					V.CoreGameManager.MapIconsModel.PlayerIcon = MapPersonIcon;
				}
				if (!_photonView.IsMine && V.CoreGameManager.MapIconsModel != null && V.CoreGameManager.MapIconsDataSo != null)
				{
					MapPersonIcon.SpriteRenderer.sprite = V.CoreGameManager.MapIconsDataSo.GetIconData(IconType.OtherPlayers).SpriteIcon;
					MapPersonIcon.gameObject.SetActive(value: false);
					V.CoreGameManager.MapIconsModel.AddMapIconModel(MapPersonIcon, IconType.OtherPlayers);
				}
			}).AddTo(this);
		}
		else if (V.CoreGameManager.MapIconsModel != null)
		{
			V.CoreGameManager.MapIconsModel.PlayerIcon = MapPersonIcon;
		}
		if ((bool)_freeDriveProvider.MapCamera)
		{
			_freeDriveProvider.MapCamera.zoomIcon(_freeDriveProvider.MapCamera.OrtSizeForSmallMapIcons);
		}
	}

	private async UniTaskVoid LoadPersonLabel(int attempt = 0)
	{
		if (!TryGetComponent<PhotonView>(out var component) || component.Owner == null)
		{
			if (attempt < 10)
			{
				Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					LoadPersonLabel(attempt + 1).Forget();
				}).AddTo(this);
			}
		}
		else
		{
			if ((bool)PersonLabel || !FlagsStorage.IsMultiFree)
			{
				return;
			}
			PersonLabel = await Addressables.InstantiateAsync("UI_PersonLabel");
			PhotonView photonView = (this ? GetComponent<PhotonView>() : null);
			if (!this || !PersonLabel || !photonView || photonView.Owner == null)
			{
				if ((bool)PersonLabel)
				{
					Addressables.ReleaseInstance(PersonLabel);
				}
				PersonLabel = null;
				return;
			}
			PersonLabel.transform.parent = base.gameObject.transform;
			speakingLabel = PersonLabel.transform.GetChild(0).gameObject;
			speakingLabel.transform.parent = null;
			Vector3 localScale = speakingLabel.transform.localScale;
			speakingLabel.transform.parent = PersonLabel.transform;
			PersonLabel.transform.localPosition = new Vector3(0f, 2.05f, 0f);
			PersonLabel.gameObject.SetActive(value: false);
			string nickName = photonView.Owner.NickName;
			TMP_FontAsset_Placeholder.EnsureFor(nickName);
			if (PersonLabel.TryGetComponent<TextMeshPro>(out var component2))
			{
				component2.text = nickName;
			}
			PersonLabel.transform.GetChild(0).transform.localScale = new Vector3((float)nickName.Length * 0.1f, 0.12f, 1f);
			PersonLabel.transform.GetChild(0).transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			if ((bool)V.CoreGameManager && PersonLabel.TryGetComponent<AllwaysLookAtCam>(out var component3))
			{
				component3.cam = V.CoreGameManager.gameObject.transform;
			}
			speakingLabel.transform.parent = null;
			speakingLabel.transform.localScale = localScale;
			speakingLabel.transform.parent = PersonLabel.transform;
			speakingLabel.SetActive(value: false);
			GameObject rankObj = PersonLabel.transform.GetChild(1).gameObject;
			(from _ in Observable.Interval(TimeSpan.FromMilliseconds(10.0))
				where multiPersonControl.rank.HasValue
				select _).Take(1).Subscribe((long _) =>
			{
				Material material = UtilsRank.LoadRankMaterialFromResources((UtilsRank.RankType)((int?)multiPersonControl.rank).Value);
				rankObj.GetComponent<MeshRenderer>().material = material;
				rankObj.GetComponent<MeshRenderer>().enabled = true;
				multiPersonControl.topLabel = PersonLabel;
			}).AddTo(this);
		}
	}

	public void HidePersonLabel()
	{
		if ((bool)PersonLabel)
		{
			PersonLabel.SetActive(value: false);
		}
	}

	public void ShowPersonLabel()
	{
		if ((bool)PersonLabel && V.DriverPerson != base.gameObject)
		{
			PersonLabel.SetActive(value: true);
		}
	}

	private void SetColliders()
	{
		EnableDisableAllColliders(WillEnable: false);
	}

	private void DisableIk()
	{
		bool num = (bool)PersonAnimationsController && PersonAnimationsController.SitGetOutCarSystem.SitGetOutData.IsSittingIntoCarAnimate;
		bool flag = (bool)PersonAnimationsController && PersonAnimationsController.SitGetOutCarSystem.SitGetOutData.IsGettingOutOfTheCarAnimate;
		if (!(num | flag))
		{
			ik.enabled = false;
		}
	}

	private void EnableAnimator()
	{
		if (!ragdollCpm || (!ragdollCpm.isRagdoll && !ragdollCpm.isStadningUp))
		{
			if (!PersonAnimationsController)
			{
				Animator.enabled = true;
			}
			else
			{
				PersonAnimationsController.EnableAnimator();
			}
			if ((bool)Animator && base.gameObject != V.DriverPerson)
			{
				Animator.cullingMode = AnimatorCullingMode.CullCompletely;
			}
		}
	}

	private void DisableAnimator()
	{
		if (!PersonAnimationsController)
		{
			Animator.enabled = false;
		}
		else
		{
			PersonAnimationsController.DisableAnimator();
		}
	}

	private bool IsEnabledAnimator()
	{
		if (!PersonAnimationsController)
		{
			return Animator.enabled;
		}
		return PersonAnimationsController.Animator.enabled;
	}

	private void checkDistanceIkEnable()
	{
		if (!V.DriverPerson)
		{
			return;
		}
		if (FlagsStorage.IsMultiFree && (bool)MapPersonIcon && (bool)_photonView && !_photonView.IsMine)
		{
			MapPersonIcon.gameObject.SetActive(!multiPersonControl.currentCar);
		}
		if (!ik || !V.CoreGameManager)
		{
			return;
		}
		if (Vector3.Distance(V.CoreGameManager.transform.position, base.transform.position) > 7f && Car != V.Car)
		{
			if (!multiPersonControl.currentCar)
			{
				if (mainPerson.SkinnedMeshRender.isVisible)
				{
					EnableAnimator();
				}
				else
				{
					DisableAnimator();
				}
				return;
			}
			if (IsEnabledAnimator())
			{
				DisableAnimator();
				UpdateIK();
				return;
			}
			if (ik.enabled && !IsEnabledAnimator() && (!(_worldInteraction != null) || !_worldInteraction.IsPosed))
			{
				UpdateIK();
				DisableIk();
			}
			else if ((bool)Car && Mathf.Abs(zRotCar - Car.transform.eulerAngles.z) > 0.2f)
			{
				UpdateIK();
				zRotCar = Car.transform.eulerAngles.z;
			}
			if (resetIK)
			{
				ik.enabled = true;
				UpdateIK();
				EnableAnimator();
				resetIK = false;
			}
			return;
		}
		if ((bool)multiPersonControl.currentCar)
		{
			if (!ik.enabled)
			{
				ik.enabled = true;
				UpdateIK();
			}
		}
		else if (ik.enabled && (!(_worldInteraction != null) || !_worldInteraction.IsPosed))
		{
			DisableIk();
		}
		if (!IsEnabledAnimator())
		{
			EnableAnimator();
		}
	}

	private void Initialise()
	{
		if ((bool)carRb)
		{
			lastVelocity = carRb.velocity;
		}
		zOff = (xOff = 0f);
		Vector3 rUpperArmPosition = _vehicleData.allPersonPoints.allTargets[multiPersonControl.placeOnCar].RUpperArmPosition;
		Vector3 lUpperArmPosition = _vehicleData.allPersonPoints.allTargets[multiPersonControl.placeOnCar].LUpperArmPosition;
		LeftUpperArm = _vehicleData.allPersonPoints.allTargets[multiPersonControl.placeOnCar].LUpperArm.transform;
		RightUpperArm = _vehicleData.allPersonPoints.allTargets[multiPersonControl.placeOnCar].RUpperArm.transform;
		baseL_Z = lUpperArmPosition.z;
		baseR_Z = rUpperArmPosition.z;
		baseL_X = lUpperArmPosition.x;
		baseR_X = rUpperArmPosition.x;
		initDone = true;
		random = UnityEngine.Random.Range(0.8f, 1.2f);
	}

	public void TriggerCrashShake(float impulseX, float impulseZ)
	{
		if (currentShakeZ == 0f)
		{
			currentShakeX = Mathf.Clamp(currentShakeX, 0f - maxXOffset, maxXOffset);
			currentShakeZ = Mathf.Clamp(currentShakeZ, 0f - maxZOffset, maxZOffset);
			currentShakeX = impulseX;
			currentShakeZ = impulseZ;
			shakeTimer = shakeDuration;
		}
	}

	private float FilterDeadZone(float input, float deadZone = 2f)
	{
		if (Mathf.Abs(input) < deadZone)
		{
			return 0f;
		}
		if (!(input < 0f))
		{
			return input - deadZone;
		}
		return input + deadZone;
	}

	private void FixedUpdate()
	{
		if (!multiPersonControl || !multiPersonControl.currentCar || (base.gameObject != V.DriverPerson && multiPersonControl.currentCar != V.Car))
		{
			return;
		}
		frameCounter++;
		if (frameCounter < skipFrames)
		{
			return;
		}
		frameCounter = 0;
		if (!carRb)
		{
			return;
		}
		if (!initDone)
		{
			if ((bool)_vehicleData && (bool)_vehicleData.allPersonPoints && _vehicleData.allPersonPoints.allTargets != null && (bool)_vehicleData.allPersonPoints.allTargets[multiPersonControl.placeOnCar].RUpperArm)
			{
				Initialise();
			}
			return;
		}
		float num = Time.time - time;
		if (num != 0f)
		{
			Vector3 velocity = carRb.velocity;
			if (base.gameObject == V.DriverPerson && (bool)mainPerson && (bool)mainPerson.SwitchHeadController)
			{
				insideHeadCam = mainPerson.SwitchHeadController.IsHeadHidden;
			}
			Vector3 direction = (velocity - lastVelocity) / num;
			time = Time.time;
			lastVelocity = velocity;
			Vector3 vector = carRb.transform.InverseTransformDirection(direction);
			localAcceleration = (insideHeadCam ? Vector3.zero : vector);
			if (insideHeadCam)
			{
				desiredZ = 0f;
				zOff = 0f;
				xOff = 0f;
			}
			float input = Mathf.Clamp(localAcceleration.z, 0f - minAccelZ, maxAccelZ);
			input = FilterDeadZone(input, deadRange);
			float t = ((Mathf.Abs(input) > 1f) ? desiredZLerp : (desiredZLerp * 3f));
			desiredZ = Mathf.Lerp(desiredZ, input * mulZ * random, t);
			float num2 = Mathf.Clamp(localAcceleration.x, 0f - minAccelX, maxAccelX);
			if (Mathf.Abs(num2) < 2f)
			{
				num2 = 0f;
			}
			float value = num2 * mulX;
			if (shakeTimer >= 0f)
			{
				shakeTimer -= num;
				shakeTimer = Mathf.Clamp(shakeTimer, 0f, shakeDuration);
				currentShakeX = Mathf.MoveTowards(currentShakeX, 0f, shakeReturnSpeed * num);
				currentShakeZ = Mathf.MoveTowards(currentShakeZ, 0f, shakeReturnSpeed * num);
			}
			zOff = Mathf.Lerp(t: (Mathf.Abs(zOff) > 0.1f) ? smoothZ : (smoothZ * 3f), a: zOff, b: desiredZ);
			zOff = Mathf.Clamp(zOff + currentShakeZ, (0f - maxZOffset) / 2f, maxZOffset);
			float b = Mathf.Clamp(value, 0f - maxXOffset, maxXOffset);
			xOff = Mathf.Lerp(xOff, b, smoothX);
			xOff = Mathf.Clamp(xOff + currentShakeX, 0f - maxXOffset, maxXOffset);
			if ((bool)LeftUpperArm)
			{
				LeftUpperArm.localPosition = new Vector3(baseL_X - xOff, LeftUpperArm.localPosition.y, baseL_Z + zOff);
			}
			if ((bool)RightUpperArm)
			{
				RightUpperArm.localPosition = new Vector3(baseR_X - xOff, RightUpperArm.localPosition.y, baseR_Z + zOff);
			}
		}
	}

	public void UpdateIK()
	{
		if ((bool)ik && ik.solver.effectors[0] != null && !float.IsNaN(ik.solver.effectors[0].positionOffset.x) && !float.IsNaN(ik.solver.effectors[0].positionOffset.y) && !float.IsNaN(ik.solver.effectors[0].positionOffset.z))
		{
			ik.solver.Update();
		}
	}

	public void EnableDisableAllColliders(bool WillEnable, bool immediately = false)
	{
		_enableCollidersDisposable?.Dispose();
		if (WillEnable)
		{
			if (immediately)
			{
				EnableCollidersAfter();
			}
			else
			{
				_enableCollidersDisposable = Observable.Timer(TimeSpan.FromSeconds(0.30000001192092896)).Subscribe((long _) =>
				{
					EnableCollidersAfter();
				}).AddTo(this);
			}
		}
		else
		{
			DisableCollidersAfter();
		}
		if ((bool)capToHide)
		{
			capToHide.SetActive(WillEnable);
		}
	}

	private void EnableCollidersAfter()
	{
		PersonInCar = false;
		if ((bool)GetComponent<MainPerson>())
		{
			ik = GetComponent<MainPerson>().ik;
		}
		Collider[] array = allColliders;
		foreach (Collider collider in array)
		{
			if ((bool)collider)
			{
				collider.enabled = true;
			}
		}
	}

	private void DisableCollidersAfter()
	{
		PersonInCar = true;
		Collider[] array = allColliders;
		foreach (Collider collider in array)
		{
			if ((bool)collider)
			{
				collider.enabled = false;
			}
		}
		if ((bool)mainPerson)
		{
			ik = mainPerson.ik;
		}
	}

	public void Shiftgear()
	{
		if (!isPassenger && (bool)Shifter)
		{
			smoothTime = 0.04f;
			_setPoserFingerDisposable?.Dispose();
			if ((bool)RightHandPoser)
			{
				RightHandPoser.weight = 0f;
			}
			_setPoserFingerDisposable = Observable.Timer(TimeSpan.FromSeconds(0.07000000029802322)).Subscribe((long _) =>
			{
				setPoserFingerShifter();
			}).AddTo(this);
			if ((bool)ShifterHandIKTarget)
			{
				ShifterHandIKTarget.transform.SetParent(Shifter);
				ShifterHandIKTarget.MoverAndRotatorByTween.ApplyToZero(0.25f, Ease.OutSine, isLocal: true);
			}
		}
	}

	public void GripWheel()
	{
		if (!isPassenger && (bool)driverControll)
		{
			smoothTime = 0.3f;
			if (driverControll.HaveSteeringWheel)
			{
				driverControll.SteeringWheel.InitHands(driverControll.LeftHandIKTarget, driverControll.RightHandIKTarget);
				driverControll.LeftHandIKTarget.MoverAndRotatorByTween.ApplyToZero(0.15f, Ease.OutSine, isLocal: true);
				driverControll.RightHandIKTarget.MoverAndRotatorByTween.ApplyToZero(0.15f, Ease.OutSine, isLocal: true);
			}
			_setPoserFingerDisposable?.Dispose();
			if (!RightHanded && (bool)RightHandPoser)
			{
				RightHandPoser.weight = 0f;
			}
			else if (RightHanded && (bool)LeftHandPoser)
			{
				LeftHandPoser.weight = 0f;
			}
			_setPoserFingerDisposable = Observable.Timer(TimeSpan.FromSeconds(0.3400000035762787)).Subscribe((long _) =>
			{
				setPoserFingerWheel();
			}).AddTo(this);
		}
	}

	private void setPoserFingerWheel()
	{
		if (isPassenger)
		{
			return;
		}
		if (oldstate != 1 && (bool)RightHandPoser)
		{
			RightHandPoser.weight = 1f;
		}
		if ((bool)LeftHandPoser)
		{
			LeftHandPoser.weight = 1f;
		}
		if ((bool)driverControll && (bool)driverControll.RightHandIKTarget)
		{
			if (!RightHanded && (bool)RightHandPoser)
			{
				RightHandPoser.poseRoot = driverControll.RightHandIKTarget.transform;
			}
			else if (RightHanded && (bool)LeftHandPoser)
			{
				LeftHandPoser.poseRoot = driverControll.LeftHandIKTarget.transform;
			}
		}
	}

	private void setPoserFingerShifter()
	{
		if (!isPassenger)
		{
			if ((bool)LeftHandPoser)
			{
				LeftHandPoser.weight = 1f;
			}
			if ((bool)RightHandPoser)
			{
				RightHandPoser.weight = 1f;
			}
		}
	}

	private void UpdateCurrentRow()
	{
		if (!V.CoreGameManager.isFreeDrive || !(V.DriverPerson == base.gameObject))
		{
			return;
		}
		if (current_tar_index <= 1)
		{
			current_tar_index = 2;
		}
		if (current_tar_index >= targets.Length - 1 || current_tar_index <= 1)
		{
			return;
		}
		float num = Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(targets[current_tar_index].position.x, targets[current_tar_index].position.z));
		float num2 = Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(targets[current_tar_index - 1].position.x, targets[current_tar_index - 1].position.z));
		float num3 = Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(targets[current_tar_index + 1].position.x, targets[current_tar_index + 1].position.z));
		float num4 = Mathf.Min(num, num2, num3);
		if (num4 == num)
		{
			return;
		}
		if (num4 == num2)
		{
			current_tar_index--;
			carDirection = -1;
		}
		else if (num4 == num3)
		{
			if (current_tar_index < targets.Length - 2)
			{
				current_tar_index++;
			}
			carDirection = 1;
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (FlagsStorage.IsPhotoMode || FlagsStorage.IgnoreTriggersAfterExit || !(V.DriverPerson == base.gameObject))
		{
			return;
		}
		onTriggerExitMutual(other);
		if (SceneManager.GetActiveScene().buildIndex == 13 && other.name == "TollP")
		{
			TallPerson component = other.GetComponent<TallPerson>();
			if (component.person == base.gameObject)
			{
				component.person = null;
				_freeDriveProvider.TallPerson = null;
			}
		}
	}

	public void onTriggerExitMutual(Collider other)
	{
		string text = other.name;
		if (text == "StayInColliderLink" && other.TryGetComponent<StayInColliderLink>(out var component))
		{
			component.OnExit();
		}
		if (text == "ProceduralDoorFrontCollider" && other.TryGetComponent<ProceduralDoorFrontCollider>(out var component2))
		{
			component2.Exit?.Invoke();
		}
		if (text == "ProceduralDoorBackCollider" && other.TryGetComponent<ProceduralDoorBackCollider>(out var component3))
		{
			component3.Exit?.Invoke();
		}
		switch (text)
		{
		case "inTunnel":
			_freeDriveProvider.FreeDriveControl.inTunnel();
			break;
		case "outTunnel":
		case "tunnelEffect":
			_freeDriveProvider.FreeDriveControl.outTunnel();
			break;
		case "action1":
			_freeDriveProvider.FreeDriveControl.HideActionButton();
			if ((bool)V.Car)
			{
				V.Car.GetComponent<VehicleData>().CarOnLift = false;
			}
			break;
		case "onParking":
			V.SceneControl.ShowHideCityParts(show: true);
			V.SceneControl.SetScenePart(V.SceneControl.cityScenePartSettings);
			break;
		case "ParkingB_Exit":
			V.SceneControl.RequestUnloadParkingBuilding();
			break;
		case "supermarket":
			other.GetComponent<superMarketBuilding>().EnableLights(enable: false);
			break;
		}
	}

	public async UniTaskVoid OnTriggerMutual(Collider other)
	{
		string text = other.name;
		if (other.gameObject.CompareTag("restrict"))
		{
			if (SceneManager.GetActiveScene().buildIndex == 13)
			{
				if (FlagsStorage.IsPassenger && V.Car.GetComponent<VehicleData>().Driver == null)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("DriverActionController.ThisWorldEndsHere"), 5f);
					_freeDriveProvider.FreeDriveControl.CarToRoad(V.Car);
					return;
				}
				switch (other.transform.name)
				{
				case "home0":
					V.CoreGameManager.SetMessage(new LocalizedString("DriverActionController.PrivateZoneOrBuyThisHouse"), 5f);
					if (FlagsStorage.OutOfCar)
					{
						base.transform.position = _homesConfig.Get(0).Positions.RestrictPosition;
						break;
					}
					V.Car.transform.position = _homesConfig.Get(0).Positions.RestrictPosition;
					V.Car.GetComponent<Rigidbody>().velocity = Vector3.zero;
					break;
				case "home1":
					V.CoreGameManager.SetMessage(new LocalizedString("DriverActionController.PrivateZoneOrBuyThisHouse"), 5f);
					if (FlagsStorage.OutOfCar)
					{
						base.transform.position = _homesConfig.Get(1).Positions.RestrictPosition;
						break;
					}
					V.Car.transform.position = _homesConfig.Get(1).Positions.RestrictPosition;
					V.Car.GetComponent<Rigidbody>().velocity = Vector3.zero;
					break;
				case "home2":
					V.CoreGameManager.SetMessage(new LocalizedString("DriverActionController.PrivateZoneOrBuyThisHouse"), 5f);
					if (FlagsStorage.OutOfCar)
					{
						base.transform.position = _homesConfig.Get(2).Positions.RestrictPosition;
						break;
					}
					V.Car.transform.position = _homesConfig.Get(2).Positions.RestrictPosition;
					V.Car.GetComponent<Rigidbody>().velocity = Vector3.zero;
					break;
				case "home3":
					V.CoreGameManager.SetMessage(new LocalizedString("DriverActionController.PrivateZoneOrBuyThisHouse"), 5f);
					if (FlagsStorage.OutOfCar)
					{
						base.transform.position = _homesConfig.Get(3).Positions.RestrictPosition;
						break;
					}
					V.Car.transform.position = _homesConfig.Get(3).Positions.RestrictPosition;
					V.Car.GetComponent<Rigidbody>().velocity = Vector3.zero;
					break;
				default:
					if (_freeDriveProvider.RoomArenaId == ArenaId.ObstacleRace_1)
					{
						V.CoreGameManager.SetMessage(new LocalizedString("DriverActionController.ThisWorldEndsHere"), 5f, NotificationType.Success, showBackground: false, 0.15f, 0.2f);
					}
					else
					{
						V.CoreGameManager.SetMessage(new LocalizedString("DriverActionController.ThisWorldEndsHere"), 5f);
					}
					_freeDriveProvider.FreeDriveControl.ToRoad();
					break;
				}
			}
			else
			{
				_levelsProvider.LevelScript.IsRestricted = true;
				_ = _levelsProvider.LevelScript.StartLevel();
			}
		}
		else if (other.CompareTag("easterEgg"))
		{
			int num = int.Parse(text);
			if (ObscuredPrefs.GetInt("egg" + num) == 1)
			{
				return;
			}
			ObscuredPrefs.SetInt("egg" + num, 1);
			int num2 = 1;
			for (int i = 0; i < 100; i++)
			{
				if (ObscuredPrefs.GetInt("egg" + i) == 1)
				{
					num2++;
				}
			}
			ObscuredInt obscuredInt = num2 * 1000;
			num2--;
			UnityEngine.Object.Destroy(other.gameObject);
			_currencyManagerController.AddMoney(new AdvancedInt(obscuredInt));
			string text2 = num2 + " of " + 88;
			CoreGameManager coreGameManager = V.CoreGameManager;
			ObscuredInt obscuredInt2 = obscuredInt;
			coreGameManager.SetMessage(text2 + " Gift found +" + obscuredInt2.ToString(), 3f);
			ObscuredPrefs.Save();
			FreeDriveDB freeDriveDB = _freeDriveProvider.FreeDriveDB;
			int? gifts = num2;
			freeDriveDB.SendRatingData(new RatingData(null, null, null, gifts), forceSend: true).Forget();
			Debug.Log("egg taken");
		}
		else
		{
			switch (text)
			{
			case "LoadDesert":
				V.SceneControl.LoadDesert(immediately: false);
				break;
			case "tunnelEffect":
				_freeDriveProvider.FreeDriveControl.inTunnel();
				break;
			case "LoadMainCity":
				V.SceneControl.LoadMainCity(immediately: false);
				break;
			case "LoadCity2":
				V.SceneControl.LoadCity2(immediately: false);
				break;
			case "LoadHighway2":
				V.SceneControl.LoadHighway2(immediately: false);
				break;
			case "LoadMountain":
				V.SceneControl.LoadMountain(immediately: false);
				break;
			case "LoadIsland":
				V.SceneControl.LoadIsland(immediately: false);
				break;
			case "Richmond":
				V.SceneControl.LoadRichmond(immediately: false);
				break;
			case "LoadRoad2":
				V.SceneControl.LoadRoad2(immediately: false);
				break;
			case "loadHome2":
				V.SceneControl.LoadHome2(immediately: false);
				break;
			case "LoadRoad1":
				V.SceneControl.LoadRoad1(immediately: false);
				break;
			case "LoadStraightRoad":
				V.SceneControl.LoadStraightRoad(immediately: false);
				break;
			}
		}
		if (text == "race track")
		{
			V.SceneControl.LoadRaceTrack(immediately: false);
			return;
		}
		if (other.CompareTag("SceneSettings"))
		{
			V.SceneControl.onSceneSettingEnter(text);
			return;
		}
		switch (text)
		{
		case "citymiddle":
			V.SceneControl.OnMiddleCity();
			return;
		case "road1middle":
			V.SceneControl.OnMiddleRoad1();
			return;
		case "dessertmiddle":
			V.SceneControl.OnMiddleDesert();
			return;
		case "mointainmiddle":
			V.SceneControl.OnMiddleMountain();
			return;
		case "road2middle":
			V.SceneControl.OnMiddleRoad2();
			return;
		case "middleStraightRoad":
			V.SceneControl.OnMiddleStraightRoad();
			return;
		case "RacingTrackmiddle":
			V.SceneControl.OnMiddleRacing();
			return;
		case "action1":
			_freeDriveProvider.FreeDriveControl.ActivateCarLift();
			if ((bool)V.Car)
			{
				V.Car.GetComponent<VehicleData>().CarOnLift = true;
			}
			return;
		case "ParkingB":
			V.SceneControl.LoadParkingBuilding(immediately: false);
			return;
		case "onParking":
			V.SceneControl.ShowHideCityParts(show: false);
			V.SceneControl.SetScenePart(V.SceneControl.parkingSceneSettings);
			return;
		case "supermarket":
			other.GetComponent<superMarketBuilding>().EnableLights(enable: true);
			return;
		}
		StayInColliderLink component;
		ProceduralDoorFrontCollider component2;
		ProceduralDoorBackCollider component3;
		if (text.Contains("Teleport") && FlagsStorage.OutOfCar)
		{
			other.transform.parent.GetComponent<Teleport>().Teleported();
		}
		else if (text == "StayInColliderLink" && other.TryGetComponent<StayInColliderLink>(out component))
		{
			component.OnEnter();
		}
		else if (text == "ProceduralDoorFrontCollider" && other.TryGetComponent<ProceduralDoorFrontCollider>(out component2))
		{
			component2.Enter?.Invoke();
		}
		else if (text == "ProceduralDoorBackCollider" && other.TryGetComponent<ProceduralDoorBackCollider>(out component3))
		{
			component3.Enter?.Invoke();
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (FlagsStorage.IsPhotoMode || FlagsStorage.IgnoreTriggersAfterExit || !FlagsStorage.OutOfCar || !_levelsProvider.CurrentLevelData || _levelsProvider.CurrentLevelData.Type == LevelType.Chasing)
		{
			return;
		}
		if ((bool)_levelsProvider.LevelScript)
		{
			_levelsProvider.LevelScript.IsShowNewMessage = true;
		}
		if (!(V.DriverPerson == base.gameObject))
		{
			return;
		}
		OnTriggerMutual(other).Forget();
		if (!FlagsStorage.IsFreeDriveMode)
		{
			switch (other.name)
			{
			case "inside parking":
				_levelsProvider.LevelScript.EnableParking(isEnabled: true);
				break;
			case "outside parking":
				if (FlagsStorage.LevelStarted)
				{
					_levelsProvider.LevelScript.EnableParking(isEnabled: false);
				}
				break;
			case "show other":
				_levelsProvider.LevelScript.OtherSceneBuildings.SetActive(value: true);
				break;
			case "hide other":
				_levelsProvider.LevelScript.OtherSceneBuildings.SetActive(value: false);
				break;
			}
		}
		else
		{
			if (!(other.name == "TollP"))
			{
				return;
			}
			TallPerson component = other.GetComponent<TallPerson>();
			component.person = base.gameObject;
			if (!_freeDriveProvider.TallPerson)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("DriverActionController.YouAreWorking"), 2f);
				component.RunTimer();
				if (V.PersonCamIndex != 1)
				{
					V.PersonCamIndex = 0;
					V.CoreGameManager.GetComponent<CarCamerasController>().SetCamera(0, V.DriverPerson.transform, newTarget: true);
				}
			}
			_freeDriveProvider.TallPerson = component;
		}
	}

	public MainPerson GetMainPerson()
	{
		if (!mainPerson)
		{
			mainPerson = base.gameObject.GetComponent<MainPerson>();
		}
		return mainPerson;
	}
}
