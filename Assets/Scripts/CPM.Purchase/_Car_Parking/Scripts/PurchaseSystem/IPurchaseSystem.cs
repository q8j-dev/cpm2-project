using System;
using System.Collections.Generic;
using Steamworks;
using UnityEngine.Purchasing;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public interface IPurchaseSystem
	{
		bool IsInitialized { get; }

		event Action OnInitializedPreCompletedEvent;

		event Action OnInitializedCompletedEvent;

		void AddPurchaseProducts(List<PurchaseProduct> purchaseProducts, ProductSource productSource);

		void AddContentToPurchaseBuilder(ConfigurationBuilder builder);

		void OnInitialized(IStoreController controller, IExtensionProvider extensions);

		PurchaseProduct GetPurchaseProduct(PurchaseItemType purchaseItemType);

		PurchaseProduct GetPurchaseProduct(string productID);

		void BuyProductID(PurchaseItemType purchaseItemType);

		void BuyProductID(string productId);

		void ProcessPurchase(PurchaseEventArgs args);

		void PurchaseFailed(Product product, PurchaseFailureReason failureReason);

		void RestorePurchases();

		InventoryDef GetSteamInventoryDef(int defId);

		InventoryDef GetSteamInventoryDefByCarId(int carId);
	}
}
