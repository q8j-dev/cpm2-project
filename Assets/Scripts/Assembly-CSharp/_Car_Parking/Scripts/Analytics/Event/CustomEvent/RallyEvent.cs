using System.Collections.Generic;
using _Car_Parking.Scripts.Analytics.Root;
using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class RallyEvent : DefaultEvent<RallyContext>
	{
		protected override string Name => "rally";

		protected override Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"status",
						base.Context.Status
					},
					{
						"mode_type",
						base.Context.ModeType
					}
				};
				switch (base.Context.Status)
				{
				case RallyStatus.STAGE_STARTED:
					AddCampaignKeys(dictionary);
					AddStageKeys(dictionary);
					dictionary["tyre_type"] = base.Context.TyreType;
					AddCarKeys(dictionary);
					AddAttemptKeys(dictionary);
					dictionary["ng_plus_level"] = base.Context.NgPlusLevel;
					dictionary["special"] = base.Context.Special;
					dictionary["checkpoint_penalty_total"] = base.Context.CheckpointPenaltyTotal;
					break;
				case RallyStatus.STAGE_COMPLETED:
					AddCampaignKeys(dictionary);
					AddStageKeys(dictionary);
					dictionary["tyre_type"] = base.Context.TyreType;
					AddCarKeys(dictionary);
					AddAttemptKeys(dictionary);
					dictionary["ng_plus_level"] = base.Context.NgPlusLevel;
					dictionary["special"] = base.Context.Special;
					dictionary["lap_time"] = base.Context.LapTime;
					dictionary["required_value"] = base.Context.RequiredValue;
					dictionary["is_personal_record"] = base.Context.IsPersonalRecord;
					dictionary["medal_type"] = base.Context.MedalType;
					dictionary["prev_medal_type"] = base.Context.PrevMedalType;
					dictionary["checkpoint_missed"] = base.Context.CheckpointMissed;
					dictionary["checkpoint_penalty_total"] = base.Context.CheckpointPenaltyTotal;
					dictionary["reward_id"] = base.Context.RewardId;
					break;
				case RallyStatus.STAGE_FAILED:
					AddCampaignKeys(dictionary);
					AddStageKeys(dictionary);
					AddCarKeys(dictionary);
					AddAttemptKeys(dictionary);
					dictionary["checkpoint_penalty_total"] = base.Context.CheckpointPenaltyTotal;
					dictionary["fail_reason"] = base.Context.FailReason;
					break;
				case RallyStatus.STAGE_RESTARTED:
					AddCampaignKeys(dictionary);
					AddStageKeys(dictionary);
					AddCarKeys(dictionary);
					AddAttemptKeys(dictionary);
					dictionary["checkpoint_penalty_total"] = base.Context.CheckpointPenaltyTotal;
					break;
				}
				return dictionary;
			}
		}

		public RallyEvent(CPMAnalytics analytics, RallyContext context)
			: base(analytics, context)
		{
		}

		private void AddCampaignKeys(Dictionary<string, object> p)
		{
			p["campaign_id"] = base.Context.CampaignId;
			p["series_id"] = base.Context.SeriesId;
			p["event_id"] = base.Context.EventId;
		}

		private void AddStageKeys(Dictionary<string, object> p)
		{
			p["stage_type"] = base.Context.StageType;
			p["stage_index"] = base.Context.StageIndex;
			p["trajectory_id"] = base.Context.TrajectoryId;
		}

		private void AddCarKeys(Dictionary<string, object> p)
		{
			p["car_id"] = base.Context.CarId;
			p["car_class"] = base.Context.CarClass;
		}

		private void AddAttemptKeys(Dictionary<string, object> p)
		{
			p["attempt_number"] = base.Context.AttemptNumber;
			p["restart_count"] = base.Context.RestartCount;
		}
	}
}
