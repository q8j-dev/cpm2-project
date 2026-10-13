using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	public interface IGuestAccountCreator
	{
		UniTask<bool> CreateGuestAccountIfNeeds(bool showWaitingScreen = false, bool showErrorMessage = false);
	}
}
