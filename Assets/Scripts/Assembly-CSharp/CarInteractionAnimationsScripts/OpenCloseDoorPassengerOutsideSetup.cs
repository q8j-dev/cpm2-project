using CarAnimationScripts;
using UnityEngine;
using _Car_Parking.Scripts.Providers;

namespace CarInteractionAnimationsScripts
{
	public class OpenCloseDoorPassengerOutsideSetup : ISetupHandler
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private CarAnimationsController _carAnimationsController;

		private VehicleData _vehicleData;

		private LevelsProvider _levelsProvider;

		private GameObject _person;

		private GameObject _car;

		private int _place;

		private CarAnimationType _carAnimationType;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public OpenCloseDoorPassengerOutsideSetup(PersonAnimationsController personAnimationsController, GameObject car, SitGetOutData sitGetOutData, int place, CarAnimationType carAnimationType)
		{
			_personAnimationsController = personAnimationsController;
			_sitGetOutData = sitGetOutData;
			_car = car;
			_place = place;
			_carAnimationType = carAnimationType;
			_carAnimationsController = car.GetComponent<CarAnimationsController>();
			_vehicleData = car.GetComponent<VehicleData>();
			_person = personAnimationsController.transform.parent.gameObject;
			_levelsProvider = _vehicleData.LevelsProvider;
		}

		public void Enter()
		{
			IsAnimate = (bool)_carAnimationsController && _place < _carAnimationsController.CarAnimationReferences.CountSeats && _place > -1;
			IsBusy = _place == -1 || (_carAnimationsController.VehicleData.CarLocked && !_vehicleData.IsMineCar);
			if (_carAnimationsController.VehicleData.CarLocked)
			{
				if ((bool)V.CoreGameManager.CarOutInControl)
				{
					V.CoreGameManager.CarOutInControl.ShowBusyMessage(1);
				}
			}
			else if (IsAnimate && !IsBusy)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _place, _person);
				_personAnimationsController.SitGetOutCarSystem.MultiPlayerSendRpcCarAnimations.SendOpenCloseCarDoor(_person, _car, _place, _carAnimationType);
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StartTimerForCancelOpenCloseDoor();
				_sitGetOutData.SetCarOpenCloseDoorOutsideStartData(_carAnimationsController, _place);
				_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: false);
				_personAnimationsController.transform.parent.GetComponent<NewMoveController>().enabled = false;
			}
		}

		public void Exit()
		{
			if (!IsBusy && IsAnimate)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _place, null);
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StopTimerCancelOpenCloseDoor();
				_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: true);
				_sitGetOutData.SetCarOpenCloseDoorOutsideExitData(isOpenClose: true);
				_personAnimationsController.StopAnimation();
				_personAnimationsController.transform.parent.GetComponent<NewMoveController>().enabled = true;
			}
		}
	}
}
