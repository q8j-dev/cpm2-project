using System;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Providers;

namespace CarInteractionAnimationsScripts
{
	public class GetOutSetup : ISetupHandler
	{
		private PersonAnimationsController _personAnimationsController;

		private CarAnimationsController _carAnimationsController;

		private SitGetOutData _sitGetOutData;

		private GameObject _person;

		private GameObject _car;

		private VehicleData _vehicleData;

		private LevelsProvider _levelsProvider;

		private bool _playAnimation;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public GetOutSetup(PersonAnimationsController personAnimationsController, GameObject car, bool playAnimation)
		{
			_personAnimationsController = personAnimationsController;
			_car = car;
			_vehicleData = (car ? car.GetComponent<VehicleData>() : null);
			_levelsProvider = (_vehicleData ? _vehicleData.LevelsProvider : null);
			_playAnimation = playAnimation;
			_sitGetOutData = personAnimationsController.SitGetOutCarSystem.SitGetOutData;
			_carAnimationsController = (car ? car.GetComponent<CarAnimationsController>() : null);
			_person = (personAnimationsController.transform.parent ? personAnimationsController.transform.parent.gameObject : null);
		}

		public void Enter()
		{
			bool flag = (bool)_car && _personAnimationsController.AnimationService.CheckOverTurnedCar(_car);
			IsAnimate = (bool)_carAnimationsController && _sitGetOutData.CurrentPlace < _carAnimationsController.CarAnimationReferences.CountSeats && _playAnimation && !flag;
			_personAnimationsController.SitGetOutCarSystem.MultiPlayerSendRpcCarAnimations.SendGetOutCar(_person, _car, _sitGetOutData.CurrentPlace, _playAnimation);
			if ((bool)_car)
			{
				_car.GetComponent<Rigidbody>();
			}
			if (!IsAnimate)
			{
				return;
			}
			if ((bool)V.CoreGameManager.MenuLevelParent.ParkingMenuUI)
			{
				V.CoreGameManager.MenuLevelParent.ParkingMenuUI.CancelRaceInvite();
			}
			if ((bool)V.CoreGameManager.FreeDriveProvider.QuickCarSettings)
			{
				V.CoreGameManager.FreeDriveProvider.QuickCarSettings.HideDoor();
				V.CoreGameManager.FreeDriveProvider.QuickCarSettings.TurnOnOffRoofButton(isOn: false);
			}
			if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.ManagerUI)
			{
				WindowScript window = V.CoreGameManager.ManagerUI.GetWindow("MenuControllerHUD");
				if ((bool)window && window is MenuGarageNavigationWindow menuGarageNavigationWindow)
				{
					menuGarageNavigationWindow.UpdateHUDClickDriverButton();
				}
			}
			_personAnimationsController.AnimationService.SuspendFirstPersonCamera();
			if ((bool)_person && _person.TryGetComponent<MainPerson>(out var component))
			{
				component.SwitchHeadVisible(visible: true);
			}
			_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StartTimerForForceGetOut();
			_person.transform.localScale = _personAnimationsController.ScalePersonOutCar;
			_sitGetOutData.SetCarInsideStartData(_carAnimationsController, _sitGetOutData.CurrentPlace, isOnlyOpenCloseDoor: false);
			_personAnimationsController.Animator.enabled = true;
			_person.GetComponent<NewMoveController>().enabled = false;
			_personAnimationsController.SitGetOutCarSystem.CheckMoveCar(_car).Forget();
			CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _sitGetOutData.CurrentPlace, _person);
			if ((bool)_carAnimationsController.CarAnimationsPoints && _carAnimationsController.CarAnimationsPoints.TryGetSeatPoint((CarDoorType)_sitGetOutData.CurrentPlace, out var seatPoint))
			{
				_person.transform.position = seatPoint.position;
			}
			bool num = _person == V.DriverPerson;
			if (num && _levelsProvider != null && (bool)_levelsProvider.LevelScript)
			{
				_levelsProvider.LevelScript.CleanForPersonView(isEnableTouch: false);
			}
			if (num && (bool)V.CoreGameManager.MenuLevelParent)
			{
				if ((bool)V.CoreGameManager.MenuLevelParent.LeftRight)
				{
					V.CoreGameManager.MenuLevelParent.LeftRight.SetActive(value: false);
				}
				if ((bool)V.CoreGameManager.MenuLevelParent.SteeringWheel)
				{
					V.CoreGameManager.MenuLevelParent.SteeringWheel.SetActive(active: false);
				}
			}
		}

		public void Exit()
		{
			if (IsBusy)
			{
				FlagsStorage.IsDoorOpenAnimation = true;
				return;
			}
			bool isChangePos = !IsAnimate || _sitGetOutData.IsForceGetOutCar;
			if (IsAnimate && (bool)_personAnimationsController)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _sitGetOutData.CurrentPlace, null);
				_sitGetOutData.SetCarGetOutExitData(isOnlyOpenCloseDoor: false);
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StopTimerForceGetOut();
				_personAnimationsController.Animator.speed = 1f;
				_person.GetComponent<NewMoveController>().enabled = true;
				Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					_sitGetOutData.IsLockSetPosGetOutCar = false;
				}).AddTo(_personAnimationsController);
			}
			_personAnimationsController.StopAnimation();
			if (_sitGetOutData.IsCanceled)
			{
				_personAnimationsController.SitGetOutCarSystem.SitGetOutData = new SitGetOutData();
				FlagsStorage.IsDoorOpenAnimation = false;
			}
			else
			{
				V.CoreGameManager.CarOutInControl.OutCar(_person, _car, enableCollidersImmediately: false, isChangePos, isSendRpc: false);
			}
		}
	}
}
