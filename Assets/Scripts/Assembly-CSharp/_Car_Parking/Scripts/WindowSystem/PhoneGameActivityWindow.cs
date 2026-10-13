using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Other;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP_Registry;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class PhoneGameActivityWindow : WindowScript, IBackableWindow
	{
		public enum OpenSourceType
		{
			None = 0,
			Phone = 1,
			MainMenu = 2
		}

		public class AdditionalData
		{
			public OpenSourceType OpenSource;

			public bool FadeIn;
		}

		[SerializeField]
		private GameObject BGMenu;

		[SerializeField]
		private GameObject BGPhone;

		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private Button BackButton;

		[SerializeField]
		private PhoneGameActivityCanvasRoot PhoneActivityCanvasRoot;

		[SerializeField]
		private NavCanvas[] NavCanvases;

		[SerializeField]
		private TMP_Text Title;

		[Header("Fade in")]
		[Tooltip("Faded from zero up to their authored alpha when the window is opened with FadeIn.")]
		[SerializeField]
		private Fader[] FadeInFaders;

		[SerializeField]
		private float FadeDuration = 0.3f;

		[Tooltip("Plays back to the vertical phone before the phone window is reopened. Phone source only.")]
		[SerializeField]
		private PhoneHorizontalVerticalRotator PhoneHorizontalVerticalRotator;

		private OpenSourceType _openSource;

		private CanvasNavigator _navigator;

		protected override InputGroupsEnum WindowInputGroup => InputGroupsEnum.Ui;

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider)
		{
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			if (string.IsNullOrEmpty(data))
			{
				return;
			}
			AdditionalData additionalData = JsonConvert.DeserializeObject<AdditionalData>(data);
			if (additionalData != null)
			{
				_openSource = additionalData.OpenSource;
				if (additionalData.FadeIn)
				{
					FadeIn();
				}
			}
		}

		private void FadeIn()
		{
			if (FadeInFaders == null)
			{
				return;
			}
			Fader[] fadeInFaders = FadeInFaders;
			foreach (Fader fader in fadeInFaders)
			{
				if ((bool)fader)
				{
					fader.FadeInFromZero(FadeDuration);
				}
			}
		}

		protected override void Start()
		{
			base.Start();
			_navigator = new CanvasNavigator(NavCanvases, PhoneActivityCanvasRoot);
			_navigator.BackAvailabilityChanged += SetBackVisible;
			_navigator.TitleChanged += SetTitle;
			BGPhone.SetActive(_openSource == OpenSourceType.Phone);
			BGMenu.SetActive(!BGPhone.activeSelf);
			SetBackVisible(_navigator.CanBack);
			SetTitle(_navigator.CurrentTitle);
			InitBackButton();
		}

		public void InitBackButton()
		{
			_managerUI.OnBackButtonActivated?.Invoke(obj: true);
			_managerUI.SetCurrentWindow(this);
		}

		public void Back()
		{
			if (_navigator != null && _navigator.CanBack)
			{
				_navigator.Back();
				ReRegisterBackButton().Forget();
			}
			else
			{
				OnCloseClicked();
			}
		}

		private async UniTaskVoid ReRegisterBackButton()
		{
			await UniTask.Yield();
			if ((bool)this)
			{
				InitBackButton();
			}
		}

		protected override void OnDestroy()
		{
			base.OnDestroy();
			if (_navigator != null)
			{
				_navigator.BackAvailabilityChanged -= SetBackVisible;
				_navigator.TitleChanged -= SetTitle;
			}
		}

		protected override void OnEnable()
		{
			CloseButton.onClick.AddListener(OnCloseClicked);
			base.OnEnable();
			BackButton.SetActive(active: false);
		}

		private void OnDisable()
		{
			CloseButton.onClick.RemoveListener(OnCloseClicked);
		}

		private void SetBackVisible(bool canBack)
		{
		}

		private void SetTitle(string title)
		{
			if ((bool)Title)
			{
				Title.text = title;
			}
		}

		private void OnBackClicked()
		{
			_navigator.Back();
		}

		private void OnCloseClicked()
		{
			OnCloseClickedAsync().Forget();
		}

		private async UniTaskVoid OnCloseClickedAsync()
		{
			CloseButton.interactable = false;
			if (_openSource == OpenSourceType.Phone && (bool)PhoneHorizontalVerticalRotator)
			{
				await PhoneHorizontalVerticalRotator.RotateAsync(this.GetCancellationTokenOnDestroy());
			}
			switch (_openSource)
			{
			case OpenSourceType.Phone:
				await _managerUI.ShowPhoneWindowAsync(fadeIn: true, cleanScreen: false);
				break;
			case OpenSourceType.MainMenu:
				_managerUI.ShowMainMenuWindow();
				_managerUI.ShowUserBalanceWindow();
				break;
			default:
				if (ScenesHelper.IsMenuSceneActive())
				{
					_managerUI.ShowMainMenuWindow();
					_managerUI.ShowUserBalanceWindow();
				}
				else
				{
					_managerUI.ShowPhoneWindowAsync().Forget();
				}
				break;
			}
			Close();
		}
	}
}
