using UnityEngine;

namespace Traffic
{
	public class MoveSMB : SceneLinkedSMB<Behaviours>
	{
		public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (!baseMono.SteamDodging)
			{
				baseMono.SteamResolveHeadOn();
				if (baseMono.IsTargetReached && (!Behaviours.SteamAlwaysWalk || !baseMono.MoveToRandomPosition()))
				{
					animator.SetBool(baseMono.hashIsMovingParam, value: false);
				}
			}
		}
	}
}
