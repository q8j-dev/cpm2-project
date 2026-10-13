using CarInteractionAnimationsScripts;
using RootMotion.FinalIK;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Person.Locomotion.Config;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public class PersonFootIK : MonoBehaviour
	{
		private const bool Diag = false;

		private const bool DiagSetup = false;

		private const bool DiagRide = false;

		private const bool DiagSlopeRide = false;

		private const int SlopeRideDiagBudget = 400;

		private const int SetupDelayFrames = 10;

		private const float MinKneeBendDegrees = 3f;

		private const float AnimatedBendEnterDegrees = 6f;

		private const float AnimatedBendExitDegrees = 3f;

		private const float WeightEpsilon = 0.0001f;

		private const float SolverInitiationWeight = 0.001f;

		private PersonFootIKConfig _config;

		private PersonLocomotionConfig _locomotion;

		private MainPerson _mainPerson;

		private PersonLocomotion _root;

		private Rigidbody _rigidbody;

		private Transform _hips;

		private IK _leftLeg;

		private IK _rightLeg;

		private GrounderIK _grounder;

		private bool _initiated;

		private bool _solversSuspended = true;

		private bool _leftAnimatedBend;

		private bool _rightAnimatedBend;

		private int _humanFrames;

		private bool _diagWaitLogged;

		private bool _probeLogged;

		private int _diagFrame;

		private int _diagRideFrame;

		private int _slopeRideDiagFrames;

		private float _diagPrevPelvis;

		private float _diagPrevLag;

		private bool _hasSlopeRideDiagPrev;

		[Inject]
		public void Construct(IPersonLocomotionConfigService configService)
		{
			_config = configService?.FootIK;
			_locomotion = configService?.Locomotion;
		}

		private void Awake()
		{
			TryGetComponent<MainPerson>(out _mainPerson);
			TryGetComponent<PersonLocomotion>(out _root);
			TryGetComponent<Rigidbody>(out _rigidbody);
		}

		private void LateUpdate()
		{
			if (!_config || !_config.Enable || !_root || !_mainPerson)
			{
				return;
			}
			if (_initiated && ((object)ResolveHips() != _hips || !AnimatorPosing(ResolveAnimator())))
			{
				Teardown();
			}
			if (_root.IsSuspended || CarInteractionAnimating())
			{
				if (_initiated)
				{
					Teardown();
				}
				_humanFrames = 0;
			}
			else if (!_root.Ground.Grounded)
			{
				if (_initiated)
				{
					_grounder.weight = 0f;
					SetSolversEnabled(enabled: false);
					NeutralizeLegSolvers();
					_solversSuspended = true;
				}
			}
			else
			{
				if (!_initiated && !TrySetup())
				{
					return;
				}
				if (_solversSuspended)
				{
					SetSolversEnabled(enabled: true);
					NeutralizeLegSolvers();
					if (!_grounder.solver.root)
					{
						_grounder.weight = 0.001f;
						return;
					}
					_grounder.weight = 0f;
					_solversSuspended = false;
					_grounder.solver.Reset();
				}
				DriveWeight();
				bool flag = _grounder.weight > 0.0001f;
				SetSolversEnabled(flag);
				if (!flag)
				{
					NeutralizeLegSolvers();
				}
			}
		}

		public void NotifyTeleport()
		{
			if (_initiated)
			{
				Teardown();
			}
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

		private void SetSolversEnabled(bool enabled)
		{
			if (enabled)
			{
				if ((bool)_leftLeg && !_leftLeg.enabled)
				{
					_leftLeg.enabled = true;
				}
				if ((bool)_rightLeg && !_rightLeg.enabled)
				{
					_rightLeg.enabled = true;
				}
				if ((bool)_grounder && !_grounder.enabled)
				{
					_grounder.enabled = true;
				}
				return;
			}
			NeutralizeLegSolvers();
			if ((bool)_grounder && _grounder.enabled)
			{
				_grounder.enabled = false;
			}
			if ((bool)_leftLeg && _leftLeg.enabled)
			{
				_leftLeg.enabled = false;
			}
			if ((bool)_rightLeg && _rightLeg.enabled)
			{
				_rightLeg.enabled = false;
			}
		}

		private void NeutralizeLegSolvers()
		{
			NeutralizeLegSolver(_leftLeg);
			NeutralizeLegSolver(_rightLeg);
		}

		private static void NeutralizeLegSolver(IK leg)
		{
			if ((bool)leg)
			{
				IKSolver iKSolver = leg.GetIKSolver();
				iKSolver.IKPositionWeight = 0f;
				if (iKSolver is IKSolverLeg iKSolverLeg)
				{
					iKSolverLeg.IKRotationWeight = 0f;
				}
				else if (iKSolver is IKSolverTrigonometric iKSolverTrigonometric)
				{
					iKSolverTrigonometric.IKRotationWeight = 0f;
				}
			}
		}

		private Animator ResolveAnimator()
		{
			if (!_mainPerson)
			{
				return null;
			}
			PersonAnimationsController personAnimationsController = _mainPerson.PersonAnimationsController;
			if (!personAnimationsController || !personAnimationsController.Animator)
			{
				return _mainPerson.Animator;
			}
			return personAnimationsController.Animator;
		}

		private Transform ResolveHips()
		{
			Animator animator = ResolveAnimator();
			if (!animator || !animator.isHuman)
			{
				return null;
			}
			return animator.GetBoneTransform(HumanBodyBones.Hips);
		}

		private static bool AnimatorPosing(Animator animator)
		{
			if ((bool)animator && animator.enabled)
			{
				return animator.speed > 0f;
			}
			return false;
		}

		private bool CaptureContextValid()
		{
			if (_root.IsActive && !_root.IsSuspended && !CarInteractionAnimating())
			{
				return _root.Ground.Grounded;
			}
			return false;
		}

		private static string CurrentClipName(Animator animator)
		{
			if (!animator.runtimeAnimatorController || animator.layerCount == 0)
			{
				return "no controller";
			}
			AnimatorClipInfo[] currentAnimatorClipInfo = animator.GetCurrentAnimatorClipInfo(0);
			if (currentAnimatorClipInfo.Length == 0 || !currentAnimatorClipInfo[0].clip)
			{
				return "none";
			}
			return currentAnimatorClipInfo[0].clip.name;
		}

		private static float KneeBendDegrees(Transform thigh, Transform calf, Transform foot)
		{
			Vector3 vector = calf.position - thigh.position;
			Vector3 to = foot.position - calf.position;
			if (vector.sqrMagnitude < 1E-08f || to.sqrMagnitude < 1E-08f)
			{
				return 0f;
			}
			return Vector3.Angle(vector, to);
		}

		private bool TrySetup()
		{
			Animator animator = ResolveAnimator();
			if (!animator || !animator.isHuman)
			{
				_humanFrames = 0;
				DiagWait("animator null or not humanoid");
				return false;
			}
			if (!AnimatorPosing(animator))
			{
				_humanFrames = 0;
				DiagWait("animator not posing (disabled or speed 0)");
				return false;
			}
			if (!CaptureContextValid())
			{
				_humanFrames = 0;
				DiagWait("not in on-foot context");
				return false;
			}
			_humanFrames++;
			if (_humanFrames < 10)
			{
				return false;
			}
			Transform boneTransform = animator.GetBoneTransform(HumanBodyBones.Hips);
			Transform boneTransform2 = animator.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
			Transform boneTransform3 = animator.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
			Transform boneTransform4 = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
			Transform boneTransform5 = animator.GetBoneTransform(HumanBodyBones.LeftToes);
			Transform boneTransform6 = animator.GetBoneTransform(HumanBodyBones.RightUpperLeg);
			Transform boneTransform7 = animator.GetBoneTransform(HumanBodyBones.RightLowerLeg);
			Transform boneTransform8 = animator.GetBoneTransform(HumanBodyBones.RightFoot);
			Transform boneTransform9 = animator.GetBoneTransform(HumanBodyBones.RightToes);
			if (!boneTransform || !boneTransform2 || !boneTransform3 || !boneTransform4 || !boneTransform6 || !boneTransform7 || !boneTransform8)
			{
				DiagWait("missing humanoid leg bones");
				return false;
			}
			float num = KneeBendDegrees(boneTransform2, boneTransform3, boneTransform4);
			float num2 = KneeBendDegrees(boneTransform6, boneTransform7, boneTransform8);
			if (num < 3f || num2 < 3f)
			{
				DiagWait("legs straight - bend plane would be degenerate");
				return false;
			}
			_leftLeg = CreateLeg(boneTransform, boneTransform2, boneTransform3, boneTransform4, boneTransform5, AvatarIKGoal.LeftFoot);
			_rightLeg = CreateLeg(boneTransform, boneTransform6, boneTransform7, boneTransform8, boneTransform9, AvatarIKGoal.RightFoot);
			_grounder = base.gameObject.AddComponent<GrounderIK>();
			_grounder.legs = new IK[2] { _leftLeg, _rightLeg };
			_grounder.pelvis = boneTransform;
			_grounder.characterRoot = base.transform;
			_grounder.rootRotationWeight = 0f;
			ConfigureSolver(_grounder.solver);
			_grounder.weight = 0f;
			this.RefreshPhotonRpcCache();
			_hips = boneTransform;
			_initiated = true;
			_diagWaitLogged = false;
			return true;
		}

		private IK CreateLeg(Transform hips, Transform thigh, Transform calf, Transform foot, Transform toe, AvatarIKGoal goal)
		{
			if ((bool)toe)
			{
				LegIK legIK = base.gameObject.AddComponent<LegIK>();
				legIK.solver.SetChain(hips, thigh, calf, foot, toe, base.transform);
				return legIK;
			}
			LimbIK limbIK = base.gameObject.AddComponent<LimbIK>();
			limbIK.solver.SetChain(thigh, calf, foot, base.transform);
			limbIK.solver.goal = goal;
			limbIK.solver.bendModifier = IKSolverLimb.BendModifier.Animation;
			return limbIK;
		}

		private void ConfigureSolver(Grounding solver)
		{
			LayerMask layerMask = (_locomotion ? _locomotion.GroundLayerMask : ((LayerMask)(-1)));
			float t = (_config.DirectionalDescent ? ResolveDescentT() : 0f);
			solver.layers = (int)layerMask & ~(1 << base.gameObject.layer);
			solver.quality = _config.Quality;
			solver.maxStep = _config.MaxStep * Mathf.Lerp(1f, _config.DescendMaxStepScale, t);
			solver.footSpeed = _config.FootSpeed;
			solver.footRadius = _config.FootRadius;
			solver.prediction = _config.Prediction * Mathf.Lerp(1f, _config.DescendPredictionScale, t);
			solver.footRotationWeight = _config.FootRotationWeight * Mathf.Lerp(1f, _config.DescendFootRotationScale, t);
			solver.footRotationSpeed = _config.FootRotationSpeed;
			solver.maxFootRotationAngle = _config.MaxFootRotationAngle;
			solver.pelvisSpeed = Mathf.Lerp(_config.PelvisSpeed, _config.DescendPelvisSpeed, t);
			solver.pelvisDamper = Mathf.Lerp(_config.PelvisDamper, _config.DescendPelvisDamper, t) * PlatformDamperScale();
			solver.lowerPelvisWeight = _config.LowerPelvisWeight;
			solver.heightOffset = _config.HeightOffset + RenderLagOffset();
		}

		private float RenderLagOffset()
		{
			if (!_config.PlatformIKCompensation || !_rigidbody)
			{
				return 0f;
			}
			float num = Mathf.Max(0f, _config.PlatformIKCompensationMax);
			return Mathf.Clamp(base.transform.position.y - _rigidbody.position.y, 0f - num, num);
		}

		private float PlatformDamperScale()
		{
			if (!_config.PlatformIKCompensation || !_root)
			{
				return 1f;
			}
			float num = Mathf.Max(0.01f, _config.PlatformDamperCutoffSpeed);
			float num2 = Mathf.Max(Mathf.Abs(_root.PlatformCarry.y), SlopeRideSpeed());
			return 1f - Mathf.Clamp01(num2 / num);
		}

		private float SlopeRideSpeed()
		{
			GroundInfo ground = _root.Ground;
			if (!_rigidbody || !ground.Grounded)
			{
				return 0f;
			}
			Vector3 velocity = _rigidbody.velocity;
			Vector3 vector = Vector3.ProjectOnPlane(velocity, ground.Normal);
			return Mathf.Min(Mathf.Abs(velocity.y), Mathf.Abs(vector.y));
		}

		private float ResolveDescentT()
		{
			Vector3 normal = _root.Ground.Normal;
			if (normal.sqrMagnitude < 0.0001f)
			{
				return 0f;
			}
			Vector3 lhs = Vector3.ProjectOnPlane(base.transform.forward, Vector3.up);
			if (lhs.sqrMagnitude < 0.0001f)
			{
				return 0f;
			}
			lhs.Normalize();
			float num = Vector3.Dot(lhs, normal);
			float descendStartDot = _config.DescendStartDot;
			float descendFullDot = _config.DescendFullDot;
			if (descendFullDot - descendStartDot < 0.0001f)
			{
				if (!(num > descendStartDot))
				{
					return 0f;
				}
				return 1f;
			}
			return Mathf.Clamp01((num - descendStartDot) / (descendFullDot - descendStartDot));
		}

		private void ApplyLegTuning(IK leg, ref bool animatedBend)
		{
			if (leg is LegIK { solver: var solver })
			{
				solver.leg.bendToTargetWeight = _config.BendToTargetWeight;
				Transform transform = solver.thigh.transform;
				Transform transform2 = solver.calf.transform;
				Transform transform3 = solver.foot.transform;
				if (!_config.AnimatedBendNormal || !transform || !transform2 || !transform3)
				{
					animatedBend = false;
					solver.leg.useAnimatedBendNormal = false;
				}
				else
				{
					float num = KneeBendDegrees(transform, transform2, transform3);
					animatedBend = (animatedBend ? (num > 3f) : (num > 6f));
					solver.leg.useAnimatedBendNormal = animatedBend;
				}
			}
		}

		private void DriveWeight()
		{
			ConfigureSolver(_grounder.solver);
			ApplyLegTuning(_leftLeg, ref _leftAnimatedBend);
			ApplyLegTuning(_rightLeg, ref _rightAnimatedBend);
			float num = (ShouldGround() ? 1f : 0f);
			if ((bool)_root && _root.CurrentRootMotion.StopActive)
			{
				num = Mathf.Min(num, _config.StopGroundWeight);
			}
			_grounder.weight = Mathf.MoveTowards(_grounder.weight, num, _config.WeightLerpSpeed * Time.deltaTime);
			if (_grounder.weight <= 0f)
			{
				NeutralizeLegSolvers();
			}
		}

		private void DiagnoseSlopeRide()
		{
			if (_slopeRideDiagFrames < 400 && (bool)_rigidbody && (bool)_root && !(_grounder.weight <= 0.01f))
			{
				GroundInfo ground = _root.Ground;
				LocomotionStateSnapshot snapshot = _root.Snapshot;
				if (!(snapshot.MoveSpeed <= 0.3f) && !(ground.SlopeAngle < 1f))
				{
					float heightOffset = _grounder.solver.pelvis.heightOffset;
					float num = base.transform.position.y - _rigidbody.position.y;
					float num2 = (_hasSlopeRideDiagPrev ? (heightOffset - _diagPrevPelvis) : 0f);
					float num3 = (_hasSlopeRideDiagPrev ? (num - _diagPrevLag) : 0f);
					_diagPrevPelvis = heightOffset;
					_diagPrevLag = num;
					_hasSlopeRideDiagPrev = true;
					_slopeRideDiagFrames++;
					Debug.Log($"[SlopeRide] f={Time.frameCount} t={Time.time:F3} ft={Time.fixedTime:F3} " + $"vy={_rigidbody.velocity.y:F3} pelvis={heightOffset:F4} dPelvis={num2:F4} " + $"lag={num:F4} dLag={num3:F4} damperScale={PlatformDamperScale():F2} " + $"carryY={_root.PlatformCarry.y:F3} w={_grounder.weight:F2} " + $"slope={ground.SlopeAngle:F1} ms={snapshot.MoveSpeed:F2}");
				}
			}
		}

		private void LogRide()
		{
			Grounding.Leg leg = _grounder.solver.legs[0];
			Vector3 position = base.transform.position;
			float num = float.NaN;
			if (Physics.Raycast(position + Vector3.up * 0.5f, Vector3.down, out var hitInfo, 1f, _grounder.solver.layers, QueryTriggerInteraction.Ignore))
			{
				num = position.y - hitInfo.point.y;
			}
			Collider collider = _root.Ground.Collider;
			Debug.Log(string.Format("[FootIK/Ride] on={0} vy={1:F2} ", collider ? collider.name : "none", _rigidbody.velocity.y) + $"carryY={_root.PlatformCarry.y:F2} damperScale={PlatformDamperScale():F2} " + $"lag={position.y - _rigidbody.position.y:F4} skew={num:F4} " + $"pelvis={_grounder.solver.pelvis.heightOffset:F4} " + $"legHit={leg.isGrounded} w={_grounder.weight:F2}");
		}

		private bool ShouldGround()
		{
			if (!_root.IsActive)
			{
				return false;
			}
			GroundInfo ground = _root.Ground;
			if (!ground.Grounded || ground.IsSteepSlope || ground.SlopeAngle > _config.MaxSlopeForIK)
			{
				return false;
			}
			LocomotionStateSnapshot snapshot = _root.Snapshot;
			if (snapshot.IsGrounded)
			{
				return !snapshot.IsJumping;
			}
			return false;
		}

		private void Teardown()
		{
			SetSolversEnabled(enabled: false);
			if ((bool)_grounder)
			{
				_grounder.weight = 0f;
				Object.Destroy(_grounder);
			}
			if ((bool)_leftLeg)
			{
				Object.Destroy(_leftLeg);
			}
			if ((bool)_rightLeg)
			{
				Object.Destroy(_rightLeg);
			}
			this.RefreshPhotonRpcCacheAfterDestroy();
			_grounder = null;
			_leftLeg = null;
			_rightLeg = null;
			_hips = null;
			_initiated = false;
			_solversSuspended = true;
			_leftAnimatedBend = false;
			_rightAnimatedBend = false;
			_humanFrames = 0;
			_diagWaitLogged = false;
			_probeLogged = false;
		}

		private void DiagWait(string reason)
		{
		}
	}
}
