using Car_Parking.Scripts.Racing;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Police;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	public class RpcPolicePenaltyRequest : RpcObjectBase
	{
		public override byte Code => 102;

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string myCarViewID, string policeUserId)
		{
			FreeDriveControl freeDriveControl = multiPlayerPosControl.FreeDriveProvider.FreeDriveControl;
			if (string.IsNullOrEmpty(myCarViewID) || string.IsNullOrEmpty(policeUserId) || !V.Car || !freeDriveControl)
			{
				return;
			}
			string text = "";
			if (V.Car.TryGetComponent<PhotonView>(out var component))
			{
				text = component.ViewID.ToString();
			}
			if (myCarViewID != text)
			{
				return;
			}
			PolicePenalizeForSpeeding policePenalizeForSpeeding = freeDriveControl.PolicePenalizeForSpeeding;
			CurrencyManagerController currencyManagerController = freeDriveControl.CurrencyManagerController;
			RpcPolicePenaltyAnswer.ResultType resultType = RpcPolicePenaltyAnswer.ResultType.None;
			resultType = ((multiPlayerPosControl.FreeDriveProvider.RacingCar2 != null || V.Car.TryGetComponent<FreeRaceCar>(out var _)) ? RpcPolicePenaltyAnswer.ResultType.CarInRace : (policePenalizeForSpeeding.JustPenalized ? RpcPolicePenaltyAnswer.ResultType.JustPenalized : ((!currencyManagerController.IsEnoughMoney(policePenalizeForSpeeding.PoliceConfig.PenaltySystem.MoneyPenalty)) ? RpcPolicePenaltyAnswer.ResultType.NoMoney : RpcPolicePenaltyAnswer.ResultType.Success)));
			GameObject gameObject = multiPlayerPosControl.OnlineCollectionsProvider.GetPlayerByPlayerID(policeUserId).gameObject;
			if ((bool)gameObject)
			{
				PhotonView component3 = gameObject.GetComponent<PhotonView>();
				if (resultType == RpcPolicePenaltyAnswer.ResultType.Success)
				{
					string format = new LocalizedString("BuyPolice.PoliceFinedYou");
					V.CoreGameManager.SetMessage(string.Format(format, component3.Owner.NickName, freeDriveControl.PolicePenalizeForSpeeding.PoliceConfig.PenaltySystem.MoneyPenalty), 4f);
					AdvancedInt price = new AdvancedInt(policePenalizeForSpeeding.PoliceConfig.PenaltySystem.MoneyPenalty);
					currencyManagerController.TryMoneySpend(out var _, price);
					policePenalizeForSpeeding.ResetPenaltyTime();
					V.Car.GetComponent<VehicleData>().StopCarCompletely(policePenalizeForSpeeding.PoliceConfig.PenaltySystem.StopCarCompletelyTime);
				}
				MultiPlayer multiPlayer = multiPlayerPosControl.FreeDriveProvider.MultiPlayer;
				string nickName = component.Owner.NickName;
				int num = (int)resultType;
				multiPlayer.SendValueStrToPlayer<RpcPolicePenaltyAnswer>(nickName, num.ToString(), policeUserId);
			}
		}
	}
}
