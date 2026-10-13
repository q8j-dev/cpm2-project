namespace _Car_Parking.Scripts.Wallet.Cars
{
	public class CarBuyResponse
	{
		public string CarInstanceID;

		public int? NewMoney;

		public int? NewCoins;

		public override string ToString()
		{
			string[] obj = new string[6] { "NewMoney: ", null, null, null, null, null };
			int? newMoney = NewMoney;
			obj[1] = newMoney.ToString();
			obj[2] = " NewCoins: ";
			newMoney = NewCoins;
			obj[3] = newMoney.ToString();
			obj[4] = " CarInstanceID: ";
			obj[5] = CarInstanceID;
			return string.Concat(obj);
		}
	}
}
