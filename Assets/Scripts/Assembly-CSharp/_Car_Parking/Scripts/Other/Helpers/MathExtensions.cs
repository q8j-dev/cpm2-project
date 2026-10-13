namespace _Car_Parking.Scripts.Other.Helpers
{
	public static class MathExtensions
	{
		public static float Abs(this float value)
		{
			if (!(value < 0f))
			{
				return value;
			}
			return 0f - value;
		}
	}
}
