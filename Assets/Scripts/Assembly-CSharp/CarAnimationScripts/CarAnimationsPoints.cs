using System.Collections.Generic;
using CarInteractionAnimationsScripts;
using UnityEngine;

namespace CarAnimationScripts
{
	public class CarAnimationsPoints : MonoBehaviour
	{
		public List<Transform> SeatPoints;

		public List<Transform> DoorsPoints;

		public List<Transform> OpenCloseDoorsPoints;

		public Dictionary<CarDoorType, Transform> SeatPointsDictionary { get; private set; }

		public List<Transform> SeatPointsToSit { get; private set; }

		public List<Transform> DoorsPointsToMove { get; private set; }

		public List<Transform> OpenCloseDoorsPointsToMove { get; private set; }

		public void InitPoints(bool isRightHanded)
		{
			Transform transform = ((SeatPoints.Count > 0) ? SeatPoints[0] : null);
			Transform transform2 = ((SeatPoints.Count > 1) ? SeatPoints[1] : null);
			Transform transform3 = ((SeatPoints.Count > 2) ? SeatPoints[2] : null);
			Transform transform4 = ((SeatPoints.Count > 3) ? SeatPoints[3] : null);
			SeatPointsDictionary = new Dictionary<CarDoorType, Transform>
			{
				{
					CarDoorType.DriverDoor,
					isRightHanded ? transform2 : transform
				},
				{
					CarDoorType.PassengerFrontDoor,
					isRightHanded ? transform : transform2
				},
				{
					CarDoorType.PassengerBackDoor1,
					isRightHanded ? transform4 : transform3
				},
				{
					CarDoorType.PassengerBackDoor2,
					isRightHanded ? transform3 : transform4
				}
			};
			SeatPointsToSit = new List<Transform>(SeatPoints);
			DoorsPointsToMove = new List<Transform>(DoorsPoints);
			OpenCloseDoorsPointsToMove = new List<Transform>(OpenCloseDoorsPoints);
			if (isRightHanded)
			{
				SetupPoints(SeatPointsToSit);
				SetupPoints(DoorsPointsToMove);
				SetupPoints(OpenCloseDoorsPointsToMove);
			}
		}

		public bool TryGetSeatPoint(CarDoorType doorType, out Transform seatPoint)
		{
			seatPoint = null;
			if (SeatPointsDictionary != null)
			{
				SeatPointsDictionary.TryGetValue(doorType, out seatPoint);
			}
			return seatPoint != null;
		}

		private void SetupPoints(List<Transform> points)
		{
			List<Transform> list = new List<Transform>(points);
			for (int i = 0; i < list.Count; i += 2)
			{
				Transform value = list[i];
				Transform value2 = list[i + 1];
				points[i] = value2;
				points[i + 1] = value;
			}
		}
	}
}
