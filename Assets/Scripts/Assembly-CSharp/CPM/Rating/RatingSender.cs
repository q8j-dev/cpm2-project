using System;
using System.Collections.Generic;
using CPM.Common;
using CPM.DailyTasks;
using CPM.PlayerData;
using CPM.Rating.Entities;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.CPMRating.AchievementsService;
using _Car_Parking.Scripts.CPMRating.PopupNotifications;
using _Car_Parking.Scripts.Rating;

namespace CPM.Rating
{
	public class RatingSender : IInitializable, IDisposable
	{
		private readonly AppStateHandler _appStateHandler;

		private readonly DailyTasksManager _dailyTasksManager;

		private readonly RatingDataCollection _ratingDataCollection;

		private readonly PlayerDataStorage _playerDataStorage;

		private readonly IAchievementsService _achievementsService;

		private readonly IRatingService _ratingService;

		private readonly AchievementPopupChannel _popupChannel;

		[Inject]
		public RatingSender(AppStateHandler appStateHandler, DailyTasksManager dailyTasksManager, RatingDataCollection ratingDataCollection, PlayerDataStorage playerDataStorage, IAchievementsService achievementsService, IRatingService ratingService, AchievementPopupChannel popupChannel)
		{
			_appStateHandler = appStateHandler;
			_dailyTasksManager = dailyTasksManager;
			_ratingDataCollection = ratingDataCollection;
			_playerDataStorage = playerDataStorage;
			_achievementsService = achievementsService;
			_ratingService = ratingService;
			_popupChannel = popupChannel;
		}

		public void Initialize()
		{
			_appStateHandler.ApplicationPause += OnPause;
		}

		public void Dispose()
		{
			if (_appStateHandler != null)
			{
				_appStateHandler.ApplicationPause -= OnPause;
			}
			SaveData();
		}

		public async UniTask SendRating(string json)
		{
			FunctionCallResult<SetRatingResponse> functionCallResult = await _ratingService.SetUserRating(json, _playerDataStorage.AccountID);
			if (functionCallResult.IsFail)
			{
				return;
			}
			SetRatingResponse data = functionCallResult.Data;
			_achievementsService?.OnRatingChanged();
			Dictionary<string, float> dictionary = data.callback?.ToDictionary();
			if (data.battlepass?.daily == null)
			{
				return;
			}
			_dailyTasksManager.finished = data.battlepass?.daily.finished;
			if (dictionary == null)
			{
				return;
			}
			foreach (KeyValuePair<string, float> item in dictionary)
			{
				if (!(item.Value < 0f))
				{
					(string, string, string, Texture, string, string) tuple = _ratingDataCollection.achievs[item.Key].Get((int)item.Value);
					PopupNotificationData data2 = new PopupAchievementData(tuple.Item1, tuple.Item2, item.Value.ToString(), tuple.Item4, tuple.Item5, tuple.Item6);
					_popupChannel.Enqueue(data2);
				}
			}
		}

		public async UniTask<RatingResultAll> GetRating(string json)
		{
			FunctionCallResult<RatingResultAll> functionCallResult = await _ratingService.GetUserRating(json);
			if (functionCallResult.IsFail)
			{
				return null;
			}
			return functionCallResult.Data;
		}

		private void OnPause(bool pause)
		{
			if (pause)
			{
				SaveData();
			}
		}

		private void SaveData()
		{
			_dailyTasksManager.Save();
		}
	}
}
