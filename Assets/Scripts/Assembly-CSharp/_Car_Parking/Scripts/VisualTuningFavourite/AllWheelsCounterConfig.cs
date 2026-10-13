using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.VisualTuningFavourite
{
	[CreateAssetMenu(fileName = "AllWheelsCounterConfig", menuName = "Scriptable/AllWheelsCounterConfig", order = 15)]
	public class AllWheelsCounterConfig : ScriptableObject
	{
		public List<int> AllWheelsIndexes = new List<int>();

		public void TrySetAllWheelsIndexes(WheelDiskData[] allWheels)
		{
			if (AllWheelsIndexes.Count == 0)
			{
				SetAllWheelsIndexes(allWheels);
			}
		}

		public void ForceSetAllWheelsIndexes(WheelDiskData[] allWheels)
		{
			AllWheelsIndexes.Clear();
			SetAllWheelsIndexes(allWheels);
		}

		private void SetAllWheelsIndexes(WheelDiskData[] allWheels)
		{
			foreach (WheelDiskData wheelDiskData in allWheels)
			{
				AllWheelsIndexes.Add(wheelDiskData.index);
			}
		}
	}
}
