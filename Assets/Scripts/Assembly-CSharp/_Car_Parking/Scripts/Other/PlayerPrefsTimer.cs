using System;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Other
{
	public class PlayerPrefsTimer : IInitializable
	{
		private readonly IRemoteConfigManager _remoteConfigManager;

		private ConfigValuesStorage _remoteConfigData;

		public const int LastResetPasswordTime = 15;

		[Inject]
		public PlayerPrefsTimer(IRemoteConfigManager remoteConfigManager)
		{
			_remoteConfigManager = remoteConfigManager;
		}

		public void Initialize()
		{
			_remoteConfigData = _remoteConfigManager.GetDefaultConfigStorage();
			InitializeAsync().Forget();
		}

		private async UniTaskVoid InitializeAsync()
		{
			_remoteConfigData = await _remoteConfigManager.GetConfigValuesStorage();
		}

		public bool IsReadyToReset(string key)
		{
			if (key == null)
			{
				Debug.LogError(new NullReferenceException("PlayerPrefsTimer key is NULL"));
				return true;
			}
			DateTime utcNow = DateTime.UtcNow;
			if (PlayerPrefs.HasKey(key))
			{
				long ticks = Convert.ToInt64(PlayerPrefs.GetString(key));
				DateTime dateTime = new DateTime(ticks);
				if ((utcNow - dateTime).TotalSeconds < (double)(GetResetTimeFromRemoteConfig(key) * 60f))
				{
					return false;
				}
			}
			PlayerPrefs.SetString(key, utcNow.Ticks.ToString());
			return true;
		}

		private float GetResetTimeFromRemoteConfig(string key)
		{
			if (key == "LastTimeResetPressed")
			{
				return 15f;
			}
			return 0f;
		}
	}
}
