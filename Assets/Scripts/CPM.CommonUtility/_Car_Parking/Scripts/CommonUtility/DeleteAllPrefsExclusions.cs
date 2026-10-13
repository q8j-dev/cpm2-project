using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using _Car_Parking.Scripts.PrefsKeys;

namespace _Car_Parking.Scripts.CommonUtility
{
	public class DeleteAllPrefsExclusions
	{
		public const string PlayerReviewShowTimePrefs = "PlayerReviewShowTime";

		public const string MarketOpenedPrefs = "MarketOpened";

		public const string NewAccountEnableStatusPlayerPrefsKey = "NewAccountEnableStatusKeyV7";

		private const string PendingFullBootstrapKey = "PendingFullBootstrap";

		private const string LastAppVersionPrefs = "LastAppVer";

		private const string AddressablesRuntimeDataPathKey = "AddressablesRuntimeDataPath";

		private const string PrivacyOnlineWarningShownPrefsKey = "PrivacyOnlineWarningShown";

		private const string PrivacyAgeShownKey = "PrivacyAgeShown";

		private const string PrivacyAgeConfirmedKey = "privacyAgeConfirmed";

		private string _lastResetPasswordTime;

		private string _lastAppVersion;

		private string _addressablesRuntimeDataPath;

		private int _onlineWaringShown;

		private int _privacyAgeShown;

		private int _privacyAgeConfirmed;

		private string _randomDeviceID = string.Empty;

		private int _newAccountEnableStatusControllerEnabled;

		private int _marketOpened;

		private int _playerReviewShowTime;

		private bool _pendingFullBootstrap;

		private bool _kickedByAnotherDevice;

		public void Save(List<IAllPrefsSaver> allPrefsSavers)
		{
			try
			{
				_lastAppVersion = PlayerPrefs.GetString("LastAppVer");
				_lastResetPasswordTime = PlayerPrefs.GetString("LastTimeResetPressed");
				_addressablesRuntimeDataPath = PlayerPrefs.GetString("AddressablesRuntimeDataPath", "");
				if (allPrefsSavers != null)
				{
					for (int i = 0; i < allPrefsSavers.Count; i++)
					{
						allPrefsSavers[i].Save();
					}
				}
				_onlineWaringShown = PlayerPrefs.GetInt("PrivacyOnlineWarningShown");
				_privacyAgeShown = PlayerPrefs.GetInt("PrivacyAgeShown");
				_privacyAgeConfirmed = PlayerPrefs.GetInt("privacyAgeConfirmed");
				_randomDeviceID = PlayerPrefs.GetString("RandomDeviceId");
				_newAccountEnableStatusControllerEnabled = PlayerPrefs.GetInt("NewAccountEnableStatusKeyV7");
				_marketOpened = PlayerPrefs.GetInt("MarketOpened");
				_playerReviewShowTime = PlayerPrefs.GetInt("PlayerReviewShowTime");
				_pendingFullBootstrap = ObscuredPrefs.GetInt("PendingFullBootstrap") == 1;
				_kickedByAnotherDevice = ManagerKeysForPrefs.KickedByAnotherDevice;
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
		}

		public void Load(List<IAllPrefsSaver> allPrefsSavers)
		{
			try
			{
				PlayerPrefs.SetString("LastAppVer", _lastAppVersion);
				if (_lastResetPasswordTime != "")
				{
					PlayerPrefs.SetString("LastTimeResetPressed", _lastResetPasswordTime);
					_lastResetPasswordTime = "";
				}
				if (_addressablesRuntimeDataPath != "")
				{
					PlayerPrefs.SetString("AddressablesRuntimeDataPath", _addressablesRuntimeDataPath);
					_addressablesRuntimeDataPath = "";
				}
				if (allPrefsSavers != null)
				{
					for (int i = 0; i < allPrefsSavers.Count; i++)
					{
						allPrefsSavers[i].Load();
					}
				}
				PlayerPrefs.SetInt("PrivacyOnlineWarningShown", _onlineWaringShown);
				PlayerPrefs.SetInt("PrivacyAgeShown", _privacyAgeShown);
				PlayerPrefs.SetInt("privacyAgeConfirmed", _privacyAgeConfirmed);
				if (!string.IsNullOrEmpty(_randomDeviceID))
				{
					PlayerPrefs.SetString("RandomDeviceId", _randomDeviceID);
				}
				PlayerPrefs.SetInt("NewAccountEnableStatusKeyV7", _newAccountEnableStatusControllerEnabled);
				PlayerPrefs.SetInt("MarketOpened", _marketOpened);
				PlayerPrefs.SetInt("PlayerReviewShowTime", _playerReviewShowTime);
				ObscuredPrefs.SetInt("PendingFullBootstrap", _pendingFullBootstrap ? 1 : 0);
				ManagerKeysForPrefs.KickedByAnotherDevice = _kickedByAnotherDevice;
			}
			catch (Exception message)
			{
				Debug.LogWarning(message);
			}
		}
	}
}
