using CPM.Database;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.SellExchangeFree;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Infrastructure
{
	[Preserve]
	public class CommonProvider
	{
		public ISlotService SlotService;

		public DataBase DataBase;

		public CharactersService CharactersService;

		public ISellExchangeFreeService SellExchangeFreeService;

		public ITradeService TradeService;

		public TradeServiceFactory TradeServiceFactory;

		public ChatExtraDataController ChatExtraDataController;

		public IPrefabFactory PrefabFactory;
	}
}
