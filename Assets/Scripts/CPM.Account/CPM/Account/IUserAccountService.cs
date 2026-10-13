using Cysharp.Threading.Tasks;
using Firebase.Auth;

namespace CPM.Account
{
	public interface IUserAccountService
	{
		string LastSignInError { get; }

		UniTask<FirebaseUser> SignIn(string email, string password, bool resetData);

		UniTask DeleteAccount();

		void SignOut();

		UniTask<bool> CheckDependencies();

		UniTask<bool> ResetPassword(string email);

		UniTask<SignUpData> ChangeEmailPassword(string email, string password);

		UniTask<bool> HaveGuestAccount();
	}
}
