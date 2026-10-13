using System;
using UnityEngine;
using _Car_Parking.Scripts.Person.Locomotion.Config;
using _Car_Parking.Scripts.Person.Locomotion.Input;
using _Car_Parking.Scripts.Person.Locomotion.Stamina;
using _Car_Parking.Scripts.WorldInteraction;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class PersonLocomotionState
	{
		private readonly Transform _transform;

		private readonly PersonLocomotionConfig _locomotion;

		private readonly PersonCapsuleConfig _capsule;

		private readonly PersonInputAdapter _input;

		private readonly PersonGroundProbe _groundProbe;

		private readonly StaminaService _stamina;

		private readonly CapsuleCollider _capsuleCollider;

		private Camera _mainCamera;

		private WorldInteractionController _worldInteraction;

		private bool _worldInteractionResolved;

		private bool _crouchActive;

		private bool _sprintActive;

		private float _coyoteTimer;

		private float _jumpCooldownTimer;

		private float _crouchCooldownTimer;

		private float _fallStartTime;

		private float _fallingDuration;

		private float _descendingDuration;

		private bool _isGrounded;

		private bool _isJumping;

		private bool _jumpRequested;

		private bool _jumpPending;

		private float _jumpWindupTimer;

		private float _jumpPressTime;

		private bool _slideJumpQueued;

		private int _slideJumpGait;

		private bool _slideJumpGaitOverride;

		private bool _landedFromJump;

		private float _landedFromJumpAge;

		private float _landingSpeed;

		private bool _landingArcSession;

		private float _landingArcSessionTimer;

		private float _landingArcTurnTimer;

		private bool _wasMoving;

		private bool _wasGrounded;

		private float _currentCapsuleHeight;

		private float _crouchBlend;

		private float _crouchBlendTarget;

		private bool _crouchSlideActive;

		private float _slideNormalizedTime;

		private bool _crouchFromSlide;

		private bool _slideSeen;

		private float _slideTimer;

		private bool _slideBlocked;

		private float _animatedSpeed;

		private float _animatedSpeedVelocity;

		private float _moveSpeedParam;

		private float _achievedHorizontalSpeed;

		private int _lastAnimatedGait;

		private int _activeGait;

		private int _stopGait;

		private bool _stopWindowActive;

		private bool _stopPending;

		private float _stopPendingTimer;

		private float _stopHoldTimer;

		private float _stopEntrySpeed;

		private float _gatePosDot;

		private float _gatePosDotTrend;

		private float _gatePrevPosDot;

		private bool _gateHasPrevPosDot;

		private bool _landedNoInput;

		private bool _stoppedLatched;

		private float _locomotionStartTimer;

		private float _stopResumeTimer;

		private bool _isStarting;

		private float _locomotionStartDirection;

		private float _startInitialDirection;

		private int _startLatchCount;

		private float _startRefireTimer;

		private float _desiredYaw;

		private float _desiredYawVel;

		private bool _hadRawInput;

		private float _noInputTimer;

		private Vector3 _startLookDir;

		private bool _plantedFootLeft;

		private bool _quickTurnArmed = true;

		private bool _quickTurnActive;

		private bool _quickTurnLeft;

		private Vector3 _quickTurnDesiredDir;

		private Vector3 _quickTurnOriginalDir;

		private Vector3 _quickTurnCutTarget;

		private float _quickTurnInitialGap;

		private float _quickTurnWarpGap;

		private float _quickTurnHold;

		private float _quickTurnTimer;

		private const int QuickTurnSampleCount = 32;

		private readonly Vector3[] _flickDirs = new Vector3[32];

		private readonly float[] _flickTimes = new float[32];

		private int _flickHead;

		private int _flickCount;

		private float _previousYaw;

		private float _leanValue;

		private float _headLookX;

		private float _bodyLookX;

		private bool _leanInitialized;

		private float _slopeInclineRaw;

		private const bool DiagSlopeIncline = false;

		private const int SlopeInclineDiagBudget = 250;

		private int _slopeInclineDiagTicks;

		private WallStopPhase _wallPhase;

		private float _wallPushTimer;

		private bool _wallBlocked;

		private Vector3 _wallNormal;

		private float _realizedHorizontalSpeed;

		private LocomotionStateSnapshot _snapshot;

		public bool IsActive { get; set; }

		public bool LastGateAligned { get; private set; }

		public float LastGateTimer { get; private set; }

		public float LastGateFirePosDot { get; private set; }

		public float LastGateTarget { get; private set; }

		public LocomotionStateSnapshot Snapshot => _snapshot;

		public bool SlideOnCrouch
		{
			get
			{
				if (!_crouchActive && WantsSlide)
				{
					if (_stamina != null)
					{
						return _stamina.CanSlide;
					}
					return true;
				}
				return false;
			}
		}

		public bool IsSlideCrouch => _crouchFromSlide;

		public GroundInfo Ground { get; private set; }

		public Vector3 DesiredWorldMoveDirection { get; private set; }

		public Vector3 RawWorldMoveDirection { get; private set; }

		public Vector3 QuickTurnOriginalDirection => _quickTurnOriginalDir;

		public Vector3 QuickTurnDesiredDirection => _quickTurnDesiredDir;

		public Vector3 QuickTurnCutTarget => _quickTurnCutTarget;

		public float QuickTurnInitialGap => _quickTurnInitialGap;

		public float QuickTurnWarpGap => _quickTurnWarpGap;

		public float QuickTurnYawSign { get; private set; }

		public Vector3 StartLookDirection => _startLookDir;

		public float TargetGaitSpeed { get; private set; }

		public float StopCapSpeed
		{
			get
			{
				if (!_stopWindowActive && !_stopPending)
				{
					return float.PositiveInfinity;
				}
				return GaitSpeed(_stopGait);
			}
		}

		public int StopGait => _stopGait;

		public bool InstantStopEngaged
		{
			get
			{
				if ((bool)_locomotion && _locomotion.InstantStopEnabled && !_crouchActive && _isGrounded && !_isJumping && _input != null)
				{
					return !_input.Current.HasMove;
				}
				return false;
			}
		}

		public bool WallBlocked => _wallBlocked;

		public Vector3 WallNormal => _wallNormal;

		public bool LandingArcActive => _landingArcSession;

		public float LandingArcTurnRate
		{
			get
			{
				if (!_landingArcSession || _landingArcTurnTimer <= 0f)
				{
					return -1f;
				}
				float num = Mathf.Max(0.5f, _locomotion.LandingArcRadius);
				return Mathf.Clamp(_achievedHorizontalSpeed / num * 57.29578f, _locomotion.LandingArcTurnRateMin, _locomotion.LandingArcTurnRateMax);
			}
		}

		public float LandingArcReleaseT
		{
			get
			{
				if (!(_locomotion.LandingArcReleaseTime > 0f))
				{
					if (!(_landingArcTurnTimer > 0f))
					{
						return 1f;
					}
					return 0f;
				}
				return 1f - Mathf.Clamp01(_landingArcTurnTimer / _locomotion.LandingArcReleaseTime);
			}
		}

		private bool WantsSlide => _lastAnimatedGait == 3;

		public PersonLocomotionState(Transform transform, PersonLocomotionConfig locomotion, PersonCapsuleConfig capsule, PersonInputAdapter input, PersonGroundProbe groundProbe, StaminaService stamina, CapsuleCollider capsuleCollider, Camera mainCamera)
		{
			_transform = transform;
			_locomotion = locomotion;
			_capsule = capsule;
			_input = input;
			_groundProbe = groundProbe;
			_stamina = stamina;
			_capsuleCollider = capsuleCollider;
			_mainCamera = mainCamera;
			if ((bool)_capsule)
			{
				_currentCapsuleHeight = _capsule.StandingHeight;
			}
			else if ((bool)_capsuleCollider)
			{
				_currentCapsuleHeight = _capsuleCollider.height;
			}
		}

		public bool ConsumeJumpRequest()
		{
			bool slideJump;
			return ConsumeJumpRequest(out slideJump);
		}

		public bool ConsumeJumpRequest(out bool slideJump)
		{
			if (!_jumpRequested)
			{
				slideJump = false;
				return false;
			}
			_jumpRequested = false;
			slideJump = _slideJumpQueued;
			_slideJumpQueued = false;
			return true;
		}

		public void SetStopGateInputs(float posDot, float posDotTrend)
		{
			_gatePosDot = posDot;
			_gatePosDotTrend = posDotTrend;
		}

		public void SetPlantedFoot(bool stopOnLeftFoot)
		{
			_plantedFootLeft = stopOnLeftFoot;
		}

		public void SetCrouchBlendTarget(float value, bool slideActive, float slideNormalizedTime)
		{
			_crouchBlendTarget = value;
			_crouchSlideActive = slideActive;
			_slideNormalizedTime = slideNormalizedTime;
		}

		public void ReportAchievedHorizontalSpeed(float value)
		{
			_achievedHorizontalSpeed = value;
		}

		public void ReportRealizedHorizontalSpeed(float value)
		{
			_realizedHorizontalSpeed = value;
		}

		private void UpdateWallStop(Vector3 rawMoveDir, bool rawHasMove, int targetGait, float deltaTime)
		{
			if (!_locomotion.WallStopEnabled)
			{
				_wallBlocked = false;
				_wallPhase = WallStopPhase.None;
				_wallPushTimer = 0f;
				return;
			}
			bool flag = rawHasMove && targetGait >= 1 && _isGrounded;
			bool flag2 = false;
			if (flag && _groundProbe != null && _groundProbe.TryDetectWall(rawMoveDir, out var hit))
			{
				Vector3 vector = rawMoveDir;
				vector.y = 0f;
				Vector3 normal = hit.normal;
				normal.y = 0f;
				if (vector.sqrMagnitude > 0.0001f && normal.sqrMagnitude > 0.0001f && Vector3.Angle(vector, -normal) < _locomotion.WallMaxFaceAngle)
				{
					flag2 = true;
					_wallNormal = hit.normal;
				}
			}
			bool flag3 = _realizedHorizontalSpeed < _locomotion.WallStallSpeed;
			switch (_wallPhase)
			{
			case WallStopPhase.None:
				if (flag2 & flag3)
				{
					_wallPhase = WallStopPhase.Pushing;
					_wallPushTimer = 0f;
				}
				break;
			case WallStopPhase.Pushing:
				if (!flag2 || !flag)
				{
					_wallPhase = WallStopPhase.None;
					break;
				}
				_wallPushTimer += deltaTime;
				if (_wallPushTimer >= _locomotion.WallPushAllowedTime)
				{
					_wallPhase = WallStopPhase.Blocked;
				}
				break;
			case WallStopPhase.Blocked:
				if (!flag2 || !flag)
				{
					_wallPhase = WallStopPhase.None;
				}
				break;
			}
			_wallBlocked = _wallPhase == WallStopPhase.Blocked;
		}

		public void ResetForResume()
		{
			_stopPending = false;
			_stopPendingTimer = 0f;
			_stopWindowActive = false;
			_stopHoldTimer = 0f;
			_stopResumeTimer = 0f;
			_stopGait = 0;
			_stopEntrySpeed = 0f;
			_stoppedLatched = false;
			_landedNoInput = false;
			_isStarting = false;
			_startLatchCount = 0;
			_wasMoving = false;
			_activeGait = 0;
			_animatedSpeed = 0f;
			_animatedSpeedVelocity = 0f;
			_moveSpeedParam = 0f;
			_achievedHorizontalSpeed = 0f;
			TargetGaitSpeed = 0f;
			DesiredWorldMoveDirection = Vector3.zero;
			RawWorldMoveDirection = Vector3.zero;
			_noInputTimer = 0f;
			_hadRawInput = false;
			_desiredYaw = 0f;
			_desiredYawVel = 0f;
		}

		public void ResetMomentum()
		{
			ResetForResume();
			_lastAnimatedGait = 0;
			_isJumping = false;
			_jumpRequested = false;
			_jumpPending = false;
			_jumpWindupTimer = 0f;
			_jumpCooldownTimer = 0f;
			_jumpPressTime = 0f;
			_slideJumpQueued = false;
			_slideJumpGaitOverride = false;
			_fallingDuration = 0f;
			_fallStartTime = Time.time;
			_descendingDuration = 0f;
			_coyoteTimer = 0f;
			_landedFromJump = false;
			_landedFromJumpAge = 0f;
			_landingSpeed = 0f;
			_landingArcSession = false;
			_landingArcSessionTimer = 0f;
			_landingArcTurnTimer = 0f;
			_quickTurnActive = false;
			_quickTurnArmed = true;
			_quickTurnHold = 0f;
			_quickTurnTimer = 0f;
			_flickCount = 0;
			_locomotionStartTimer = 0f;
			_startRefireTimer = 0f;
			_gateHasPrevPosDot = false;
		}

		public void ResetToRest()
		{
			ResetMomentum();
			_crouchActive = false;
			_crouchSlideActive = false;
			_crouchFromSlide = false;
			_slideBlocked = false;
			_slideSeen = false;
			_slideTimer = 0f;
			_crouchBlend = 0f;
			_crouchBlendTarget = 0f;
			_crouchCooldownTimer = 0f;
			ResetSnapshotToRest();
			SnapCapsuleToStanding();
		}

		private void SnapCapsuleToStanding()
		{
			if ((bool)_capsule && (bool)_capsuleCollider)
			{
				_currentCapsuleHeight = _capsule.StandingHeight;
				_capsuleCollider.height = _currentCapsuleHeight;
				Vector3 center = _capsuleCollider.center;
				center.y = _capsule.StandingCentre;
				_capsuleCollider.center = center;
			}
		}

		private void ResetSnapshotToRest()
		{
			_snapshot = default;
			_snapshot.StrafeDirectionZ = 1f;
			_snapshot.ForwardStrafe = 1f;
			_snapshot.ShuffleDirectionZ = 1f;
			_snapshot.IsGrounded = true;
		}

		public void PhysicsTick(float fixedDeltaTime)
		{
			if (!_locomotion || _input == null)
			{
				return;
			}
			UpdateGround();
			UpdateCrouch();
			LocomotionInput current = _input.Current;
			float magnitude = current.Move.magnitude;
			StaminaService stamina = _stamina;
			int num = ResolveGait(magnitude, current.SprintHeld, current.SprintTierHeld, stamina);
			TargetGaitSpeed = GaitSpeed(num);
			Vector3 vector = (RawWorldMoveDirection = ResolveWorldDirection(current));
			DesiredWorldMoveDirection = SmoothDesiredDirection(vector, fixedDeltaTime);
			UpdateSlopeIncline(current.HasMove, fixedDeltaTime);
			TargetGaitSpeed *= ComputeSlopeSpeedMultiplier();
			if (_stopPending)
			{
				TargetGaitSpeed = GaitSpeed(1);
				DesiredWorldMoveDirection = FlatFacing();
			}
			UpdateWallStop(vector, current.HasMove, num, fixedDeltaTime);
			if (_wallBlocked)
			{
				TargetGaitSpeed = 0f;
				DesiredWorldMoveDirection = Vector3.zero;
			}
			TickLandingArc(current.HasMove, fixedDeltaTime);
			TryJump(current, stamina);
			TickJumpWindup(fixedDeltaTime);
			bool flag = num == 3;
			if (stamina != null)
			{
				if (flag)
				{
					stamina.DrainSprint(fixedDeltaTime);
				}
				stamina.Tick(fixedDeltaTime, flag);
			}
			_sprintActive = flag;
		}

		public void AnimationTick(float deltaTime)
		{
			if ((bool)_locomotion && _input != null)
			{
				LocomotionInput current = _input.Current;
				if (_wallBlocked)
				{
					current.Move = Vector2.zero;
					current.HasMove = false;
					current.MovementInputTapped = false;
					current.MovementInputPressed = false;
					current.MovementInputHeld = false;
					current.ShuffleRequested = false;
				}
				bool hasMove = current.HasMove;
				Vector3 desiredWorldMoveDirection = DesiredWorldMoveDirection;
				Vector3 vector = ResolveWorldDirection(current);
				bool num = _stopWindowActive || _stopPending;
				UpdateAnimatedSpeed(deltaTime);
				int num2 = ResolveAnimatedGait();
				int intendedGait = ResolveGait(stamina: _stamina, moveMagnitude: current.Move.magnitude, runHeld: current.SprintHeld, sprintTierHeld: current.SprintTierHeld);
				UpdateStopState(hasMove, num2, current.ShuffleRequested, deltaTime);
				if (num & hasMove)
				{
					_stopResumeTimer = _locomotion.StartLatchTime;
				}
				else
				{
					_stopResumeTimer = Mathf.Max(0f, _stopResumeTimer - deltaTime);
				}
				UpdateStarting(hasMove, desiredWorldMoveDirection, vector, deltaTime);
				UpdateQuickTurn(hasMove, vector, deltaTime);
				UpdateLean(desiredWorldMoveDirection, deltaTime);
				BuildSnapshot(num2, intendedGait, current, current.MovementInputTapped, hasMove);
				_wasMoving = hasMove;
				_wasGrounded = _isGrounded;
			}
		}

		private void UpdateLean(Vector3 desiredDirection, float deltaTime)
		{
			if (deltaTime <= 0f)
			{
				return;
			}
			float y = _transform.eulerAngles.y;
			if (!_leanInitialized)
			{
				_previousYaw = y;
				_leanInitialized = true;
			}
			float num = Mathf.DeltaAngle(_previousYaw, y) / deltaTime;
			_previousYaw = y;
			float num2 = ((_locomotion.MaxLeanRate > 0f) ? _locomotion.MaxLeanRate : 275f);
			float num3 = ((_locomotion.LeanSpeedReference > 0f) ? _locomotion.LeanSpeedReference : 4.5f);
			float num4 = Mathf.Clamp(num / num2, -1f, 1f);
			float num5 = Mathf.Clamp01(_animatedSpeed / num3);
			float num6 = _locomotion.LeanSmoothing * deltaTime;
			_leanValue = Mathf.Lerp(_leanValue, num4 * num5, num6);
			float num7 = ((_locomotion.HeadLookAngleRange > 0f) ? _locomotion.HeadLookAngleRange : 90f);
			float num8 = _locomotion.HeadLeanLimit / num7;
			if (TryHeadLookTowardDirection(desiredDirection, out var target, out var progress))
			{
				float t = Mathf.Pow(progress, Mathf.Max(1f, _locomotion.HeadLookTurnAccelPower));
				float t2 = Mathf.Lerp(_locomotion.HeadLookTurnStartSmoothing, _locomotion.HeadLookTurnEndSmoothing, t) * deltaTime;
				_headLookX = Mathf.Lerp(_headLookX, target * _locomotion.HeadLookStrength, t2);
				_bodyLookX = Mathf.Lerp(_bodyLookX, target * _locomotion.BodyLookStrength, t2);
				return;
			}
			if (!_input.Current.HasMove)
			{
				num8 = _locomotion.HeadLeanLimitWhileStanding / num7;
				Camera camera = ResolveMainCamera();
				if ((bool)camera)
				{
					Vector3 to = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);
					num4 = Mathf.Clamp(Vector3.SignedAngle(Vector3.ProjectOnPlane(_transform.forward, Vector3.up), to, Vector3.up) / num7, -1f, 1f);
					if (num4 > num8 || num4 < 0f - num8)
					{
						num4 = 0f;
					}
				}
				else
				{
					num4 = 0f;
				}
				num6 /= 3f;
			}
			_headLookX = Mathf.Clamp(Mathf.Lerp(_headLookX, num4 * _locomotion.HeadLookStrength, num6), 0f - num8, num8);
			_bodyLookX = Mathf.Lerp(_bodyLookX, num4 * _locomotion.BodyLookStrength, num6);
		}

		private Camera ResolveMainCamera()
		{
			if ((bool)_mainCamera)
			{
				return _mainCamera;
			}
			_mainCamera = Camera.main;
			return _mainCamera;
		}

		private bool TryHeadLookTowardDirection(Vector3 desiredDirection, out float target, out float progress)
		{
			target = 0f;
			progress = 1f;
			if (!_locomotion.HeadLookTowardDirectionEnabled)
			{
				return false;
			}
			bool num = _isStarting && Mathf.Abs(_startInitialDirection) >= _locomotion.HeadLookTurnMinAngle;
			bool quickTurnActive = _quickTurnActive;
			if (!num && !quickTurnActive)
			{
				return false;
			}
			Vector3 direction = (quickTurnActive ? _quickTurnDesiredDir : _startLookDir);
			if (direction.sqrMagnitude < 0.0001f)
			{
				direction = desiredDirection;
			}
			float num2 = ((_locomotion.HeadLookAngleRange > 0f) ? _locomotion.HeadLookAngleRange : 90f);
			float num3 = ComputeStartDirection(direction);
			float num4 = Mathf.Clamp01(_locomotion.HeadLeanLimitWhileStanding / num2);
			target = Mathf.Clamp(num3 / num2, 0f - num4, num4);
			float num5 = (quickTurnActive ? _quickTurnInitialGap : Mathf.Abs(_startInitialDirection));
			progress = ((num5 > 1f) ? Mathf.Clamp01(1f - Mathf.Abs(num3) / num5) : 1f);
			return true;
		}

		private void UpdateAnimatedSpeed(float deltaTime)
		{
			float achievedHorizontalSpeed = _achievedHorizontalSpeed;
			if (_locomotion.MoveSpeedDampTime > 0f)
			{
				_animatedSpeed = Mathf.SmoothDamp(_animatedSpeed, achievedHorizontalSpeed, ref _animatedSpeedVelocity, _locomotion.MoveSpeedDampTime, float.PositiveInfinity, deltaTime);
			}
			else
			{
				_animatedSpeed = achievedHorizontalSpeed;
				_animatedSpeedVelocity = 0f;
			}
			if (_animatedSpeed < 0.001f)
			{
				_animatedSpeed = 0f;
			}
		}

		private int ResolveAnimatedGait()
		{
			if (_animatedSpeed < 0.01f)
			{
				_lastAnimatedGait = 0;
				return _lastAnimatedGait;
			}
			float num = (_locomotion.WalkSpeed + _locomotion.RunSpeed) * 0.5f;
			float num2 = (_locomotion.RunSpeed + _locomotion.SprintSpeed) * 0.5f;
			int num3 = Mathf.Max(_lastAnimatedGait, 1);
			if (num3 < 2 && _animatedSpeed > num + 0.25f)
			{
				num3 = 2;
			}
			else if (num3 >= 2 && _animatedSpeed < num - 0.25f)
			{
				num3 = 1;
			}
			if (num3 < 3 && _animatedSpeed > num2 + 0.25f)
			{
				num3 = 3;
			}
			else if (num3 >= 3 && _animatedSpeed < num2 - 0.25f)
			{
				num3 = 2;
			}
			_lastAnimatedGait = num3;
			return num3;
		}

		private void UpdateStopState(bool hasMove, int reportedGait, bool shuffleRequested, float deltaTime)
		{
			bool isGrounded = _isGrounded;
			if (_stopHoldTimer > 0f)
			{
				_stopHoldTimer -= deltaTime;
			}
			if ((hasMove & isGrounded) && reportedGait >= 1 && !_stopWindowActive && !_stopPending && _stopResumeTimer <= 0f)
			{
				_activeGait = reportedGait;
			}
			bool isStarting = _isStarting;
			if ((_wasMoving | isStarting) && !hasMove && !shuffleRequested && ((_activeGait >= 1) | isStarting))
			{
				if (InstantStopEngaged)
				{
					_stopWindowActive = false;
					_stopPending = false;
					_activeGait = 0;
				}
				else
				{
					BeginStop(isGrounded, isStarting);
				}
			}
			if (_stopPending)
			{
				UpdateStopPending(hasMove, isGrounded, deltaTime);
			}
			if (hasMove)
			{
				_stopWindowActive = false;
				_stopPending = false;
				_stopHoldTimer = 0f;
			}
			else if (_animatedSpeed < 0.01f && _stopHoldTimer <= 0f)
			{
				_stopWindowActive = false;
				_activeGait = 0;
			}
			if (isGrounded && !_wasGrounded && !hasMove)
			{
				_landedNoInput = true;
			}
			if (hasMove)
			{
				_landedNoInput = false;
			}
		}

		private void BeginStop(bool grounded, bool fromStart)
		{
			int value = (fromStart ? 1 : _activeGait);
			_stopGait = Mathf.Clamp(value, 1, 2);
			_stopEntrySpeed = Mathf.Clamp(_animatedSpeed, GaitSpeed(1), GaitSpeed(_stopGait));
			if ((_locomotion.StopPhaseGateEnabled & grounded) && !_crouchActive && _stopGait == 1 && !fromStart && (!_wallBlocked || _locomotion.WallStopUsePhaseGate))
			{
				_stopPending = true;
				_stopPendingTimer = 0f;
				_stopWindowActive = false;
				_gateHasPrevPosDot = false;
			}
			else
			{
				_stopWindowActive = true;
				_stopPending = false;
			}
			_stopHoldTimer = _locomotion.StopHoldTime;
		}

		private void UpdateStopPending(bool hasMove, bool grounded, float deltaTime)
		{
			if (hasMove || !grounded)
			{
				_stopPending = false;
				return;
			}
			_stopPendingTimer += deltaTime;
			float stopEntryTargetLeft = _locomotion.StopEntryTargetLeft;
			float stopEntryTargetRight = _locomotion.StopEntryTargetRight;
			float stopMatchTolerance = _locomotion.StopMatchTolerance;
			bool flag = false;
			bool flag2 = false;
			if (_gateHasPrevPosDot)
			{
				bool flag3 = _gatePrevPosDot < stopEntryTargetLeft && _gatePosDot >= stopEntryTargetLeft;
				bool num = _gatePrevPosDot > stopEntryTargetRight && _gatePosDot <= stopEntryTargetRight;
				bool flag4 = _gatePosDotTrend > 0f && Mathf.Abs(_gatePosDot - stopEntryTargetLeft) <= stopMatchTolerance;
				bool flag5 = _gatePosDotTrend < 0f && Mathf.Abs(_gatePosDot - stopEntryTargetRight) <= stopMatchTolerance;
				flag = flag3 | flag4;
				flag2 = num | flag5;
			}
			_gatePrevPosDot = _gatePosDot;
			_gateHasPrevPosDot = true;
			bool flag6 = _stopPendingTimer >= _locomotion.StopPhaseGateMaxWait;
			if (flag | flag2 | flag6)
			{
				LastGateAligned = flag | flag2;
				LastGateTimer = _stopPendingTimer;
				LastGateFirePosDot = _gatePosDot;
				LastGateTarget = (flag ? stopEntryTargetLeft : (flag2 ? stopEntryTargetRight : ((Mathf.Abs(_gatePosDot - stopEntryTargetLeft) <= Mathf.Abs(_gatePosDot - stopEntryTargetRight)) ? stopEntryTargetLeft : stopEntryTargetRight)));
				_stopWindowActive = true;
				_stopPending = false;
			}
		}

		private Vector3 FlatFacing()
		{
			Vector3 forward = _transform.forward;
			forward.y = 0f;
			if (!(forward.sqrMagnitude > 0.0001f))
			{
				return Vector3.zero;
			}
			return forward.normalized;
		}

		private void UpdateStarting(bool hasMove, Vector3 direction, Vector3 rawDirection, float deltaTime)
		{
			_locomotionStartTimer = Mathf.Max(0f, _locomotionStartTimer - deltaTime);
			_startRefireTimer = Mathf.Max(0f, _startRefireTimer - deltaTime);
			bool flag = false;
			if (_locomotionStartTimer <= 0f)
			{
				float num = ((_stopResumeTimer > 0f) ? _locomotion.StopResumeStartSpeedThreshold : _locomotion.StartSpeedThreshold);
				if (hasMove && _animatedSpeed < num)
				{
					flag = true;
				}
				if (flag)
				{
					if (!_isStarting)
					{
						if (_locomotion.StartRefireDebounceEnabled && _startRefireTimer > 0f)
						{
							flag = false;
							_startRefireTimer = _locomotion.StartRefireDebounce;
						}
						else
						{
							_startInitialDirection = ComputeStartDirection(rawDirection);
							_startLookDir = rawDirection;
							_startLatchCount++;
							_startRefireTimer = _locomotion.StartRefireDebounce;
						}
					}
					if (flag)
					{
						_locomotionStartDirection = ComputeStartDirection(direction);
						_locomotionStartTimer = _locomotion.StartLatchTime;
					}
				}
			}
			else
			{
				flag = true;
				_locomotionStartDirection = ComputeStartDirection(direction);
			}
			_isStarting = flag;
		}

		private float ComputeStartDirection(Vector3 direction)
		{
			if (direction.sqrMagnitude < 0.0001f)
			{
				return 0f;
			}
			Vector3 forward = _transform.forward;
			forward.y = 0f;
			if (forward.sqrMagnitude < 0.0001f)
			{
				return 0f;
			}
			return Vector3.SignedAngle(forward.normalized, direction, Vector3.up);
		}

		private void UpdateQuickTurn(bool hasMove, Vector3 direction, float deltaTime)
		{
			_quickTurnHold = Mathf.Max(0f, _quickTurnHold - deltaTime);
			if (!_locomotion.QuickTurnEnabled)
			{
				_quickTurnActive = false;
				_quickTurnArmed = true;
				_quickTurnHold = 0f;
				_flickCount = 0;
				return;
			}
			if (_quickTurnActive)
			{
				_quickTurnTimer += deltaTime;
			}
			bool flag = direction.sqrMagnitude > 0.0001f;
			float num = (flag ? Mathf.Abs(ComputeStartDirection(direction)) : 0f);
			if (!_quickTurnArmed && (!flag || num <= _locomotion.QuickTurnRearmAngle || _quickTurnTimer >= _locomotion.QuickTurnMaxDuration))
			{
				_quickTurnArmed = true;
				_quickTurnActive = false;
			}
			float num2 = 0f;
			if (flag)
			{
				num2 = PushFlickAndMeasure(direction);
			}
			else
			{
				_flickCount = 0;
			}
			bool flag2 = _isStarting && _locomotion.StartTurnRootMotionEnabled && Mathf.Abs(_startInitialDirection) >= _locomotion.StartTurnRootMotionMinAngle;
			bool flag3 = !_isJumping && (_isGrounded || _coyoteTimer > 0f);
			bool flag4 = _landedFromJump && _locomotion.LandingQuickTurnEnabled;
			if ((_quickTurnArmed & flag3 & hasMove & flag) && !flag2 && _animatedSpeed >= _locomotion.QuickTurnMinSpeed && num >= _locomotion.QuickTurnMinAngle && ((num2 >= _locomotion.QuickTurnFlickAngle) | flag4))
			{
				_landedFromJump = false;
				_landingArcSession = false;
				_landingArcTurnTimer = 0f;
				_quickTurnActive = true;
				_quickTurnArmed = false;
				_quickTurnTimer = 0f;
				_quickTurnHold = _locomotion.QuickTurnTriggerHold;
				_quickTurnLeft = _plantedFootLeft;
				Vector3 vector = FlatFacing();
				bool flag5 = _locomotion.QuickTurnExact180Enabled && vector.sqrMagnitude > 0.0001f;
				_quickTurnOriginalDir = vector;
				_quickTurnDesiredDir = (flag5 ? (-vector) : direction);
				_quickTurnCutTarget = _quickTurnOriginalDir;
				_quickTurnInitialGap = (flag5 ? 180f : num);
				_quickTurnWarpGap = num;
				QuickTurnYawSign = 0f;
			}
		}

		private float PushFlickAndMeasure(Vector3 direction)
		{
			float time = Time.time;
			_flickDirs[_flickHead] = direction;
			_flickTimes[_flickHead] = time;
			_flickHead = (_flickHead + 1) % 32;
			if (_flickCount < 32)
			{
				_flickCount++;
			}
			float quickTurnFlickWindow = _locomotion.QuickTurnFlickWindow;
			float num = 0f;
			for (int i = 0; i < _flickCount; i++)
			{
				if (!(time - _flickTimes[i] > quickTurnFlickWindow))
				{
					float num2 = Vector3.Angle(_flickDirs[i], direction);
					if (num2 > num)
					{
						num = num2;
					}
				}
			}
			return num;
		}

		private void UpdateSlopeIncline(bool hasMove, float dt)
		{
			float num = 0f;
			if ((bool)_locomotion && (_locomotion.InclineAngleEnabled || _locomotion.SlopeSpeedEnabled || _locomotion.ProceduralLeanEnabled))
			{
				GroundInfo ground = Ground;
				if (hasMove && ground.Grounded && !ground.IsSteepSlope && ground.SlopeAngle > _locomotion.InclineAngleDeadZone)
				{
					Vector3 desiredWorldMoveDirection = DesiredWorldMoveDirection;
					desiredWorldMoveDirection.y = 0f;
					if (desiredWorldMoveDirection.sqrMagnitude > 0.0001f)
					{
						Vector3 vector = Vector3.ProjectOnPlane(desiredWorldMoveDirection.normalized, ground.Normal);
						if (vector.sqrMagnitude > 0.0001f)
						{
							vector.Normalize();
							num = Mathf.Asin(Mathf.Clamp(vector.y, -1f, 1f)) * 57.29578f;
						}
					}
				}
			}
			float num2 = (_locomotion ? _locomotion.InclineAngleSmoothing : 0f);
			if (num2 > 0f)
			{
				_slopeInclineRaw = Mathf.MoveTowards(_slopeInclineRaw, num, num2 * dt);
			}
			else
			{
				_slopeInclineRaw = num;
			}
		}

		private void DiagnoseSlopeIncline(float target, bool hasMove)
		{
			if (_slopeInclineDiagTicks < 250)
			{
				GroundInfo ground = Ground;
				if (hasMove && !(ground.SlopeAngle < 1f))
				{
					_slopeInclineDiagTicks++;
					Debug.Log($"[SlopeIncline] ft={Time.fixedTime:F3} target={target:F3} raw={_slopeInclineRaw:F3} " + $"hasMove={hasMove} grounded={ground.Grounded} steep={ground.IsSteepSlope} " + string.Format("slope={0:F1} n={1} col={2}", ground.SlopeAngle, ground.Normal, ground.Collider ? ground.Collider.name : "none"));
				}
			}
		}

		private float ComputeInclineAngle(bool hasMove)
		{
			if (!_locomotion || !_locomotion.InclineAngleEnabled)
			{
				return 0f;
			}
			return Mathf.Clamp(_slopeInclineRaw * _locomotion.InclineAngleScale, 0f - _locomotion.InclineAngleMax, _locomotion.InclineAngleMax);
		}

		private float ComputeSlopeSpeedMultiplier()
		{
			if (!_locomotion || !_locomotion.SlopeSpeedEnabled)
			{
				return 1f;
			}
			float slopeInclineRaw = _slopeInclineRaw;
			float slopeSpeedStartAngle = _locomotion.SlopeSpeedStartAngle;
			if (slopeInclineRaw <= slopeSpeedStartAngle)
			{
				return 1f;
			}
			float num = Mathf.Max(_locomotion.SlopeSpeedFullAngle, slopeSpeedStartAngle + 0.01f);
			float t = Mathf.Clamp01((slopeInclineRaw - slopeSpeedStartAngle) / (num - slopeSpeedStartAngle));
			return Mathf.Lerp(1f, _locomotion.SlopeSpeedMinMultiplier, t);
		}

		private float ComputeProceduralLeanAngle()
		{
			if (!_locomotion || !_locomotion.ProceduralLeanEnabled)
			{
				return 0f;
			}
			return Mathf.Clamp(_slopeInclineRaw * _locomotion.ProceduralLeanScale, 0f - _locomotion.ProceduralLeanMaxAngle, _locomotion.ProceduralLeanMaxAngle);
		}

		private void UpdateGround()
		{
			GroundInfo ground = default;
			if (_groundProbe == null || !_groundProbe.TryProbe(out ground))
			{
				ground = new GroundInfo
				{
					Grounded = false
				};
			}
			bool flag = ground.Grounded;
			if (flag && _isJumping && _groundProbe.VerticalVelocity > _locomotion.StepMidJumpVelocityThreshold)
			{
				flag = false;
				ground.Grounded = false;
			}
			Ground = ground;
			if (flag)
			{
				_coyoteTimer = _locomotion.CoyoteTime;
				if (_jumpCooldownTimer > 0f)
				{
					_jumpCooldownTimer -= Time.fixedDeltaTime;
				}
				if (!_isGrounded)
				{
					if (_isJumping && _fallingDuration >= _locomotion.LandingArcMinAirTime)
					{
						_landedFromJump = true;
						_landedFromJumpAge = 0f;
						_landingSpeed = _achievedHorizontalSpeed;
					}
					_isJumping = false;
					_slideJumpQueued = false;
				}
				_fallingDuration = 0f;
				_fallStartTime = Time.time;
				_descendingDuration = 0f;
			}
			else
			{
				_coyoteTimer -= Time.fixedDeltaTime;
				_fallingDuration = Time.time - _fallStartTime;
				if (_groundProbe != null && _groundProbe.VerticalVelocity < 0f)
				{
					_descendingDuration += Time.fixedDeltaTime;
				}
			}
			_isGrounded = flag;
		}

		private bool IsWorldPoseActive()
		{
			if (!_worldInteractionResolved)
			{
				_worldInteractionResolved = true;
				if ((bool)_transform)
				{
					_transform.TryGetComponent<WorldInteractionController>(out _worldInteraction);
				}
			}
			if ((bool)_worldInteraction)
			{
				return _worldInteraction.IsPosed;
			}
			return false;
		}

		private void UpdateCrouch()
		{
			if (_crouchCooldownTimer > 0f)
			{
				_crouchCooldownTimer -= Time.fixedDeltaTime;
			}
			if (_input.ConsumeCrouchToggle() && !IsWorldPoseActive() && _crouchCooldownTimer <= 0f)
			{
				if (!_crouchActive)
				{
					bool wantsSlide = WantsSlide;
					bool flag = wantsSlide && (_stamina == null || _stamina.TryConsumeSlide());
					_slideBlocked = wantsSlide && !flag;
					_crouchActive = true;
					_crouchCooldownTimer = _locomotion.CrouchCooldown;
					BeginSlideTracking(flag);
				}
				else if (!_locomotion.CeilingGatesStandUp || !CeilingBlocksStand())
				{
					_crouchActive = false;
					_crouchCooldownTimer = _locomotion.CrouchCooldown;
					ClearSlideTracking();
				}
			}
			UpdateSlideAutoStand();
			if ((bool)_capsule && (bool)_capsuleCollider)
			{
				float num = (_crouchSlideActive ? _capsule.SlideTransitionSpeed : _capsule.CrouchTransitionSpeed);
				_crouchBlend = Mathf.MoveTowards(_crouchBlend, _crouchBlendTarget, num * Time.fixedDeltaTime);
				_currentCapsuleHeight = Mathf.Lerp(_capsule.StandingHeight, _capsule.CrouchingHeight, _crouchBlend);
				_capsuleCollider.height = _currentCapsuleHeight;
				Vector3 center = _capsuleCollider.center;
				center.y = Mathf.Lerp(_capsule.StandingCentre, _capsule.CrouchingCentre, _crouchBlend);
				_capsuleCollider.center = center;
			}
		}

		private void BeginSlideTracking(bool fromSlide)
		{
			_crouchFromSlide = fromSlide;
			_slideSeen = false;
			_slideTimer = 0f;
		}

		private void ClearSlideTracking()
		{
			_crouchFromSlide = false;
			_slideSeen = false;
			_slideTimer = 0f;
		}

		private void UpdateSlideAutoStand()
		{
			if (!_crouchFromSlide)
			{
				return;
			}
			if (!_crouchActive)
			{
				ClearSlideTracking();
				return;
			}
			_slideTimer += Time.fixedDeltaTime;
			if (_crouchSlideActive)
			{
				_slideSeen = true;
				if (!(_slideNormalizedTime < _locomotion.SlideStandNormalizedTime))
				{
					TryStandFromSlide();
				}
			}
			else if (_slideSeen || _achievedHorizontalSpeed <= _locomotion.SlideEndSpeed || _slideTimer >= _locomotion.SlideMaxDuration)
			{
				TryStandFromSlide();
			}
		}

		private void TryStandFromSlide()
		{
			ClearSlideTracking();
			if (_locomotion.AutoStandAfterSlide && !CeilingBlocksStand())
			{
				_crouchActive = false;
			}
		}

		private bool CeilingBlocksStand()
		{
			if (_groundProbe == null || !_capsule)
			{
				return false;
			}
			return _groundProbe.HasCeiling(_capsule.CrouchingHeight, _capsule.StandingHeight + _capsule.CeilingClearance);
		}

		private int ResolveGait(float moveMagnitude, bool runHeld, bool sprintTierHeld, StaminaService stamina)
		{
			if (moveMagnitude < 0.01f)
			{
				return 0;
			}
			if (_crouchActive)
			{
				return 1;
			}
			bool flag = stamina == null || (_sprintActive ? stamina.CanContinueSprint : stamina.CanStartSprint);
			if (sprintTierHeld)
			{
				if (!flag)
				{
					return 2;
				}
				return 3;
			}
			if (runHeld)
			{
				return 2;
			}
			return 1;
		}

		private float GaitSpeed(int gait)
		{
			return (Gait)gait switch
			{
				Gait.Walk => _locomotion.WalkSpeed, 
				Gait.Run => _locomotion.RunSpeed, 
				Gait.Sprint => _locomotion.SprintSpeed, 
				_ => 0f, 
			};
		}

		private Vector3 ResolveWorldDirection(LocomotionInput input)
		{
			if (!input.HasMove)
			{
				return Vector3.zero;
			}
			Vector3 vector = new Vector3(input.Move.x, 0f, input.Move.y);
			return (Quaternion.Euler(0f, input.CameraYaw, 0f) * vector).normalized;
		}

		public void LatchQuickTurnCutTarget(Vector3 target)
		{
			if (!(target.sqrMagnitude < 0.0001f))
			{
				_quickTurnCutTarget = target.normalized;
			}
		}

		public void SetQuickTurnYawSign(float sign)
		{
			QuickTurnYawSign = sign;
		}

		public void AlignDesiredDirectionToRaw()
		{
			Vector3 rawWorldMoveDirection = RawWorldMoveDirection;
			if (!(rawWorldMoveDirection.sqrMagnitude < 0.0001f))
			{
				_desiredYaw = Mathf.Atan2(rawWorldMoveDirection.x, rawWorldMoveDirection.z) * 57.29578f;
				_desiredYawVel = 0f;
				_hadRawInput = true;
				_noInputTimer = 0f;
				DesiredWorldMoveDirection = rawWorldMoveDirection;
			}
		}

		private Vector3 SmoothDesiredDirection(Vector3 rawDir, float dt)
		{
			if (!_locomotion.SourceDirectionSmoothingEnabled)
			{
				return rawDir;
			}
			if (rawDir.sqrMagnitude < 0.0001f)
			{
				_noInputTimer += dt;
				if (_noInputTimer >= _locomotion.SourceDirectionResetTime)
				{
					_hadRawInput = false;
				}
				_desiredYawVel = 0f;
				return Vector3.zero;
			}
			_noInputTimer = 0f;
			float num = Mathf.Atan2(rawDir.x, rawDir.z) * 57.29578f;
			if (!_hadRawInput)
			{
				_desiredYaw = num;
				_desiredYawVel = 0f;
				_hadRawInput = true;
			}
			else
			{
				float goal = _desiredYaw + Mathf.DeltaAngle(_desiredYaw, num);
				SimpleSpringDamperExact(ref _desiredYaw, ref _desiredYawVel, goal, _locomotion.SourceDirectionHalflife, dt);
				_desiredYaw = Mathf.Repeat(_desiredYaw + 180f, 360f) - 180f;
			}
			float f = _desiredYaw * ((float)Math.PI / 180f);
			return new Vector3(Mathf.Sin(f), 0f, Mathf.Cos(f));
		}

		private static void SimpleSpringDamperExact(ref float x, ref float v, float goal, float halflife, float dt)
		{
			float num = 2.7725887f / (halflife + 1E-05f) * 0.5f;
			float num2 = x - goal;
			float num3 = v + num2 * num;
			float num4 = num * dt;
			float num5 = 1f / (1f + num4 + 0.48f * num4 * num4 + 0.235f * num4 * num4 * num4);
			x = num5 * (num2 + num3 * dt) + goal;
			v = num5 * (v - num3 * num * dt);
		}

		private void TickLandingArc(bool hasMove, float deltaTime)
		{
			if (_landedFromJump)
			{
				_landedFromJumpAge += deltaTime;
				if (_landedFromJumpAge > _locomotion.LandingQuickTurnLatchTime)
				{
					_landedFromJump = false;
				}
			}
			if ((_landedFromJump && !_landingArcSession) & hasMove)
			{
				_quickTurnArmed = true;
				_quickTurnActive = false;
				_quickTurnTimer = 0f;
				Vector3 desiredWorldMoveDirection = DesiredWorldMoveDirection;
				float num = ((desiredWorldMoveDirection.sqrMagnitude > 0.0001f) ? Mathf.Abs(ComputeStartDirection(desiredWorldMoveDirection)) : 0f);
				if (!_locomotion.QuickTurnEnabled || !_locomotion.LandingQuickTurnEnabled || !(num >= _locomotion.QuickTurnMinAngle))
				{
					_landedFromJump = false;
					float num2 = Mathf.Clamp((_landingSpeed - _locomotion.WalkSpeed) * _locomotion.LandingArcWindowPerSpeed, 0f, _locomotion.LandingArcWindowMax);
					if (_locomotion.LandingArcEnabled && num2 > 0.001f)
					{
						_landingArcSession = true;
						_landingArcSessionTimer = 0f;
						_landingArcTurnTimer = num2;
					}
				}
			}
			if (_landingArcSession)
			{
				_landingArcSessionTimer += deltaTime;
				_landingArcTurnTimer = Mathf.Max(0f, _landingArcTurnTimer - deltaTime);
				if (!hasMove || !_isGrounded || _isJumping || _wallBlocked || _stopWindowActive || _stopPending || _quickTurnActive || (_input != null && _input.ScriptedMoveActive) || _landingArcSessionTimer >= _locomotion.LandingArcSessionMax)
				{
					_landingArcSession = false;
					_landingArcTurnTimer = 0f;
				}
			}
		}

		private void TryJump(LocomotionInput input, StaminaService stamina)
		{
			if (IsWorldPoseActive())
			{
				_input.ConsumeJump();
			}
			else
			{
				if (!_isGrounded && !(_coyoteTimer > 0f))
				{
					return;
				}
				bool flag = _crouchActive && IsSliding();
				bool flag2 = _crouchActive && !flag;
				if (flag2 && (!_locomotion.CrouchJumpEnabled || CeilingBlocksStand()))
				{
					_input.ConsumeJump();
				}
				else if (_input.ConsumeJump() && !_isJumping && !_jumpPending && !(_jumpCooldownTimer > 0f) && (stamina == null || stamina.TryConsumeJump()))
				{
					_isJumping = true;
					_jumpCooldownTimer = _locomotion.JumpCooldown;
					if (flag)
					{
						_slideJumpQueued = true;
						_crouchActive = false;
						_crouchCooldownTimer = _locomotion.CrouchCooldown;
						ClearSlideTracking();
						_slideJumpGaitOverride = true;
						_slideJumpGait = ((_achievedHorizontalSpeed >= _locomotion.SprintSpeed * 0.8f) ? 3 : 2);
					}
					else if (flag2)
					{
						_crouchActive = false;
						_crouchCooldownTimer = _locomotion.CrouchCooldown;
						ClearSlideTracking();
						Debug.Log($"[CrouchJump] launch gait={_lastAnimatedGait} v={_achievedHorizontalSpeed:F2}");
					}
					bool flag3 = ResolveAnimGait(_lastAnimatedGait) > 0;
					if (!flag && !flag3 && input.Move.magnitude < _locomotion.StandingJumpMaxMoveInput && _locomotion.JumpLaunchMaxWait > 0f)
					{
						_jumpPending = true;
						_jumpWindupTimer = _locomotion.JumpLaunchMaxWait;
						_jumpPressTime = Time.time;
					}
					else
					{
						_jumpRequested = true;
						_isGrounded = false;
						_coyoteTimer = 0f;
						_fallStartTime = Time.time;
					}
				}
			}
		}

		private bool IsSliding()
		{
			if (!_locomotion.SlideJumpEnabled)
			{
				return false;
			}
			return _achievedHorizontalSpeed >= _locomotion.SlideJumpMinSpeed;
		}

		public void NotifyJumpLaunch()
		{
			if ((bool)_locomotion && _locomotion.JumpUseAnimationEvent && _jumpPending)
			{
				TriggerJumpLaunch("EVENT");
			}
		}

		private void TickJumpWindup(float fixedDeltaTime)
		{
			if (_jumpPending)
			{
				_jumpWindupTimer -= fixedDeltaTime;
				if (!(_jumpWindupTimer > 0f))
				{
					TriggerJumpLaunch("FALLBACK");
				}
			}
		}

		private void TriggerJumpLaunch(string source)
		{
			Debug.Log($"[JumpLaunch] {source} delay={Time.time - _jumpPressTime:F3}");
			_jumpRequested = true;
			_jumpPending = false;
		}

		private int ResolveAnimGait(int gait)
		{
			if (!_stopWindowActive)
			{
				return gait;
			}
			return _stopGait;
		}

		private void BuildSnapshot(int gait, int intendedGait, LocomotionInput input, bool movementTapped, bool hasMove)
		{
			int num = ResolveAnimGait(gait);
			if (_slideJumpGaitOverride)
			{
				if (_isJumping)
				{
					num = _slideJumpGait;
				}
				else
				{
					_slideJumpGaitOverride = false;
				}
			}
			if (_slideBlocked)
			{
				if (_crouchActive)
				{
					num = Mathf.Min(num, 2);
				}
				else
				{
					_slideBlocked = false;
				}
			}
			_snapshot.Gait = num;
			if (_locomotion.RootMotionLocomotionEnabled)
			{
				if (InstantStopEngaged)
				{
					_moveSpeedParam = 0f;
				}
				else if (_stopWindowActive || _stopPending)
				{
					float stopEntrySpeed = _stopEntrySpeed;
					_moveSpeedParam = ((stopEntrySpeed > _moveSpeedParam) ? stopEntrySpeed : Mathf.MoveTowards(_moveSpeedParam, stopEntrySpeed, _locomotion.RootMotionBlendRamp * Time.deltaTime));
				}
				else
				{
					float num2 = (hasMove ? TargetGaitSpeed : 0f);
					_moveSpeedParam = Mathf.MoveTowards(_moveSpeedParam, num2, _locomotion.RootMotionBlendRamp * Time.deltaTime);
					if (_locomotion.StartPoseFollowsSpeedEnabled && _isStarting)
					{
						_moveSpeedParam = Mathf.Max(_moveSpeedParam, Mathf.Min(num2, _achievedHorizontalSpeed));
					}
					if (_locomotion.StopResumeMatchIntent && _stopResumeTimer > 0f)
					{
						float num3 = GaitSpeed(intendedGait);
						if (num3 > 0f && _moveSpeedParam > num3)
						{
							_moveSpeedParam = num3;
						}
					}
				}
				_snapshot.MoveSpeed = _moveSpeedParam;
			}
			else
			{
				_snapshot.MoveSpeed = (_stopWindowActive ? GaitSpeed(_stopGait) : _animatedSpeed);
				_moveSpeedParam = _snapshot.MoveSpeed;
			}
			_snapshot.StrafeDirectionX = 0f;
			_snapshot.StrafeDirectionZ = 1f;
			_snapshot.ForwardStrafe = 1f;
			_snapshot.LocomotionStartDirection = _locomotionStartDirection;
			_snapshot.StartInitialDirection = _startInitialDirection;
			_snapshot.StartLatchCount = _startLatchCount;
			_snapshot.InclineAngle = ComputeInclineAngle(hasMove);
			_snapshot.SlopeLeanAngle = ComputeProceduralLeanAngle();
			_snapshot.FallingDuration = _fallingDuration;
			_snapshot.DescendingDuration = _descendingDuration;
			_snapshot.IsStrafing = false;
			_snapshot.IsCrouching = _crouchActive;
			_snapshot.IsSliding = _crouchSlideActive || _slideJumpGaitOverride;
			_snapshot.IsJumping = _isJumping;
			_snapshot.IsGrounded = _isGrounded && !_isJumping;
			if (hasMove)
			{
				_stoppedLatched = false;
			}
			else if (_stopWindowActive || _landedNoInput || _animatedSpeed < _locomotion.StoppedSpeedThreshold)
			{
				_stoppedLatched = true;
			}
			_snapshot.IsStopped = _stoppedLatched;
			_snapshot.IsStarting = _isStarting;
			_snapshot.IsWalking = intendedGait == 1;
			_snapshot.IsSprinting = intendedGait == 3;
			_snapshot.IsTurningInPlace = false;
			_snapshot.IsQuickTurning = _quickTurnHold > 0f;
			_snapshot.QuickTurnLeft = _quickTurnLeft;
			_snapshot.MovementInputTapped = movementTapped;
			_snapshot.MovementInputPressed = input.MovementInputPressed;
			_snapshot.MovementInputHeld = input.MovementInputHeld;
			_snapshot.ShuffleRequested = input.ShuffleRequested;
			_snapshot.CameraRotationOffset = 0f;
			_snapshot.ShuffleDirectionX = 0f;
			_snapshot.ShuffleDirectionZ = 1f;
			_snapshot.LeanValue = _leanValue;
			_snapshot.HeadLookX = _headLookX;
			_snapshot.HeadLookY = 0f;
			_snapshot.BodyLookX = _bodyLookX;
			_snapshot.BodyLookY = 0f;
		}
	}
}
