using System;
using System.Threading;
using CPM.Account;
using CPM.RemoteConfig.Scriptables;
using Cysharp.Threading.Tasks;
using Firebase.Impl;
using Firebase.RemoteConfig;
using Newtonsoft.Json;
using UniRx;
using UnityEngine;
using VContainer;

namespace CPM.RemoteConfig
{
	public class ControllerFirebaseRemoteConfig : IAuthorizationDependant, IRemoteConfigManager, IAuthorizationFailureDependant, IDisposable
	{
		private const float ConfigUpdateHourStep = 0.5f;

		private readonly DefaultRemoteConfig defaultRemoteConfig;

		private ConfigValuesStorage configValuesStorage;

		private readonly CancellationTokenSource updateCancellationTokenSource;

		private IDisposable _updateFetchSubscription;

		[Inject]
		public ControllerFirebaseRemoteConfig(DefaultRemoteConfig defaultRemoteConfig)
		{
			this.defaultRemoteConfig = defaultRemoteConfig;
			updateCancellationTokenSource = new CancellationTokenSource();
			Functions.SetRegion(defaultRemoteConfig.configValuesStorage.funcRegion);
			RemoteConfigSnapshot.Apply(defaultRemoteConfig.configValuesStorage);
		}

		private async UniTask FetchingConfig()
		{
			try
			{
				await FirebaseRemoteConfig.DefaultInstance.FetchAsync();
				string raw = FirebaseRemoteConfig.DefaultInstance.GetRaw();
				configValuesStorage = ((!string.IsNullOrEmpty(raw)) ? JsonConvert.DeserializeObject<ConfigValuesStorage>(raw) : null);
			}
			catch (Exception arg)
			{
				Debug.LogError($"[RemoteConfig] FetchingConfig failed: {arg}");
			}
			finally
			{
				if (configValuesStorage == null)
				{
					configValuesStorage = defaultRemoteConfig.configValuesStorage;
				}
			}
			Functions.SetRegion(configValuesStorage.funcRegion);
			RemoteConfigSnapshot.Apply(configValuesStorage);
		}

		~ControllerFirebaseRemoteConfig()
		{
			updateCancellationTokenSource.Cancel();
		}

		public UniTaskVoid OnAuthorizationFailure()
		{
			Start().Forget();
			return default;
		}

		public async UniTask<ConfigValuesStorage> GetConfigValuesStorage()
		{
			if (configValuesStorage != null)
			{
				return configValuesStorage;
			}
			if (await UniTask.WaitUntil(() => configValuesStorage != null).TimeoutWithoutException(TimeSpan.FromMinutes(1.0)))
			{
				return defaultRemoteConfig.configValuesStorage;
			}
			return configValuesStorage;
		}

		public ConfigValuesStorage GetDefaultConfigStorage()
		{
			return defaultRemoteConfig.configValuesStorage;
		}

		private async UniTaskVoid Start()
		{
			await FetchingConfig();
			UpdateFetchConfig();
		}

		private void UpdateFetchConfig()
		{
			_updateFetchSubscription?.Dispose();
			_updateFetchSubscription = Observable.Interval(TimeSpan.FromHours(0.5)).Subscribe((long _) =>
			{
				FetchingConfig().Forget();
			}).AddTo(updateCancellationTokenSource.Token);
		}

		public async UniTaskVoid OnSuccessfulAuthorization()
		{
			Start().Forget();
		}

		public void OnSignOut()
		{
			Start().Forget();
		}

		public void Dispose()
		{
			_updateFetchSubscription?.Dispose();
		}
	}
}
