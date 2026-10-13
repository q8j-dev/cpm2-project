using System.IO;
using CPM.Account;
using CPM.PlayerData;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

public class RootChecker : MonoBehaviour
{
	private PlayerDataStorage _playerDataStorage;

	private IAccountController _accountController;

	private IRemoteConfigManager _remoteConfigManager;

	private ConfigValuesStorage _configStorage;

	[Inject]
	public void Construct(PlayerDataStorage playerDataStorage, IAccountController accountController, IRemoteConfigManager remoteConfigManager)
	{
		_playerDataStorage = playerDataStorage;
		_accountController = accountController;
		_remoteConfigManager = remoteConfigManager;
	}

	private void Start()
	{
	}

	private async UniTaskVoid Init()
	{
		bool IsRooted = PlayerPrefs.GetInt("IsRooted") == 1;
		_configStorage = _remoteConfigManager.GetDefaultConfigStorage();
		_configStorage = await _remoteConfigManager.GetConfigValuesStorage();
		if (!IsRooted || _configStorage.needHoldRootDevices)
		{
			SendData();
		}
	}

	private void SendData()
	{
		if (_playerDataStorage != null && _accountController.HasEmail)
		{
			int num = (isRooted() ? 1 : 0);
			int num2 = (IsEmulator() ? 1 : 0);
			_ = num + "|" + num2;
			PlayerPrefs.SetInt("IsRooted", 1);
		}
	}

	public void GetData()
	{
		string text = "";
		if (Application.platform == RuntimePlatform.Android)
		{
			text = text + "Rooted : " + isRooted();
			text += " /";
			text = text + "Emulator : " + IsEmulator();
		}
	}

	public bool isRooted()
	{
		bool result = false;
		if (Application.platform == RuntimePlatform.Android)
		{
			if (isRootedPrivate("/system/bin/su"))
			{
				result = true;
			}
			if (isRootedPrivate("/system/xbin/su"))
			{
				result = true;
			}
			if (isRootedPrivate("/system/app/SuperUser.apk"))
			{
				result = true;
			}
			if (isRootedPrivate("/data/data/com.noshufou.android.su"))
			{
				result = true;
			}
			if (isRootedPrivate("/sbin/su"))
			{
				result = true;
			}
		}
		return result;
	}

	private bool IsEmulator()
	{
		if (Application.platform == RuntimePlatform.Android)
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build");
			string text = androidJavaClass.GetStatic<string>("FINGERPRINT");
			string text2 = androidJavaClass.GetStatic<string>("MODEL");
			string text3 = androidJavaClass.GetStatic<string>("MANUFACTURER");
			string text4 = androidJavaClass.GetStatic<string>("BRAND");
			string text5 = androidJavaClass.GetStatic<string>("DEVICE");
			string text6 = androidJavaClass.GetStatic<string>("PRODUCT");
			if (!text.Contains("generic") && !text.Contains("unknown") && !text2.Contains("google_sdk") && !text2.Contains("Emulator") && !text2.Contains("Android SDK built for x86") && !text3.Contains("Genymotion") && (!text4.Contains("generic") || !text5.Contains("generic")) && !text6.Equals("google_sdk"))
			{
				return text6.Equals("unknown");
			}
			return true;
		}
		return false;
	}

	private static bool isRootedPrivate(string path)
	{
		bool result = false;
		if (File.Exists(path))
		{
			result = true;
		}
		return result;
	}
}
