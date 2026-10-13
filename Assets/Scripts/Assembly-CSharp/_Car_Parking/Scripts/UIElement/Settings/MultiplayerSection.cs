using System.Collections.Generic;
using GME_voice_chat;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class MultiplayerSection : SettingsSectionBase
	{
		[SerializeField]
		private Toggle IgnoreFriendRequestToggle;

		[SerializeField]
		private Toggle IgnoreRacingRequestToggle;

		[SerializeField]
		private Toggle IgnoreCarSellingRequestToggle;

		[SerializeField]
		private Toggle HideChatToggle;

		[SerializeField]
		private Toggle HideVinylsToggle;

		[SerializeField]
		private Toggle HideNickToggle;

		[SerializeField]
		private Toggle VoiceChatToggle;

		[SerializeField]
		private Toggle ReduceNoisesToggle;

		[SerializeField]
		private ToggleSwitchHandler NoisesToggleInteractable;

		[SerializeField]
		private ToggleSwitchHandler VoiceChatToggleInteractable;

		[SerializeField]
		private ToggleSwitchHandler HideChatToggleInteractable;

		[SerializeField]
		private ToggleSwitchHandler HideVinylsToggleInteractable;

		[SerializeField]
		private CanvasGroup CurrentCanvasGroup;

		[SerializeField]
		private GameObject PrivacySection;

		private MultiPlayer _multiPlayer;

		private OptionalDependency<MultiPlayer> _multiPlayerDep;

		private ParkingMenuUI _parkingMenuUI;

		private OptionalDependency<ParkingMenuUI> _parkingMenuUIDep;

		private ParentalControlUtility _parentalControl;

		private Dictionary<Toggle, bool> _toggles = new Dictionary<Toggle, bool>();

		[Inject]
		public void Construct(ParentalControlUtility parentalControlUtility, OptionalDependency<MultiPlayer> multiPlayer, OptionalDependency<ParkingMenuUI> parkingMenuUI)
		{
			_parentalControl = parentalControlUtility;
			_multiPlayerDep = multiPlayer;
			_parkingMenuUIDep = parkingMenuUI;
		}

		protected override void Awake()
		{
			base.Awake();
			_multiPlayer = _multiPlayerDep?.Value;
			_parkingMenuUI = _parkingMenuUIDep?.Value;
		}

		private void OnEnable()
		{
			SaveStartValues();
		}

		private void OnDisable()
		{
			SendAnalytics();
		}

		private void Start()
		{
			Restore();
			ChangePrivacySection();
			if (_parentalControl.IsParentalControlEnabled)
			{
				UpdatePrivacySettings();
			}
			IgnoreRacingRequestToggle.onValueChanged.AddListener(OnIgnoreRacingTogglePressed);
			IgnoreFriendRequestToggle.onValueChanged.AddListener(OnIgnoreFriendshipTogglePressed);
			IgnoreCarSellingRequestToggle.onValueChanged.AddListener(OnIgnoreCarSellTogglePressed);
			HideChatToggle.onValueChanged.AddListener(OnHideAllChatTogglePressed);
			HideVinylsToggle.onValueChanged.AddListener(OnHideVinylsTogglePressed);
			HideNickToggle.onValueChanged.AddListener(HideNickNames);
			VoiceChatToggle.onValueChanged.AddListener(OnVoiceChatTogglePressed);
			ReduceNoisesToggle.onValueChanged.AddListener(ReduceNoises);
			SaveStartValues();
		}

		private void OnIgnoreRacingTogglePressed(bool isOn)
		{
			PlayerPrefs.SetInt("ignoreRacingRequest", isOn ? 1 : 0);
			if (FlagsStorage.IsMultiFree)
			{
				_multiPlayer.ignoreRacingRequest = isOn;
			}
		}

		private void OnIgnoreFriendshipTogglePressed(bool isOn)
		{
			PlayerPrefs.SetInt("ignoreFriendRequest", isOn ? 1 : 0);
			if (FlagsStorage.IsMultiFree)
			{
				_multiPlayer.ignoreFriendRequest = isOn;
			}
		}

		private void OnIgnoreCarSellTogglePressed(bool isOn)
		{
			MultiPlayerHelper.IgnoreCarSellRequest = isOn;
		}

		private void OnHideAllChatTogglePressed(bool isOn)
		{
			PlayerPrefs.SetInt("hideAllChat", isOn ? 1 : 0);
			if (FlagsStorage.IsMultiFree && (bool)_multiPlayer)
			{
				_multiPlayer.hideAllChat = isOn;
			}
			if (isOn && (bool)FreeDriveControl && (bool)FreeDriveControl.chatControl)
			{
				FreeDriveControl.chatControl.HideAllChat();
			}
		}

		private void OnHideVinylsTogglePressed(bool isOn)
		{
			PlayerPrefs.SetInt("hideAllVinyls", isOn ? 1 : 0);
			GlobalEventSystem.InvokeOnlineVinylsVisibilityChanged(!isOn);
		}

		private void HideNickNames(bool isOn)
		{
			PlayerPrefs.SetInt("hideAllTopLabel", isOn ? 1 : 0);
			FlagsStorage.HideAllTopLabel = isOn;
			if (FlagsStorage.IsMultiFree)
			{
				PlayerDatas.DisableEnableAll(!isOn, _multiPlayer.allLabels.ToArray());
			}
		}

		private void OnVoiceChatTogglePressed(bool isOn)
		{
			PlayerPrefs.SetInt("VoiceChat", isOn ? 1 : 0);
			if (!VoiceChatToggle || !VoiceChatToggle.gameObject || !VoiceChatToggle.gameObject.activeInHierarchy)
			{
				return;
			}
			if (FlagsStorage.IsMultiFree)
			{
				if ((bool)_multiPlayer)
				{
					_multiPlayer.ActivateDeactivateVoiceChat(isOn);
				}
				else
				{
					Debug.LogError("NO MULTIPLAYER: FIXED");
				}
				if ((bool)_parkingMenuUI)
				{
					_parkingMenuUI.ActivateVoiceChatButton(isOn);
				}
			}
			if ((bool)NoisesToggleInteractable)
			{
				NoisesToggleInteractable.SetInteractable(isOn);
			}
			else
			{
				Debug.LogError("NO NoisesToggleInteractable: FIXED");
			}
		}

		private void ReduceNoises(bool isOn)
		{
			PlayerPrefs.SetInt("lowerVolumeOnVoiceChat", isOn ? 1 : 0);
		}

		public override void Restore()
		{
			IgnoreRacingRequestToggle.isOn = PlayerPrefs.GetInt("ignoreRacingRequest", 0) == 1;
			IgnoreFriendRequestToggle.isOn = PlayerPrefs.GetInt("ignoreFriendRequest", 0) == 1;
			IgnoreCarSellingRequestToggle.isOn = MultiPlayerHelper.IgnoreCarSellRequest;
			bool flag = PlayerPrefs.GetInt("VoiceChat", GMEVoiceChatManager.VoiceChatDefault) == 1;
			VoiceChatToggle.isOn = flag;
			if ((bool)NoisesToggleInteractable)
			{
				NoisesToggleInteractable.SetInteractable(flag);
			}
			ReduceNoisesToggle.isOn = PlayerPrefs.GetInt("lowerVolumeOnVoiceChat", 1) == 1;
			HideChatToggle.isOn = PlayerPrefs.GetInt("hideAllChat", 0) == 1;
			HideVinylsToggle.isOn = PlayerPrefs.GetInt("hideAllVinyls", 0) == 1;
			HideNickToggle.isOn = PlayerPrefs.GetInt("hideAllTopLabel", 0) == 1;
		}

		public void UpdatePrivacySettings()
		{
			bool flag = PlayerPrefs.GetInt("privacyAgeConfirmed", 0) == 1;
			if (!flag)
			{
				if ((bool)HideChatToggle)
				{
					HideChatToggle.isOn = true;
				}
				if ((bool)HideChatToggleInteractable)
				{
					HideChatToggleInteractable.SetInteractable(isOn: false);
				}
				if ((bool)HideVinylsToggle)
				{
					HideVinylsToggle.isOn = true;
				}
				if ((bool)HideVinylsToggleInteractable)
				{
					HideVinylsToggleInteractable.SetInteractable(isOn: false);
				}
				if ((bool)VoiceChatToggle)
				{
					VoiceChatToggle.isOn = false;
				}
				if ((bool)VoiceChatToggleInteractable)
				{
					VoiceChatToggleInteractable.SetInteractable(isOn: false);
				}
				if ((bool)ReduceNoisesToggle)
				{
					ReduceNoisesToggle.isOn = false;
				}
				if ((bool)NoisesToggleInteractable)
				{
					NoisesToggleInteractable.SetInteractable(isOn: false);
				}
			}
			else
			{
				if ((bool)HideChatToggleInteractable)
				{
					HideChatToggleInteractable.SetInteractable(isOn: true);
				}
				if ((bool)HideVinylsToggleInteractable)
				{
					HideVinylsToggleInteractable.SetInteractable(isOn: true);
				}
				if ((bool)VoiceChatToggleInteractable)
				{
					VoiceChatToggleInteractable.SetInteractable(isOn: true);
				}
				if ((bool)NoisesToggleInteractable)
				{
					NoisesToggleInteractable.SetInteractable(isOn: true);
				}
			}
			bool isOn = PlayerPrefs.GetInt("hideAllChat", 0) == 1 || !flag;
			bool isOn2 = PlayerPrefs.GetInt("hideAllVinyls", 0) == 1 || !flag;
			OnHideAllChatTogglePressed(isOn);
			OnHideVinylsTogglePressed(isOn2);
			bool flag2 = PlayerPrefs.GetInt("VoiceChat", GMEVoiceChatManager.VoiceChatDefault) == 1;
			OnVoiceChatTogglePressed(flag2 & flag);
			if (flag)
			{
				bool isOn3 = PlayerPrefs.GetInt("lowerVolumeOnVoiceChat", 1) == 1;
				ReduceNoises(isOn3);
			}
		}

		private void ChangePrivacySection()
		{
			if (_parentalControl.IsSocialBlocked)
			{
				PrivacySection.SetActive(value: false);
			}
			else if (_parentalControl.IsVinylsEnabled)
			{
				PrivacySection.SetActive(value: true);
				HideChatToggle.gameObject.SetActive(value: false);
				HideVinylsToggle.gameObject.SetActive(value: true);
				VoiceChatToggle.gameObject.SetActive(value: false);
				ReduceNoisesToggle.gameObject.SetActive(value: false);
			}
		}

		private void SaveStartValues()
		{
			_toggles[IgnoreFriendRequestToggle] = IgnoreFriendRequestToggle.isOn;
			_toggles[IgnoreRacingRequestToggle] = IgnoreRacingRequestToggle.isOn;
			_toggles[IgnoreCarSellingRequestToggle] = IgnoreCarSellingRequestToggle.isOn;
			_toggles[HideChatToggle] = HideChatToggle.isOn;
			_toggles[HideVinylsToggle] = HideVinylsToggle.isOn;
			_toggles[HideNickToggle] = HideNickToggle.isOn;
			_toggles[VoiceChatToggle] = VoiceChatToggle.isOn;
			_toggles[ReduceNoisesToggle] = ReduceNoisesToggle.isOn;
		}

		private void SendAnalytics()
		{
			LogAnalytics(UserPreferenceType.ignore_friend_request, IgnoreFriendRequestToggle);
			LogAnalytics(UserPreferenceType.ignore_racing_request, IgnoreRacingRequestToggle);
			LogAnalytics(UserPreferenceType.ignore_car_selling, IgnoreCarSellingRequestToggle);
			LogAnalytics(UserPreferenceType.hide_chat, HideChatToggle);
			LogAnalytics(UserPreferenceType.hide_vinyls, HideVinylsToggle);
			LogAnalytics(UserPreferenceType.hide_nick, HideNickToggle);
			LogAnalytics(UserPreferenceType.voice_chat, VoiceChatToggle);
			LogAnalytics(UserPreferenceType.reduce_noises, ReduceNoisesToggle);
		}

		private void LogAnalytics(UserPreferenceType type, Toggle toggle)
		{
			if (_toggles.TryGetValue(toggle, out var value))
			{
				bool isOn = toggle.isOn;
				if (isOn != value)
				{
					base.UserPreferenceAnalytic.LogUserPreference(type, value.ToString(), isOn.ToString());
				}
			}
		}
	}
}
