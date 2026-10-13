using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.VisualTuning.Wheels
{
	[CreateAssetMenu(fileName = "WheelDiskTexturesConfig", menuName = "Wheel/WheelDiskTexturesConfig", order = 21)]
	public class WheelDiskTexturesConfig : ScriptableObject
	{
		public List<TextureData> AllTextureData = new List<TextureData>();

		public void TrySetAllTextureData(WheelDiskData[] allWheels)
		{
			if (AllTextureData.Count == 0)
			{
				SetAllTextureData(allWheels);
			}
		}

		public void ForceSetAllTextureData(WheelDiskData[] allWheels)
		{
			AllTextureData.Clear();
			SetAllTextureData(allWheels);
		}

		private void SetAllTextureData(WheelDiskData[] allWheels)
		{
		}
	}
}
