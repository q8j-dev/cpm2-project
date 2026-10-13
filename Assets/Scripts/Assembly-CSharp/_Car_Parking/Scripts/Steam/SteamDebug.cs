using Cysharp.Threading.Tasks;
using DebugPagesUi;
using LunarConsolePlugin;
using Steamworks;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Steam
{
	public static class SteamDebug
	{
		private const int ITEM_TEST_ID = 100230;

		private static ISteamPurchaseService _steamPurchaseService;

		private static ISteamInventoryService _steamInventoryService;

		private static bool _enableToClearInventory;

		public static void Init(ISteamPurchaseService purchase, ISteamInventoryService inventory)
		{
			_steamPurchaseService = purchase;
			_steamInventoryService = inventory;
		}

		[DebugButton("Dev/")]
		public static void ShowLunarConsole()
		{
			LunarConsole.Show();
		}

		[DebugButton("Dev/")]
		public static void HideLunarConsole()
		{
			LunarConsole.Hide();
		}

		[DebugButton("Steam/Overlay/")]
		public static void OverlayTest()
		{
			SteamFriends.OpenWebOverlay("https://www.youtube.com/watch?v=xvFZjo5PgG0");
		}

		[DebugButton("Steam/Inventory/")]
		public static void ClearInventoryTest()
		{
			if (_enableToClearInventory)
			{
				_steamInventoryService.DeleteAllInventoryItems().Forget();
			}
		}

		[DebugBoolHandler("Steam/Inventory/", false)]
		public static void ToggleEnableClearInventory(bool value)
		{
			_enableToClearInventory = value;
		}

		[DebugButton("Steam/Inventory/")]
		public static void PrintAllInventoryItems()
		{
			PrintAllInventoryItemAsync().Forget();
		}

		public static async UniTaskVoid PrintAllInventoryItemAsync()
		{
			if (!SteamManager.IsInitialized())
			{
				return;
			}
			InventoryResult? inventoryResult = await SteamInventory.GetAllItemsAsync();
			if (inventoryResult.HasValue)
			{
				InventoryItem[] items = inventoryResult.Value.GetItems();
				for (int i = 0; i < items.Length; i++)
				{
					_ = ref items[i];
				}
			}
		}

		[DebugButton("Steam/Inventory/")]
		public static void GenerateItemTest()
		{
			SteamInventory.GenerateItemAsync(SteamInventory.FindDefinition(100230), 1);
		}

		[DebugButton("Steam/Inventory/")]
		public static async UniTaskVoid ConsumeItemTestAsync()
		{
			InventoryResult? inventoryResult = await SteamInventory.GetAllItemsAsync();
			if (!inventoryResult.HasValue)
			{
				return;
			}
			InventoryItem[] items = inventoryResult.Value.GetItems();
			InventoryItem[] array = items;
			for (int i = 0; i < array.Length; i++)
			{
				InventoryItem inventoryItem = array[i];
				if (inventoryItem.DefId == 3)
				{
					await inventoryItem.ConsumeAsync(inventoryItem.Quantity);
				}
			}
		}

		[DebugButton("Steam/Backend/")]
		public static void CheckUserAgent()
		{
			CheckUserAgentAsync().Forget();
		}

		private static async UniTaskVoid CheckUserAgentAsync()
		{
			await new FirebaseWrappedFunctionFactory().Create("CheckUserAgent23_1", 30).CallAsyncResult<string>();
		}
	}
}
