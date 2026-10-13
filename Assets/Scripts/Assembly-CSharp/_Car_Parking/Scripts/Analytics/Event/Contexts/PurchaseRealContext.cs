using System.Collections.Generic;
using UnityEngine.Purchasing;
using _Car_Parking.Scripts.Analytics.Data;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Analytics.Event.Contexts
{
	public class PurchaseRealContext : IEventContext
	{
		public PurchaseStatus Status;

		public string AttemptId;

		public ProductType ProductType;

		public PurchaseRealErrorOrigin ErrorOrigin;

		public PurchaseRealErrorReason ErrorReason;

		public PurchaseRealErrorSdkReason SdkFailReason;

		public bool IsRetry;

		public string OfferId;

		public string ProductId;

		public string PriceType;

		public float Price;

		public List<ItemIdData> Items;

		public string TransactionId;

		public int SteamItemDefId;
	}
}
