using System;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Cypher;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects
{
	[Preserve]
	public class RpcCarEnteredCarSellTrigger : RpcObjectBase
	{
		private readonly CommonProvider _commonProvider;

		public override byte Code => 42;

		public static void Send(MultiPlayer multiPlayer, GameObject firstCarMultiplayer, string carModel, int carPrice, int sellCarTriggerNumber, string tradeOfferID)
		{
			int viewID = firstCarMultiplayer.GetComponent<PhotonView>().ViewID;
			int creatorActorNr = V.DriverPerson.GetComponent<PhotonView>().CreatorActorNr;
			int num = firstCarMultiplayer.GetComponent<VehicleData>().carInfo.TuningData.CalcShortTuningStates();
			string message = $"{carModel}?{carPrice}?{sellCarTriggerNumber}?{creatorActorNr}?{num}?{tradeOfferID}";
			message = WalletCypher.Encode(message, "1");
			multiPlayer.SendValueStr<RpcCarEnteredCarSellTrigger>(viewID.ToString(), message, MultiPlayer.PhotonViewSource.FirstCarMultiPlayer);
		}

		public RpcCarEnteredCarSellTrigger(CommonProvider commonProvider)
		{
			_commonProvider = commonProvider;
		}

		public override void Execute(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			ExecuteAsync(multiPlayerPosControl, b, name).Forget();
		}

		private async UniTaskVoid ExecuteAsync(MultiPlayerPosControl multiPlayerPosControl, string b, string name)
		{
			try
			{
				PhotonView photonView = multiPlayerPosControl.photonView;
				int viewID = photonView.ViewID;
				if (int.Parse(b) != viewID || _commonProvider.SellExchangeFreeService == null || _commonProvider.TradeService == null || !V.CoreGameManager || V.CoreGameManager.FreeDriveProvider == null || !V.CoreGameManager.FreeDriveProvider.MultiPlayer)
				{
					return;
				}
				name = WalletCypher.DecodeWithOtherUID(name, photonView.CreatorActorNr);
				string[] array = name.Split("?"[0]);
				if (array == null || array.Length != 6)
				{
					return;
				}
				string modelCar = array[0];
				int price = int.Parse(array[1]);
				int place = int.Parse(array[2]);
				int carCreatorActorNr = int.Parse(array[3]);
				int tuningData = int.Parse(array[4]);
				string tradeOfferID = array[5];
				if (carCreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber && carCreatorActorNr == photonView.CreatorActorNr)
				{
					TradeItemCar tradeItemCar = await _commonProvider.TradeServiceFactory.CreateItemCarAsync(carCreatorActorNr, tuningData);
					if ((bool)multiPlayerPosControl && _commonProvider.TradeServiceFactory != null)
					{
						TradeOffer offer = _commonProvider.TradeServiceFactory.CreateOfferOnTrigger(tradeItemCar, new TradeItemMoney(new AdvancedInt(price)), carCreatorActorNr, tradeOfferID);
						_commonProvider.TradeService.AddOffer(offer);
						_commonProvider.SellExchangeFreeService.SellCarPlaced(modelCar, multiPlayerPosControl.gameObject, place, tradeOfferID);
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
