using System;
using System.Diagnostics;
using CPMEngine;
using UnityEngine;
using UnityEngine.Device;
using _Car_Parking;

public static class SteerAssistDiagnostics
{
	private readonly struct Snapshot : IEquatable<Snapshot>
	{
		public readonly SteerMode Mode;

		public readonly SteerAssistReason Reason;

		public readonly bool AssistField;

		public readonly bool IsDesktop;

		public readonly DesktopControllerType Desktop;

		public readonly MobileControllerType Mobile;

		public readonly bool KeepRow;

		public readonly bool FreeDrive;

		public readonly bool Racing;

		public readonly bool Autopilot;

		public readonly bool InputEnabled;

		public readonly AiType AiType;

		public readonly bool AiAssist;

		public readonly bool DriftMode;

		public readonly int ManagerId;

		public readonly int CarId;

		public Snapshot(SteerMode mode, SteerAssistReason reason, bool assistField, bool isDesktop, DesktopControllerType desktop, MobileControllerType mobile, bool keepRow, bool freeDrive, bool racing, bool autopilot, bool inputEnabled, AiType aiType, bool aiAssist, bool driftMode, int managerId, int carId)
		{
			Mode = mode;
			Reason = reason;
			AssistField = assistField;
			IsDesktop = isDesktop;
			Desktop = desktop;
			Mobile = mobile;
			KeepRow = keepRow;
			FreeDrive = freeDrive;
			Racing = racing;
			Autopilot = autopilot;
			InputEnabled = inputEnabled;
			AiType = aiType;
			AiAssist = aiAssist;
			DriftMode = driftMode;
			ManagerId = managerId;
			CarId = carId;
		}

		public bool Equals(Snapshot o)
		{
			if (Mode == o.Mode && Reason == o.Reason && AssistField == o.AssistField && IsDesktop == o.IsDesktop && Desktop == o.Desktop && Mobile == o.Mobile && KeepRow == o.KeepRow && FreeDrive == o.FreeDrive && Racing == o.Racing && Autopilot == o.Autopilot && InputEnabled == o.InputEnabled && AiType == o.AiType && AiAssist == o.AiAssist && DriftMode == o.DriftMode && ManagerId == o.ManagerId)
			{
				return CarId == o.CarId;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is Snapshot o)
			{
				return Equals(o);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (int)((uint)Mode ^ (uint)((int)Reason << 4) ^ (uint)ManagerId) ^ (CarId << 8);
		}
	}

	private const string Tag = "[SteerAssistDbg]";

	private const string PrefKey = "Stabilization";

	private static Snapshot _last;

	private static bool _hasLast;

	public static SteerAssistReason Explain(bool ignoreSteerInput, bool keepRow, bool isDesktop, DesktopControllerType desktop, bool autopilot)
	{
		if (autopilot)
		{
			return SteerAssistReason.Autopilot;
		}
		if (ignoreSteerInput)
		{
			return SteerAssistReason.IgnoreSteerInput;
		}
		if (keepRow)
		{
			return SteerAssistReason.KeepRow;
		}
		if (isDesktop && desktop == DesktopControllerType.SteeringWheel)
		{
			return SteerAssistReason.PhysicalWheel;
		}
		return SteerAssistReason.Toggle;
	}

	[Conditional("DEVELOPMENT_BUILD")]
	[Conditional("UNITY_EDITOR")]
	public static void Sample(CarInputManager manager, bool isDesktop, bool autopilot, SteerMode resolvedMode, CarAIController ai)
	{
		if ((bool)manager)
		{
			SteerAssistReason reason = Explain(manager.IgnoreSteerInput, FlagsStorage.KeepRow, isDesktop, manager.DesktopControllerType, autopilot);
			SteerMode mode = (autopilot ? SteerMode.Stabilized : resolvedMode);
			bool flag = ai;
			Powertrain getPowertrain = manager.GetPowertrain;
			Snapshot s = new Snapshot(mode, reason, manager.SteerAssistance, isDesktop, manager.DesktopControllerType, manager.MobileControllerType, FlagsStorage.KeepRow, FlagsStorage.IsFreeDriveMode, FlagsStorage.RacingGameMode, autopilot, FlagsStorage.IsInputEnabledInThisWindow, flag ? ai.AiType : AiType.None, flag && ai.SteerAssistent, manager.DriftMode, manager.GetInstanceID(), getPowertrain ? getPowertrain.GetInstanceID() : 0);
			if (!_hasLast || !s.Equals(_last))
			{
				_last = s;
				_hasLast = true;
				Report(in s, getPowertrain ? getPowertrain.name : "<none>");
			}
		}
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	public static void LogInitHelpers(CarInputManager manager, bool isDesktop)
	{
		if ((bool)manager)
		{
			bool flag = UnityEngine.Device.SystemInfo.deviceType == DeviceType.Desktop;
		}
	}

	[Conditional("UNITY_EDITOR")]
	[Conditional("DEVELOPMENT_BUILD")]
	public static void LogToggle(bool isOn, CarInputManager target)
	{
		if ((bool)target)
		{
			_ = $"mgr={target.GetInstanceID()} SteerAssistance={target.SteerAssistance}";
		}
	}

	private static void Report(in Snapshot s, string carName)
	{
		int rawPref = PlayerPrefs.GetInt("Stabilization", -1);
		if (s.Mode == SteerMode.Stabilized)
		{
			_ = s.Reason;
		}
		if (s.KeepRow && s.FreeDrive)
		{
			_ = s.Racing;
		}
		_ = s.AiType;
		bool flag = ExpectedFieldFromPref(in s, rawPref);
		_ = s.AssistField;
	}

	private static bool ExpectedFieldFromPref(in Snapshot s, int rawPref)
	{
		if (s.IsDesktop)
		{
			if (s.Desktop == DesktopControllerType.Keyboard)
			{
				return ((rawPref == -1) ? 1 : rawPref) == 1;
			}
			return false;
		}
		if (CarInputManager.SupportsSteerAssist(s.Mobile))
		{
			return ((rawPref != -1) ? rawPref : 0) == 1;
		}
		return false;
	}

	private static string PrefToString()
	{
		if (!PlayerPrefs.HasKey("Stabilization"))
		{
			return "unset";
		}
		return PlayerPrefs.GetInt("Stabilization").ToString();
	}
}
