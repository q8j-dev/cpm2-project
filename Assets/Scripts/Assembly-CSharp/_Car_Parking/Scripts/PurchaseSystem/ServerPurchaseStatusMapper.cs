using Firebase.Functions;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public static class ServerPurchaseStatusMapper
	{
		public static PurchaseItemResultStatus ToResultStatus(int serverStatusCode)
		{
			if (IsAlreadyProcessed((FunctionStatus)serverStatusCode))
			{
				return PurchaseItemResultStatus.ItemWasBought;
			}
			return (PurchaseItemResultStatus)serverStatusCode;
		}

		public static int ToWalletFailStatusCode(FunctionStatus status)
		{
			if (IsAlreadyProcessed(status))
			{
				return (int)status;
			}
			return 0;
		}

		private static bool IsAlreadyProcessed(FunctionStatus status)
		{
			if (status != FunctionStatus.AppPurchaseAlreadyBought)
			{
				return status == FunctionStatus.AppPurchaseDuplicateTransaction;
			}
			return true;
		}
	}
}
