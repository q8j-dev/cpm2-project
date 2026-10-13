using System.Threading;
using CPM.Rating.Entities;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using _Car_Parking.Scripts.CPMRating;
using _Car_Parking.Scripts.Other.Cypher;

namespace _Car_Parking.Scripts.Rating
{
	public class FirebaseRatingService : IRatingService
	{
		public const string SetUserRatingFnEncrypt = "SetUserRating24_1";

		public const string GetUserRatingFn = "GetUserRatingCall24_1";

		private readonly FirebaseWrappedFunctionFactory _factory = new FirebaseWrappedFunctionFactory();

		public UniTask<FunctionCallResult<SetRatingResponse>> SetUserRating(string json, string accountID, CancellationToken token = default(CancellationToken))
		{
			token.ThrowIfCancellationRequested();
			string functionName = RatingFunctionsChanger.SetUserRatingFunctionName("SetUserRating24_1");
			return _factory.Create(functionName).CallAsyncResult<SetRatingResponse>(WalletCypher.Encode(json, null));
		}

		public UniTask<FunctionCallResult<RatingResultAll>> GetUserRating(string accountID, CancellationToken token = default(CancellationToken))
		{
			token.ThrowIfCancellationRequested();
			return _factory.Create("GetUserRatingCall24_1").CallAsyncResult<RatingResultAll>(accountID);
		}
	}
}
