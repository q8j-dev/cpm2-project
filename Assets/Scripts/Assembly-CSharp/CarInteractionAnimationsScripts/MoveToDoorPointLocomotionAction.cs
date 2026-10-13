using System;
using System.Threading;
using CarAnimationScripts;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Person.Locomotion;

namespace CarInteractionAnimationsScripts
{
	public class MoveToDoorPointLocomotionAction : IAction, IDisposable
	{
		private const float ArriveDistance = 0.2f;

		private const float SlowDownDistance = 1.2f;

		private const float MinWalkMagnitude = 0.35f;

		private const float MaxWalkMagnitude = 1f;

		private const float StopWaitSeconds = 0.5f;

		private readonly PersonAnimationsController _personAnimationsController;

		private readonly PersonLocomotion _locomotion;

		private readonly SitGetOutData _sitGetOutData;

		private readonly bool _isOnlyOpenCloseDoor;

		private readonly WalkInputCancelWatcher _cancelWatcher;

		public MoveToDoorPointLocomotionAction(PersonAnimationsController personAnimationsController, PersonLocomotion locomotion, bool isOnlyOpenCloseDoor = false, WalkInputCancelWatcher cancelWatcher = null)
		{
			_personAnimationsController = personAnimationsController;
			_locomotion = locomotion;
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
			Transform doorPoint = ResolveDoorPoint(carAnimController, person);
			_personAnimationsController.AnimationService.TurnOnOffCollider(person.gameObject, isOn: true);
			bool canceled = false;
			try
			{
				_locomotion.CancelSprintLock();
				while (FlatDistance(person, doorPoint) > 0.2f)
				{
					Vector3 worldDir = doorPoint.position - person.position;
					worldDir.y = 0f;
					_locomotion.SetScriptedMoveWorld(worldDir, ResolveMagnitude(worldDir.magnitude));
					await UniTask.Yield();
					if (_cancelWatcher != null && _cancelWatcher.TryCancelSeat())
					{
						canceled = true;
						return;
					}
					if (checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancelToken))
					{
						return;
					}
				}
				_locomotion.ClearScriptedMove();
				float waited = 0f;
				while (!_locomotion.IsFullyStopped && waited < 0.5f)
				{
					waited += Time.deltaTime;
					await UniTask.Yield();
					if (_cancelWatcher != null && _cancelWatcher.TryCancelSeat())
					{
						canceled = true;
						break;
					}
					if (checkerStopAction.CheckStop(_personAnimationsController, carAnimController, cancelToken))
					{
						break;
					}
				}
			}
			finally
			{
				_locomotion.ClearScriptedMove();
				_locomotion.Suspend();
				if (!canceled && !_sitGetOutData.IsCanceled && !cancelToken.IsCancellationRequested)
				{
					_personAnimationsController.AnimationService.TurnOnOffCollider(person.gameObject, isOn: false);
					person.position = doorPoint.position;
				}
			}
		}

		private float ResolveMagnitude(float distance)
		{
			if (distance >= 1.2f)
			{
				return 1f;
			}
			float t = Mathf.InverseLerp(0.2f, 1.2f, distance);
			return Mathf.Lerp(0.35f, 1f, t);
		}

		private Transform ResolveDoorPoint(CarAnimationsController carAnimController, Transform person)
		{
			Transform result = ((carAnimController.OpenDoors[(CarDoorType)_sitGetOutData.CurrentPlace] && _isOnlyOpenCloseDoor) ? carAnimController.CarAnimationsPoints.OpenCloseDoorsPointsToMove[_sitGetOutData.CurrentPlace] : carAnimController.CarAnimationsPoints.DoorsPointsToMove[_sitGetOutData.CurrentPlace]);
			if (carAnimController.CarAnimationReferences.IsF1Car && _personAnimationsController.AnimationService.IsPlayerRightSideFromCar(carAnimController.transform, person))
			{
				result = carAnimController.CarAnimationsPoints.DoorsPointsToMove[1];
			}
			return result;
		}

		private float FlatDistance(Transform from, Transform to)
		{
			Vector2 vector = new Vector2(from.position.x, from.position.z);
			return (new Vector2(to.position.x, to.position.z) - vector).magnitude;
		}
	}
}
