using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public struct RallyStageResult
	{
		public float LapTime;

		public float RequiredValue;

		public bool IsPersonalRecord;

		public RallyMedalType MedalType;

		public RallyMedalType PrevMedalType;

		public int CheckpointMissed;

		public float CheckpointPenaltyTotal;

		public string RewardId;
	}
}
