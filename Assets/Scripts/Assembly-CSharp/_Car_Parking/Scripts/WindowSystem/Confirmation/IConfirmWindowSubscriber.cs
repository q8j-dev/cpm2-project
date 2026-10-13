namespace _Car_Parking.Scripts.WindowSystem.Confirmation
{
	public interface IConfirmWindowSubscriber
	{
		void OnConfirmed();

		void OnCanceled();
	}
}
