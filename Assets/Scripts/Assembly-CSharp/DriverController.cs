using System;
using System.Globalization;
using System.Threading;
using CPM.PlayerData;
using CPMEngine;
using CarAnimationScripts;
using Car_Parking.Scripts.Racing;
using Cysharp.Threading.Tasks;
using Gley.TrafficSystem;
using Photon.Pun;
using RootMotion.FinalIK;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Person;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

public class DriverController : MonoBehaviour
{
	public bool isSpectator;

	private int lastPlaceCamPos;

	public GameObject DriverPerson;

	public GameObject DriversTarget;

	public GameObject PassangerTargets;

	public FullBodyBipedIK ik;

	public GameObject camInsideView;

	[NonSerialized]
	public bool SuppressInsideViewReturn;

	private CarCameras carCameras;

	private VehicleData _vehicleData;

	private bool _seatWhenReadyPending;

	public Vector3 rand;

	public Vector3 camPosInitialInsideView;

	public Vector3 camPosDefaultInsideView;

	public float threshold = 5000f;

	public float xRagDollForce = 1000f;

	public float yRagDollForce = 1000f;

	public float zRagDollForce = -20000f;

	[NonSerialized]
	public HandIKTarget LeftHandIKTarget;

	[NonSerialized]
	public HandIKTarget RightHandIKTarget;

	[NonSerialized]
	public SteeringWheel SteeringWheel;

	private IPrefabFactory _prefabFactory;

	private CharactersService _charactersService;

	private ICharacterEquipmentDataService _characterEquipmentDataService;

	private IDisposable _disposableUpdateRandomVector;

	private IDisposable _disposableEnableMeshes2;

	private PlayerDataStorage _playerDataStorage;

	private LevelsProvider _levelsProvider;

	public CarInputManager _carInputManager;

	private FreeTimeAttackController _freeTimeAttackController;

	private OptionalDependency<FreeTimeAttackController> _freeTimeAttackControllerDep;

	private readonly float _toDevide = 300000f;

	private static readonly WaitForSeconds _wait01 = new WaitForSeconds(0.1f);

	private Rigidbody _rigidbody;

	private float _timeCollided;

	public bool HaveSteeringWheel
	{
		get
		{
			if ((bool)SteeringWheel && SteeringWheel.HaveHandPositions && (bool)LeftHandIKTarget)
			{
				return RightHandIKTarget;
			}
			return false;
		}
	}

	[Inject]
	public void Construct(IPrefabFactory prefabFactory, CharactersService charactersService, ICharacterEquipmentDataService characterEquipmentDataService, PlayerDataStorage playerDataStorage, LevelsProvider levelsProvider, CarInputManager carInputManager, OptionalDependency<FreeTimeAttackController> freeTimeAttackController)
	{
		_levelsProvider = levelsProvider;
		_playerDataStorage = playerDataStorage;
		_characterEquipmentDataService = characterEquipmentDataService;
		_charactersService = charactersService;
		_prefabFactory = prefabFactory;
		_carInputManager = carInputManager;
		_freeTimeAttackControllerDep = freeTimeAttackController;
	}

	private void Awake()
	{
		_freeTimeAttackController = _freeTimeAttackControllerDep?.Value;
		_rigidbody = GetComponent<Rigidbody>();
		TryGetComponent<VehicleData>(out _vehicleData);
		yRagDollForce = 2000f;
		zRagDollForce = 50000f;
	}

	private void Start()
	{
		carCameras = V.CoreGameManager.GetComponent<CarCameras>();
	}

	public void SetupDriverWhenReady(GameObject person)
	{
		if ((bool)person)
		{
			if (!_vehicleData || _vehicleData.IsInitialized)
			{
				SetupDriver(person);
			}
			else if (!_seatWhenReadyPending)
			{
				_seatWhenReadyPending = true;
				_vehicleData.OnInitialized += TrySeat;
			}
		}
		void TrySeat()
		{
			if (_vehicleData.IsInitialized)
			{
				_vehicleData.OnInitialized -= TrySeat;
				_seatWhenReadyPending = false;
				SetupDriver(person);
			}
		}
	}

	public void SetupDriver(GameObject person)
	{
		if (!person)
		{
			return;
		}
		DriverPerson = person;
		MultiPersonControl component = DriverPerson.GetComponent<MultiPersonControl>();
		if (!FlagsStorage.IsMultiFree)
		{
			if (base.gameObject == V.Car || base.gameObject == _levelsProvider.RacingCarLevel)
			{
				component.currentCar = base.gameObject;
				SeatDriverIntoMembers();
			}
		}
		else if (base.gameObject == V.Car)
		{
			component.currentCar = base.gameObject;
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				SendFirstCarEnter();
			}).AddTo(this);
			component.SendCarEnter(DriverPerson, base.gameObject);
			SeatDriverIntoMembers();
			MultiPlayerPosControl component2 = GetComponent<MultiPlayerPosControl>();
			if (!component2.photonView.IsMine)
			{
				component2.updateLabel(DriverPerson, Show: true);
				component2.updateLabel(DriverPerson, Show: false);
			}
			FlagsStorage.NowPolice = _vehicleData.LightsComposition.IsPolice;
		}
		if (base.gameObject == V.Car)
		{
			V.DriverPerson = DriverPerson;
			_carInputManager.AttachVehicle(base.gameObject);
		}
		if (ScenesHelper.IsFreeDriveSceneActive() && !FlagsStorage.IsMultiFree)
		{
			V.DriverPerson.AddComponent<PlayerComponent>().enabled = false;
		}
		if (!FlagsStorage.IsMultiFree && base.gameObject == _levelsProvider.RacingCarLevel)
		{
			SeatRacingOpponentAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}
		else
		{
			PresetAfterWhenReadyAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}
	}

	private void SeatDriverIntoMembers()
	{
		AssignDriverToMembers();
		if (TryGetComponent<Powertrain>(out var component))
		{
			component.RestoreRigidbodyPhysics();
		}
	}

	private void AssignDriverToMembers()
	{
		_vehicleData.VehicleMembersController.InitIfNeeds();
		_vehicleData.VehicleMembersController.SetDriver(DriverPerson);
	}

	private void ReleaseHandPosers()
	{
		if ((bool)DriverPerson && DriverPerson.TryGetComponent<DriverActionController>(out var component))
		{
			if ((bool)component.RightHandPoser)
			{
				component.RightHandPoser.enabled = false;
			}
			if ((bool)component.LeftHandPoser)
			{
				component.LeftHandPoser.enabled = false;
			}
		}
	}

	private void PresetAfter()
	{
		if (!(base.gameObject != V.Car))
		{
			PreSetPerson(DriverPerson, 0);
			ReleaseHandPosers();
		}
	}

	private async UniTaskVoid PresetAfterWhenReadyAsync(CancellationToken ct)
	{
		await UniTask.Delay(TimeSpan.FromSeconds(0.009999999776482582), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		PresetAfter();
		MainPerson mainPerson = (DriverPerson ? DriverPerson.GetComponent<MainPerson>() : null);
		if ((bool)mainPerson && !mainPerson.ik)
		{
			await UniTask.WaitUntil(() => !mainPerson || (bool)mainPerson.ik, PlayerLoopTiming.Update, ct);
		}
		PresetAfter();
	}

	private void SendFirstCarEnter()
	{
		if (!FlagsStorage.OutOfCar && !FlagsStorage.IsDoorOpenAnimation)
		{
			DriverPerson.GetComponent<MultiPersonControl>().SendCarEnter(DriverPerson, base.gameObject);
			DriverPerson.GetComponent<PhotonView>().Synchronization = ViewSynchronization.Off;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (!_rigidbody || Time.time - _timeCollided < 1f)
		{
			return;
		}
		Vector3 vector = base.transform.InverseTransformDirection(collision.impulse);
		vector.y = 0f;
		float magnitude = vector.magnitude;
		if (magnitude <= threshold)
		{
			return;
		}
		Vector3 to = -collision.relativeVelocity;
		to.y = 0f;
		if ((_freeTimeAttackController != null && _freeTimeAttackController.IsRaceInProgress()) || ScenesHelper.IsDragRacingSceneActive() || ScenesHelper.IsLapRacingSceneActive())
		{
			return;
		}
		float x = 0f;
		if (to.sqrMagnitude > 1E-06f)
		{
			x = Vector3.SignedAngle(base.transform.forward, to, Vector3.up);
		}
		_timeCollided = Time.time;
		float num = (0f - vector.z) / _toDevide;
		float num2 = vector.x / _toDevide;
		float num3 = UnityEngine.Random.Range(0.8f, 1.2f);
		foreach (GameObject member in _vehicleData.VehicleMembersController.Members)
		{
			if ((bool)member)
			{
				DriverActionController component = member.GetComponent<DriverActionController>();
				if ((bool)component)
				{
					component.TriggerCrashShake(num2, num * num3);
				}
				if (MathF.Abs(x) < 30f && magnitude > 80000f)
				{
					OnRagdoll(member, new Vector3(x, num2, num)).Forget();
				}
			}
		}
	}

	private async UniTaskVoid OnRagdoll(GameObject person, Vector3 forceInput)
	{
		Vector3 force2 = new Vector3(forceInput.x, forceInput.y * yRagDollForce, forceInput.z * zRagDollForce);
		await UniTask.Delay(10);
		if (!person || !person.TryGetComponent<DriverActionController>(out var component))
		{
			return;
		}
		RagdollCPM ragdollCpm = component.ragdollCpm;
		if ((bool)ragdollCpm)
		{
			if (PhotonNetwork.IsConnected)
			{
				int viewID = person.GetComponent<PhotonView>().ViewID;
				string text = force2.x.ToString("N0", CultureInfo.InvariantCulture) + "?" + force2.y.ToString("N0", CultureInfo.InvariantCulture) + "?" + force2.z.ToString("N0", CultureInfo.InvariantCulture);
				Vector3 vector = person.transform.localPosition * 10f;
				text = text + "?" + vector.x.ToString("N0", CultureInfo.InvariantCulture) + "?" + vector.y.ToString("N0", CultureInfo.InvariantCulture) + "?" + vector.z.ToString("N0", CultureInfo.InvariantCulture);
				text = text.Replace(",", "");
				V.CoreGameManager.FreeDriveProvider.MultiPlayer.SendValueStr(59, viewID.ToString(), text);
			}
			ragdollCpm.RagdollEnabler(force2).Forget();
		}
	}

	public void PreSetPerson(GameObject DriverPerson, int placeToSit)
	{
		if (!DriverPerson)
		{
			return;
		}
		DriverPerson.transform.parent = base.gameObject.transform;
		DriverPerson.transform.localPosition = Vector3.zero;
		DriverActionController component = DriverPerson.GetComponent<DriverActionController>();
		component.Init(base.gameObject, _vehicleData, this);
		component.isPassenger = placeToSit != 0;
		MainPerson component2 = DriverPerson.GetComponent<MainPerson>();
		if (DriverPerson.TryGetComponent<Rigidbody>(out var component3))
		{
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.CarOutInControl)
			{
				V.CoreGameManager.CarOutInControl.SetPersonKinematicWithIgnoreTrigger(component3, !FlagsStorage.OutOfCar).Forget();
			}
			else
			{
				component3.isKinematic = true;
			}
		}
		if ((bool)component2)
		{
			ik = component2.ik;
			component.ik = ik;
			ApplyInCarOutfit(component2);
		}
		if (TryGetComponent<VehicleData>(out var component4))
		{
			component.RightHanded = component4.RightHanded;
		}
		if ((bool)ik)
		{
			ik.solver.iterations = 2;
			SetToSeatPersonPoints(DriverPerson, placeToSit);
			SetCamHeadPoint(DriverPerson, placeToSit);
		}
		bool flag = DriverPerson.TryGetComponent<MultiPersonControl>(out var _);
		if (flag)
		{
			DriverPerson.transform.parent = null;
		}
		if (FlagsStorage.IsFreeDriveMode & flag)
		{
			DriverPerson.transform.parent = V.CoreGameManager.globalPoint.transform;
		}
		if ((bool)ik)
		{
			EnablePersonMesh();
		}
	}

	public void ApplyInCarOutfit(MainPerson mainPerson)
	{
		if (!mainPerson || mainPerson.allKits == null)
		{
			return;
		}
		bool flag = (bool)_vehicleData && CarIdStorage.F1Cars.Contains(_vehicleData.CarId);
		if ((bool)mainPerson.mainBodySkin)
		{
			mainPerson.mainBodySkin.shadowCastingMode = ShadowCastingMode.Off;
		}
		if ((bool)mainPerson.CombinedBodySkin)
		{
			mainPerson.CombinedBodySkin.shadowCastingMode = ShadowCastingMode.Off;
		}
		if ((bool)mainPerson.allKits[7])
		{
			mainPerson.allKits[7].SetActive(value: false);
		}
		bool flag2 = false;
		if ((bool)mainPerson.allKits[3] && !flag && PersonElements.CapNeedHideInCar(mainPerson.Gender, mainPerson.allKits[3].GetComponent<Equipment>().id))
		{
			mainPerson.allKits[3].SetActive(value: false);
			flag2 = true;
		}
		if (flag2 && (bool)mainPerson.allKits[0])
		{
			mainPerson.allKits[0].SetActive(value: true);
		}
		if (flag2 && (bool)mainPerson.allKits[2] && mainPerson.allKits[3].GetComponent<Equipment>().HideBeard && (!mainPerson.allKits[4] || !mainPerson.allKits[4].GetComponent<Equipment>().HideBeard))
		{
			mainPerson.allKits[2].SetActive(value: true);
		}
		if ((int)mainPerson.Gender != 1 || !mainPerson.allKits[0] || flag)
		{
			return;
		}
		Equipment component = mainPerson.allKits[0].GetComponent<Equipment>();
		int id = component.id;
		if (!mainPerson.HairsToIgnore.Contains(id))
		{
			return;
		}
		try
		{
			SkinnedMeshRenderer component2 = mainPerson.allKits[0].GetComponent<SkinnedMeshRenderer>();
			component2.sharedMesh = component.defaultMesh;
			component2.materials = component.defaultMats;
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}

	public static void RestoreOutOfCarOutfit(MainPerson mainPerson)
	{
		if (!mainPerson || mainPerson.allKits == null)
		{
			return;
		}
		if ((bool)mainPerson.mainBodySkin)
		{
			mainPerson.mainBodySkin.shadowCastingMode = ShadowCastingMode.On;
		}
		if ((bool)mainPerson.CombinedBodySkin)
		{
			mainPerson.CombinedBodySkin.shadowCastingMode = ShadowCastingMode.On;
		}
		if ((bool)mainPerson.allKits[7])
		{
			mainPerson.allKits[7].SetActive(value: true);
		}
		if ((bool)mainPerson.allKits[3] && PersonElements.CapNeedHideInCar(mainPerson.Gender, mainPerson.allKits[3].GetComponent<Equipment>().id))
		{
			mainPerson.allKits[3].SetActive(value: true);
			if (mainPerson.HairHiddenByHat && (bool)mainPerson.allKits[0])
			{
				mainPerson.allKits[0].SetActive(value: false);
			}
			if ((bool)mainPerson.allKits[2] && mainPerson.allKits[3].GetComponent<Equipment>().HideBeard)
			{
				mainPerson.allKits[2].SetActive(value: false);
			}
		}
		if ((int)mainPerson.Gender != 1 || !mainPerson.allKits[0])
		{
			return;
		}
		Equipment component = mainPerson.allKits[0].GetComponent<Equipment>();
		int id = component.id;
		if (!mainPerson.HairsToIgnore.Contains(id))
		{
			return;
		}
		try
		{
			SkinnedMeshRenderer component2 = mainPerson.allKits[0].GetComponent<SkinnedMeshRenderer>();
			component2.sharedMesh = component.originalMesh;
			component2.materials = component.originalMats;
		}
		catch (Exception message)
		{
			Debug.LogWarning(message);
		}
	}

	private void EnableMeshesAndUnsubscribe()
	{
		_vehicleData.OnCarGraphicLoaded -= EnableMeshesAndUnsubscribe;
		EnableMeshes();
	}

	private void EnablePersonMesh()
	{
		if (_vehicleData.IsCarGraphicLoaded)
		{
			EnableMeshes();
		}
		else
		{
			_vehicleData.OnCarGraphicLoaded += EnableMeshesAndUnsubscribe;
		}
	}

	private void OnDestroy()
	{
		_disposableEnableMeshes2?.Dispose();
		if ((bool)_vehicleData)
		{
			_vehicleData.OnCarGraphicLoaded -= ApplyInCarScaleToMembersAndUnsubscribe;
		}
	}

	private void EnableMeshes()
	{
		_disposableEnableMeshes2?.Dispose();
		_disposableEnableMeshes2 = Observable.Timer(TimeSpan.FromSeconds(0.05000000074505806)).Subscribe((long _) =>
		{
			EnableMeshes2();
		}).AddTo(this);
	}

	private void EnableMeshes2()
	{
		SmoothLoadCarInMenu.SkinPersonRendererEnabler(enable: true);
	}

	public void SetToSeatPersonPoints(GameObject DriverPerson, int place)
	{
		DriverActionController component = DriverPerson.GetComponent<DriverActionController>();
		_vehicleData.allPersonPoints.CreatePersonTargets(place);
		TargetPositionRecorder targetPositionRecorder = _vehicleData.allPersonPoints.allTargets[place];
		if (_vehicleData.allPersonPoints.editMode)
		{
			targetPositionRecorder = _vehicleData.allPersonPoints.mainTPR;
		}
		MultiPersonControl component2 = DriverPerson.GetComponent<MultiPersonControl>();
		MainPerson component3 = DriverPerson.GetComponent<MainPerson>();
		if ((bool)component3)
		{
			ik = component3.ik;
		}
		else
		{
			ik = component.ik;
		}
		if ((bool)component)
		{
			component.Shifter = null;
		}
		HandPoser leftHandPoser = SetupHandEffector(ik.solver.leftHandEffector, targetPositionRecorder.LHand.transform);
		HandPoser rightHandPoser = SetupHandEffector(ik.solver.rightHandEffector, targetPositionRecorder.RHand.transform);
		if ((bool)component)
		{
			component.LeftHandPoser = leftHandPoser;
			component.LeftHandPoser.enabled = false;
			component.RightHandPoser = rightHandPoser;
			component.RightHandPoser.enabled = false;
		}
		SetEffector(ik.solver.rightFootEffector, targetPositionRecorder.RFoot.transform);
		SetEffector(ik.solver.leftFootEffector, targetPositionRecorder.LFoot.transform);
		SetEffector(ik.solver.bodyEffector, targetPositionRecorder.mainBody.transform);
		ik.solver.leftShoulderEffector.target = targetPositionRecorder.LUpperArm.transform;
		ik.solver.leftShoulderEffector.positionWeight = 1f;
		ik.solver.rightShoulderEffector.target = targetPositionRecorder.RUpperArm.transform;
		ik.solver.rightShoulderEffector.positionWeight = 1f;
		if ((bool)component)
		{
			component.Shifter = targetPositionRecorder.Shifter.transform;
		}
		SetBendGoal(ik.solver.leftArmChain.bendConstraint, targetPositionRecorder.LBendGoal.transform);
		SetBendGoal(ik.solver.rightArmChain.bendConstraint, targetPositionRecorder.RBendGoal.transform);
		SetBendGoal(ik.solver.leftLegChain.bendConstraint, targetPositionRecorder.LLegBendGoal.transform);
		SetBendGoal(ik.solver.rightLegChain.bendConstraint, targetPositionRecorder.RLegBendGoal.transform);
		if ((bool)targetPositionRecorder.Head && (bool)component2)
		{
			component2.IkHeadPoint = targetPositionRecorder.Head.transform;
		}
		if (component2 != null)
		{
			Transform parent = targetPositionRecorder.mainBody.transform.parent;
			targetPositionRecorder.mainBody.transform.parent = base.gameObject.transform;
			component2.localCarPos = targetPositionRecorder.mainBody.transform.localPosition - new Vector3(0f, 2f, -1f);
			targetPositionRecorder.mainBody.transform.parent = parent;
		}
		if (place == 0 && !_vehicleData.allPersonPoints.editMode)
		{
			_vehicleData.InteriorsComposition.SetHandsToCurrentInteriorWheel();
		}
		ApplyInCarScale(DriverPerson);
	}

	private void ApplyInCarScale(GameObject person)
	{
		if (!person)
		{
			return;
		}
		CarAnimationReferences carAnimationReferences = (TryGetComponent<CarAnimationsController>(out var component) ? component.CarAnimationReferences : null);
		if ((bool)carAnimationReferences)
		{
			person.transform.localScale = carAnimationReferences.SizePlayerInCar;
			return;
		}
		person.transform.localScale = new Vector3(1.1f, 1.1f, 1.2f);
		if ((bool)_vehicleData && !_vehicleData.IsCarGraphicLoaded)
		{
			_vehicleData.OnCarGraphicLoaded -= ApplyInCarScaleToMembersAndUnsubscribe;
			_vehicleData.OnCarGraphicLoaded += ApplyInCarScaleToMembersAndUnsubscribe;
		}
	}

	private void ApplyInCarScaleToMembersAndUnsubscribe()
	{
		_vehicleData.OnCarGraphicLoaded -= ApplyInCarScaleToMembersAndUnsubscribe;
		VehicleMembersController vehicleMembersController = _vehicleData.VehicleMembersController;
		if (vehicleMembersController != null)
		{
			for (int i = 0; i < vehicleMembersController.Members.Count; i++)
			{
				ApplyInCarScale(vehicleMembersController.Members[i]);
			}
		}
	}

	private HandPoser SetupHandEffector(IKEffector effector, Transform target)
	{
		SetEffector(effector, target);
		GameObject gameObject = effector.bone.gameObject;
		if (!gameObject.GetComponent<HandPoser>())
		{
			gameObject.AddComponent<HandPoser>();
		}
		HandPoser component = gameObject.GetComponent<HandPoser>();
		component.poseRoot = target;
		return component;
	}

	private void SetEffector(IKEffector effector, Transform target)
	{
		effector.target = target;
		effector.positionWeight = 1f;
		effector.rotationWeight = 1f;
	}

	private void SetBendGoal(IKConstraintBend bendConstraint, Transform goal)
	{
		bendConstraint.bendGoal = goal;
		bendConstraint.weight = 1f;
	}

	public void CreateStearingWheelHand(TargetPositionRecorder tpr, GameObject person)
	{
		if ((bool)_vehicleData.InteriorsComposition.SteeringWheelParent)
		{
			if ((bool)LeftHandIKTarget)
			{
				UnityEngine.Object.Destroy(LeftHandIKTarget.gameObject);
			}
			if ((bool)RightHandIKTarget)
			{
				UnityEngine.Object.Destroy(RightHandIKTarget.gameObject);
			}
			if ((bool)tpr.LHand)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(tpr.LHand);
				gameObject.transform.parent = tpr.LHand.transform.parent.transform;
				gameObject.transform.position = tpr.LHand.transform.position;
				gameObject.transform.eulerAngles = tpr.LHand.transform.eulerAngles;
				gameObject.transform.parent = _vehicleData.InteriorsComposition.SteeringWheelParent.transform;
				LeftHandIKTarget = gameObject.AddComponent<HandIKTarget>();
			}
			if ((bool)tpr.RHand)
			{
				GameObject gameObject2 = UnityEngine.Object.Instantiate(tpr.RHand);
				gameObject2.transform.parent = tpr.RHand.transform.parent.transform;
				gameObject2.transform.position = tpr.RHand.transform.position;
				gameObject2.transform.eulerAngles = tpr.RHand.transform.eulerAngles;
				gameObject2.transform.parent = _vehicleData.InteriorsComposition.SteeringWheelParent.transform;
				RightHandIKTarget = gameObject2.AddComponent<HandIKTarget>();
			}
			SteeringWheel = _vehicleData.InteriorsComposition.SteeringWheelParent.GetComponent<SteeringWheel>();
			if ((bool)SteeringWheel && (bool)LeftHandIKTarget && (bool)RightHandIKTarget)
			{
				SteeringWheel.InitHands(LeftHandIKTarget, RightHandIKTarget);
			}
			if (person.TryGetComponent<DriverActionController>(out var component))
			{
				component.GripWheel();
			}
		}
	}

	public void SetHandWheelToSteeringWheel(GameObject person)
	{
		if (!person)
		{
			return;
		}
		person.TryGetComponent<DriverActionController>(out var component);
		if (((bool)component && component.isPassenger) || !person || !_vehicleData.InteriorsComposition.SteeringWheelParent || !_vehicleData.allPersonPoints)
		{
			return;
		}
		if (_vehicleData.allPersonPoints.allTargets.Length != 0 && _vehicleData.allPersonPoints.allTargets[0] != null)
		{
			CreateStearingWheelHand(_vehicleData.allPersonPoints.allTargets[0], person);
		}
		DriverActionController component2 = person.GetComponent<DriverActionController>();
		FullBodyBipedIK fullBodyBipedIK = person.GetComponent<MainPerson>().ik;
		if ((bool)component2 && !(fullBodyBipedIK == null))
		{
			if ((bool)RightHandIKTarget && (bool)component2.RightHandPoser)
			{
				fullBodyBipedIK.solver.rightHandEffector.target = RightHandIKTarget.transform;
				component2.RightHandPoser.enabled = true;
			}
			if ((bool)LeftHandIKTarget && (bool)component2.LeftHandPoser)
			{
				fullBodyBipedIK.solver.leftHandEffector.target = LeftHandIKTarget.transform;
				component2.LeftHandPoser.enabled = true;
			}
			component2.GripWheel();
		}
	}

	public void RestoreHandsToSeatPoints()
	{
		GameObject gameObject = (_vehicleData ? _vehicleData.VehicleMembersController.Driver : null);
		if (!gameObject || !_vehicleData || !_vehicleData.allPersonPoints || (gameObject.TryGetComponent<DriverActionController>(out var component) && component.isPassenger) || _vehicleData.allPersonPoints.allTargets == null || _vehicleData.allPersonPoints.allTargets.Length == 0)
		{
			return;
		}
		TargetPositionRecorder targetPositionRecorder = _vehicleData.allPersonPoints.allTargets[0];
		if (targetPositionRecorder != null && gameObject.TryGetComponent<MainPerson>(out var component2) && (bool)component2.ik)
		{
			if ((bool)targetPositionRecorder.LHand)
			{
				component2.ik.solver.leftHandEffector.target = targetPositionRecorder.LHand.transform;
			}
			if ((bool)targetPositionRecorder.RHand)
			{
				component2.ik.solver.rightHandEffector.target = targetPositionRecorder.RHand.transform;
			}
			if ((bool)LeftHandIKTarget)
			{
				UnityEngine.Object.Destroy(LeftHandIKTarget.gameObject);
			}
			if ((bool)RightHandIKTarget)
			{
				UnityEngine.Object.Destroy(RightHandIKTarget.gameObject);
			}
			LeftHandIKTarget = null;
			RightHandIKTarget = null;
			SteeringWheel = null;
		}
	}

	public void SetCamHeadPoint(GameObject DriverPerson, int place, bool skipUSerCheck = false)
	{
		if (DriverPerson != V.DriverPerson && !skipUSerCheck)
		{
			return;
		}
		FullBodyBipedIK fullBodyBipedIK = DriverPerson.GetComponent<MainPerson>().ik;
		DriverActionController component = DriverPerson.GetComponent<DriverActionController>();
		if (place != lastPlaceCamPos && (bool)camInsideView)
		{
			UnityEngine.Object.Destroy(camInsideView);
			camInsideView = null;
			lastPlaceCamPos = place;
		}
		if (!camInsideView)
		{
			camInsideView = new GameObject();
		}
		if ((bool)fullBodyBipedIK)
		{
			if ((bool)component)
			{
				component.UpdateIK();
			}
			camInsideView.transform.position = fullBodyBipedIK.references.head.position;
			camInsideView.transform.parent = base.gameObject.transform;
			camInsideView.transform.localEulerAngles = Vector3.zero;
		}
		Vector3 localPosition = camInsideView.transform.localPosition;
		camInsideView.transform.localPosition = new Vector3(localPosition.x, localPosition.y + 0.1f, localPosition.z + 0.15f);
		camInsideView.transform.localPosition = new Vector3(localPosition.x, localPosition.y + 0.1f, localPosition.z);
		camInsideView.name = "driverViewCam";
		camPosInitialInsideView = camInsideView.transform.localPosition;
		camPosDefaultInsideView = camPosInitialInsideView;
		_disposableUpdateRandomVector?.Dispose();
		_disposableUpdateRandomVector = Observable.Timer(TimeSpan.FromSeconds(1.0)).Concat(Observable.Interval(TimeSpan.FromSeconds(1.0))).Subscribe((long _) =>
		{
			UpdateRandomVector();
		})
			.AddTo(this);
	}

	public async UniTask presetAfterRacingPerson(CancellationToken ct = default(CancellationToken))
	{
		await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
		MainPerson driverPersonMain = (DriverPerson ? DriverPerson.GetComponent<MainPerson>() : null);
		if ((bool)driverPersonMain && !driverPersonMain.ik)
		{
			await UniTask.WaitUntil(() => !driverPersonMain || (bool)driverPersonMain.ik, PlayerLoopTiming.Update, ct);
		}
		PreSetPerson(DriverPerson, 0);
		if (base.gameObject == V.Car)
		{
			ReleaseHandPosers();
		}
		if ((bool)DriverPerson && DriverPerson.TryGetComponent<MainPerson>(out var component))
		{
			if ((bool)component.ik)
			{
				component.ik.enabled = true;
			}
			component.lastCar = base.gameObject;
			Animator animator = (component.racingPerson ? component.Animator : component.PersonAnimationsController.Animator);
			if ((bool)animator)
			{
				animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
			}
		}
		if ((bool)DriverPerson && DriverPerson.TryGetComponent<MultiPersonControl>(out var component2))
		{
			component2.currentCar = base.gameObject;
		}
		AssignDriverToMembers();
	}

	private async UniTask SeatRacingOpponentAsync(CancellationToken ct)
	{
		await UniTask.DelayFrame(2, PlayerLoopTiming.Update, ct).SuppressCancellationThrow();
		if (!ct.IsCancellationRequested && (bool)DriverPerson)
		{
			if (DriverPerson.TryGetComponent<MainPerson>(out var component))
			{
				component.racingPerson = true;
			}
			if (DriverPerson.TryGetComponent<DriverActionController>(out var component2))
			{
				component2.EnableDisableAllColliders(WillEnable: false);
			}
			await presetAfterRacingPerson(ct);
		}
	}

	private void UpdateRandomVector()
	{
		rand = new Vector3(UnityEngine.Random.Range(-0.02f, 0.02f), UnityEngine.Random.Range(-0.02f, 0.02f), UnityEngine.Random.Range(-0.02f, 0.02f));
	}

	private void Update()
	{
		if ((bool)carCameras && carCameras.driverView && (bool)camInsideView && V.Car == base.gameObject && !SuppressInsideViewReturn)
		{
			Vector3 localPosition = camInsideView.transform.localPosition;
			camInsideView.transform.localPosition = Vector3.Lerp(localPosition, camPosInitialInsideView + rand, Time.deltaTime * 0.2f);
		}
		if (base.gameObject.activeSelf)
		{
			_vehicleData.VehicleMembersController.SetActiveMember(isActive: true);
		}
	}
}
