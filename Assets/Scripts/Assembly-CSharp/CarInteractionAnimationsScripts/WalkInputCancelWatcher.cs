using UnityEngine;
using UnityEngine.InputSystem;
using _Car_Parking.Scripts;

namespace CarInteractionAnimationsScripts
{
	public class WalkInputCancelWatcher
	{
		private const float MoveThreshold = 0.2f;

		private readonly PersonAnimationsController _personAnimationsController;

		private readonly GameObject _person;

		private bool _armed;

		public WalkInputCancelWatcher(PersonAnimationsController personAnimationsController)
		{
			_personAnimationsController = personAnimationsController;
			_person = (personAnimationsController.transform.parent ? personAnimationsController.transform.parent.gameObject : null);
		}

		public bool TryCancelSeat()
		{
			if (!_personAnimationsController || !_person || _person != V.DriverPerson)
			{
				return false;
			}
			SitGetOutData sitGetOutData = _personAnimationsController.SitGetOutCarSystem.SitGetOutData;
			if (sitGetOutData == null || sitGetOutData.IsCanceled || sitGetOutData.IsForceSeatIntoCar || sitGetOutData.IsForceGetOutCar || !sitGetOutData.IsSittingIntoCarAnimate)
			{
				return false;
			}
			bool flag = IsWalkInputPressed();
			if (!_armed)
			{
				_armed = !flag;
				return false;
			}
			if (!flag)
			{
				return false;
			}
			_personAnimationsController.SitGetOutCarSystem.CarAnimationStopper.CancelSeat();
			return true;
		}

		private bool IsWalkInputPressed()
		{
			if (InputStorage.LeftJoyIsDrag)
			{
				return true;
			}
			if (new Vector2(InputStorage.LeftJoyInputX, InputStorage.LeftJoyInputY).sqrMagnitude > 0.040000003f)
			{
				return true;
			}
			Keyboard current = Keyboard.current;
			if (current != null)
			{
				if (!current.wKey.isPressed && !current.sKey.isPressed && !current.aKey.isPressed && !current.dKey.isPressed && !current.upArrowKey.isPressed && !current.downArrowKey.isPressed && !current.leftArrowKey.isPressed)
				{
					return current.rightArrowKey.isPressed;
				}
				return true;
			}
			return false;
		}
	}
}
