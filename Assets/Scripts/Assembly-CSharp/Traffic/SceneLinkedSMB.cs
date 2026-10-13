using UnityEngine;
using UnityEngine.Animations;

namespace Traffic
{
	public class SceneLinkedSMB<TMonoBehaviour> : SealedSMB where TMonoBehaviour : MonoBehaviour
	{
		protected TMonoBehaviour baseMono;

		private bool _firstFrameHappened;

		private bool _lastFrameHappened;

		public static void Initialise(Animator animator, TMonoBehaviour monoBehaviour)
		{
			SceneLinkedSMB<TMonoBehaviour>[] behaviours = animator.GetBehaviours<SceneLinkedSMB<TMonoBehaviour>>();
			for (int i = 0; i < behaviours.Length; i++)
			{
				behaviours[i].InternalInitialise(animator, monoBehaviour);
			}
		}

		protected void InternalInitialise(Animator animator, TMonoBehaviour monoBehaviour)
		{
			baseMono = monoBehaviour;
			OnStart(animator);
		}

		public sealed override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			_firstFrameHappened = false;
			OnSLStateEnter(animator, stateInfo, layerIndex);
			OnSLStateEnter(animator, stateInfo, layerIndex, controller);
		}

		public sealed override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			if (animator.gameObject.activeSelf && (bool)baseMono)
			{
				if (animator.IsInTransition(layerIndex) && animator.GetNextAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
				{
					OnSLTransitionToStateUpdate(animator, stateInfo, layerIndex);
					OnSLTransitionToStateUpdate(animator, stateInfo, layerIndex, controller);
				}
				if (!animator.IsInTransition(layerIndex) && _firstFrameHappened)
				{
					OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex);
					OnSLStateNoTransitionUpdate(animator, stateInfo, layerIndex, controller);
				}
				if (animator.IsInTransition(layerIndex) && !_lastFrameHappened && _firstFrameHappened)
				{
					_lastFrameHappened = true;
					OnSLStatePreExit(animator, stateInfo, layerIndex);
					OnSLStatePreExit(animator, stateInfo, layerIndex, controller);
				}
				if (!animator.IsInTransition(layerIndex) && !_firstFrameHappened)
				{
					_firstFrameHappened = true;
					OnSLStatePostEnter(animator, stateInfo, layerIndex);
					OnSLStatePostEnter(animator, stateInfo, layerIndex, controller);
				}
				if (animator.IsInTransition(layerIndex) && animator.GetCurrentAnimatorStateInfo(layerIndex).fullPathHash == stateInfo.fullPathHash)
				{
					OnSLTransitionFromStateUpdate(animator, stateInfo, layerIndex);
					OnSLTransitionFromStateUpdate(animator, stateInfo, layerIndex, controller);
				}
			}
		}

		public sealed override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
			_lastFrameHappened = false;
			OnSLStateExit(animator, stateInfo, layerIndex);
			OnSLStateExit(animator, stateInfo, layerIndex, controller);
		}

		public virtual void OnStart(Animator animator)
		{
		}

		public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

		public virtual void OnSLTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

		public virtual void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

		public virtual void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

		public virtual void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

		public virtual void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

		public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
		}

		public virtual void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
		}

		public virtual void OnSLTransitionToStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
		}

		public virtual void OnSLStatePostEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
		}

		public virtual void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
		}

		public virtual void OnSLStatePreExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
		}

		public virtual void OnSLTransitionFromStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
		}

		public virtual void OnSLStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex, AnimatorControllerPlayable controller)
		{
		}
	}
}
