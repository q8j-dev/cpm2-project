using UnityEngine;
using UnityEngine.Purchasing;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchaserStoreListener : IStoreListener
	{
		private const string BundleId_Android = "com.olzhas.carparking.multyplayer2";

		private const string BundleId_IOS = "com.olzhas.carparking.multyplayer2.ios";

		private const string BundleId_Steam = "steam";

		private readonly IPurchaseSystem _purchaseSystem;

		private readonly FreeDriveProvider _freeDriveProvider;

		private IStoreController _storeController;

		private IExtensionProvider _storeExtensionProvider;

		private readonly ProcessingTransactionRegistry _processingTransactions = new ProcessingTransactionRegistry();

		public IStoreController StoreController => _storeController;

		public string StorePrefix => "steam";

		private bool IsInitialized
		{
			get
			{
				if (_storeController != null)
				{
					return _storeExtensionProvider != null;
				}
				return false;
			}
		}

		public PurchaserStoreListener(IPurchaseSystem purchaseSystem, FreeDriveProvider freeDriveProvider)
		{
			_purchaseSystem = purchaseSystem;
			_freeDriveProvider = freeDriveProvider;
		}

		public void Init()
		{
			if (!IsInitialized)
			{
				ConfigurationBuilder configurationBuilder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
				_purchaseSystem.AddContentToPurchaseBuilder(configurationBuilder);
				UnityPurchasing.Initialize(this, configurationBuilder);
			}
		}

		public void BuyProductID(string productId)
		{
			if (IsInitialized)
			{
				Product product = _storeController.products.WithID(productId);
				if (product != null && product.availableToPurchase)
				{
					_storeController.InitiatePurchase(product);
				}
			}
		}

		public void RestorePurchases()
		{
			if (!IsInitialized)
			{
				Debug.Log("[PurchaserStoreListener] RestorePurchases FAIL. Not initialized.");
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer)
			{
				Debug.Log("[PurchaserStoreListener] RestorePurchases started ...");
				_storeExtensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions((bool success, string message) =>
				{
					Debug.Log($"[PurchaserStoreListener] RestorePurchases continuing: {success} {message}. If no further messages, no purchases available to restore.");
				});
			}
			else
			{
				Debug.Log("[PurchaserStoreListener] RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
			}
		}

		public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
		{
			Debug.Log("[PurchaserStoreListener] OnInitialized: PASS");
			_storeController = controller;
			_storeExtensionProvider = extensions;
			if (_storeController != null)
			{
				V.StoreController = _storeController;
				FlagsStorage.StoreInitialized = true;
				if (_freeDriveProvider != null && (bool)_freeDriveProvider.CoinPurchaseWindow)
				{
					_freeDriveProvider.CoinPurchaseWindow.UpdateCoinPrice();
				}
				FlagsStorage.IsPurchaserInitialized = true;
				_purchaseSystem.OnInitialized(controller, extensions);
			}
		}

		public void OnInitializeFailed(InitializationFailureReason error)
		{
		}

		public void OnInitializeFailed(InitializationFailureReason error, string message = null)
		{
		}

		public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
		{
			bool flag = true;
			if (!Application.isEditor && !flag && !Application.isEditor && Application.platform != RuntimePlatform.IPhonePlayer)
			{
				return PurchaseProcessingResult.Complete;
			}
			string transactionID = args.purchasedProduct.transactionID;
			if (!_processingTransactions.TryBegin(transactionID))
			{
				return PurchaseProcessingResult.Pending;
			}
			_purchaseSystem.ProcessPurchase(args);
			return PurchaseProcessingResult.Pending;
		}

		public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
		{
			if (true || Application.isEditor)
			{
				_purchaseSystem.PurchaseFailed(product, failureReason);
			}
		}

		public void ConfirmPendingPurchase(Product product)
		{
			_processingTransactions.Release(product.transactionID);
			StoreController?.ConfirmPendingPurchase(product);
		}

		public void ReleaseFailedTransaction(Product product)
		{
			_processingTransactions.Release(product.transactionID);
		}
	}
}
