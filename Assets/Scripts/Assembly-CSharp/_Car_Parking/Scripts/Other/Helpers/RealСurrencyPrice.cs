namespace _Car_Parking.Scripts.Other.Helpers
{
	public class RealСurrencyPrice
	{
		public string ProductId;

		public float Price;

		public string PriceText;

		public string IsoCurrencyCode;

		public float OldPrice;

		public string OldPriceText;

		public int Discount;

		public int OfferID = -99;

		public RealСurrencyPrice(string productId, float price, string isoCurrencyCode, float oldPrice = 0f, int discount = 0)
		{
			ProductId = productId;
			Price = price;
			IsoCurrencyCode = isoCurrencyCode;
			OldPrice = oldPrice;
			Discount = discount;
		}
	}
}
