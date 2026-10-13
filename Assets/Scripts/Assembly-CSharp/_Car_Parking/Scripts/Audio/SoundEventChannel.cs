using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Audio
{
	[CreateAssetMenu(fileName = "SoundEventChannel", menuName = "Scriptable/EventChannels/SoundEventChannel")]
	public class SoundEventChannel : ScriptableObject
	{
		public event Action OnFinish;

		public event Action<string> OnDragSpeakerEvent;

		public void InvokeOnFinishSound()
		{
			OnFinish?.Invoke();
		}

		public void InvokeDragSpeakerEvent(string @event)
		{
			OnDragSpeakerEvent?.Invoke(@event);
		}
	}
}
