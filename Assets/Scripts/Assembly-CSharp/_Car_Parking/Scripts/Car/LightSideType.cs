using System;

namespace _Car_Parking.Scripts.Car
{
	[Flags]
	public enum LightSideType
	{
		None = 0,
		Left = 1,
		Right = 2,
		Both = Left | Right
	}
}
