using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Audio
{
	public class TribuneSoundPlayer : MonoBehaviour
	{
		[SerializeField]
		private SoundEventChannel SoundEventChannel;

		private RacingProvider _racingProvider;

		[Inject]
		public void Construct(RacingProvider racingProvider)
		{
			_racingProvider = racingProvider;
		}

		private void OnEnable()
		{
			SoundEventChannel.OnFinish += OnFinish;
		}

		private void OnDisable()
		{
			SoundEventChannel.OnFinish -= OnFinish;
		}

		private void OnFinish()
		{
			if (!_racingProvider.DragRacingController.qualification && !_racingProvider.DragRacingController.testRun)
			{
				Debug.Log("ON FINISH EVENT -> Play_Crowd_Reaction_To_Finish");
				AudioWrapper.PostEvent("Play_Crowd_Reaction_To_Finish", base.gameObject);
			}
		}
	}
}
