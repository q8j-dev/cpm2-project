using System;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	[Flags]
	public enum ChangeFields
	{
		None = 0,
		Path = 1,
		Interactions = 2,
		Processors = 4
	}
}
