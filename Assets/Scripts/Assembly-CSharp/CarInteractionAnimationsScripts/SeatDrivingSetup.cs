using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking;

namespace CarInteractionAnimationsScripts
{
	public class SeatDrivingSetup : SeatSetupBase, ISetupHandler
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private VehicleData _vehicleData;

		private GameObject _person;

		private bool _playAnimation;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public SeatDrivingSetup(PersonAnimationsController personAnimationsController, GameObject car, bool playAnimation)
			: base(car)
		{
			_personAnimationsController = personAnimationsController;
			_sitGetOutData = personAnimationsController.SitGetOutCarSystem.SitGetOutData;
			_playAnimation = playAnimation;
			_vehicleData = car.GetComponent<VehicleData>();
			_person = personAnimationsController.transform.parent.gameObject;
		}

		public void Enter()
		{
			bool flag = _personAnimationsController.AnimationService.CheckOverTurnedCar(_car);
			_ = _vehicleData.LevelsProvider.LevelScript;
			IsAnimate = (bool)_carAnimationsController && (bool)_carAnimationsController.CarAnimationReferences && _playAnimation && !flag;
			IsBusy = (bool)_vehicleData.Driver || (_vehicleData.CarLocked && !_vehicleData.IsMineCar);
			if (IsBusy)
			{
				int indexMessage = 0;
				if (_vehicleData.CarLocked)
				{
					indexMessage = 1;
				}
				if ((bool)V.CoreGameManager.CarOutInControl)
				{
					V.CoreGameManager.CarOutInControl.ShowBusyMessage(indexMessage);
				}
			}
			else if (IsAnimate)
			{
				if ((bool)V.CoreGameManager.MenuLevelParent.ParkingMenuUI)
				{
					V.CoreGameManager.MenuLevelParent.ParkingMenuUI.CancelRaceInvite();
				}
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, 0, _person);
				_personAnimationsController.AnimationService.SuspendFirstPersonCamera();
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StartTimerForForceSeat();
				_sitGetOutData.SetCarSitData(_car, 0, isSitting: true);
				_personAnimationsController.Animator.enabled = true;
				_vehicleData.VehicleMembersController.SetDriver(_personAnimationsController.transform.parent.gameObject);
				_personAnimationsController.AnimationService.TurnOnOffCollider(_person.gameObject, isOn: false);
				_personAnimationsController.transform.parent.GetComponent<NewMoveController>().enabled = false;
				_personAnimationsController.SitGetOutCarSystem.CheckMoveCar(_car).Forget();
			}
		}

		public void Exit()
		{
			if (IsBusy || !_personAnimationsController)
			{
				FlagsStorage.IsDoorOpenAnimation = true;
				return;
			}
			if (IsAnimate)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, 0, null);
				_sitGetOutData.SetCarOpenCloseDoorOutsideExitData(isOpenClose: false);
				if ((bool)_vehicleData)
				{
					_vehicleData.VehicleMembersController.SetDriver(null);
				}
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StopTimerForceSeat();
				_personAnimationsController.transform.parent.GetComponent<NewMoveController>().enabled = true;
				_personAnimationsController.StopAnimation(withoutTransition: true);
			}
			if (_sitGetOutData.IsCanceled)
			{
				_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: true);
				_personAnimationsController.SitGetOutCarSystem.SitGetOutData = new SitGetOutData();
				FlagsStorage.IsDoorOpenAnimation = false;
			}
			else if (!_car)
			{
				_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: true);
				_personAnimationsController.SitGetOutCarSystem.SitGetOutData = new SitGetOutData();
				FlagsStorage.IsDoorOpenAnimation = true;
			}
			else
			{
				TryCloseHoodAndTrunk();
				V.CoreGameManager.CarOutInControl.InCar(_person, _car, isSendRpc: false, !IsAnimate).Forget();
			}
		}
	}
}
