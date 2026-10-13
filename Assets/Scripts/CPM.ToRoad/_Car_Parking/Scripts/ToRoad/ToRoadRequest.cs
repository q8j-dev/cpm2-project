using UnityEngine;

namespace _Car_Parking.Scripts.ToRoad
{
	public readonly struct ToRoadRequest
	{
		public readonly GameObject Person;

		public readonly GameObject Car;

		public readonly ToRoadSource Source;

		public bool MovesPerson => !Car;

		public ToRoadRequest(GameObject person, GameObject car, ToRoadSource source)
		{
			Person = person;
			Car = car;
			Source = source;
		}
	}
}
