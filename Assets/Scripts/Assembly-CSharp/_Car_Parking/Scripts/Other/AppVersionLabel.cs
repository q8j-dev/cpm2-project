using CPM.Account;
using CPM.Connection;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace _Car_Parking.Scripts.Other
{
	public class AppVersionLabel : MonoBehaviour
	{
		private const int MaxLengthGraphicsDeviceVersionToShow = 25;

		private IConnectionManager _connectionManager;

		public Text text;

		public GameObject Canvas;

		private ManagerUI _managerUI;

		private IAccountController _accountController;

		public static bool IsInitialized { get; private set; }

		public static string LabelText { get; private set; }

		public static string Version { get; private set; }

		public static string DeviceModel { get; private set; }

		[Inject]
		public void Construct(IConnectionManager connectionManager, ManagerUI managerUI, IAccountController accountController)
		{
			_managerUI = managerUI;
			_connectionManager = connectionManager;
			_accountController = accountController;
		}

		public void Start()
		{
			_managerUI.AppVersion = this;
			_connectionManager.OnHashStatusChange += ConnectionManagerOnOnHashStatusChange;
			SetText();
		}

		private void SetText()
		{
			string text = "Version :";
			if ((int)_connectionManager.H == 2)
			{
				text = "Version:";
			}
			else if ((int)_connectionManager.H == 0)
			{
				text = "version:";
			}
			else if ((int)_connectionManager.H == 3)
			{
				text = "version_:";
			}
			LabelText = text + " " + Application.version;
			Version = text + " " + Application.version;
			string deviceModel = SystemInfo.deviceModel;
			LabelText = LabelText + " " + (int)_accountController.AccountType;
			TextAsset textAsset = Resources.Load<TextAsset>("BuildInfo");
			if (textAsset != null && !string.IsNullOrEmpty(textAsset.text))
			{
				string[] array = textAsset.text.Split('|');
				string text2 = ((array.Length != 0) ? array[0] : "?");
				string text3 = ((array.Length > 2) ? array[2] : "");
				string text4 = ((array.Length > 3) ? array[3] : "");
				if (text4.Length > 40)
				{
					text4 = text4.Substring(0, 40) + "...";
				}
				LabelText = LabelText + "  build " + text2 + "  " + text3 + "  " + text4;
			}
			DeviceModel = deviceModel;
			if (this.text != null)
			{
				this.text.text = LabelText;
			}
			IsInitialized = true;
		}

		private string GetVersionAndroid()
		{
			string[] array = SystemInfo.operatingSystem.Split(' ');
			if (array.Length >= 3)
			{
				return array[2];
			}
			return string.Empty;
		}

		private void OnDestroy()
		{
			if ((bool)_managerUI)
			{
				_managerUI.AppVersion = null;
			}
			_connectionManager.OnHashStatusChange -= ConnectionManagerOnOnHashStatusChange;
		}

		private void ConnectionManagerOnOnHashStatusChange()
		{
			SetText();
		}

		public void EnableText(bool enable)
		{
			text.enabled = enable;
		}
	}
}
