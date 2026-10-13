using Cysharp.Threading.Tasks;
using Firebase.Errors;
using Firebase.Impl;
using Unions;

namespace Firebase.Auth
{
	public abstract class FirebaseAuth
	{
		public static readonly FirebaseAuth DefaultInstance = new Firebase.Impl.Auth();

		public abstract FirebaseUser CurrentUser { get; }

		public abstract UniTask SendPasswordResetEmail(string email);

		public abstract UniTask<Union<FirebaseUser, CreationError>> CreateUserWithEmailAndPassword(string email, string password);

		public abstract UniTask<Union<FirebaseUser, SignInError>> SignInWithEmailAndPassword(string email, string password);

		public abstract void SignOut();

		public abstract UniTask RefreshToken(bool force = false);

		public abstract UniTask<Union<FirebaseUser, SignInError>> SignInAnonymously();

		public abstract UniTask<Union<FirebaseUser, SignInError>> SignInWithCustomToken(string customToken);
	}
}
