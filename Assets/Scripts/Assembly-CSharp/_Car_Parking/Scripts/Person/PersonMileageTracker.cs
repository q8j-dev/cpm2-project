using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace _Car_Parking.Scripts.Person
{
	public class PersonMileageTracker : MonoBehaviour
	{
		public ObscuredFloat distanceTravelled;

		public ObscuredFloat mileagePassenger;

		public int skipDistanceTravel;

		private Vector2 _lastPosition;

		private void FixedUpdate()
		{
			if (!(base.gameObject != V.DriverPerson))
			{
				CalculateTravelledDistance();
			}
		}

		private void CalculateTravelledDistance()
		{
			if (!FlagsStorage.OutOfCar && !FlagsStorage.IsPassenger)
			{
				return;
			}
			if (skipDistanceTravel > 5)
			{
				skipDistanceTravel = 0;
				Vector2 vector = new Vector2(base.transform.localPosition.x, base.transform.localPosition.z);
				float num = Vector2.Distance(vector, _lastPosition) * 0.001f;
				if (FlagsStorage.OutOfCar)
				{
					if (num < 0.05f)
					{
						distanceTravelled = (float)distanceTravelled + num;
					}
				}
				else if (num < 60f)
				{
					mileagePassenger = (float)mileagePassenger + num;
				}
				_lastPosition = vector;
			}
			else
			{
				skipDistanceTravel++;
			}
		}
	}
}
