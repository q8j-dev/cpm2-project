using UnityEngine;

namespace Traffic
{
	public class IdleSMB : SceneLinkedSMB<Behaviours>
	{
		public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (Behaviours.SteamAlwaysWalk)
			{
				if (baseMono.MoveToRandomPosition())
				{
					animator.SetBool(baseMono.hashIsMovingParam, value: true);
				}
				return;
			}
			float value = Random.value;
			float num = 0f;
			if (num <= value && value < num + baseMono.startWalkProbability && baseMono.MoveToRandomPosition())
			{
				animator.SetBool(baseMono.hashIsMovingParam, value: true);
				return;
			}
			num += baseMono.startWalkProbability;
			if (num <= value && value < num + baseMono.startCheerProbability)
			{
				animator.SetBool(baseMono.hashIsCheeringParam, value: true);
				return;
			}
			num += baseMono.startCheerProbability;
			if (num <= value && value < num + baseMono.startAction1Probability)
			{
				animator.SetBool(baseMono.hashAction1Param, value: true);
				return;
			}
			num += baseMono.startAction1Probability;
			if (num <= value && value < num + baseMono.startAction2Probability)
			{
				animator.SetBool(baseMono.hashAction2Param, value: true);
			}
			else
			{
				num += baseMono.startAction2Probability;
			}
		}
	}
}
