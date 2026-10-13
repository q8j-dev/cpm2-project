using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.PlayerProperty.Processor
{
	public class GarageBuyProcessor : IPropertyTriggerProcessor
	{
		private readonly int _garageId;

		public GarageBuyProcessor(int garageId)
		{
			_garageId = garageId;
		}

		public void Process()
		{
			V.CoreGameManager.ManagerUI.ShowConfirmGarageBuyWindow(new ConfirmGarageBuyWindow.AdditionalData
			{
				GarageIndex = _garageId
			});
		}
	}
}
