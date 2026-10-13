using System;
using _Car_Parking.Scripts.Drag.Engine;

namespace CPMEngine
{
	public class TransferCase
	{
		public const float MaxShiftHighRangeKmh = 60f;

		public const float MaxShiftLowRangeKmh = 5f;

		public const float MaxLockEnableKmh = 20f;

		public const string ReasonTooFastHighRange = "TransferCase.DenyTooFast4H";

		public const string ReasonTooFastLowRange = "TransferCase.DenyTooFast4L";

		public const string ReasonTooFastLock = "TransferCase.DenyTooFastLock";

		public const string ReasonNoLock = "TransferCase.DenyNoLock";

		public const string ReasonNoH2 = "TransferCase.DenyNoH2";

		public const string ReasonFrontLockH2 = "TransferCase.DenyFrontLockH2";

		public const string ReasonNoLowRange = "TransferCase.DenyNoLowRange";

		private readonly TransferCaseConfig _config;

		public TransferCaseMode Mode { get; private set; }

		public bool FrontLockOn { get; private set; }

		public bool RearLockOn { get; private set; }

		public bool CenterLockOn { get; private set; }

		public bool HasFrontLock => _config.HasFrontLock;

		public bool HasLowRange => _config.HasLowRange;

		public bool HasRearLock => _config.HasRearLock;

		public bool IsFullTime => _config.IsFullTime;

		public bool HasCenterLock => _config.HasCenterLock;

		public bool CenterLockedEffective
		{
			get
			{
				if (_config.IsFullTime && !CenterLockOn)
				{
					return Mode == TransferCaseMode.L4;
				}
				return true;
			}
		}

		public float FinalDriveMultiplier
		{
			get
			{
				if (Mode != TransferCaseMode.L4)
				{
					return 1f;
				}
				return _config.LowRangeRatio;
			}
		}

		public bool WhinesAudibly => Mode == TransferCaseMode.L4;

		public bool FrontLockAvailable
		{
			get
			{
				if (_config.HasFrontLock)
				{
					return Mode != TransferCaseMode.H2;
				}
				return false;
			}
		}

		public TransferCase(TransferCaseConfig config)
		{
			_config = config;
			Mode = ((config.IsFullTime && config.DefaultMode == TransferCaseMode.H2) ? TransferCaseMode.H4 : config.DefaultMode);
			if (!config.HasLowRange && Mode == TransferCaseMode.L4)
			{
				Mode = TransferCaseMode.H4;
			}
		}

		public static bool SupportsBase(GearboxType baseType)
		{
			if (baseType != GearboxType.Gear4Wd)
			{
				return baseType == GearboxType.Gear6Wd;
			}
			return true;
		}

		public GearboxType EffectiveGearType(GearboxType baseType)
		{
			if (baseType != GearboxType.Gear4Wd && baseType != GearboxType.GearAwd)
			{
				return baseType;
			}
			if (Mode != TransferCaseMode.H2)
			{
				return baseType;
			}
			return GearboxType.GearRwd;
		}

		public bool TrySetMode(TransferCaseMode target, float speedKmh, out string denyReason)
		{
			denyReason = null;
			if (target == Mode)
			{
				return true;
			}
			if (target == TransferCaseMode.H2 && _config.IsFullTime)
			{
				denyReason = "TransferCase.DenyNoH2";
				return false;
			}
			if (target == TransferCaseMode.L4 && !_config.HasLowRange)
			{
				denyReason = "TransferCase.DenyNoLowRange";
				return false;
			}
			float num = Math.Abs(speedKmh);
			if (target == TransferCaseMode.L4 || Mode == TransferCaseMode.L4)
			{
				if (num >= 5f)
				{
					denyReason = "TransferCase.DenyTooFast4L";
					return false;
				}
			}
			else if (num > 60f)
			{
				denyReason = "TransferCase.DenyTooFast4H";
				return false;
			}
			Mode = target;
			if (Mode == TransferCaseMode.H2)
			{
				FrontLockOn = false;
			}
			return true;
		}

		public bool TrySetFrontLock(bool on, float speedKmh, out string denyReason)
		{
			if (!TryValidateLockToggle(_config.HasFrontLock, on, speedKmh, out denyReason))
			{
				return false;
			}
			if (on && Mode == TransferCaseMode.H2)
			{
				denyReason = "TransferCase.DenyFrontLockH2";
				return false;
			}
			FrontLockOn = on;
			return true;
		}

		public bool TrySetRearLock(bool on, float speedKmh, out string denyReason)
		{
			if (!TryValidateLockToggle(_config.HasRearLock, on, speedKmh, out denyReason))
			{
				return false;
			}
			RearLockOn = on;
			return true;
		}

		public bool TrySetCenterLock(bool on, float speedKmh, out string denyReason)
		{
			if (!TryValidateLockToggle(_config.IsFullTime && _config.HasCenterLock, on, speedKmh, out denyReason))
			{
				return false;
			}
			CenterLockOn = on;
			return true;
		}

		private static bool TryValidateLockToggle(bool hasHardware, bool on, float speedKmh, out string denyReason)
		{
			denyReason = null;
			if (!hasHardware)
			{
				denyReason = "TransferCase.DenyNoLock";
				return false;
			}
			if (on && Math.Abs(speedKmh) > 20f)
			{
				denyReason = "TransferCase.DenyTooFastLock";
				return false;
			}
			return true;
		}

		public static LSDValues CreateLockedCopy(LSDValues src)
		{
			return new LSDValues
			{
				Type = src.Type,
				AccelBias = src.AccelBias,
				BrakeBias = src.BrakeBias,
				MaxBias = src.MaxBias,
				TorqueBiasRatio = src.TorqueBiasRatio,
				MaxResistance = src.MaxResistance,
				LockPercent = 100f
			};
		}
	}
}
