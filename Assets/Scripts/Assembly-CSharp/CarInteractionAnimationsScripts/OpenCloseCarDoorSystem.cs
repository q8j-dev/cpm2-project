using System.Collections.Generic;
using CarAnimationScripts;
using UnityEngine;
using _Car_Parking;

namespace CarInteractionAnimationsScripts
{
	public class OpenCloseCarDoorSystem
	{
		private PersonAnimationsController _personAnimationsController;

		private GameObject _person;

		public OpenCloseCarDoorSystem(PersonAnimationsController personAnimationsController)
		{
			_personAnimationsController = personAnimationsController;
			_person = _personAnimationsController.transform.parent.gameObject;
		}

		public (List<IAction>, ISetupHandler) GetActionsOpenCloseDoor(GameObject car, bool isDriverDoor)
		{
			CarAnimationsController component = car.GetComponent<CarAnimationsController>();
			List<IAction> list = null;
			ISetupHandler setupHandler = null;
			bool outOfCar = FlagsStorage.OutOfCar;
			if (isDriverDoor)
			{
				if (outOfCar)
				{
					CarAnimationType carAnimationType = (component.OpenDoors[CarDoorType.DriverDoor] ? CarAnimationType.CloseOutside : CarAnimationType.OpenOutside);
					setupHandler = new OpenCloseDriverDoorOutsideSetup(_personAnimationsController, car, _personAnimationsController.SitGetOutCarSystem.SitGetOutData, carAnimationType);
					list = new List<IAction>
					{
						new MoveToDoorPointAction(_personAnimationsController, isOnlyOpenCloseDoor: true),
						new RotateToDoorAction(_personAnimationsController, isOnlyOpenCloseDoor: true),
						new PlayOpenCloseDoorAnimation(_personAnimationsController, carAnimationType)
					};
				}
				else
				{
					CarAnimationType carAnimationType2 = (component.OpenDoors[CarDoorType.DriverDoor] ? CarAnimationType.CloseInside : CarAnimationType.OpenInside);
					setupHandler = new OpenCloseDriverDoorInsideSetup(_personAnimationsController, car, _personAnimationsController.SitGetOutCarSystem.SitGetOutData, carAnimationType2);
					list = new List<IAction>
					{
						new PlayOpenCloseDoorAnimation(_personAnimationsController, carAnimationType2)
					};
				}
			}
			else if (outOfCar)
			{
				int doorPlaceForPassenger = _personAnimationsController.SitGetOutCarSystem.GetDoorPlaceForPassenger(car);
				bool num = doorPlaceForPassenger < component.CarAnimationReferences.CountSeats && doorPlaceForPassenger > -1;
				bool flag = false;
				if (num)
				{
					flag = component.OpenDoors[(CarDoorType)doorPlaceForPassenger];
				}
				CarAnimationType carAnimationType3 = (flag ? CarAnimationType.CloseOutside : CarAnimationType.OpenOutside);
				setupHandler = new OpenCloseDoorPassengerOutsideSetup(_personAnimationsController, car, _personAnimationsController.SitGetOutCarSystem.SitGetOutData, doorPlaceForPassenger, carAnimationType3);
				list = new List<IAction>
				{
					new MoveToDoorPointAction(_personAnimationsController, isOnlyOpenCloseDoor: true),
					new RotateToDoorAction(_personAnimationsController, isOnlyOpenCloseDoor: true),
					new PlayOpenCloseDoorAnimation(_personAnimationsController, carAnimationType3)
				};
			}
			else
			{
				bool num2 = _personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentPlace < component.CarAnimationReferences.CountSeats;
				CarAnimationType carAnimationType4 = CarAnimationType.None;
				if (num2)
				{
					carAnimationType4 = (component.OpenDoors[(CarDoorType)_personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentPlace] ? CarAnimationType.CloseInside : CarAnimationType.OpenInside);
				}
				setupHandler = new OpenCloseDoorPassengerInsideSetup(_personAnimationsController, car, _personAnimationsController.SitGetOutCarSystem.SitGetOutData, carAnimationType4);
				list = new List<IAction>
				{
					new PlayOpenCloseDoorAnimation(_personAnimationsController, carAnimationType4)
				};
			}
			return (list, setupHandler);
		}

		public (List<IAction>, ISetupHandler) GetActionsOpenCloseDoorOtherPlayer(GameObject car, int place, CarAnimationType carAnimationType)
		{
			CarAnimationsController component = car.GetComponent<CarAnimationsController>();
			List<IAction> list = null;
			ISetupHandler setupHandler = null;
			if (carAnimationType == CarAnimationType.CloseOutside || carAnimationType == CarAnimationType.OpenOutside)
			{
				setupHandler = new OpenCloseDoorOutsideOtherPlayerSetup(_personAnimationsController, car, _personAnimationsController.SitGetOutCarSystem.SitGetOutData, place, carAnimationType);
				list = new List<IAction>
				{
					new MoveToDoorPointAction(_personAnimationsController, isOnlyOpenCloseDoor: true),
					new RotateToDoorAction(_personAnimationsController, isOnlyOpenCloseDoor: true),
					new PlayOpenCloseDoorAnimation(_personAnimationsController, carAnimationType)
				};
			}
			else
			{
				setupHandler = new OpenCloseDoorInsideOtherPlayerSetup(component, _personAnimationsController, _personAnimationsController.SitGetOutCarSystem.SitGetOutData, place);
				list = new List<IAction>
				{
					new PlayOpenCloseDoorAnimation(_personAnimationsController, carAnimationType)
				};
			}
			return (list, setupHandler);
		}
	}
}
