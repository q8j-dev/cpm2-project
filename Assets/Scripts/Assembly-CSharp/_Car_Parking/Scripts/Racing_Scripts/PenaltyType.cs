using System;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[Serializable]
	public enum PenaltyType
	{
		OffRoad = 0,
		MissedCheckpoint = 1,
		MarshalCollision = 2,
		CutMarker = 3,
		PeopleCollision = 4,
		ReturnToRoad = 5,
		Other = 6
	}
}
