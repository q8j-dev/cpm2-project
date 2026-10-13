namespace CPM.Ads
{
	public enum AdFailureCause
	{
		None = 0,
		NoConnection = 1,
		NoFill = 2,
		NotReady = 3,
		Timeout = 4,
		ReachedCap = 5,
		Rejected = 6,
		Configuration = 7,
		NotInitialized = 8,
		ManagerDisabled = 9,
		AlreadyDisplayed = 10,
		AppNotForeground = 11,
		Unknown = 12
	}
}
