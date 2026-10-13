using UnityEngine;

namespace Traffic
{
	public class CheerSMB : SceneLinkedSMB<Behaviours>
	{
		public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetBool(baseMono.hashIsCheeringParam, value: false);
		}
	}
}
