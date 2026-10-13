using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Audio.SoundBanks;
using _Car_Parking.Scripts.Person.Locomotion.Audio;
using _Car_Parking.Scripts.Person.Locomotion.Config;
using _Car_Parking.Scripts.Person.Locomotion.Input;
using _Car_Parking.Scripts.Person.Locomotion.Stamina;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class PersonLocomotion : MonoBehaviour
	{
		[SerializeField]
		private RuntimeAnimatorController _maleController;

		[SerializeField]
		private RuntimeAnimatorController _femaleController;

		[SerializeField]
		[Tooltip("Optional slim controllers for REMOTE players (Tools/Locomotion/Build Lite Remote Controllers). If unassigned, remotes fall back to the base controllers.")]
		private RuntimeAnimatorController _maleControllerLite;

		[SerializeField]
		private RuntimeAnimatorController _femaleControllerLite;

		private PersonInputAdapter _input;

		private PersonLocomotionState _state;

		private PersonGroundProbe _groundProbe;

		private PersonSlopeRules _slopeRules;

		private StaminaService _stamina;

		private CapsuleCollider _capsuleCollider;

		private MainPerson _mainPerson;

		private Rigidbody _rigidbody;

		private TouchMovePerson _touchMovePerson;

		private CarInputManager _carInputManager;

		private PersonFootIK _footIK;

		private MultiPersonControl _multiPersonControl;

		private bool _frozen;

		private IPersonLocomotionConfigService _configService;

		private IPersonActivation _activation;

		private PersonAnimatorBridge _bridge;

		private PersonPlatformRider _rider;

		private PersonMovementMotor _motor;

		private IRagdollController _ragdoll;

		private PersonLocomotionAudio _audio;

		private PersonBreathAudio _breath;

		private SoundBankLoadService _soundBankLoadService;

		private int _suspendCount;

		private float _resumeDelayTimer;

		private bool _suspensionApplied;

		private const float FullyStoppedSpeedThreshold = 0.15f;

		public RuntimeAnimatorController MaleController => _maleController;

		public RuntimeAnimatorController FemaleController => _femaleController;

		public RuntimeAnimatorController MaleControllerLite => _maleControllerLite;

		public RuntimeAnimatorController FemaleControllerLite => _femaleControllerLite;

		private bool RagdollOwnsSkeleton
		{
			get
			{
				if (_ragdoll != null)
				{
					return _ragdoll.IsRagdoll;
				}
				return false;
			}
		}

		public Vector3 VisualOffset
		{
			get
			{
				if (_motor == null)
				{
					return Vector3.zero;
				}
				return _motor.VisualOffset;
			}
		}

		public RootMotionFlags CurrentRootMotion
		{
			get
			{
				if (_bridge == null)
				{
					return default;
				}
				return _bridge.CurrentRootMotion;
			}
		}

		public bool IsActive
		{
			get
			{
				if (_state != null)
				{
					return _state.IsActive;
				}
				return false;
			}
		}

		public bool IsSuspended
		{
			get
			{
				if (_suspendCount <= 0)
				{
					return _resumeDelayTimer > 0f;
				}
				return true;
			}
		}

		public bool IsFullyStopped
		{
			get
			{
				if (_state == null || !_state.IsActive)
				{
					return false;
				}
				LocomotionStateSnapshot snapshot = _state.Snapshot;
				if (!snapshot.IsGrounded || snapshot.IsStarting || snapshot.IsJumping || snapshot.Gait != 0)
				{
					return false;
				}
				if (snapshot.MoveSpeed > 0.15f)
				{
					return false;
				}
				if ((bool)_rigidbody)
				{
					Vector3 vector = _rigidbody.velocity - ((_motor != null) ? _motor.PlatformCarry : Vector3.zero);
					vector.y = 0f;
					if (vector.magnitude > 0.15f)
					{
						return false;
					}
				}
				return true;
			}
		}

		public GroundInfo Ground
		{
			get
			{
				if (_state == null)
				{
					return default;
				}
				return _state.Ground;
			}
		}

		public Vector3 PlatformCarry
		{
			get
			{
				if (_motor == null)
				{
					return Vector3.zero;
				}
				return _motor.PlatformCarry;
			}
		}

		public LocomotionStateSnapshot Snapshot
		{
			get
			{
				if (_state == null)
				{
					return default;
				}
				return _state.Snapshot;
			}
		}

		public float StaminaNormalized
		{
			get
			{
				if (_stamina == null)
				{
					return 1f;
				}
				return _stamina.Normalized.Value;
			}
		}

		public bool SlideOnCrouch
		{
			get
			{
				if (_state != null)
				{
					return _state.SlideOnCrouch;
				}
				return false;
			}
		}

		public bool IsSlideCrouch
		{
			get
			{
				if (_state != null)
				{
					return _state.IsSlideCrouch;
				}
				return false;
			}
		}

		public bool AutoRun
		{
			get
			{
				if (_input != null)
				{
					return _input.AutoRun;
				}
				return false;
			}
		}

		public bool IsFrozen => _frozen;

		public void Suspend()
		{
			_suspendCount++;
			_resumeDelayTimer = 0f;
			ApplySuspension();
		}

		public void Resume(float delaySeconds)
		{
			if (_suspendCount > 0)
			{
				_suspendCount--;
			}
			if (_suspendCount <= 0)
			{
				_resumeDelayTimer = Mathf.Max(delaySeconds, 0f);
				if (_resumeDelayTimer <= 0f)
				{
					ReleaseSuspension();
				}
			}
		}

		private void ApplySuspension()
		{
			if (!_suspensionApplied && _bridge != null && !RagdollOwnsSkeleton)
			{
				_suspensionApplied = true;
				if (_state != null)
				{
					_state.IsActive = false;
				}
				_input?.ResetMoveState();
				_bridge?.ReleaseToLegacy();
				_audio?.Stop();
				_breath?.Stop();
			}
		}

		private void ReleaseSuspension()
		{
			if (_suspensionApplied)
			{
				_suspensionApplied = false;
				_input?.ResetMoveState();
				_state?.ResetForResume();
				_motor?.ResetMotionState();
				_bridge?.PrepareForResume();
			}
		}

		public void ResetStamina()
		{
			_stamina?.Reset();
		}

		public void ResetToRest(bool resetStamina = true)
		{
			_input?.ResetAll();
			_state?.ResetToRest();
			if (resetStamina)
			{
				_stamina?.Reset();
			}
			_motor?.ResetMotionState();
			if (!RagdollOwnsSkeleton && !IsSuspended)
			{
				_bridge?.WriteNeutralAnimatorState();
			}
		}

		public void OnJumpLaunch()
		{
			_state?.NotifyJumpLaunch();
		}

		public void SetScriptedMoveWorld(Vector3 worldDir, float magnitude)
		{
			_input?.SetScriptedMoveWorld(worldDir, magnitude);
		}

		public void ClearScriptedMove()
		{
			_input?.ClearScriptedMove();
		}

		public void CancelSprintLock()
		{
			_input?.CancelSprintLock();
		}

		public void ToggleAutoRun()
		{
			_input?.ToggleAutoRun();
		}

		public void SetAutoRun(bool on)
		{
			_input?.SetAutoRun(on);
		}

		public void Freeze(bool frozen)
		{
			if (_frozen != frozen)
			{
				_frozen = frozen;
				_input?.SetFrozen(frozen);
				_motor?.SetFrozen(frozen);
				_bridge?.SetFrozen(frozen);
				_bridge?.WriteNeutralAnimatorState();
				if (frozen)
				{
					_audio?.Stop();
					_breath?.Stop();
				}
			}
		}

		public void Teleport(Vector3 position, bool resyncNetwork = false)
		{
			ApplyTeleport(position, base.transform.rotation, resyncNetwork);
		}

		public void Teleport(Vector3 position, Quaternion rotation, bool resyncNetwork = false)
		{
			ApplyTeleport(position, rotation, resyncNetwork);
		}

		private void ApplyTeleport(Vector3 position, Quaternion rotation, bool resyncNetwork)
		{
			_input?.ResetMoveState();
			_input?.ResetPendingJump();
			_state?.ResetMomentum();
			_motor?.PrepareTeleport();
			if ((bool)_rigidbody)
			{
				if (!_rigidbody.isKinematic)
				{
					_rigidbody.velocity = Vector3.zero;
					_rigidbody.angularVelocity = Vector3.zero;
				}
				_rigidbody.position = position;
				_rigidbody.rotation = rotation;
			}
			base.transform.SetPositionAndRotation(position, rotation);
			_bridge?.ResetRootMotionDelta();
			_bridge?.WriteNeutralAnimatorState();
			_bridge?.SnapToIdlePose();
			if ((bool)_footIK)
			{
				_footIK.NotifyTeleport();
			}
			if (resyncNetwork && (bool)_multiPersonControl)
			{
				_multiPersonControl.ResyncAfterTeleport();
			}
			ResetStamina();
		}

		[Inject]
		public void Construct(IPersonLocomotionConfigService configService, IPersonActivation activation, IObjectResolver resolver)
		{
			_configService = configService;
			_activation = activation;
			resolver?.TryResolve<CarInputManager>(out _carInputManager);
			if (resolver == null || !resolver.TryResolve<SoundBankLoadService>(out _soundBankLoadService))
			{
				_soundBankLoadService = (V.CoreGameManager ? V.CoreGameManager.SoundBankLoadService : null);
			}
			BuildSubsystems();
		}

		private void Awake()
		{
			AcquireComponents();
		}

		private void OnDisable()
		{
			_audio?.Stop();
			_breath?.Stop();
		}

		private void AcquireComponents()
		{
			TryGetComponent<CapsuleCollider>(out _capsuleCollider);
			TryGetComponent<MainPerson>(out _mainPerson);
			TryGetComponent<Rigidbody>(out _rigidbody);
			TryGetComponent<TouchMovePerson>(out _touchMovePerson);
			TryGetComponent<PersonFootIK>(out _footIK);
			TryGetComponent<MultiPersonControl>(out _multiPersonControl);
		}

		private void BuildSubsystems()
		{
			AcquireComponents();
			IRagdollController ragdoll = (_ragdoll = new MultiPersonControlRagdollAdapter(base.gameObject));
			_input = new PersonInputAdapter(base.gameObject, _touchMovePerson, _carInputManager, _configService?.Locomotion);
			_slopeRules = new PersonSlopeRules(_configService?.Locomotion);
			_groundProbe = new PersonGroundProbe(base.transform, _capsuleCollider, _rigidbody, _configService?.Locomotion, _configService?.Capsule, _slopeRules);
			_stamina = ((_configService?.Stamina != null) ? new StaminaService(_configService.Stamina, _configService.Locomotion) : null);
			_state = new PersonLocomotionState(base.transform, _configService?.Locomotion, _configService?.Capsule, _input, _groundProbe, _stamina, _capsuleCollider, V.CoreGameManager ? V.CoreGameManager.MainCamera : null);
			_bridge = new PersonAnimatorBridge(base.transform, _mainPerson, _state, _rigidbody, _input, _configService?.Locomotion, _maleController, _femaleController);
			_audio = new PersonLocomotionAudio(base.transform, _mainPerson, _state, _configService?.Locomotion, _soundBankLoadService);
			_breath = new PersonBreathAudio(base.transform, _mainPerson, _state, _stamina, _configService?.Locomotion, _soundBankLoadService);
			_rider = new PersonPlatformRider(_rigidbody, _configService?.Locomotion);
			_motor = new PersonMovementMotor(base.transform, _rigidbody, _mainPerson, _state, _groundProbe, _input, _bridge, ragdoll, _activation, _configService?.Locomotion, _rider, _slopeRules);
			if (_frozen)
			{
				_input.SetFrozen(frozen: true);
				_motor.SetFrozen(frozen: true);
				_bridge.SetFrozen(frozen: true);
				_bridge.WriteNeutralAnimatorState();
			}
		}

		private void Update()
		{
			if (_suspendCount > 0)
			{
				ApplySuspension();
				return;
			}
			if (_resumeDelayTimer > 0f)
			{
				_resumeDelayTimer -= Time.deltaTime;
				if (_resumeDelayTimer > 0f)
				{
					return;
				}
				_resumeDelayTimer = 0f;
				ReleaseSuspension();
			}
			if (_input != null)
			{
				_input.Tick();
			}
		}

		private void FixedUpdate()
		{
			if (IsSuspended)
			{
				_stamina?.Tick(Time.fixedDeltaTime, usingStamina: false);
				return;
			}
			if (_motor == null || !_motor.PhysicsStep())
			{
				_stamina?.Tick(Time.fixedDeltaTime, usingStamina: false);
			}
			_audio?.PhysicsTick();
		}

		private void LateUpdate()
		{
			if (!IsSuspended && !RagdollOwnsSkeleton)
			{
				_bridge?.AnimationStep();
				_bridge?.ApplyProceduralLean();
				_motor?.VisualStep();
				_audio?.Tick();
				_breath?.Tick();
			}
		}
	}
}
