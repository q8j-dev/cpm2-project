using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.PurchaseItemProviders
{
	[CreateAssetMenu(fileName = "PurchaseItemsCollection", menuName = "Scriptable/ItemPurchase/PurchaseItemsCollection")]
	[Preserve]
	public class PurchaseItemsCollection : ScriptableObject
	{
		[SerializeField]
		private PurchaseItemData[] purchaseItems = new PurchaseItemData[0];

		public IReadOnlyList<PurchaseItemData> PurchaseItems => purchaseItems;

		[Preserve]
		public PurchaseItemsCollection()
		{
		}

		public PurchaseItemData GetPurchaseItemData(PurchaseItemType itemType)
		{
			PurchaseItemData[] array = purchaseItems;
			foreach (PurchaseItemData purchaseItemData in array)
			{
				if (purchaseItemData.purchaseItemType == itemType)
				{
					return purchaseItemData;
				}
			}
			Debug.LogError($"PurchaseItemData not found for type: {itemType}");
			return null;
		}

		public PurchaseItemData GetPurchaseItemData(string purchaseId)
		{
			PurchaseItemData[] array = purchaseItems;
			foreach (PurchaseItemData purchaseItemData in array)
			{
				if (purchaseItemData.PurchaseId == purchaseId)
				{
					return purchaseItemData;
				}
			}
			return null;
		}
	}
}
