using System;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Database.PlayerData;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.WindowSystem;
using _Car_Parking.Scripts.WindowSystem.PrivacyPolicy;

namespace _Car_Parking.Scripts.EventSystem.View
{
	public class TimeAttackBannerHelper : MonoBehaviour
	{
		private MultiplayerEventStartHandler _multiplayerEventStartHandler;

		private IRemoteConfigManager _remoteConfigManager;

		private ManagerUI _managerUI;

		private V.placeToSpawn _placeToSpawn;

		private Camera _mainCamera;

		private IAutoSaveService _autoSaveService;

		private OptionalDependency<IAutoSaveService> _autoSaveServiceDep;

		[Inject]
		[UnityEngine.Scripting.Preserve]
		public void Construct(IRemoteConfigManager remoteConfigManager, ManagerUI managerUI, Camera mainCamera, OptionalDependency<IAutoSaveService> autoSaveService)
		{
			_remoteConfigManager = remoteConfigManager;
			_managerUI = managerUI;
			_mainCamera = mainCamera;
			_autoSaveServiceDep = autoSaveService;
		}

		private void Awake()
		{
			_autoSaveService = _autoSaveServiceDep?.Value;
		}

		private void Start()
		{
			_multiplayerEventStartHandler = GetComponentInParent<MultiplayerEventStartHandler>();
		}

		private async UniTask StartMultiplayer(V.placeToSpawn placeToSpawn)
		{
			if (!_multiplayerEventStartHandler)
			{
				return;
			}
			if (_autoSaveService != null)
			{
				WaitingWindow.AdditionalData value = new WaitingWindow.AdditionalData
				{
					OverrideSortingLayer = false
				};
				await ManagerUIHelper.ShowWaitingWindow(set: true, backButtonState: false, alwaysOnTop: true, JsonConvert.SerializeObject(value));
				if (await _autoSaveService.SaveDataIfNeedAsync(AutoSaveService.AutoSaveFrom.MultiPlayerMenu) == AutoSaveService.SaveResult.ErrorShouldLogOutOnThisDevice)
				{
					return;
				}
			}
			_placeToSpawn = placeToSpawn;
			_multiplayerEventStartHandler.SetPlaceToSpawn(placeToSpawn);
			_multiplayerEventStartHandler.TryJoinToEventRoom();
		}

		public void OnStart(V.placeToSpawn placeToSpawn)
		{
			_placeToSpawn = placeToSpawn;
			StartMultiplayer(_placeToSpawn).Forget();
		}

		public async UniTaskVoid TryEnablePrivacy()
		{
			ConfigValuesStorage configValuesStorage = _remoteConfigManager.GetDefaultConfigStorage();
			try
			{
				configValuesStorage = await _remoteConfigManager.GetConfigValuesStorage().Timeout(TimeSpan.FromSeconds(5.0));
			}
			catch (Exception message)
			{
				Debug.LogError(message);
			}
			if (configValuesStorage.parentalControlStatus == 0)
			{
				TryEnableOnlineWarningAsync().Forget();
			}
		}

		private async UniTaskVoid TryEnableOnlineWarningAsync()
		{
			OnlineWarningWindow obj = await _managerUI.CreateWindowGeneric<OnlineWarningWindow>("OnlineWarningWindow", onTop: true);
			obj.DontShowNexWindow = true;
			obj.InitOnCloseCallback(() =>
			{
				TryEnablePrivacyAsync().Forget();
			});
		}

		private async UniTaskVoid TryEnablePrivacyAsync()
		{
			(await _managerUI.CreateWindowGeneric<PrivacyAgeConfirmationWindow>("PrivacyAgeConfirmationWindow", onTop: true)).InitOnCloseCallback(() =>
			{
				StartMultiplayer(_placeToSpawn).Forget();
			});
		}

		public void ShowSelectCarWindow(EventType eventType)
		{
			if (ScenesHelper.IsMenuSceneActive())
			{
				_mainCamera.enabled = true;
				V.CoreGameManager.ManagerUI.ShowCircuitCarSelectWindow(JsonConvert.SerializeObject(eventType));
				V.CoreGameManager.ManagerUI.CloseCurrentWindow();
			}
		}
	}
}
