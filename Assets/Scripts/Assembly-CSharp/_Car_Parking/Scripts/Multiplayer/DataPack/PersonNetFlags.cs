using System;

namespace _Car_Parking.Scripts.Multiplayer.DataPack
{
	[Flags]
	public enum PersonNetFlags : byte
	{
		None = 0,
		Grounded = 1,
		Jumping = 2,
		Crouching = 4,
		Sliding = 8,
		Ragdoll = 0x10,
		Starting = 0x20,
		QuickTurning = 0x40,
		QuickTurnLeft = 0x80
	}
}
