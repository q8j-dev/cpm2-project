namespace _Car_Parking.Scripts.EventSystem
{
	public interface IEventUIWrapper
	{
		void ShowHud(bool state);

		void ShowStartMessage();

		void ShowMessage(bool pass, string message);

		void ShowResult(bool pass, int current, int target);

		void ShowEventProgressMessage(string message);
	}
}
