using UnityEngine;

namespace Traffic
{
	public class Action1SMB : SceneLinkedSMB<Behaviours>
	{
		public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetBool(baseMono.hashAction1Param, value: false);
		}
	}
}
