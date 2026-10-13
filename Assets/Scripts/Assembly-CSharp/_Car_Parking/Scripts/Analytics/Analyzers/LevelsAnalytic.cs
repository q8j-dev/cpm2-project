using CodeStage.AntiCheat.Storage;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class LevelsAnalytic : AnalyticProviderBase
	{
		public const string FinishCompleted = "complete";

		public const string QuitManual = "quit_manual";

		public const string Timeout = "timeout";

		public const string TrafficAccident = "traffic_accident";

		private readonly LevelsProvider _levelsProvider;

		private float BestTimeForLevel
		{
			get
			{
				float num = ObscuredPrefs.GetFloat($"bestResultID{_levelsProvider.CurrentLevelData.ID}");
				if (num == 0f)
				{
					num = _levelsProvider.LevelScript.PlayDuration;
				}
				return num;
			}
		}

		public LevelsAnalytic(AnalyticEventFactory analyticEventFactory, LevelsProvider levelsProvider)
			: base(analyticEventFactory)
		{
			_levelsProvider = levelsProvider;
		}

		private bool HasLevelSession(string caller)
		{
			if (_levelsProvider == null || base.AnalyticEventFactory == null)
			{
				Debug.LogError("[LevelsAnalytic] " + caller + " пропущен: аналитика собрана без зависимостей " + $"(levelsProvider: {_levelsProvider != null}, eventFactory: {base.AnalyticEventFactory != null})");
				return false;
			}
			if (_levelsProvider.CurrentLevelData == null || _levelsProvider.LevelScript == null)
			{
				return false;
			}
			return true;
		}

		public void LogStartLevel()
		{
			if (HasLevelSession("LogStartLevel"))
			{
				base.AnalyticEventFactory.CreateEvent<LevelStartEvent, LevelContext>(new LevelContext
				{
					LevelId = _levelsProvider.CurrentLevelData.name,
					MapId = SceneManager.GetActiveScene().name
				}).Send();
			}
		}

		public void LogRestartLevel()
		{
			if (HasLevelSession("LogRestartLevel"))
			{
				base.AnalyticEventFactory.CreateEvent<LevelRestartEvent, LevelContext>(new LevelContext
				{
					LevelId = _levelsProvider.CurrentLevelData.name,
					MapId = SceneManager.GetActiveScene().name
				}).Send();
			}
		}

		public void LogFinishLevel(string finishReason)
		{
			if (HasLevelSession("LogFinishLevel(" + finishReason + ")"))
			{
				LevelFinishContext context = new LevelFinishContext
				{
					LevelId = _levelsProvider.CurrentLevelData.name,
					FinishReason = finishReason,
					PlayDuration = _levelsProvider.LevelScript.PlayDuration,
					ModeDuration = _levelsProvider.LevelScript.ModeDuration,
					RewardSoft = ((finishReason == "complete") ? _levelsProvider.LevelScript.MoneyWon : (-1)),
					BestCompletionTime = BestTimeForLevel,
					AttemptNumber = _levelsProvider.LevelScript.AttemptNumber
				};
				base.AnalyticEventFactory.CreateEvent<LevelFinishEvent, LevelFinishContext>(context).Send();
			}
		}
	}
}
