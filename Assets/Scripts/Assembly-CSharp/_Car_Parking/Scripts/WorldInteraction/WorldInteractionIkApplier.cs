using RootMotion.FinalIK;
using UnityEngine;

namespace _Car_Parking.Scripts.WorldInteraction
{
	public class WorldInteractionIkApplier
	{
		private struct ChainState
		{
			public float pull;

			public float reach;
		}

		private readonly FullBodyBipedIK _ik;

		private bool _owns;

		private float _target;

		private float _savedPullV;

		private float _savedPullH;

		private ChainState _armL;

		private ChainState _armR;

		private ChainState _legL;

		private ChainState _legR;

		private float _savedHandPoserL;

		private float _savedHandPoserR;

		private float _savedMapArmL;

		private float _savedMapArmR;

		private float _savedMapLegL;

		private float _savedMapLegR;

		private float _savedHeadMaintain;

		public bool IsActive => _owns;

		public WorldInteractionIkApplier(FullBodyBipedIK ik)
		{
			_ik = ik;
		}

		public void Apply(WorldPoseData pose, WorldPoseTargets t)
		{
			if ((bool)_ik)
			{
				_ik.enabled = true;
				IKSolverFullBodyBiped solver = _ik.solver;
				if (!_owns)
				{
					_savedPullV = solver.pullBodyVertical;
					_savedPullH = solver.pullBodyHorizontal;
					_armL = SaveChain(solver.leftArmChain);
					_armR = SaveChain(solver.rightArmChain);
					_legL = SaveChain(solver.leftLegChain);
					_legR = SaveChain(solver.rightLegChain);
					_savedHandPoserL = GetHandPoserWeight(solver.leftHandEffector);
					_savedHandPoserR = GetHandPoserWeight(solver.rightHandEffector);
					_savedMapArmL = GetLimbMap(solver, 0);
					_savedMapArmR = GetLimbMap(solver, 1);
					_savedMapLegL = GetLimbMap(solver, 2);
					_savedMapLegR = GetLimbMap(solver, 3);
					_savedHeadMaintain = GetHeadMaintain(solver);
				}
				solver.pullBodyVertical = 0f;
				solver.pullBodyHorizontal = 0f;
				solver.bodyEffector.target = t.Body;
				solver.bodyEffector.positionWeight = pose.BodyWeight;
				solver.bodyEffector.rotationWeight = pose.BodyWeight;
				SetEffector(solver.leftFootEffector, t.LeftFoot, pose.UseFeet ? pose.FootWeight : 0f, pose.UseFeet ? pose.FootRotationWeight : 0f);
				SetEffector(solver.rightFootEffector, t.RightFoot, pose.UseFeet ? pose.FootWeight : 0f, pose.UseFeet ? pose.FootRotationWeight : 0f);
				SetEffector(solver.leftShoulderEffector, t.LeftShoulder, pose.UseShoulders ? pose.ShoulderWeight : 0f, 0f);
				SetEffector(solver.rightShoulderEffector, t.RightShoulder, pose.UseShoulders ? pose.ShoulderWeight : 0f, 0f);
				SetEffector(solver.leftThighEffector, t.LeftThigh, pose.UseThighs ? pose.ThighWeight : 0f, 0f);
				SetEffector(solver.rightThighEffector, t.RightThigh, pose.UseThighs ? pose.ThighWeight : 0f, 0f);
				SetEffector(solver.leftHandEffector, t.LeftHand, pose.UseHands ? pose.HandWeight : 0f, pose.UseHands ? pose.HandRotationWeight : 0f);
				SetEffector(solver.rightHandEffector, t.RightHand, pose.UseHands ? pose.HandWeight : 0f, pose.UseHands ? pose.HandRotationWeight : 0f);
				SetBend(solver.leftArmChain, t.LeftElbow, pose.UseArmBend ? pose.ArmBendWeight : 0f);
				SetBend(solver.rightArmChain, t.RightElbow, pose.UseArmBend ? pose.ArmBendWeight : 0f);
				SetBend(solver.leftLegChain, t.LeftKnee, pose.UseLegBend ? pose.LegBendWeight : 0f);
				SetBend(solver.rightLegChain, t.RightKnee, pose.UseLegBend ? pose.LegBendWeight : 0f);
				SetLimbMap(solver, 0, pose.LeftArmMappingWeight);
				SetLimbMap(solver, 1, pose.RightArmMappingWeight);
				SetLimbMap(solver, 2, pose.LeftLegMappingWeight);
				SetLimbMap(solver, 3, pose.RightLegMappingWeight);
				SetHeadMaintain(solver, pose.MaintainHeadRot);
				DisableHandPoser(solver.leftHandEffector);
				DisableHandPoser(solver.rightHandEffector);
				_owns = true;
				_target = 1f;
			}
		}

		public void ReadBack(WorldPoseData pose)
		{
			if ((bool)_ik && _owns && pose != null)
			{
				IKSolverFullBodyBiped solver = _ik.solver;
				pose.BodyWeight = solver.bodyEffector.positionWeight;
				ReadGroup(ref pose.UseFeet, ref pose.FootWeight, ref pose.FootRotationWeight, solver.leftFootEffector, solver.rightFootEffector);
				ReadGroup(ref pose.UseHands, ref pose.HandWeight, ref pose.HandRotationWeight, solver.leftHandEffector, solver.rightHandEffector);
				float rotW = 0f;
				ReadGroup(ref pose.UseShoulders, ref pose.ShoulderWeight, ref rotW, solver.leftShoulderEffector, solver.rightShoulderEffector);
				rotW = 0f;
				ReadGroup(ref pose.UseThighs, ref pose.ThighWeight, ref rotW, solver.leftThighEffector, solver.rightThighEffector);
				ReadBendGroup(ref pose.UseArmBend, ref pose.ArmBendWeight, solver.leftArmChain, solver.rightArmChain);
				ReadBendGroup(ref pose.UseLegBend, ref pose.LegBendWeight, solver.leftLegChain, solver.rightLegChain);
				pose.LeftArmMappingWeight = GetLimbMap(solver, 0);
				pose.RightArmMappingWeight = GetLimbMap(solver, 1);
				pose.LeftLegMappingWeight = GetLimbMap(solver, 2);
				pose.RightLegMappingWeight = GetLimbMap(solver, 3);
				pose.MaintainHeadRot = GetHeadMaintain(solver);
			}
		}

		private static void ReadGroup(ref bool use, ref float posW, ref float rotW, IKEffector l, IKEffector r)
		{
			float num = Mathf.Max(l.positionWeight, r.positionWeight);
			if (use || !(num <= 0f))
			{
				use = true;
				posW = num;
				rotW = Mathf.Max(l.rotationWeight, r.rotationWeight);
			}
		}

		private static void ReadBendGroup(ref bool use, ref float w, FBIKChain l, FBIKChain r)
		{
			float num = Mathf.Max(l.bendConstraint.weight, r.bendConstraint.weight);
			if (use || !(num <= 0f))
			{
				use = true;
				w = num;
			}
		}

		public void Tick(float lerpSpeed)
		{
			if ((bool)_ik && _owns)
			{
				IKSolverFullBodyBiped solver = _ik.solver;
				solver.IKPositionWeight = Mathf.MoveTowards(solver.IKPositionWeight, _target, lerpSpeed * Time.deltaTime);
				if (_target <= 0f && solver.IKPositionWeight <= 0.001f)
				{
					ClearAndRelease();
				}
			}
		}

		public void BeginRelease()
		{
			_target = 0f;
		}

		public void ReleaseImmediate()
		{
			if ((bool)_ik && _owns)
			{
				ClearAndRelease();
				_ik.solver.IKPositionWeight = 0f;
				_target = 0f;
			}
		}

		public void HandOff()
		{
			if ((bool)_ik && _owns)
			{
				ClearAndRelease();
				_target = 0f;
			}
		}

		private void ClearAndRelease()
		{
			IKSolverFullBodyBiped solver = _ik.solver;
			ClearEffector(solver.bodyEffector);
			ClearEffector(solver.leftFootEffector);
			ClearEffector(solver.rightFootEffector);
			ClearEffector(solver.leftShoulderEffector);
			ClearEffector(solver.rightShoulderEffector);
			ClearEffector(solver.leftThighEffector);
			ClearEffector(solver.rightThighEffector);
			ClearEffector(solver.leftHandEffector);
			ClearEffector(solver.rightHandEffector);
			ClearBend(solver.leftArmChain);
			ClearBend(solver.rightArmChain);
			ClearBend(solver.leftLegChain);
			ClearBend(solver.rightLegChain);
			solver.pullBodyVertical = _savedPullV;
			solver.pullBodyHorizontal = _savedPullH;
			RestoreChain(solver.leftArmChain, _armL);
			RestoreChain(solver.rightArmChain, _armR);
			RestoreChain(solver.leftLegChain, _legL);
			RestoreChain(solver.rightLegChain, _legR);
			RestoreHandPoser(solver.leftHandEffector, _savedHandPoserL);
			RestoreHandPoser(solver.rightHandEffector, _savedHandPoserR);
			SetLimbMap(solver, 0, _savedMapArmL);
			SetLimbMap(solver, 1, _savedMapArmR);
			SetLimbMap(solver, 2, _savedMapLegL);
			SetLimbMap(solver, 3, _savedMapLegR);
			SetHeadMaintain(solver, _savedHeadMaintain);
			_owns = false;
		}

		private static void SetEffector(IKEffector e, Transform target, float posW, float rotW)
		{
			e.target = target;
			e.positionWeight = (target ? posW : 0f);
			e.rotationWeight = (target ? rotW : 0f);
		}

		private static void ClearEffector(IKEffector e)
		{
			e.target = null;
			e.positionWeight = 0f;
			e.rotationWeight = 0f;
		}

		private static void SetBend(FBIKChain chain, Transform goal, float weight)
		{
			chain.bendConstraint.bendGoal = goal;
			chain.bendConstraint.weight = (goal ? weight : 0f);
			chain.pull = 0f;
			chain.reach = 0f;
		}

		private static void ClearBend(FBIKChain chain)
		{
			chain.bendConstraint.bendGoal = null;
			chain.bendConstraint.weight = 0f;
		}

		private static ChainState SaveChain(FBIKChain chain)
		{
			return new ChainState
			{
				pull = chain.pull,
				reach = chain.reach
			};
		}

		private static void RestoreChain(FBIKChain chain, ChainState st)
		{
			chain.pull = st.pull;
			chain.reach = st.reach;
		}

		private static IKMappingLimb Limb(IKSolverFullBodyBiped s, int i)
		{
			if (s.limbMappings == null || i >= s.limbMappings.Length)
			{
				return null;
			}
			return s.limbMappings[i];
		}

		private static IKMappingBone HeadMap(IKSolverFullBodyBiped s)
		{
			if (s.boneMappings == null || s.boneMappings.Length == 0)
			{
				return null;
			}
			return s.boneMappings[0];
		}

		private static float GetLimbMap(IKSolverFullBodyBiped s, int i)
		{
			return Limb(s, i)?.weight ?? 1f;
		}

		private static void SetLimbMap(IKSolverFullBodyBiped s, int i, float w)
		{
			IKMappingLimb iKMappingLimb = Limb(s, i);
			if (iKMappingLimb != null)
			{
				iKMappingLimb.weight = w;
			}
		}

		private static float GetHeadMaintain(IKSolverFullBodyBiped s)
		{
			return HeadMap(s)?.maintainRotationWeight ?? 0f;
		}

		private static void SetHeadMaintain(IKSolverFullBodyBiped s, float w)
		{
			IKMappingBone iKMappingBone = HeadMap(s);
			if (iKMappingBone != null)
			{
				iKMappingBone.maintainRotationWeight = w;
			}
		}

		private static void DisableHandPoser(IKEffector hand)
		{
			if ((bool)hand?.bone)
			{
				HandPoser component = hand.bone.GetComponent<HandPoser>();
				if ((bool)component)
				{
					component.weight = 0f;
				}
			}
		}

		private static float GetHandPoserWeight(IKEffector hand)
		{
			if (!(hand?.bone))
			{
				return 0f;
			}
			HandPoser component = hand.bone.GetComponent<HandPoser>();
			if (!component)
			{
				return 0f;
			}
			return component.weight;
		}

		private static void RestoreHandPoser(IKEffector hand, float weight)
		{
			if ((bool)hand?.bone)
			{
				HandPoser component = hand.bone.GetComponent<HandPoser>();
				if ((bool)component)
				{
					component.weight = weight;
				}
			}
		}
	}
}
