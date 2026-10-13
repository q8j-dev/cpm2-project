using System;
using System.Collections.Generic;

namespace Firebase.Errors
{
	[Serializable]
	public class GoogleError : FirebaseError
	{
		public int code;

		public string message;

		public List<GoogleInnerError> errors;
	}
}
