using System;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using GME_voice_chat;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Infrastructure
{
	public class ParentalControlUtility : IInitializable
	{
		private readonly IRemoteConfigManager _remoteConfigManager;

		private int _parentalControlStatus;

		public bool IsParentalControlEnabled => _parentalControlStatus == 0;

		public bool IsVinylsEnabled => _parentalControlStatus == 1;

		public bool IsSocialBlocked => _parentalControlStatus == 2;

		public event Action OnAgeConfirmChanged;

		[Inject]
		public ParentalControlUtility(IRemoteConfigManager remoteConfigManager)
		{
			_remoteConfigManager = remoteConfigManager;
		}

		public void Initialize()
		{
			Init().Forget();
		}

		private async UniTaskVoid Init()
		{
			_parentalControlStatus = 3;
		}

		public void InvokeAgeConfirmChanged()
		{
			try
			{
				OnAgeConfirmChanged?.Invoke();
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
		}

		private void OnStatusChanged()
		{
			int parentalControlStatus = _parentalControlStatus;
			if (parentalControlStatus == 2 || parentalControlStatus == 1)
			{
				if (_parentalControlStatus == 2)
				{
					PlayerPrefs.SetInt("hideAllVinyls", 1);
				}
				PlayerPrefs.SetInt("hideAllChat", 1);
				PlayerPrefs.SetInt("VoiceChat", 0);
				PlayerPrefs.SetInt("lowerVolumeOnVoiceChat", 0);
			}
			else
			{
				bool flag = PlayerPrefs.GetInt("privacyAgeConfirmed", 0) == 1;
				GMEVoiceChatManager.VoiceChatDefault = ((!IsParentalControlEnabled || flag) ? 1 : 0);
			}
		}
	}
}
