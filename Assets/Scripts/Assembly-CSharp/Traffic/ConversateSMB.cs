using UnityEngine;

namespace Traffic
{
	public class ConversateSMB : SceneLinkedSMB<Behaviours>
	{
		public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			baseMono.isInConversation = true;
			baseMono.IsAgentStopped = true;
		}

		public override void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			baseMono.IsAgentStopped = false;
			baseMono.isInConversation = false;
		}
	}
}
