namespace CPM.Cdn
{
	public sealed class CdnGetResult
	{
		public CdnGetStatus Status { get; private set; }

		public byte[] Bytes { get; private set; }

		public string Etag { get; private set; }

		public string ErrorMessage { get; private set; }

		public static CdnGetResult Ok(byte[] bytes, string etag)
		{
			return new CdnGetResult
			{
				Status = CdnGetStatus.Ok,
				Bytes = bytes,
				Etag = etag
			};
		}

		public static CdnGetResult NotModified()
		{
			return new CdnGetResult
			{
				Status = CdnGetStatus.NotModified
			};
		}

		public static CdnGetResult Error(string message)
		{
			return new CdnGetResult
			{
				Status = CdnGetStatus.Error,
				ErrorMessage = message
			};
		}
	}
}
