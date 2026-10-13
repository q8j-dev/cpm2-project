using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchaseCheckerServerAvailable : IPurchaseCheckProductToBuy
	{
		public const string PingFn = "Ping24_1";

		public async UniTask<PreBuyActionResult> CheckProductToBuyAsync(string productID)
		{
			return (await new FirebaseWrappedFunctionFactory().Create("Ping24_1").CallAsyncResult<EmptyClass>()).IsSuccess ? PreBuyActionResult.Success : PreBuyActionResult.Unknown;
		}
	}
}
