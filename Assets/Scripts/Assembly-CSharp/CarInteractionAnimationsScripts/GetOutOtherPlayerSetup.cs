using System;
using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class GetOutOtherPlayerSetup : ISetupHandler
	{
		private PersonAnimationsController _personAnimationsController;

		private CarAnimationsController _carAnimationsController;

		private SitGetOutData _sitGetOutData;

		private int _place;

		private GameObject _person;

		private GameObject _car;

		public bool IsAnimate { get; private set; }

		public bool IsBusy { get; private set; }

		public GetOutOtherPlayerSetup(PersonAnimationsController personAnimationsController, GameObject car, int place)
		{
			_personAnimationsController = personAnimationsController;
			_car = car;
			_sitGetOutData = personAnimationsController.SitGetOutCarSystem.SitGetOutData;
			_place = place;
			_carAnimationsController = car.GetComponent<CarAnimationsController>();
			_person = personAnimationsController.transform.parent.gameObject;
		}

		public void Enter()
		{
			bool flag = _personAnimationsController.AnimationService.CheckOverTurnedCar(_car);
			IsAnimate = (bool)_carAnimationsController && (bool)_carAnimationsController.CarAnimationReferences && _place < _carAnimationsController.CarAnimationReferences.CountSeats && !flag;
			if (!IsAnimate)
			{
				return;
			}
			if (_person.TryGetComponent<MultiPersonControl>(out var component) && _person.TryGetComponent<MainPerson>(out var component2))
			{
				component.currentCar = null;
				if ((bool)component2.mainBodySkin)
				{
					component2.mainBodySkin.enabled = true;
				}
			}
			_person.transform.localScale = _personAnimationsController.ScalePersonOutCar;
			_sitGetOutData.CancellationTokenSource?.Cancel();
			_sitGetOutData.CancellationTokenSource = new CancellationTokenSource();
			_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StartTimerForForceGetOut();
			_sitGetOutData.SetCarInsideStartData(_carAnimationsController, _place, isOnlyOpenCloseDoor: false);
			if ((bool)_carAnimationsController.CarAnimationsPoints && _carAnimationsController.CarAnimationsPoints.TryGetSeatPoint((CarDoorType)_sitGetOutData.CurrentPlace, out var seatPoint))
			{
				_person.transform.position = seatPoint.position;
			}
			_personAnimationsController.AnimationService.EnableDisableIk(isOn: true);
			_personAnimationsController.SetAnimatorCullingModeAlways(value: true);
			_personAnimationsController.Animator.enabled = true;
			_personAnimationsController.SitGetOutCarSystem.CheckMoveCar(_car).Forget();
		}

		public void Exit()
		{
			if (!_personAnimationsController)
			{
				return;
			}
			if (IsAnimate)
			{
				_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.StopTimerForceGetOut();
				_sitGetOutData.SetCarGetOutExitData(isOnlyOpenCloseDoor: false);
				_personAnimationsController.SetAnimatorCullingModeAlways(value: false);
				Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
				{
					_sitGetOutData.IsLockSetPosGetOutCar = false;
				}).AddTo(_personAnimationsController);
			}
			_personAnimationsController.StopAnimation();
			V.CoreGameManager.FreeDriveProvider.MultiPlayer.PlayerExitFromCar(_person, _car, !IsAnimate);
		}
	}
}
