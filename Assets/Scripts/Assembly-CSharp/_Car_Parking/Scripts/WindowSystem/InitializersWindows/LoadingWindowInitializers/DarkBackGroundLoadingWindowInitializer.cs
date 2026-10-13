using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem.InitializersWindows.LoadingWindowInitializers
{
	[Preserve]
	public class DarkBackGroundLoadingWindowInitializer : IWindowInitializer
	{
		public void Init(WindowScript windowScript)
		{
			if (windowScript is WaitingWindow waitingWindow)
			{
				Color color = waitingWindow.Background.color;
				color.a = 1f;
				waitingWindow.Background.color = color;
			}
		}
	}
}
