using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class SeatOtherPlayerSetup : SeatSetupBase, ISetupHandler
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private VehicleData _vehicleData;

		private GameObject _person;

		private int _place;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public SeatOtherPlayerSetup(PersonAnimationsController personAnimController, GameObject car, int place)
			: base(car)
		{
			_personAnimationsController = personAnimController;
			_sitGetOutData = personAnimController.SitGetOutCarSystem.SitGetOutData;
			_place = place;
			_vehicleData = car.GetComponent<VehicleData>();
			_person = personAnimController.transform.parent.gameObject;
		}

		public void Enter()
		{
			bool flag = _personAnimationsController.AnimationService.CheckOverTurnedCar(_car);
			IsAnimate = (bool)_carAnimationsController && (bool)_carAnimationsController.CarAnimationReferences && _place < _carAnimationsController.CarAnimationReferences.CountSeats && !flag;
			_sitGetOutData.CurrentPlace = _place;
			if (CheckBusySeat(_vehicleData.VehicleMembersController.GetMember(_place)))
			{
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.CancelSeat();
			}
			_vehicleData.VehicleMembersController.SetMember(_place, _personAnimationsController.transform.parent.gameObject);
			if (IsAnimate)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _place, _person);
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StartTimerForForceSeat();
				_sitGetOutData.SetCarSitData(_car, _place, isSitting: true);
				_personAnimationsController.SetAnimatorCullingModeAlways(value: true);
				_personAnimationsController.AnimationService.TurnOnOffCollider(_person.gameObject, isOn: false);
				_personAnimationsController.SitGetOutCarSystem.CheckMoveCar(_car).Forget();
			}
		}

		public void Exit()
		{
			if (!_personAnimationsController)
			{
				return;
			}
			if (IsAnimate)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _place, null);
				_sitGetOutData.SetCarOpenCloseDoorOutsideExitData(isOpenClose: false);
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StopTimerForceSeat();
				_personAnimationsController.StopAnimation(withoutTransition: true);
				_personAnimationsController.SetAnimatorCullingModeAlways(value: false);
				if (!_car)
				{
					_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: true);
					_sitGetOutData.CurrentPlace = -1;
					_sitGetOutData.CurrentCar = null;
				}
			}
			if (_sitGetOutData.IsCanceled)
			{
				if ((bool)_vehicleData)
				{
					_vehicleData.VehicleMembersController.SetMember(_place, null);
				}
				_personAnimationsController.SitGetOutCarSystem.SitGetOutData = new SitGetOutData();
				_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: true);
				return;
			}
			MultiPlayer multiPlayer = null;
			if ((bool)V.CoreGameManager && V.CoreGameManager.FreeDriveProvider != null)
			{
				multiPlayer = V.CoreGameManager.FreeDriveProvider.MultiPlayer;
			}
			if (_place == 0)
			{
				TryCloseHoodAndTrunk();
				if ((bool)multiPlayer)
				{
					multiPlayer.PlayerEnteredToCar(_person, _car, !IsAnimate);
				}
			}
			else if ((bool)multiPlayer)
			{
				multiPlayer.PlayerEnteredToCarPassenger(_person, _car, _place, !IsAnimate);
			}
		}

		private bool CheckBusySeat(GameObject person)
		{
			if (!person)
			{
				return false;
			}
			MainPerson component = person.GetComponent<MainPerson>();
			PersonAnimationsController personAnimationsController = (component ? component.PersonAnimationsController : null);
			if (!personAnimationsController)
			{
				return false;
			}
			personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.CancelSeat();
			return true;
		}
	}
}
