using GMESDK;
using UnityEngine;

public class GMEPause : MonoBehaviour
{
	private void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			MonoBehaviour.print("GME pause");
			GMEWWisePluginNative.GMEWWisePlugin_Pause();
		}
		else
		{
			MonoBehaviour.print("GME resume");
			GMEWWisePluginNative.GMEWWisePlugin_Resume();
		}
	}
}
