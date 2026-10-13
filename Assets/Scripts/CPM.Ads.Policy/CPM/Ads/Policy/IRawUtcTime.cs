using System;

namespace CPM.Ads.Policy
{
	public interface IRawUtcTime
	{
		DateTime UtcNow { get; }
	}
}
