using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public class RallyAnalytic : AnalyticProviderBase
	{
		public RallyAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void LogModeEntered(RallyModeType modeType)
		{
			RallyContext ctx = new RallyContext
			{
				Status = RallyStatus.MODE_ENTERED,
				ModeType = modeType
			};
			Send(ctx);
		}

		public void LogStageStarted(RallyStageInfo info, int attemptNumber, int restartCount)
		{
			RallyContext ctx = BuildStageContext(RallyStatus.STAGE_STARTED, info, attemptNumber, restartCount);
			Send(ctx);
		}

		public void LogStageCompleted(RallyStageInfo info, RallyStageResult result, int attemptNumber, int restartCount)
		{
			RallyContext rallyContext = BuildStageContext(RallyStatus.STAGE_COMPLETED, info, attemptNumber, restartCount);
			rallyContext.LapTime = result.LapTime;
			rallyContext.RequiredValue = result.RequiredValue;
			rallyContext.IsPersonalRecord = result.IsPersonalRecord;
			rallyContext.MedalType = result.MedalType;
			rallyContext.PrevMedalType = result.PrevMedalType;
			rallyContext.CheckpointMissed = result.CheckpointMissed;
			rallyContext.CheckpointPenaltyTotal = result.CheckpointPenaltyTotal;
			rallyContext.RewardId = (string.IsNullOrEmpty(result.RewardId) ? "[]" : result.RewardId);
			Send(rallyContext);
		}

		public void LogStageFailed(RallyStageInfo info, RallyFailReason failReason, int attemptNumber, int restartCount)
		{
			RallyContext rallyContext = BuildStageContext(RallyStatus.STAGE_FAILED, info, attemptNumber, restartCount);
			rallyContext.FailReason = failReason;
			Send(rallyContext);
		}

		public void LogStageRestarted(RallyStageInfo info, int attemptNumber, int restartCount)
		{
			RallyContext ctx = BuildStageContext(RallyStatus.STAGE_RESTARTED, info, attemptNumber, restartCount);
			Send(ctx);
		}

		private static RallyContext BuildStageContext(RallyStatus status, RallyStageInfo info, int attemptNumber, int restartCount)
		{
			return new RallyContext
			{
				Status = status,
				ModeType = info.ModeType,
				CampaignId = RallyAnalyticsHelper.BuildCampaignId(info.LocationName, info.LocationIndex),
				SeriesId = RallyAnalyticsHelper.BuildSeriesId(info.LocationName, info.TrajectoryIndex),
				EventId = RallyAnalyticsHelper.BuildEventId(info.LocationName, info.TrajectoryIndex),
				StageType = RallyAnalyticsHelper.MapStageType(info.IsNight),
				StageIndex = info.StageIndex,
				TrajectoryId = info.TrajectoryId,
				TyreType = info.TyreType,
				CarId = info.CarId,
				CarClass = info.CarClass,
				CheckpointPenaltyTotal = info.CheckpointPenaltyTotal,
				AttemptNumber = attemptNumber,
				RestartCount = restartCount,
				NgPlusLevel = info.NgPlusLevel,
				Special = info.Special
			};
		}

		private void Send(RallyContext ctx)
		{
			base.AnalyticEventFactory.CreateEvent<RallyEvent, RallyContext>(ctx).Send();
		}
	}
}
