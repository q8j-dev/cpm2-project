using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Generic;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class RacingAnalytic : AnalyticProviderBase
	{
		private readonly RacingProvider _racingProvider;

		public RacingAnalytic(AnalyticEventFactory analyticEventFactory, RacingProvider racingProvider)
			: base(analyticEventFactory)
		{
			_racingProvider = racingProvider;
		}

		public void LogRaceStart(RaceType raceMode)
		{
			CircuitRaceControlller.HardCurrency = 0;
			CircuitRaceControlller.SoftReward = 0;
			base.AnalyticEventFactory.CreateEvent<RaceStartEvent, RaceStartContext>(new RaceStartContext
			{
				CarId = AnalyticsHelper.GetCurrentCarId(),
				RaceMode = raceMode.ToString()
			}).Send();
		}

		public void LogRaceFinish(LapRaceCar lapRaceCar, float modeDuration, string exitReason)
		{
			int countAwards = -1;
			if (lapRaceCar.place < 3 && CircuitRaceControlller.SoftReward != 0 && CircuitRaceControlller.SoftReward != 0)
			{
				countAwards = lapRaceCar.place + 1;
			}
			int rewardSoft = ((exitReason != "quit_manual") ? CircuitRaceControlller.SoftReward : 0);
			int rewardHard = ((exitReason != "quit_manual") ? CircuitRaceControlller.HardCurrency : 0);
			base.AnalyticEventFactory.CreateEvent<RaceFinishEvent, RaceFinishContext>(new RaceFinishContext
			{
				RacePosition = lapRaceCar.place + 1,
				RaceFinishTime = lapRaceCar.GetClearTime(),
				BestLap = lapRaceCar.GetBestLap(),
				ModeDuration = modeDuration,
				Penalty = lapRaceCar.GetPenaltyTime(),
				CountAwards = countAwards,
				RewardSoft = rewardSoft,
				RewardHard = rewardHard,
				ExitReason = exitReason
			}).Send();
		}

		public void LogDragRacingStart()
		{
			base.AnalyticEventFactory.CreateEvent<DragRacingStartEvent, DefaultContext>(new DefaultContext()).Send();
		}

		public void LogDragRacingFinish()
		{
			if ((bool)_racingProvider.DragRacingController)
			{
				base.AnalyticEventFactory.CreateEvent<DragRacingFinishEvent, DragRacingFinishContext>(new DragRacingFinishContext
				{
					Duration = _racingProvider.DragRacingController.PassedInModeTime
				}).Send();
			}
		}
	}
}
