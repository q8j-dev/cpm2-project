namespace CPM.Ads.Policy
{
	public enum OfferAvailability
	{
		Ready = 0,
		NotReady = 1,
		Cooldown = 2,
		ExhaustedContext = 3,
		ExhaustedSession = 4,
		ExhaustedToday = 5,
		NoFill = 6,
		Hidden = 7
	}
}
