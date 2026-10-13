using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Audio.Dummy
{
	[Serializable]
	public class EventDummy : BaseTypeDummy
	{
		public object WwiseObjectReference;

		private uint m_playingId = default;

		public uint PlayingId => m_playingId;

		public override object ObjectReference { get; set; }

		public override object WwiseObjectType { get; }

		private void VerifyPlayingID(uint playingId)
		{
		}

		public uint Post(GameObject gameObject)
		{
			return m_playingId;
		}

		public uint Post(GameObject gameObject, object flags, object callback, object cookie = null)
		{
			return m_playingId;
		}

		public uint Post(GameObject gameObject, uint flags, object callback, object cookie = null)
		{
			return m_playingId;
		}

		public uint Post(ulong gameObjectId)
		{
			return m_playingId;
		}

		public void Stop(GameObject gameObject, int transitionDuration = 0, object curveInterpolation = null)
		{
		}

		public void ExecuteAction(GameObject gameObject, object actionOnEventType, int transitionDuration, object curveInterpolation)
		{
		}

		public void PostMIDI(GameObject gameObject, object array)
		{
		}

		public void PostMIDI(GameObject gameObject, object array, int count)
		{
		}

		public void StopMIDI(GameObject gameObject)
		{
		}

		public void StopMIDI()
		{
		}
	}
}
