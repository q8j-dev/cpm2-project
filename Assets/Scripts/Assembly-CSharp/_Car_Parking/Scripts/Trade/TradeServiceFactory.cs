using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Trade
{
	public class TradeServiceFactory
	{
		public const float P2POfferTimeLife = 30f;

		private readonly FreeDriveProvider _freeDriveProvider;

		public TradeServiceFactory(FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
		}

		public async UniTask<TradeItemCar> CreateItemCarAsync(int carActorNr, int tuningData, CancellationToken ct = default(CancellationToken))
		{
			string carInstanceId = "";
			int carID = -1;
			if (PhotonNetwork.LocalPlayer.ActorNumber == carActorNr && (bool)_freeDriveProvider.FirstCarMultiPlayer)
			{
				VehicleData component = _freeDriveProvider.FirstCarMultiPlayer.GetComponent<VehicleData>();
				carInstanceId = component.carInfo.CarInstanceId;
				carID = component.carInfo.CarID;
			}
			else if (PhotonNetwork.LocalPlayer.ActorNumber != carActorNr && (bool)_freeDriveProvider.MultiPlayer)
			{
				GameObject otherCar = _freeDriveProvider.MultiPlayer.GetCarByOwnerID(carActorNr);
				if ((bool)otherCar)
				{
					await otherCar.GetComponent<MultiPlayerPosControl>().ApplyDataIfNeedAsync(ct);
					if ((bool)otherCar)
					{
						VehicleData component2 = otherCar.GetComponent<VehicleData>();
						if ((bool)component2 && component2.CarInfoProvider?.CarInfo != null)
						{
							carInstanceId = component2.CarInfoProvider.CarInfo.CarInstanceId;
							carID = component2.CarId;
						}
					}
				}
			}
			return new TradeItemCar(carActorNr, tuningData, carID, carInstanceId);
		}

		public TradeOffer CreateOfferOnTrigger(TradeItemCar tradeItemCar, TradeItemMoney tradeItemMoney, int offerActorNr, string tradeOfferID = "")
		{
			if (string.IsNullOrEmpty(tradeOfferID))
			{
				tradeOfferID = GetNewRandomTradeOfferID();
			}
			return new TradeOffer(tradeOfferID, tradeItemCar, tradeItemMoney, TradeOfferVisibleType.Public, offerActorNr, -1, -1f);
		}

		public TradeOffer CreateOfferP2PSell(TradeItemCar tradeItemCar, TradeItemMoney tradeItemMoney, int offerActorNr, int requestActorNr, string tradeOfferID = "")
		{
			if (string.IsNullOrEmpty(tradeOfferID))
			{
				tradeOfferID = GetNewRandomTradeOfferID();
			}
			return new TradeOffer(tradeOfferID, tradeItemCar, tradeItemMoney, TradeOfferVisibleType.Private, offerActorNr, requestActorNr, 30f);
		}

		public TradeOffer CreateOfferP2PExchange(TradeItemCar tradeItemCar1, TradeItemCar tradeItemCar2, int offerActorNr, int requestActorNr, string tradeOfferID = "")
		{
			if (string.IsNullOrEmpty(tradeOfferID))
			{
				tradeOfferID = GetNewRandomTradeOfferID();
			}
			return new TradeOffer(tradeOfferID, tradeItemCar1, tradeItemCar2, TradeOfferVisibleType.Private, offerActorNr, requestActorNr, 30f);
		}

		public string GetNewRandomTradeOfferID()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
