using CarAnimationScripts;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class OpenCloseDoorOutsideOtherPlayerSetup : ISetupHandler
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private CarAnimationsController _carAnimationsController;

		private VehicleData _vehicleData;

		private GameObject _person;

		private GameObject _car;

		private int _place;

		private CarAnimationType _carAnimationType;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public OpenCloseDoorOutsideOtherPlayerSetup(PersonAnimationsController personAnimationsController, GameObject car, SitGetOutData sitGetOutData, int place, CarAnimationType carAnimationType)
		{
			_personAnimationsController = personAnimationsController;
			_sitGetOutData = sitGetOutData;
			_place = place;
			_carAnimationType = carAnimationType;
			_car = car;
			_carAnimationsController = car.GetComponent<CarAnimationsController>();
			_vehicleData = car.GetComponent<VehicleData>();
			_person = personAnimationsController.transform.parent.gameObject;
		}

		public void Enter()
		{
			IsAnimate = true;
			CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _place, _person);
			_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StartTimerForCancelOpenCloseDoor();
			_sitGetOutData.SetCarOpenCloseDoorOutsideStartData(_carAnimationsController, _place);
			_vehicleData.VehicleMembersController.SetMember(_place, _personAnimationsController.transform.parent.gameObject);
			_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: false);
		}

		public void Exit()
		{
			if ((bool)_person && (bool)_personAnimationsController)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _place, null);
				_personAnimationsController.AnimationService.TurnOnOffCollider(_person, isOn: true);
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StopTimerCancelOpenCloseDoor();
				_sitGetOutData.SetCarOpenCloseDoorOutsideExitData(isOpenClose: true);
				_personAnimationsController.StopAnimation();
				if ((bool)_vehicleData)
				{
					_vehicleData.VehicleMembersController.SetMember(_place, null);
				}
			}
		}
	}
}
