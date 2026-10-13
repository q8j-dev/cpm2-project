using UnityEngine;

namespace _Car_Parking.Scripts.Person.Locomotion.Config
{
	[CreateAssetMenu(fileName = "PersonLocomotionConfig", menuName = "Scriptable/Person/PersonLocomotionConfig")]
	public class PersonLocomotionConfig : ScriptableObject
	{
		private const string Cfg = "Config";

		private const string MoveTab = "Config/Movement";

		private const string TurnTab = "Config/Turning";

		private const string AnimTab = "Config/Animation";

		private const string InputTab = "Config/Input";

		private const string StopTab = "Config/Start & Stop";

		private const string JumpTab = "Config/Jump";

		private const string MiscTab = "Config/Misc";

		private const string AudioTab = "Config/Audio";

		public float WalkSpeed = 1.4f;

		public float RunSpeed = 2.5f;

		public float SprintSpeed = 6f;

		public float Acceleration = 12f;

		public float StartAcceleration = 5f;

		public float Deceleration = 16f;

		public float AirControlFactor = 0.35f;

		public float GroundFriction = 8f;

		public float AirDrag = 0.5f;

		public bool LockRotationInAir = true;

		public float RotationSmoothing = 12f;

		public float TurnRateStationary = 540f;

		public float TurnRateAtSpeed = 75f;

		public float TurnSpeedReference = 1.6f;

		public float SharpTurnAngle = 135f;

		public float MaxLeanRate = 275f;

		public float LeanSmoothing = 5f;

		public float LeanSpeedReference = 4.5f;

		public float HeadLeanLimit = 18f;

		public float HeadLeanLimitWhileStanding = 18f;

		public float HeadLookAngleRange = 90f;

		public float HeadLookStrength = 1f;

		public float BodyLookStrength = 0.6f;

		public bool HeadLookTowardDirectionEnabled = true;

		public float HeadLookTurnMinAngle = 45f;

		public float HeadLookTurnStartSmoothing = 2f;

		public float HeadLookTurnEndSmoothing = 12f;

		public float HeadLookTurnAccelPower = 2f;

		public float SpeedChangeDamping = 10f;

		public float MoveSpeedDampTime = 0.05f;

		public float ButtonHoldThreshold = 0.15f;

		public float ShuffleLatchDuration = 0.3f;

		public float StartSpeedThreshold = 1f;

		public float StoppedSpeedThreshold = 0.5f;

		public float StartLatchTime = 0.2f;

		public float StopResumeStartSpeedThreshold = 0.35f;

		public bool StopResumeMatchIntent = true;

		public float MoveCoastTime = 0.15f;

		public float StartTierSettleTime = 0.05f;

		public float KeyboardChordSettleTime = 0.05f;

		public float RunDeflectionEnter = 0.9f;

		public float RunDeflectionExit = 0.75f;

		public SprintActivationMode SprintActivation;

		public float DeflectionRunEnter = 0.55f;

		public float DeflectionRunExit = 0.45f;

		public float SprintDeflectionEnter = 0.88f;

		public float SprintDeflectionExit = 0.75f;

		public bool SprintLockEnabled;

		public float SprintLockMinHoldTime = 0.6f;

		public float SprintLockZoneMinUp = 0.7f;

		public bool IdleBreaksEnabled = true;

		public float IdleBreakMinInterval = 8f;

		public float IdleBreakMaxInterval = 20f;

		public float IdleBreakFirstDelayMin = 6f;

		public float IdleBreakFirstDelayMax = 14f;

		public int IdleBreakCount = 7;

		public bool AnimatorUpdateInFixedStep = true;

		public bool EmoteRootMotionEnabled = true;

		public bool RootMotionLocomotionEnabled;

		public float RootMotionBlendRamp = 4f;

		public bool StartPoseFollowsSpeedEnabled = true;

		public bool ExitSpeedFollowsPoseEnabled = true;

		public bool StartRootMotionEnabled = true;

		public float StartRootMotionMaxAngle = 45f;

		public bool StartDirectionSnapEnabled = true;

		public bool StartTurnRootMotionEnabled = true;

		public float StartTurnRootMotionMinAngle = 135f;

		public bool StartTurnRootMotionMaleWalkFullRange = true;

		public bool StartTurnRootMotionRunFullRange;

		public bool StartTurnExitRampEnabled = true;

		public bool StartExitRampEnabled = true;

		public bool StartExitFollowsRootMotion = true;

		public bool CrouchStartTurnCompletionEnabled = true;

		public float CrouchStartTurnCompletionSmoothing = 12f;

		public bool StartStopFacingFixEnabled = true;

		public bool StartTurnEnabled = true;

		public float StartTurnRate = 250f;

		public float StartTurnRateMin = 12f;

		public float StartTurnResponsiveness = 4.5f;

		public bool StartLockRotationToCommitted;

		public bool StartLockDiag;

		public bool StartAimSmoothingEnabled;

		public float StartAimSlewRate = 180f;

		public bool StartRefireDebounceEnabled;

		public float StartRefireDebounce = 0.35f;

		public bool SourceDirectionSmoothingEnabled = true;

		public float SourceDirectionHalflife = 0.15f;

		public float SourceDirectionResetTime = 0.22f;

		public bool StartCommitRearmFixEnabled = true;

		public bool QuickTurnEnabled = true;

		public float QuickTurnMinSpeed = 1.1f;

		public float QuickTurnMinAngle = 140f;

		public float QuickTurnFlickAngle = 120f;

		public float QuickTurnFlickWindow = 0.15f;

		public float QuickTurnTriggerHold = 0.2f;

		public float QuickTurnRearmAngle = 45f;

		public float QuickTurnMaxDuration = 0.9f;

		public bool QuickTurnExitRampEnabled = true;

		public bool QuickTurnExitBoundedRateEnabled = true;

		public float QuickTurnExitMaxDegPerSec = 240f;

		public float QuickTurnTopUpScale = 1.12f;

		public bool QuickTurnExact180Enabled = true;

		public bool QuickTurnScheduleAssistEnabled = true;

		public float QuickTurnAssistMaxDegPerSec = 360f;

		public float QuickTurnScheduleEndProgress = 0.8f;

		public bool QuickTurnPoseLeadCompEnabled = true;

		public float QuickTurnPoseLeadDeg = 12f;

		public bool QuickTurnReverseEnabled;

		public float QuickTurnReverseAngle = 55f;

		public float QuickTurnReverseMaxProgress = 0.45f;

		public float QuickTurnReverseRate = 400f;

		public float QuickTurnReverseEaseTime = 0.15f;

		public float QuickTurnReverseSettleAngle = 8f;

		public float QuickTurnReverseMaxDuration = 0.6f;

		public bool QuickTurnCommitLockEnabled;

		public bool QuickTurnRedirectEnabled = true;

		public float QuickTurnRedirectMinAngle = 40f;

		public float QuickTurnRedirectCutAngle = 25f;

		public float QuickTurnRedirectMaxArc = 220f;

		public float QuickTurnWarpScaleMin = 0.7f;

		public float QuickTurnWarpScaleMax = 1.8f;

		public float QuickTurnSnapMaxAngle = 30f;

		public bool LandingQuickTurnEnabled = true;

		public float LandingQuickTurnLatchTime = 0.25f;

		public bool LandingArcEnabled = true;

		public float LandingArcMinAirTime = 0.15f;

		public float LandingArcWindowPerSpeed = 0.12f;

		public float LandingArcWindowMax = 0.9f;

		public float LandingArcReleaseTime = 0.2f;

		public float LandingArcRadius = 9f;

		public float LandingArcTurnRateMin = 25f;

		public float LandingArcTurnRateMax = 90f;

		public float LandingArcSpeedBleed = 3f;

		public float LandingArcSessionMax = 1.5f;

		public bool StopSpeedClampEnabled = true;

		public float StopOffsetScale = 0.25f;

		public float StopPhaseRefLeft = 0.55f;

		public float StopPhaseRefRight = 0.05f;

		public float StopOffsetMax = 0.2f;

		public float StopFootLevelDeadzone = 0.15f;

		public bool WalkStopPhaseMatchEnabled = true;

		public float WalkStopEntryFade = 0.225f;

		public AnimationCurve WalkStopEntryTimeByPosDot = new AnimationCurve();

		public bool StopPhaseGateEnabled = true;

		public float StopPhaseGateWindow = 0.08f;

		public float StopPhaseGateMaxWait = 0.7f;

		public float StopHoldTime = 0.5f;

		public bool StartRollbackEnabled = true;

		public float StartRollbackSpeed = 1.5f;

		public float StartRollbackExitBlend = 0.1f;

		public bool InstantStopEnabled;

		public float InstantStopBlendTime = 0.05f;

		public float StopEntryTargetLeft = 0.065f;

		public float StopEntryTargetRight = -0.065f;

		public float StopMatchTolerance = 0.04f;

		public float StopClipStartOffset = 0.04f;

		public float RunStopClipStartOffset;

		public bool WallStopEnabled = true;

		public LayerMask WallLayerMask = -1;

		public float WallProbeDistance = 0.15f;

		public float WallMaxFaceAngle = 55f;

		public float WallMinSurfaceAngle = 60f;

		public float WallStallSpeed = 0.4f;

		public float WallPushAllowedTime = 0.35f;

		public bool WallStopUsePhaseGate = true;

		public float JumpHeight = 1.2f;

		public float JumpForwardBoostMale = 100f;

		public float JumpForwardBoostFemale = 100f;

		public float CoyoteTime = 0.12f;

		public float JumpCooldown = 0.35f;

		public float JumpBufferTime = 0.12f;

		public float MinSpeedForForwardJump = 0.2f;

		public bool JumpFromAnyStateEnabled = true;

		public float JumpLaunchMaxWait = 0.5f;

		public float StandingJumpMaxMoveInput = 0.15f;

		public bool JumpUseAnimationEvent = true;

		public bool SlideJumpEnabled = true;

		public float SlideJumpMinSpeed = 2f;

		public float SlideJumpHeight = 1.1f;

		public float SlideJumpForwardBoost = 3f;

		public float SlideJumpMinLaunchSpeed = 5f;

		public float CrouchCooldown = 0.5f;

		public bool CeilingGatesStandUp = true;

		public bool AutoStandAfterSlide = true;

		public float SlideStandNormalizedTime = 0.7f;

		public float SlideEndSpeed = 1.6f;

		public float SlideMaxDuration = 2.5f;

		public bool CrouchJumpEnabled = true;

		public LayerMask GroundLayerMask = -1;

		public float GroundCheckRadius = 0.25f;

		public float GroundCheckDistance = 0.35f;

		public float MaxSlopeAngle = 50f;

		public float SlopeSlideSpeed = 4f;

		public bool SlopeSlideAngleRampEnabled = true;

		public float SlopeSlideStartAngle = 30f;

		public float SlopeSlideFullAngle = 55f;

		public float SlopeSlideMinSpeed = 1f;

		public float SlopeSlideAcceleration = 8f;

		public float ArenaMaxSlopeAngle = 30f;

		public float ArenaSlopeSlideStartAngle = 30f;

		public float ArenaSlopeSlideFullAngle = 55f;

		public bool InclineAngleEnabled = true;

		public float InclineAngleMax = 25f;

		public float InclineAngleScale = 1f;

		public float InclineAngleSmoothing = 90f;

		public float InclineAngleDeadZone = 1f;

		public bool SlopeSpeedEnabled = true;

		public float SlopeSpeedStartAngle = 8f;

		public float SlopeSpeedFullAngle = 30f;

		public float SlopeSpeedMinMultiplier = 0.55f;

		public bool ProceduralLeanEnabled = true;

		public float ProceduralLeanScale = 0.5f;

		public float ProceduralLeanMaxAngle = 20f;

		public float StepHeight = 1f;

		public float LandingEpsilon = 0.02f;

		public float StepForwardProbeDistance = 0.5f;

		public float StepMaxFaceAngle = 50f;

		public float StepLedgeMaxSurfaceAngle = 45f;

		public int StepForwardSampleCount = 6;

		public float StepMidJumpVelocityThreshold = 0.1f;

		public float StepInputThreshold = 0.05f;

		public float StepSmoothTime = 0.18f;

		public bool StepRampEnabled = true;

		public float StepRampAngle = 45f;

		public float StepRampMinSpeed = 1.5f;

		public float StepRampMaxHeight = 0.45f;

		public bool RagdollOnFall = true;

		public float RagdollFallSpeed = 9f;

		public float RagdollMinAirTime = 0.25f;

		public bool UseMCUnityCharacter;

		public GameObject MCUnityCharacterPrefab;

		public bool InfiniteStamina;

		public bool PlatformRideEnabled = true;

		public bool PlatformRotationCarryEnabled = true;

		public float PlatformExitMomentumScale = 1f;

		public float PlatformMaxCarrySpeed = 25f;

		public bool FootstepAudioEnabled = true;

		[Range(0.05f, 1f)]
		public float FootPlantSpeedRatio = 0.45f;

		[Range(0.1f, 2f)]
		public float FootLiftSpeedRatio = 0.9f;

		public float FootReferenceMinSpeed = 0.6f;

		public float FootGroundedMaxHeight = 0.35f;

		public float FootstepMinInterval = 0.12f;

		public bool ClothesWithFootsteps = true;

		public bool BreathAudioEnabled = true;

		public float BreathMoveSpeedThreshold = 0.15f;

		[Range(0.5f, 1f)]
		public float BreathStaminaFullThreshold = 0.99f;

		public float PlayerSpeedRtpcMax = 5f;

		private bool IsDeflectionSprint => SprintActivation == SprintActivationMode.Deflection;

		private bool ShowQuickTurnExitBoundedRate
		{
			get
			{
				if (QuickTurnEnabled)
				{
					return QuickTurnExitBoundedRateEnabled;
				}
				return false;
			}
		}

		private bool ShowQuickTurnScheduleAssist
		{
			get
			{
				if (QuickTurnEnabled)
				{
					return QuickTurnScheduleAssistEnabled;
				}
				return false;
			}
		}

		private bool ShowQuickTurnPoseLeadComp
		{
			get
			{
				if (QuickTurnEnabled)
				{
					return QuickTurnPoseLeadCompEnabled;
				}
				return false;
			}
		}

		private bool ShowQuickTurnReverse
		{
			get
			{
				if (QuickTurnEnabled)
				{
					return QuickTurnReverseEnabled;
				}
				return false;
			}
		}

		private bool ShowQuickTurnRedirect
		{
			get
			{
				if (QuickTurnEnabled)
				{
					return QuickTurnRedirectEnabled;
				}
				return false;
			}
		}

		private bool ShowLandingQuickTurn
		{
			get
			{
				if (QuickTurnEnabled)
				{
					return LandingQuickTurnEnabled;
				}
				return false;
			}
		}

		private bool IsFootLiftAbovePlant(float value)
		{
			return value > FootPlantSpeedRatio;
		}

		private bool IsRunSpeedAboveWalk(float value)
		{
			return value > WalkSpeed;
		}

		private bool IsSprintSpeedAboveRun(float value)
		{
			return value > RunSpeed;
		}

		private bool IsSlopeFullAngleAboveStart(float value)
		{
			return value > SlopeSpeedStartAngle;
		}

		private bool IsSlideFullAngleAboveStart(float value)
		{
			return value > SlopeSlideStartAngle;
		}

		private bool IsArenaSlideFullAngleAboveStart(float value)
		{
			return value > ArenaSlopeSlideStartAngle;
		}

		private bool IsRunDeflectionExitBelowEnter(float value)
		{
			return value < RunDeflectionEnter;
		}

		private bool IsDeflectionRunExitBelowEnter(float value)
		{
			return value < DeflectionRunEnter;
		}

		private bool IsSprintDeflectionExitBelowEnter(float value)
		{
			return value < SprintDeflectionEnter;
		}

		private bool IsIdleBreakMaxAboveMin(float value)
		{
			return value >= IdleBreakMinInterval;
		}

		private bool IsIdleBreakFirstDelayMaxAboveMin(float value)
		{
			return value >= IdleBreakFirstDelayMin;
		}

		private bool IsWarpScaleMaxAboveMin(float value)
		{
			return value > QuickTurnWarpScaleMin;
		}

		private bool IsStopGateMaxWaitAboveWindow(float value)
		{
			return value > StopPhaseGateWindow;
		}
	}
}
