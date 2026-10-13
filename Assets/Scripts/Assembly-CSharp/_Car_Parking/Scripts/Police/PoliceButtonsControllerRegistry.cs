namespace _Car_Parking.Scripts.Police
{
	public class PoliceButtonsControllerRegistry
	{
		public GameplayPoliceButtonsController Controller { get; private set; }

		public void Register(GameplayPoliceButtonsController controller)
		{
			Controller = controller;
		}

		public void Unregister(GameplayPoliceButtonsController controller)
		{
			if (Controller == controller)
			{
				Controller = null;
			}
		}
	}
}
