using System;
using CarAnimationScripts;
using UniRx;
using UnityEngine;
using _Car_Parking.Scripts.Providers;

namespace CarInteractionAnimationsScripts
{
	public class OpenCloseDriverDoorInsideSetup : ISetupHandler
	{
		private PersonAnimationsController _personAnimationsController;

		private CarAnimationsController _carAnimationsController;

		private SitGetOutData _sitGetOutData;

		private GameObject _person;

		private GameObject _car;

		private VehicleData _vehicleData;

		private LevelsProvider _levelsProvider;

		private CarAnimationType _carAnimationType;

		private const float DelayForShowDoorButton = 1f;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public OpenCloseDriverDoorInsideSetup(PersonAnimationsController personAnimationsController, GameObject car, SitGetOutData sitGetOutData, CarAnimationType carAnimationType)
		{
			_personAnimationsController = personAnimationsController;
			_car = car;
			_vehicleData = car.GetComponent<VehicleData>();
			_sitGetOutData = sitGetOutData;
			_carAnimationType = carAnimationType;
			_carAnimationsController = car.GetComponent<CarAnimationsController>();
			_person = personAnimationsController.transform.parent.gameObject;
			_levelsProvider = _vehicleData.LevelsProvider;
		}

		public void Enter()
		{
			IsAnimate = _carAnimationsController;
			FreeDriveProvider freeDriveProvider = _vehicleData.FreeDriveProvider;
			IsBusy = _vehicleData.CarLocked && !_vehicleData.IsMineCar;
			if (IsBusy)
			{
				V.CoreGameManager.CarOutInControl.ShowBusyMessage(1);
			}
			else if (IsAnimate)
			{
				if ((bool)freeDriveProvider.QuickCarSettings)
				{
					freeDriveProvider.QuickCarSettings.HideDoor();
					_sitGetOutData.DisableDriverButtonInCar = true;
				}
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, 0, _person);
				_personAnimationsController.SitGetOutCarSystem.MultiPlayerSendRpcCarAnimations.SendOpenCloseCarDoor(_person, _car, 0, _carAnimationType);
				_person.transform.localScale = _personAnimationsController.ScalePersonOutCar;
				_sitGetOutData.SetCarInsideStartData(_carAnimationsController, 0, isOnlyOpenCloseDoor: true);
				_personAnimationsController.Animator.enabled = true;
			}
		}

		public void Exit()
		{
			if (!IsAnimate)
			{
				return;
			}
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				_sitGetOutData.DisableDriverButtonInCar = false;
				if ((bool)V.CoreGameManager)
				{
					FreeDriveProvider freeDriveProvider = V.CoreGameManager.FreeDriveProvider;
					if ((bool)freeDriveProvider.QuickCarSettings)
					{
						freeDriveProvider.QuickCarSettings.ShowDoor();
					}
				}
			}).AddTo(_personAnimationsController);
			CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, 0, null);
			_person.transform.localScale = CarAnimationReferences.GetSizePlayerInCarSafe(_carAnimationsController, _person.transform.localScale);
			_personAnimationsController.AnimationService.EnableDisableIk(isOn: true);
			_sitGetOutData.SetCarGetOutExitData(isOnlyOpenCloseDoor: true);
			_sitGetOutData.IsLockSetPosGetOutCar = false;
			_personAnimationsController.StopAnimation(withoutTransition: true);
		}
	}
}
