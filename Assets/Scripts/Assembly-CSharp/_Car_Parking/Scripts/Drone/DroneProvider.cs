using VContainer;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Drone
{
	public class DroneProvider
	{
		private ChatControl _chatControl;

		public DroneControlWindow DroneWindow { get; private set; }

		[Inject]
		public void Construct(ChatControl chatControl)
		{
			_chatControl = chatControl;
		}

		public void SetDroneWindow(DroneControlWindow droneControlWindow)
		{
			DroneWindow = droneControlWindow;
		}

		public void ResetDroneWindow()
		{
			DroneWindow = null;
		}

		public void ShowDroneUI(bool state)
		{
			if ((bool)DroneWindow)
			{
				DroneWindow.ShowUI(state);
				if (state)
				{
					_chatControl.ShowPartiallyChat();
				}
				else
				{
					_chatControl.HideAllChat();
				}
			}
		}
	}
}
