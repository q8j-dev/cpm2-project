using System;

namespace CPM.Cdn
{
	public sealed class CdnFetchRequest
	{
		public string Url;

		public EtagFileCache Cache;

		public int[] AttemptTimeoutsSeconds = new int[3] { 5, 5, 5 };

		public TimeSpan RetryDelay = TimeSpan.FromSeconds(2.0);

		public Func<byte[], bool> AcceptPayload;

		public string Label = "Fetch";
	}
}
