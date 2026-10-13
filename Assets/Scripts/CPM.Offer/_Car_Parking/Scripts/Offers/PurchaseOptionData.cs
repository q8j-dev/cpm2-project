using System;

namespace _Car_Parking.Scripts.Offers
{
	[Serializable]
	public class PurchaseOptionData
	{
		public PurchaseType Type;

		public string BundleID;

		public int Price;

		public int SteamItemDefId;
	}
}
