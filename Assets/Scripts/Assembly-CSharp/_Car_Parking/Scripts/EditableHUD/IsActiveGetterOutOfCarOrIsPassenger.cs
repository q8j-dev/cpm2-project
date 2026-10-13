using System;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class IsActiveGetterOutOfCarOrIsPassenger : EditableHUDElementIsActiveGetter
	{
		public override bool IsActive
		{
			get
			{
				if (FlagsStorage.OutOfCar)
				{
					return true;
				}
				return FlagsStorage.IsPassenger;
			}
		}

		public override void Initialization()
		{
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
			V.OnIsPassengerChanged = (Action<bool>)Delegate.Remove(V.OnIsPassengerChanged, new Action<bool>(IsPassengerChangedCallback));
			V.OnIsPassengerChanged = (Action<bool>)Delegate.Combine(V.OnIsPassengerChanged, new Action<bool>(IsPassengerChangedCallback));
		}

		protected override void OnDestroy()
		{
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
			V.OnIsPassengerChanged = (Action<bool>)Delegate.Remove(V.OnIsPassengerChanged, new Action<bool>(IsPassengerChangedCallback));
			base.OnDestroy();
		}

		private void OutOfCarChangedCallback(bool value)
		{
			OnOnIsActiveChangedEvent();
		}

		private void IsPassengerChangedCallback(bool value)
		{
			OnOnIsActiveChangedEvent();
		}
	}
}
