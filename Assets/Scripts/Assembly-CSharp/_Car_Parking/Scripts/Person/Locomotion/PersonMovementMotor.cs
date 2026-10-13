using System;
using CarInteractionAnimationsScripts;
using UnityEngine;
using _Car_Parking.Scripts.ArtistTools;
using _Car_Parking.Scripts.Person.Locomotion.Config;
using _Car_Parking.Scripts.Person.Locomotion.Input;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class PersonMovementMotor
	{
		private readonly Transform _transform;

		private readonly Rigidbody _rigidbody;

		private readonly MainPerson _mainPerson;

		private readonly PersonLocomotionState _state;

		private readonly PersonGroundProbe _groundProbe;

		private readonly PersonInputAdapter _input;

		private readonly PersonAnimatorBridge _bridge;

		private readonly IRagdollController _ragdoll;

		private readonly IPersonActivation _activation;

		private readonly PersonLocomotionConfig _config;

		private readonly PersonPlatformRider _rider;

		private readonly PersonSlopeRules _slopeRules;

		private const float GroundStickSpeed = 2f;

		private const float WallGuardLookAhead = 0.2f;

		private const float SharpTurnExitAngle = 20f;

		private const float ScriptedApproachTurnRateDeg = 600f;

		private bool _wasActive;

		private bool _frozen;

		private bool _sharpTurning;

		private Vector3 _horizontalVelocity;

		private Vector3 _stopEntryVelocity;

		private bool _prevStopRootActive;

		private Vector3 _startEntryVelocity;

		private bool _prevStartRootActive;

		private Vector3 _locoEntryVelocity;

		private bool _prevLocoRootActive;

		private bool _prevLocoSettled;

		private Vector3 _quickTurnEntryVelocity;

		private bool _prevQuickTurnRootActive;

		private const bool DiagQuickTurn = false;

		private int _qtDiagFrames;

		private float _qtAccumYaw;

		private bool _qtPrevActive;

		private float _reverseElapsed;

		private bool _reversePrevActive;

		private float _qtYawSign;

		private float _qtWarpScale = 1f;

		private bool _qtWarpPrevActive;

		private bool _qtRedirectSticky;

		private float _qtTurnSign;

		private float _qtSchedP0;

		private float _qtSchedGap0;

		private const bool DiagGaitSpeed = false;

		private const bool DiagStartEsc = false;

		private const int StartEscTailFrames = 25;

		private float _diagRootSpeed;

		private int _diagEscTail;

		private const float GaitDiagWindow = 1f;

		private int _gaitDiagGait = -1;

		private int _gaitDiagSamples;

		private float _gaitDiagSum;

		private float _gaitDiagMin;

		private float _gaitDiagMax;

		private float _gaitDiagElapsed;

		private Vector3 _gaitDiagPrevPos;

		private bool _gaitDiagHasPrevPos;

		private float _verticalVelocity;

		private float _slopeSlideSpeed;

		private Vector3 _visualOffset;

		private Vector3 _visualOffsetVelocity;

		private Transform _skinRoot;

		private bool _stepClimbing;

		private bool _climbCollisionsDisabled;

		private Vector3 _stepTarget;

		private float _stepClimbSpeed;

		private float _stepClimbElapsed;

		private Vector3 _lastRbPos;

		private bool _hasLastPos;

		private Vector3 _appliedCarryLastTick;

		private Vector3 _exitMomentum;

		private bool _startCommitActive;

		private Vector3 _startCommitDir;

		private float _startCommitHold;

		private int _startCommitFrames;

		private int _lastStartLatch = -1;

		private int _lastAimLatch = -1;

		private int _startSpeedLatch = -1;

		private float _startEntryGaitSpeed;

		private int _startDiagFrames;

		private bool _prevSnapshotStarting;

		private bool _startAimActive;

		private Vector3 _startAimDir;

		private bool _seedStartAimFromInput;

		private CapsuleCollider _wallCapsule;

		private bool _wallCapsuleResolved;

		private static readonly RaycastHit[] SteamPropHits = new RaycastHit[8];

		public static float SteamPropLookAhead = 0.08f;

		public static bool SteamPropBlockLog;

		public Vector3 VisualOffset => _visualOffset;

		public float HorizontalSpeed => _horizontalVelocity.magnitude;

		public Vector3 PlatformCarry => _appliedCarryLastTick;

		public PersonMovementMotor(Transform transform, Rigidbody rigidbody, MainPerson mainPerson, PersonLocomotionState state, PersonGroundProbe groundProbe, PersonInputAdapter input, PersonAnimatorBridge bridge, IRagdollController ragdoll, IPersonActivation activation, PersonLocomotionConfig config, PersonPlatformRider rider, PersonSlopeRules slopeRules)
		{
			_transform = transform;
			_rigidbody = rigidbody;
			_mainPerson = mainPerson;
			_state = state;
			_groundProbe = groundProbe;
			_input = input;
			_bridge = bridge;
			_ragdoll = ragdoll;
			_activation = activation;
			_config = config;
			_rider = rider;
			_slopeRules = slopeRules;
		}

		public void SetFrozen(bool frozen)
		{
			if (_frozen == frozen)
			{
				return;
			}
			_frozen = frozen;
			if (_frozen)
			{
				ResetMotionState();
				if ((bool)_rigidbody && !_rigidbody.isKinematic)
				{
					Vector3 velocity = _rigidbody.velocity;
					_rigidbody.velocity = new Vector3(0f, velocity.y, 0f);
				}
			}
		}

		public void PrepareTeleport()
		{
			ResetMotionState();
			_hasLastPos = false;
		}

		public bool PhysicsStep()
		{
			bool flag = IsActive();
			if (_state != null)
			{
				_state.IsActive = flag;
			}
			if (_frozen)
			{
				ApplyFrozenStep();
				return false;
			}
			bool flag2 = HandleRagdollInterrupt();
			if (!flag)
			{
				HandleDeactivation();
				_wasActive = flag;
				return false;
			}
			_wasActive = flag;
			if ((bool)_rigidbody)
			{
				_rigidbody.useGravity = false;
				if (_rigidbody.interpolation != RigidbodyInterpolation.Interpolate)
				{
					_rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
				}
				if (_hasLastPos && !_stepClimbing && Time.fixedDeltaTime > 0f)
				{
					Vector3 vector = _rigidbody.position - _lastRbPos;
					Vector3 appliedCarryLastTick = _appliedCarryLastTick;
					appliedCarryLastTick.y = 0f;
					vector.y = 0f;
					vector -= appliedCarryLastTick * Time.fixedDeltaTime;
					_state?.ReportRealizedHorizontalSpeed(vector.magnitude / Time.fixedDeltaTime);
				}
				_lastRbPos = _rigidbody.position;
				_hasLastPos = true;
			}
			if (flag2)
			{
				ResetPlatformRide();
				return false;
			}
			if (HandleFallRagdoll())
			{
				ResetPlatformRide();
				return false;
			}
			_state?.PhysicsTick(Time.fixedDeltaTime);
			if (_rider != null && _state != null)
			{
				_rider.Sample(_state.Ground.Collider, _state.Ground.Grounded);
			}
			if (_stepClimbing)
			{
				AdvanceStepClimb(Time.fixedDeltaTime);
				_rider?.Commit();
				return true;
			}
			ApplyRotation();
			ApplyMovement();
			ApplyStepUp();
			_rider?.Commit();
			return true;
		}

		private void ApplyFrozenStep()
		{
			ResetMotionState();
			if ((bool)_rigidbody)
			{
				if (!_rigidbody.isKinematic)
				{
					_rigidbody.useGravity = true;
					Vector3 velocity = _rigidbody.velocity;
					_rigidbody.velocity = new Vector3(0f, velocity.y, 0f);
				}
				_lastRbPos = _rigidbody.position;
				_hasLastPos = true;
			}
		}

		private void ResetPlatformRide()
		{
			_rider?.Reset();
			_appliedCarryLastTick = Vector3.zero;
			_exitMomentum = Vector3.zero;
		}

		private Vector3 FlatForward()
		{
			Vector3 forward = _transform.forward;
			forward.y = 0f;
			if (!(forward.sqrMagnitude > 0.0001f))
			{
				return Vector3.forward;
			}
			return forward.normalized;
		}

		private void DiagnoseGaitSpeed(Vector3 rootVel, float dt, bool settled)
		{
			if (!settled || _state == null)
			{
				ResetGaitDiagWindow();
				return;
			}
			int gait = _state.Snapshot.Gait;
			if (gait != _gaitDiagGait)
			{
				_gaitDiagGait = gait;
				ResetGaitDiagWindow();
				return;
			}
			float magnitude = new Vector2(rootVel.x, rootVel.z).magnitude;
			_gaitDiagSum += magnitude;
			_gaitDiagSamples++;
			_gaitDiagMin = Mathf.Min(_gaitDiagMin, magnitude);
			_gaitDiagMax = Mathf.Max(_gaitDiagMax, magnitude);
			_gaitDiagElapsed += dt;
			if (!(_gaitDiagElapsed < 1f) && _gaitDiagSamples > 0)
			{
				float num = _gaitDiagSum / (float)_gaitDiagSamples;
				float targetGaitSpeed = _state.TargetGaitSpeed;
				float num2 = ((targetGaitSpeed > 0.01f) ? (num / targetGaitSpeed) : 0f);
				string arg = (((bool)_mainPerson && (int)_mainPerson.Gender == 0) ? "F" : "M");
				float num3 = 0f;
				if (_gaitDiagHasPrevPos && (bool)_rigidbody)
				{
					Vector3 vector = _rigidbody.position - _gaitDiagPrevPos;
					vector.y = 0f;
					num3 = vector.magnitude / _gaitDiagElapsed;
				}
				Debug.Log($"[GaitSpeed] sex={arg} gait={(Gait)gait} param={_state.Snapshot.MoveSpeed:F2} " + $"target={targetGaitSpeed:F2} mean={num:F2} real={num3:F2} " + $"min={_gaitDiagMin:F2} max={_gaitDiagMax:F2} " + $"ratio={num2:F3} n={_gaitDiagSamples} {DiagDominantClips()}");
				ResetGaitDiagWindow();
			}
		}

		private void ResetGaitDiagWindow()
		{
			_gaitDiagSamples = 0;
			_gaitDiagSum = 0f;
			_gaitDiagMin = float.MaxValue;
			_gaitDiagMax = 0f;
			_gaitDiagElapsed = 0f;
			_gaitDiagPrevPos = (_rigidbody ? _rigidbody.position : Vector3.zero);
			_gaitDiagHasPrevPos = _rigidbody;
		}

		private string DiagDominantClips()
		{
			Animator animator = (_mainPerson ? _mainPerson.Animator : null);
			if (!animator)
			{
				return "clips=no-animator";
			}
			AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
			if (currentAnimatorClipInfo == null || currentAnimatorClipInfo.Length == 0)
			{
				return "clips=none";
			}
			int num = -1;
			int num2 = -1;
			for (int i = 0; i < currentAnimatorClipInfo.Length; i++)
			{
				if (num < 0 || currentAnimatorClipInfo[i].weight > currentAnimatorClipInfo[num].weight)
				{
					num2 = num;
					num = i;
				}
				else if (num2 < 0 || currentAnimatorClipInfo[i].weight > currentAnimatorClipInfo[num2].weight)
				{
					num2 = i;
				}
			}
			string arg = DiagClipLabel(currentAnimatorClipInfo[num]);
			string arg2 = ((num2 >= 0) ? DiagClipLabel(currentAnimatorClipInfo[num2]) : "-");
			return $"leaves={currentAnimatorClipInfo.Length} top={arg} next={arg2}";
		}

		private static string DiagClipLabel(AnimatorClipInfo info)
		{
			string arg = (info.clip ? info.clip.name : "null");
			return $"{arg}@{info.weight:F2}";
		}

		private bool IsActive()
		{
			if (!_config || !_rigidbody || _state == null || _activation == null)
			{
				return false;
			}
			if (!_activation.IsActiveDriver(_transform.gameObject))
			{
				return false;
			}
			if (CarInteractionAnimating() && (_input == null || !_input.ScriptedMoveActive))
			{
				return false;
			}
			return !_rigidbody.isKinematic;
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

		public void ResetMotionState()
		{
			if (_climbCollisionsDisabled && (bool)_rigidbody)
			{
				_rigidbody.detectCollisions = true;
				_climbCollisionsDisabled = false;
			}
			_stepClimbing = false;
			_horizontalVelocity = Vector3.zero;
			_prevStopRootActive = false;
			_prevStartRootActive = false;
			_prevLocoRootActive = false;
			_prevLocoSettled = false;
			_prevQuickTurnRootActive = false;
			_verticalVelocity = 0f;
			_slopeSlideSpeed = 0f;
			_visualOffset = Vector3.zero;
			_visualOffsetVelocity = Vector3.zero;
			_hasLastPos = false;
			_startAimActive = false;
			_seedStartAimFromInput = true;
			_startSpeedLatch = -1;
			_startEntryGaitSpeed = 0f;
			ResetPlatformRide();
			if ((bool)_skinRoot)
			{
				_skinRoot.localPosition = Vector3.zero;
			}
		}

		public void ForceDeactivate()
		{
			HandleDeactivation();
			_wasActive = false;
			if (_state != null)
			{
				_state.IsActive = false;
			}
		}

		public void ResetForResume()
		{
			ResetMotionState();
			_sharpTurning = false;
			_startCommitActive = false;
			_startAimActive = false;
		}

		private void HandleDeactivation()
		{
			ResetMotionState();
			if (_wasActive && (bool)_rigidbody)
			{
				_rigidbody.useGravity = true;
				if (!_rigidbody.isKinematic)
				{
					_rigidbody.velocity = Vector3.zero;
				}
			}
		}

		private bool HandleRagdollInterrupt()
		{
			if (!_input.Current.HasMove)
			{
				return false;
			}
			if (_activation == null || !_transform || !_activation.IsActiveDriver(_transform.gameObject))
			{
				return false;
			}
			if (_ragdoll.IsRagdoll && !_ragdoll.IsStandingUp)
			{
				if (_ragdoll.CanStandUpFromInput)
				{
					_ragdoll.DisableRagdoll();
				}
				return true;
			}
			return false;
		}

		private bool HandleFallRagdoll()
		{
			if (_frozen)
			{
				return false;
			}
			if (!_config.RagdollOnFall || !_ragdoll.Exists)
			{
				return false;
			}
			if (_state.Ground.Grounded || _verticalVelocity > 0f - _config.RagdollFallSpeed)
			{
				return false;
			}
			if (_state.Snapshot.FallingDuration < _config.RagdollMinAirTime)
			{
				return false;
			}
			if (_ragdoll.IsRagdoll)
			{
				return false;
			}
			Vector3 force = new Vector3(_transform.eulerAngles.y, 0f, 0f);
			Vector3 initialVelocity = (_rigidbody ? _rigidbody.velocity : Vector3.zero);
			_ragdoll.EnableFallRagdoll(force, initialVelocity);
			return true;
		}

		private void ApplyMovement()
		{
			float fixedDeltaTime = Time.fixedDeltaTime;
			RootMotionFlags rootMotionFlags = _bridge?.CurrentRootMotion ?? default(RootMotionFlags);
			bool flag = _state.DesiredWorldMoveDirection.sqrMagnitude > 0.0001f;
			GroundInfo ground = _state.Ground;
			bool grounded = ground.Grounded;
			Vector3 obj = (flag ? FlatForward() : Vector3.zero);
			Vector3 vector = obj * _state.TargetGaitSpeed;
			Vector3 b = obj * PoseAlignedGaitSpeed();
			if (rootMotionFlags.EmoteActive)
			{
				Vector3 vector2 = _bridge?.ConsumeRootDelta() ?? Vector3.zero;
				_horizontalVelocity = ((fixedDeltaTime > 0f) ? (vector2 / fixedDeltaTime) : Vector3.zero);
			}
			else if (rootMotionFlags.QuickTurnReverseActive)
			{
				if (_bridge != null)
				{
					_bridge.ConsumeRootDelta();
				}
				float num = Mathf.Min(_horizontalVelocity.magnitude, _state.TargetGaitSpeed);
				_horizontalVelocity = FlatForward() * num;
			}
			else if (rootMotionFlags.StopActive)
			{
				Vector3 vector3 = ((_bridge != null) ? _bridge.ConsumeRootDelta() : Vector3.zero);
				Vector3 vector4 = ((fixedDeltaTime > 0f) ? (vector3 / fixedDeltaTime) : Vector3.zero);
				if (!_prevStopRootActive)
				{
					_stopEntryVelocity = _horizontalVelocity;
				}
				Vector3 horizontalVelocity = (rootMotionFlags.StopSettled ? vector4 : Vector3.Lerp(_stopEntryVelocity, vector4, Mathf.Clamp01(rootMotionFlags.StopBlend)));
				float stopCapSpeed = _state.StopCapSpeed;
				if (_config.StopSpeedClampEnabled && horizontalVelocity.magnitude > stopCapSpeed)
				{
					horizontalVelocity = horizontalVelocity.normalized * stopCapSpeed;
				}
				_horizontalVelocity = horizontalVelocity;
			}
			else if (grounded && _bridge != null && _bridge.InstantStopEscaping)
			{
				_bridge?.ConsumeRootDelta();
				_horizontalVelocity = Vector3.zero;
			}
			else if (grounded && (rootMotionFlags.StartUncommitted || rootMotionFlags.StartRollbackActive))
			{
				_bridge?.ConsumeRootDelta();
				_horizontalVelocity = Vector3.zero;
			}
			else if (rootMotionFlags.StartExitActive)
			{
				Vector3 vector5 = _bridge?.ConsumeRootDelta() ?? Vector3.zero;
				Vector3 vector6 = ((fixedDeltaTime > 0f) ? (vector5 / fixedDeltaTime) : Vector3.zero);
				_diagRootSpeed = new Vector2(vector6.x, vector6.z).magnitude;
				bool flag2 = rootMotionFlags.TurnStartActive || (_config.StartExitFollowsRootMotion && _config.RootMotionLocomotionEnabled);
				_horizontalVelocity = (flag2 ? vector6 : Vector3.Lerp(vector6, b, Mathf.Clamp01(rootMotionFlags.StartExitBlend)));
			}
			else if (rootMotionFlags.StartActive || rootMotionFlags.TurnStartActive)
			{
				Vector3 vector7 = _bridge?.ConsumeRootDelta() ?? Vector3.zero;
				Vector3 vector8 = ((fixedDeltaTime > 0f) ? (vector7 / fixedDeltaTime) : Vector3.zero);
				_diagRootSpeed = new Vector2(vector8.x, vector8.z).magnitude;
				if (!_prevStartRootActive)
				{
					_startEntryVelocity = _horizontalVelocity;
				}
				_horizontalVelocity = (rootMotionFlags.StartSettled ? vector8 : Vector3.Lerp(_startEntryVelocity, vector8, Mathf.Clamp01(rootMotionFlags.StartBlend)));
			}
			else if (rootMotionFlags.QuickTurnExitActive)
			{
				Vector3 vector9 = _bridge?.ConsumeRootDelta() ?? Vector3.zero;
				Vector3 a = ((fixedDeltaTime > 0f) ? (vector9 / fixedDeltaTime) : Vector3.zero);
				_horizontalVelocity = Vector3.Lerp(a, vector, Mathf.Clamp01(rootMotionFlags.QuickTurnExitBlend));
			}
			else if (rootMotionFlags.QuickTurnActive)
			{
				Vector3 vector10 = _bridge?.ConsumeRootDelta() ?? Vector3.zero;
				Vector3 vector11 = ((fixedDeltaTime > 0f) ? (vector10 / fixedDeltaTime) : Vector3.zero);
				if (!_prevQuickTurnRootActive)
				{
					_quickTurnEntryVelocity = _horizontalVelocity;
				}
				_horizontalVelocity = (rootMotionFlags.QuickTurnSettled ? vector11 : Vector3.Lerp(_quickTurnEntryVelocity, vector11, Mathf.Clamp01(rootMotionFlags.QuickTurnBlend)));
			}
			else if (rootMotionFlags.LocoActive)
			{
				Vector3 vector12 = _bridge?.ConsumeRootDelta() ?? Vector3.zero;
				Vector3 vector13 = ((fixedDeltaTime > 0f) ? (vector12 / fixedDeltaTime) : Vector3.zero);
				_diagRootSpeed = new Vector2(vector13.x, vector13.z).magnitude;
				if (!_prevLocoRootActive || (!rootMotionFlags.LocoSettled && _prevLocoSettled))
				{
					_locoEntryVelocity = _horizontalVelocity;
				}
				_horizontalVelocity = (rootMotionFlags.LocoSettled ? vector13 : Vector3.Lerp(_locoEntryVelocity, vector13, Mathf.Clamp01(rootMotionFlags.LocoBlend)));
			}
			else if (_state.LandingArcActive & flag)
			{
				float num2 = Mathf.MoveTowards(_horizontalVelocity.magnitude, _state.TargetGaitSpeed, _config.LandingArcSpeedBleed * fixedDeltaTime);
				_horizontalVelocity = FlatForward() * num2;
			}
			else
			{
				float num3;
				if (flag)
				{
					num3 = ((!grounded) ? (_config.Acceleration * _config.AirControlFactor) : ((!(vector.magnitude >= _horizontalVelocity.magnitude)) ? _config.Deceleration : ((_horizontalVelocity.magnitude < _config.WalkSpeed) ? _config.StartAcceleration : _config.Acceleration)));
				}
				else
				{
					num3 = (grounded ? _config.GroundFriction : _config.AirDrag);
					vector = Vector3.zero;
				}
				_horizontalVelocity = Vector3.MoveTowards(_horizontalVelocity, vector, num3 * fixedDeltaTime);
			}
			if (_rider != null && _rider.TryConsumeExitVelocity(out var velocity))
			{
				_exitMomentum = velocity * _config.PlatformExitMomentumScale;
			}
			if (grounded)
			{
				_exitMomentum = Vector3.zero;
			}
			else
			{
				_exitMomentum = Vector3.MoveTowards(_exitMomentum, Vector3.zero, _config.AirDrag * fixedDeltaTime);
			}
			if (_state.WallBlocked)
			{
				Vector3 wallNormal = _state.WallNormal;
				wallNormal.y = 0f;
				if (wallNormal.sqrMagnitude > 0.0001f)
				{
					wallNormal.Normalize();
					float num4 = Vector3.Dot(_horizontalVelocity, -wallNormal);
					if (num4 > 0f)
					{
						_horizontalVelocity += wallNormal * num4;
					}
				}
			}
			bool flag3 = _state.ConsumeJumpRequest(out var slideJump);
			if (flag3)
			{
				float num5 = (slideJump ? _config.SlideJumpHeight : _config.JumpHeight);
				_verticalVelocity = Mathf.Sqrt(2f * Mathf.Abs(Physics.gravity.y) * num5);
				if (slideJump)
				{
					ApplySlideJumpForwardBoost(ref _horizontalVelocity);
				}
				else
				{
					ApplyJumpForwardBoost(ref _horizontalVelocity);
				}
				Debug.Log($"[SlideJump] slide={slideJump} gait={_state.Snapshot.Gait} v={_horizontalVelocity.magnitude:F2} vy={_verticalVelocity:F2}");
			}
			else if (grounded && _verticalVelocity <= 0.01f)
			{
				_verticalVelocity = -2f;
			}
			else
			{
				_verticalVelocity += Physics.gravity.y * fixedDeltaTime;
			}
			float num6 = ((grounded && !flag3) ? ComputeSlopeSlideTarget(ground) : 0f);
			_slopeSlideSpeed = (_config.SlopeSlideAngleRampEnabled ? Mathf.MoveTowards(_slopeSlideSpeed, num6, _config.SlopeSlideAcceleration * fixedDeltaTime) : num6);
			bool flag4 = grounded && !flag3 && ground.IsSteepSlope && !_config.SlopeSlideAngleRampEnabled;
			Vector3 velocity2;
			if (grounded && !flag3 && !flag4)
			{
				Vector3 vector14 = Vector3.ProjectOnPlane(_horizontalVelocity, ground.Normal);
				if (_verticalVelocity > 0f)
				{
					velocity2 = new Vector3(vector14.x, vector14.y + _verticalVelocity, vector14.z);
				}
				else
				{
					Vector3 normalized = Vector3.ProjectOnPlane(Vector3.down, ground.Normal).normalized;
					velocity2 = vector14 + normalized * _slopeSlideSpeed - ground.Normal * 2f;
				}
			}
			else if (flag4)
			{
				Vector3 normalized2 = Vector3.ProjectOnPlane(Vector3.down, ground.Normal).normalized;
				Vector3 vector15 = _horizontalVelocity + normalized2 * _slopeSlideSpeed;
				velocity2 = new Vector3(vector15.x, _verticalVelocity, vector15.z);
			}
			else
			{
				velocity2 = new Vector3(_horizontalVelocity.x, _verticalVelocity, _horizontalVelocity.z);
			}
			Vector3 vector16 = ((_rider != null) ? _rider.CarryVelocity : Vector3.zero) + _exitMomentum;
			velocity2 += vector16;
			_appliedCarryLastTick = vector16;
			Vector3 velocity3 = BlockVelocityIntoWall(velocity2);
			velocity3 = SteamBlockVelocityIntoProp(velocity3);
			_rigidbody.velocity = velocity3;
			_prevStopRootActive = rootMotionFlags.StopActive;
			_prevStartRootActive = rootMotionFlags.StartActive;
			_prevLocoRootActive = rootMotionFlags.LocoActive;
			_prevLocoSettled = rootMotionFlags.LocoSettled;
			_prevQuickTurnRootActive = rootMotionFlags.QuickTurnActive;
			_state.ReportAchievedHorizontalSpeed((rootMotionFlags.EmoteActive && !flag) ? 0f : HorizontalSpeed);
		}

		private void DiagnoseStartEscalation(RootMotionFlags rm, float poseSpeed)
		{
			if (rm.StartActive || rm.StartExitActive || rm.TurnStartActive || (rm.LocoActive && !rm.LocoSettled))
			{
				_diagEscTail = 25;
			}
			else
			{
				if (_diagEscTail <= 0)
				{
					return;
				}
				_diagEscTail--;
			}
			LocomotionStateSnapshot snapshot = _state.Snapshot;
			string text = (rm.StartExitActive ? "exit" : ((rm.StartActive || rm.TurnStartActive) ? "start" : (rm.LocoActive ? "loco" : "tail")));
			Debug.Log($"[StartEsc] {text} turn={rm.TurnStartActive} exitB={rm.StartExitBlend:F2} locoB={rm.LocoBlend:F2} locoSet={rm.LocoSettled} ms={snapshot.MoveSpeed:F2} tgt={_state.TargetGaitSpeed:F2} pose={poseSpeed:F2} root={_diagRootSpeed:F2} vel={new Vector2(_horizontalVelocity.x, _horizontalVelocity.z).magnitude:F2} gait={snapshot.Gait} latchN={snapshot.StartLatchCount}");
		}

		private float PoseAlignedGaitSpeed()
		{
			float targetGaitSpeed = _state.TargetGaitSpeed;
			int startLatchCount = _state.Snapshot.StartLatchCount;
			if (_startSpeedLatch != startLatchCount)
			{
				_startSpeedLatch = startLatchCount;
				_startEntryGaitSpeed = targetGaitSpeed;
			}
			if (!_config.ExitSpeedFollowsPoseEnabled || !_config.RootMotionLocomotionEnabled)
			{
				return targetGaitSpeed;
			}
			if (targetGaitSpeed <= _startEntryGaitSpeed + 0.01f)
			{
				return targetGaitSpeed;
			}
			float moveSpeed = _state.Snapshot.MoveSpeed;
			if (!(moveSpeed > 0.01f))
			{
				return targetGaitSpeed;
			}
			return Mathf.Min(targetGaitSpeed, moveSpeed);
		}

		private float ComputeSlopeSlideTarget(GroundInfo ground)
		{
			if (!_config.SlopeSlideAngleRampEnabled)
			{
				if (!ground.IsSteepSlope)
				{
					return 0f;
				}
				return _config.SlopeSlideSpeed;
			}
			if (!ground.Grounded)
			{
				return 0f;
			}
			float slideStartAngle = _slopeRules.SlideStartAngle;
			if (ground.SlopeAngle <= slideStartAngle)
			{
				return 0f;
			}
			float num = Mathf.Max(_slopeRules.SlideFullAngle, slideStartAngle + 0.01f);
			float t = Mathf.Clamp01((ground.SlopeAngle - slideStartAngle) / (num - slideStartAngle));
			return Mathf.Lerp(_config.SlopeSlideMinSpeed, _config.SlopeSlideSpeed, t);
		}

		private void ApplyJumpForwardBoost(ref Vector3 horizontal)
		{
			float sprintSpeed = _config.SprintSpeed;
			float num = ((sprintSpeed > 0f) ? Mathf.Clamp01(horizontal.magnitude / sprintSpeed) : 0f);
			if (!(horizontal.magnitude < _config.MinSpeedForForwardJump))
			{
				float num2 = JumpForwardBoost() * num;
				Vector3 vector = ((horizontal.sqrMagnitude > 0.0001f) ? horizontal.normalized : _transform.forward);
				horizontal += vector * num2;
			}
		}

		private void ApplySlideJumpForwardBoost(ref Vector3 horizontal)
		{
			Vector3 vector = ((horizontal.sqrMagnitude > 0.0001f) ? horizontal.normalized : FlatForward());
			float num = Mathf.Max(horizontal.magnitude + _config.SlideJumpForwardBoost, _config.SlideJumpMinLaunchSpeed);
			horizontal = vector * num;
		}

		private float JumpForwardBoost()
		{
			return _config.JumpForwardBoostMale;
		}

		private void ApplyRotation()
		{
			float num = ((_rider != null) ? _rider.YawDelta : 0f);
			if (Mathf.Abs(num) > 0.0001f)
			{
				_transform.rotation = Quaternion.AngleAxis(num, Vector3.up) * _transform.rotation;
			}
			if (_config.LockRotationInAir && !_state.Snapshot.IsGrounded)
			{
				if (_bridge != null)
				{
					_bridge.ConsumeRootRotation();
				}
				_sharpTurning = false;
				return;
			}
			RootMotionFlags rm = ((_bridge != null) ? _bridge.CurrentRootMotion : default(RootMotionFlags));
			if (rm.QuickTurnActive && !_qtWarpPrevActive)
			{
				_qtYawSign = 0f;
				_qtWarpScale = _config.QuickTurnTopUpScale;
				_qtRedirectSticky = false;
				_qtTurnSign = 0f;
				_qtSchedP0 = 0f;
				_qtSchedGap0 = 0f;
				_state.SetQuickTurnYawSign(0f);
			}
			_qtWarpPrevActive = rm.QuickTurnActive;
			if (rm.QuickTurnReverseActive)
			{
				if (_bridge != null)
				{
					_bridge.ConsumeRootRotation();
				}
				if (!_reversePrevActive)
				{
					_reverseElapsed = 0f;
				}
				_reverseElapsed += Time.fixedDeltaTime;
				_reversePrevActive = true;
				Vector3 quickTurnCutTarget = _state.QuickTurnCutTarget;
				if (quickTurnCutTarget.sqrMagnitude > 0.0001f)
				{
					float num2 = ((_config.QuickTurnReverseEaseTime > 0f) ? Mathf.Clamp01(_reverseElapsed / _config.QuickTurnReverseEaseTime) : 1f);
					Quaternion to = Quaternion.LookRotation(quickTurnCutTarget, Vector3.up);
					_transform.rotation = Quaternion.RotateTowards(_transform.rotation, to, _config.QuickTurnReverseRate * num2 * Time.fixedDeltaTime);
				}
				QTurnFrame("QTReverse", 0f, rm);
				_sharpTurning = false;
				return;
			}
			_reversePrevActive = false;
			if (rm.QuickTurnActive)
			{
				Quaternion quaternion = ((_bridge != null) ? _bridge.ConsumeRootRotation() : Quaternion.identity);
				float rootYaw = QTurnYaw(quaternion);
				ApplyQuickTurnRotation(quaternion, rm.QuickTurnCommitActive, rm.QuickTurnProgress);
				QTurnFrame("QTActive", rootYaw, rm);
				_sharpTurning = false;
				return;
			}
			if (rm.QuickTurnExitActive)
			{
				Quaternion quaternion2 = ((_bridge != null) ? _bridge.ConsumeRootRotation() : Quaternion.identity);
				float rootYaw2 = QTurnYaw(quaternion2);
				if (!ReverseImminent(rm.QuickTurnCommitActive))
				{
					ApplyQuickTurnRotation(quaternion2, rm.QuickTurnCommitActive, -1f);
				}
				Vector3 forward = (rm.QuickTurnCommitActive ? _state.QuickTurnDesiredDirection : (_config.QuickTurnExact180Enabled ? ResolveQuickTurnDesired() : _state.DesiredWorldMoveDirection));
				if (forward.sqrMagnitude > 0.0001f)
				{
					Quaternion quaternion3 = Quaternion.LookRotation(forward, Vector3.up);
					if (_config.QuickTurnScheduleAssistEnabled && _config.QuickTurnExact180Enabled && _config.QuickTurnPoseLeadCompEnabled && _config.QuickTurnPoseLeadDeg > 0f && !_qtRedirectSticky && _qtTurnSign != 0f)
					{
						float num3 = _config.QuickTurnPoseLeadDeg * (1f - Mathf.Clamp01(rm.QuickTurnExitBlend));
						quaternion3 = Quaternion.AngleAxis((0f - _qtTurnSign) * num3, Vector3.up) * quaternion3;
					}
					_transform.rotation = (_config.QuickTurnExitBoundedRateEnabled ? Quaternion.RotateTowards(_transform.rotation, quaternion3, _config.QuickTurnExitMaxDegPerSec * Time.fixedDeltaTime) : Quaternion.Slerp(_transform.rotation, quaternion3, Mathf.Clamp01(rm.QuickTurnExitBlend)));
				}
				QTurnFrame("QTExit", rootYaw2, rm);
				_sharpTurning = false;
				return;
			}
			float fixedDeltaTime = Time.fixedDeltaTime;
			bool flag = _horizontalVelocity.magnitude < _config.StartSpeedThreshold;
			Vector3 desiredWorldMoveDirection = _state.DesiredWorldMoveDirection;
			UpdateStartCommit(flag, desiredWorldMoveDirection, fixedDeltaTime);
			Vector3 vector = desiredWorldMoveDirection;
			if (vector.sqrMagnitude < 0.0001f)
			{
				float num4 = 0f;
				if (_bridge != null)
				{
					Quaternion quaternion4 = _bridge.ConsumeRootRotation();
					if (rm.EmoteActive)
					{
						num4 = QTurnYaw(quaternion4);
						_transform.rotation = Quaternion.AngleAxis(num4, Vector3.up) * _transform.rotation;
					}
					else if (rm.TurnStartActive && _config.StartStopFacingFixEnabled)
					{
						_transform.rotation = quaternion4 * _transform.rotation;
						num4 = QTurnYaw(quaternion4);
					}
				}
				QTurnFrame("noinput", num4, rm);
				_sharpTurning = false;
				return;
			}
			if (_startCommitActive && _startCommitDir.sqrMagnitude > 0.0001f)
			{
				vector = _startCommitDir;
			}
			if (_config.StartAimSmoothingEnabled & flag)
			{
				int startLatchCount = _state.Snapshot.StartLatchCount;
				bool flag2 = _config.StartCommitRearmFixEnabled && _lastAimLatch != startLatchCount;
				if (!_startAimActive | flag2)
				{
					_startAimActive = true;
					_lastAimLatch = startLatchCount;
					if (_seedStartAimFromInput)
					{
						_startAimDir = vector;
						_seedStartAimFromInput = false;
					}
					else
					{
						Vector3 forward2 = _transform.forward;
						forward2.y = 0f;
						_startAimDir = ((forward2.sqrMagnitude > 0.0001f) ? forward2.normalized : vector);
					}
				}
				_startAimDir = Vector3.RotateTowards(_startAimDir, vector, _config.StartAimSlewRate * ((float)Math.PI / 180f) * fixedDeltaTime, 0f);
				if (_startAimDir.sqrMagnitude > 0.0001f)
				{
					vector = _startAimDir.normalized;
				}
			}
			else
			{
				_startAimActive = false;
			}
			Vector3 forward3 = _transform.forward;
			forward3.y = 0f;
			float num5 = ((forward3.sqrMagnitude > 0.0001f) ? Vector3.Angle(forward3, vector) : 0f);
			if (num5 >= _config.SharpTurnAngle)
			{
				_sharpTurning = true;
			}
			else if (num5 <= 20f)
			{
				_sharpTurning = false;
			}
			if (rm.TurnStartActive)
			{
				Quaternion quaternion5 = ((_bridge != null) ? _bridge.ConsumeRootRotation() : Quaternion.identity);
				if (_config.StartDirectionSnapEnabled)
				{
					float num6 = PersonStartDirectionSnap.YawScale(_state.Snapshot.StartInitialDirection);
					if (!Mathf.Approximately(num6, 1f))
					{
						quaternion5 = Quaternion.SlerpUnclamped(Quaternion.identity, quaternion5, num6);
					}
				}
				_transform.rotation = quaternion5 * _transform.rotation;
				if (rm.StartExitActive)
				{
					Vector3 forward4 = ((_startCommitActive && _startCommitDir.sqrMagnitude > 0.0001f) ? _startCommitDir : desiredWorldMoveDirection);
					if (forward4.sqrMagnitude > 0.0001f)
					{
						Quaternion b = Quaternion.LookRotation(forward4, Vector3.up);
						if (rm.CrouchTurnStart && _config.CrouchStartTurnCompletionEnabled)
						{
							float t = Mathf.Clamp01(rm.StartExitBlend) * (1f - Mathf.Exp((0f - _config.CrouchStartTurnCompletionSmoothing) * Time.fixedDeltaTime));
							_transform.rotation = Quaternion.Slerp(_transform.rotation, b, t);
						}
						else
						{
							float num7 = Mathf.Clamp01(rm.StartExitBlend);
							_transform.rotation = Quaternion.Slerp(_transform.rotation, b, num7 * num7);
						}
					}
				}
				QTurnFrame("TurnStart", QTurnYaw(quaternion5), rm);
			}
			else
			{
				float landingArcTurnRate = _state.LandingArcTurnRate;
				float num8;
				if (flag || rm.StartTurnActive)
				{
					num8 = Mathf.Clamp(num5 * _config.StartTurnResponsiveness, _config.StartTurnRateMin, _config.StartTurnRate);
				}
				else if (!(landingArcTurnRate >= 0f))
				{
					num8 = ((!_sharpTurning) ? GenericTurnRate() : _config.TurnRateStationary);
				}
				else
				{
					num8 = Mathf.Lerp(landingArcTurnRate, GenericTurnRate(), _state.LandingArcReleaseT);
					_sharpTurning = false;
				}
				if (_input != null && _input.ScriptedMoveActive)
				{
					num8 = Mathf.Max(num8, 600f);
				}
				Quaternion to2 = Quaternion.LookRotation(vector, Vector3.up);
				_transform.rotation = Quaternion.RotateTowards(_transform.rotation, to2, num8 * fixedDeltaTime);
				if (_bridge != null)
				{
					_bridge.ConsumeRootRotation();
				}
				QTurnFrame(_sharpTurning ? "genSharp" : "generic", 0f, rm);
			}
		}

		private float GenericTurnRate()
		{
			float t = ((_config.TurnSpeedReference > 0f) ? Mathf.Clamp01(_horizontalVelocity.magnitude / _config.TurnSpeedReference) : 0f);
			if (!_state.Ground.Grounded)
			{
				return _config.TurnRateAtSpeed * _config.AirControlFactor;
			}
			return Mathf.Lerp(_config.TurnRateStationary, _config.TurnRateAtSpeed, t);
		}

		private void UpdateStartCommit(bool starting, Vector3 liveDir, float dt)
		{
			if (_config.StartLockRotationToCommitted & starting)
			{
				int startLatchCount = _state.Snapshot.StartLatchCount;
				bool flag = _config.StartCommitRearmFixEnabled && _lastStartLatch != startLatchCount;
				if ((!_startCommitActive | flag) && liveDir.sqrMagnitude > 0.0001f)
				{
					_startCommitActive = true;
					_startCommitDir = liveDir;
					_startCommitFrames = 0;
					_lastStartLatch = startLatchCount;
					if (_config.StartLockDiag)
					{
						Debug.Log($"[StartCommit] ARM dir={liveDir} vel={_horizontalVelocity.magnitude:F2} isStarting={_state.Snapshot.IsStarting} moveSpeed={_state.Snapshot.MoveSpeed:F2}");
					}
				}
				_startCommitHold = _config.StartLatchTime;
			}
			else if (_startCommitActive)
			{
				_startCommitHold -= dt;
				if (_startCommitHold <= 0f)
				{
					if (_config.StartLockDiag)
					{
						Debug.Log($"[StartCommit] RELEASE frames={_startCommitFrames} vel={_horizontalVelocity.magnitude:F2}");
					}
					_startCommitActive = false;
				}
			}
			if (_startCommitActive)
			{
				_startCommitFrames++;
			}
		}

		private Vector3 ResolveQuickTurnDesired()
		{
			Vector3 rawWorldMoveDirection = _state.RawWorldMoveDirection;
			if (!_config.QuickTurnExact180Enabled)
			{
				return rawWorldMoveDirection;
			}
			Vector3 quickTurnDesiredDirection = _state.QuickTurnDesiredDirection;
			if (quickTurnDesiredDirection.sqrMagnitude < 0.0001f)
			{
				return rawWorldMoveDirection;
			}
			if (rawWorldMoveDirection.sqrMagnitude < 0.0001f)
			{
				if (!_qtRedirectSticky)
				{
					return quickTurnDesiredDirection;
				}
				return rawWorldMoveDirection;
			}
			if (_qtRedirectSticky)
			{
				return rawWorldMoveDirection;
			}
			if (Vector3.Angle(rawWorldMoveDirection, quickTurnDesiredDirection) > _config.QuickTurnRedirectMinAngle)
			{
				_qtRedirectSticky = true;
				return rawWorldMoveDirection;
			}
			return quickTurnDesiredDirection;
		}

		private void ApplyQuickTurnRotation(Quaternion rootRot, bool lockToTarget, float progress)
		{
			Vector3 vector = ResolveQuickTurnDesired();
			if (vector.sqrMagnitude < 0.0001f)
			{
				_transform.rotation = rootRot * _transform.rotation;
				return;
			}
			bool num = _config.QuickTurnScheduleAssistEnabled && _config.QuickTurnExact180Enabled && !_qtRedirectSticky;
			float f = QTurnYaw(rootRot);
			if (Mathf.Abs(f) > 0.05f)
			{
				_qtYawSign = Mathf.Sign(f);
				if (_qtTurnSign == 0f)
				{
					_qtTurnSign = _qtYawSign;
				}
				_state.SetQuickTurnYawSign(_qtYawSign);
			}
			if (num && progress >= 0f && _qtTurnSign != 0f && _config.QuickTurnPoseLeadCompEnabled && _config.QuickTurnPoseLeadDeg > 0f)
			{
				vector = Quaternion.AngleAxis((0f - _qtTurnSign) * _config.QuickTurnPoseLeadDeg, Vector3.up) * vector;
			}
			float target = _config.QuickTurnTopUpScale;
			if (!num && _config.QuickTurnRedirectEnabled && progress >= 0f && _qtYawSign != 0f)
			{
				float f2 = ResidualTo(vector);
				float num2 = ((Mathf.Sign(f2) == _qtYawSign) ? Mathf.Abs(f2) : (360f - Mathf.Abs(f2)));
				if (num2 <= _config.QuickTurnRedirectMaxArc)
				{
					float num3 = Mathf.Max((_config.QuickTurnScheduleAssistEnabled ? _state.QuickTurnWarpGap : _state.QuickTurnInitialGap) * (1f - progress), 10f);
					target = Mathf.Clamp(num2 / num3, _config.QuickTurnWarpScaleMin, _config.QuickTurnWarpScaleMax);
				}
			}
			_qtWarpScale = Mathf.MoveTowards(_qtWarpScale, target, 4f * Time.fixedDeltaTime);
			float qtWarpScale = _qtWarpScale;
			if (Mathf.Approximately(qtWarpScale, 1f))
			{
				_transform.rotation = rootRot * _transform.rotation;
			}
			else
			{
				float num4 = ResidualTo(vector);
				Quaternion quaternion = Quaternion.SlerpUnclamped(Quaternion.identity, rootRot, qtWarpScale);
				_transform.rotation = quaternion * _transform.rotation;
				float num5 = ResidualTo(vector);
				bool flag = num4 > 0f != num5 > 0f;
				bool flag2 = Mathf.Abs(num5) > Mathf.Abs(num4);
				if (Mathf.Abs(num4) < _config.QuickTurnSnapMaxAngle && (flag | flag2) && !ReverseImminent(lockToTarget))
				{
					_transform.rotation = Quaternion.LookRotation(vector, Vector3.up);
				}
			}
			if (!num || !(progress > 0f) || _qtTurnSign == 0f || ReverseImminent(lockToTarget))
			{
				return;
			}
			float f3 = ResidualTo(vector);
			float num6 = ((Mathf.Sign(f3) == _qtTurnSign) ? Mathf.Abs(f3) : (360f - Mathf.Abs(f3)));
			if (num6 <= _config.QuickTurnRedirectMaxArc)
			{
				if (_qtSchedGap0 <= 0f || progress < _qtSchedP0)
				{
					_qtSchedP0 = progress;
					_qtSchedGap0 = num6;
				}
				float num7 = Mathf.Max(_config.QuickTurnScheduleEndProgress - _qtSchedP0, 0.0001f);
				float num8 = _qtSchedGap0 * Mathf.Clamp01(1f - (progress - _qtSchedP0) / num7);
				float num9 = num6 - num8;
				if (num9 > 0f)
				{
					float num10 = Mathf.Min(num9, _config.QuickTurnAssistMaxDegPerSec * Time.fixedDeltaTime);
					_transform.rotation = Quaternion.AngleAxis(_qtTurnSign * num10, Vector3.up) * _transform.rotation;
				}
			}
		}

		private bool ReverseImminent(bool lockToTarget)
		{
			if (lockToTarget || !_config.QuickTurnReverseEnabled)
			{
				return false;
			}
			Vector3 rawWorldMoveDirection = _state.RawWorldMoveDirection;
			Vector3 quickTurnOriginalDirection = _state.QuickTurnOriginalDirection;
			if (rawWorldMoveDirection.sqrMagnitude < 0.0001f || quickTurnOriginalDirection.sqrMagnitude < 0.0001f)
			{
				return false;
			}
			return Vector3.Angle(rawWorldMoveDirection, quickTurnOriginalDirection) <= _config.QuickTurnReverseAngle;
		}

		private static float QTurnYaw(Quaternion q)
		{
			return Mathf.DeltaAngle(0f, q.eulerAngles.y);
		}

		private float QTurnResidual()
		{
			return ResidualTo(_state.DesiredWorldMoveDirection);
		}

		private float ResidualTo(Vector3 desired)
		{
			Vector3 forward = _transform.forward;
			forward.y = 0f;
			if (forward.sqrMagnitude < 0.0001f || desired.sqrMagnitude < 0.0001f)
			{
				return 0f;
			}
			return Vector3.SignedAngle(forward, desired, Vector3.up);
		}

		private void QTurnFrame(string tag, float rootYaw, RootMotionFlags rm)
		{
		}

		private Vector3 BlockVelocityIntoWall(Vector3 velocity)
		{
			if (!_rigidbody || !_config)
			{
				return velocity;
			}
			if (!_wallCapsuleResolved)
			{
				_wallCapsule = _rigidbody.GetComponent<CapsuleCollider>();
				_wallCapsuleResolved = true;
			}
			if (!_wallCapsule)
			{
				return velocity;
			}
			Vector3 lhs = new Vector3(velocity.x, 0f, velocity.z);
			if (lhs.sqrMagnitude < 0.0001f)
			{
				return velocity;
			}
			Vector3 normalized = lhs.normalized;
			if (!Physics.SphereCast(_transform.position + _wallCapsule.center, maxDistance: _wallCapsule.radius + 0.2f, layerMask: (int)_config.WallLayerMask & ~(1 << _transform.gameObject.layer), radius: _wallCapsule.radius * 0.9f, direction: normalized, hitInfo: out var hitInfo, queryTriggerInteraction: QueryTriggerInteraction.Ignore))
			{
				return velocity;
			}
			if (Vector3.Angle(hitInfo.normal, Vector3.up) <= _config.WallMinSurfaceAngle)
			{
				return velocity;
			}
			Vector3 normal = hitInfo.normal;
			normal.y = 0f;
			if (normal.sqrMagnitude < 0.0001f)
			{
				return velocity;
			}
			normal.Normalize();
			float num = Vector3.Dot(lhs, -normal);
			if (num <= 0f)
			{
				return velocity;
			}
			lhs += normal * num;
			return new Vector3(lhs.x, velocity.y, lhs.z);
		}

		private Vector3 SteamBlockVelocityIntoProp(Vector3 velocity)
		{
			if (!_rigidbody)
			{
				return velocity;
			}
			if (!_wallCapsuleResolved)
			{
				_wallCapsule = _rigidbody.GetComponent<CapsuleCollider>();
				_wallCapsuleResolved = true;
			}
			if (!_wallCapsule)
			{
				return velocity;
			}
			Vector3 lhs = new Vector3(velocity.x, 0f, velocity.z);
			if (lhs.sqrMagnitude < 0.0001f)
			{
				return velocity;
			}
			Vector3 normalized = lhs.normalized;
			int layerMask = ~(1 << _transform.gameObject.layer);
			float radius = _wallCapsule.radius;
			float num = Mathf.Max(_wallCapsule.height * 0.5f - radius, 0f);
			Vector3 vector = _transform.TransformPoint(_wallCapsule.center);
			Vector3 up = _transform.up;
			Vector3 point = vector + up * num;
			Vector3 point2 = vector - up * num;
			float maxDistance = lhs.magnitude * Time.fixedDeltaTime + SteamPropLookAhead;
			int num2 = Physics.CapsuleCastNonAlloc(point, point2, radius * 0.95f, normalized, SteamPropHits, maxDistance, layerMask, QueryTriggerInteraction.Collide);
			Vector3 vector2 = Vector3.zero;
			float num3 = float.MaxValue;
			for (int i = 0; i < num2; i++)
			{
				RaycastHit raycastHit = SteamPropHits[i];
				if (!raycastHit.collider || !raycastHit.collider.isTrigger || raycastHit.distance >= num3 || !raycastHit.collider.GetComponentInParent<DestroyableMeshBase>())
				{
					continue;
				}
				Vector3 vector3 = raycastHit.normal;
				if (raycastHit.distance <= 0.0001f)
				{
					vector3 = vector - raycastHit.collider.bounds.center;
					vector3.y = 0f;
					if (vector3.sqrMagnitude < 0.0001f)
					{
						continue;
					}
					vector3.Normalize();
				}
				num3 = raycastHit.distance;
				vector2 = vector3;
				_ = raycastHit.collider.name;
			}
			if (num3 == float.MaxValue)
			{
				return velocity;
			}
			vector2.y = 0f;
			if (vector2.sqrMagnitude < 0.0001f)
			{
				return velocity;
			}
			vector2.Normalize();
			float num4 = Vector3.Dot(lhs, -vector2);
			if (num4 <= 0f)
			{
				return velocity;
			}
			_ = SteamPropBlockLog;
			lhs += vector2 * num4;
			return new Vector3(lhs.x, velocity.y, lhs.z);
		}

		private void ApplyStepUp()
		{
			if (_groundProbe == null || !_rigidbody)
			{
				return;
			}
			float magnitude = _input.Current.Move.magnitude;
			if (!_groundProbe.TryStepUp(magnitude, out var proposal))
			{
				return;
			}
			float num = proposal.TargetPosition.y - _rigidbody.position.y;
			if ((bool)_config && _config.StepRampEnabled && num <= _config.StepRampMaxHeight)
			{
				BeginStepClimb(proposal.TargetPosition);
				return;
			}
			Vector3 position = _rigidbody.position;
			Vector3 targetPosition = proposal.TargetPosition;
			_rigidbody.position = targetPosition;
			if (proposal.ZeroVerticalVelocity)
			{
				Vector3 velocity = _rigidbody.velocity;
				velocity.y = 0f;
				_rigidbody.velocity = velocity;
			}
			_visualOffset += position - targetPosition;
		}

		private void BeginStepClimb(Vector3 target)
		{
			_stepTarget = target;
			_stepClimbSpeed = Mathf.Max(_horizontalVelocity.magnitude, _config.StepRampMinSpeed);
			_stepClimbElapsed = 0f;
			if ((bool)_rigidbody)
			{
				_rigidbody.detectCollisions = false;
				_climbCollisionsDisabled = true;
			}
			_stepClimbing = true;
		}

		private void AdvanceStepClimb(float dt)
		{
			if (!_rigidbody)
			{
				EndStepClimb();
				return;
			}
			_stepClimbElapsed += dt;
			if (_bridge != null)
			{
				_bridge.ConsumeRootDelta();
				_bridge.ConsumeRootRotation();
			}
			Vector3 position = _rigidbody.position;
			Vector3 vector = new Vector3(_stepTarget.x - position.x, 0f, _stepTarget.z - position.z);
			float magnitude = vector.magnitude;
			float num = _stepTarget.y - position.y;
			float num2 = _config.StepRampMaxHeight / Mathf.Max(_stepClimbSpeed, 0.01f) * 2f + 0.1f;
			if ((magnitude < 0.03f && num <= 0.02f) || _stepClimbElapsed > num2)
			{
				_rigidbody.position = _stepTarget;
				EndStepClimb();
				return;
			}
			Vector3 obj = ((magnitude > 0.0001f) ? (vector / magnitude) : FlatForward());
			float num3 = Mathf.Min(_stepClimbSpeed, magnitude / dt);
			Vector3 velocity = obj * num3;
			if (num > 0.02f)
			{
				float num4 = Mathf.Tan(_config.StepRampAngle * ((float)Math.PI / 180f));
				velocity.y = Mathf.Min(_stepClimbSpeed * num4, num / dt);
			}
			_rigidbody.velocity = velocity;
		}

		private void EndStepClimb()
		{
			_stepClimbing = false;
			if (_climbCollisionsDisabled && (bool)_rigidbody)
			{
				_rigidbody.detectCollisions = true;
				_climbCollisionsDisabled = false;
			}
			_horizontalVelocity = FlatForward() * _stepClimbSpeed;
			if ((bool)_rigidbody)
			{
				_rigidbody.velocity = _horizontalVelocity;
			}
			_verticalVelocity = -2f;
			_prevStopRootActive = false;
			_prevStartRootActive = false;
			_prevLocoRootActive = false;
			_prevLocoSettled = false;
			_prevQuickTurnRootActive = false;
		}

		public void VisualStep()
		{
			if (!(_visualOffset == Vector3.zero) || !(_visualOffsetVelocity == Vector3.zero))
			{
				_visualOffset = Vector3.SmoothDamp(_visualOffset, Vector3.zero, ref _visualOffsetVelocity, _config ? _config.StepSmoothTime : 0.18f);
				if (_visualOffset.sqrMagnitude < 4E-06f)
				{
					_visualOffset = Vector3.zero;
					_visualOffsetVelocity = Vector3.zero;
				}
				Transform transform = ResolveSkinRoot();
				if ((bool)transform)
				{
					transform.localPosition = Quaternion.Inverse(_transform.rotation) * _visualOffset;
				}
			}
		}

		private Transform ResolveSkinRoot()
		{
			if ((bool)_skinRoot)
			{
				return _skinRoot;
			}
			SkinnedMeshRenderer componentInChildren = _transform.GetComponentInChildren<SkinnedMeshRenderer>();
			if (!componentInChildren)
			{
				return null;
			}
			Transform transform = componentInChildren.transform;
			while ((bool)transform.parent && transform.parent != _transform)
			{
				transform = transform.parent;
			}
			if (transform != _transform)
			{
				_skinRoot = transform;
			}
			return _skinRoot;
		}
	}
}
