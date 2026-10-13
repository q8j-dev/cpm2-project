namespace CPM.Cdn
{
	public sealed class CdnFetchResult
	{
		public CdnFetchStatus Status { get; private set; }

		public byte[] Bytes { get; private set; }

		public string LastError { get; private set; }

		public static CdnFetchResult Updated(byte[] bytes)
		{
			return new CdnFetchResult
			{
				Status = CdnFetchStatus.Updated,
				Bytes = bytes
			};
		}

		public static CdnFetchResult NotModified(byte[] cachedBytes)
		{
			return new CdnFetchResult
			{
				Status = CdnFetchStatus.NotModified,
				Bytes = cachedBytes
			};
		}

		public static CdnFetchResult Failed(string lastError)
		{
			return new CdnFetchResult
			{
				Status = CdnFetchStatus.Failed,
				LastError = lastError
			};
		}
	}
}
