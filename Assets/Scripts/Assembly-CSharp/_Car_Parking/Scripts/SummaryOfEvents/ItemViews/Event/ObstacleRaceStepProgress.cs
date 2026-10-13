using System;

namespace _Car_Parking.Scripts.SummaryOfEvents.ItemViews.Event
{
	public static class ObstacleRaceStepProgress
	{
		public static (int Done, int Size) Calculate(int[] steps, int progress, bool claimable)
		{
			int i;
			for (i = 0; i < steps.Length && steps[i] <= progress; i++)
			{
			}
			int num;
			int num2;
			if (!claimable)
			{
				num = ((i >= steps.Length) ? 1 : 0);
				if (num == 0)
				{
					num2 = i;
					goto IL_0032;
				}
			}
			else
			{
				num = 1;
			}
			num2 = Math.Max(i - 1, 0);
			goto IL_0032;
			IL_0032:
			int num3 = num2;
			int num4 = ((num3 > 0) ? steps[num3 - 1] : 0);
			int num5 = Math.Max(steps[num3] - num4, 0);
			return (Done: (num != 0) ? num5 : Math.Clamp(progress - num4, 0, num5), Size: num5);
		}
	}
}
