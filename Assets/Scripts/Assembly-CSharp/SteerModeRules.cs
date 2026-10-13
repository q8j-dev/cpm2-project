using CPMEngine;

public static class SteerModeRules
{
	public static SteerMode Resolve(bool ignoreSteerInput, bool keepRow, bool assistEnabled, bool isDesktop, DesktopControllerType desktop)
	{
		if (ignoreSteerInput)
		{
			return SteerMode.Raw;
		}
		if (keepRow)
		{
			return SteerMode.Stabilized;
		}
		if (isDesktop && desktop == DesktopControllerType.SteeringWheel)
		{
			return SteerMode.Raw;
		}
		if (!assistEnabled)
		{
			return SteerMode.Raw;
		}
		return SteerMode.Stabilized;
	}
}
