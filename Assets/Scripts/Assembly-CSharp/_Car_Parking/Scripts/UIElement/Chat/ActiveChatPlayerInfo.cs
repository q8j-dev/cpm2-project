using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class ActiveChatPlayerInfo : MonoBehaviour
	{
		[SerializeField]
		private Image RankImage;

		[SerializeField]
		private TextMeshProUGUI PlayerName;

		[SerializeField]
		private Button OpenProfileButton;

		[SerializeField]
		private Toggle OptionsToggle;

		[SerializeField]
		private Button OpenProfileSmallButton;

		[SerializeField]
		private Toggle MuteToggle;

		[SerializeField]
		private ToggleSwitchHandler MuteToggleSwitcher;

		[SerializeField]
		private GameObject OptionsPanel;

		private string _playerName;

		private string _playerId;

		private MultiPersonControl _activePerson;

		private PersonalChatView _activePersonalChat;

		private ChatControl _chatControl;

		private ChatWindow _chatWindow;

		private ManagerUI _managerUI;

		[Inject]
		public void Construct(ChatControl chatControl, ManagerUI managerUI)
		{
			_managerUI = managerUI;
			_chatControl = chatControl;
		}

		private void Start()
		{
			OpenProfileButton.onClick.AddListener(OpenProfile);
			OpenProfileSmallButton.onClick.AddListener(OpenProfile);
			OptionsToggle.onValueChanged.AddListener(ShowOptions);
			MuteToggle.onValueChanged.AddListener(TryMutePlayer);
		}

		public void SetInfo(MultiPersonControl person, PersonalChatView personalChatView, ChatWindow chatWindow)
		{
			_activePersonalChat = personalChatView;
			_chatWindow = chatWindow;
			_activePerson = person;
			RankImage.sprite = UtilsRank.LoadRankSpriteFromResources(personalChatView.RankType);
			string text = ChatHelper.RemoveRichText(_activePersonalChat.PlayerName);
			TMP_FontAsset_Placeholder.EnsureFor(text);
			PlayerName.text = text;
			PlayerName.color = PlayerNameColoredMessageView.DefaultNameColor;
			if (_chatWindow.IsFriend(personalChatView.PlayerId))
			{
				PlayerName.color = PlayerNameColoredMessageView.FriendNameColor;
			}
			MuteToggle.SetIsOnWithoutNotify(_activePersonalChat.IsMuted);
			MuteToggleSwitcher.Switch(_activePersonalChat.IsMuted);
			_playerId = _activePersonalChat.PlayerId;
			_playerName = _activePersonalChat.PlayerName;
		}

		private void OpenProfile()
		{
			_chatWindow.OnNickClicked(_playerId, _playerName);
			OptionsToggle.isOn = false;
		}

		private void TryMutePlayer(bool isMuted)
		{
			if (!_activePerson)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("PlayersListControl.Playeralreadylefttheroom"), 2f, Color.red);
			}
			else if (isMuted)
			{
				_managerUI.ShowSimpleConfirmWindow(new LocalizedString("Chat.MutePlayer"), new LocalizedString("PlayerProfileWindow.PlayerMuteDescription"), MutePlayer, SetOffMuteToggle, new LocalizedString("PlayerProfile.Mute")).Forget();
			}
			else
			{
				SetMuteState(isMuted: false);
			}
		}

		private void MutePlayer()
		{
			SetMuteState(isMuted: true);
		}

		private void SetMuteState(bool isMuted)
		{
			_activePersonalChat.Mute(isMuted);
			_activePerson.isMuted = isMuted;
			_chatControl.OnPlayerMuted?.Invoke(_activePerson.ThisPlayerID, isMuted);
		}

		private void SetOffMuteToggle()
		{
			MuteToggle.SetIsOnWithoutNotify(value: false);
			MuteToggleSwitcher.Switch(isOn: false);
		}

		private void ShowOptions(bool show)
		{
			OptionsPanel.SetActive(show);
		}
	}
}
