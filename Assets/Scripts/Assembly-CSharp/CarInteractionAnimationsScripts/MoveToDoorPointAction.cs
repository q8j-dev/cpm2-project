using System;
using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class MoveToDoorPointAction : IAction, IDisposable
	{
		private const float MinDistanceForDoorPoint = 0.09f;

		private const float MinAngleForRotate = 5f;

		private const float SpeedRotate = 10f;

		private PersonAnimationsController _personAnimationsController;

		private SitGetOutData _sitGetOutData;

		private bool _isOnlyOpenCloseDoor;

		private WalkInputCancelWatcher _cancelWatcher;

		public MoveToDoorPointAction(PersonAnimationsController personAnimationsController, bool isOnlyOpenCloseDoor = false, WalkInputCancelWatcher cancelWatcher = null)
		{
			_personAnimationsController = personAnimationsController;
			_isOnlyOpenCloseDoor = isOnlyOpenCloseDoor;
			_cancelWatcher = cancelWatcher;
			_sitGetOutData = personAnimationsController.SitGetOutCarSystem.SitGetOutData;
		}

		public void Dispose()
		{
		}

		public async UniTask Run(CarAnimationsController carAnimController, CancellationToken cancelToken)
		{
			CheckerStopAction checkerStopAction = new CheckerStopAction();
			Transform person = _personAnimationsController.transform.parent;
			Transform doorPoint = ((carAnimController.OpenDoors[(CarDoorType)_sitGetOutData.CurrentPlace] && _isOnlyOpenCloseDoor) ? carAnimController.CarAnimationsPoints.OpenCloseDoorsPointsToMove[_sitGetOutData.CurrentPlace] : carAnimController.CarAnimationsPoints.DoorsPointsToMove[_sitGetOutData.CurrentPlace]);
			if (carAnimController.CarAnimationReferences.IsF1Car)
			{
				Transform parent = _personAnimationsController.transform.parent;
				Transform transform = carAnimController.transform;
				if (_personAnimationsController.AnimationService.IsPlayerRightSideFromCar(transform, parent))
				{
					doorPoint = carAnimController.CarAnimationsPoints.DoorsPointsToMove[1];
				}
			}
			_personAnimationsController.AnimationService.TurnOnOffCollider(person.gameObject, isOn: true);
			float distanceV2 = GetDistanceV2(person, doorPoint);
			while (distanceV2 > 0.09f)
			{
				_personAnimationsController.PlayMoveAnimation(0f, 0.2f);
				_personAnimationsController.SetBoolIsMoving(value: true);
				distanceV2 = GetDistanceV2(person, doorPoint);
				Vector3 vector = doorPoint.position - person.position;
				vector.y = 0f;
				if (Vector3.Angle(person.forward, vector) > 5f)
				{
					Quaternion b = Quaternion.LookRotation(vector);
					person.rotation = Quaternion.Slerp(person.rotation, b, Time.deltaTime * 10f);
				}
				await UniTask.Yield();
				if (_cancelWatcher != null && _cancelWatcher.TryCancelSeat())
				{
					_personAnimationsController.PlayMoveAnimation(0f, 0f);
					_personAnimationsController.SetBoolIsMoving(value: false);
					return;
				}
				if (checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancelToken))
				{
					return;
				}
			}
			_personAnimationsController.PlayMoveAnimation(0f, 0f);
			_personAnimationsController.SetBoolIsMoving(value: false);
			_personAnimationsController.AnimationService.TurnOnOffCollider(person.gameObject, isOn: false);
			person.transform.position = doorPoint.position;
		}

		private float GetDistanceV2(Transform from, Transform to)
		{
			Vector2 vector = new Vector2(from.position.x, from.position.z);
			return (new Vector2(to.position.x, to.position.z) - vector).sqrMagnitude;
		}
	}
}
