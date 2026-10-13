namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class RebindDevicesExtensions
	{
		public static bool Includes(this RebindDevices devices, RebindDeviceKind kind)
		{
			return ((uint)devices & (uint)(1 << (int)kind)) != 0;
		}
	}
}
