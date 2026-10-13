using System;
using Cysharp.Threading.Tasks;
using Steamworks;

namespace _Car_Parking.Scripts.Steam
{
	public class SteamInventoryService : ISteamInventoryService
	{
		private const int MAX_FIND_ATTEMPTS = 30;

		public async UniTask<InventoryResult?> Refresh()
		{
			if (!SteamManager.IsInitialized())
			{
				return null;
			}
			return await SteamInventory.GetAllItemsAsync();
		}

		private async UniTask<InventoryItem[]> GetInventoryItemsWithRetry()
		{
			for (int i = 0; i < 30; i++)
			{
				InventoryResult? result = await Refresh();
				await UniTask.Delay(1000);
				if (result.HasValue)
				{
					InventoryItem[] items = result.Value.GetItems();
					if (items != null && items.Length != 0)
					{
						return items;
					}
				}
			}
			return Array.Empty<InventoryItem>();
		}

		public async UniTask DeleteAllInventoryItems()
		{
			if (!SteamManager.IsInitialized())
			{
				return;
			}
			InventoryResult? inventoryResult = await SteamInventory.GetAllItemsAsync();
			if (inventoryResult.HasValue)
			{
				InventoryItem[] items = inventoryResult.Value.GetItems();
				InventoryItem[] array = items;
				for (int i = 0; i < array.Length; i++)
				{
					InventoryItem inventoryItem = array[i];
					await inventoryItem.ConsumeAsync(inventoryItem.Quantity);
				}
			}
		}

		public InventoryDef GetInventoryDef(int itemDefId)
		{
			if (SteamManager.IsInitialized())
			{
				return SteamInventory.FindDefinition(itemDefId);
			}
			return null;
		}
	}
}
