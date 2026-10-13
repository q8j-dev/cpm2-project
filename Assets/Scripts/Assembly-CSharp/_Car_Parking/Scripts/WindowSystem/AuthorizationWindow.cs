using System;
using CPM.Account;
using CPM.Database.Account;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.Account;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.FirstGameSetup;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem.ProgressWindows;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class AuthorizationWindow : WindowScript, IBackableWindow
	{
		public enum TypeActions
		{
			Both = 0,
			SignIn = 1,
			SignUp = 2
		}

		public class AdditionalData
		{
			public TypeActions TypeActions;

			public string BackWindowKey;
		}

		private const string PersonCustomizationCompletedPrefsKey = "PersonCustomizationCompleted";

		[SerializeField]
		private ButtonEnableDisableColor SignInButton;

		[SerializeField]
		private CanvasGroup CurrentCanvasGroup;

		[SerializeField]
		private Button ResetPasswordButton;

		[SerializeField]
		private Button SteamSignInButton;

		[SerializeField]
		private string BackWindowKey;

		[SerializeField]
		private GameObject TopBoth;

		[SerializeField]
		private GameObject TopOnlySignIn;

		[SerializeField]
		private GameObject TopOnlySignUp;

		[SerializeField]
		private GameObject SteamSignInObject;

		[SerializeField]
		private TMP_Text[] SignInTitles;

		[SerializeField]
		private TMP_Text[] SignUpTitles;

		private bool _isAllInputEmpty = true;

		private bool _isEmailEmpty;

		private bool _isPasswordEmpty;

		private InputFieldStateController _emailStateController;

		private InputFieldStateController _passwordStateController;

		private InputFieldStateController _confirmPasswordStateController;

		private IRegistrationController _registrationController;

		private ITimerTuningService _timerTuningService;

		private ProgressContainer _progressContainer;

		private IEngineTuningTimerService _engineTuningTimerService;

		private FreeDriveProvider _freeDriveProvider;

		private ProfileAnalytic _profileAnalytic;

		private ServerTimeService _serverTimeService;

		private CharacterCreationAnalytic _characterCreationAnalytic;

		private AdditionalData _additionalData;

		private bool _firstSingUp;

		private IAccountController _accountController;

		private bool _started;

		[field: SerializeField]
		public Toggle SignUpToggle { get; private set; }

		[field: SerializeField]
		public Button BackButton { get; private set; }

		[field: SerializeField]
		public TMP_InputField Email { get; set; }

		[field: SerializeField]
		public TMP_InputField Password { get; set; }

		[field: SerializeField]
		public TMP_InputField ConfirmPassword { get; set; }

		[field: SerializeField]
		public ButtonEnableDisableColor SignUpButton { get; set; }

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(IRegistrationController registrationController, ITimerTuningService timerTuningService, IEngineTuningTimerService engineTuningTimerService, FreeDriveProvider freeDriveProvider, ProfileAnalytic profileAnalytic, ServerTimeService serverTimeService, CharacterCreationAnalytic characterCreationAnalytic, IAccountController accountController)
		{
			_freeDriveProvider = freeDriveProvider;
			_engineTuningTimerService = engineTuningTimerService;
			_timerTuningService = timerTuningService;
			_registrationController = registrationController;
			_profileAnalytic = profileAnalytic;
			_serverTimeService = serverTimeService;
			_characterCreationAnalytic = characterCreationAnalytic;
			_accountController = accountController;
		}

		protected override void Start()
		{
			base.Start();
			TryInitWindow();
			_emailStateController = Email.GetComponent<InputFieldStateController>();
			_passwordStateController = Password.GetComponent<InputFieldStateController>();
			_confirmPasswordStateController = ConfirmPassword.GetComponent<InputFieldStateController>();
			_emailStateController.OnEmpty += OnEmailEmpty;
			_passwordStateController.OnEmpty += OnPasswordEmpty;
			_confirmPasswordStateController.OnEmpty += OnPassConfirmEmpty;
			SignUpToggle.onValueChanged.AddListener(ShowSignUpElements);
			SignInButton.Button.onClick.AddListener(() =>
			{
				TrySignIn().Forget();
			});
			SignUpButton.Button.onClick.AddListener(() =>
			{
				TrySignUp().Forget();
			});
			ResetPasswordButton.onClick.AddListener(() =>
			{
				TryResetPassword().Forget();
			});
			SteamSignInButton.onClick.AddListener(() =>
			{
				TrySteamSignIn().Forget();
			});
			_timerTuningService.IsPause = true;
			_engineTuningTimerService.IsOnPause = true;
			SignInButton.Disable();
			SignUpButton.Disable();
			ApplyTypeActions();
			TryInitBackButtonFromAdditionalData();
			string localizationKey = "Authorizationwindow.SignIn";
			string localizationKey2 = "Authorizationwindow.SignUp";
			if (AccountSystemV2Extension.IsSteamOnlyAccount(_accountController.Email))
			{
				localizationKey = "Authorizationwindow.SignIn";
				localizationKey2 = "Steamauthorization.LinkEmail";
			}
			else if (_accountController.HasEmail && !AccountSystemV2Extension.IsGuestAccount(_accountController.Email))
			{
				localizationKey = "Achievmentsauthorizationwindow.ChangeAccount";
				localizationKey2 = "Achievmentsauthorizationwindow.NewAccount";
			}
			TMP_Text[] signInTitles = SignInTitles;
			foreach (TMP_Text tMP_Text in signInTitles)
			{
				if ((bool)tMP_Text)
				{
					tMP_Text.text = new LocalizedString(localizationKey);
				}
			}
			signInTitles = SignUpTitles;
			foreach (TMP_Text tMP_Text2 in signInTitles)
			{
				if ((bool)tMP_Text2)
				{
					tMP_Text2.text = new LocalizedString(localizationKey2);
				}
			}
		}

		private void UpdateTopGraphics(TypeActions typeActions)
		{
			if ((bool)TopBoth)
			{
				TopBoth.SetActive(typeActions == TypeActions.Both);
			}
			if ((bool)TopOnlySignIn)
			{
				TopOnlySignIn.SetActive(typeActions == TypeActions.SignIn);
			}
			if ((bool)TopOnlySignUp)
			{
				TopOnlySignUp.SetActive(typeActions == TypeActions.SignUp);
			}
		}

		private void TryInitWindow()
		{
			if (_managerUI.AuthorizationWindowInitializer != null)
			{
				_managerUI.AuthorizationWindowInitializer.Init(this);
				_managerUI.AuthorizationWindowInitializer = null;
			}
		}

		public void InitFirstSingUp()
		{
			_firstSingUp = true;
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			BackButton.SetActive(active: false);
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			if (string.IsNullOrEmpty(data))
			{
				return;
			}
			_additionalData = JsonConvert.DeserializeObject<AdditionalData>(data);
			if (_additionalData != null)
			{
				if (!string.IsNullOrEmpty(_additionalData.BackWindowKey))
				{
					BackWindowKey = _additionalData.BackWindowKey;
				}
				if (_started)
				{
					ApplyTypeActions();
				}
				if (_started)
				{
					TryInitBackButtonFromAdditionalData();
				}
			}
		}

		private void TryInitBackButtonFromAdditionalData()
		{
			if (_additionalData != null && !_firstSingUp && BackWindowKey != "WrongAuthorization")
			{
				InitBackButton();
			}
		}

		private void ApplyTypeActions()
		{
			_started = true;
			UpdateTopGraphics(TypeActions.Both);
			if (_additionalData != null && _additionalData.TypeActions == TypeActions.SignIn)
			{
				ShowSignUpElements(isActive: false);
				UpdateTopGraphics(TypeActions.SignIn);
			}
			else if (_additionalData != null && _additionalData.TypeActions == TypeActions.SignUp)
			{
				ShowSignUpElements(isActive: true);
				UpdateTopGraphics(TypeActions.SignUp);
			}
		}

		private async UniTask TryResetPassword()
		{
			Email.text = Email.text.Trim();
			if (!AccountUtilities.IsValidEmail(Email.text) || AccountSystemV2Extension.IsReservedGuestEmail(Email.text))
			{
				ShowWrongEmailError();
				return;
			}
			(bool, string) tuple = await _registrationController.ResetPassword(Email.text);
			NotificationType type = ((!tuple.Item1) ? NotificationType.Error : NotificationType.Success);
			_managerUI.ShowNotificationWindow(tuple.Item2, 3f, type).Forget();
		}

		protected override void OnDestroy()
		{
			_timerTuningService.IsPause = false;
			_engineTuningTimerService.IsOnPause = false;
			BackButton.onClick.RemoveAllListeners();
			base.OnDestroy();
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			_managerUI.ShowWindow(BackWindowKey).Forget();
		}

		private async UniTaskVoid TrySignUp()
		{
			UpdateEmailPasswordDebug();
			Email.text = Email.text.Trim();
			if (!AccountUtilities.IsValidEmail(Email.text) || AccountSystemV2Extension.IsReservedGuestEmail(Email.text))
			{
				ShowWrongEmailError();
				return;
			}
			if (Password.text.Trim() != ConfirmPassword.text.Trim())
			{
				ShowPasswordNotMatchError();
				return;
			}
			if (Password.text.Length < 6)
			{
				ShowShortPasswordError();
				return;
			}
			ToggleAllOtherUI(enable: false);
			await _managerUI.ShowProgressWindow(new LocalizedString("AuthorizationWindow.SignUp"));
			(SignUpResult, bool) tuple = await _registrationController.CreateNewAccount(Email.text, Password.text);
			if (tuple.Item1 != SignUpResult.Success || !tuple.Item2)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("AuthorizationWindow.Can'tcreateaccount."), 3f, NotificationType.Error).Forget();
				_profileAnalytic.LogRegistration(RegistrationStatus.REGISTER_FAILED, Email.text);
			}
			else
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("AuthorizationWindow.Accountwascreated."), 3f).Forget();
				if ((bool)V.CoreGameManager)
				{
					if (!V.CoreGameManager.IsPlayerSetuping)
					{
						_managerUI.ShowMainMenuWindow();
					}
					else
					{
						V.CoreGameManager.IsPlayerSetuping = false;
						PlayerPrefs.SetInt("PersonCustomizationCompleted", 1);
						_characterCreationAnalytic.LogCharacterCreation(CharacterCreationType.Sign_Up);
					}
					if (V.CoreGameManager.NikNameInputText.Length > 0)
					{
						_registrationController.PlayerNameChanged(V.CoreGameManager.NikNameInputText);
						if (PlayerStorageServiceExtension.IsRandomGuestName(V.CoreGameManager.NikNameInputText))
						{
							_registrationController.PlayerNameChanged(PlayerStorageServiceExtension.GetRandomPlayerName());
						}
					}
				}
				if (LevelProgressionHelper.LevelCompletedAmount > 0)
				{
					LevelProgressionHelper.SendLevelsProgressionToDataBase(forceSend: true, _freeDriveProvider);
				}
				ES3.Save("loginTime", _serverTimeService.CurrentServerUnixTimeSec);
				_profileAnalytic.LogRegistration(RegistrationStatus.REGISTER_COMPLETED, Email.text);
				Close();
			}
			ToggleAllOtherUI(enable: true);
		}

		private async UniTaskVoid TrySignIn()
		{
			UpdateEmailPasswordDebug();
			Email.text = Email.text.Trim();
			if (Password.text.Trim().Length < 6)
			{
				ShowShortPasswordError();
				return;
			}
			if (!AccountUtilities.IsValidEmail(Email.text) || AccountSystemV2Extension.IsReservedGuestEmail(Email.text))
			{
				ShowWrongEmailError();
				return;
			}
			ToggleAllOtherUI(enable: false);
			await _managerUI.ShowProgressWindow(new LocalizedString("AuthorizationWindow.SignIn"));
			bool num = await _registrationController.SignInNew(Email.text, Password.text);
			if (num && (bool)V.CoreGameManager && V.CoreGameManager.IsPlayerSetuping)
			{
				_characterCreationAnalytic.LogCharacterCreation(CharacterCreationType.Log_In);
			}
			if (num && LevelProgressionHelper.LevelCompletedAmount > 0)
			{
				LevelProgressionHelper.SendLevelsProgressionToDataBase(forceSend: true, _freeDriveProvider);
			}
			ToggleAllOtherUI(enable: true);
		}

		private async UniTaskVoid TrySteamSignIn()
		{
			ToggleAllOtherUI(enable: false);
			await _managerUI.ShowProgressWindow(new LocalizedString("AuthorizationWindow.SignIn"));
			try
			{
				if (await _registrationController.SignInWithSteamNew() && (bool)V.CoreGameManager && V.CoreGameManager.IsPlayerSetuping)
				{
					_characterCreationAnalytic.LogCharacterCreation(CharacterCreationType.Log_In);
				}
			}
			catch (Exception)
			{
				_managerUI.ShowNotificationWindow("Steam sign-in error", 3f, NotificationType.Error).Forget();
			}
			ToggleAllOtherUI(enable: true);
		}

		private void ToggleAllOtherUI(bool enable)
		{
			SetSignButtonsInteractable(enable);
			_managerUI.ToggleBalanceWindow(enable);
			if (enable)
			{
				ShowWindow();
			}
			else
			{
				HideWindow();
			}
		}

		private void SetSignButtonsInteractable(bool enable)
		{
			if ((bool)SignInButton && (bool)SignUpButton)
			{
				SignInButton.Button.interactable = enable;
				SignUpButton.Button.interactable = enable;
				if (enable)
				{
					SignInButton.Enable();
					SignUpButton.Enable();
				}
				else
				{
					SignInButton.Disable();
					SignUpButton.Disable();
				}
			}
		}

		private void ShowWrongEmailError()
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("AuthorizationWindow.Wrongemail"), 3f, NotificationType.Error).Forget();
			_emailStateController.SetError();
		}

		private void ShowPasswordNotMatchError()
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("AuthorizationWindow.Passwordsdoesn'tmatch"), 3f, NotificationType.Error).Forget();
			_passwordStateController.SetError();
		}

		private void ShowShortPasswordError()
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("AuthorizationWindow.Passwordmustbeatleast6characters!"), 3f, NotificationType.Error).Forget();
			_passwordStateController.SetError();
		}

		private void ShowSignUpElements(bool isActive)
		{
			ConfirmPassword.SetActive(isActive);
			SignUpButton.SetActive(isActive);
			SignInButton.SetActive(!isActive);
			ResetPasswordButton.SetActive(!isActive);
			SteamSignInObject.SetActive(!isActive);
		}

		private void OnEmailEmpty(bool isEmpty)
		{
			_isEmailEmpty = isEmpty;
			CheckAllInputField();
		}

		private void OnPasswordEmpty(bool isEmpty)
		{
			_isPasswordEmpty = isEmpty;
			CheckAllInputField();
		}

		private void OnPassConfirmEmpty(bool isEmpty)
		{
			CheckAllInputField();
		}

		private void CheckAllInputField()
		{
			SetSignButtonsInteractable(!_isEmailEmpty && !_isPasswordEmpty);
		}

		private void HideWindow()
		{
			CurrentCanvasGroup.DOFade(0f, 0.1f);
		}

		private void ShowWindow()
		{
			CurrentCanvasGroup.DOFade(1f, 0.1f);
		}

		private void UpdateEmailPasswordDebug()
		{
		}
	}
}
