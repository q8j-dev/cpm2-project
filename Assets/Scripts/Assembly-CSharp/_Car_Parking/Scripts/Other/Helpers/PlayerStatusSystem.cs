using CPM.Account;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.AccountSystemNew;

namespace _Car_Parking.Scripts.Other.Helpers
{
	[Preserve]
	public class PlayerStatusSystem : IPlayerStatusSystem
	{
		public const string GameModePlayedPrefsKey = "GameModePlayed01";

		private IAccountController _accountController;

		public bool IsOldPlayer
		{
			get
			{
				if (PlayerPrefs.GetInt("GameModePlayed01", 0) != 1)
				{
					if (_accountController.HasEmail)
					{
						return !AccountSystemV2Extension.IsGuestAccount(_accountController.Email);
					}
					return false;
				}
				return true;
			}
		}

		public PlayerStatusSystem(IAccountController accountController)
		{
			_accountController = accountController;
		}

		public void OnGameModePlayed()
		{
			PlayerPrefs.SetInt("GameModePlayed01", 1);
		}
	}
}
