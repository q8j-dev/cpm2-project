using System.Collections.Generic;

namespace _Car_Parking.Scripts.UIElement
{
	public static class ResultRowLayout
	{
		public static bool ShouldPinLocalRow(int rowCount, int pinAfterPlace, int localIndex)
		{
			if (pinAfterPlace <= 0)
			{
				return false;
			}
			if (localIndex < 0 || localIndex >= rowCount)
			{
				return false;
			}
			return localIndex + 1 > pinAfterPlace;
		}

		public static int FindLocalIndex<TRow>(IReadOnlyList<TRow> rows) where TRow : IResultRow
		{
			if (rows == null)
			{
				return -1;
			}
			for (int i = 0; i < rows.Count; i++)
			{
				if (rows[i].IsLocal)
				{
					return i;
				}
			}
			return -1;
		}
	}
}
