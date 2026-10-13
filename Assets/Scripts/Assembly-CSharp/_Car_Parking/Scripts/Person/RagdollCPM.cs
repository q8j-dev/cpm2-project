using System;
using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using RootMotion.FinalIK;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;
using _Car_Parking.Scripts.SOAP;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.ToRoad;
using _Car_Parking.Scripts.WindowSystem;
using _Car_Parking.Scripts.WorldInteraction;

namespace _Car_Parking.Scripts.Person
{
	public class RagdollCPM : MonoBehaviour
	{
		private const float DefaultFixedDeltaTime = 0.02f;

		private const float StandUpTime = 0.75f;

		private const float GroundProbeDistance = 1.2f;

		public bool isRagdoll;

		public bool isStadningUp;

		public bool waitingFinalAnimation;

		public DriverActionController driverActionController;

		public GameObject person;

		public RagdollUtility.Rigidbone[] rigidbones = new RagdollUtility.Rigidbone[0];

		public Rigidbody mainRigidbody;

		public ChildData[] children = new ChildData[0];

		public ChildData[] legs = new ChildData[0];

		public bool enableRagdollFlag;

		public bool followPositionOnline = true;

		public float ragdollStartTime;

		public bool onlinePerson;

		public List<GameObject> objectsToDisable = new List<GameObject>();

		public Vector3 posPerson;

		public float angle;

		public float distance;

		public float standingUpTime;

		public float speedStandUp = 3f;

		public float minRagdollTime = 1.5f;

		public Transform hips;

		public Vector3 offset;

		private Vector3 _localOffset;

		public Transform oldParent;

		public Vector3 scaleHips;

		public float forceAdjust = 1.5f;

		public float Up = 1f;

		public Vector3 angleOnImpace = new Vector3(53f, 0f, 0f);

		public string soundEvent = "Play_P_Voice_Vocalization";

		private readonly uint _genderSwitch = AudioWrapper.GetId("P_Voice_Gender");

		private readonly uint _maleSwitch = AudioWrapper.GetId("male");

		private readonly uint _femaleSwitch = AudioWrapper.GetId("female");

		private readonly uint _stateVoiceSwitch = AudioWrapper.GetId("P_Voice_Vocalizations");

		private readonly uint _stateGruntVoiceSwitch = AudioWrapper.GetId("Grunt");

		private readonly uint _stateCheerVoiceSwitch = AudioWrapper.GetId("Cheering");

		private readonly uint _stateHappyVoiceSwitch = AudioWrapper.GetId("Shout_Happy");

		private readonly float _timeToStartSetPos = 0.1f;

		private CarInputManager _carInputManager;

		private PhotoModeObserver _photoModeObserver;

		private bool _simulatePhysicForRagdoll;

		private int _counterFrames;

		private Animator _animator;

		private Vector3 _prevPos;

		private Rigidbody _personRb;

		private readonly int[] _animationsToPlay = new int[5] { 34, 35, 2, 3, 7 };

		public float createTime;

		private bool _startingFixAnim;

		private OptionalDependency<InterestGroupManager> _interestGroupManager;

		private const float StuckInAirToRoadSeconds = 10f;

		private InputMapSetEvent _inputMapSetEvent;

		private EventRegistry _eventRegistry;

		private RegistryHandle<ScriptableEvent> _toRoadHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ToRoad);

		private float _lastGroundedTime;

		public bool CanStandUpFromInput
		{
			get
			{
				if (isRagdoll)
				{
					if (Time.time - ragdollStartTime >= minRagdollTime)
					{
						return IsBodyGrounded();
					}
					return false;
				}
				return true;
			}
		}

		private static bool IsArenaMode
		{
			get
			{
				if ((bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null)
				{
					return V.CoreGameManager.FreeDriveProvider.RoomArenaId == ArenaId.ObstacleRace_1;
				}
				return false;
			}
		}

		private bool IsSeatedInCar
		{
			get
			{
				if (!onlinePerson)
				{
					return !FlagsStorage.OutOfCar;
				}
				if ((bool)driverActionController.multiPersonControl)
				{
					return driverActionController.multiPersonControl.currentCar;
				}
				return false;
			}
		}

		public bool StartedInCar { get; private set; }

		public static event Action LocalRagdollStarted;

		private void Start()
		{
			followPositionOnline = true;
			person = base.transform.parent.gameObject;
			driverActionController = person.GetComponent<DriverActionController>();
			onlinePerson = V.DriverPerson != person;
			_animator = person.GetComponent<Animator>();
			angleOnImpace = new Vector3(70f, 0f, 0f);
			forceAdjust = 1.5f;
			base.enabled = false;
		}

		[Inject]
		public void Construct(OptionalDependency<InterestGroupManager> interestGroupManager)
		{
			_interestGroupManager = interestGroupManager;
		}

		[Inject]
		public void ConstructInput(InputMapSetEvent inputMapSetEvent, EventRegistry eventRegistry)
		{
			_inputMapSetEvent = inputMapSetEvent;
			_eventRegistry = eventRegistry;
		}

		private void ToRoadIfStuckInAir()
		{
			if (IsBodyGrounded())
			{
				_lastGroundedTime = Time.time;
			}
			else if (!(Time.time - _lastGroundedTime < 10f))
			{
				_lastGroundedTime = Time.time;
				_toRoadHandle.Get(_eventRegistry)?.Raise();
			}
		}

		private void SetRagdollNoInput()
		{
			if (!onlinePerson)
			{
				_inputMapSetEvent?.TrySetGroup(InputGroupsEnum.NoInput, setRoot: true);
			}
		}

		private void RestorePersonInput()
		{
			if (!onlinePerson)
			{
				_inputMapSetEvent?.TrySetGroup(InputGroupsEnum.Person, setRoot: true);
			}
		}

		private async UniTask RestorePersonInputWhenGrounded(CancellationToken ct)
		{
			if (onlinePerson)
			{
				return;
			}
			float startTime = Time.time;
			while (Time.time - startTime < 1f)
			{
				await UniTask.Yield(PlayerLoopTiming.Update, ct);
				if (ct.IsCancellationRequested || !isRagdoll)
				{
					return;
				}
				if (Time.time - startTime >= 0.15f && (bool)mainRigidbody && mainRigidbody.velocity.magnitude < 1.5f)
				{
					break;
				}
			}
			RestorePersonInput();
		}

		public void Init(DriverActionController driverActionController, PhotoModeObserver photoModeObserver, CarInputManager carInputManager)
		{
			this.driverActionController = driverActionController;
			_photoModeObserver = photoModeObserver;
			if (_photoModeObserver != null)
			{
				_photoModeObserver.OnPhotoModeClosed += PhotoModeClosedHandler;
			}
			_carInputManager = carInputManager;
		}

		private bool IsBodyGrounded()
		{
			if (!hips)
			{
				return true;
			}
			int layerMask = ~LayerMask.GetMask("DriverPerson", "pedestrain", "Ignore Raycast");
			return Physics.Raycast(hips.position, Vector3.down, 1.2f, layerMask, QueryTriggerInteraction.Ignore);
		}

		public static bool IsDriverRagdolled()
		{
			if (!V.DriverPerson)
			{
				return false;
			}
			DriverActionController component = V.DriverPerson.GetComponent<DriverActionController>();
			RagdollCPM ragdollCPM = (component ? component.ragdollCpm : null);
			if ((bool)ragdollCPM)
			{
				if (!ragdollCPM.isRagdoll)
				{
					return ragdollCPM.isStadningUp;
				}
				return true;
			}
			return false;
		}

		public void StoreLocalState()
		{
			ChildData[] array = children;
			for (int i = 0; i < array.Length; i++)
			{
				array[i]?.StoreLocalState();
			}
		}

		private Transform FindDeep(Transform root, string name)
		{
			Transform[] componentsInChildren = root.GetComponentsInChildren<Transform>(includeInactive: true);
			foreach (Transform transform in componentsInChildren)
			{
				if (transform.name == name)
				{
					return transform;
				}
			}
			return null;
		}

		public void FillLocalState()
		{
			ChildData[] array = children;
			foreach (ChildData childData in array)
			{
				childData.t = FindDeep(base.transform, childData.nameTemp);
			}
		}

		private void Update()
		{
			if (isStadningUp)
			{
				if ((!followPositionOnline || !onlinePerson) && Time.timeScale != 0f)
				{
					person.transform.position = Vector3.Lerp(person.transform.position, posPerson, Time.deltaTime * speedStandUp);
					SyncPersonBody();
				}
			}
			else
			{
				if (!isRagdoll || !hips)
				{
					return;
				}
				if (Time.frameCount % 10 == 0 && V.DriverPerson == person && isRagdoll)
				{
					if (person.transform.position.y < -100f)
					{
						DisableRagdoll(immediately: true);
						V.CoreGameManager.FreeDriveProvider.FreeDriveControl.ToRoad(ToRoadSource.FellThroughWorld);
						return;
					}
					ToRoadIfStuckInAir();
				}
				if (onlinePerson && followPositionOnline)
				{
					if (Time.time - ragdollStartTime > _timeToStartSetPos && (bool)hips)
					{
						Vector3 vector = (hips.parent ? hips.parent.InverseTransformPoint(person.transform.position) : person.transform.position);
						hips.localPosition = vector - offset + _localOffset;
					}
				}
				else if ((bool)hips)
				{
					person.transform.localPosition = hips.localPosition + offset - _localOffset;
					SyncPersonBody();
				}
				if (_simulatePhysicForRagdoll)
				{
					StepRagdollPhysics();
				}
			}
		}

		private void SyncPersonBody()
		{
			if (!_personRb && (bool)person)
			{
				_personRb = person.GetComponent<Rigidbody>();
			}
			if ((bool)_personRb)
			{
				_personRb.position = person.transform.position;
				_personRb.rotation = person.transform.rotation;
			}
		}

		public void ReleaseForTeleport()
		{
			if (isRagdoll)
			{
				DisableRagdoll(immediately: true);
			}
			isStadningUp = false;
		}

		public void DisableRagdoll(bool immediately = false)
		{
			if (isRagdoll && (!isStadningUp || immediately))
			{
				DisableRagdollSmooth(base.destroyCancellationToken, immediately).Forget();
			}
		}

		private async UniTask DisableRagdollSmooth(CancellationToken cancellationToken, bool immediately = false)
		{
			if (!driverActionController || !driverActionController.ik)
			{
				return;
			}
			distance = 10f;
			_counterFrames = 0;
			for (int i = 0; i < rigidbones.Length; i++)
			{
				rigidbones[i].r.isKinematic = true;
				rigidbones[i].collider.enabled = false;
			}
			_simulatePhysicForRagdoll = false;
			if (!hips)
			{
				return;
			}
			hips.transform.parent = oldParent;
			hips.localScale = scaleHips;
			Vector3 origin = hips.transform.position + Vector3.up * 3f;
			hips = null;
			if (Physics.Raycast(origin, Vector3.down, out var hitInfo, float.PositiveInfinity))
			{
				posPerson = hitInfo.point;
			}
			else
			{
				immediately = true;
			}
			angle = 10f;
			if (!immediately)
			{
				isStadningUp = true;
				StoreLocalState();
			}
			else
			{
				isStadningUp = false;
			}
			if (person == V.DriverPerson && PhotonNetwork.IsConnected)
			{
				int viewID = person.GetComponent<PhotonView>().ViewID;
				V.CoreGameManager.FreeDriveProvider.MultiPlayer.SendValueStr(60, viewID.ToString(), "");
			}
			if (!immediately)
			{
				waitingFinalAnimation = true;
				standingUpTime = 0f;
				_animator.enabled = true;
				_animator.Update(0f);
				_animator.Rebind();
				while (isStadningUp)
				{
					await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
				}
				if (!IsArenaMode)
				{
					driverActionController.GetComponent<NewMoveController>().Input.Emotion = _animationsToPlay[UnityEngine.Random.Range(0, _animationsToPlay.Length)];
					await UniTask.Delay(500, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
					MultiPersonControl component = driverActionController.GetComponent<MultiPersonControl>();
					if (!onlinePerson || (_interestGroupManager.HasValue && (bool)component && _interestGroupManager.Value.IsSubscribedToInterestGroup(component.InterestGroup)))
					{
						ApplyGenderSwitch();
						AudioWrapper.SetSwitch(_stateVoiceSwitch, _stateCheerVoiceSwitch, base.gameObject);
						AudioWrapper.PostEvent(soundEvent, base.gameObject);
					}
					await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}
				}
				waitingFinalAnimation = false;
			}
			else
			{
				person.transform.position = posPerson;
				SyncPersonBody();
				_animator.enabled = true;
				_animator.Update(0f);
				_animator.Rebind();
			}
			_animator.enabled = true;
			driverActionController.allColliders[0].enabled = true;
			driverActionController.ik.fixTransforms = true;
			isRagdoll = false;
			isStadningUp = false;
			Rigidbody component2 = person.GetComponent<Rigidbody>();
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.CarOutInControl)
			{
				V.CoreGameManager.CarOutInControl.SetPersonKinematicWithIgnoreTrigger(component2, isKinematic: false).Forget();
			}
			else
			{
				component2.isKinematic = false;
			}
			component2.velocity = Vector3.zero;
			driverActionController.ik.solver.IKPositionWeight = 1f;
			_animator.updateMode = AnimatorUpdateMode.Normal;
			if (onlinePerson)
			{
				_animator.applyRootMotion = false;
			}
			Vector3 localEulerAngles = person.transform.localEulerAngles;
			localEulerAngles.x = 0f;
			localEulerAngles.z = 0f;
			person.transform.localEulerAngles = localEulerAngles;
			SyncPersonBody();
			RestorePersonInput();
		}

		private void LateUpdate()
		{
			if (isStadningUp)
			{
				Transform pelvis = driverActionController.ik.references.pelvis;
				Quaternion localRotation = children[2].localRotation;
				if (_counterFrames < 20)
				{
					ChildData[] array = legs;
					foreach (ChildData childData in array)
					{
						childData.t.localPosition = childData.localPosition;
						childData.t.localRotation = childData.localRotation;
					}
					_counterFrames++;
				}
				if (Time.timeScale != 0f)
				{
					ChildData[] array = children;
					foreach (ChildData childData2 in array)
					{
						childData2.t.localPosition = Vector3.Lerp(childData2.localPosition, childData2.t.localPosition, standingUpTime / 0.75f);
						childData2.t.localRotation = Quaternion.Lerp(childData2.localRotation, childData2.t.localRotation, standingUpTime / 0.75f);
					}
				}
				angle = Quaternion.Angle(pelvis.localRotation, localRotation);
				distance = Vector3.Distance(children[2].t.localPosition, children[2].localPosition);
				isStadningUp = distance > 0.1f || angle > 10f;
				standingUpTime += Time.deltaTime;
				if (standingUpTime > 0.75f)
				{
					isStadningUp = false;
				}
			}
			if (!isRagdoll && !isStadningUp && (bool)driverActionController.GetMainPerson() && (bool)driverActionController.GetMainPerson().mainBodySkin && driverActionController.GetMainPerson().mainBodySkin.isVisible)
			{
				disableEnableAnim().Forget();
				base.enabled = false;
			}
		}

		private async UniTask disableEnableAnim()
		{
			if (_startingFixAnim || driverActionController.gameObject == V.DriverPerson)
			{
				return;
			}
			_startingFixAnim = true;
			_animator.applyRootMotion = true;
			_animator.enabled = false;
			await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
			if (!base.destroyCancellationToken.IsCancellationRequested)
			{
				_animator.enabled = true;
				await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
				if (!base.destroyCancellationToken.IsCancellationRequested)
				{
					_animator.Update(0f);
					_animator.Rebind();
					_animator.applyRootMotion = false;
					_startingFixAnim = false;
				}
			}
		}

		public async UniTask RagdollEnabler(Vector3 force, Vector3 origin = default(Vector3), bool autoStandUp = false, Vector3 initialVelocity = default(Vector3), bool autoStandUpPhysics = true)
		{
			if (!driverActionController || !driverActionController.ik)
			{
				return;
			}
			if (!onlinePerson && (bool)V.CoreGameManager.MenuLevelParent && (bool)V.CoreGameManager.MenuLevelParent.driftSign)
			{
				V.CoreGameManager.MenuLevelParent.driftSign.SetActive(value: false);
			}
			if (!onlinePerson && (bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null && (bool)V.CoreGameManager.FreeDriveProvider.FreeDriveControl)
			{
				FreeDriveControl freeDriveControl = V.CoreGameManager.FreeDriveProvider.FreeDriveControl;
				if ((bool)V.LastUsedCar && V.LastUsedCar.TryGetComponent<VehicleData>(out var component) && component.racingState == RacingState.None)
				{
					freeDriveControl.InvitationReject(checkHaveReceived: true);
				}
				if ((bool)freeDriveControl.ParkingMenuUI)
				{
					freeDriveControl.ParkingMenuUI.CancelRaceInvite();
				}
			}
			base.enabled = true;
			person.GetComponent<NewMoveController>().Input.RunX = 0f;
			person.GetComponent<NewMoveController>().Input.RunZ = 0f;
			if ((onlinePerson && Time.time - createTime < 10f) || isRagdoll)
			{
				return;
			}
			isRagdoll = true;
			SetRagdollNoInput();
			_lastGroundedTime = Time.time;
			if (person == V.DriverPerson)
			{
				LocalRagdollStarted?.Invoke();
			}
			if ((bool)person && person.TryGetComponent<WorldInteractionController>(out var component2))
			{
				component2.CancelForRagdoll();
			}
			ragdollStartTime = Time.time;
			driverActionController.ik.solver.IKPositionWeight = 0f;
			if (origin != default(Vector3))
			{
				person.transform.localPosition = origin;
			}
			hips = driverActionController.ik.references.pelvis;
			if (!onlinePerson && !hips.TryGetComponent<RagdollRestrictTrigger>(out var _))
			{
				hips.gameObject.AddComponent<RagdollRestrictTrigger>().Init(this);
			}
			EnsureBodyHitTrigger();
			offset = hips.position - person.transform.position;
			oldParent = hips.parent;
			scaleHips = hips.localScale;
			Vector3 launchDirection = Vector3.zero;
			if (!autoStandUp & autoStandUpPhysics)
			{
				angleOnImpace.y = force.x;
				Quaternion localRotation = hips.localRotation;
				hips.transform.localEulerAngles = angleOnImpace;
				launchDirection = hips.up;
				hips.localRotation = localRotation;
			}
			hips.transform.parent = ((V.GlobalPoint == null) ? null : V.GlobalPoint.transform);
			driverActionController.ik.enabled = false;
			_animator.updateMode = AnimatorUpdateMode.Normal;
			_animator.enabled = false;
			_localOffset = new Vector3(0f, (!IsSeatedInCar) ? 1 : 2, 0f);
			StartedInCar = IsSeatedInCar;
			_personRb = person.GetComponent<Rigidbody>();
			if (!onlinePerson && (bool)_personRb)
			{
				_personRb.isKinematic = true;
			}
			if ((bool)driverActionController.Car)
			{
				driverActionController.Car.GetComponent<VehicleData>().WindowComposition.SetWindowShatterHoleActive(isBreak: true);
				hips.transform.localPosition += new Vector3(0f, 1f, 0f);
				if (!onlinePerson)
				{
					V.DriverPerson.GetComponent<TouchMovePerson>().posOnRagdollStart = V.CoreGameManager.carCameras.transform.localPosition;
					if ((bool)driverActionController.multiPersonControl && !driverActionController.multiPersonControl.isPassanger)
					{
						StopCar(_carInputManager.GetPowertrain).Forget();
					}
					V.CoreGameManager.CarOutInControl.OutCar(person, driverActionController.Car, enableCollidersImmediately: true, isChangePos: false);
				}
			}
			driverActionController.allColliders[0].enabled = false;
			PrepareForRagdollBody(force, launchDirection, initialVelocity);
			if (autoStandUp)
			{
				PlaySoundGrunt();
			}
			else
			{
				PlaySound();
			}
			RestorePersonInputWhenGrounded(base.destroyCancellationToken).Forget();
			await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
			if (base.destroyCancellationToken.IsCancellationRequested)
			{
				return;
			}
			float num = Vector3.Distance(_prevPos, person.transform.position);
			if (onlinePerson)
			{
				person.GetComponent<Rigidbody>().isKinematic = true;
			}
			while (num > 0.02f)
			{
				await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
				if (base.destroyCancellationToken.IsCancellationRequested)
				{
					return;
				}
				if (!isRagdoll)
				{
					break;
				}
				num = Vector3.Distance(_prevPos, person.transform.position);
				_prevPos = person.transform.position;
			}
			await UniTask.Delay(100, ignoreTimeScale: false, PlayerLoopTiming.Update, base.destroyCancellationToken);
			if (base.destroyCancellationToken.IsCancellationRequested)
			{
				return;
			}
			if (autoStandUp && isRagdoll && !onlinePerson)
			{
				DisableRagdoll(immediately: true);
			}
			if (onlinePerson && isRagdoll)
			{
				RagdollUtility.Rigidbone[] array = rigidbones;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].r.isKinematic = true;
				}
			}
		}

		private async UniTask StopCar(Powertrain car)
		{
			await UniTask.Delay(500);
			if ((bool)car)
			{
				car.ResetTransmission(resetGear: false);
			}
		}

		private void PrepareForRagdollBody(Vector3 force, Vector3 launchDirection, Vector3 initialVelocity = default(Vector3))
		{
			Collider collider = (driverActionController.Car ? driverActionController.Car.GetComponent<Collider>() : null);
			Rigidbody r = rigidbones[0].r;
			bool flag = initialVelocity != Vector3.zero;
			for (int i = 0; i < rigidbones.Length; i++)
			{
				if ((bool)collider)
				{
					Physics.IgnoreCollision(rigidbones[i].collider, collider);
				}
				if (r == null)
				{
					r = rigidbones[i].r;
				}
				rigidbones[i].r.isKinematic = false;
				rigidbones[i].collider.enabled = true;
				rigidbones[i].r.WakeUp();
				if (flag)
				{
					rigidbones[i].r.velocity = initialVelocity;
				}
			}
			mainRigidbody = r;
			Vector3 position = r.worldCenterOfMass + Vector3.up * Up;
			Vector3 obj = ((launchDirection.sqrMagnitude > 0f) ? launchDirection.normalized : r.transform.up.normalized);
			force.x = 0f;
			Vector3 force2 = obj * (force.magnitude * forceAdjust);
			r.AddForceAtPosition(force2, position);
			if (FlagsStorage.IsPhotoMode && Time.fixedDeltaTime < 0.02f)
			{
				_simulatePhysicForRagdoll = true;
			}
		}

		private void StepRagdollPhysics()
		{
			if ((bool)mainRigidbody)
			{
				Vector3 vector = Physics.gravity * 0.02f;
				mainRigidbody.velocity += vector;
			}
		}

		private void EnsureBodyHitTrigger()
		{
			if ((bool)hips)
			{
				if (!hips.TryGetComponent<RagdollTrigger>(out var component))
				{
					component = hips.gameObject.AddComponent<RagdollTrigger>();
				}
				component.ragdollCpm = this;
				WiseHelper.SetAudioWiseComponents(hips.gameObject, addColliderTrigger: false);
			}
		}

		private void ApplyGenderSwitch()
		{
			if ((int)driverActionController.GetMainPerson().Gender == 0)
			{
				AudioWrapper.SetSwitch(_genderSwitch, _femaleSwitch, base.gameObject);
			}
			else
			{
				AudioWrapper.SetSwitch(_genderSwitch, _maleSwitch, base.gameObject);
			}
		}

		public bool IsAudible()
		{
			if (!onlinePerson)
			{
				return true;
			}
			MultiPersonControl multiPersonControl = (driverActionController ? driverActionController.GetComponent<MultiPersonControl>() : null);
			if (_interestGroupManager.HasValue && (bool)multiPersonControl)
			{
				return _interestGroupManager.Value.IsSubscribedToInterestGroup(multiPersonControl.InterestGroup);
			}
			return true;
		}

		private void PlaySound()
		{
			if (IsAudible())
			{
				ApplyGenderSwitch();
				AudioWrapper.SetSwitch(_stateVoiceSwitch, _stateHappyVoiceSwitch, base.gameObject);
				if (hips.TryGetComponent<RagdollTrigger>(out var component))
				{
					component.timeStartSount = Time.time;
				}
				AudioWrapper.PostEvent(soundEvent, base.gameObject);
			}
		}

		private bool IsInvalidVector(Vector3 vector)
		{
			if (!float.IsNaN(vector.x) && !float.IsNaN(vector.y) && !float.IsNaN(vector.z) && !float.IsInfinity(vector.x) && !float.IsInfinity(vector.y))
			{
				return float.IsInfinity(vector.z);
			}
			return true;
		}

		public void PlaySoundGrunt()
		{
			if (IsAudible())
			{
				ApplyGenderSwitch();
				AudioWrapper.SetSwitch(_stateVoiceSwitch, _stateGruntVoiceSwitch, base.gameObject);
				AudioWrapper.PostEvent(soundEvent, base.gameObject);
			}
		}

		private void PhotoModeClosedHandler()
		{
			if (isStadningUp || !isRagdoll || rigidbones == null || rigidbones.Length == 0)
			{
				return;
			}
			for (int i = 0; i < rigidbones.Length; i++)
			{
				Vector3 velocity = rigidbones[i].r.velocity;
				if (velocity.y > 0.1f)
				{
					velocity.y = 0f;
				}
				rigidbones[i].r.velocity = velocity;
			}
		}

		private void OnDestroy()
		{
			if (_photoModeObserver != null)
			{
				_photoModeObserver.OnPhotoModeClosed -= PhotoModeClosedHandler;
			}
		}
	}
}
