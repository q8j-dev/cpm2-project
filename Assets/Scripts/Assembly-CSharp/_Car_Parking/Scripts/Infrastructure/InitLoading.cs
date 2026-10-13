using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.GraphicsSettings;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Infrastructure
{
	public class InitLoading : MonoBehaviour
	{
		private const string SceneName = "LoadingSecond";

		[SerializeField]
		private Image ProgressBar;

		[SerializeField]
		private TextMeshProUGUI ProgressText;

		public Camera Camera;

		public GameObject AllUi;

		private static InitLoading Instance;

		private SceneLoadingTimer _sceneLoadingTimer;

		private static int _mRemainingsToDownload = 0;

		private static float _progress;

		private static float _bytesDownloaded;

		private static float _bytesAll;

		private string _downloadingText = "Downloading...";

		private static Action<AndroidAssetPackInfo> DownloadAssetPackAsyncCallback = (AndroidAssetPackInfo info) =>
		{
			_progress = Mathf.Clamp01((float)info.bytesDownloaded / (float)info.size);
			_bytesDownloaded = info.bytesDownloaded / 1024;
			_bytesAll = info.size / 1024;
			if (info.status == AndroidAssetPackStatus.Completed)
			{
				_mRemainingsToDownload--;
				if (_mRemainingsToDownload == 0)
				{
					Instance?.OtherStartDelayed();
				}
			}
		};

		private int _cointOfLoadedPacks;

		public bool Started { get; set; }

		[Inject]
		public void Construct(SceneLoadingTimer sceneLoadingTimer)
		{
			_sceneLoadingTimer = sceneLoadingTimer;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		public static void PreWarm()
		{
			Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
			UnityEngine.Debug.unityLogger.filterLogType = LogType.Assert;
		}

		private void Awake()
		{
			Instance = this;
			_progress = 0f;
			_bytesDownloaded = 0f;
			_bytesAll = 0f;
			_sceneLoadingTimer?.StartLoadingTimer();
		}

		private void Start()
		{
			StartIEnumeratorAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}

		private async UniTask StartIEnumeratorAsync(CancellationToken ct = default(CancellationToken))
		{
			_ = 3;
			try
			{
				InitUiLogic();
				await new PerformanceDetector().DetectPerformanceTierAsync(ct);
				await UniTask.Yield(ct);
				await UniTask.Yield(ct);
				await UniTask.Yield(ct);
				Started = true;
				StartLogic();
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void StartLogic()
		{
			Log("StartLogic");
			CheckLoading();
			if (GetAvailableMemoryInGB(out var memory))
			{
				MonoBehaviour.print(memory + " ram size available");
			}
		}

		private void InitUiLogic()
		{
			ProgressBar.fillAmount = 0f;
			ProgressText.text = "";
			_downloadingText = ResolveDownloadingText();
			AllUi.gameObject.SetActive(value: true);
			Camera.gameObject.SetActive(value: true);
		}

		private static string ResolveDownloadingText()
		{
			try
			{
				return new LocalizedString("AssetBundleManager.Downloading");
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.LogWarning("Localization is not available yet: " + ex.Message);
				return "Downloading...";
			}
		}

		private void Update()
		{
			if (Started)
			{
				ProgressBar.fillAmount = _progress;
				if (_bytesDownloaded == 0f && _bytesAll == 0f)
				{
					ProgressText.text = _downloadingText;
				}
				else
				{
					ProgressText.text = $"{_downloadingText}  {_bytesDownloaded}kb/{_bytesAll}kb";
				}
			}
		}

		private void CheckLoading()
		{
			StartCheckAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}

		private bool GetAvailableMemoryInGB(out float memory)
		{
			try
			{
				using AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				using AndroidJavaObject androidJavaObject = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
				using AndroidJavaObject androidJavaObject2 = androidJavaObject.Call<AndroidJavaObject>("getSystemService", new object[1] { "activity" });
				using AndroidJavaObject androidJavaObject3 = new AndroidJavaObject("android.app.ActivityManager$MemoryInfo");
				androidJavaObject2.Call("getMemoryInfo", androidJavaObject3);
				float num = (float)androidJavaObject3.Get<long>("availMem") / 1.0737418E+09f;
				memory = num;
				return true;
			}
			catch
			{
				memory = 0f;
				return false;
			}
		}

		private async UniTask StartCheckAsync(CancellationToken ct = default(CancellationToken))
		{
			try
			{
				if (GetAvailableMemoryInGB(out var memory) && memory < 0.4f)
				{
					Application.Quit();
					return;
				}
				if (AndroidAssetPacks.GetCoreUnityAssetPackNames().Length == 0)
				{
					OtherStart();
					return;
				}
				if (!AndroidAssetPacks.coreUnityAssetPacksDownloaded)
				{
					string[] coreUnityAssetPackNames = AndroidAssetPacks.GetCoreUnityAssetPackNames();
					_mRemainingsToDownload = coreUnityAssetPackNames.Length;
					_cointOfLoadedPacks = _mRemainingsToDownload;
					AndroidAssetPacks.DownloadAssetPackAsync(coreUnityAssetPackNames, DownloadAssetPackAsyncCallback);
				}
				else
				{
					OtherStart();
				}
				await UniTask.Yield(ct);
			}
			catch (OperationCanceledException)
			{
			}
		}

		private void OtherStartDelayed()
		{
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe(OnOtherStart).AddTo(this);
		}

		private void OnOtherStart(long _)
		{
			OtherStart();
		}

		private void OtherStart()
		{
			Log("OtherStart load scene");
			SceneManager.LoadScene("LoadingSecond");
		}

		[Conditional("DEVELOPMENT_BUILD")]
		[Conditional("UNITY_EDITOR")]
		private static void WLog(string message)
		{
			UnityEngine.Debug.Log("|EDITOR ONLY LOGS|: " + message);
		}

		private static void Log(string message)
		{
			UnityEngine.Debug.Log(message);
		}

		private void OnDestroy()
		{
			Instance = null;
		}
	}
}
