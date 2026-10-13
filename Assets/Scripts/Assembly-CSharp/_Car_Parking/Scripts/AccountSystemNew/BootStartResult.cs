namespace _Car_Parking.Scripts.AccountSystemNew
{
	public enum BootStartResult
	{
		Success = 0,
		SuccessOfflineMode = 1,
		ErrorSignInGuest = 2,
		ErrorMasterMainStartupResult = 3,
		ErrorSignInPrefs = 4,
		CantPlayGuest = 5,
		CantPlayMain = 6,
		Canceled = 7,
		DeserializeCarsError = 8,
		DeserializePlayerRecordsError = 9,
		CheckInternetConnection = 10
	}
}
