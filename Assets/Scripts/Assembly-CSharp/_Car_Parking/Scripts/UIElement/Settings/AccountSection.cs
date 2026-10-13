using System;
using CPM.Account;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.DataStorageService;
using _Car_Parking.Scripts.Database.Account;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning.TuningTimer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.OfflineMode;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Steam;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class AccountSection : SettingsSectionBase
	{
		public static ObscuredInt MaxPlayerNameLength = 12;

		public static ObscuredInt SellCarModelTextLength = 40;

		private const float TimerToDeleteAccount = 10f;

		[SerializeField]
		private Button SaveButton;

		[SerializeField]
		private Button SignOutButton;

		[SerializeField]
		private Button DeleteAccountButton;

		[SerializeField]
		private Button ChangeAccountButton;

		[SerializeField]
		private Button SignInButton;

		[SerializeField]
		private Button SendEmailButton;

		[SerializeField]
		private Button EditNickNameButton;

		[SerializeField]
		private Button RestorePurchaseButton;

		[SerializeField]
		private Button SteamLinkButton;

		[SerializeField]
		private Button SteamUnlinkButton;

		[SerializeField]
		private GameObject SignInMethodsGroup;

		[SerializeField]
		private GameObject GooglePlaySignInButton;

		[SerializeField]
		private TMP_InputField EditableNickName;

		[SerializeField]
		private Text NameLegacyText;

		[SerializeField]
		private InputFieldSizeFitter NickNameSizeFitter;

		[SerializeField]
		private TextMeshProUGUI RankText;

		[SerializeField]
		private TextMeshProUGUI IdText;

		[SerializeField]
		private TextMeshProUGUI EmailText;

		[SerializeField]
		private RawImage RankIcon;

		[SerializeField]
		private GameObject ProgressDescription;

		[SerializeField]
		private GameObject Progress;

		[SerializeField]
		private GameObject Confirmation;

		private ManagerUI _managerUI;

		private IAccountController _accountController;

		private DataBase _database;

		private OptionalDependency<DataBase> _databaseDep;

		private ITimerTuningService _timerTuningService;

		private OptionalDependency<ITimerTuningService> _timerTuningServiceDep;

		private IRegistrationController _registrationController;

		private OptionalDependency<IRegistrationController> _registrationControllerDep;

		private IAccountDataProvider _accountDataProvider;

		private IEngineTuningTimerService _engineTuningTimerService;

		private OptionalDependency<IEngineTuningTimerService> _engineTuningTimerServiceDep;

		private IOfflineModeSystem _offlineModeSystem;

		private IDisposable _purchaseInteractableWait;

		private IPurchaseSystem _purchaseSystem;

		private ISteamAccountService _steamAccountService;

		private float _waitTime = 2f;

		private const float NickNameCaretBlinkSeconds = 0.5f;

		private const string NickNameCaret = "|";

		private bool _nickNameCaretRunning;

		[Inject]
		public void Construct(ManagerUI managerUI, IAccountDataProvider accountDataProvider, IAccountController accountController, IPurchaseSystem purchaseSystem, ISteamAccountService steamAccountService, IOfflineModeSystem offlineModeSystem, OptionalDependency<DataBase> database, OptionalDependency<ITimerTuningService> timerTuningService, OptionalDependency<IEngineTuningTimerService> engineTuningTimerService, OptionalDependency<IRegistrationController> registrationController)
		{
			_engineTuningTimerServiceDep = engineTuningTimerService;
			_accountDataProvider = accountDataProvider;
			_registrationControllerDep = registrationController;
			_timerTuningServiceDep = timerTuningService;
			_databaseDep = database;
			_accountController = accountController;
			_managerUI = managerUI;
			_purchaseSystem = purchaseSystem;
			_offlineModeSystem = offlineModeSystem;
			_steamAccountService = steamAccountService;
		}

		protected override void Awake()
		{
			base.Awake();
			_database = _databaseDep?.Value;
			_timerTuningService = _timerTuningServiceDep?.Value;
			_engineTuningTimerService = _engineTuningTimerServiceDep?.Value;
			_registrationController = _registrationControllerDep?.Value;
		}

		private void Start()
		{
			EditableNickName.text = _accountDataProvider.Name;
			EditableNickName.characterLimit = MaxPlayerNameLength;
			EditableNickName.onValueChanged.AddListener((string value) =>
			{
				NameLegacyText.text = value;
			});
			NameLegacyText.text = _accountDataProvider.Name;
			NickNameSizeFitter.Init();
			IdText.text = _accountDataProvider.LocalID;
			GooglePlaySignInButton?.SetActive(value: false);
			if (string.IsNullOrEmpty(_accountController.Email))
			{
				EmailText.transform.parent.gameObject.SetActive(value: false);
			}
			else
			{
				EmailText.text = _accountController.Email;
			}
			RankIcon.texture = UtilsRank.LoadMyRankTextureFromResources();
			RankText.text = UtilsRank.GetMyRankName();
			if (ScenesHelper.IsMenuSceneActive() && _offlineModeSystem.Disabled)
			{
				_timerTuningService.IsPause = true;
				_engineTuningTimerService.IsOnPause = true;
				EnableAuthorizedButtons(_accountController.HasEmail);
				EnableSaveButton(_database.SynchronizingNow.Value);
				SignInButton.onClick.AddListener(OpenAuthorizationWindow);
				EditNickNameButton.onClick.AddListener(SetEditNickName);
				SaveButton.onClick.AddListener(Save);
				SignOutButton.onClick.AddListener(SignOut);
				DeleteAccountButton.onClick.AddListener(DeleteAccount);
				ChangeAccountButton.onClick.AddListener(ChangeAccount);
				SendEmailButton.onClick.AddListener(OnEmailVerificationPressed);
				EditableNickName.onSubmit.AddListener(SetName);
				SteamLinkButton.onClick.AddListener(OnSteamLinkClicked);
				SteamUnlinkButton.onClick.AddListener(OnSteamUnlinkClicked);
				_database.SynchronizingNow.ObserveEveryValueChanged((ReactiveProperty<bool> x) => x.Value).Subscribe((bool x) =>
				{
					EnableSaveButton(x);
				}).AddTo(this);
				ChangeAccountButton.gameObject.SetActive(value: false);
				bool flag = AccountSystemV2Extension.IsGuestAccount(_accountController.Email);
				bool flag2 = AccountSystemV2Extension.IsSteamOnlyAccount(_accountController.Email);
				SignInButton.SetActive(flag | flag2);
				SignOutButton.SetActive(!flag && !flag2);
				SaveButton.SetActive(active: true);
				DeleteAccountButton.SetActive(active: true);
				if (!flag)
				{
					SendEmailButton.SetActive(!_accountController.IsUserVerified());
				}
				ProgressDescription.SetActive(flag);
				Progress.SetActive(!flag);
				EmailText.transform.parent.gameObject.SetActive(!flag);
				if (_accountController.HasEmail && !flag && !flag2)
				{
					SignOutButton.SetActive(active: false);
					ChangeAccountButton.gameObject.SetActive(value: true);
				}
				if (SteamManager.IsInitialized())
				{
					SignInMethodsGroup?.SetActive(value: true);
					bool isSteamLinked = ManagerKeysForPrefs.IsSteamLinked;
					SteamLinkButton?.SetActive(!isSteamLinked);
					SteamUnlinkButton?.SetActive(isSteamLinked);
					RefreshSteamLinkFromBackend().Forget();
				}
				else
				{
					SignInMethodsGroup?.SetActive(value: false);
					SteamLinkButton?.SetActive(active: false);
					SteamUnlinkButton?.SetActive(active: false);
				}
			}
			else
			{
				EditNickNameButton.SetActive(active: false);
				SignInButton.SetActive(active: false);
				SaveButton.SetActive(active: false);
				SignOutButton.SetActive(active: false);
				DeleteAccountButton.SetActive(active: false);
				Progress.SetActive(value: false);
				ProgressDescription.SetActive(value: false);
				EditableNickName.interactable = false;
				if (AccountSystemV2Extension.IsGuestAccount(_accountController.Email))
				{
					EmailText.transform.parent.gameObject.SetActive(value: false);
				}
			}
		}

		private void SetName(string newName)
		{
			string text = _registrationController.PlayerNameChanged(newName);
			EditableNickName.text = text;
		}

		private void EnableSaveButton(bool savingInProgress)
		{
			if (savingInProgress)
			{
				SaveButton.interactable = false;
			}
			else
			{
				SaveButton.interactable = true;
			}
		}

		private void SetEditNickName()
		{
			EditableNickName.interactable = true;
			EditableNickName.Select();
			EditableNickName.caretPosition = EditableNickName.text.Length;
			SteamShowNickNameCaret();
		}

		private void OpenAuthorizationWindow()
		{
			_managerUI.ShowAuthorizationWindow();
			base.Settings.Close();
		}

		public override void Restore()
		{
			EditableNickName.text = _accountDataProvider.Name;
			IdText.text = _accountDataProvider.LocalID;
			EmailText.text = _accountController.Email;
		}

		private void OnEmailVerificationPressed()
		{
			SendEmailVerificationLink().Forget();
		}

		private async UniTaskVoid SendEmailVerificationLink()
		{
			if (_accountController.IsUserVerified())
			{
				SendEmailButton.SetActive(active: false);
				_managerUI.ShowNotificationWindow(new LocalizedString("AccountSection.Youraccountisalreadyverified"), 3f).Forget();
			}
			else if (await _accountController.VerifyEmail())
			{
				SendEmailButton.SetActive(active: false);
				_managerUI.ShowNotificationWindow(new LocalizedString("AccountSection.Checkyouremail"), 3f).Forget();
			}
			else
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("AccountSection.Errorsendingverificationlink"), 3f, NotificationType.Error).Forget();
			}
		}

		private void EnableAuthorizedButtons(bool enable)
		{
			SignInButton.SetActive(!enable);
			SignOutButton.SetActive(enable);
			SaveButton.SetActive(enable);
			DeleteAccountButton.SetActive(active: true);
			if (enable)
			{
				SendEmailButton.SetActive(!_accountController.IsUserVerified());
			}
			ProgressDescription.SetActive(!enable);
			Progress.SetActive(enable);
			EmailText.transform.parent.gameObject.SetActive(enable);
		}

		private void OnDestroy()
		{
			ChangeAccountButton.onClick.RemoveListener(ChangeAccount);
			if (ScenesHelper.IsMenuSceneActive())
			{
				_purchaseInteractableWait?.Dispose();
				_timerTuningService.IsPause = false;
				_engineTuningTimerService.IsOnPause = false;
			}
		}

		private void ToggleAllButtons(bool enable)
		{
			SaveButton.interactable = enable;
			SignOutButton.interactable = enable;
			DeleteAccountButton.interactable = enable;
			_managerUI.ToggleBalanceWindow(enable);
		}

		private void DeleteAccount()
		{
			if (Application.internetReachability == NetworkReachability.NotReachable)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("MultiplayerLobbyHandler.Nointernetconnection"), 4f).Forget();
				return;
			}
			base.Settings.Hide();
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData();
			float time = 10f;
			additionalData.ConfirmTimer = new ConfirmWindow.AdditionalData.ConfirmTimerData(isEnabled: true, time);
			_managerUI.ShowConfirmWindow(() =>
			{
				DeleteAccountConfirm();
			}, new LocalizedString("AccountSection.DeleteAccount"), new LocalizedString("AccountSection.AllYourProgressWillBeDeleted"), () =>
			{
				base.Settings.Show();
			}, "Confirm", null, additionalData).Forget();
		}

		private void ChangeAccount()
		{
			AuthorizationWindow.AdditionalData value = new AuthorizationWindow.AdditionalData
			{
				TypeActions = AuthorizationWindow.TypeActions.Both
			};
			_managerUI.ShowAuthorizationWindow(JsonConvert.SerializeObject(value));
			base.Settings.Close();
		}

		private void SignOut()
		{
			base.Settings.Hide();
			_managerUI.ShowConfirmWindow(() =>
			{
				SignOutConfirm();
			}, new LocalizedString("AccountSection.SignOut"), new LocalizedString("AccountSection.DoYouWantToSignOutFromAccount"), () =>
			{
				base.Settings.Show();
			}).Forget();
		}

		private async void SignOutConfirm()
		{
			ToggleAllButtons(enable: false);
			Time.timeScale = 1f;
			await _managerUI.ShowProgressWindow(new LocalizedString("AccountSection.SignOut"));
			await _registrationController.SignOut();
			base.Settings.Show();
		}

		private void DeleteAccountConfirm()
		{
			try
			{
				ToggleAllButtons(enable: false);
				_accountController.DeleteAccount().Forget();
				_managerUI.ShowProgressWindow(new LocalizedString("AccountSection.DeleteAccount")).Forget();
			}
			catch (Exception ex)
			{
				Debug.LogError("Error deleting account: " + ex);
				SceneManager.LoadScene(0);
			}
		}

		private async void Save()
		{
			if (!InternetHelper.HasInternetConnection())
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("AccountSection.Nointernetconnection."), 2f).Forget();
				return;
			}
			if (_database.SynchronizingNow.Value)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("AccountSection.Savinginprogressnow.Wait."), 2f).Forget();
				return;
			}
			ToggleAllButtons(enable: false);
			await _managerUI.ShowProgressWindow(new LocalizedString("AccountSection.Saving"));
			if (!(await _database.ShouldLogOutOnThisDevice()))
			{
				await _database.SendAllRecords();
				ToggleAllButtons(enable: true);
			}
		}

		private void ShowSignUpWindow()
		{
			AuthorizationWindow.AdditionalData value = new AuthorizationWindow.AdditionalData
			{
				TypeActions = AuthorizationWindow.TypeActions.SignUp
			};
			_managerUI.ShowAuthorizationWindow(JsonConvert.SerializeObject(value));
			base.Settings.Close();
		}

		public void OnSteamLinkClicked()
		{
			_steamAccountService.AssignSteamId(_accountController.Email, UpdateSteamButtons).Forget();
		}

		public void OnSteamUnlinkClicked()
		{
			if (AccountSystemV2Extension.IsSteamOnlyAccount(_accountController.Email))
			{
				_managerUI.ShowConfirmWindow(ShowSignUpWindow, new LocalizedString("Steamauthorization.UnlinkSteamAccount"), new LocalizedString("Steamauthorization.SteamOnlyWayToLogIn"), () =>
				{
					base.Settings.Show();
				}).Forget();
				return;
			}
			float time = 10f;
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData();
			additionalData.ConfirmTimer = new ConfirmWindow.AdditionalData.ConfirmTimerData(isEnabled: true, time);
			_managerUI.ShowConfirmWindow(() =>
			{
				DoRevokeSteam().Forget();
			}, new LocalizedString("Steamauthorization.UnlinkSteamAccount"), new LocalizedString("Steamauthorization.AuthorizationWillNoLongerBeAvailable"), () =>
			{
				base.Settings.Show();
			}, "Confirm", null, additionalData).Forget();
		}

		private async UniTaskVoid DoRevokeSteam()
		{
			await _steamAccountService.RevokeSteamId(UpdateSteamButtons);
		}

		private void UpdateSteamButtons()
		{
			if (SteamManager.IsInitialized())
			{
				SignInMethodsGroup?.SetActive(value: true);
				bool isSteamLinked = ManagerKeysForPrefs.IsSteamLinked;
				SteamLinkButton?.SetActive(!isSteamLinked);
				SteamUnlinkButton?.SetActive(isSteamLinked);
			}
		}

		private async UniTaskVoid RefreshSteamLinkFromBackend()
		{
			if (_steamAccountService != null)
			{
				await _steamAccountService.RefreshSteamLinkAsync(_accountController.Email);
				if ((bool)this)
				{
					UpdateSteamButtons();
				}
			}
		}

		private void SteamShowNickNameCaret()
		{
			if (!_nickNameCaretRunning)
			{
				_nickNameCaretRunning = true;
				BlinkNickNameCaret().Forget();
			}
		}

		private async UniTaskVoid BlinkNickNameCaret()
		{
			_ = 1;
			try
			{
				await UniTask.NextFrame(this.GetCancellationTokenOnDestroy());
				bool visible = true;
				while ((bool)this && (bool)EditableNickName && EditableNickName.isFocused)
				{
					if ((bool)NameLegacyText)
					{
						NameLegacyText.text = (visible ? (EditableNickName.text + "|") : EditableNickName.text);
					}
					visible = !visible;
					await UniTask.Delay(TimeSpan.FromSeconds(0.5), DelayType.UnscaledDeltaTime, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy());
				}
			}
			catch (OperationCanceledException)
			{
			}
			finally
			{
				_nickNameCaretRunning = false;
				if ((bool)this && (bool)NameLegacyText && (bool)EditableNickName)
				{
					NameLegacyText.text = EditableNickName.text;
				}
			}
		}
	}
}
