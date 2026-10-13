using System;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Types;

[Serializable]
public class TimeAttackContext : IEventContext
{
	public TimeAttackStatus Status { get; set; }

	public string FailReason { get; set; }

	public string CarId { get; set; }

	public string GameEventId { get; set; }

	public int EventDurationDays { get; set; }

	public string CarClass { get; set; }

	public OpenWorldType ModeType { get; set; }

	public float LapTime { get; set; }

	public int Ttd_code { get; set; }

	public bool IsSuccessful { get; set; }

	public bool IsPersonalRecord { get; set; }

	public int AttemptNumber { get; set; }

	public bool RewardRecieved { get; set; }

	public string RewardId { get; set; }

	public string RewardType { get; set; }

	public int RestartCount { get; set; }

	public string CheckPoints { get; set; }

	public int CheckpointsCount { get; set; }

	public int CheckpointMissed { get; set; }

	public bool FinishValid { get; set; }

	public int ValidationRuleCode { get; set; }

	public float CheckpointPenaltyTotal { get; set; }

	public string EventVariant { get; set; }

	public string RequiredCarClasses { get; set; }

	public float RequiredValue { get; set; }

	public string TrackLayoutId { get; set; }

	public string TrajectoryId { get; set; }

	public int DriftScoreTotal { get; set; }

	public int DriftBaseScoreTotal { get; set; }

	public int DriftComboXMax { get; set; }

	public int DriftChangeComboCount { get; set; }

	public string DriftChainBreakCounts { get; set; }
}
