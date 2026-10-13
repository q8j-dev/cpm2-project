using System;

namespace _Car_Parking.Scripts.EventSystem
{
	[Serializable]
	public enum ZoneState
	{
		None = 0,
		Prepared = 1,
		Started = 2,
		Pause = 3,
		Cooldown = 4
	}
}
