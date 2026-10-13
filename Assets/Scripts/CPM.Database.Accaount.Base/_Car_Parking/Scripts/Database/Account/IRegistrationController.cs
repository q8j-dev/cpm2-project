using CPM.Account;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Database.Account
{
	public interface IRegistrationController
	{
		string PlayerNameChanged(string newName);

		UniTask SignOut();

		UniTask<(bool, string)> ResetPassword(string email);

		UniTask<(SignUpResult, bool)> CreateNewAccount(string email, string password);

		UniTask<bool> SignInNew(string email, string password);

		UniTask<bool> SignInWithSteamNew();
	}
}
