using System;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Steamworks;
using Steamworks.Data;
using VContainer.Unity;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Steam.Purchase
{
	public class SteamPurchaseService : ISteamPurchaseService, IInitializable, IDisposable
	{
		private const string TRY_BUY_FN = "TryBuySteamItem24_1";

		private const int MAX_FIND_ATTEMPTS = 30;

		private const int FIND_DELAY_MS = 1000;

		private const int OVERLAY_OPEN_TIMEOUT_MS = 5000;

		private const int OVERLAY_CONFIRM_TIMEOUT_MS = 180000;

		private const int WATCHDOG_POLL_MS = 500;

		private bool overlayOpened;

		private bool overlayClosed;

		private bool callbackInProgress;

		private bool callbackFired;

		private readonly ISteamInventoryService steamInventoryService;

		private readonly ISteamPurchaseGate steamPurchaseGate;

		private SteamPurchaseData lastSteamPurchaseData;

		private Func<SteamPurchaseData, UniTask<bool>> pendingApplyHandler;

		private bool isPurchasing;

		public bool IsPurchasing => isPurchasing;

		public SteamPurchaseService(ISteamInventoryService steamInventoryService, ISteamPurchaseGate steamPurchaseGate)
		{
			this.steamInventoryService = steamInventoryService;
			this.steamPurchaseGate = steamPurchaseGate;
		}

		public void Initialize()
		{
			SteamUser.OnMicroTxnAuthorizationResponse += OnMicroTxnAuthorizationResponseGet;
			SteamFriends.OnGameOverlayActivated += OnGameOverlayActivated;
		}

		public void Dispose()
		{
			SteamUser.OnMicroTxnAuthorizationResponse -= OnMicroTxnAuthorizationResponseGet;
			SteamFriends.OnGameOverlayActivated -= OnGameOverlayActivated;
		}

		private void OnGameOverlayActivated(bool active)
		{
			if (isPurchasing)
			{
				if (active)
				{
					overlayOpened = true;
				}
				else if (overlayOpened)
				{
					overlayClosed = true;
				}
			}
		}

		public async UniTask StartPurchase(int itemDefId, Func<SteamPurchaseData, UniTask<bool>> onApply)
		{
			if (!SteamManager.IsInitialized() || isPurchasing)
			{
				return;
			}
			if (steamPurchaseGate != null && !steamPurchaseGate.PurchasesAllowed)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("SteamPurchases.NotAvailable"), 3f);
				return;
			}
			isPurchasing = true;
			overlayOpened = false;
			overlayClosed = false;
			callbackInProgress = false;
			callbackFired = false;
			pendingApplyHandler = onApply;
			ManagerUIHelper.ShowWaitingWindow(set: true, backButtonState: false, alwaysOnTop: true, "", updateBackButton: false).Forget();
			try
			{
				InventoryDef def = SteamInventory.FindDefinition(itemDefId);
				if (def == null)
				{
					SteamInventory.LoadItemDefinitions();
					await UniTask.Delay(500);
					def = SteamInventory.FindDefinition(itemDefId);
				}
				if (def == null)
				{
					Cleanup();
					return;
				}
				InventoryPurchaseResult? result = await SteamInventory.StartPurchaseAsync(new InventoryDef[1] { def });
				await UniTask.Delay(1000);
				if (!result.HasValue)
				{
					Cleanup();
					return;
				}
				lastSteamPurchaseData = new SteamPurchaseData
				{
					Itemdefid = itemDefId.ToString(),
					LocalPrice = def.LocalPrice,
					OrderID = result.Value.OrderID.ToString(),
					TransactionID = result.Value.TransID.ToString()
				};
				int elapsed = 0;
				while (isPurchasing && !overlayOpened && elapsed < 5000)
				{
					await UniTask.Delay(500);
					elapsed += 500;
				}
				if (isPurchasing && !overlayOpened)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("DebugAccountData.SomethingWentWrong"), 3f);
					Cleanup();
					return;
				}
				elapsed = 0;
				while (isPurchasing && !callbackFired && !overlayClosed && elapsed < 180000)
				{
					await UniTask.Delay(500);
					elapsed += 500;
				}
				int grace = 0;
				while (isPurchasing && !callbackFired && grace < 1500)
				{
					await UniTask.Delay(500);
					grace += 500;
				}
				if (isPurchasing && !callbackFired)
				{
					Cleanup();
				}
			}
			catch (Exception)
			{
				Cleanup();
			}
		}

		private void Cleanup()
		{
			if (overlayOpened && !overlayClosed)
			{
				SteamManager.ReleaseOverlayInputBlock();
			}
			isPurchasing = false;
			pendingApplyHandler = null;
			lastSteamPurchaseData = null;
			ManagerUIHelper.CloseWaitingWindow();
		}

		private void OnMicroTxnAuthorizationResponseGet(AppId appId, ulong orderId, bool authorized)
		{
			OnMicroTxnAuthorizationResponseGetAsync(authorized).Forget();
		}

		private async UniTask OnMicroTxnAuthorizationResponseGetAsync(bool authorized)
		{
			callbackFired = true;
			SteamPurchaseData data = lastSteamPurchaseData;
			Func<SteamPurchaseData, UniTask<bool>> handler = pendingApplyHandler;
			if (!authorized || data == null || handler == null)
			{
				Cleanup();
				return;
			}
			callbackInProgress = true;
			try
			{
				await CallTryBuy(data.TransactionID, data.Itemdefid);
				ManagerUIHelper.ShowWaitingWindow(set: true, backButtonState: false, alwaysOnTop: true, "", updateBackButton: false).Forget();
				await steamInventoryService.Refresh();
				int defId = int.Parse(data.Itemdefid);
				InventoryItem? inventoryItem = await FindInventoryItem(defId);
				if (!inventoryItem.HasValue)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("DebugAccountData.SomethingWentWrong"), 3f);
					return;
				}
				data.Name = inventoryItem?.Def.Name;
				await handler(data);
			}
			catch (Exception)
			{
			}
			finally
			{
				callbackInProgress = false;
				Cleanup();
			}
		}

		private async UniTask CallTryBuy(string transactionId, string itemdefid = null)
		{
			try
			{
				TryBuySteamItemPayload value = new TryBuySteamItemPayload(transactionId, itemdefid);
				await new FirebaseWrappedFunctionFactory().Create("TryBuySteamItem24_1").CallAsyncResult<object>(JsonConvert.SerializeObject(value));
			}
			catch (Exception)
			{
			}
		}

		private async UniTask<InventoryItem?> FindInventoryItem(int defId)
		{
			for (int attempt = 0; attempt < 30; attempt++)
			{
				InventoryResult? result = await steamInventoryService.Refresh();
				await UniTask.Delay(1000);
				if (!result.HasValue)
				{
					continue;
				}
				InventoryItem[] items = result.Value.GetItems();
				if (items == null)
				{
					continue;
				}
				InventoryItem[] array = items;
				for (int i = 0; i < array.Length; i++)
				{
					InventoryItem value = array[i];
					if (value.DefId == defId)
					{
						return value;
					}
				}
			}
			return null;
		}
	}
}
