using System;

namespace CPM.Ads.Policy
{
	public interface IUtcClock
	{
		DateTime UtcNow();
	}
}
