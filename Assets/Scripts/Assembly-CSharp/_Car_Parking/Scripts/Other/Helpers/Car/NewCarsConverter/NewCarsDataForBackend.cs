using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Other.Helpers.Car.NewCarsConverter
{
	[Serializable]
	public class NewCarsDataForBackend
	{
		public int CarId;

		public List<VinylPreset> VinylPresets;

		public List<BodyKitDataBuilder.BodyKitData> BodyKitDatas;
	}
}
