namespace CPM.Account
{
	public enum SignUpResult
	{
		Unknown = 0,
		ErrorUserExists = 1,
		ErrorUnknown = 2,
		Success = 3,
		NoInternet = 4,
		DownloadLatestVersion = 5,
		ErrorCantSignInAnon = 6,
		ErrorCantSignUp = 7,
		ErrorCantSignIn = 8,
		ErrorAccountDontCreated = 9
	}
}
