namespace _Car_Parking.Scripts.PlayerProperty.Business
{
	public interface IBusinessProfitControl
	{
		int GetProfit(int businessId);

		void ClearProfit(int businessId);

		void Save();

		void OnMoneyCollect(int buildingId);
	}
}
