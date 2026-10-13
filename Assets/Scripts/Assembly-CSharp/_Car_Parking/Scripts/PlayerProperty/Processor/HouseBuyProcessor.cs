namespace _Car_Parking.Scripts.PlayerProperty.Processor
{
	public class HouseBuyProcessor : IPropertyTriggerProcessor
	{
		private readonly int _houseId;

		public HouseBuyProcessor(int houseId)
		{
			_houseId = houseId;
		}

		public void Process()
		{
			V.CoreGameManager.ManagerUI.ShowApartmentGarageBuyWindow(_houseId);
		}
	}
}
