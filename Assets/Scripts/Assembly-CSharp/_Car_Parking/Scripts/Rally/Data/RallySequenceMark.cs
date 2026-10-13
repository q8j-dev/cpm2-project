using System;

namespace _Car_Parking.Scripts.Rally.Data
{
	[Serializable]
	public enum RallySequenceMark
	{
		None = 0,
		Caution = 1,
		WarningInside = 2,
		WarningOutside = 3,
		DontCut = 4,
		Cut = 5,
		Brake = 6,
		HandBrake = 7,
		KeepIn = 8,
		KeepOut = 9,
		KeepLeft = 10,
		KeepMiddle = 11,
		KeepRight = 12,
		BadCamber = 13,
		Blind = 14,
		Hidden = 15,
		Slippery = 16,
		Maybe = 17,
		Opens = 18,
		Tightens = 19,
		OverCrest = 20,
		OverJump = 21,
		ThroughDip = 22,
		Long = 23,
		VeryLong = 24,
		Short = 25,
		Early = 26,
		Late = 27,
		Narrows = 28,
		Widens = 29,
		Suddden = 30,
		Kinks = 31
	}
}
