using CPM.Transmission;

namespace CPMEngine
{
	public interface ITransmissionHost
	{
		float CurrentRpm { get; }

		float WEngine { get; }

		float WDiff { get; }

		float Ratio { get; }

		float IdleRpm { get; }

		float EngineTorque { get; }

		float ClutchCapacity { get; }

		float LongAccel { get; }

		float RevMatchBlip { get; set; }

		float Redline { get; }

		float Velocity { get; }

		bool AutoClutch { get; }

		GearboxType GearType { get; }

		bool IsActiveLaunchControlNow { get; }

		bool DriveDecoupled { get; }

		bool OverheatTransmission { get; }

		float TcLockupRpm { get; }

		float TargetGearTotalRatio { get; }

		float Starter { get; }

		float FinalDrive { get; }

		float DrivenWheelRadius { get; }

		float ThrottleDropTimer { get; }

		float ThrottleRecentMax { get; }

		float EngineRevLimitedSustained { get; }

		float Clutch { get; set; }

		float ClutchPedalInput { get; }

		float TcMult { get; set; }

		bool TccLocked { get; set; }

		bool TcSlipLatched { get; set; }

		float ClutchB { get; set; }

		int DormantGear { get; set; }

		float RatioB { get; }

		float RatioOverride { get; set; }

		bool SportMode { get; }

		bool LowMode { get; }

		int LowMaxGear { get; }

		float ThrottleOverride { get; set; }

		int CurrentGear { get; set; }

		int TargetGear { get; set; }

		int NumGears { get; }

		float GearTimer { get; }

		float GearShiftingDelay { get; }

		float AccelWant { get; set; }

		float Throttle { get; }

		float ThrottleIntent { get; }

		float Brake { get; }

		float Handbrake { get; }

		GearboxState GearboxState { get; }

		bool DynorunMode { get; }

		GearboxProfile Profile { get; }

		float StallFloorRpm { get; }

		bool EngineWorks { get; }

		float ClutchEngagement { get; }

		float IdleGovernorScale { get; set; }

		float GearRatio(int index);

		void StallEngine();
	}
}
