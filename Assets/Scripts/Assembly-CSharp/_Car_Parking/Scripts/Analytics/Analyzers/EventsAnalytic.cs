using System;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.EventSystem.Data;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public class EventsAnalytic : AnalyticProviderBase
	{
		public EventsAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void LogEventFirstShow(_Car_Parking.Scripts.EventSystem.Data.Event eventData, string AlowedCarClases = "")
		{
			if (eventData.Status == EventStatus.InProgress)
			{
				int eventDaysCount = GetEventDaysCount(eventData.EndTime);
				base.AnalyticEventFactory.CreateEvent<TimeAttackEvent, TimeAttackContext>(new TimeAttackContext
				{
					Status = TimeAttackStatus.START,
					GameEventId = eventData.Id,
					EventVariant = eventData.Type.ToString(),
					EventDurationDays = eventDaysCount,
					ModeType = (FlagsStorage.IsMultiFree ? OpenWorldType.Multiplayer : OpenWorldType.Single),
					RequiredCarClasses = AlowedCarClases
				}).Send();
			}
		}

		public void LogPlayerStartRace(TimeAttackContext timeAttackContext, _Car_Parking.Scripts.EventSystem.Data.Event eventData)
		{
			int eventDaysCount = GetEventDaysCount(eventData.EndTime);
			timeAttackContext.EventDurationDays = eventDaysCount;
			base.AnalyticEventFactory.CreateEvent<TimeAttackEvent, TimeAttackContext>(timeAttackContext).Send();
		}

		public void LogPlayerReStartRace(TimeAttackContext timeAttackContext)
		{
			base.AnalyticEventFactory.CreateEvent<TimeAttackEvent, TimeAttackContext>(timeAttackContext).Send();
		}

		public void LogPlayerEndRace(TimeAttackContext timeAttackContext, _Car_Parking.Scripts.EventSystem.Data.Event eventData)
		{
			timeAttackContext.EventDurationDays = GetEventDaysCount(eventData.EndTime);
			base.AnalyticEventFactory.CreateEvent<TimeAttackEvent, TimeAttackContext>(timeAttackContext).Send();
		}

		public void LogClaimReward(string eventId, bool rewardRecieved, string rewardId, string rewardType)
		{
			base.AnalyticEventFactory.CreateEvent<TimeAttackEvent, TimeAttackContext>(new TimeAttackContext
			{
				Status = TimeAttackStatus.REWARD_CLAIMED,
				GameEventId = eventId,
				RewardRecieved = rewardRecieved,
				RewardId = rewardId,
				RewardType = rewardType
			}).Send();
		}

		public void LogClaimReward(TimeAttackContext timeAttackContext)
		{
			base.AnalyticEventFactory.CreateEvent<TimeAttackEvent, TimeAttackContext>(timeAttackContext).Send();
		}

		public void LogClaimReward(string eventId, bool rewardRecieved)
		{
			base.AnalyticEventFactory.CreateEvent<TimeAttackEvent, TimeAttackContext>(new TimeAttackContext
			{
				Status = TimeAttackStatus.REWARD_CLAIMED,
				GameEventId = eventId,
				RewardRecieved = rewardRecieved
			}).Send();
		}

		public void LogFailRace(TimeAttackContext timeAttackContext)
		{
			base.AnalyticEventFactory.CreateEvent<TimeAttackEvent, TimeAttackContext>(timeAttackContext).Send();
		}

		private int GetEventDaysCount(long endTime)
		{
			long num = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
			return TimeSpan.FromMilliseconds(endTime - num).Days;
		}

		public void LogSnowmanStart(string eventId, int countTotal, string mapId, string zoneId, string carId, string carClass, string modeType)
		{
			base.AnalyticEventFactory.CreateEvent<SnowmanEvent, SnowmanContext>(new SnowmanContext
			{
				Status = SnowmanStatus.START,
				GameEventId = eventId,
				CountTotal = countTotal,
				MapId = mapId,
				ZoneId = zoneId,
				CarId = carId,
				CarClass = carClass,
				ModeType = modeType
			}).Send();
		}

		public void LogSnowmanBegin(string eventId, int countTotal)
		{
			base.AnalyticEventFactory.CreateEvent<SnowmanEvent, SnowmanContext>(new SnowmanContext
			{
				Status = SnowmanStatus.BEGIN,
				GameEventId = eventId,
				CountTotal = countTotal
			}).Send();
		}

		public void LogSnowmanProgress(string eventId, int countDone, float timeLeft, float timeLimit, string mapId)
		{
			base.AnalyticEventFactory.CreateEvent<SnowmanEvent, SnowmanContext>(new SnowmanContext
			{
				Status = SnowmanStatus.PROGRESS,
				GameEventId = eventId,
				CountDone = countDone,
				TimeLeft = timeLeft,
				TimeLimit = timeLimit,
				MapId = mapId
			}).Send();
		}

		public void LogSnowmanSuccess(string eventId, int countDone, float timeLeft, float timeLimit, string mapId)
		{
			base.AnalyticEventFactory.CreateEvent<SnowmanEvent, SnowmanContext>(new SnowmanContext
			{
				Status = SnowmanStatus.SUCCESS,
				GameEventId = eventId,
				CountDone = countDone,
				TimeLeft = timeLeft,
				TimeLimit = timeLimit,
				MapId = mapId
			}).Send();
		}

		public void LogSnowmanFail(string eventId, int countDone, float timeLeft, float timeLimit, string mapId)
		{
			base.AnalyticEventFactory.CreateEvent<SnowmanEvent, SnowmanContext>(new SnowmanContext
			{
				Status = SnowmanStatus.FAIL,
				GameEventId = eventId,
				CountDone = countDone,
				TimeLeft = timeLeft,
				TimeLimit = timeLimit,
				MapId = mapId
			}).Send();
		}

		public void LogSnowmanRewardClaimed(string eventId, string rewardId, bool rewardReceived)
		{
			base.AnalyticEventFactory.CreateEvent<SnowmanEvent, SnowmanContext>(new SnowmanContext
			{
				Status = SnowmanStatus.REWARD_CLAIMED,
				GameEventId = eventId,
				RewardId = rewardId,
				RewardReceived = rewardReceived
			}).Send();
		}

		public void LogSnowmanExit(string eventId)
		{
			base.AnalyticEventFactory.CreateEvent<SnowmanEvent, SnowmanContext>(new SnowmanContext
			{
				Status = SnowmanStatus.EXIT,
				GameEventId = eventId
			}).Send();
		}
	}
}
