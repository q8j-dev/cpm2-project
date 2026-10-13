using System;
using _Car_Parking.Scripts.ArtistTools.Aspects;

namespace _Car_Parking.Scripts.ArtistTools.Trackables
{
	public abstract class TrackableWithAspects : TrackableWithAspectsBase
	{
		public event Action<TrackableWithAspects> OnDestroyEvent;

		protected sealed override void OnDestroy()
		{
			OnDestroyEvent?.Invoke(this);
		}

		public bool HasAspect<T>(out T aspect) where T : IAspect
		{
			aspect = default;
			if (!(this is T val))
			{
				return false;
			}
			aspect = val;
			return true;
		}
	}
}
