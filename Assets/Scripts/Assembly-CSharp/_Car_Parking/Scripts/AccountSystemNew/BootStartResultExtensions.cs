using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	public static class BootStartResultExtensions
	{
		public static (bool, SceneNotificationReason) ToSceneNotificationReason(BootStartResult result)
		{
			switch (result)
			{
			case BootStartResult.CantPlayGuest:
				return (true, SceneNotificationReason.WrongVersionAuthorizationGuest);
			case BootStartResult.CantPlayMain:
				return (true, SceneNotificationReason.WrongVersionAuthorizationMain);
			case BootStartResult.ErrorSignInGuest:
				return (true, SceneNotificationReason.WrongAuthorizationGuest);
			case BootStartResult.ErrorSignInPrefs:
				return (true, SceneNotificationReason.WrongAuthorizationMain);
			case BootStartResult.ErrorMasterMainStartupResult:
			case BootStartResult.DeserializeCarsError:
			case BootStartResult.DeserializePlayerRecordsError:
				return (true, SceneNotificationReason.SignInDeserializeDataError);
			case BootStartResult.CheckInternetConnection:
				return (true, SceneNotificationReason.NetworkError);
			default:
				return (false, SceneNotificationReason.AppUpdate);
			}
		}
	}
}
