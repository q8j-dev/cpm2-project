using CPM.Connection;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Wallet.PurchaseItemProviders
{
	[Preserve]
	public class PurchaseItemService
	{
		private readonly CurrencyManagerController _currencyManagerController;

		private readonly IConnectionManager _connectionManager;

		private readonly IWalletService _walletService;

		private readonly PurchaseItemsCollection _purchaseItemsCollection;

		private PurchaseItemData _lastPurchaseItem;

		public PurchaseItemService(CurrencyManagerController currencyManagerController, IConnectionManager connectionManager, IWalletService walletService, PurchaseItemsCollection purchaseItemsCollection)
		{
			_purchaseItemsCollection = purchaseItemsCollection;
			_walletService = walletService;
			_connectionManager = connectionManager;
			_currencyManagerController = currencyManagerController;
		}

		public ObscuredInt GetPrice(PurchaseItemType itemType)
		{
			if (_lastPurchaseItem != null && _lastPurchaseItem.purchaseItemType == itemType)
			{
				return _lastPurchaseItem.Price;
			}
			return _purchaseItemsCollection.GetPurchaseItemData(itemType)?.Price ?? ((ObscuredInt)0);
		}

		public async UniTask<PurchaseItemResult> TryPurchase(PurchaseItemType itemType, object payLoad = null, AdvancedInt overridePrice = null)
		{
			_lastPurchaseItem = ((_lastPurchaseItem != null && _lastPurchaseItem.purchaseItemType == itemType) ? _lastPurchaseItem : _purchaseItemsCollection.GetPurchaseItemData(itemType));
			if (_lastPurchaseItem == null)
			{
				return new PurchaseItemResult
				{
					Status = new AdvancedBoolean(v: false),
					StatusCode = -1
				};
			}
			ObscuredInt value = _lastPurchaseItem.Price;
			if (overridePrice != null)
			{
				overridePrice.Get(out value);
			}
			if (!_currencyManagerController.IsEnoughCoins(value, withMessage: true))
			{
				return new PurchaseItemResult
				{
					Status = new AdvancedBoolean(v: false),
					StatusCode = -2
				};
			}
			_connectionManager.CorrectHash.Get(out var value2);
			if (!value2)
			{
				return new PurchaseItemResult
				{
					Status = new AdvancedBoolean(v: false),
					StatusCode = -3
				};
			}
			ItemData data = new ItemData(_lastPurchaseItem.PurchaseId, payLoad);
			WalletOperationResult walletOperationResult = await _walletService.PurchaseItem(data);
			if (walletOperationResult == null)
			{
				return new PurchaseItemResult
				{
					Status = new AdvancedBoolean(v: false),
					StatusCode = 0
				};
			}
			if (walletOperationResult.IsSuccess())
			{
				_currencyManagerController.UpdateCoinsFromDatabase().Forget();
			}
			return new PurchaseItemResult
			{
				Status = new AdvancedBoolean(walletOperationResult.IsSuccess()),
				StatusCode = walletOperationResult.Success,
				Value = walletOperationResult.Value,
				Data = walletOperationResult.Data
			};
		}

		public async UniTask<PurchaseItemResult> RestorePurchase(PurchaseItemType itemType, object payLoad = null)
		{
			_lastPurchaseItem = ((_lastPurchaseItem != null && _lastPurchaseItem.purchaseItemType == itemType) ? _lastPurchaseItem : _purchaseItemsCollection.GetPurchaseItemData(itemType));
			if (_lastPurchaseItem == null)
			{
				return new PurchaseItemResult
				{
					Status = new AdvancedBoolean(v: false),
					StatusCode = -1
				};
			}
			_connectionManager.CorrectHash.Get(out var value);
			if (!value)
			{
				return new PurchaseItemResult
				{
					Status = new AdvancedBoolean(v: false),
					StatusCode = -3
				};
			}
			ItemData data = new ItemData(_lastPurchaseItem.PurchaseId, payLoad);
			WalletOperationResult walletOperationResult = await _walletService.RestorePurchase(data);
			if (walletOperationResult == null)
			{
				return new PurchaseItemResult
				{
					Status = new AdvancedBoolean(v: false),
					StatusCode = 0
				};
			}
			return new PurchaseItemResult
			{
				Status = new AdvancedBoolean(walletOperationResult.IsSuccess()),
				StatusCode = walletOperationResult.Success
			};
		}
	}
}
