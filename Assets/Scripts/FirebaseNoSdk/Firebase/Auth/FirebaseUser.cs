using Cysharp.Threading.Tasks;
using Firebase.Errors;
using Unions;

namespace Firebase.Auth
{
	public abstract class FirebaseUser
	{
		public abstract string UserId { get; }

		public abstract string DisplayName { get; }

		public abstract bool IsEmailVerified { get; }

		public abstract string Email { get; }

		public abstract bool IsAnonymous { get; }

		public abstract Metadata Metadata { get; }

		public abstract UniTask<bool> SendEmailVerificationAsync();

		public abstract UniTask<Union<FirebaseUser, CreationError>> LinkWithCredential(Credential credential);
	}
}
