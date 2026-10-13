public static class SurfaceCodes
{
	public const int WireBits = 5;

	public static int WireCapacity => 32;

	public static bool FitsWire(Surface surface)
	{
		if (surface >= Surface.None)
		{
			return (int)surface < WireCapacity;
		}
		return false;
	}
}
