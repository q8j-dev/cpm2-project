using System.Threading;
using CPM.Rating.Entities;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using VContainer;

namespace _Car_Parking.Scripts.Rating
{
	public class RatingServiceRouter : IRatingService
	{
		private readonly FirebaseRatingService _firebase;

		private readonly BackendRatingService _backend;

		private readonly IRemoteConfigManager _remoteConfig;

		[Inject]
		public RatingServiceRouter(FirebaseRatingService firebase, BackendRatingService backend, IRemoteConfigManager remoteConfig)
		{
			_firebase = firebase;
			_backend = backend;
			_remoteConfig = remoteConfig;
		}

		public async UniTask<FunctionCallResult<SetRatingResponse>> SetUserRating(string json, string accountID, CancellationToken token = default(CancellationToken))
		{
			return await (await ResolveAsync()).SetUserRating(json, accountID, token);
		}

		public async UniTask<FunctionCallResult<RatingResultAll>> GetUserRating(string accountID, CancellationToken token = default(CancellationToken))
		{
			return await (await ResolveAsync()).GetUserRating(accountID, token);
		}

		private async UniTask<IRatingService> ResolveAsync()
		{
			if ((await _remoteConfig.GetConfigValuesStorage()).useNewRatingBackend)
			{
				return _backend;
			}
			return _firebase;
		}
	}
}
