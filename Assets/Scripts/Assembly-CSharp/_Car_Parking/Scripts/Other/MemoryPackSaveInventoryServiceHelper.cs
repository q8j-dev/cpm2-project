namespace _Car_Parking.Scripts.Other
{
	public static class MemoryPackSaveInventoryServiceHelper
	{
		public static MemoryPackDictionaryBase GetMemoryPackDictionary(string funcName)
		{
			return funcName switch
			{
				"SaveEngineInventory24_1" => new MemoryPackDictionarySaveEngineInventory(), 
				"SavePartsInventory24_1" => new MemoryPackSavePartsInventory(), 
				"SavePlateVinyls24_1" => new MemoryPackDictionarySavePlateVinyls(), 
				"SavePlayerRecords24_1" => new MemoryPackDictionarySavePlayerRecords(), 
				"SaveSlotsCollection24_1" => new MemoryPackDictionarySaveSlotsCollection(), 
				"SaveWalletData24_1" => new MemoryPackDictionarySaveWalletData(), 
				"SaveFriends24_1" => null, 
				_ => null, 
			};
		}
	}
}
