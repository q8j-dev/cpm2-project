using System;

namespace CPMEngine
{
	[Flags]
	public enum GearboxType
	{
		GearRwd = 1,
		GearFwd = 2,
		GearAwd = 4,
		Gear4Wd = 8,
		Gear4Wdr1 = 0x10,
		Gear4Wdr2 = 0x20,
		Gear6Wd = 0x40,
		Gear6Wdr2 = 0x80,
		Gear8Wd = 0x100,
		Gear0Wd = 0x200
	}
}
