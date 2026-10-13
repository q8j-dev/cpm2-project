namespace CPM.Transmission
{
	public readonly struct GearboxProfile
	{
		public readonly GearboxKind Kind;

		public readonly bool HasTorqueConverter;

		public readonly bool HasClutchSlip;

		public readonly bool IsPreselective;

		public readonly bool IsContinuous;

		public readonly bool IsSingleSpeed;

		public readonly bool DefaultAutomatic;

		public readonly bool DefaultAutoClutch;

		public readonly float KStall;

		public readonly float TccLockEnterRatio;

		public readonly float TccLockExitRatio;

		public readonly float TccLockMinVeloEnter;

		public readonly float TccLockMinVeloExit;

		public readonly float StallCoupling;

		public readonly float StallFraction;

		public readonly float StallEngMin;

		public readonly float AmtClutchOpenMs;

		public readonly float AmtGearEngageMs;

		public readonly float AmtClutchCloseMs;

		public readonly float AmtRevMatchMaxMs;

		public readonly bool AmtThrottleCutsOnEngage;

		public readonly float AmtLaunchBite;

		public readonly float AmtLaunchSpeed;

		public readonly float DctHandoverMs;

		public readonly float DctPreShiftMs;

		public readonly int CvtVirtualGears;

		public readonly float CvtUpshiftIdleFrac;

		public readonly float CvtUpshiftPowerFrac;

		public readonly float CvtDownshiftHysteresis;

		public readonly float CvtSportIdleFrac;

		public readonly float CvtSportUpshiftPowerFrac;

		public readonly float CvtSportDownshiftHyst;

		public readonly float SeqShiftMs;

		public GearboxProfile(GearboxKind kind, bool hasTorqueConverter, bool hasClutchSlip, bool isPreselective, bool isContinuous, bool isSingleSpeed, bool defaultAutomatic, bool defaultAutoClutch)
			: this(kind, hasTorqueConverter, hasClutchSlip, isPreselective, isContinuous, isSingleSpeed, defaultAutomatic, defaultAutoClutch, 25f, 15f, 40f, 250f, 0.6f, 4f, amtThrottleCutsOnEngage: true)
		{
		}

		private GearboxProfile(GearboxKind kind, bool hasTorqueConverter, bool hasClutchSlip, bool isPreselective, bool isContinuous, bool isSingleSpeed, bool defaultAutomatic, bool defaultAutoClutch, float amtClutchOpenMs, float amtGearEngageMs, float amtClutchCloseMs, float amtRevMatchMaxMs, float amtLaunchBite, float amtLaunchSpeed, bool amtThrottleCutsOnEngage)
		{
			Kind = kind;
			HasTorqueConverter = hasTorqueConverter;
			HasClutchSlip = hasClutchSlip;
			IsPreselective = isPreselective;
			IsContinuous = isContinuous;
			IsSingleSpeed = isSingleSpeed;
			DefaultAutomatic = defaultAutomatic;
			DefaultAutoClutch = defaultAutoClutch;
			KStall = 3f;
			TccLockEnterRatio = 0.8f;
			TccLockExitRatio = 0.7f;
			TccLockMinVeloEnter = 8f;
			TccLockMinVeloExit = 5f;
			StallCoupling = 0.3f;
			StallFraction = 0.6f;
			StallEngMin = 0.2f;
			AmtClutchOpenMs = amtClutchOpenMs;
			AmtGearEngageMs = amtGearEngageMs;
			AmtClutchCloseMs = amtClutchCloseMs;
			AmtRevMatchMaxMs = amtRevMatchMaxMs;
			AmtThrottleCutsOnEngage = amtThrottleCutsOnEngage;
			AmtLaunchBite = amtLaunchBite;
			AmtLaunchSpeed = amtLaunchSpeed;
			DctHandoverMs = 120f;
			DctPreShiftMs = 60f;
			CvtVirtualGears = 6;
			CvtUpshiftIdleFrac = 0.4f;
			CvtUpshiftPowerFrac = 0.92f;
			CvtDownshiftHysteresis = 0.6f;
			CvtSportIdleFrac = 0.7f;
			CvtSportUpshiftPowerFrac = 0.98f;
			CvtSportDownshiftHyst = 0.8f;
			SeqShiftMs = 90f;
		}

		public static GearboxProfile For(GearboxKind kind)
		{
			return kind switch
			{
				GearboxKind.MT => new GearboxProfile(kind, hasTorqueConverter: false, hasClutchSlip: true, isPreselective: false, isContinuous: false, isSingleSpeed: false, defaultAutomatic: false, defaultAutoClutch: false, 100f, 80f, 120f, 300f, 0.5f, 4f, amtThrottleCutsOnEngage: true), 
				GearboxKind.AT => new GearboxProfile(kind, hasTorqueConverter: true, hasClutchSlip: false, isPreselective: false, isContinuous: false, isSingleSpeed: false, defaultAutomatic: true, defaultAutoClutch: true), 
				GearboxKind.AMT => new GearboxProfile(kind, hasTorqueConverter: false, hasClutchSlip: true, isPreselective: false, isContinuous: false, isSingleSpeed: false, defaultAutomatic: true, defaultAutoClutch: true), 
				GearboxKind.DCT => new GearboxProfile(kind, hasTorqueConverter: false, hasClutchSlip: true, isPreselective: true, isContinuous: false, isSingleSpeed: false, defaultAutomatic: true, defaultAutoClutch: true), 
				GearboxKind.CVT => new GearboxProfile(kind, hasTorqueConverter: true, hasClutchSlip: false, isPreselective: false, isContinuous: true, isSingleSpeed: false, defaultAutomatic: true, defaultAutoClutch: true), 
				GearboxKind.Sequential => new GearboxProfile(kind, hasTorqueConverter: false, hasClutchSlip: true, isPreselective: false, isContinuous: false, isSingleSpeed: false, defaultAutomatic: false, defaultAutoClutch: true), 
				GearboxKind.DirectDrive => new GearboxProfile(kind, hasTorqueConverter: true, hasClutchSlip: false, isPreselective: false, isContinuous: false, isSingleSpeed: true, defaultAutomatic: true, defaultAutoClutch: true), 
				_ => For(GearboxKind.AT), 
			};
		}

		public static GearboxKind LegacyMtFlagToKind(bool legacyMt)
		{
			if (!legacyMt)
			{
				return GearboxKind.AT;
			}
			return GearboxKind.MT;
		}
	}
}
