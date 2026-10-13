using System;

namespace _Car_Parking.CarMaintenance
{
	[Flags]
	public enum ServiceMessages
	{
		None = 0,
		NotEnoughMoney = 1,
		CarIsClean = 2
	}
}
