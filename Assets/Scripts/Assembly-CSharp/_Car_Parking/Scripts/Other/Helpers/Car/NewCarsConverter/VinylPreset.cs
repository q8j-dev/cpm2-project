using System;
using System.Collections.Generic;
using CPM.Database;
using DataEntities.Vinyl;
using UnityEngine;
using _Car_Parking.Scripts.CommonUtility;

namespace _Car_Parking.Scripts.Other.Helpers.Car.NewCarsConverter
{
	[Serializable]
	public class VinylPreset
	{
		public uint Color;

		public List<VinylData> AllVinyl;

		public VinylPreset(VynilOneCar presetSource, Color carColor)
		{
			AllVinyl = new List<VinylData>(presetSource.allVynils);
			Color = BitMaskColorPacker.PackColor(carColor);
		}
	}
}
