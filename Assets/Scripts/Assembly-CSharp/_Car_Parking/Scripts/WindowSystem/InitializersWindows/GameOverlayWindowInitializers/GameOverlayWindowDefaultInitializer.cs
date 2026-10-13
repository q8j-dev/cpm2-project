using UnityEngine.Scripting;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem.InitializersWindows.GameOverlayWindowInitializers
{
	[Preserve]
	public class GameOverlayWindowDefaultInitializer : IWindowInitializer
	{
		public void Init(WindowScript windowScript)
		{
			if (windowScript is GameOverlayWindow gameOverlayWindow)
			{
				gameOverlayWindow.ActivateSettingsButton(enable: true);
			}
		}
	}
}
