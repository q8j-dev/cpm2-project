using CarInteractionAnimationsScripts;
using UnityEngine;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Person.Locomotion.Config;
using _Car_Parking.Scripts.Person.Locomotion.Input;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class PersonAnimatorBridge
	{
		private static readonly int CurrentGaitHash = Animator.StringToHash("CurrentGait");

		private static readonly int MoveSpeedHash = Animator.StringToHash("MoveSpeed");

		private static readonly int StrafeDirectionXHash = Animator.StringToHash("StrafeDirectionX");

		private static readonly int StrafeDirectionZHash = Animator.StringToHash("StrafeDirectionZ");

		private static readonly int ForwardStrafeHash = Animator.StringToHash("ForwardStrafe");

		private static readonly int LocomotionStartDirectionHash = Animator.StringToHash("LocomotionStartDirection");

		private static readonly int InclineAngleHash = Animator.StringToHash("InclineAngle");

		private static readonly int FallingDurationHash = Animator.StringToHash("FallingDuration");

		private static readonly int IsStrafingHash = Animator.StringToHash("IsStrafing");

		private static readonly int IsStoppedHash = Animator.StringToHash("IsStopped");

		private static readonly int IsStartingHash = Animator.StringToHash("IsStarting");

		private static readonly int IsWalkingHash = Animator.StringToHash("IsWalking");

		private static readonly int IsSprintingHash = Animator.StringToHash("IsSprinting");

		private static readonly int IsCrouchingHash = Animator.StringToHash("IsCrouching");

		private static readonly int IsJumpingHash = Animator.StringToHash("IsJumping");

		private static readonly int DoJumpHash = Animator.StringToHash("DoJump");

		private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

		private static readonly int IsTurningInPlaceHash = Animator.StringToHash("IsTurningInPlace");

		private static readonly int IsQuickTurningHash = Animator.StringToHash("IsQuickTurning");

		private static readonly int QuickTurnLeftHash = Animator.StringToHash("QuickTurnLeft");

		private static readonly int QuickTurnReverseHash = Animator.StringToHash("QuickTurnReverse");

		private static readonly int MovementInputTappedHash = Animator.StringToHash("MovementInputTapped");

		private static readonly int MovementInputPressedHash = Animator.StringToHash("MovementInputPressed");

		private static readonly int MovementInputHeldHash = Animator.StringToHash("MovementInputHeld");

		private static readonly int ShuffleRequestedHash = Animator.StringToHash("ShuffleRequested");

		private static readonly int CameraRotationOffsetHash = Animator.StringToHash("CameraRotationOffset");

		private static readonly int ShuffleDirectionXHash = Animator.StringToHash("ShuffleDirectionX");

		private static readonly int ShuffleDirectionZHash = Animator.StringToHash("ShuffleDirectionZ");

		private static readonly int StopOnLeftFootHash = Animator.StringToHash("StopOnLeftFoot");

		private static readonly int StopPhaseMatchedHash = Animator.StringToHash("StopPhaseMatched");

		private static readonly int WalkStopLeftStateHash = Animator.StringToHash("Walk_Stop_LFoot");

		private static readonly int WalkStopRightStateHash = Animator.StringToHash("Walk_Stop_RFoot");

		private const string WalkStopLeftStatePath = "Base Layer.Locomotion.Walk_Stop_LFoot";

		private const string WalkStopRightStatePath = "Base Layer.Locomotion.Walk_Stop_RFoot";

		private static readonly int StopCycleOffsetHash = Animator.StringToHash("StopCycleOffset");

		private static readonly int StartPlaybackSpeedHash = Animator.StringToHash("StartPlaybackSpeed");

		private static readonly int LeanValueHash = Animator.StringToHash("LeanValue");

		private static readonly int HeadLookXHash = Animator.StringToHash("HeadLookX");

		private static readonly int HeadLookYHash = Animator.StringToHash("HeadLookY");

		private static readonly int BodyLookXHash = Animator.StringToHash("BodyLookX");

		private static readonly int BodyLookYHash = Animator.StringToHash("BodyLookY");

		private static readonly int IdleBreakIndexHash = Animator.StringToHash("IdleBreakIndex");

		private static readonly int PlayIdleBreakHash = Animator.StringToHash("PlayIdleBreak");

		private static readonly int StopAnimationHash = Animator.StringToHash("StopAnimation");

		private static readonly int IdleStandingHash = Animator.StringToHash("Idle_Standing");

		private const string IdleStandingPath = "Base Layer.Idle.Idle_Standing";

		private const string EmoteExitLocomotionState = "LocomotionBlendTree";

		private const float EmoteExitBlendDuration = 0.25f;

		private const float StopFootPhaseSplit = 0.5f;

		private const float DefaultStopOffsetScale = 0.25f;

		private const float DefaultStopPhaseRefLeft = 0.13f;

		private const float DefaultStopPhaseRefRight = 0.53f;

		private const float DefaultStopOffsetMax = 0.2f;

		private const float DefaultStopFootLevelDeadzone = 0.15f;

		private const float MaxFootSwingSpeed = 15f;

		private readonly Transform _transform;

		private readonly MainPerson _mainPerson;

		private readonly PersonLocomotionState _state;

		private readonly Rigidbody _rigidbody;

		private readonly PersonInputAdapter _input;

		private readonly PersonLocomotionConfig _config;

		private readonly RuntimeAnimatorController _maleController;

		private readonly RuntimeAnimatorController _femaleController;

		private Animator _animator;

		private RagdollCPM _ragdollCpm;

		private RuntimeAnimatorController _originalController;

		private bool _originalRootMotion;

		private bool _swapped;

		private bool _controllerApplied;

		private bool _emoteStateSeen;

		private bool _stopOnLeftFoot;

		private bool _stopFootLatched;

		private float _stopCycleOffset;

		private Transform _leftFoot;

		private Transform _rightFoot;

		private Transform _spine;

		private float _lastLeanStamp = float.NaN;

		private bool _stopFootFromBones;

		private bool _diagLeftFootForward;

		private float _diagStopPhase;

		private Vector3 _prevLeftFootPos;

		private Vector3 _prevRightFootPos;

		private bool _hasPrevFootPos;

		private bool _diagVelPickValid;

		private bool _diagVelLeftForward;

		private float _diagLeftFwdVel;

		private float _diagRightFwdVel;

		private float _diagPosDot;

		private Vector3 _diagPrevStopFootWorld;

		private bool _diagHasStopFootPrev;

		private PersonRootMotion _rootMotion;

		private readonly PersonStartRollback _startRollback = new PersonStartRollback();

		private readonly PersonAnimatorEdgeLatch _tapLatch = new PersonAnimatorEdgeLatch();

		private int _startSpeedParam;

		private bool _startSpeedEngaged;

		private bool _frozen;

		private float _idleDwellTimer;

		private float _nextBreakDelay;

		private PersonKitAnimationLayer[] _kitLayers;

		private const bool DiagStarts = false;

		private const bool DiagFootSkate = false;

		private const int FootSkateTailFrames = 30;

		private Vector3 _skatePrevLeft;

		private Vector3 _skatePrevRight;

		private bool _hasSkatePrev;

		private int _skateTailFrames;

		private bool _prevStarting;

		private bool _prevIsJumping;

		private int _diagFrames;

		private int _jumpSeqDiagFrames;

		private const bool DiagResume = false;

		private bool _prevHasMove;

		private int _resumeDiagFrames;

		private const bool DiagStops = false;

		private const bool DiagWalkProfile = false;

		private const bool DiagStopEntry = false;

		private const bool DiagStopFacing = false;

		private bool _prevStoppedFacing;

		private int _stopFacingFrames;

		private const int DiagForceStopFoot = 0;

		private bool _prevInStopFull;

		private bool _prevInStopDiag;

		private bool _instantStopFiring;

		private int _stopEntryFrames;

		private bool _prevStopped;

		private bool _prevStopRequested;

		private bool _walkStopEntryPending;

		private bool _prevGrounded;

		private int _stopDiagFrames;

		private bool _reverseLatched;

		private float _reverseTimer;

		private int _redirectDeviationSteps;

		private int _emoteDiagFrames;

		private const bool DiagSlopeLean = false;

		private const int SlopeLeanDiagBudget = 300;

		private Transform _head;

		private int _slopeLeanDiagFrames;

		private float _diagPrevLeanAngle;

		private float _diagPrevHeadPitch;

		private float _diagPrevHeadRise;

		private bool _hasSlopeLeanDiagPrev;

		private static readonly string[] DiagStateNames = new string[11]
		{
			"SpecialAnimations", "OneShot", "Idle_Standing", "Idle", "LocomotionBlendTree", "Locomotion_BlendTree", "LocomotionBlendTree ", "Walk_BlendTree", "Walk_Start_BlendTree", "Walk_Start_F360",
			"Run_Start_BlendTree"
		};

		public RootMotionFlags CurrentRootMotion { get; private set; }

		public bool InstantStopEscaping { get; private set; }

		public PersonAnimatorBridge(Transform transform, MainPerson mainPerson, PersonLocomotionState state, Rigidbody rigidbody, PersonInputAdapter input, PersonLocomotionConfig config, RuntimeAnimatorController maleController, RuntimeAnimatorController femaleController)
		{
			_transform = transform;
			_mainPerson = mainPerson;
			_state = state;
			_rigidbody = rigidbody;
			_input = input;
			_config = config;
			_maleController = maleController;
			_femaleController = femaleController;
			_kitLayers = (transform ? transform.GetComponents<PersonKitAnimationLayer>() : null);
		}

		public void SetFrozen(bool frozen)
		{
			_frozen = frozen;
		}

		public void ResetRootMotionDelta()
		{
			if ((bool)_rootMotion)
			{
				_rootMotion.ResetDelta();
			}
		}

		public void WriteNeutralAnimatorState()
		{
			if (ResolveAnimator())
			{
				if (_animator.updateMode != AnimatorUpdateMode.Normal)
				{
					_animator.updateMode = AnimatorUpdateMode.Normal;
				}
				_animator.SetInteger(CurrentGaitHash, 0);
				_animator.SetFloat(MoveSpeedHash, 0f);
				_animator.SetFloat(StrafeDirectionXHash, 0f);
				_animator.SetFloat(StrafeDirectionZHash, 1f);
				_animator.SetFloat(ForwardStrafeHash, 1f);
				_animator.SetFloat(LocomotionStartDirectionHash, 0f);
				_animator.SetFloat(InclineAngleHash, 0f);
				_animator.SetFloat(FallingDurationHash, 0f);
				_animator.SetFloat(IsStrafingHash, 0f);
				_animator.SetFloat(CameraRotationOffsetHash, 0f);
				_animator.SetFloat(ShuffleDirectionXHash, 0f);
				_animator.SetFloat(ShuffleDirectionZHash, 0f);
				_animator.SetFloat(LeanValueHash, 0f);
				_animator.SetFloat(HeadLookXHash, 0f);
				_animator.SetFloat(HeadLookYHash, 0f);
				_animator.SetFloat(BodyLookXHash, 0f);
				_animator.SetFloat(BodyLookYHash, 0f);
				_animator.SetBool(IsStoppedHash, value: false);
				_animator.SetBool(IsStartingHash, value: false);
				_animator.SetBool(IsWalkingHash, value: false);
				_animator.SetBool(IsSprintingHash, value: false);
				_animator.SetBool(IsCrouchingHash, value: false);
				_animator.SetBool(IsJumpingHash, value: false);
				_animator.SetBool(IsGroundedHash, value: true);
				_animator.SetBool(IsTurningInPlaceHash, value: false);
				_animator.SetBool(IsQuickTurningHash, value: false);
				_animator.SetBool(QuickTurnLeftHash, value: false);
				_animator.SetBool(QuickTurnReverseHash, value: false);
				_animator.SetBool(MovementInputTappedHash, value: false);
				_animator.SetBool(MovementInputPressedHash, value: false);
				_animator.SetBool(MovementInputHeldHash, value: false);
				_animator.SetBool(ShuffleRequestedHash, value: false);
				_prevIsJumping = false;
				_prevStopRequested = false;
				_walkStopEntryPending = false;
				ResetStartRollback();
				_animator.ResetTrigger(DoJumpHash);
				_animator.ResetTrigger(PlayIdleBreakHash);
				if ((bool)_rootMotion)
				{
					_rootMotion.PassThrough = false;
					_rootMotion.Active = false;
					_rootMotion.ResetDelta();
				}
			}
		}

		public void SnapToIdlePose()
		{
			if (!ResolveAnimator())
			{
				return;
			}
			AnimatorUpdateMode animatorUpdateMode = ((!_config || _config.AnimatorUpdateInFixedStep) ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal);
			if (_animator.updateMode != animatorUpdateMode)
			{
				_animator.updateMode = animatorUpdateMode;
			}
			if (!RagdollOwnsRig())
			{
				if (_animator.HasState(0, IdleStandingHash))
				{
					_animator.CrossFadeInFixedTime(IdleStandingHash, 0f, 0);
				}
				else
				{
					_animator.CrossFadeInFixedTime("Base Layer.Idle.Idle_Standing", 0f, 0);
				}
				_instantStopFiring = false;
				InstantStopEscaping = false;
				ResetStartRollback();
				_reverseLatched = false;
				_reverseTimer = 0f;
				_hasPrevFootPos = false;
				_stopFootLatched = false;
				_prevIsJumping = false;
				_lastLeanStamp = float.NaN;
			}
		}

		public void AnimationStep()
		{
			if (_state == null || !ResolveAnimator())
			{
				return;
			}
			if (_frozen)
			{
				InstantStopEscaping = false;
				WriteNeutralAnimatorState();
				return;
			}
			if (!_state.IsActive)
			{
				InstantStopEscaping = false;
				_startRollback.Reset();
				ReleaseToLegacy();
				return;
			}
			CancelEmoteWhenInterrupted();
			EnsureRootMotion();
			ApplyControllerOnce();
			AnimatorUpdateMode animatorUpdateMode = ((!_config || _config.AnimatorUpdateInFixedStep) ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal);
			if (_animator.updateMode != animatorUpdateMode)
			{
				_animator.updateMode = animatorUpdateMode;
			}
			_state.AnimationTick(Time.deltaTime);
			LocomotionStateSnapshot snapshot = _state.Snapshot;
			_animator.SetInteger(CurrentGaitHash, snapshot.Gait);
			_animator.SetFloat(MoveSpeedHash, snapshot.MoveSpeed);
			_animator.SetFloat(StrafeDirectionXHash, snapshot.StrafeDirectionX);
			_animator.SetFloat(StrafeDirectionZHash, snapshot.StrafeDirectionZ);
			_animator.SetFloat(ForwardStrafeHash, snapshot.ForwardStrafe);
			_animator.SetFloat(InclineAngleHash, snapshot.InclineAngle);
			_animator.SetFloat(FallingDurationHash, snapshot.FallingDuration);
			_animator.SetFloat(IsStrafingHash, snapshot.IsStrafing ? 1f : 0f);
			_animator.SetFloat(CameraRotationOffsetHash, snapshot.CameraRotationOffset);
			_animator.SetFloat(ShuffleDirectionXHash, snapshot.ShuffleDirectionX);
			_animator.SetFloat(ShuffleDirectionZHash, snapshot.ShuffleDirectionZ);
			_animator.SetFloat(LeanValueHash, snapshot.LeanValue);
			_animator.SetFloat(HeadLookXHash, snapshot.HeadLookX);
			_animator.SetFloat(HeadLookYHash, snapshot.HeadLookY);
			_animator.SetFloat(BodyLookXHash, snapshot.BodyLookX);
			_animator.SetFloat(BodyLookYHash, snapshot.BodyLookY);
			bool flag = _animator.IsInTransition(0);
			bool flag2 = IsStartState(_animator.GetCurrentAnimatorStateInfo(0)) || (flag && IsStartState(_animator.GetNextAnimatorStateInfo(0)));
			bool inStandingStartAny = IsStandingStartState(_animator.GetCurrentAnimatorStateInfo(0)) || (flag && IsStandingStartState(_animator.GetNextAnimatorStateInfo(0)));
			TickStartRollback(snapshot, flag, inStandingStartAny);
			bool flag3 = !ResolveInstantStopEscape() && snapshot.IsStopped && !_startRollback.Active;
			_animator.SetBool(IsStoppedHash, flag3);
			_animator.SetBool(IsStartingHash, snapshot.IsStarting);
			_animator.SetBool(IsWalkingHash, snapshot.IsWalking);
			_animator.SetBool(IsSprintingHash, snapshot.IsSprinting);
			_animator.SetBool(IsCrouchingHash, snapshot.IsCrouching);
			_animator.SetBool(IsJumpingHash, snapshot.IsJumping);
			if (_config.JumpFromAnyStateEnabled && snapshot.IsJumping && !_prevIsJumping)
			{
				_animator.SetTrigger(DoJumpHash);
			}
			_prevIsJumping = snapshot.IsJumping;
			_animator.SetBool(IsGroundedHash, snapshot.IsGrounded);
			_animator.SetBool(IsTurningInPlaceHash, snapshot.IsTurningInPlace);
			_animator.SetBool(IsQuickTurningHash, snapshot.IsQuickTurning);
			_animator.SetBool(QuickTurnLeftHash, snapshot.QuickTurnLeft);
			_animator.SetBool(MovementInputTappedHash, _tapLatch.Resolve(snapshot.MovementInputTapped, Time.fixedTime, _animator.updateMode == AnimatorUpdateMode.AnimatePhysics));
			_animator.SetBool(MovementInputPressedHash, snapshot.MovementInputPressed);
			_animator.SetBool(MovementInputHeldHash, snapshot.MovementInputHeld);
			_animator.SetBool(ShuffleRequestedHash, snapshot.ShuffleRequested);
			if (!snapshot.IsStopped)
			{
				UpdateStopFoot();
			}
			_animator.SetBool(StopOnLeftFootHash, _stopOnLeftFoot);
			_animator.SetFloat(StopCycleOffsetHash, ResolveStopCycleOffset());
			bool flag4 = (bool)_config && _config.WalkStopPhaseMatchEnabled;
			_animator.SetBool(StopPhaseMatchedHash, flag4);
			if (flag4)
			{
				EnterPhaseMatchedWalkStop(snapshot, flag3);
			}
			_prevStopRequested = flag3;
			_state.SetStopGateInputs(_diagPosDot, _diagLeftFwdVel - _diagRightFwdVel);
			_state.SetPlantedFoot(_stopOnLeftFoot);
			bool flag5 = !flag && IsStopState(_animator.GetCurrentAnimatorStateInfo(0));
			bool flag6 = flag && IsStopState(_animator.GetNextAnimatorStateInfo(0));
			bool flag7 = flag5 | flag6;
			bool flag8 = false;
			bool flag9 = false;
			if ((bool)_config)
			{
				float num = Mathf.Abs(snapshot.StartInitialDirection);
				bool flag10 = num <= _config.StartRootMotionMaxAngle;
				bool num2 = _config.StartTurnRootMotionMaleWalkFullRange && (IsWalkStartState(_animator.GetCurrentAnimatorStateInfo(0)) || IsCrouchStartState(_animator.GetCurrentAnimatorStateInfo(0)) || (flag && (IsWalkStartState(_animator.GetNextAnimatorStateInfo(0)) || IsCrouchStartState(_animator.GetNextAnimatorStateInfo(0)))));
				bool flag11 = _config.StartTurnRootMotionRunFullRange && (IsRunStartState(_animator.GetCurrentAnimatorStateInfo(0)) || (flag && IsRunStartState(_animator.GetNextAnimatorStateInfo(0))));
				float num3 = ((num2 | flag11) ? _config.StartRootMotionMaxAngle : _config.StartTurnRootMotionMinAngle);
				flag9 = _config.StartTurnRootMotionEnabled && num >= num3;
				flag8 = _config.StartRootMotionEnabled && (flag10 | flag9);
			}
			bool flag12 = flag8 && !flag && IsStartState(_animator.GetCurrentAnimatorStateInfo(0));
			bool flag13 = (flag8 & flag) && IsStartState(_animator.GetNextAnimatorStateInfo(0));
			bool flag14 = flag12 | flag13;
			bool flag15 = (flag8 & flag) && IsStartState(_animator.GetCurrentAnimatorStateInfo(0)) && (IsLocoState(_animator.GetNextAnimatorStateInfo(0)) || IsCrouchLocoState(_animator.GetNextAnimatorStateInfo(0)));
			bool flag16 = flag9 & flag15;
			bool flag17 = flag15 && (flag9 ? _config.StartTurnExitRampEnabled : _config.StartExitRampEnabled);
			bool flag18 = ((flag9 && (bool)_config && _config.StartStopFacingFixEnabled) & flag) && IsStartState(_animator.GetCurrentAnimatorStateInfo(0)) && IsStopState(_animator.GetNextAnimatorStateInfo(0));
			bool flag19 = flag9 && (flag14 | flag16 | flag18);
			bool crouchTurnStart = flag19 && (IsCrouchStartState(_animator.GetCurrentAnimatorStateInfo(0)) || (flag && IsCrouchStartState(_animator.GetNextAnimatorStateInfo(0))));
			float value = (flag19 ? snapshot.StartInitialDirection : snapshot.LocomotionStartDirection);
			if ((bool)_config && _config.StartDirectionSnapEnabled)
			{
				value = PersonStartDirectionSnap.Snap(snapshot.StartInitialDirection);
			}
			_animator.SetFloat(LocomotionStartDirectionHash, value);
			int num4;
			int num5;
			if ((bool)_config)
			{
				num4 = (_config.RootMotionLocomotionEnabled ? 1 : 0);
				if (num4 != 0 && !flag)
				{
					num5 = (IsLocoState(_animator.GetCurrentAnimatorStateInfo(0)) ? 1 : 0);
					goto IL_0819;
				}
			}
			else
			{
				num4 = 0;
			}
			num5 = 0;
			goto IL_0819;
			IL_0cb8:
			if (_reverseLatched)
			{
				_reverseLatched = false;
				_reverseTimer = 0f;
			}
			goto IL_0cd2;
			IL_0cd2:
			int num6;
			if (num6 != 0)
			{
				_animator.SetBool(QuickTurnReverseHash, _reverseLatched);
			}
			bool flag20;
			bool flag21;
			float num7;
			bool flag22;
			bool quickTurnCommitActive = (_config.QuickTurnCommitLockEnabled & flag20) && !_reverseLatched && ((flag21 && num7 >= _config.QuickTurnReverseMaxProgress) | flag22);
			bool flag23 = flag22 && _config.QuickTurnExitRampEnabled && !_reverseLatched;
			int num8;
			int num9;
			if ((bool)_config)
			{
				num8 = (_config.EmoteRootMotionEnabled ? 1 : 0);
				if (num8 != 0 && !flag)
				{
					num9 = (IsSpecialState(_animator.GetCurrentAnimatorStateInfo(0)) ? 1 : 0);
					goto IL_0d77;
				}
			}
			else
			{
				num8 = 0;
			}
			num9 = 0;
			goto IL_0d77;
			IL_0819:
			bool flag24 = (byte)num5 != 0;
			bool flag25 = ((uint)num4 & (flag ? 1u : 0u)) != 0 && IsLocoState(_animator.GetNextAnimatorStateInfo(0));
			bool flag26 = flag24 | flag25;
			bool flag27 = num4 != 0 && !flag && IsCrouchLocoState(_animator.GetCurrentAnimatorStateInfo(0));
			bool flag28 = ((uint)num4 & (flag ? 1u : 0u)) != 0 && IsCrouchLocoState(_animator.GetNextAnimatorStateInfo(0));
			bool flag29 = flag27 | flag28;
			float num10 = StateCrouchAmount(_animator.GetCurrentAnimatorStateInfo(0));
			float value2 = (flag ? Mathf.Lerp(num10, StateCrouchAmount(_animator.GetNextAnimatorStateInfo(0)), _animator.GetAnimatorTransitionInfo(0).normalizedTime) : num10);
			AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			bool num11 = currentAnimatorStateInfo.IsName("Sprint_ToCrouch");
			bool slideActive = num11 || (flag && _animator.GetNextAnimatorStateInfo(0).IsName("Sprint_ToCrouch"));
			float slideNormalizedTime = (num11 ? currentAnimatorStateInfo.normalizedTime : 0f);
			_state.SetCrouchBlendTarget(value2, slideActive, slideNormalizedTime);
			flag20 = (bool)_config && _config.QuickTurnEnabled;
			flag21 = flag20 && !flag && IsQuickTurnState(_animator.GetCurrentAnimatorStateInfo(0));
			bool flag30 = (flag20 & flag) && IsQuickTurnState(_animator.GetNextAnimatorStateInfo(0));
			bool flag31 = flag21 | flag30;
			flag22 = (flag20 & flag) && IsQuickTurnState(_animator.GetCurrentAnimatorStateInfo(0)) && IsLocoState(_animator.GetNextAnimatorStateInfo(0));
			num7 = (flag21 ? (_animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f) : (flag30 ? 0f : (-1f)));
			if (flag20)
			{
				if (!_config.QuickTurnReverseEnabled)
				{
					num6 = (_config.QuickTurnRedirectEnabled ? 1 : 0);
					if (num6 == 0)
					{
						goto IL_0cb8;
					}
				}
				else
				{
					num6 = 1;
				}
				if (!_reverseLatched)
				{
					Vector3 rawWorldMoveDirection = _state.RawWorldMoveDirection;
					Vector3 quickTurnOriginalDirection = _state.QuickTurnOriginalDirection;
					Vector3 quickTurnDesiredDirection = _state.QuickTurnDesiredDirection;
					bool flag32 = flag31 && num7 >= 0f && rawWorldMoveDirection.sqrMagnitude > 0.0001f && quickTurnOriginalDirection.sqrMagnitude > 0.0001f;
					bool num12 = (_config.QuickTurnReverseEnabled & flag32) && num7 < _config.QuickTurnReverseMaxProgress && Vector3.Angle(rawWorldMoveDirection, quickTurnOriginalDirection) <= _config.QuickTurnReverseAngle;
					bool flag33 = (_config.QuickTurnRedirectEnabled & flag32) && quickTurnDesiredDirection.sqrMagnitude > 0.0001f && Vector3.Angle(rawWorldMoveDirection, quickTurnDesiredDirection) > _config.QuickTurnRedirectMinAngle;
					_redirectDeviationSteps = (flag33 ? (_redirectDeviationSteps + 1) : 0);
					bool num13 = flag33 && _redirectDeviationSteps >= 2;
					Vector3 forward = _transform.forward;
					forward.y = 0f;
					float f = ((forward.sqrMagnitude > 0.0001f) ? Vector3.SignedAngle(forward, rawWorldMoveDirection, Vector3.up) : 0f);
					float quickTurnYawSign = _state.QuickTurnYawSign;
					float num14 = ((quickTurnYawSign == 0f) ? Mathf.Abs(f) : ((Mathf.Sign(f) == quickTurnYawSign) ? Mathf.Abs(f) : (360f - Mathf.Abs(f))));
					bool flag34 = num13 && Mathf.Abs(f) <= _config.QuickTurnRedirectCutAngle;
					bool flag35 = num13 && quickTurnYawSign != 0f && num14 > _config.QuickTurnRedirectMaxArc;
					if (num12 | flag34 | flag35)
					{
						_reverseLatched = true;
						_reverseTimer = 0f;
						_redirectDeviationSteps = 0;
						_state.LatchQuickTurnCutTarget(rawWorldMoveDirection);
						_state.AlignDesiredDirectionToRaw();
					}
				}
				else
				{
					_reverseTimer += Time.deltaTime;
					Vector3 quickTurnCutTarget = _state.QuickTurnCutTarget;
					Vector3 forward2 = _transform.forward;
					forward2.y = 0f;
					float num15 = ((forward2.sqrMagnitude > 0.0001f && quickTurnCutTarget.sqrMagnitude > 0.0001f) ? Mathf.Abs(Vector3.SignedAngle(forward2, quickTurnCutTarget, Vector3.up)) : 0f);
					if ((!flag31 && !flag22) || num15 <= _config.QuickTurnReverseSettleAngle || _reverseTimer >= _config.QuickTurnReverseMaxDuration)
					{
						_reverseLatched = false;
						_reverseTimer = 0f;
					}
				}
				goto IL_0cd2;
			}
			num6 = 0;
			goto IL_0cb8;
			IL_0d77:
			bool flag36 = (byte)num9 != 0;
			bool flag37 = ((uint)num8 & (flag ? 1u : 0u)) != 0 && IsSpecialState(_animator.GetNextAnimatorStateInfo(0));
			bool flag38 = ((uint)num8 & (flag ? 1u : 0u)) != 0 && IsSpecialState(_animator.GetCurrentAnimatorStateInfo(0)) && !IsSpecialState(_animator.GetNextAnimatorStateInfo(0));
			bool flag39 = flag36 | flag37 | flag38;
			bool flag40 = flag7 | flag14 | flag16 | flag17 | flag26 | flag29 | flag31 | flag23 | flag39;
			_animator.applyRootMotion = flag40;
			if ((bool)_rootMotion)
			{
				_rootMotion.PassThrough = false;
				_rootMotion.Active = flag40;
				if (!flag40)
				{
					_rootMotion.ResetDelta();
				}
			}
			float startExitBlend = (flag15 ? _animator.GetAnimatorTransitionInfo(0).normalizedTime : 0f);
			float quickTurnBlend = (flag21 ? 1f : (flag30 ? _animator.GetAnimatorTransitionInfo(0).normalizedTime : 0f));
			float quickTurnExitBlend = (flag22 ? _animator.GetAnimatorTransitionInfo(0).normalizedTime : 0f);
			CurrentRootMotion = new RootMotionFlags(flag7, flag5, flag5 ? 1f : (flag6 ? _animator.GetAnimatorTransitionInfo(0).normalizedTime : 0f), flag14, flag12, flag12 ? 1f : (flag13 ? _animator.GetAnimatorTransitionInfo(0).normalizedTime : 0f), (((bool)_config && _config.StartTurnEnabled) & flag2) && !flag19, flag19, crouchTurnStart, flag17, startExitBlend, flag26 | flag29, flag24 | flag27, (flag24 | flag27) ? 1f : ((flag25 | flag28) ? _animator.GetAnimatorTransitionInfo(0).normalizedTime : 0f), flag31, flag21, quickTurnBlend, flag23, quickTurnExitBlend, _reverseLatched, quickTurnCommitActive, num7, flag39, _startRollback.Uncommitted, _startRollback.Active);
			UpdateIdleBreaks(snapshot);
		}

		private void UpdateIdleBreaks(LocomotionStateSnapshot snapshot)
		{
			if (!_config || !_config.IdleBreaksEnabled)
			{
				return;
			}
			if (AnyKitWorn())
			{
				_idleDwellTimer = 0f;
				_nextBreakDelay = 0f;
				return;
			}
			if (!snapshot.IsStopped || snapshot.Gait != 0 || !snapshot.IsGrounded || snapshot.IsStarting || snapshot.IsJumping || snapshot.IsCrouching || snapshot.IsTurningInPlace)
			{
				_idleDwellTimer = 0f;
				_nextBreakDelay = 0f;
				return;
			}
			if (_animator.IsInTransition(0) || !_animator.GetCurrentAnimatorStateInfo(0).IsName("Idle_Standing"))
			{
				_idleDwellTimer = 0f;
				return;
			}
			if (_nextBreakDelay <= 0f)
			{
				_nextBreakDelay = Random.Range(_config.IdleBreakFirstDelayMin, _config.IdleBreakFirstDelayMax);
			}
			_idleDwellTimer += Time.deltaTime;
			if (!(_idleDwellTimer < _nextBreakDelay))
			{
				int maxExclusive = Mathf.Max(1, _config.IdleBreakCount);
				_animator.SetInteger(IdleBreakIndexHash, Random.Range(0, maxExclusive));
				_animator.SetTrigger(PlayIdleBreakHash);
				_idleDwellTimer = 0f;
				_nextBreakDelay = Random.Range(_config.IdleBreakMinInterval, _config.IdleBreakMaxInterval);
			}
		}

		private bool AnyKitWorn()
		{
			if (_kitLayers == null)
			{
				return false;
			}
			for (int i = 0; i < _kitLayers.Length; i++)
			{
				if ((bool)_kitLayers[i] && _kitLayers[i].IsWorn)
				{
					return true;
				}
			}
			return false;
		}

		private void UpdateStopFoot()
		{
			_diagStopPhase = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1f;
			bool flag = false;
			_diagVelPickValid = false;
			if (ResolveFootBones())
			{
				Vector3 forward = _transform.forward;
				forward.y = 0f;
				if (forward.sqrMagnitude > 0.0001f)
				{
					forward.Normalize();
					Vector3 position = _leftFoot.position;
					Vector3 position2 = _rightFoot.position;
					_diagPosDot = Vector3.Dot(position - position2, forward);
					_diagLeftFootForward = _diagPosDot > 0f;
					_stopFootFromBones = true;
					flag = true;
					float deltaTime = Time.deltaTime;
					bool flag2 = _hasPrevFootPos && deltaTime > 0f;
					if (flag2)
					{
						_diagLeftFwdVel = Vector3.Dot((position - _prevLeftFootPos) / deltaTime, forward);
						_diagRightFwdVel = Vector3.Dot((position2 - _prevRightFootPos) / deltaTime, forward);
						flag2 = Mathf.Abs(_diagLeftFwdVel) < 15f && Mathf.Abs(_diagRightFwdVel) < 15f;
						_diagVelLeftForward = _diagLeftFwdVel > _diagRightFwdVel;
						_diagVelPickValid = flag2;
					}
					float num = (_config ? _config.StopFootLevelDeadzone : 0.15f);
					bool stopOnLeftFoot = ((Mathf.Abs(_diagPosDot) > num) ? (_stopFootLatched = _diagPosDot > 0f) : ((!flag2) ? _stopFootLatched : (_stopFootLatched = !_diagVelLeftForward)));
					_stopOnLeftFoot = stopOnLeftFoot;
					_prevLeftFootPos = position;
					_prevRightFootPos = position2;
					_hasPrevFootPos = true;
				}
			}
			if (!flag)
			{
				_stopOnLeftFoot = _diagStopPhase < 0.5f;
				_stopFootFromBones = false;
				_hasPrevFootPos = false;
			}
			_stopCycleOffset = ComputeStopCycleOffset();
		}

		private void EnterPhaseMatchedWalkStop(LocomotionStateSnapshot snapshot, bool stopRequested)
		{
			if (!stopRequested)
			{
				_walkStopEntryPending = false;
				return;
			}
			if (!_prevStopRequested)
			{
				_walkStopEntryPending = true;
			}
			if (!_walkStopEntryPending || !snapshot.IsGrounded || snapshot.MovementInputTapped || snapshot.IsJumping || snapshot.IsStrafing)
			{
				return;
			}
			AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			if (IsStopState(currentAnimatorStateInfo))
			{
				return;
			}
			bool flag = IsWalkStartState(currentAnimatorStateInfo) || IsRunStartState(currentAnimatorStateInfo);
			if (!IsLocoState(currentAnimatorStateInfo) && !flag)
			{
				return;
			}
			bool flag2 = false;
			if (_animator.IsInTransition(0))
			{
				AnimatorStateInfo nextAnimatorStateInfo = _animator.GetNextAnimatorStateInfo(0);
				if (IsStopState(nextAnimatorStateInfo))
				{
					_walkStopEntryPending = false;
					return;
				}
				if (!flag || !IsLocoState(nextAnimatorStateInfo))
				{
					return;
				}
				flag2 = true;
			}
			if ((!flag || !_startRollback.OwnsStart) && (flag || snapshot.Gait < 2))
			{
				_walkStopEntryPending = false;
				bool flag3 = (_diagVelPickValid ? (!_diagVelLeftForward) : _stopOnLeftFoot);
				float fixedTimeOffset = ResolveWalkStopEntryTime(flag3);
				float fixedTransitionDuration = (_config ? Mathf.Max(0f, _config.WalkStopEntryFade) : 0.225f);
				int num = (flag3 ? WalkStopLeftStateHash : WalkStopRightStateHash);
				if (_animator.HasState(0, num))
				{
					_animator.CrossFadeInFixedTime(num, fixedTransitionDuration, 0, fixedTimeOffset);
				}
				else
				{
					_animator.CrossFadeInFixedTime(flag3 ? "Base Layer.Locomotion.Walk_Stop_LFoot" : "Base Layer.Locomotion.Walk_Stop_RFoot", fixedTransitionDuration, 0, fixedTimeOffset);
				}
			}
		}

		private float ResolveWalkStopEntryTime(bool onLeftFoot)
		{
			if (!_stopFootFromBones || !_config)
			{
				return 0f;
			}
			AnimationCurve walkStopEntryTimeByPosDot = _config.WalkStopEntryTimeByPosDot;
			if (walkStopEntryTimeByPosDot == null || walkStopEntryTimeByPosDot.length < 2)
			{
				return 0f;
			}
			float value = (onLeftFoot ? _diagPosDot : (0f - _diagPosDot));
			float time = walkStopEntryTimeByPosDot.keys[0].time;
			float time2 = walkStopEntryTimeByPosDot.keys[walkStopEntryTimeByPosDot.length - 1].time;
			return Mathf.Max(0f, walkStopEntryTimeByPosDot.Evaluate(Mathf.Clamp(value, time, time2)));
		}

		private float ResolveStopCycleOffset()
		{
			if (!_config)
			{
				return _stopCycleOffset;
			}
			if (_state == null || _state.StopGait < 2)
			{
				return _config.StopClipStartOffset;
			}
			return _config.RunStopClipStartOffset;
		}

		private float ComputeStopCycleOffset()
		{
			float num = (_config ? _config.StopOffsetScale : 0.25f);
			float num2 = (_config ? _config.StopPhaseRefLeft : 0.13f);
			float num3 = (_config ? _config.StopPhaseRefRight : 0.53f);
			float max = (_config ? _config.StopOffsetMax : 0.2f);
			float num4 = (_stopOnLeftFoot ? num2 : num3);
			return Mathf.Clamp(Mathf.Repeat(_diagStopPhase - num4, 1f) * num, 0f, max);
		}

		private bool ResolveFootBones()
		{
			if ((bool)_leftFoot && (bool)_rightFoot)
			{
				return true;
			}
			if (!_animator || !_animator.isHuman)
			{
				return false;
			}
			_leftFoot = _animator.GetBoneTransform(HumanBodyBones.LeftFoot);
			_rightFoot = _animator.GetBoneTransform(HumanBodyBones.RightFoot);
			if ((bool)_leftFoot)
			{
				return _rightFoot;
			}
			return false;
		}

		private bool ResolveSpineBone()
		{
			if ((bool)_spine)
			{
				return true;
			}
			if (!_animator || !_animator.isHuman)
			{
				return false;
			}
			_spine = _animator.GetBoneTransform(HumanBodyBones.Spine);
			return _spine;
		}

		public void ApplyProceduralLean()
		{
			if (_frozen || _state == null || !_state.IsActive || !ResolveAnimator() || !ResolveSpineBone())
			{
				return;
			}
			float num = 0f;
			if ((bool)_config && _config.ProceduralLeanEnabled)
			{
				num = _state.Snapshot.SlopeLeanAngle;
				if ((_animator.updateMode != AnimatorUpdateMode.AnimatePhysics || _lastLeanStamp != Time.fixedUnscaledTime) && Mathf.Abs(num) >= 0.01f)
				{
					_spine.rotation = Quaternion.AngleAxis(num, _transform.right) * _spine.rotation;
					_lastLeanStamp = Time.fixedUnscaledTime;
				}
			}
		}

		private bool ResolveHeadBone()
		{
			if ((bool)_head)
			{
				return true;
			}
			if (!_animator || !_animator.isHuman)
			{
				return false;
			}
			_head = _animator.GetBoneTransform(HumanBodyBones.Head);
			return _head;
		}

		private void DiagnoseSlopeLean(float angle)
		{
			if (_slopeLeanDiagFrames < 300 && ResolveHeadBone())
			{
				GroundInfo ground = _state.Ground;
				if (!(ground.SlopeAngle < 1f) && !(_state.Snapshot.MoveSpeed <= 0.3f))
				{
					float num = Vector3.SignedAngle(_transform.forward, _head.rotation * Vector3.forward, _transform.right);
					float num2 = _head.position.y - _transform.position.y;
					float num3 = (_hasSlopeLeanDiagPrev ? (angle - _diagPrevLeanAngle) : 0f);
					float num4 = (_hasSlopeLeanDiagPrev ? Mathf.DeltaAngle(_diagPrevHeadPitch, num) : 0f);
					float num5 = (_hasSlopeLeanDiagPrev ? (num2 - _diagPrevHeadRise) : 0f);
					_diagPrevLeanAngle = angle;
					_diagPrevHeadPitch = num;
					_diagPrevHeadRise = num2;
					_hasSlopeLeanDiagPrev = true;
					_slopeLeanDiagFrames++;
					LocomotionStateSnapshot snapshot = _state.Snapshot;
					Debug.Log($"[SlopeLean] f={Time.frameCount} t={Time.time:F3} ft={Time.fixedTime:F3} " + $"ang={angle:F3} dAng={num3:F3} hPitch={num:F2} dPitch={num4:F3} " + $"hRise={num2:F4} dRise={num5:F4} " + $"grounded={ground.Grounded} slope={ground.SlopeAngle:F1} n={ground.Normal} " + $"held={snapshot.MovementInputHeld} ms={snapshot.MoveSpeed:F2}");
				}
			}
		}

		private void DiagnoseStop(LocomotionStateSnapshot snapshot)
		{
			bool isGrounded = snapshot.IsGrounded;
			bool flag = snapshot.IsStopped && !_prevStopped;
			bool flag2 = isGrounded && !_prevGrounded;
			if (flag | flag2)
			{
				_stopDiagFrames = 40;
				_diagHasStopFootPrev = false;
				string text = (flag ? "STOP-edge" : "LAND-edge");
				string text2 = (_diagLeftFootForward ? "L" : "R");
				string text3 = ((!_diagVelPickValid) ? "?" : (_diagVelLeftForward ? "L" : "R"));
				string text4 = ((!_diagVelPickValid) ? "NA" : ((_diagLeftFootForward == _diagVelLeftForward) ? "AGREE" : "DISAGREE"));
				Debug.Log($"[StopDiag] {text} gateMatch={_state.LastGateAligned} gateTimer={_state.LastGateTimer:F2} firePosDot={_state.LastGateFirePosDot:F3} target={_state.LastGateTarget:F3} posPick={text2} velPick={text3} {text4} posDot={_diagPosDot:F3} lFwdV={_diagLeftFwdVel:F2} rFwdV={_diagRightFwdVel:F2} bodyV={BodyFlatSpeed():F2} fromBones={_stopFootFromBones} phase={_diagStopPhase:F2} off={_stopCycleOffset:F3} offLive={ResolveStopCycleOffset():F3} dz={(_config ? _config.StopFootLevelDeadzone : 0.15f):F2} stopLeft={_stopOnLeftFoot} gait={snapshot.Gait} curGait={_animator.GetInteger(CurrentGaitHash)} ms={snapshot.MoveSpeed:F2} animMS={_animator.GetFloat(MoveSpeedHash):F2} held={snapshot.MovementInputHeld} grounded={isGrounded} jump={snapshot.IsJumping}");
			}
			if (_stopDiagFrames > 0)
			{
				_stopDiagFrames--;
				AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
				bool flag3 = _animator.IsInTransition(0);
				string text5 = "-";
				if (flag3)
				{
					text5 = StopStateLabel(_animator.GetNextAnimatorStateInfo(0));
				}
				float num = StopFootSlideXZ();
				Debug.Log($"[StopDiag] cur={StopStateLabel(currentAnimatorStateInfo)} t={currentAnimatorStateInfo.normalizedTime % 1f:F3} pd={ComputeLiveFootPosDot():F3} trans={flag3} next={text5} bodyV={BodyFlatSpeed():F2} footSlideXZ={num:F3} rootAct={CurrentRootMotion.StopActive} rootSet={CurrentRootMotion.StopSettled} blend={CurrentRootMotion.StopBlend:F2} | gait={snapshot.Gait} curGait={_animator.GetInteger(CurrentGaitHash)} ms={snapshot.MoveSpeed:F2} stopped={snapshot.IsStopped} held={snapshot.MovementInputHeld} grounded={snapshot.IsGrounded} jump={snapshot.IsJumping}");
			}
			_prevStopped = snapshot.IsStopped;
			_prevGrounded = isGrounded;
		}

		private float BodyFlatSpeed()
		{
			if (!_rigidbody)
			{
				return -1f;
			}
			Vector3 velocity = _rigidbody.velocity;
			velocity.y = 0f;
			return velocity.magnitude;
		}

		private float ComputeLiveFootPosDot()
		{
			if (!ResolveFootBones())
			{
				return -999f;
			}
			Vector3 forward = _transform.forward;
			forward.y = 0f;
			if (forward.sqrMagnitude < 0.0001f)
			{
				return -999f;
			}
			forward.Normalize();
			return Vector3.Dot(_leftFoot.position - _rightFoot.position, forward);
		}

		private float StopFootSlideXZ()
		{
			if (!ResolveFootBones())
			{
				_diagHasStopFootPrev = false;
				return -1f;
			}
			Vector3 position = (_stopOnLeftFoot ? _leftFoot : _rightFoot).position;
			float result = -1f;
			if (_diagHasStopFootPrev)
			{
				Vector3 vector = position - _diagPrevStopFootWorld;
				vector.y = 0f;
				result = vector.magnitude;
			}
			_diagPrevStopFootWorld = position;
			_diagHasStopFootPrev = true;
			return result;
		}

		private static string StopStateLabel(AnimatorStateInfo st)
		{
			if (st.IsName("Idle_Standing"))
			{
				return "idle";
			}
			if (st.IsName("LocomotionBlendTree"))
			{
				return "loco";
			}
			if (st.IsName("Walk_Stop_LFoot"))
			{
				return "walkStopL";
			}
			if (st.IsName("Walk_Stop_RFoot"))
			{
				return "walkStopR";
			}
			if (st.IsName("Run_Stop_LFoot"))
			{
				return "runStopL";
			}
			if (st.IsName("Run_Stop_RFoot"))
			{
				return "runStopR";
			}
			if (st.IsName("Crouch_Stop"))
			{
				return "crouchStop";
			}
			if (st.IsName("LandingDefault"))
			{
				return "landDef";
			}
			if (st.IsName("LandingWalk"))
			{
				return "landWalk";
			}
			if (st.IsName("LandingRun"))
			{
				return "landRun";
			}
			if (st.IsName("LandingSoft"))
			{
				return "landSoft";
			}
			if (st.IsName("LandingMedium"))
			{
				return "landMed";
			}
			if (st.IsName("LandingHard"))
			{
				return "landHard";
			}
			if (st.IsName("LandingSprint"))
			{
				return "landSprint";
			}
			return "?";
		}

		private void DiagnoseStart(LocomotionStateSnapshot snapshot)
		{
			if (snapshot.IsStarting && !_prevStarting)
			{
				_diagFrames = 80;
				Debug.Log($"[StartDiag] RISING StartDir={snapshot.LocomotionStartDirection:F1} Gait={snapshot.Gait} MoveSpeed={snapshot.MoveSpeed:F2}");
			}
			if (_diagFrames > 0)
			{
				_diagFrames--;
				AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
				bool flag = _animator.IsInTransition(0);
				string text = StateLabel(currentAnimatorStateInfo);
				if (text == "?")
				{
					text = StopStateLabel(currentAnimatorStateInfo);
				}
				string text2 = "-";
				float num = 0f;
				float num2 = 0f;
				if (flag)
				{
					AnimatorStateInfo nextAnimatorStateInfo = _animator.GetNextAnimatorStateInfo(0);
					text2 = StateLabel(nextAnimatorStateInfo);
					if (text2 == "?")
					{
						text2 = StopStateLabel(nextAnimatorStateInfo);
					}
					num = nextAnimatorStateInfo.normalizedTime % 1f;
					num2 = nextAnimatorStateInfo.length;
				}
				Vector3 desiredWorldMoveDirection = _state.DesiredWorldMoveDirection;
				bool flag2 = _input != null && _input.Current.HasMove;
				float num3 = (_rigidbody ? _rigidbody.velocity.magnitude : (-1f));
				Debug.Log($"[StartDiag] cur={text} t={currentAnimatorStateInfo.normalizedTime % 1f:F2} len={currentAnimatorStateInfo.length:F2} trans={flag} next={text2} nt={num:F2} nlen={num2:F2} | msSnap={snapshot.MoveSpeed:F2} msAnim={_animator.GetFloat(MoveSpeedHash):F2} gait={snapshot.Gait} start={snapshot.IsStarting} walk={snapshot.IsWalking} held={snapshot.MovementInputHeld} | dir=({desiredWorldMoveDirection.x:F2},{desiredWorldMoveDirection.z:F2}) move={flag2} vel={num3:F2}");
			}
			_prevStarting = snapshot.IsStarting;
		}

		private void DiagnoseResume(LocomotionStateSnapshot snapshot)
		{
			bool flag = _input != null && _input.Current.HasMove;
			if (flag && !_prevHasMove)
			{
				_resumeDiagFrames = 20;
				AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
				string text = StateLabel(currentAnimatorStateInfo);
				if (text == "?")
				{
					text = StopStateLabel(currentAnimatorStateInfo);
				}
				Debug.Log($"[ResumeEdge] REPRESS cur={text} t={currentAnimatorStateInfo.normalizedTime % 1f:F2} bodyV={BodyFlatSpeed():F2} ms={snapshot.MoveSpeed:F2} stopped={snapshot.IsStopped} starting={snapshot.IsStarting} rootAct={CurrentRootMotion.StopActive} rootSet={CurrentRootMotion.StopSettled} grounded={snapshot.IsGrounded}");
			}
			if (_resumeDiagFrames > 0)
			{
				_resumeDiagFrames--;
				AnimatorStateInfo currentAnimatorStateInfo2 = _animator.GetCurrentAnimatorStateInfo(0);
				bool flag2 = _animator.IsInTransition(0);
				string text2 = StateLabel(currentAnimatorStateInfo2);
				if (text2 == "?")
				{
					text2 = StopStateLabel(currentAnimatorStateInfo2);
				}
				string text3 = "-";
				if (flag2)
				{
					AnimatorStateInfo nextAnimatorStateInfo = _animator.GetNextAnimatorStateInfo(0);
					text3 = StateLabel(nextAnimatorStateInfo);
					if (text3 == "?")
					{
						text3 = StopStateLabel(nextAnimatorStateInfo);
					}
				}
				Debug.Log($"[ResumeEdge] cur={text2} trans={flag2} next={text3} bodyV={BodyFlatSpeed():F2} ms={snapshot.MoveSpeed:F2} stopped={snapshot.IsStopped} starting={snapshot.IsStarting} rootAct={CurrentRootMotion.StopActive}");
			}
			_prevHasMove = flag;
		}

		private static string StateLabel(AnimatorStateInfo st)
		{
			if (st.IsName("Idle_Standing"))
			{
				return "idle";
			}
			if (st.IsName("Walk_Start_F360"))
			{
				return "walkStart";
			}
			if (st.IsName("Run_Start_F360"))
			{
				return "runStart";
			}
			if (st.IsName("LocomotionBlendTree"))
			{
				return "loco";
			}
			if (st.IsName("CrouchingBlendTree"))
			{
				return "crouchLoco";
			}
			return "?";
		}

		private bool ResolveAnimator()
		{
			Animator animator = null;
			if ((bool)_mainPerson)
			{
				PersonAnimationsController personAnimationsController = _mainPerson.PersonAnimationsController;
				animator = (((bool)personAnimationsController && (bool)personAnimationsController.Animator) ? personAnimationsController.Animator : _mainPerson.Animator);
			}
			if ((object)animator != _animator)
			{
				_animator = animator;
				_swapped = false;
				_controllerApplied = false;
				_originalController = null;
				_rootMotion = null;
				_leftFoot = null;
				_rightFoot = null;
				_hasPrevFootPos = false;
				_stopFootLatched = false;
				_reverseLatched = false;
				_reverseTimer = 0f;
			}
			return _animator;
		}

		private void EnsureRootMotion()
		{
			if ((bool)_animator && !_rootMotion && !_animator.TryGetComponent<PersonRootMotion>(out _rootMotion))
			{
				_rootMotion = _animator.gameObject.AddComponent<PersonRootMotion>();
				_animator.RefreshPhotonRpcCache();
			}
		}

		public Vector3 ConsumeRootDelta()
		{
			if (!_rootMotion)
			{
				return Vector3.zero;
			}
			return _rootMotion.ConsumeHorizontalDelta();
		}

		public Quaternion ConsumeRootRotation()
		{
			if (!_rootMotion)
			{
				return Quaternion.identity;
			}
			return _rootMotion.ConsumeRotation();
		}

		private static bool IsStopState(AnimatorStateInfo st)
		{
			if (!st.IsName("Walk_Stop_LFoot") && !st.IsName("Walk_Stop_RFoot") && !st.IsName("Run_Stop_LFoot") && !st.IsName("Run_Stop_RFoot"))
			{
				return st.IsName("Crouch_Stop");
			}
			return true;
		}

		private void DiagnoseFootSkate(LocomotionStateSnapshot snapshot, bool inStart, bool exitingStart, bool inLoco, bool locoSettled)
		{
			if ((inStart | exitingStart) || (inLoco && !locoSettled))
			{
				_skateTailFrames = 30;
			}
			else
			{
				if (_skateTailFrames <= 0)
				{
					_hasSkatePrev = false;
					return;
				}
				_skateTailFrames--;
			}
			if (!ResolveFootBones())
			{
				_hasSkatePrev = false;
				return;
			}
			Vector3 position = _leftFoot.position;
			Vector3 position2 = _rightFoot.position;
			float deltaTime = Time.deltaTime;
			if (!_hasSkatePrev || deltaTime <= 0f)
			{
				_skatePrevLeft = position;
				_skatePrevRight = position2;
				_hasSkatePrev = true;
				return;
			}
			Vector3 vector = position - _skatePrevLeft;
			Vector3 vector2 = position2 - _skatePrevRight;
			vector.y = 0f;
			vector2.y = 0f;
			float num = vector.magnitude / deltaTime;
			float num2 = vector2.magnitude / deltaTime;
			bool flag = position.y <= position2.y;
			_skatePrevLeft = position;
			_skatePrevRight = position2;
			AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			string text = StateLabel(currentAnimatorStateInfo);
			if (text == "?")
			{
				text = StopStateLabel(currentAnimatorStateInfo);
			}
			float num3 = (_animator.IsInTransition(0) ? _animator.GetAnimatorTransitionInfo(0).normalizedTime : 0f);
			float y = _transform.position.y;
			Debug.Log(string.Format("[FootSkate] {0} blend={1:F2} foot={2} ", text, num3, flag ? "L" : "R") + $"skate={(flag ? num : num2):F2} lSk={num:F2} rSk={num2:F2} " + $"lY={position.y - y:F3} rY={position2.y - y:F3} bodyV={BodyFlatSpeed():F2} " + $"ms={snapshot.MoveSpeed:F2} gait={snapshot.Gait}");
		}

		private static bool IsStartState(AnimatorStateInfo st)
		{
			if (!st.IsName("Walk_Start_F360") && !st.IsName("Run_Start_F360"))
			{
				return st.IsName("Crouch_Start");
			}
			return true;
		}

		private static bool IsWalkStartState(AnimatorStateInfo st)
		{
			return st.IsName("Walk_Start_F360");
		}

		private static bool IsRunStartState(AnimatorStateInfo st)
		{
			return st.IsName("Run_Start_F360");
		}

		private static bool IsCrouchStartState(AnimatorStateInfo st)
		{
			return st.IsName("Crouch_Start");
		}

		private bool ResolveInstantStopEscape()
		{
			if (_state == null || !_state.InstantStopEngaged)
			{
				_ = _instantStopFiring;
				_instantStopFiring = false;
				InstantStopEscaping = false;
				return false;
			}
			AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			if (!IsLocoState(currentAnimatorStateInfo) && !IsStandingStartState(currentAnimatorStateInfo) && !IsQuickTurnState(currentAnimatorStateInfo))
			{
				_ = _instantStopFiring;
				_instantStopFiring = false;
				InstantStopEscaping = false;
				return false;
			}
			if (!_animator.IsInTransition(0))
			{
				float fixedTransitionDuration = Mathf.Max(0f, _config ? _config.InstantStopBlendTime : 0f);
				if (_animator.HasState(0, IdleStandingHash))
				{
					_animator.CrossFadeInFixedTime(IdleStandingHash, fixedTransitionDuration, 0);
				}
				else
				{
					_animator.CrossFadeInFixedTime("Base Layer.Idle.Idle_Standing", fixedTransitionDuration, 0);
				}
			}
			_instantStopFiring = true;
			InstantStopEscaping = true;
			return true;
		}

		private void TickStartRollback(LocomotionStateSnapshot snapshot, bool inTransition, bool inStandingStartAny)
		{
			bool flag = (bool)_config && _config.StartRollbackEnabled && HasStartSpeedParam();
			StartRollbackInput input = new StartRollbackInput(flag, _config ? _config.StartRollbackSpeed : 1f, inStandingStartAny, ResolveStartNormalizedTime(inTransition), snapshot.StartLatchCount, snapshot.MovementInputTapped || snapshot.MovementInputPressed || snapshot.MovementInputHeld, snapshot.MovementInputHeld, snapshot.IsGrounded, snapshot.IsJumping, Time.deltaTime);
			_startRollback.Tick(in input);
			if (flag)
			{
				_animator.SetFloat(StartPlaybackSpeedHash, _startRollback.PlaybackSpeed);
			}
			else if (_startSpeedEngaged && HasStartSpeedParam())
			{
				_animator.SetFloat(StartPlaybackSpeedHash, 1f);
			}
			_startSpeedEngaged = flag;
			if (_startRollback.CompletedThisFrame)
			{
				float fixedTransitionDuration = Mathf.Max(0f, _config ? _config.StartRollbackExitBlend : 0f);
				if (_animator.HasState(0, IdleStandingHash))
				{
					_animator.CrossFadeInFixedTime(IdleStandingHash, fixedTransitionDuration, 0);
				}
				else
				{
					_animator.CrossFadeInFixedTime("Base Layer.Idle.Idle_Standing", fixedTransitionDuration, 0);
				}
			}
		}

		private void ResetStartRollback()
		{
			_startRollback.Reset();
			_startSpeedEngaged = false;
			if (HasStartSpeedParam())
			{
				_animator.SetFloat(StartPlaybackSpeedHash, 1f);
			}
		}

		private bool HasStartSpeedParam()
		{
			if (_startSpeedParam != 0)
			{
				return _startSpeedParam == 1;
			}
			bool flag = false;
			AnimatorControllerParameter[] parameters = _animator.parameters;
			for (int i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].nameHash == StartPlaybackSpeedHash)
				{
					flag = parameters[i].type == AnimatorControllerParameterType.Float;
					break;
				}
			}
			_startSpeedParam = (flag ? 1 : 2);
			return flag;
		}

		private float ResolveStartNormalizedTime(bool inTransition)
		{
			AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
			if (IsStartState(currentAnimatorStateInfo))
			{
				return currentAnimatorStateInfo.normalizedTime;
			}
			if (!inTransition)
			{
				return 0f;
			}
			AnimatorStateInfo nextAnimatorStateInfo = _animator.GetNextAnimatorStateInfo(0);
			if (!IsStartState(nextAnimatorStateInfo))
			{
				return 0f;
			}
			return nextAnimatorStateInfo.normalizedTime;
		}

		private static bool IsStandingStartState(AnimatorStateInfo st)
		{
			if (!st.IsName("Walk_Start_F360"))
			{
				return st.IsName("Run_Start_F360");
			}
			return true;
		}

		private static bool IsQuickTurnState(AnimatorStateInfo st)
		{
			if (!st.IsName("Walk_Turn_180L") && !st.IsName("Walk_Turn_180R") && !st.IsName("Run_Turn_180L"))
			{
				return st.IsName("Run_Turn_180R");
			}
			return true;
		}

		private static bool IsLocoState(AnimatorStateInfo st)
		{
			return st.IsName("LocomotionBlendTree");
		}

		private static bool IsSpecialState(AnimatorStateInfo st)
		{
			return st.IsName("SpecialAnimations");
		}

		private static bool IsCrouchLocoState(AnimatorStateInfo st)
		{
			return st.IsName("CrouchingBlendTree");
		}

		private static float StateCrouchAmount(AnimatorStateInfo st)
		{
			if (st.IsName("Stand_ToCrouch"))
			{
				return Mathf.Clamp01(st.normalizedTime);
			}
			if (st.IsName("Crouch_ToStand"))
			{
				return Mathf.Clamp01(1f - st.normalizedTime);
			}
			if (st.IsName("Sprint_ToCrouch") || st.IsName("Idle_Crouching") || st.IsName("CrouchingBlendTree") || st.IsName("Crouch_Start") || st.IsName("Crouch_Stop") || st.IsName("ShuffleCrouchingBlendTree") || st.IsName("Shuffle_Crouching") || st.IsName("TurnInPlace_Crouching") || st.IsName("Crouch_FwdStrafe_BlendTree") || st.IsName("Crouch_BckStrafe_BlendTree"))
			{
				return 1f;
			}
			return 0f;
		}

		private void ApplyControllerOnce()
		{
			if (_controllerApplied)
			{
				return;
			}
			RuntimeAnimatorController runtimeAnimatorController = (((bool)_mainPerson && (int)_mainPerson.Gender == 1) ? _maleController : _femaleController);
			if (!runtimeAnimatorController)
			{
				return;
			}
			RuntimeAnimatorController runtimeAnimatorController2 = _animator.runtimeAnimatorController;
			if (runtimeAnimatorController2 is AnimatorOverrideController animatorOverrideController)
			{
				if ((object)animatorOverrideController.runtimeAnimatorController != runtimeAnimatorController)
				{
					if (!_swapped)
					{
						_originalController = animatorOverrideController.runtimeAnimatorController;
						_originalRootMotion = _animator.applyRootMotion;
						_swapped = true;
					}
					animatorOverrideController.runtimeAnimatorController = runtimeAnimatorController;
					_startSpeedParam = 0;
				}
				_controllerApplied = true;
			}
			else if ((object)runtimeAnimatorController2 == runtimeAnimatorController)
			{
				_controllerApplied = true;
			}
			else
			{
				_originalController = runtimeAnimatorController2;
				_originalRootMotion = _animator.applyRootMotion;
				_animator.runtimeAnimatorController = runtimeAnimatorController;
				_startSpeedParam = 0;
				_swapped = true;
				_controllerApplied = true;
			}
		}

		private bool RagdollOwnsRig()
		{
			if (!_ragdollCpm && (bool)_transform)
			{
				DriverActionController component = _transform.GetComponent<DriverActionController>();
				_ragdollCpm = (component ? component.ragdollCpm : null);
			}
			if ((bool)_ragdollCpm)
			{
				if (!_ragdollCpm.isRagdoll)
				{
					return _ragdollCpm.isStadningUp;
				}
				return true;
			}
			return false;
		}

		private bool CarInteractionAnimating()
		{
			if (!_mainPerson)
			{
				return false;
			}
			PersonAnimationsController personAnimationsController = _mainPerson.PersonAnimationsController;
			if (!personAnimationsController || personAnimationsController.SitGetOutCarSystem == null)
			{
				return false;
			}
			SitGetOutData sitGetOutData = personAnimationsController.SitGetOutCarSystem.SitGetOutData;
			if (sitGetOutData == null)
			{
				return false;
			}
			if (!sitGetOutData.IsSittingIntoCarAnimate)
			{
				return sitGetOutData.IsGettingOutOfTheCarAnimate;
			}
			return true;
		}

		private void CancelEmoteWhenInterrupted()
		{
			if ((bool)_mainPerson)
			{
				PersonAnimationsController personAnimationsController = _mainPerson.PersonAnimationsController;
				if (!personAnimationsController || personAnimationsController.CurrentSpecialAnimationId == 0)
				{
					_emoteStateSeen = false;
				}
				else if (_input != null && _input.Current.HasMove)
				{
					_emoteStateSeen = false;
					EmotionController.oldSelected = 0;
					personAnimationsController.StopSpecialAnimation();
					_animator.ResetTrigger(StopAnimationHash);
					_animator.CrossFadeInFixedTime("LocomotionBlendTree", 0.25f, 0);
				}
				else if (IsSpecialState(_animator.GetCurrentAnimatorStateInfo(0)) || (_animator.IsInTransition(0) && IsSpecialState(_animator.GetNextAnimatorStateInfo(0))))
				{
					_emoteStateSeen = true;
				}
				else if (_emoteStateSeen)
				{
					_emoteStateSeen = false;
					EmotionController.oldSelected = 0;
					personAnimationsController.StopSpecialAnimation();
					_animator.ResetTrigger(StopAnimationHash);
				}
			}
		}

		private string DiagStateName(int shortHash)
		{
			for (int i = 0; i < DiagStateNames.Length; i++)
			{
				if (Animator.StringToHash(DiagStateNames[i]) == shortHash)
				{
					return DiagStateNames[i];
				}
			}
			return shortHash.ToString();
		}

		private void EmoteExitDiag()
		{
			if (_emoteDiagFrames > 0 && (bool)_animator)
			{
				_emoteDiagFrames--;
				AnimatorStateInfo currentAnimatorStateInfo = _animator.GetCurrentAnimatorStateInfo(0);
				bool flag = _animator.IsInTransition(0);
				string text = "-";
				if (flag)
				{
					text = DiagStateName(_animator.GetNextAnimatorStateInfo(0).shortNameHash);
				}
				Debug.Log($"[EmoteExit] cur={DiagStateName(currentAnimatorStateInfo.shortNameHash)} next={text} trans={flag} moveSpeed={_animator.GetFloat(MoveSpeedHash):F2} isWalking={_animator.GetBool(IsWalkingHash)} hasMove={_input != null && _input.Current.HasMove}");
			}
		}

		private void RestoreController()
		{
			if (!_swapped)
			{
				_controllerApplied = false;
				return;
			}
			if ((bool)_originalController)
			{
				if (_animator.runtimeAnimatorController is AnimatorOverrideController animatorOverrideController)
				{
					if ((object)animatorOverrideController.runtimeAnimatorController != _originalController)
					{
						animatorOverrideController.runtimeAnimatorController = _originalController;
					}
				}
				else
				{
					_animator.runtimeAnimatorController = _originalController;
				}
			}
			_animator.applyRootMotion = _originalRootMotion;
			_startSpeedParam = 0;
			_swapped = false;
			_controllerApplied = false;
		}

		public void ReleaseToLegacy()
		{
			if (ResolveAnimator())
			{
				_reverseLatched = false;
				_reverseTimer = 0f;
				_hasPrevFootPos = false;
				_stopFootLatched = false;
				_prevIsJumping = false;
				if (_animator.updateMode != AnimatorUpdateMode.Normal)
				{
					_animator.updateMode = AnimatorUpdateMode.Normal;
				}
				if (!RagdollOwnsRig())
				{
					RestoreController();
				}
				if (!_animator.applyRootMotion)
				{
					_animator.applyRootMotion = true;
				}
				if ((bool)_rootMotion)
				{
					_rootMotion.Active = false;
					_rootMotion.PassThrough = true;
					_rootMotion.ResetDelta();
					Object.Destroy(_rootMotion);
					_rootMotion = null;
					_animator.RefreshPhotonRpcCacheAfterDestroy();
				}
			}
		}

		public void PrepareForResume()
		{
			if (ResolveAnimator())
			{
				_controllerApplied = false;
				_hasPrevFootPos = false;
			}
		}
	}
}
