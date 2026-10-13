using System;
using System.Collections.Generic;
using DataEntities.Vinyl;

namespace CPM.Database
{
	[Serializable]
	public class VynilOneCar
	{
		public List<VinylData> allVynils;

		public int CarID;

		public string CarInstanceID;

		public VynilOneCar()
		{
		}

		public VynilOneCar(int carID)
		{
			CarID = carID;
		}
	}
}
