using System;
using CPM.Account;
using CPM.Balance;
using CPM.Connection;
using CPM.Connection.Entities;
using CPM.Database;
using CPM.Scripts.Garage;
using CarInteractionAnimationsScripts;
using Cinemachine;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Persistence.Systems;
using Racing.CameraManagement;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Characters;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.Account;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.FirstGameSetup.Presets;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.UIElement.Settings;
using _Car_Parking.Scripts.WindowSystem;
using _Car_Parking.Scripts.WindowSystem.InitializersWindows.AuthorizationWindowInitializers;
using _Car_Parking.Scripts.WindowSystem.InitializersWindows.GameOverlayWindowInitializers;
using _Car_Parking.Scripts.WindowSystem.InitializersWindows.PersonWindowInitializers;

namespace _Car_Parking.Windows.FirstGameWindow
{
	public class FirstGameWindow : WindowScript, IBackableWindow
	{
		private const string PersonCustomizationCompletedPrefsKey = "PersonCustomizationCompleted";

		private FreeDriveProvider _freeDriveProvider;

		private ICharacterEquipmentDataService _characterEquipmentDataService;

		private IPrefabFactory _prefabFactory;

		private IGarageSelectorService _garageSelectorService;

		private GaragePositionsConfig _garagePositionsConfig;

		private Camera _camera;

		private CameraManager _cameraManager;

		private IRegistrationController _registrationController;

		private OptionalDependency<Camera> _cameraDep;

		private OptionalDependency<CameraManager> _cameraManagerDep;

		private OptionalDependency<IRegistrationController> _registrationControllerDep;

		private IAccountDataProvider _accountDataProvider;

		private CharacterCreationAnalytic _characterCreationAnalytic;

		private IAccountController _accountController;

		private IGuestAccountCreator _guestAccountCreator;

		private PresetGrantSession _presetGrantSession;

		private ISteamAccountService _steamAccountService;

		private DataBase _dataBase;

		private BootStartData _bootStartData;

		private ServerTimeService _serverTimeService;

		private const string ExitGameConfirmString = "SteamSettings.ExitGameConfirm";

		[SerializeField]
		private Button ExitGameButton;

		public MainPerson _mainPerson;

		private bool _isDestroyed;

		private IConnectionDataHandler _connectionDataHandler;

		private IBalanceDocumentSynchronizer _balanceDocumentSynchronizer;

		private IGameDataRepository _gameDataRepository;

		[field: SerializeField]
		[field: Header("Canvas objects")]
		public Button LogInButton { get; private set; }

		[field: SerializeField]
		public Button ContinueButton { get; private set; }

		[field: SerializeField]
		public Button SteamLoginButton { get; private set; }

		[field: SerializeField]
		public Button EditCharacterButton { get; private set; }

		[field: SerializeField]
		public TMP_InputField NiknameInputField { get; private set; }

		[field: SerializeField]
		public CanvasGroup CanvasGroup { get; private set; }

		[field: SerializeField]
		public GameObject MainPanel { get; private set; }

		[field: SerializeField]
		public GameObject DiscardChangesPanel { get; private set; }

		[field: SerializeField]
		public Button KeepButton { get; private set; }

		[field: SerializeField]
		public Button ResetButton { get; private set; }

		[field: Header("Views")]
		[field: SerializeField]
		public PersonPresetsView PersonPresetsView { get; private set; }

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Garage;

		protected override bool ChangeRootGameInputGroup => true;

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, ICharacterEquipmentDataService characterEquipmentDataService, IPrefabFactory prefabFactory, IGarageSelectorService garageSelectorService, GaragePositionsConfig garagePositionsConfig, ManagerUI managerUI, IAccountDataProvider accountDataProvider, CharacterCreationAnalytic characterCreationAnalytic, IAccountController accountController, IGuestAccountCreator guestAccountCreator, PresetGrantSession presetGrantSession, OptionalDependency<Camera> camera, OptionalDependency<CameraManager> cameraManager, OptionalDependency<IRegistrationController> registrationController, ISteamAccountService steamAccountService, DataBase dataBase, BootStartData bootStartData, ServerTimeService serverTimeService)
		{
			_freeDriveProvider = freeDriveProvider;
			_characterEquipmentDataService = characterEquipmentDataService;
			_prefabFactory = prefabFactory;
			_garageSelectorService = garageSelectorService;
			_garagePositionsConfig = garagePositionsConfig;
			_cameraDep = camera;
			_cameraManagerDep = cameraManager;
			_registrationControllerDep = registrationController;
			_managerUI = managerUI;
			_accountDataProvider = accountDataProvider;
			_characterCreationAnalytic = characterCreationAnalytic;
			_accountController = accountController;
			_steamAccountService = steamAccountService;
			_dataBase = dataBase;
			_guestAccountCreator = guestAccountCreator;
			_bootStartData = bootStartData;
			_serverTimeService = serverTimeService;
			_presetGrantSession = presetGrantSession;
		}

		[Inject]
		public void ConstructSteamStartup(IConnectionDataHandler connectionDataHandler, IBalanceDocumentSynchronizer balanceDocumentSynchronizer, IGameDataRepository gameDataRepository)
		{
			_connectionDataHandler = connectionDataHandler;
			_balanceDocumentSynchronizer = balanceDocumentSynchronizer;
			_gameDataRepository = gameDataRepository;
		}

		protected override void Start()
		{
			base.Start();
			_camera = _cameraDep?.Value;
			_cameraManager = _cameraManagerDep?.Value;
			_registrationController = _registrationControllerDep?.Value;
			InitAsync().Forget();
			if ((bool)ExitGameButton)
			{
				ExitGameButton.onClick.AddListener(ExitGame);
				InitBackButton();
			}
		}

		public void InitBackButton()
		{
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			ExitGame();
		}

		public async UniTask InitAsync()
		{
			await Init();
			PersonPresetsView.Init(this);
			LogInButton.onClick.AddListener(OpenLogIn);
			EditCharacterButton.onClick.AddListener(EditCharacter);
			NiknameInputField.text = _accountDataProvider.Name;
			bool flag = AccountSystemV2Extension.IsGuestAccount(_accountController.Email);
			LogInButton.SetActive(flag || !_accountController.HasEmail);
			bool flag2 = !_accountController.HasEmail | flag;
			if ((_steamAccountService != null && !ManagerKeysForPrefs.IsSteamLinked) & flag2)
			{
				ContinueButton.gameObject.SetActive(value: false);
				SteamLoginButton.gameObject.SetActive(value: true);
				SteamLoginButton.onClick.AddListener(OnContinueWithSteam);
			}
			else
			{
				SteamLoginButton.gameObject.SetActive(value: false);
				ContinueButton.gameObject.SetActive(value: true);
				ContinueButton.onClick.AddListener(Continue);
			}
			EditCharacterButton.SetActive(active: false);
		}

		private void ExitGame()
		{
			HideShowAllElements(show: false);
			_managerUI.ShowConfirmWindow(() =>
			{
				Application.Quit();
			}, new LocalizedString("SteamSettings.ExitGameConfirm"), "", () =>
			{
				_managerUI.CloseWindow("ConfirmWindow");
				HideShowAllElements(show: true);
				InitBackButton();
			}).Forget();
		}

		private void HideShowAllElements(bool show)
		{
			CanvasGroup.alpha = (show ? 1 : 0);
			CanvasGroup.interactable = show;
		}

		private async UniTask Init()
		{
			if (!V.DriverPerson)
			{
				_characterEquipmentDataService.Gender = PersonPresetsView.CharacterEquipmentPresetsSo.Presets[0].Gender;
				V.DriverPerson = _prefabFactory.Instantiate(Resources.Load("MenuPerson/person" + _characterEquipmentDataService.Gender, typeof(GameObject)) as GameObject);
			}
			else
			{
				PersonPresetsView.ToggleGroup.allowSwitchOff = true;
			}
			_mainPerson = V.DriverPerson.GetComponent<MainPerson>();
			NewMoveController component = V.DriverPerson.GetComponent<NewMoveController>();
			if ((bool)_mainPerson && (bool)_mainPerson.PersonAnimationsController)
			{
				PersonAnimationsController personAnimationsController = _mainPerson.PersonAnimationsController;
				personAnimationsController.Animator = personAnimationsController.GetComponent<Animator>();
				_mainPerson.PersonAnimationsController.Init(component);
			}
			V.DriverPerson.transform.position = _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].PersonCustomizationPosition.Position;
			V.DriverPerson.transform.rotation = _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].PersonCustomizationPosition.Rotation;
			NiknameInputField.characterLimit = AccountSection.MaxPlayerNameLength;
			NiknameInputField.SetTextWithoutNotify(V.CoreGameManager.NikNameInputText);
			NiknameInputField.onValueChanged.AddListener((string name) =>
			{
				V.CoreGameManager.NikNameInputText = name;
			});
			FlagsStorage.OutOfCar = true;
			if (!V.CoreGameManager.PersonAnimated && _mainPerson.PersonAnimationsController.IsThereMoveAnimations)
			{
				AnimatePlayer(null, () =>
				{
					V.CoreGameManager.PersonAnimated = true;
					InitCamera(_mainPerson.middle);
				}).Forget();
			}
			else
			{
				InitCamera(_mainPerson.middle);
				_mainPerson.PersonAnimationsController.Animator.applyRootMotion = false;
				_cameraManager.PersonShopCam.LockCamera(isLocked: false);
				CanvasGroup.alpha = 1f;
				CanvasGroup.interactable = true;
			}
		}

		public async UniTask AnimatePlayer(Action enter, Action exit)
		{
			enter?.Invoke();
			_cameraManager.PersonShopCam.LockCamera(isLocked: true);
			Transform obj = _mainPerson.PersonAnimationsController.transform;
			obj.position = _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].StartPlaceToMovePlayerToShop.Position;
			obj.rotation = _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].StartPlaceToMovePlayerToShop.Rotation;
			await UniTask.Delay(TimeSpan.FromSeconds(1.5));
			if (_isDestroyed || !_mainPerson)
			{
				return;
			}
			_mainPerson.PersonAnimationsController.PlayRandomMoveOneShotAnimation();
			_mainPerson.PersonAnimationsController.IsOneShotAnimating = false;
			while ((bool)_cameraManager.CurrentDollyCart)
			{
				await UniTask.Yield();
				if (_isDestroyed)
				{
					return;
				}
			}
			Transform transform = new GameObject("targetToLookAt").transform;
			transform.position = _garagePositionsConfig.GaragePositionsDatas[_garageSelectorService.CurrentGarageIndex].PositionTargetForCamera;
			_cameraManager.ChangeTarget(transform);
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				_cameraManager.PersonShopCam.LockCamera(isLocked: false);
			}).AddTo(this);
			CanvasGroup.alpha = 1f;
			CanvasGroup.interactable = true;
			_mainPerson.PersonAnimationsController.Animator.applyRootMotion = false;
			exit?.Invoke();
		}

		private void InitCamera(Transform target)
		{
			_cameraManager.firstCam.SetActive(value: false);
			_cameraManager.ActivateCamera(RacingCameraType.PersonShop, target, new CinemachineBlendDefinition(CinemachineBlendDefinition.Style.EaseOut, 0f));
			_cameraManager.ForceSetCameraZoom(50f);
			_cameraManager.PersonShopCam.SetOriginalZoomValue(0.8f);
			_cameraManager.PersonShopCam.SetMiddleRigScreenX(0.5f);
		}

		private void OpenLogIn()
		{
			_managerUI.AuthorizationWindowInitializer = _prefabFactory.Create<OnlyLogInInitializer>(Array.Empty<object>());
			_managerUI.SetPrevWindow(base.gameObject.name);
			AuthorizationWindow.AdditionalData value = new AuthorizationWindow.AdditionalData
			{
				BackWindowKey = "FirstGameWindow"
			};
			_managerUI.ShowWindow("AuthorizationWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, JsonConvert.SerializeObject(value)).Forget();
		}

		private void Continue()
		{
			ContinueButton.onClick.RemoveListener(Continue);
			_presetGrantSession.Clear();
			_characterEquipmentDataService.Save();
			_guestAccountCreator.CreateGuestAccountIfNeeds().Forget();
			_managerUI.CloseBalanceWindow();
			UnityEngine.Object.Destroy(V.DriverPerson);
			V.CoreGameManager.IsPlayerSetuping = false;
			PlayerPrefs.SetInt("PersonCustomizationCompleted", 1);
			if (V.CoreGameManager.NikNameInputText.Length > 0)
			{
				_registrationController.PlayerNameChanged(V.CoreGameManager.NikNameInputText);
			}
			_characterCreationAnalytic.LogCharacterCreation(CharacterCreationType.Continue);
		}

		private void OnContinueWithSteam()
		{
			SteamLoginButton.onClick.RemoveListener(OnContinueWithSteam);
			SteamLoginButton.interactable = false;
			ContinueButton.interactable = false;
			ContinueWithSteamAsync().Forget();
		}

		private async UniTaskVoid ContinueWithSteamAsync()
		{
			_ = 6;
			try
			{
				_managerUI.ShowWaitingWindow(set: true).Forget();
				bool isNewAccount = false;
				bool signedIn = false;
				string newLocalId = null;
				if (!SteamManager.IsInitialized())
				{
					SteamLoginButton.interactable = true;
					ContinueButton.interactable = true;
					SteamLoginButton.onClick.AddListener(OnContinueWithSteam);
					_managerUI.CloseWaitingWindow();
					_managerUI.ShowNotificationWindow(new LocalizedString("SteamError.SteamUnavailable"), 3f, NotificationType.Error).Forget();
					return;
				}
				SteamAuthResponse steamAuthResponse = await _steamAccountService.CheckSteamIdAsync();
				ObscuredPrefs.SetString("CarsToSave", "");
				_dataBase.ReInit();
				EasySaveDataHandler.DeleteAll();
				if (steamAuthResponse != null && steamAuthResponse.CustomToken != "0")
				{
					signedIn = await _accountController.SignInWithSteamToken(steamAuthResponse.CustomToken, isNewAccount: false, steamAuthResponse.Email);
				}
				else if (steamAuthResponse != null)
				{
					SteamAuthResponse startResponse = await _steamAccountService.StartWithSteamIDAsync();
					if (startResponse == null)
					{
						SteamLoginButton.interactable = true;
						SteamLoginButton.onClick.AddListener(OnContinueWithSteam);
						_managerUI.CloseWaitingWindow();
						return;
					}
					signedIn = await _accountController.SignInWithSteamToken(startResponse.CustomToken, isNewAccount: true, startResponse.Email);
					isNewAccount = true;
					newLocalId = startResponse.LocalId;
				}
				if (signedIn)
				{
					EasySaveDataHandler.Initialize();
					await _accountController.HandleDeviceLogin();
					_steamAccountService.LogSuccessfulLogin();
					if (isNewAccount)
					{
						if (!string.IsNullOrEmpty(newLocalId))
						{
							_accountDataProvider.LocalID = newLocalId;
						}
						await _dataBase.SendAllRecords(saveCars: true, fastSave: false, useEndProgressContainer: false);
						_connectionDataHandler.ProcessUserConnectionData(UserConnectionData.Default);
						await _balanceDocumentSynchronizer.EnsureFreshAsync(this.GetCancellationTokenOnDestroy());
						_gameDataRepository.ExecuteAsync(GameDataRepositoryDataType.Part).Forget();
					}
					ES3.Save("loginTime", _serverTimeService.CurrentServerUnixTimeSec);
					ObscuredPrefs.Save();
					PlayerPrefs.Save();
					if (!isNewAccount)
					{
						PlayerPrefs.SetInt("PersonCustomizationCompleted", 1);
						PlayerPrefs.Save();
						_bootStartData.LoadingType = BootStartLoadingType.RegistrationSignIn;
						SceneManager.LoadScene(0);
						return;
					}
					_managerUI.CloseWaitingWindow();
					_managerUI.ShowNotificationWindow(new LocalizedString("Steamauthorization.AccountLinkedToSteamProfile"), 3f).Forget();
					UnityEngine.Object.Destroy(V.DriverPerson);
					V.CoreGameManager.IsPlayerSetuping = false;
					PlayerPrefs.SetInt("PersonCustomizationCompleted", 1);
					PlayerPrefs.Save();
					if (V.CoreGameManager.NikNameInputText.Length > 0)
					{
						_registrationController?.PlayerNameChanged(V.CoreGameManager.NikNameInputText);
					}
					_characterCreationAnalytic.LogCharacterCreation(CharacterCreationType.Continue);
				}
				else
				{
					SteamLoginButton.interactable = true;
					ContinueButton.interactable = true;
					SteamLoginButton.onClick.AddListener(OnContinueWithSteam);
					_managerUI.CloseWaitingWindow();
					_managerUI.ShowNotificationWindow("Steam authentication failed", 3f, NotificationType.Error).Forget();
				}
			}
			catch (Exception)
			{
				SteamLoginButton.interactable = true;
				ContinueButton.interactable = true;
				SteamLoginButton.onClick.AddListener(OnContinueWithSteam);
				_managerUI.CloseWaitingWindow();
				_managerUI.ShowNotificationWindow("Steam authentication failed", 3f, NotificationType.Error).Forget();
			}
		}

		private void EditCharacter()
		{
			_managerUI.PersonWindowInitializer = _prefabFactory.Create<FirstOpenGamePersonWindowInitializer>(Array.Empty<object>());
			_managerUI.ShowPersonCustomizationWindow();
			_managerUI.SetPrevWindow(base.gameObject.name);
			_managerUI.ShowUserBalanceWindow(_prefabFactory.Create<GameOverlayOnlyBalanceWindowInitializer>(Array.Empty<object>()));
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			ContinueButton.onClick.RemoveAllListeners();
			LogInButton.onClick.RemoveAllListeners();
			EditCharacterButton.onClick.RemoveAllListeners();
			KeepButton.onClick.RemoveAllListeners();
			ResetButton.onClick.RemoveAllListeners();
			SteamLoginButton.onClick.RemoveAllListeners();
			ExitGameButton.onClick.RemoveAllListeners();
			_isDestroyed = true;
		}
	}
}
