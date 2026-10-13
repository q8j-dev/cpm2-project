using System;
using CodeStage.AntiCheat.ObscuredTypes;

namespace _Car_Parking.Scripts.Wallet.PurchaseItemProviders
{
	[Serializable]
	public class RealPurchaseData
	{
		public RealPurchaseType Type;

		public ObscuredInt Amount;

		public int GarageIndex;

		public string EditorName => Type switch
		{
			RealPurchaseType.Coin => $"{Type} {Amount}", 
			RealPurchaseType.Money => $"{Type} {Amount}", 
			RealPurchaseType.Garage => $"{Type} {GarageIndex}", 
			_ => $"{Type}", 
		};
	}
}
