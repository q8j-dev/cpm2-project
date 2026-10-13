using System.Collections.Generic;
using UnityEngine;

namespace CPM.CarVisualModel
{
	[CreateAssetMenu(fileName = "CarModelLibrary", menuName = "CarParking/Configs/CarModelLibrary")]
	public sealed class CarModelLibrary : ScriptableObject
	{
		public List<CarModelEntry> Cars = new List<CarModelEntry>();

		public bool TryGet(int carId, out CarModelEntry entry)
		{
			if (Cars != null)
			{
				for (int i = 0; i < Cars.Count; i++)
				{
					if (Cars[i] != null && Cars[i].CarId == carId)
					{
						entry = Cars[i];
						return true;
					}
				}
			}
			entry = null;
			return false;
		}
	}
}
