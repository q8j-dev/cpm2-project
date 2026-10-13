using UnityEngine.UI;
using VContainer;

namespace _Car_Parking.Scripts.Drag
{
	public class DashboardWindow : WindowScript
	{
		public static DashboardWindow instance;

		public Button settingsButton;

		[Inject]
		public void Construct(ManagerUI managerUI)
		{
			_managerUI = managerUI;
		}

		protected override void Awake()
		{
			if (instance == null)
			{
				instance = this;
			}
			if ((bool)settingsButton)
			{
				settingsButton.onClick.AddListener(_managerUI.ShowSettingsWindow);
			}
			base.Awake();
		}
	}
}
