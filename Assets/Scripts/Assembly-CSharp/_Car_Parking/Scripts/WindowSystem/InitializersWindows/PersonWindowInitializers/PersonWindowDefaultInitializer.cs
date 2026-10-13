using UnityEngine.Scripting;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.WindowSystem.InitializersWindows.PersonWindowInitializers
{
	[Preserve]
	public class PersonWindowDefaultInitializer : IWindowInitializer
	{
		public void Init(WindowScript windowScript)
		{
			if (windowScript is PersonWindow personWindow)
			{
				personWindow.OnPersonWindowOpen();
				personWindow.InitBackButton();
			}
		}
	}
}
