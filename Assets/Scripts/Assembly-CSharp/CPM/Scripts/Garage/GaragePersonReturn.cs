using CarAnimationScripts;
using UnityEngine;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace CPM.Scripts.Garage
{
	public static class GaragePersonReturn
	{
		private const float PersonSideOffset = 2f;

		private const float PersonHeightOffset = -0.2f;

		public static void PlaceAtLastCarDriverDoor(GameObject person)
		{
			if ((bool)person && (bool)V.LastUsedCar)
			{
				ArenaLocalPerson.PlaceAt(ResolveDriverDoorPosition(), person.transform.rotation, updateCamera: false);
			}
		}

		private static Vector3 ResolveDriverDoorPosition()
		{
			if (TryGetDriverDoorPoint(out var doorPoint))
			{
				return doorPoint.position;
			}
			Transform transform = V.LastUsedCar.transform;
			VehicleData component = V.LastUsedCar.GetComponent<VehicleData>();
			float num = (((bool)component && component.RightHanded) ? 2f : (-2f));
			return transform.position + transform.right * num + new Vector3(0f, -0.2f, 0f);
		}

		private static bool TryGetDriverDoorPoint(out Transform doorPoint)
		{
			doorPoint = null;
			if (!V.LastUsedCar.TryGetComponent<CarAnimationsController>(out var component) || !component.CarAnimationReferences)
			{
				return false;
			}
			CarAnimationsPoints carAnimationsPoints = component.CarAnimationsPoints;
			if (!carAnimationsPoints || carAnimationsPoints.DoorsPointsToMove == null || carAnimationsPoints.DoorsPointsToMove.Count <= 0)
			{
				return false;
			}
			doorPoint = carAnimationsPoints.DoorsPointsToMove[0];
			return doorPoint;
		}
	}
}
