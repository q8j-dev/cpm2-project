using _Car_Parking;

namespace CarInteractionAnimationsScripts
{
	public class FirstPersonCameraSuspender
	{
		private const string GarageHudWindowName = "MenuControllerHUD";

		private const int InsideCarCamIndex = 0;

		private const int ThirdPersonCarCamIndex = 1;

		private readonly PersonAnimationsController _personAnimationsController;

		private bool _walkCameraSuspended;

		private int _walkCameraPositionToRestore;

		private bool _carCameraSuspended;

		private bool _garageCameraSuspended;

		public FirstPersonCameraSuspender(PersonAnimationsController personAnimationsController)
		{
			_personAnimationsController = personAnimationsController;
		}

		public void Suspend()
		{
			if (IsLocalDriverPerson())
			{
				if (FlagsStorage.OutOfCar)
				{
					SuspendWalkCamera();
				}
				else
				{
					SuspendCarCamera();
				}
			}
		}

		public void Restore()
		{
			RestoreWalkCamera();
			RestoreGarageCamera();
			RestoreCarCamera();
		}

		private void SuspendWalkCamera()
		{
			NewMoveController newMoveController = ResolveMoveController();
			if ((bool)newMoveController && newMoveController.IsFirstPersonCamera)
			{
				_walkCameraPositionToRestore = newMoveController.CurrentCameraPosition;
				newMoveController.SetCameraPosition(0, save: false);
				_walkCameraSuspended = true;
			}
		}

		private void RestoreWalkCamera()
		{
			if (_walkCameraSuspended)
			{
				_walkCameraSuspended = false;
				NewMoveController newMoveController = ResolveMoveController();
				if ((bool)newMoveController)
				{
					newMoveController.SetCameraPosition(_walkCameraPositionToRestore, save: false);
				}
			}
		}

		private void SuspendCarCamera()
		{
			MenuGarageNavigationWindow menuGarageNavigationWindow = ResolveGarageWindow();
			if ((bool)menuGarageNavigationWindow && menuGarageNavigationWindow.TrySuspendInCarFirstPersonCamera())
			{
				_garageCameraSuspended = true;
				return;
			}
			CarCamerasController carCamerasController = ResolveCarCamerasController();
			if ((bool)carCamerasController && carCamerasController.IsFirstPersonView && (bool)V.Car)
			{
				carCamerasController.SetCamera(1, V.Car.transform, newTarget: true, temporary: true);
				_carCameraSuspended = true;
			}
		}

		private void RestoreGarageCamera()
		{
			if (_garageCameraSuspended)
			{
				_garageCameraSuspended = false;
				MenuGarageNavigationWindow menuGarageNavigationWindow = ResolveGarageWindow();
				if ((bool)menuGarageNavigationWindow)
				{
					menuGarageNavigationWindow.RestoreInCarFirstPersonCamera();
				}
			}
		}

		private void RestoreCarCamera()
		{
			if (!_carCameraSuspended)
			{
				return;
			}
			_carCameraSuspended = false;
			if (!FlagsStorage.OutOfCar && (bool)V.Car)
			{
				CarCamerasController carCamerasController = ResolveCarCamerasController();
				if ((bool)carCamerasController && V.Car.TryGetComponent<DriverController>(out var component) && (bool)component.camInsideView)
				{
					carCamerasController.SetCamera(0, component.camInsideView.transform, newTarget: true, temporary: true);
				}
			}
		}

		private bool IsLocalDriverPerson()
		{
			if (!_personAnimationsController || !_personAnimationsController.transform.parent)
			{
				return false;
			}
			return _personAnimationsController.transform.parent.gameObject == V.DriverPerson;
		}

		private NewMoveController ResolveMoveController()
		{
			if (!V.DriverPerson)
			{
				return null;
			}
			V.DriverPerson.TryGetComponent<NewMoveController>(out var component);
			return component;
		}

		private CarCamerasController ResolveCarCamerasController()
		{
			if (!V.CoreGameManager)
			{
				return null;
			}
			return V.CoreGameManager.carCamerasController;
		}

		private MenuGarageNavigationWindow ResolveGarageWindow()
		{
			if (!FlagsStorage.InGarageFree && !FlagsStorage.InGarageMenu)
			{
				return null;
			}
			if (!V.CoreGameManager || !V.CoreGameManager.ManagerUI)
			{
				return null;
			}
			return V.CoreGameManager.ManagerUI.GetWindow("MenuControllerHUD") as MenuGarageNavigationWindow;
		}
	}
}
