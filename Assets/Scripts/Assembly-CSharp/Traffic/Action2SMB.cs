using UnityEngine;

namespace Traffic
{
	public class Action2SMB : SceneLinkedSMB<Behaviours>
	{
		public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			animator.SetBool(baseMono.hashAction2Param, value: false);
		}
	}
}
