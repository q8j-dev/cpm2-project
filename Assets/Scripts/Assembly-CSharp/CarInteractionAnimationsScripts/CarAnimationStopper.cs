using System;
using CarAnimationScripts;
using UniRx;

namespace CarInteractionAnimationsScripts
{
	public class CarAnimationStopper
	{
		private const float TimeForForceSeat = 10f;

		private const float TimeForForceGetOut = 10f;

		private const float TimeForCancelOpenCloseDoor = 5f;

		private PersonAnimationsController _personAnimationsController;

		private IDisposable _forceSeatDisposable;

		private IDisposable _forceGetOutDisposable;

		private IDisposable _cancelOpenCloseDoorDisposable;

		private CarAnimationsController _carAnimationsController => _personAnimationsController.SitGetOutCarSystem.SitGetOutData.CurrentCar;

		private SitGetOutData SafeSitGetOutData
		{
			get
			{
				if (!_personAnimationsController)
				{
					return null;
				}
				return _personAnimationsController.SitGetOutCarSystem?.SitGetOutData;
			}
		}

		public CarAnimationStopper(PersonAnimationsController personAnimationsController)
		{
			_personAnimationsController = personAnimationsController;
		}

		public void ForceSit()
		{
			SitGetOutData safeSitGetOutData = SafeSitGetOutData;
			if (safeSitGetOutData != null)
			{
				StopTimerForceSeat();
				safeSitGetOutData.IsForceSeatIntoCar = true;
				if ((bool)_personAnimationsController.Animator)
				{
					_personAnimationsController.Animator.speed = 1f;
				}
				safeSitGetOutData.CancellationTokenSource?.Cancel();
				if ((bool)_carAnimationsController)
				{
					_carAnimationsController.FastOpenCloseDoor((CarDoorType)safeSitGetOutData.CurrentPlace, isOpen: false);
				}
			}
		}

		public void ForceGetOut()
		{
			SitGetOutData safeSitGetOutData = SafeSitGetOutData;
			if (safeSitGetOutData != null)
			{
				StopTimerForceGetOut();
				safeSitGetOutData.IsForceGetOutCar = true;
				if ((bool)_personAnimationsController.Animator)
				{
					_personAnimationsController.Animator.speed = 1f;
				}
				safeSitGetOutData.CancellationTokenSource?.Cancel();
				if ((bool)_carAnimationsController)
				{
					_carAnimationsController.FastOpenCloseDoor((CarDoorType)safeSitGetOutData.CurrentPlace, isOpen: false);
				}
			}
		}

		public void CancelOpenCloseDoor()
		{
			SitGetOutData safeSitGetOutData = SafeSitGetOutData;
			if (safeSitGetOutData != null)
			{
				StopTimerCancelOpenCloseDoor();
				safeSitGetOutData.CancellationTokenSource?.Cancel();
			}
		}

		public void StartTimerForForceSeat()
		{
			StopTimerForceSeat();
			_forceSeatDisposable = Observable.Timer(TimeSpan.FromSeconds(10.0)).Subscribe((long _) =>
			{
				ForceSit();
			}).AddTo(_personAnimationsController);
		}

		public void StartTimerForForceGetOut()
		{
			StopTimerForceGetOut();
			_forceGetOutDisposable = Observable.Timer(TimeSpan.FromSeconds(10.0)).Subscribe((long _) =>
			{
				ForceGetOut();
			}).AddTo(_personAnimationsController);
		}

		public void StartTimerForCancelOpenCloseDoor()
		{
			StopTimerCancelOpenCloseDoor();
			_cancelOpenCloseDoorDisposable = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
			{
				CancelOpenCloseDoor();
			}).AddTo(_personAnimationsController);
		}

		public void StopTimerForceSeat()
		{
			if (_forceSeatDisposable != null)
			{
				_forceSeatDisposable?.Dispose();
			}
			_forceSeatDisposable = null;
		}

		public void StopTimerForceGetOut()
		{
			if (_forceGetOutDisposable != null)
			{
				_forceGetOutDisposable?.Dispose();
			}
			_forceGetOutDisposable = null;
		}

		public void StopTimerCancelOpenCloseDoor()
		{
			if (_cancelOpenCloseDoorDisposable != null)
			{
				_cancelOpenCloseDoorDisposable?.Dispose();
			}
			_cancelOpenCloseDoorDisposable = null;
		}

		public void CancelSeat()
		{
			SitGetOutData safeSitGetOutData = SafeSitGetOutData;
			if (safeSitGetOutData != null && !safeSitGetOutData.IsCanceled)
			{
				StopTimerForceSeat();
				safeSitGetOutData.CancellationTokenSource?.Cancel();
				safeSitGetOutData.IsCanceled = true;
				CloseDoorOpenedForSeat(safeSitGetOutData);
				SendCancelSeatToOtherPlayers(safeSitGetOutData);
			}
		}

		public void CancelGetOut()
		{
			SitGetOutData safeSitGetOutData = SafeSitGetOutData;
			if (safeSitGetOutData != null && !safeSitGetOutData.IsCanceled)
			{
				StopTimerForceGetOut();
				safeSitGetOutData.CancellationTokenSource?.Cancel();
				safeSitGetOutData.IsCanceled = true;
			}
		}

		private void CloseDoorOpenedForSeat(SitGetOutData sitGetOutData)
		{
			if ((bool)_carAnimationsController && _carAnimationsController.OpenDoors != null && sitGetOutData.CurrentPlace >= 0 && !sitGetOutData.IsCurrentDoorWasOpened)
			{
				CarDoorType currentPlace = (CarDoorType)sitGetOutData.CurrentPlace;
				if (_carAnimationsController.OpenDoors.TryGetValue(currentPlace, out var value) && value)
				{
					_carAnimationsController.FastOpenCloseDoor(currentPlace, isOpen: false);
				}
			}
		}

		private void SendCancelSeatToOtherPlayers(SitGetOutData sitGetOutData)
		{
			if ((bool)_carAnimationsController && (bool)_personAnimationsController.transform.parent)
			{
				_personAnimationsController.SitGetOutCarSystem.MultiPlayerSendRpcCarAnimations.SendGetOutCar(_personAnimationsController.transform.parent.gameObject, _carAnimationsController.gameObject, sitGetOutData.CurrentPlace, playAnimation: false);
			}
		}
	}
}
