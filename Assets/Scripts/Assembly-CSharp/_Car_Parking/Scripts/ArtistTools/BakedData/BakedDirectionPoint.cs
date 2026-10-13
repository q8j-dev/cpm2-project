using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.ArtistTools.Enums;

namespace _Car_Parking.Scripts.ArtistTools.BakedData
{
	[Serializable]
	public class BakedDirectionPoint
	{
		public BakedAnimSpeedType SpeedType;

		public HitDirection Direction;

		public List<BakedPointData> BakedPoints = new List<BakedPointData>();
	}
}
