using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class SurvivalEliminationTimes
	{
		public static double?[] Build(IReadOnlyList<int> eliminationOrder, Func<int, double?> claimOf)
		{
			if (eliminationOrder == null)
			{
				return Array.Empty<double?>();
			}
			double?[] array = new double?[eliminationOrder.Count];
			if (claimOf == null)
			{
				return array;
			}
			double num = 0.0;
			for (int i = 0; i < eliminationOrder.Count; i++)
			{
				double? num2 = claimOf(eliminationOrder[i]);
				if (num2.HasValue)
				{
					double num3 = ((num2.Value > num) ? num2.Value : num);
					array[i] = num3;
					num = num3;
				}
			}
			return array;
		}
	}
}
