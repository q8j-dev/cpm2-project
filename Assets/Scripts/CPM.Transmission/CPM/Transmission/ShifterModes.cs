namespace CPM.Transmission
{
	public static class ShifterModes
	{
		public static bool SupportsManual(GearboxKind kind, AutoShifterModes modes)
		{
			if (kind != GearboxKind.MT && kind != GearboxKind.Sequential)
			{
				return (modes & AutoShifterModes.Manual) != 0;
			}
			return true;
		}
	}
}
