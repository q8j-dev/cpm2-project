using UnityEngine;
using _Car_Parking.Scripts.SummaryOfEvents.ItemViews.Event;

namespace _Car_Parking.Scripts.UIElement.StepProgress
{
	public static class StepProgressPlanner
	{
		public static ProgressCellPlan GetPlan(int index, int from, int to, int max)
		{
			if (index < 0)
			{
				return ProgressCellPlan.Hidden;
			}
			to = Mathf.Clamp(to, 0, Mathf.Max(max, 0));
			from = Mathf.Clamp(from, 0, to);
			switch (ProgressIconLayoutUtility.GetState(index, to, max))
			{
			default:
				return ProgressCellPlan.Empty;
			case ProgressIconState.Hidden:
				return ProgressCellPlan.Hidden;
			case ProgressIconState.Filled:
				if (index >= from)
				{
					return ProgressCellPlan.Animated;
				}
				return ProgressCellPlan.Filled;
			}
		}
	}
}
