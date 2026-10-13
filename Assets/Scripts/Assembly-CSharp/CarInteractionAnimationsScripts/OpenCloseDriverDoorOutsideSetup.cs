using CarAnimationScripts;
using UnityEngine;
using _Car_Parking.Scripts.Providers;

namespace CarInteractionAnimationsScripts
{
	public class OpenCloseDriverDoorOutsideSetup : ISetupHandler
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private CarAnimationsController _carAnimationsController;

		private VehicleData _vehicleData;

		private LevelsProvider _levelsProvider;

		private GameObject _person;

		private GameObject _car;

		private CarAnimationType _carAnimationType;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public OpenCloseDriverDoorOutsideSetup(PersonAnimationsController personAnimationsController, GameObject car, SitGetOutData sitGetOutData, CarAnimationType carAnimationType)
		{
			_personAnimationsController = personAnimationsController;
			_sitGetOutData = sitGetOutData;
			_carAnimationType = carAnimationType;
			_car = car;
			_carAnimationsController = car.GetComponent<CarAnimationsController>();
			_vehicleData = car.GetComponent<VehicleData>();
			_person = personAnimationsController.transform.parent.gameObject;
			_levelsProvider = _vehicleData.LevelsProvider;
		}

		public void Enter()
		{
			IsAnimate = _carAnimationsController;
			IsBusy = _vehicleData.CarLocked && !_vehicleData.IsMineCar;
			if (IsBusy)
			{
				V.CoreGameManager.CarOutInControl.ShowBusyMessage(1);
			}
			else if (IsAnimate && !IsBusy)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, 0, _person);
				_personAnimationsController.SitGetOutCarSystem.MultiPlayerSendRpcCarAnimations.SendOpenCloseCarDoor(_person, _car, 0, _carAnimationType);
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StartTimerForCancelOpenCloseDoor();
				_sitGetOutData.SetCarOpenCloseDoorOutsideStartData(_carAnimationsController, 0);
				_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: false);
				_personAnimationsController.transform.parent.GetComponent<NewMoveController>().enabled = false;
			}
		}

		public void Exit()
		{
			if (!IsBusy && (bool)_person && IsAnimate)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, 0, null);
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StopTimerCancelOpenCloseDoor();
				_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: true);
				_sitGetOutData.SetCarOpenCloseDoorOutsideExitData(isOpenClose: true);
				_personAnimationsController.StopAnimation();
				_personAnimationsController.transform.parent.GetComponent<NewMoveController>().enabled = true;
			}
		}
	}
}
