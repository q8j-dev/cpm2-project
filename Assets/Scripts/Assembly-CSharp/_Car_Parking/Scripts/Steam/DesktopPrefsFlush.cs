using UnityEngine;

namespace _Car_Parking.Scripts.Steam
{
	public class DesktopPrefsFlush : MonoBehaviour
	{
		private static DesktopPrefsFlush _instance;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Bootstrap()
		{
			if (!(_instance != null))
			{
				GameObject obj = new GameObject("[DesktopPrefsFlush]");
				Object.DontDestroyOnLoad(obj);
				_instance = obj.AddComponent<DesktopPrefsFlush>();
				Application.quitting += PlayerPrefs.Save;
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			if (!hasFocus)
			{
				PlayerPrefs.Save();
			}
		}
	}
}
