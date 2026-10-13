using System;

namespace Firebase.Errors
{
	[Serializable]
	public class GoogleInnerError : FirebaseError
	{
		public string message;

		public string domain;

		public string reason;
	}
}
