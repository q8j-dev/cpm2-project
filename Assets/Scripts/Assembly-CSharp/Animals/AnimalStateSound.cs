using UnityEngine;

namespace Animals
{
	public class AnimalStateSound : StateMachineBehaviour
	{
		[SerializeField]
		private string PlayEvent = default;

		[SerializeField]
		private string StopEvent = default;

		private int _lastCycle;

		private bool IsContinuous => !string.IsNullOrEmpty(StopEvent);

		public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			_lastCycle = Mathf.FloorToInt(stateInfo.normalizedTime);
			if (ShouldPlayOnEnter(stateInfo.normalizedTime, IsContinuous))
			{
				Post(PlayEvent, animator);
			}
		}

		public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (stateInfo.loop && !IsContinuous && !animator.IsInTransition(layerIndex) && CrossedCycle(stateInfo.normalizedTime, ref _lastCycle))
			{
				Post(PlayEvent, animator);
			}
		}

		public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
		{
			if (IsContinuous)
			{
				Post(StopEvent, animator);
			}
		}

		public static bool ShouldPlayOnEnter(float normalizedTime, bool continuous)
		{
			if (!continuous)
			{
				return normalizedTime <= 0.01f;
			}
			return true;
		}

		public static bool CrossedCycle(float normalizedTime, ref int lastCycle)
		{
			int num = Mathf.FloorToInt(normalizedTime);
			if (num <= lastCycle)
			{
				return false;
			}
			lastCycle = num;
			return true;
		}

		private static void Post(string eventName, Animator animator)
		{
			if (!string.IsNullOrEmpty(eventName))
			{
				AkSoundEngine.PostEvent(eventName, animator.gameObject);
			}
		}
	}
}
