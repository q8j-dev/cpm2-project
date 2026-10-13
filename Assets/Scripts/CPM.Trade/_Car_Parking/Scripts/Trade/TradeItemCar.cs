namespace _Car_Parking.Scripts.Trade
{
	public class TradeItemCar : TradeItem
	{
		public int CreatorActorNr { get; private set; }

		public int ShortTuningData { get; private set; }

		public string CarInstanceId { get; private set; }

		public int CarID { get; private set; }

		public override TradeItemType ItemType => TradeItemType.Car;

		public TradeItemCar(int creatorActorNr, int shortTuningData, int carID, string carInstanceId)
		{
			CreatorActorNr = creatorActorNr;
			ShortTuningData = shortTuningData;
			CarInstanceId = carInstanceId;
			CarID = carID;
		}
	}
}
