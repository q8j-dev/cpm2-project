using CPM.Scripts.Garage;
using UnityEngine;

namespace _Car_Parking.Scripts.ToRoad.Handlers
{
	public sealed class GarageToRoadHandler : IToRoadHandler
	{
		private const float UnderGroundThreshold = 10f;

		private readonly GarageController _garageController;

		public ToRoadPriority Priority => ToRoadPriority.Context;

		public GarageToRoadHandler(GarageController garageController)
		{
			_garageController = garageController;
		}

		public bool TryResolve(in ToRoadRequest request, out ToRoadResult result)
		{
			result = ToRoadResult.NotHandled();
			if (!FlagsStorage.InGarageFree)
			{
				return false;
			}
			if (request.MovesPerson)
			{
				PlacePersonNextToCar(request.Person);
			}
			else
			{
				_garageController.ResetCarPosition();
			}
			result = ToRoadResult.Handled();
			return true;
		}

		private void PlacePersonNextToCar(GameObject person)
		{
			if (person.transform.position.y < V.GlobalOffset.y - 10f)
			{
				_garageController.ResetCarToSavedPosition();
			}
			GaragePersonReturn.PlaceAtLastCarDriverDoor(person);
		}

		bool IToRoadHandler.TryResolve(in ToRoadRequest request, out ToRoadResult result)
		{
			return TryResolve(in request, out result);
		}
	}
}
