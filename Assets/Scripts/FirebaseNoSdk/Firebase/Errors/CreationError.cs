namespace Firebase.Errors
{
	public abstract class CreationError : FirebaseError
	{
		public class EmailExists : CreationError
		{
		}

		public class Unknown : CreationError
		{
		}
	}
}
