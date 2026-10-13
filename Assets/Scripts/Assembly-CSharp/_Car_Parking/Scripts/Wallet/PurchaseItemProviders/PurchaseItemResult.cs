using _Car_Parking.Scripts.CustomDataTypes;

namespace _Car_Parking.Scripts.Wallet.PurchaseItemProviders
{
	public class PurchaseItemResult
	{
		public AdvancedBoolean Status;

		public int StatusCode;

		public int Value;

		public string Data;

		public bool IsSuccess => StatusCode == 1;

		public static PurchaseItemResult GetFailResult()
		{
			return new PurchaseItemResult
			{
				Status = new AdvancedBoolean(v: false),
				StatusCode = 0
			};
		}
	}
}
