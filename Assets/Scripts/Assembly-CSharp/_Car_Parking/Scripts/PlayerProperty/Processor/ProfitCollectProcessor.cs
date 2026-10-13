using _Car_Parking.Scripts.PlayerProperty.Business;

namespace _Car_Parking.Scripts.PlayerProperty.Processor
{
	public class ProfitCollectProcessor : IPropertyTriggerProcessor
	{
		private readonly BusinessModel _businessModel;

		public ProfitCollectProcessor(BusinessModel businessModel)
		{
			_businessModel = businessModel;
		}

		public void Process()
		{
			V.CoreGameManager.ManagerUI.ShowBusinessProfitWindow(_businessModel);
		}
	}
}
