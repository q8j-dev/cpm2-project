using System;
using Cysharp.Threading.Tasks;
using Steamworks;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Car_Parking.Scripts.Steam
{
	public class SteamManager : MonoBehaviour
	{
		private const int APP_ID = 4252340;

		private UnityEngine.EventSystems.EventSystem _chachedEventSystem;

		private static SteamManager _instance;

		private const float InitRetrySeconds = 0.5f;

		private const int InitAttempts = 6;

		public static string InitError { get; private set; }

		private void Awake()
		{
			if ((bool)_instance && _instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			_instance = this;
			UnityEngine.Object.DontDestroyOnLoad(this);
			if (!TryInitSteam())
			{
				RetryInitAsync().Forget();
			}
			SteamFriends.OnGameOverlayActivated += OnSteamOverlayActivated;
			Debug.developerConsoleVisible = false;
		}

		private async UniTaskVoid RetryInitAsync()
		{
			for (int attempt = 2; attempt <= 6; attempt++)
			{
				await UniTask.Delay(TimeSpan.FromSeconds(0.5), ignoreTimeScale: false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy()).SuppressCancellationThrow();
				if (!this || TryInitSteam())
				{
					break;
				}
			}
		}

		public static async UniTask<bool> WaitForInitAsync(float timeoutSeconds)
		{
			float deadline = Time.realtimeSinceStartup + timeoutSeconds;
			while (!IsInitialized() && Time.realtimeSinceStartup < deadline)
			{
				await UniTask.Yield();
			}
			return IsInitialized();
		}

		private static bool TryInitSteam()
		{
			try
			{
				if (!SteamClient.IsValid)
				{
					SteamClient.Init(4252340u);
				}
				SteamInventory.LoadItemDefinitions();
				InitError = null;
				return true;
			}
			catch (Exception ex)
			{
				InitError = ex.GetType().Name + ": " + ex.Message;
				return false;
			}
		}

		private void OnDestroy()
		{
			SteamFriends.OnGameOverlayActivated -= OnSteamOverlayActivated;
		}

		private void OnSteamOverlayActivated(bool isActive)
		{
			if (isActive)
			{
				_chachedEventSystem = UnityEngine.EventSystems.EventSystem.current;
				if (!_chachedEventSystem)
				{
					_chachedEventSystem = UnityEngine.Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
				}
				if (_chachedEventSystem != null)
				{
					_chachedEventSystem.enabled = false;
				}
			}
			else if ((bool)_chachedEventSystem)
			{
				_chachedEventSystem.enabled = true;
				_chachedEventSystem = null;
			}
			else
			{
				UnityEngine.EventSystems.EventSystem eventSystem = UnityEngine.Object.FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>();
				if ((bool)eventSystem)
				{
					eventSystem.enabled = true;
				}
			}
		}

		public static void ReleaseOverlayInputBlock()
		{
			if ((bool)_instance)
			{
				_instance.OnSteamOverlayActivated(isActive: false);
			}
		}

		private void Update()
		{
			SteamClient.RunCallbacks();
		}

		private void OnApplicationQuit()
		{
			Debug.Log("[Quit] SteamManager.OnApplicationQuit begin");
			try
			{
				SteamClient.Shutdown();
				Debug.Log("[Quit] SteamClient.Shutdown returned");
			}
			catch (Exception ex)
			{
				Debug.LogError("[Quit] SteamClient.Shutdown threw " + ex.GetType().Name + ": " + ex.Message);
			}
		}

		public static bool IsInitialized()
		{
			_ = SteamClient.IsValid;
			return SteamClient.IsValid;
		}

		public static bool IsSteamRunningOnSteamDeck()
		{
			if (IsInitialized())
			{
				return SteamUtils.IsRunningOnSteamDeck;
			}
			return false;
		}

		public static bool IsSteamOverlayEnabled()
		{
			if (IsInitialized())
			{
				return SteamUtils.IsOverlayEnabled;
			}
			return false;
		}

		public static SteamId GetUserID()
		{
			if (!IsInitialized())
			{
				return default;
			}
			return SteamClient.SteamId;
		}

		public static string GetUserIdString()
		{
			if (!IsInitialized())
			{
				return "0";
			}
			return SteamClient.SteamId.Value.ToString();
		}

		public static uint GetUserAccountId()
		{
			if (!IsInitialized())
			{
				return 0u;
			}
			return SteamClient.SteamId.AccountId;
		}

		public static int GetSteamLevel()
		{
			if (!IsInitialized())
			{
				return 0;
			}
			return SteamUser.SteamLevel;
		}

		public static string GetSteamLanguage()
		{
			if (!IsInitialized())
			{
				return "en";
			}
			return SteamUtils.SteamUILanguage;
		}
	}
}
