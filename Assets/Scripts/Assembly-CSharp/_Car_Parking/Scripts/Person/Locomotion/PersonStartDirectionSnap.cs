using System;

namespace _Car_Parking.Scripts.Person.Locomotion
{
	public static class PersonStartDirectionSnap
	{
		private const float ForwardLimit = 45f;

		private const float SideLimit = 135f;

		public static float Snap(float startAngle)
		{
			float num = Math.Abs(startAngle);
			if (num < 45f)
			{
				return 0f;
			}
			float num2 = ((startAngle < 0f) ? (-1f) : 1f);
			if (!(num < 135f))
			{
				return 180f * num2;
			}
			return 90f * num2;
		}

		public static float YawScale(float startAngle)
		{
			float num = Snap(startAngle);
			if (num == 0f)
			{
				return 1f;
			}
			return Math.Abs(startAngle) / Math.Abs(num);
		}
	}
}
