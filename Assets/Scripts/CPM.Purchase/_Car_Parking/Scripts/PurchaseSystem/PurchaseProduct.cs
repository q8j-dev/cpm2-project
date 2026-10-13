using UnityEngine.Purchasing;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	public class PurchaseProduct
	{
		public bool Initialized;

		public ProductType ProductType;

		public string ProductID;

		public string StoreProductID;

		public string IsoCurrencyCode;

		public float LocalizedPrice;

		public string LocalizedPriceText;

		public string BundleSuffix;

		public int ItemDefId;

		public PurchaseProduct(string productID, ProductType productType, string bundleSuffix)
		{
			ProductType = productType;
			BundleSuffix = bundleSuffix;
			ProductID = productID;
		}
	}
}
