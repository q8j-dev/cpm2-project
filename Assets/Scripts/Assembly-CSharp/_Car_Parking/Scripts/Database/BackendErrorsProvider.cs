using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Database
{
	public static class BackendErrorsProvider
	{
		public static Dictionary<ResponseCode, string> ErrorMessages = new Dictionary<ResponseCode, string>
		{
			{
				ResponseCode.Unknown,
				"Errors.GarageForSaleNotFound"
			},
			{
				ResponseCode.UnknownClient,
				"DataBase.SomethingWentWrongCheck"
			},
			{
				ResponseCode.GarageForSaleNotFound,
				"Errors.GarageForSaleNotFound"
			},
			{
				ResponseCode.TransferImpossible,
				"Errors.ClearYourGarage"
			}
		};

		public static bool ValidateResponse(ResponseCode status)
		{
			switch (status)
			{
			case ResponseCode.Unknown:
			case ResponseCode.GarageForSaleNotFound:
			case ResponseCode.UnknownClient:
				V.CoreGameManager.ManagerUI.ShowLocalizedErrorNotification(ErrorMessages[status]).Forget();
				return false;
			case ResponseCode.TransferImpossible:
				V.CoreGameManager.ManagerUI.ShowLocalizedErrorNotification(ErrorMessages[status]).Forget();
				return false;
			default:
				return true;
			}
		}
	}
}
