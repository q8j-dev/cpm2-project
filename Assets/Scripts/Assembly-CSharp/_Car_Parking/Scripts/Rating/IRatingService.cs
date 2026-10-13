using System.Threading;
using CPM.Rating.Entities;
using Cysharp.Threading.Tasks;
using Firebase.Functions;

namespace _Car_Parking.Scripts.Rating
{
	public interface IRatingService
	{
		UniTask<FunctionCallResult<SetRatingResponse>> SetUserRating(string json, string accountID, CancellationToken token = default(CancellationToken));

		UniTask<FunctionCallResult<RatingResultAll>> GetUserRating(string accountID, CancellationToken token = default(CancellationToken));
	}
}
