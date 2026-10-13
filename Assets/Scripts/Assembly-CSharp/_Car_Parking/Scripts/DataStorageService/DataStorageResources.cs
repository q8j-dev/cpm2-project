using System;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Inventory;

namespace _Car_Parking.Scripts.DataStorageService
{
	[Serializable]
	public class DataStorageResources
	{
		public PlayerStorage PlayerStorage;

		public InventoryData InventoryData;

		public GarageSpotCollection GarageSpotCollection;

		public EngineKitsInventoryData EngineKitsInventoryData;
	}
}
