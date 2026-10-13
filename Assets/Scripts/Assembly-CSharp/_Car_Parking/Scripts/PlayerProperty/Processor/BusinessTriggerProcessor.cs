using _Car_Parking.Scripts.PlayerProperty.Business;

namespace _Car_Parking.Scripts.PlayerProperty.Processor
{
	public class BusinessTriggerProcessor : IPropertyTriggerProcessor
	{
		private readonly BusinessModel _businessModel;

		public BusinessTriggerProcessor(BusinessModel businessModel)
		{
			_businessModel = businessModel;
		}

		public void Process()
		{
			V.CoreGameManager.ManagerUI.ShowBusinessWindow(_businessModel);
		}
	}
}
