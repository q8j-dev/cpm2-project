using _Car_Parking.Scripts.ArtistTools.Trackables;

namespace _Car_Parking.Scripts.ArtistTools.Aspects
{
	public interface IDestroyAspect : IAspect
	{
		void OnDestroyImpl(TrackableWithAspects trackable);
	}
}
