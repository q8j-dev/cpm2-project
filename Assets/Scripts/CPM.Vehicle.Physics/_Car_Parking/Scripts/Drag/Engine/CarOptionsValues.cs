using System;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public struct CarOptionsValues
	{
		public int AbsLevel;

		public int TrcLevel;

		public int EspLevel;

		public bool Lc;

		public bool MTAssist;

		public bool ArcadeMode;

		public bool AnalogClutch;

		public bool AnalogThrottle;

		public bool AnalogBrake;

		public bool Abs
		{
			get
			{
				return AbsLevel > 0;
			}
			set
			{
				AbsLevel = (value ? 5 : 0);
			}
		}

		public bool Trc
		{
			get
			{
				return TrcLevel > 0;
			}
			set
			{
				TrcLevel = (value ? 5 : 0);
			}
		}

		public bool Esp
		{
			get
			{
				return EspLevel > 0;
			}
			set
			{
				EspLevel = (value ? 5 : 0);
			}
		}
	}
}
