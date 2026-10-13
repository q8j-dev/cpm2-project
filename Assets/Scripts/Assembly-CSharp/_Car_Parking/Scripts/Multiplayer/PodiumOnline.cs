using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer
{
	public class PodiumOnline : MonoBehaviour
	{
		[SerializeField]
		private Transform EntryTarget;

		[Tooltip("Authored stands, best place first: element 0 is 1st place. Places past the last element go to OtherPlaces.")]
		[SerializeField]
		private Transform[] FirstPlaces;

		[SerializeField]
		[Tooltip("Layout the remaining places are spread over, starting right after the last stand.")]
		private SpawnLayoutProvider OtherPlaces;

		[SerializeField]
		[Tooltip("The camera takes this pose once the run ends and holds it. Empty: the camera keeps following the player.")]
		private Transform CameraPosition;

		public Transform CameraMount => CameraPosition;

		public PositionsAndRotation Entry => PositionsAndRotation.Of(EntryTarget ? EntryTarget : base.transform);

		public PositionsAndRotation GetPlacePose(int place, int participantCount)
		{
			if (place < 1)
			{
				return Entry;
			}
			Transform[] firstPlaces = FirstPlaces;
			int num = ((firstPlaces != null) ? firstPlaces.Length : 0);
			if (place <= num)
			{
				return PositionsAndRotation.Of(FirstPlaces[place - 1]) ?? Entry;
			}
			if (!OtherPlaces)
			{
				return Entry;
			}
			return OtherPlaces.GetPoint(place - num - 1, participantCount - num) ?? Entry;
		}
	}
}
