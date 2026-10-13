using System;

namespace _Car_Parking.Scripts.EditableHUD
{
	public class IsActiveGetterOutOfCar : EditableHUDElementIsActiveGetter
	{
		public override bool IsActive => FlagsStorage.OutOfCar;

		public override void Initialization()
		{
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Combine(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
		}

		protected override void OnDestroy()
		{
			V.OnOutOfCarChanged = (Action<bool>)Delegate.Remove(V.OnOutOfCarChanged, new Action<bool>(OutOfCarChangedCallback));
			base.OnDestroy();
		}

		private void OutOfCarChangedCallback(bool value)
		{
			OnOnIsActiveChangedEvent();
		}
	}
}
