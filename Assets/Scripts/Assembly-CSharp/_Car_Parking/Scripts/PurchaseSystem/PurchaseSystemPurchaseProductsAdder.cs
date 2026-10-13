using System.Collections.Generic;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine.Purchasing;
using _Car_Parking.Scripts.Backend;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchaseSystemPurchaseProductsAdder
	{
		private readonly IPurchaseSystem _purchaseSystem;

		private readonly PurchaseItemsCollection _purchaseItemsCollection;

		private readonly AllCarsInfo _allCarsInfo;

		private readonly CheckBackendService _checkBackendService;

		private readonly IOfflineModeSystem _offlineModeSystem;

		private const float DelayToCheckOfflineMode = 10f;

		public PurchaseSystemPurchaseProductsAdder(IPurchaseSystem purchaseSystem, PurchaseItemsCollection purchaseItemsCollection, AllCarsInfo allCarsInfo, CheckBackendService checkBackendService, IOfflineModeSystem offlineModeSystem)
		{
			_purchaseSystem = purchaseSystem;
			_purchaseItemsCollection = purchaseItemsCollection;
			_allCarsInfo = allCarsInfo;
			_checkBackendService = checkBackendService;
			_offlineModeSystem = offlineModeSystem;
		}

		public async UniTask InitAsync()
		{
			AddOffersProducts();
			AddMoneyProducts();
			AddCoinsProducts();
			AddCarProducts();
			AddGarageProducts();
			AddGarageOldProducts();
		}

		private void AddOffersProducts()
		{
			AddProducts(RealPurchaseType.Offer, ProductSource.Offers);
		}

		private void AddMoneyProducts()
		{
			AddProducts(RealPurchaseType.Money, ProductSource.Money);
		}

		private void AddCoinsProducts()
		{
			AddProducts(RealPurchaseType.Coin, ProductSource.Coins);
		}

		private void AddGarageProducts()
		{
			AddProducts(RealPurchaseType.Garage, ProductSource.Garage);
		}

		private void AddGarageOldProducts()
		{
			AddProducts(RealPurchaseType.GarageOld, ProductSource.GarageOld, ProductType.NonConsumable);
		}

		private void AddCarProducts()
		{
			List<PurchaseProduct> list = new List<PurchaseProduct>();
			for (int i = 0; i < _allCarsInfo.AllCars.Count; i++)
			{
				if (_allCarsInfo.AllCars[i].PurchaseItemType != PurchaseItemType.None)
				{
					PurchaseItemData purchaseItemData = _purchaseItemsCollection.GetPurchaseItemData(_allCarsInfo.AllCars[i].PurchaseItemType);
					if (purchaseItemData != null)
					{
						list.Add(new PurchaseProduct(purchaseItemData.PurchaseId, ProductType.Consumable, purchaseItemData.BundleIdSuffix));
					}
				}
			}
			_purchaseSystem.AddPurchaseProducts(list, ProductSource.Cars);
		}

		private void AddProducts(RealPurchaseType realPurchaseType, ProductSource productSource, ProductType productType = ProductType.Consumable)
		{
			List<PurchaseProduct> list = new List<PurchaseProduct>();
			foreach (PurchaseItemData purchaseItem in _purchaseItemsCollection.PurchaseItems)
			{
				if (purchaseItem != null && purchaseItem.RealPurchaseData != null && purchaseItem.RealPurchaseData.Type == realPurchaseType)
				{
					list.Add(new PurchaseProduct(purchaseItem.PurchaseId, productType, purchaseItem.BundleIdSuffix));
				}
			}
			_purchaseSystem.AddPurchaseProducts(list, productSource);
		}
	}
}
