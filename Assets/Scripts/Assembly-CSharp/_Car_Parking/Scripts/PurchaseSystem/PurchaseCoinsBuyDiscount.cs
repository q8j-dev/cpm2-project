using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using _Car_Parking.Scripts.Wallet.Cypher;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public class PurchaseCoinsBuyDiscount
	{
		private const string CheckFN = "HasPurchaseHistory24_1";

		public async UniTask<int[]> CheckProductToBuyAsync()
		{
			FunctionCallResult<string> functionCallResult = await new FirebaseWrappedFunctionFactory().Create("HasPurchaseHistory24_1").CallAsyncResult<string>();
			if (functionCallResult.IsFail || string.IsNullOrEmpty(functionCallResult.Data))
			{
				return null;
			}
			string text = BuyCarCypher.Decode(functionCallResult.Data);
			return JsonConvert.DeserializeObject<int[]>("[" + text + "]");
		}
	}
}
