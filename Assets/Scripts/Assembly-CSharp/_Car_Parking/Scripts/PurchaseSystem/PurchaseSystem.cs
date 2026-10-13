using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Connection;
using CPM.Database;
using CPM.PlayerData;
using Cysharp.Threading.Tasks;
using Steamworks;
using UnityEngine.Purchasing;
using UnityEngine.Scripting;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Steam;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[UnityEngine.Scripting.Preserve]
	public class PurchaseSystem : IPurchaseSystem, IInitializable, IDisposable
	{
		private const float DelayToCheckNextHash = 2f;

		private const float DelayToCheckAccountID = 1f;

		private readonly PlayerDataStorage _playerDataStorage;

		private readonly CPM.Connection.IConnectionManager _connectionManager;

		private readonly ISteamPurchaseService _steamPurchaseService;

		private readonly ISteamPurchaseGate _steamPurchaseGate;

		private readonly ISteamItemCatalog _steamItemCatalog;

		private readonly ISteamItemApplier _steamItemApplier;

		private readonly IOfferSystem _offerSystem;

		private readonly RealPurchaseAnalytic _realPurchaseAnalytic;

		private readonly PurchaseItemsCollection _purchaseItemsCollection;

		private readonly PurchaseHandlersContainer _purchaseHandlersContainer;

		private readonly IObjectResolver _container;

		private readonly PurchaseAttemptIdStorage _attemptIdStorage;

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly IGuestAccountCreator _guestAccountCreator;

		private PurchaserStoreListener _storeListener;

		private bool _wasInitialized;

		private readonly List<PurchaseProduct> _purchaseProducts = new List<PurchaseProduct>();

		private readonly List<ProductSource> _sourcesReady = new List<ProductSource>();

		private CancellationTokenSource _mainCts;

		private InventoryDef[] _steamDefinitions;

		private readonly AllCarsInfo _allCarsInfo;

		public bool IsInitialized { get; private set; }

		public event Action OnInitializedPreCompletedEvent;

		public event Action OnInitializedCompletedEvent;

		public PurchaseSystem(PlayerDataStorage playerDataStorage, CPM.Connection.IConnectionManager connectionManager, RealPurchaseAnalytic realPurchaseAnalytic, PurchaseItemsCollection purchaseItemsCollection, IEnumerable<IPurchaseHandler> handlers, IObjectResolver container, FreeDriveProvider freeDriveProvider, IGuestAccountCreator guestAccountCreator, ISteamPurchaseService steamPurchaseService, ISteamItemCatalog steamItemCatalog, ISteamItemApplier steamItemApplier, AllCarsInfo allCarsInfo, IOfferSystem offerSystem, ISteamPurchaseGate steamPurchaseGate)
		{
			_playerDataStorage = playerDataStorage;
			_connectionManager = connectionManager;
			_realPurchaseAnalytic = realPurchaseAnalytic;
			_purchaseItemsCollection = purchaseItemsCollection;
			_container = container;
			_freeDriveProvider = freeDriveProvider;
			_guestAccountCreator = guestAccountCreator;
			_mainCts = new CancellationTokenSource();
			_purchaseHandlersContainer = new PurchaseHandlersContainer(handlers);
			_attemptIdStorage = new PurchaseAttemptIdStorage();
			_steamPurchaseService = steamPurchaseService;
			_steamPurchaseGate = steamPurchaseGate;
			_steamItemCatalog = steamItemCatalog;
			_steamItemApplier = steamItemApplier;
			_offerSystem = offerSystem;
			_allCarsInfo = allCarsInfo;
		}

		public void Initialize()
		{
			_storeListener = new PurchaserStoreListener(this, _freeDriveProvider);
			TryInitializePurchasing();
			InitializeAsync().Forget();
		}

		private async UniTask InitializeAsync()
		{
			await _container.Resolve<PurchaseSystemPurchaseProductsAdder>().InitAsync();
			LoadSteamPrices().Forget();
		}

		public void Dispose()
		{
			CommonExtensions.CancelAndDisposeToken(ref _mainCts);
		}

		private async UniTask LoadSteamPrices()
		{
			if (!SteamManager.IsInitialized())
			{
				return;
			}
			for (int i = 0; i < 15; i++)
			{
				if (SteamInventory.Definitions != null && SteamInventory.Definitions.Length != 0)
				{
					break;
				}
				if (_mainCts.IsCancellationRequested)
				{
					break;
				}
				SteamInventory.LoadItemDefinitions();
				await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, _mainCts.Token).SuppressCancellationThrow();
			}
			InventoryDef[] definitions = null;
			for (int i = 0; i < 15; i++)
			{
				if (_mainCts.IsCancellationRequested)
				{
					break;
				}
				definitions = await SteamInventory.GetDefinitionsWithPricesAsync();
				if (definitions != null && definitions.Length > 0)
				{
					break;
				}
				await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, _mainCts.Token).SuppressCancellationThrow();
			}
			if (definitions == null || definitions.Length <= 0)
			{
				InventoryDef[] definitions2 = SteamInventory.Definitions;
				int num = ((definitions2 != null) ? definitions2.Length : (-1));
				if (num > 0)
				{
					_ = $"Definitions loaded ({num}) but prices missing → item price_categories likely NOT PUBLISHED on Steamworks / app not configured for in-app purchasing.";
				}
				return;
			}
			_steamDefinitions = definitions;
			foreach (PurchaseItemData purchaseItem in _purchaseItemsCollection.PurchaseItems)
			{
				if (!purchaseItem.SteamItem)
				{
					continue;
				}
				PurchaseProduct purchaseProduct = GetPurchaseProduct(purchaseItem.PurchaseId);
				if (purchaseProduct == null)
				{
					continue;
				}
				InventoryDef inventoryDef = null;
				InventoryDef[] array = definitions;
				foreach (InventoryDef inventoryDef2 in array)
				{
					if (inventoryDef2.Id == purchaseItem.SteamItem.SteamDefId)
					{
						inventoryDef = inventoryDef2;
						break;
					}
				}
				if (!(inventoryDef == null))
				{
					purchaseProduct.LocalizedPrice = (float)inventoryDef.LocalPrice / 100f;
					purchaseProduct.LocalizedPriceText = inventoryDef.LocalPriceFormatted;
					purchaseProduct.IsoCurrencyCode = SteamInventory.Currency;
					string arg = (string.IsNullOrEmpty(inventoryDef.Name) ? "unknown" : inventoryDef.Name);
					purchaseProduct.StoreProductID = $"steam.{inventoryDef.Id}.{arg}";
					purchaseProduct.Initialized = true;
				}
			}
			IsInitialized = true;
			OnInitializedPreCompletedEvent?.Invoke();
			OnInitializedCompletedEvent?.Invoke();
		}

		public void AddContentToPurchaseBuilder(ConfigurationBuilder builder)
		{
			for (int i = 0; i < _purchaseProducts.Count; i++)
			{
				builder.AddProduct(_purchaseProducts[i].StoreProductID, _purchaseProducts[i].ProductType);
			}
		}

		public void OnInitialized(IStoreController storeController, IExtensionProvider extensions)
		{
			for (int i = 0; i < _purchaseProducts.Count; i++)
			{
				if (!_purchaseProducts[i].Initialized)
				{
					Product product = storeController.products.WithID(_purchaseProducts[i].StoreProductID);
					_purchaseProducts[i].LocalizedPrice = ((product != null) ? ((float)product.metadata.localizedPrice) : 0f);
					_purchaseProducts[i].LocalizedPriceText = ((product != null) ? product.metadata.localizedPriceString : "");
					_purchaseProducts[i].IsoCurrencyCode = ((product != null) ? product.metadata.isoCurrencyCode : "");
					_purchaseProducts[i].Initialized = product?.availableToPurchase ?? false;
				}
			}
			IsInitialized = true;
			OnInitializedPreCompletedEvent?.Invoke();
			OnInitializedCompletedEvent?.Invoke();
		}

		public void AddPurchaseProducts(List<PurchaseProduct> purchaseProducts, ProductSource productSource)
		{
			for (int i = 0; i < purchaseProducts.Count; i++)
			{
				if (purchaseProducts[i] != null && GetPurchaseProduct(purchaseProducts[i].ProductID) == null)
				{
					_purchaseProducts.Add(purchaseProducts[i]);
				}
			}
			_sourcesReady.Add(productSource);
			TryInitializePurchasing();
		}

		public PurchaseProduct GetPurchaseProduct(PurchaseItemType purchaseItemType)
		{
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(purchaseItemType);
			if (purchaseItemData == null)
			{
				return null;
			}
			return GetPurchaseProduct(purchaseItemData.PurchaseId);
		}

		public PurchaseProduct GetPurchaseProduct(string productID)
		{
			for (int i = 0; i < _purchaseProducts.Count; i++)
			{
				if (_purchaseProducts[i].ProductID == productID)
				{
					return _purchaseProducts[i];
				}
			}
			return null;
		}

		public void BuyProductID(PurchaseItemType purchaseItemType)
		{
			BuyProductID(GetPurchaseProduct(purchaseItemType));
		}

		public void BuyProductID(string productId)
		{
			BuyProductID(GetPurchaseProduct(productId));
		}

		private void BuyProductID(PurchaseProduct product)
		{
			if (product == null)
			{
				return;
			}
			if (SteamManager.IsInitialized())
			{
				if (_steamPurchaseService.IsPurchasing)
				{
					return;
				}
				if (_steamPurchaseGate != null && !_steamPurchaseGate.PurchasesAllowed)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("SteamPurchases.NotAvailable"), 3f);
					return;
				}
				OfferModel offer = _offerSystem.GetOffer(product.ProductID);
				if (offer != null && offer.Payload.FirstSteamItemDefId > 0 && offer.Payload.RewardIsCar)
				{
					BuySteamOffer(offer).Forget();
					return;
				}
				PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(product.ProductID);
				if ((bool)purchaseItemData?.SteamItem)
				{
					BuySteamItem(product, purchaseItemData.SteamItem).Forget();
					return;
				}
				OfferModel offer2 = _offerSystem.GetOffer(product.ProductID);
				if (offer2 != null && offer2.Payload.FirstSteamItemDefId > 0)
				{
					BuySteamOffer(offer2).Forget();
					return;
				}
			}
			TryBuyProductIDAsync(product.ProductID).Forget();
		}

		private async UniTask BuySteamOffer(OfferModel offer)
		{
			int steamDefId = offer.Payload.FirstSteamItemDefId;
			_offerSystem.SetCurrentOfferID(offer.ID);
			PurchaseProduct purchaseProduct = GetPurchaseProduct(offer.Payload.FirstBundleID);
			string attemptId = "";
			bool isNew = false;
			if (purchaseProduct != null)
			{
				attemptId = _attemptIdStorage.GetOrCreate(purchaseProduct.ProductID, out isNew);
			}
			if (isNew)
			{
				LogSteamPurchase(purchaseProduct, offer, PurchaseStatus.PURCHASE_INITIATED, "", PurchaseRealErrorOrigin.None, PurchaseRealErrorReason.None, steamDefId);
			}
			bool callbackFired = false;
			bool applied = false;
			await _steamPurchaseService.StartPurchase(steamDefId, async (SteamPurchaseData data) =>
			{
				callbackFired = true;
				if (purchaseProduct != null)
				{
					_attemptIdStorage.AddProcessPurchase(attemptId);
				}
				LogSteamPurchase(purchaseProduct, offer, PurchaseStatus.PURCHASE_SDK_CALLBACK_RECEIVED, data.TransactionID, PurchaseRealErrorOrigin.None, PurchaseRealErrorReason.None, steamDefId);
				OffersPurchaseHandler offersPurchaseHandler = _purchaseHandlersContainer.GetHandler(offer.Payload.FirstBundleID) as OffersPurchaseHandler;
				bool flag = offersPurchaseHandler != null;
				if (flag)
				{
					flag = await offersPurchaseHandler.ProcessSteamPurchaseAsync(offer, data);
				}
				bool flag2 = flag;
				LogSteamPurchase(purchaseProduct, offer, flag2 ? PurchaseStatus.PURCHASE_COMPLETED : PurchaseStatus.PURCHASE_CANCELED_BY_SYSTEM, data.TransactionID, (!flag2) ? PurchaseRealErrorOrigin.Backend : PurchaseRealErrorOrigin.None, (!flag2) ? PurchaseRealErrorReason.ClientProcessPurchaseAsync : PurchaseRealErrorReason.None, steamDefId);
				applied = flag2;
				return flag2;
			});
			if (purchaseProduct != null)
			{
				ClearAttemptIfResolved(purchaseProduct.ProductID, callbackFired, applied);
			}
		}

		private async UniTaskVoid BuySteamItem(PurchaseProduct product, SteamItem steamItem)
		{
			string attemptId = _attemptIdStorage.GetOrCreate(product.ProductID, out var isNew);
			if (isNew)
			{
				LogSteamPurchase(product, null, PurchaseStatus.PURCHASE_INITIATED, "", PurchaseRealErrorOrigin.None, PurchaseRealErrorReason.None, steamItem.SteamDefId);
			}
			bool callbackFired = false;
			bool applied = false;
			await _steamPurchaseService.StartPurchase(steamItem.SteamDefId, async (SteamPurchaseData data) =>
			{
				callbackFired = true;
				_attemptIdStorage.AddProcessPurchase(attemptId);
				data.AddValue = steamItem.AddValue.ToString();
				bool flag = await _steamItemApplier.Apply(steamItem, data);
				LogSteamPurchase(product, null, flag ? PurchaseStatus.PURCHASE_COMPLETED : PurchaseStatus.PURCHASE_FAILED, data.TransactionID, (!flag) ? PurchaseRealErrorOrigin.Client : PurchaseRealErrorOrigin.None, (!flag) ? PurchaseRealErrorReason.ClientProcessPurchaseAsync : PurchaseRealErrorReason.None, steamItem.SteamDefId);
				applied = flag;
				return flag;
			});
			ClearAttemptIfResolved(product.ProductID, callbackFired, applied);
		}

		private void ClearAttemptIfResolved(string productId, bool callbackFired, bool applied)
		{
			if (!callbackFired | applied)
			{
				_attemptIdStorage.Remove(productId);
			}
		}

		private void LogSteamPurchase(PurchaseProduct purchaseProduct, OfferModel offer, PurchaseStatus status, string transactionId, PurchaseRealErrorOrigin errorOrigin, PurchaseRealErrorReason errorReason, int itemDefId)
		{
			if (purchaseProduct != null)
			{
				purchaseProduct.ItemDefId = itemDefId;
				IPurchaseHandler handler = _purchaseHandlersContainer.GetHandler(purchaseProduct.ProductID);
				if (handler != null)
				{
					string attemptId = _attemptIdStorage.Get(purchaseProduct.ProductID);
					bool isRetry = _attemptIdStorage.IsRetry(attemptId);
					_realPurchaseAnalytic.Log(purchaseProduct, errorOrigin, errorReason, PurchaseRealErrorSdkReason.Unknown, isRetry, (offer != null) ? offer.ID.ToString() : handler.GetOfferId(purchaseProduct.ProductID), handler.GetItemIds(purchaseProduct.ProductID), transactionId, attemptId, status);
				}
			}
		}

		public void ProcessPurchase(PurchaseEventArgs args)
		{
			PurchaseProduct purchaseProductByStoreProductID = GetPurchaseProductByStoreProductID(args.purchasedProduct.definition.id);
			if (purchaseProductByStoreProductID != null)
			{
				ProcessPurchaseAsync(args, purchaseProductByStoreProductID, _mainCts.Token).Forget();
			}
		}

		public void PurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			PurchaseFailedInternalAsync(product, failureReason, _mainCts.Token).Forget();
		}

		private async UniTask PurchaseFailedInternalAsync(Product product, PurchaseFailureReason failureReason, CancellationToken cancellationToken)
		{
			while (!_playerDataStorage.HasAccountID)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
			PurchaseProduct purchaseProductByStoreProductID = GetPurchaseProductByStoreProductID(product.definition.id);
			if (purchaseProductByStoreProductID != null)
			{
				if (failureReason == PurchaseFailureReason.UserCancelled)
				{
					Log(purchaseProductByStoreProductID, PurchaseStatus.PURCHASE_CANCELED_BY_USER, "", PurchaseRealErrorOrigin.Store, failureReason.ToPurchaseRealErrorReason(), PurchaseRealErrorSdkReason.Unknown);
				}
				else
				{
					Log(purchaseProductByStoreProductID, PurchaseStatus.PURCHASE_FAILED, "", PurchaseRealErrorOrigin.Store, failureReason.ToPurchaseRealErrorReason(), failureReason.ToPurchaseRealErrorSdkReason());
				}
				_attemptIdStorage.Remove(purchaseProductByStoreProductID.ProductID);
				_purchaseHandlersContainer.GetHandler(purchaseProductByStoreProductID.ProductID)?.PurchaseFailedAsync(purchaseProductByStoreProductID, product, failureReason).Forget();
			}
		}

		private void TryInitializePurchasing()
		{
			if (!_wasInitialized && Enum.GetValues(typeof(ProductSource)).Length == _sourcesReady.Count)
			{
				InitializePurchasing();
			}
		}

		private void InitializePurchasing()
		{
			_wasInitialized = true;
			for (int i = 0; i < _purchaseProducts.Count; i++)
			{
				if (!_purchaseProducts[i].Initialized || string.IsNullOrEmpty(_purchaseProducts[i].StoreProductID) || !_purchaseProducts[i].StoreProductID.StartsWith("steam."))
				{
					_purchaseProducts[i].StoreProductID = _storeListener.StorePrefix;
					string text = (string.IsNullOrEmpty(_purchaseProducts[i].BundleSuffix) ? _purchaseProducts[i].ProductID : _purchaseProducts[i].BundleSuffix);
					if (!string.IsNullOrEmpty(text) && text.Length > 0 && text[0] == '.')
					{
						_purchaseProducts[i].StoreProductID += text;
						continue;
					}
					PurchaseProduct purchaseProduct = _purchaseProducts[i];
					purchaseProduct.StoreProductID = purchaseProduct.StoreProductID + "." + text;
				}
			}
			_storeListener.Init();
		}

		public void RestorePurchases()
		{
			_storeListener?.RestorePurchases();
		}

		private async UniTaskVoid TryBuyProductIDAsync(string productID)
		{
			PurchaseProduct product = GetPurchaseProduct(productID);
			if (product == null)
			{
				return;
			}
			try
			{
				ManagerUIHelper.ShowWaitingWindow(set: true, backButtonState: false, alwaysOnTop: true, "", updateBackButton: false).Forget();
				await _guestAccountCreator.CreateGuestAccountIfNeeds();
				_attemptIdStorage.GetOrCreate(productID, out var isNew);
				if (isNew)
				{
					Log(product, PurchaseStatus.PURCHASE_INITIATED, "", PurchaseRealErrorOrigin.None, PurchaseRealErrorReason.None, PurchaseRealErrorSdkReason.Unknown);
				}
				IPurchaseHandler handler = _purchaseHandlersContainer.GetHandler(productID);
				if (handler == null)
				{
					Log(product, PurchaseStatus.PURCHASE_CANCELED_BY_SYSTEM, "", PurchaseRealErrorOrigin.Client, PurchaseRealErrorReason.None, PurchaseRealErrorSdkReason.Unknown);
					_attemptIdStorage.Remove(productID);
					return;
				}
				PreBuyActionResult preBuyActionResult = await handler.PreBuyAction(productID);
				if (preBuyActionResult != PreBuyActionResult.Success)
				{
					Log(product, PurchaseStatus.PURCHASE_CANCELED_BY_SYSTEM, "", PurchaseRealErrorOrigin.Client, preBuyActionResult.ToPurchaseRealErrorReason(), PurchaseRealErrorSdkReason.Unknown);
					_attemptIdStorage.Remove(productID);
				}
				else
				{
					_storeListener.BuyProductID(product.StoreProductID);
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				ManagerUIHelper.CloseWaitingWindow().Forget();
			}
		}

		private PurchaseProduct GetPurchaseProductByStoreProductID(string storeProductID)
		{
			for (int i = 0; i < _purchaseProducts.Count; i++)
			{
				if (string.Equals(_purchaseProducts[i].StoreProductID, storeProductID, StringComparison.Ordinal))
				{
					return _purchaseProducts[i];
				}
			}
			return null;
		}

		private async UniTaskVoid ProcessPurchaseAsync(PurchaseEventArgs args, PurchaseProduct purchaseProduct, CancellationToken cancellationToken)
		{
			bool transactionConfirmed = false;
			try
			{
				while (!_playerDataStorage.HasAccountID)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
				}
				Log(purchaseProduct, PurchaseStatus.PURCHASE_SDK_CALLBACK_RECEIVED, args.purchasedProduct.transactionID, PurchaseRealErrorOrigin.None, PurchaseRealErrorReason.None, PurchaseRealErrorSdkReason.Unknown);
				await UniTask.NextFrame();
				ManagerUIHelper.ShowWaitingWindow(set: true, backButtonState: false, alwaysOnTop: true, "", updateBackButton: false).Forget();
				await UniTask.NextFrame();
				_connectionManager.CorrectHash.Get(out var value);
				while (!value && !_connectionManager.CheckHashLimitExceeded)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(2.0));
					_connectionManager.CorrectHash.Get(out value);
				}
				if (!value)
				{
					Log(purchaseProduct, PurchaseStatus.PURCHASE_CLIENT_VALIDATION_FAILED, args.purchasedProduct.transactionID, PurchaseRealErrorOrigin.None, PurchaseRealErrorReason.None, PurchaseRealErrorSdkReason.Unknown);
					return;
				}
				IPurchaseHandler handler = _purchaseHandlersContainer.GetHandler(purchaseProduct.ProductID);
				if (handler != null)
				{
					string attemptId = _attemptIdStorage.Get(purchaseProduct.ProductID);
					_attemptIdStorage.AddProcessPurchase(attemptId);
					PurchaseItemResultStatus purchaseItemResultStatus = await handler.ProcessPurchaseAsync(args, purchaseProduct, attemptId);
					if (purchaseItemResultStatus == PurchaseItemResultStatus.Success || purchaseItemResultStatus == PurchaseItemResultStatus.ItemWasBought)
					{
						Log(purchaseProduct, PurchaseStatus.PURCHASE_COMPLETED, args.purchasedProduct.transactionID, PurchaseRealErrorOrigin.None, PurchaseRealErrorReason.None, PurchaseRealErrorSdkReason.Unknown);
					}
					else
					{
						Log(purchaseProduct, PurchaseStatus.PURCHASE_CANCELED_BY_SYSTEM, args.purchasedProduct.transactionID, PurchaseRealErrorOrigin.Backend, purchaseItemResultStatus.ToPurchaseRealErrorReason(), PurchaseRealErrorSdkReason.Unknown);
					}
					if (PurchaseItemResultExtension.IsConfirmPendingPurchase(purchaseItemResultStatus))
					{
						_storeListener.ConfirmPendingPurchase(args.purchasedProduct);
						_attemptIdStorage.Remove(purchaseProduct.ProductID);
						transactionConfirmed = true;
					}
				}
			}
			catch (Exception)
			{
				Log(purchaseProduct, PurchaseStatus.PURCHASE_FAILED, args.purchasedProduct.transactionID, PurchaseRealErrorOrigin.Client, PurchaseRealErrorReason.ClientProcessPurchaseAsync, PurchaseRealErrorSdkReason.Unknown);
			}
			finally
			{
				if (!transactionConfirmed)
				{
					_storeListener.ReleaseFailedTransaction(args.purchasedProduct);
				}
				ManagerUIHelper.CloseWaitingWindow().Forget();
			}
		}

		public InventoryDef GetSteamInventoryDef(int defId)
		{
			if (_steamDefinitions == null)
			{
				return null;
			}
			InventoryDef[] steamDefinitions = _steamDefinitions;
			foreach (InventoryDef inventoryDef in steamDefinitions)
			{
				if (inventoryDef.Id == defId)
				{
					return inventoryDef;
				}
			}
			return null;
		}

		public InventoryDef GetSteamInventoryDefByCarId(int carId)
		{
			VehicleStockTuningData vehicleStockTuningData = _allCarsInfo.GetVehicleStockTuningData(carId);
			if (!vehicleStockTuningData || vehicleStockTuningData.PurchaseItemType == PurchaseItemType.None)
			{
				return null;
			}
			PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(vehicleStockTuningData.PurchaseItemType);
			if (!(purchaseItemData?.SteamItem))
			{
				return null;
			}
			return GetSteamInventoryDef(purchaseItemData.SteamItem.SteamDefId);
		}

		private void Log(PurchaseProduct purchaseProduct, PurchaseStatus purchaseStatus, string transactionId, PurchaseRealErrorOrigin errorOrigin, PurchaseRealErrorReason errorReason, PurchaseRealErrorSdkReason errorSdkReason)
		{
			if (purchaseProduct != null)
			{
				IPurchaseHandler handler = _purchaseHandlersContainer.GetHandler(purchaseProduct.ProductID);
				if (handler != null)
				{
					string attemptId = _attemptIdStorage.Get(purchaseProduct.ProductID);
					bool isRetry = _attemptIdStorage.IsRetry(attemptId);
					_realPurchaseAnalytic.Log(purchaseProduct, errorOrigin, errorReason, errorSdkReason, isRetry, handler.GetOfferId(purchaseProduct.ProductID), handler.GetItemIds(purchaseProduct.ProductID), transactionId, attemptId, purchaseStatus);
				}
			}
		}
	}
}
