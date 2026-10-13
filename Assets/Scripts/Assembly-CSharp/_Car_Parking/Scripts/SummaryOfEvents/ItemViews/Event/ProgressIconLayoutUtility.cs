namespace _Car_Parking.Scripts.SummaryOfEvents.ItemViews.Event
{
	public static class ProgressIconLayoutUtility
	{
		public static ProgressIconState GetState(int iconIndex, int progress, int maxProgress)
		{
			if (iconIndex >= maxProgress)
			{
				return ProgressIconState.Hidden;
			}
			if (iconIndex < progress)
			{
				return ProgressIconState.Filled;
			}
			return ProgressIconState.Empty;
		}

		public static bool CanShowIcons(int iconCount, int maxProgress)
		{
			if (maxProgress > 0)
			{
				return iconCount >= maxProgress;
			}
			return false;
		}
	}
}
