using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public abstract class SeatSetupBase
	{
		protected CarAnimationsController _carAnimationsController;

		protected GameObject _car;

		protected SeatSetupBase(GameObject car)
		{
			_car = car;
			_carAnimationsController = car.GetComponent<CarAnimationsController>();
		}

		protected void TryCloseHoodAndTrunk()
		{
			if ((bool)_carAnimationsController && _carAnimationsController.OpenDoors.ContainsKey(CarDoorType.Trunk) && _carAnimationsController.OpenDoors[CarDoorType.Trunk])
			{
				_carAnimationsController.OpenCloseTrunk(isOpen: false, sendData: true).Forget();
			}
			if ((bool)_carAnimationsController && _carAnimationsController.OpenDoors.ContainsKey(CarDoorType.Hood) && _carAnimationsController.OpenDoors[CarDoorType.Hood])
			{
				_carAnimationsController.OpenCloseCarElement(CarDoorType.Hood, isOpen: false, isSendData: true);
			}
		}
	}
}
