using CPM.Account;
using CPM.Connection;
using CPM.DailyTasks;
using CPM.Rating;
using CPM.Rating.Entities;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using VContainer;

namespace _Car_Parking.Scripts.Database.Account
{
	public class LevelRatingSender
	{
		private RatingSender _ratingController;

		private DailyTasksManager _dailyTasksManager;

		private static int _lastSentLevel;

		private IAccountController _accountController;

		private IConnectionManager _connectionManager;

		[Inject]
		public LevelRatingSender(RatingSender ratingController, DailyTasksManager dailyTasksManager, IAccountController accountController, IConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
			_accountController = accountController;
			_dailyTasksManager = dailyTasksManager;
			_ratingController = ratingController;
		}

		public async UniTask SendRatingAfterLevels()
		{
			if (_accountController.IsAnonymous || !_accountController.Authorized)
			{
				return;
			}
			_connectionManager.CorrectHash.Get(out var value);
			if (!value)
			{
				return;
			}
			RatingData rating = new RatingData();
			int num = 0;
			for (int i = 0; i < 200; i++)
			{
				if (ObscuredPrefs.HasKey("bestResultID" + i))
				{
					num++;
				}
			}
			if (num > 0 && num > _lastSentLevel)
			{
				rating.levels = num;
				_lastSentLevel = num;
			}
			float num2 = (float)V.AnalyticWheres.multiFreeDrive + (float)V.AnalyticWheres.singleFree + V.AnalyticWheres.level + (float)V.AnalyticWheres.DragRacing + (float)V.AnalyticWheres.CircuitRacing;
			if (num2 > 1f)
			{
				rating.time = (int)num2;
			}
			string text = rating.ToJson();
			if (text != "}")
			{
				await _ratingController.SendRating(text);
				_dailyTasksManager.UpdateData(rating).Forget();
			}
		}
	}
}
