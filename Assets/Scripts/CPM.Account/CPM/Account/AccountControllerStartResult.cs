namespace CPM.Account
{
	public enum AccountControllerStartResult
	{
		SignInNewSystem = 0,
		SignUpNewSystemNewAccount = 1,
		NoAcccount = 2,
		Other = 3,
		FailAndRestart = 4,
		CheckHashError = 5,
		FailSignUpToGuestAccount = 6,
		FailSignInToGuestAccount = 7,
		MigrateAnon = 8,
		ClearGuestAndOldAccount = 9
	}
}
