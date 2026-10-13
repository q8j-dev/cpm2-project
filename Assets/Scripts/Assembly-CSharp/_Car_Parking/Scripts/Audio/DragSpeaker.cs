using UnityEngine;

namespace _Car_Parking.Scripts.Audio
{
	public class DragSpeaker : MonoBehaviour
	{
		[SerializeField]
		private SoundEventChannel SoundEventChannel = default;

		private bool _isStartEvent;

		private void OnEnable()
		{
			SoundEventChannel.OnDragSpeakerEvent += OnDragSpeakerEvent;
		}

		private void OnDisable()
		{
			SoundEventChannel.OnDragSpeakerEvent -= OnDragSpeakerEvent;
		}

		private void OnDragSpeakerEvent(string @switch)
		{
			AudioWrapper.SetSwitch("PA_Speaker_Phrases", @switch, base.gameObject);
			if (!_isStartEvent)
			{
				AudioWrapper.PostEvent("Play_PA_Speaker_Drag", base.gameObject);
				_isStartEvent = true;
			}
		}

		private void OnDestroy()
		{
			AudioWrapper.StopAll(base.gameObject);
		}
	}
}
