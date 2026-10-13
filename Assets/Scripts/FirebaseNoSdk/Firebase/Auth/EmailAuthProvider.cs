using System;

namespace Firebase.Auth
{
	public abstract class EmailAuthProvider
	{
		public static Credential GetCredential(string email, string password)
		{
			throw new NotImplementedException();
		}
	}
}
