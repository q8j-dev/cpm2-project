using System;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Steam.Purchase
{
	public class SteamPurchaseRestoreService : ISteamPurchaseRestoreService
	{
		private const string RESTORE_STEAM_PURCHASES_FN = "RestoreSteamPurchases24_1";

		private const int RESTORE_PARTIAL_SUCCESS_NO_SLOT = 2;

		private readonly ISteamAccountService _steamAccountService;

		public bool HasUnrestoredPurchaseNoSlot { get; private set; }

		public void ClearUnrestoredPurchaseFlag()
		{
			HasUnrestoredPurchaseNoSlot = false;
		}

		public SteamPurchaseRestoreService(ISteamAccountService steamAccountService)
		{
			_steamAccountService = steamAccountService;
		}

		public async UniTask RestoreIfNeeded()
		{
			if (!SteamManager.IsInitialized())
			{
				return;
			}
			try
			{
				await CallBackendRestorePurchases();
			}
			catch (Exception)
			{
			}
		}

		private async UniTask CallBackendRestorePurchases()
		{
			string text = await _steamAccountService.GetAuthSessionTicketAsync();
			if (!string.IsNullOrEmpty(text))
			{
				FunctionCallResult<string> functionCallResult = await new FirebaseWrappedFunctionFactory().Create("RestoreSteamPurchases24_1").CallAsyncResult<string>(text);
				HasUnrestoredPurchaseNoSlot = functionCallResult.Status == FunctionStatus.AppPurchasesAlreadyBought;
				_ = functionCallResult.Status;
			}
		}

		private static string DescribeFailure(FunctionStatus status)
		{
			return status switch
			{
				FunctionStatus.SteamRestoreInventoryFetchFailed => "Steam inventory could not be read (57)", 
				FunctionStatus.SteamRestoreFailed => "restore failed after the inventory was read (58)", 
				FunctionStatus.SteamInvalidItem => "item def id is not in the Steam item maps (59)", 
				FunctionStatus.SteamInvalidTransaction => "transaction id missing or empty (299)", 
				FunctionStatus.SteamIDUidIncorrect => "uid does not match the caller (55)", 
				FunctionStatus.SteamIDNotLinked => "account has no Steam id on file (50)", 
				_ => status.ToString(), 
			};
		}
	}
}
