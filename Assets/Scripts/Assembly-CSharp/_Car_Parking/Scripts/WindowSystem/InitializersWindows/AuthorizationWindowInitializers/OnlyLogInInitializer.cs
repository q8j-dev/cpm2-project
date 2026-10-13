using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem.InitializersWindows.AuthorizationWindowInitializers
{
	[UnityEngine.Scripting.Preserve]
	public class OnlyLogInInitializer : IWindowInitializer
	{
		[Inject]
		private ManagerUI _managerUI;

		public void Init(WindowScript windowScript)
		{
			_ = windowScript is AuthorizationWindow;
		}
	}
}
