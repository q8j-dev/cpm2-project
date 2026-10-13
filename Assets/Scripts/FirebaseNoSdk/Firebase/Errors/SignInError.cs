namespace Firebase.Errors
{
	public class SignInError : FirebaseError
	{
		public readonly GoogleError Error;

		public SignInError(GoogleError error)
		{
			Error = error;
		}
	}
}
