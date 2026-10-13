using System;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	[Flags]
	public enum RebindDevices
	{
		None = 0,
		KeyboardMouse = 1,
		Gamepad = 2,
		Wheel = 4
	}
}
