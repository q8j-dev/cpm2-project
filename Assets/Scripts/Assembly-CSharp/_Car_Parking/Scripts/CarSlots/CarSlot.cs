using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.CarSlots
{
	[Preserve]
	public class CarSlot
	{
		public int ID { get; set; }

		public string CarFilePath { get; set; }

		public bool IsEmpty => string.IsNullOrEmpty(CarFilePath);

		public CarSlot()
		{
			ID = -1;
			CarFilePath = "";
		}

		public CarSlot(int id)
		{
			ID = id;
			CarFilePath = "";
		}

		public CarSlot(int id, string carFilePath)
		{
			ID = id;
			CarFilePath = carFilePath;
		}
	}
}
