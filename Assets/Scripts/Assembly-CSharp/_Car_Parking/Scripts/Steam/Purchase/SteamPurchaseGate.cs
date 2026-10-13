using System;
using Cysharp.Threading.Tasks;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Steam.Purchase
{
	public class SteamPurchaseGate : ISteamPurchaseGate
	{
		private const string IS_PURCHASABLE_FN = "IsSteamPurchasable24_1";

		private const string LogTag = "[SteamPurchaseGate]";

		public bool PurchasesAllowed { get; private set; }

		public async UniTask FetchAsync()
		{
			PurchasesAllowed = false;
			if (!SteamManager.IsInitialized())
			{
				return;
			}
			try
			{
				PurchasesAllowed = (await new FirebaseWrappedFunctionFactory().Create("IsSteamPurchasable24_1").CallAsyncResult<string>()).IsSuccess;
			}
			catch (Exception)
			{
				PurchasesAllowed = false;
			}
		}
	}
}
