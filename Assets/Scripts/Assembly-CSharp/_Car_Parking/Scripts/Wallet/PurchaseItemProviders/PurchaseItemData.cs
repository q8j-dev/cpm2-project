using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.Steam;

namespace _Car_Parking.Scripts.Wallet.PurchaseItemProviders
{
	[Serializable]
	public class PurchaseItemData
	{
		public PurchaseItemType purchaseItemType;

		public string PurchaseId;

		[Tooltip("almost always equals bundle ID, so can be left empty")]
		public string BundleIdSuffix;

		public string AnalyticsProductId;

		public ObscuredInt Price;

		public RealPurchaseData RealPurchaseData;

		[Header("Steam")]
		public SteamItem SteamItem;

		public string EditorName
		{
			get
			{
				string text = $"{purchaseItemType} {PurchaseId} {AnalyticsProductId}";
				if (RealPurchaseData != null)
				{
					text = text + " " + RealPurchaseData.EditorName;
				}
				return text;
			}
		}
	}
}
