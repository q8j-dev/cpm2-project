namespace Firebase.Errors
{
	public class HttpError : FirebaseError
	{
		private const string AuthExceptionMarker = "FirebaseAuthException";

		public long ResponseCode;

		public string Body;

		public bool IsAuthFailure
		{
			get
			{
				if (ResponseCode != 401 && ResponseCode != 403)
				{
					if (Body != null)
					{
						return Body.Contains("FirebaseAuthException");
					}
					return false;
				}
				return true;
			}
		}

		public HttpError()
		{
		}

		public HttpError(long responseCode, string body = null)
		{
			ResponseCode = responseCode;
			Body = body;
		}
	}
}
