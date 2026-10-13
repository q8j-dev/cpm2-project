using UnityEngine.Scripting;
using _Car_Parking.Scripts.SellExchangeFree;

namespace _Car_Parking.Scripts.Multiplayer.RequestResponseService
{
	[Preserve]
	public class RpcRrResponseSendBuyCarOffer : RpcRrResponseObjectBase
	{
		public enum SendBuyCarOfferStatusCode
		{
			Success = 0,
			SuccessAndBlocked = 1,
			IsBusy = 2,
			IsInTrade = 3
		}

		public SendBuyCarOfferStatusCode StatusCode;

		public static SellExchangeFreeMessage.ErrorCode StatusCodeToErrorCode(SendBuyCarOfferStatusCode sendBuyCarOfferStatusCode)
		{
			return sendBuyCarOfferStatusCode switch
			{
				SendBuyCarOfferStatusCode.Success => SellExchangeFreeMessage.ErrorCode.Error, 
				SendBuyCarOfferStatusCode.IsBusy => SellExchangeFreeMessage.ErrorCode.PlayerIsBusy, 
				SendBuyCarOfferStatusCode.IsInTrade => SellExchangeFreeMessage.ErrorCode.OtherPlayerIsInTrade, 
				_ => SellExchangeFreeMessage.ErrorCode.Error, 
			};
		}

		public RpcRrResponseSendBuyCarOffer()
			: base(CodeType.Fail)
		{
		}

		public RpcRrResponseSendBuyCarOffer(CodeType code, SendBuyCarOfferStatusCode statusCode)
			: base(code)
		{
			StatusCode = statusCode;
		}
	}
}
