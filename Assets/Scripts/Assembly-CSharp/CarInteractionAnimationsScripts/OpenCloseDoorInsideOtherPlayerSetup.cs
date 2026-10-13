using CarAnimationScripts;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class OpenCloseDoorInsideOtherPlayerSetup : ISetupHandler
	{
		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private CarAnimationsController _carAnimationsController;

		private GameObject _person;

		private int _place;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public OpenCloseDoorInsideOtherPlayerSetup(CarAnimationsController car, PersonAnimationsController personAnimationsController, SitGetOutData sitGetOutData, int place)
		{
			_personAnimationsController = personAnimationsController;
			_sitGetOutData = sitGetOutData;
			_place = place;
			_person = personAnimationsController.transform.parent.gameObject;
			_carAnimationsController = car;
		}

		public void Enter()
		{
			IsAnimate = true;
			CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _place, _person);
			_person.transform.localScale = _personAnimationsController.ScalePersonOutCar;
			_sitGetOutData.SetCarInsideStartData(_sitGetOutData.CurrentCar, _place, isOnlyOpenCloseDoor: true);
			_personAnimationsController.Animator.enabled = true;
			_personAnimationsController.AnimationService.EnableDisableIk(isOn: true);
		}

		public void Exit()
		{
			if ((bool)_person && (bool)_personAnimationsController)
			{
				CarAnimationReferences.SetAnimateSeatSafe(_carAnimationsController, _place, null);
				_person.transform.localScale = CarAnimationReferences.GetSizePlayerInCarSafe(_carAnimationsController, _person.transform.localScale);
				_personAnimationsController.AnimationService.EnableDisableIk(isOn: true);
				_sitGetOutData.SetCarGetOutExitData(isOnlyOpenCloseDoor: true);
				_sitGetOutData.IsLockSetPosGetOutCar = false;
				_personAnimationsController.StopAnimation(withoutTransition: true);
			}
		}
	}
}
