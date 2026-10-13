namespace CPM.Ads.Policy
{
	public readonly struct GlobalRules
	{
		public readonly int DailyCap;

		public readonly int SessionCap;

		public readonly int SuccessCooldownSec;

		public readonly int TechnicalCooldownSec;

		public readonly int NoFillLockSec;

		public GlobalRules(int dailyCap, int sessionCap, int successCooldownSec, int technicalCooldownSec, int noFillLockSec)
		{
			DailyCap = dailyCap;
			SessionCap = sessionCap;
			SuccessCooldownSec = successCooldownSec;
			TechnicalCooldownSec = technicalCooldownSec;
			NoFillLockSec = noFillLockSec;
		}
	}
}
