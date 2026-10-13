using System.Threading;
using CarAnimationScripts;
using UnityEngine;

namespace CarInteractionAnimationsScripts
{
	public class SitGetOutData
	{
		private CarAnimationsController _currentCar;

		public bool IsSittingIntoCarAnimate { get; set; }

		public bool IsGettingOutOfTheCarAnimate { get; set; }

		public bool IsLockSetPosGetOutCar { get; set; }

		public bool IsForceSeatIntoCar { get; set; }

		public bool IsForceGetOutCar { get; set; }

		public bool IsIKPoseActive { get; set; }

		public bool IsKitPoseSuppressed { get; set; }

		public CarAnimationsController CurrentCar
		{
			get
			{
				CommonExtensions.NullIfDestroyed(ref _currentCar);
				return _currentCar;
			}
			set
			{
				_currentCar = value;
			}
		}

		public CancellationTokenSource CancellationTokenSource { get; set; }

		public int CurrentPlace { get; set; } = -1;

		public bool IsCurrentDoorWasOpened { get; set; }

		public bool IsCanceled { get; set; }

		public bool DisableDriverButtonInCar { get; set; }

		public void SetCarSitData(GameObject car, int place, bool isSitting)
		{
			if (car.TryGetComponent<CarAnimationsController>(out var component))
			{
				CurrentCar = component;
				IsCurrentDoorWasOpened = component.OpenDoors[(CarDoorType)place];
			}
			else
			{
				CurrentCar = null;
			}
			if (isSitting)
			{
				IsSittingIntoCarAnimate = true;
				CancellationTokenSource?.Cancel();
				CancellationTokenSource?.Dispose();
				CancellationTokenSource = new CancellationTokenSource();
			}
			CurrentPlace = place;
		}

		public void SetCarOpenCloseDoorOutsideStartData(CarAnimationsController carAnimationsController, int place)
		{
			IsSittingIntoCarAnimate = true;
			CancellationTokenSource?.Cancel();
			CancellationTokenSource?.Dispose();
			CancellationTokenSource = new CancellationTokenSource();
			CurrentPlace = place;
			CurrentCar = carAnimationsController;
		}

		public void SetCarOpenCloseDoorOutsideExitData(bool isOpenClose)
		{
			IsSittingIntoCarAnimate = false;
			IsForceSeatIntoCar = false;
			if (isOpenClose)
			{
				CurrentPlace = -1;
				CurrentCar = null;
			}
			else
			{
				IsCurrentDoorWasOpened = false;
			}
		}

		public void SetCarGetOutExitData(bool isOnlyOpenCloseDoor)
		{
			IsGettingOutOfTheCarAnimate = false;
			IsForceGetOutCar = false;
			if (!isOnlyOpenCloseDoor)
			{
				IsCurrentDoorWasOpened = false;
				CurrentCar = null;
				CurrentPlace = -1;
			}
		}

		public void SetCarInsideStartData(CarAnimationsController carAnimationsController, int place, bool isOnlyOpenCloseDoor)
		{
			IsGettingOutOfTheCarAnimate = true;
			IsLockSetPosGetOutCar = true;
			CancellationTokenSource?.Cancel();
			CancellationTokenSource?.Dispose();
			CancellationTokenSource = new CancellationTokenSource();
			CurrentPlace = place;
			if (CurrentPlace == -1)
			{
				CurrentPlace = 0;
			}
			if (!CurrentCar)
			{
				CurrentCar = carAnimationsController;
			}
			if (!isOnlyOpenCloseDoor)
			{
				IsCurrentDoorWasOpened = carAnimationsController.OpenDoors[(CarDoorType)CurrentPlace];
			}
		}
	}
}
