using System;
using CPM.PlayerData;
using CPM.RemoteConfig;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CommonUtility.Contracts;
using _Car_Parking.Scripts.CustomDataTypes;

namespace CPM.Connection
{
	public class PhotonConnectionHandler
	{
		public static bool isConnectingToDevServer;

		private PlayerDataStorage playerDataStorage;

		private ObscuredInt multiplayerStatus;

		private IPhotonCarValidator photonCarValidator;

		private ConfigValuesStorage _config;

		private AdvancedBoolean _appData;

		public bool HasCachedConfig
		{
			get
			{
				if (_config != null)
				{
					return _appData != null;
				}
				return false;
			}
		}

		[Inject]
		public PhotonConnectionHandler(PlayerDataStorage playerDataStorage, IPhotonCarValidator photonCarValidator)
		{
			this.playerDataStorage = playerDataStorage;
			this.photonCarValidator = photonCarValidator;
		}

		public string GetTargetAppVersion(PhotonConnectionProfile profile)
		{
			_appData.Get(out var value);
			PhotonAppVersionInput input = new PhotonAppVersionInput
			{
				PhotonGameVersion = _config.photonGameVersion,
				Profile = profile,
				LanguageIndex = PlayerPrefs.GetInt("languageIndex", (int)Application.systemLanguage),
				CorrectHash = value,
				HasPlayerId = (playerDataStorage.PlayerID.ToString().Trim() != ""),
				Banned = (ObscuredPrefs.GetInt("Banned") == 1)
			};
			return PhotonAppVersion.Build(in input);
		}

		public async UniTaskVoid MakeConnection(ConfigValuesStorage configValuesStorage, AdvancedBoolean appData, PhotonConnectionProfile profile)
		{
			_config = configValuesStorage;
			_appData = appData;
			multiplayerStatus = configValuesStorage.multiPlayerStatus;
			appData.Get(out var value);
			bool flag = false;
			if ((int)multiplayerStatus != 2)
			{
				flag = (int)multiplayerStatus == 4;
			}
			else if (!value)
			{
				_ = Application.isEditor;
				flag = true;
			}
			if (!flag)
			{
				RunConnection(GetTargetAppVersion(profile)).Forget();
			}
		}

		private async UniTaskVoid RunConnection(string verConnect)
		{
			PhotonNetwork.AuthValues = new AuthenticationValues
			{
				UserId = (((string)playerDataStorage.PlayerID) ?? "")
			};
			PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = verConnect;
			await UniTask.WaitUntil(() => PhotonNetwork.NetworkClientState == ClientState.Disconnected || PhotonNetwork.NetworkClientState == ClientState.PeerCreated).TimeoutWithoutException(TimeSpan.FromMinutes(1.0));
			PhotonNetwork.ConnectUsingSettings();
		}

		public void Reconnect(PhotonConnectionProfile profile)
		{
			if (HasCachedConfig)
			{
				if (PhotonNetwork.IsConnected)
				{
					PhotonNetwork.Disconnect();
				}
				RunConnection(GetTargetAppVersion(profile)).Forget();
			}
		}
	}
}
