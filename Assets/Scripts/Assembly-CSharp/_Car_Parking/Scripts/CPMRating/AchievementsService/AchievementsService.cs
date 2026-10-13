using System.Threading;
using CPM.PlayerData;
using CPM.Rating.Entities;
using Cysharp.Threading.Tasks;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.CPMRating.AchievementsService
{
	[Preserve]
	public class AchievementsService : IAchievementsService
	{
		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly PlayerDataStorage _playerDataStorage;

		private RatingResultAll _playerUserRating;

		private UniTask<RatingResultAll>? _currentRatingTask;

		public AchievementsService(FreeDriveProvider freeDriveProvider, PlayerDataStorage playerDataStorage)
		{
			_freeDriveProvider = freeDriveProvider;
			_playerDataStorage = playerDataStorage;
		}

		public async UniTask<RatingResultAll> GetUserRating(CancellationToken cancellationToken)
		{
			if (_playerUserRating != null)
			{
				return _playerUserRating;
			}
			if (_currentRatingTask.HasValue)
			{
				return await _currentRatingTask.Value;
			}
			UniTask<RatingResultAll> userRating = _freeDriveProvider.FreeDriveDB.GetUserRating(_playerDataStorage.AccountID, cancellationToken);
			_currentRatingTask = userRating;
			try
			{
				_playerUserRating = await userRating;
				return _playerUserRating;
			}
			finally
			{
				_currentRatingTask = null;
			}
		}

		public void OnRatingChanged()
		{
			_playerUserRating = null;
		}
	}
}
