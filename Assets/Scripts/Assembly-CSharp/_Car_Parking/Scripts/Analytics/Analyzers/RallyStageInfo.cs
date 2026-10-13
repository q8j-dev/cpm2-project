using _Car_Parking.Scripts.Analytics.Types;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	public struct RallyStageInfo
	{
		public RallyModeType ModeType;

		public int LocationIndex;

		public int TrajectoryIndex;

		public int StageIndex;

		public bool IsNight;

		public string TrajectoryId;

		public string LocationName;

		public string TyreType;

		public string CarId;

		public string CarClass;

		public float CheckpointPenaltyTotal;

		public int NgPlusLevel;

		public bool Special;
	}
}
