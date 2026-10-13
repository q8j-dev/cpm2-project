using System;
using System.Collections.Generic;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.EventSystem.Data.Response
{
	[Serializable]
	[Preserve]
	public class CollectablesEventData : EvenDataBase
	{
		public List<int> Items;

		public int ItemCount;

		public void AddClaimed(int claimed)
		{
			if (!HaveClaimed(claimed))
			{
				Items.Add(claimed);
			}
		}

		public bool HaveClaimed(int claimed)
		{
			if (Items != null)
			{
				return Items.Contains(claimed);
			}
			return false;
		}
	}
}
