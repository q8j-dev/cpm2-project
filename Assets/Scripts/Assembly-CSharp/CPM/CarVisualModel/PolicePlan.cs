using System;
using System.Collections.Generic;
using CPM.Database;
using _Car_Parking.Scripts.Police;

namespace CPM.CarVisualModel
{
	public static class PolicePlan
	{
		public static List<PoliceKitRequest> BuildLoadList(PoliceData police)
		{
			List<PoliceKitRequest> list = new List<PoliceKitRequest>();
			if (police == null || !police.Installed || police.InstalledBodyKits == null)
			{
				return list;
			}
			foreach (PoliceBodyKitType value in Enum.GetValues(typeof(PoliceBodyKitType)))
			{
				int index = police.InstalledBodyKits.GetIndex(value);
				if (index >= 0)
				{
					list.Add(new PoliceKitRequest(value, index));
				}
			}
			return list;
		}
	}
}
