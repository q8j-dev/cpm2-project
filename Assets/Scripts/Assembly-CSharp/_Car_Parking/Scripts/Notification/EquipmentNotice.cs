using System;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.Notification
{
	[Serializable]
	[Preserve]
	public class EquipmentNotice : INotice
	{
		public int Type;

		public int Index;

		public int Gender;

		public EquipmentNotice(int type, int index, int gender)
		{
			Type = type;
			Index = index;
			Gender = gender;
		}
	}
}
