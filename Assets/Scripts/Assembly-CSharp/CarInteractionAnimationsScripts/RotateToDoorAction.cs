using System;
using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class RotateToDoorAction : IAction, IDisposable
	{
		private const float MinAngleForRotate = 5f;

		private const float SpeedForRotate = 5f;

		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private bool _isOnlyOpenCloseDoor;

		private WalkInputCancelWatcher _cancelWatcher;

		public RotateToDoorAction(PersonAnimationsController personAnimationsController, bool isOnlyOpenCloseDoor = false, WalkInputCancelWatcher cancelWatcher = null)
		{
			_personAnimationsController = personAnimationsController;
			_isOnlyOpenCloseDoor = isOnlyOpenCloseDoor;
			_cancelWatcher = cancelWatcher;
			_sitGetOutData = personAnimationsController.SitGetOutCarSystem.SitGetOutData;
		}

		public void Dispose()
		{
		}

		public async UniTask Run(CarAnimationsController car, CancellationToken cancellationToken)
		{
			CheckerStopAction checkerStopAction = new CheckerStopAction();
			Transform personTransform = _personAnimationsController.transform.parent;
			Quaternion pointRotation = ((car.OpenDoors[(CarDoorType)_sitGetOutData.CurrentPlace] && _isOnlyOpenCloseDoor) ? car.CarAnimationsPoints.OpenCloseDoorsPointsToMove[_sitGetOutData.CurrentPlace].rotation : car.CarAnimationsPoints.DoorsPointsToMove[_sitGetOutData.CurrentPlace].rotation);
			if (car.CarAnimationReferences.IsF1Car)
			{
				Transform parent = _personAnimationsController.transform.parent;
				Transform transform = car.transform;
				if (_personAnimationsController.AnimationService.IsPlayerRightSideFromCar(transform, parent))
				{
					pointRotation = car.CarAnimationsPoints.DoorsPointsToMove[1].rotation;
				}
			}
			_personAnimationsController.Animator.Rebind();
			_personAnimationsController.Animator.speed = 0f;
			try
			{
				while (Quaternion.Angle(personTransform.rotation, pointRotation) > 5f)
				{
					personTransform.rotation = Quaternion.Slerp(personTransform.rotation, pointRotation, Time.deltaTime * 5f);
					await UniTask.Yield();
					if ((_cancelWatcher != null && _cancelWatcher.TryCancelSeat()) || checkerStopAction.CheckStop(_personAnimationsController, car, cancellationToken))
					{
						return;
					}
				}
				personTransform.rotation = pointRotation;
			}
			finally
			{
				if ((bool)_personAnimationsController && (bool)_personAnimationsController.Animator)
				{
					_personAnimationsController.Animator.speed = 1f;
				}
			}
		}
	}
}
