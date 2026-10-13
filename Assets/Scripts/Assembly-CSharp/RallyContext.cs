using System;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Types;

[Serializable]
public class RallyContext : IEventContext
{
	public RallyStatus Status { get; set; }

	public RallyModeType ModeType { get; set; }

	public string CampaignId { get; set; }

	public string SeriesId { get; set; }

	public string EventId { get; set; }

	public RallyStageType StageType { get; set; }

	public int StageIndex { get; set; }

	public string TrajectoryId { get; set; }

	public string TyreType { get; set; }

	public string CarId { get; set; }

	public string CarClass { get; set; }

	public int AttemptNumber { get; set; }

	public int RestartCount { get; set; }

	public int NgPlusLevel { get; set; }

	public bool Special { get; set; }

	public float LapTime { get; set; }

	public float RequiredValue { get; set; }

	public bool IsPersonalRecord { get; set; }

	public RallyMedalType MedalType { get; set; }

	public RallyMedalType PrevMedalType { get; set; }

	public int CheckpointMissed { get; set; }

	public float CheckpointPenaltyTotal { get; set; }

	public RallyFailReason FailReason { get; set; }

	public string RewardId { get; set; }
}
