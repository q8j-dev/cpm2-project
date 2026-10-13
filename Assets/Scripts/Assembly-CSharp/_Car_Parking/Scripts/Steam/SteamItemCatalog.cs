using System.Collections.Generic;
using System.Linq;
using VContainer.Unity;
using _Car_Parking.Scripts.PurchaseItemProviders;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.Steam
{
	public class SteamItemCatalog : ISteamItemCatalog, IInitializable
	{
		private readonly PurchaseItemsCollection _purchaseItemsCollection;

		private List<SteamItem> _items;

		public IReadOnlyList<SteamItem> AllItems => _items;

		public SteamItemCatalog(PurchaseItemsCollection purchaseItemsCollection)
		{
			_purchaseItemsCollection = purchaseItemsCollection;
		}

		public void Initialize()
		{
			_items = new List<SteamItem>();
			foreach (PurchaseItemData purchaseItem in _purchaseItemsCollection.PurchaseItems)
			{
				if ((bool)purchaseItem.SteamItem)
				{
					_items.Add(purchaseItem.SteamItem);
				}
			}
		}

		public SteamItem GetSteamItemByDefId(int defId)
		{
			return _items.FirstOrDefault((SteamItem x) => (bool)x && x.SteamDefId == defId);
		}
	}
}
