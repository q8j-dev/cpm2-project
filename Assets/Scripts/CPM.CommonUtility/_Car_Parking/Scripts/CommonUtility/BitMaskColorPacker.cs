using UnityEngine;

namespace _Car_Parking.Scripts.CommonUtility
{
	public static class BitMaskColorPacker
	{
		private const uint RedMask = 255u;

		private const uint GreenMask = 65280u;

		private const uint BlueMask = 16711680u;

		private const uint AlphaMask = 4278190080u;

		public static uint PackColor(Color color)
		{
			return 0 | (uint)(color.r * 255f) | ((uint)(color.g * 255f) << 8) | ((uint)(color.b * 255f) << 16) | ((uint)(color.a * 255f) << 24);
		}

		public static Color UnpackColor(uint bitmask)
		{
			float r = (float)(bitmask & 0xFF) / 255f;
			float g = (float)((bitmask & 0xFF00) >> 8) / 255f;
			float b = (float)((bitmask & 0xFF0000) >> 16) / 255f;
			float a = (float)((bitmask & 0xFF000000u) >> 24) / 255f;
			return new Color(r, g, b, a);
		}
	}
}
