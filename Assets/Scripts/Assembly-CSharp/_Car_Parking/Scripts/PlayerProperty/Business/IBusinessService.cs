namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	public interface IBusinessService
	{
		bool HasBought(int businessId);

		BusinessModel GetBusinessData(int businessId);

		bool BuyBusiness(int businessId);
	}
}
