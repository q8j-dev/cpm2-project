using System;
using CarAnimationScripts;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Providers;

namespace CarInteractionAnimationsScripts
{
	public class OpenCloseDoorPassengerInsideSetup : ISetupHandler
	{
		private const float DelayForShowDoorButton = 1f;

		private PersonAnimationsController _personAnimationsController;

		private CarAnimationsController _carAnimationsController;

		private VehicleData _vehicleData;

		private SitGetOutData _sitGetOutData;

		private LevelsProvider _levelsProvider;

		private GameObject _person;

		private GameObject _car;

		private CarAnimationType _carAnimationType;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public OpenCloseDoorPassengerInsideSetup(PersonAnimationsController personAnimationsController, GameObject car, SitGetOutData sitGetOutData, CarAnimationType carAnimationType)
		{
			_personAnimationsController = personAnimationsController;
			_car = car;
			_sitGetOutData = sitGetOutData;
			_carAnimationType = carAnimationType;
			_carAnimationsController = car.GetComponent<CarAnimationsController>();
			_person = personAnimationsController.transform.parent.gameObject;
			_vehicleData = car.GetComponent<VehicleData>();
			_levelsProvider = _vehicleData.LevelsProvider;
		}

		public void Enter()
		{
			IsAnimate = (bool)_carAnimationsController && _sitGetOutData.CurrentPlace < _carAnimationsController.CarAnimationReferences.CountSeats;
			IsBusy = _carAnimationsController.VehicleData.CarLocked && !_vehicleData.IsMineCar;
			if (IsBusy)
			{
				if ((bool)V.CoreGameManager.CarOutInControl)
				{
					V.CoreGameManager.CarOutInControl.ShowBusyMessage(1);
				}
			}
			else if (IsAnimate)
			{
				if ((bool)_vehicleData.FreeDriveProvider.QuickCarSettings)
				{
					_vehicleData.FreeDriveProvider.QuickCarSettings.HideDoor();
					_sitGetOutData.DisableDriverButtonInCar = true;
				}
				_personAnimationsController.SitGetOutCarSystem.MultiPlayerSendRpcCarAnimations.SendOpenCloseCarDoor(_person, _car, _sitGetOutData.CurrentPlace, _carAnimationType);
				_person.transform.localScale = _personAnimationsController.ScalePersonOutCar;
				_sitGetOutData.SetCarInsideStartData(_carAnimationsController, _sitGetOutData.CurrentPlace, isOnlyOpenCloseDoor: true);
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _sitGetOutData.CurrentPlace, _person);
				_person.GetComponent<NewMoveController>().enabled = false;
				_personAnimationsController.Animator.enabled = true;
			}
		}

		public void Exit()
		{
			if (IsBusy || !IsAnimate)
			{
				return;
			}
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				_sitGetOutData.DisableDriverButtonInCar = false;
				if ((bool)_vehicleData && (bool)_vehicleData.FreeDriveProvider.QuickCarSettings)
				{
					_vehicleData.FreeDriveProvider.QuickCarSettings.ShowDoor();
				}
			}).AddTo(_personAnimationsController);
			CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _sitGetOutData.CurrentPlace, null);
			_person.transform.localScale = CarAnimationReferences.GetSizePlayerInCarSafe(_carAnimationsController, _person.transform.localScale);
			_personAnimationsController.AnimationService.EnableDisableIk(isOn: true);
			_sitGetOutData.SetCarGetOutExitData(isOnlyOpenCloseDoor: true);
			_personAnimationsController.StopAnimation(withoutTransition: true);
			_sitGetOutData.IsLockSetPosGetOutCar = false;
			_person.GetComponent<NewMoveController>().enabled = true;
		}
	}
}
