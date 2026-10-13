namespace _Car_Parking.Scripts.Inventory
{
	public static class InventoryExtensions
	{
		public static InventoryData Read(ES3File file)
		{
			return file.Load<InventoryData>("InventoryData");
		}

		public static EngineKitsInventoryData ReadEnginesSetData(ES3File file)
		{
			return file.Load<EngineKitsInventoryData>("InventoryEnginesData");
		}
	}
}
