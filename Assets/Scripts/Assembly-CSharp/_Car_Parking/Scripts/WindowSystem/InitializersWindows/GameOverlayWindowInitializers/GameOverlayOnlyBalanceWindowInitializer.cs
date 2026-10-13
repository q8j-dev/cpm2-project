using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.WindowSystem.InitializersWindows.GameOverlayWindowInitializers
{
	[UnityEngine.Scripting.Preserve]
	public class GameOverlayOnlyBalanceWindowInitializer : IWindowInitializer
	{
		[Inject]
		private ManagerUI _managerUI;

		[Inject]
		private IPrefabFactory _prefabFactory;

		public void Init(WindowScript windowScript)
		{
			if (windowScript is GameOverlayWindow gameOverlayWindow)
			{
				gameOverlayWindow.settingsBtn.gameObject.SetActive(value: false);
				gameOverlayWindow.achievmentsBtn.gameObject.SetActive(value: false);
				gameOverlayWindow.Background.SetActive(value: false);
				_managerUI.OnBackButtonActivated?.Invoke(obj: false);
			}
		}
	}
}
