using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking;

namespace CarInteractionAnimationsScripts
{
	public class SeatPassengerSetup : ISetupHandler
	{
		private PersonAnimationsController _personAnimationsController;

		private CarAnimationsController _carAnimationsController;

		private SitGetOutData _sitGetOutData;

		private VehicleData _vehicleData;

		private GameObject _person;

		private GameObject _car;

		private bool _playAnimation;

		private int _passengerPlace;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public SeatPassengerSetup(PersonAnimationsController personAnimationsController, GameObject car, bool playAnimation, int passengerPlace)
		{
			_personAnimationsController = personAnimationsController;
			_car = car;
			_playAnimation = playAnimation;
			_sitGetOutData = personAnimationsController.SitGetOutCarSystem.SitGetOutData;
			_person = personAnimationsController.transform.parent.gameObject;
			_carAnimationsController = car.GetComponent<CarAnimationsController>();
			_vehicleData = car.GetComponent<VehicleData>();
			_passengerPlace = passengerPlace;
		}

		public void Enter()
		{
			if (_passengerPlace < 0)
			{
				_passengerPlace = _personAnimationsController.SitGetOutCarSystem.GetPlaceForPassenger(_car);
			}
			IsBusy = IsNotSeat(_passengerPlace);
			if (!IsBusy)
			{
				if ((bool)V.CoreGameManager.MenuLevelParent.ParkingMenuUI)
				{
					V.CoreGameManager.MenuLevelParent.ParkingMenuUI.CancelRaceInvite();
				}
				_sitGetOutData.CurrentPlace = _passengerPlace;
				bool flag = _personAnimationsController.AnimationService.CheckOverTurnedCar(_car);
				IsAnimate = (bool)_carAnimationsController && (bool)_carAnimationsController.CarAnimationReferences && _passengerPlace < _carAnimationsController.CarAnimationReferences.CountSeats && _playAnimation && !flag;
				if (IsAnimate)
				{
					CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _passengerPlace, _person);
					_personAnimationsController.AnimationService.SuspendFirstPersonCamera();
					_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StartTimerForForceSeat();
					_sitGetOutData.SetCarSitData(_car, _passengerPlace, isSitting: true);
					_vehicleData.VehicleMembersController.SetMember(_passengerPlace, _person);
					_personAnimationsController.AnimationService.TurnOnOffCollider(_person.gameObject, isOn: false);
					_personAnimationsController.transform.parent.GetComponent<NewMoveController>().enabled = false;
					_personAnimationsController.SitGetOutCarSystem.CheckMoveCar(_car).Forget();
				}
			}
		}

		public void Exit()
		{
			if (IsBusy || !_personAnimationsController)
			{
				return;
			}
			if (IsAnimate)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _sitGetOutData.CurrentPlace, null);
				_sitGetOutData.SetCarOpenCloseDoorOutsideExitData(isOpenClose: false);
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StopTimerForceSeat();
				if ((bool)_vehicleData)
				{
					_vehicleData.VehicleMembersController.SetMember(_sitGetOutData.CurrentPlace, null);
				}
				_personAnimationsController.transform.parent.GetComponent<NewMoveController>().enabled = true;
				_personAnimationsController.StopAnimation(withoutTransition: true);
				if (!_car)
				{
					_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: true);
					_sitGetOutData.CurrentPlace = -1;
					_sitGetOutData.CurrentCar = null;
				}
				if (_sitGetOutData.IsCanceled)
				{
					if ((bool)_vehicleData)
					{
						_vehicleData.VehicleMembersController.SetMember(_passengerPlace, null);
					}
					_personAnimationsController.SitGetOutCarSystem.SitGetOutData = new SitGetOutData();
					_personAnimationsController.AnimationService.TurnOnOffCollider(_person.gameObject, isOn: true);
				}
				else
				{
					V.CoreGameManager.CarOutInControl.InCarPassenger(_person, _car, _sitGetOutData.CurrentPlace, isSendRpc: false, !IsAnimate);
				}
			}
			else if (_sitGetOutData.IsCanceled)
			{
				_vehicleData.VehicleMembersController.SetMember(_passengerPlace, null);
				_personAnimationsController.SitGetOutCarSystem.SitGetOutData = new SitGetOutData();
			}
			else
			{
				if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.ManagerUI && !FlagsStorage.InGarageFree && !FlagsStorage.InGarageMenu)
				{
					V.CoreGameManager.ManagerUI.CloseAllWindows();
				}
				V.CoreGameManager.CarOutInControl.InCarPassenger(_person, _car, _sitGetOutData.CurrentPlace, isSendRpc: false);
			}
		}

		private bool IsNotSeat(int place)
		{
			if (!_vehicleData)
			{
				return true;
			}
			_ = _vehicleData.LevelsProvider.LevelScript;
			bool isTraffiCar = _vehicleData.isTraffiCar;
			bool hasPlaceForPassenger = _vehicleData.VehicleMembersController.HasPlaceForPassenger;
			bool flag = _vehicleData.CarLocked && !_vehicleData.IsMineCar;
			bool num = (((place == -1 || (bool)_vehicleData.VehicleMembersController.GetMember(place)) | isTraffiCar) || !hasPlaceForPassenger) | flag;
			if (num)
			{
				int indexMessage = 0;
				if (flag)
				{
					indexMessage = 1;
				}
				else if (isTraffiCar)
				{
					indexMessage = 2;
				}
				if ((bool)V.CoreGameManager.CarOutInControl)
				{
					V.CoreGameManager.CarOutInControl.ShowBusyMessage(indexMessage);
				}
			}
			return num;
		}
	}
}
