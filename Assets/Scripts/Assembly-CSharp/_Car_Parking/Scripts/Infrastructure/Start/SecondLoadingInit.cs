using System;
using System.Threading;
using CPM.Account;
using CPM.Common;
using CPM.Scripts.Garage.GarageBuyer;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.VinylsAi;

namespace _Car_Parking.Scripts.Infrastructure.Start
{
	public class SecondLoadingInit : MonoBehaviour
	{
		private const float AccountSystemLoadingLerp = 1f;

		private const int SceneID = 1;

		[SerializeField]
		private Slider LoadingBar = default;

		[SerializeField]
		private TextMeshProUGUI LoadingProgressText = default;

		[SerializeField]
		private ShaderPrewarmer ShaderPrewarmer = default;

		private PreMenuState _preMenuState;

		private bool _bootStartCompleted;

		private bool _preMenuCompleted;

		private AppStateHandler _appStateHandler;

		private SceneLoadingTimer _sceneLoadingTimer;

		private IGarageBuyService _garageBuyService;

		private IBootstrapHandler _bootstrapHandler;

		private AsyncOperation _menuLoadOperation;

		private bool _bootFailed;

		private float _accountSystemLoadingProgress;

		[Inject]
		public void Construct(SceneLoadingTimer sceneLoadingTimer, IBootstrapHandler bootstrapHandler, IGarageBuyService garageBuyService, IAccountController accountController, PreMenuState preMenuState, AppStateHandler appStateHandler)
		{
			_sceneLoadingTimer = sceneLoadingTimer;
			_garageBuyService = garageBuyService;
			_bootstrapHandler = bootstrapHandler;
			_preMenuState = preMenuState;
			_appStateHandler = appStateHandler;
		}

		private void Start()
		{
			AnalyticStorage.StartSession();
			_appStateHandler.ApplicationPause -= AnalyticStorage.OnApplicationPause;
			_appStateHandler.ApplicationPause += AnalyticStorage.OnApplicationPause;
			_garageBuyService.Init();
			Debug.Log("SecondLoadingInit Start");
			BootstrapAsync().Forget();
			_preMenuState.BeforeLoadMenu(OtherStart).Forget();
			StartLoader();
		}

		private void OtherStart()
		{
			if (!(this == null))
			{
				Debug.Log("SecondLoadingInit OtherStart. Pre-menu preparations finished.");
				_preMenuCompleted = true;
			}
		}

		private async UniTask AsyncLoadAsync(CancellationToken ct = default(CancellationToken))
		{
			_ = 3;
			try
			{
				_accountSystemLoadingProgress = 0f;
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				PreWarmAI.TryPreWarm();
				await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
				if (_bootFailed)
				{
					return;
				}
				AsyncOperation operation = SceneManager.LoadSceneAsync(1);
				operation.allowSceneActivation = false;
				_menuLoadOperation = operation;
				while (!operation.isDone)
				{
					float a = Mathf.Clamp01(operation.progress / 0.9f);
					float b = Mathf.Clamp01(GetLoadingProgress());
					float b2 = Mathf.Max(a, b);
					_accountSystemLoadingProgress = Mathf.Lerp(_accountSystemLoadingProgress, b2, 1f * Time.deltaTime);
					LoadingBar.value = _accountSystemLoadingProgress;
					LoadingProgressText.text = $"{(int)(_accountSystemLoadingProgress * 100f)}%";
					if (IsInitialized())
					{
						LoadingBar.value = 1f;
						LoadingProgressText.text = "100%";
						await UniTask.Delay(TimeSpan.FromSeconds(0.05000000074505806), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
						operation.allowSceneActivation = true;
					}
					await UniTask.Yield(ct);
				}
			}
			catch (OperationCanceledException)
			{
			}
		}

		private bool IsInitialized()
		{
			ShaderPrewarmer shaderPrewarmer = ShaderPrewarmer;
			if (((object)shaderPrewarmer == null || shaderPrewarmer.Finished) && _bootStartCompleted)
			{
				return _preMenuCompleted;
			}
			return false;
		}

		private void StartLoader()
		{
			Debug.Log("Start load menu.");
			AsyncLoadAsync(this.GetCancellationTokenOnDestroy()).Forget();
		}

		private float GetLoadingProgress()
		{
			float num = _bootstrapHandler.Progress * (ShaderPrewarmer?.GetCurrentPrewarm() ?? 1f);
			return 1f * num;
		}

		private void OnDestroy()
		{
			if (_sceneLoadingTimer != null)
			{
				_sceneLoadingTimer.StopLoadingTimer();
			}
		}

		private async UniTask BootstrapAsync()
		{
			BootStartResult bootStartResult = await _bootstrapHandler.ExecuteAsync(base.destroyCancellationToken);
			if (bootStartResult == BootStartResult.Success || bootStartResult == BootStartResult.SuccessOfflineMode)
			{
				_bootStartCompleted = true;
				return;
			}
			(bool, SceneNotificationReason) tuple = BootStartResultExtensions.ToSceneNotificationReason(bootStartResult);
			if (tuple.Item1)
			{
				_bootFailed = true;
				if (_menuLoadOperation != null)
				{
					_menuLoadOperation.allowSceneActivation = true;
				}
				ScenesHelper.LoadNotificationScene(tuple.Item2);
			}
		}
	}
}
