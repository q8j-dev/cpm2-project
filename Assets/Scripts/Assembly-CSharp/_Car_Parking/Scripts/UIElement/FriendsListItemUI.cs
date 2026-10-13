using System;
using CodeStage.AntiCheat.Storage;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.UIElement
{
	public class FriendsListItemUI : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI Text;

		[SerializeField]
		private TMP_InputField PasswordInputLabel;

		[SerializeField]
		private Button JoinButton;

		[SerializeField]
		private Button DeleteButton;

		[SerializeField]
		private Image OnlineIndicator;

		[SerializeField]
		private Image JoinIcon;

		private string _friendIDInRoom;

		private string _accountID;

		private string _name;

		private int _currentPlayerInRoom;

		private bool _friendInRoom;

		private bool _showingOfflineMes;

		private int _maxPlayerInRoom;

		private ArenaId _arenaId;

		private PlayerDatas.FriendsUtility _friendsUtility;

		private MultiplayerLobbyHandler _lobbyHandler;

		public string RoomNameToJoin { get; private set; }

		public int PassWord { get; set; }

		[Inject]
		public void Construct(PlayerDatas.FriendsUtility friendsUtility)
		{
			_friendsUtility = friendsUtility;
		}

		private void Start()
		{
			JoinButton.onClick.AddListener(JoinRoomToThisFriend);
			DeleteButton.onClick.AddListener(DeleteFromFriend);
			ShowMainWindow();
		}

		private void OnDestroy()
		{
			JoinButton.onClick.RemoveAllListeners();
			DeleteButton.onClick.RemoveAllListeners();
		}

		private void JoinRoomToThisFriend()
		{
			if (!_showingOfflineMes)
			{
				if (PassWord == 0 || PassWord == -1)
				{
					FlagsStorage.IsPrivateRoom = true;
					ProceedToFreeDrive();
				}
				else
				{
					_lobbyHandler.ShowPasswordInput = true;
					ShowPasswordWindow();
				}
			}
		}

		private void ProceedToFreeDrive()
		{
			if (!string.IsNullOrEmpty(RoomNameToJoin) && ObscuredPrefs.HasKey(RoomNameToJoin))
			{
				int num = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				int num2 = ObscuredPrefs.GetInt(RoomNameToJoin, num);
				if (num - num2 < 7200)
				{
					SetText(new LocalizedString("FriendsListItemUI.YouCanTEnterThisRoom"));
					_showingOfflineMes = true;
					Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
					{
						ResetText();
					}).AddTo(this);
					return;
				}
				ObscuredPrefs.DeleteKey(RoomNameToJoin);
			}
			if (!string.IsNullOrEmpty(RoomNameToJoin))
			{
				_lobbyHandler.TryToJoinRoom(this);
			}
			else if (!_friendInRoom)
			{
				SetText(new LocalizedString("FriendsListItemUI.PlayerOffline"));
				_showingOfflineMes = true;
				Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
				{
					ResetText();
				}).AddTo(this);
			}
			else if (_currentPlayerInRoom >= _maxPlayerInRoom)
			{
				SetText(new LocalizedString("MultiplayerLobbyHandler.RoomIsFull"));
				_showingOfflineMes = true;
				Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
				{
					ResetText();
				}).AddTo(this);
			}
			else
			{
				_lobbyHandler.TryToJoinRoom(this);
			}
		}

		private void DeleteFromFriend()
		{
			_lobbyHandler.ShowPasswordInput = false;
			_friendsUtility.RemoveFriendWithLocalID(_friendIDInRoom);
			_lobbyHandler.UpdateListFriendUI();
		}

		private void ResetText()
		{
			string text = (string.IsNullOrEmpty(_name) ? _accountID : _name);
			if (_arenaId != ArenaId.Free)
			{
				text += $"\n<size=70%>{_arenaId}</size>";
			}
			SetText(text);
			_showingOfflineMes = false;
		}

		public void SetArenaInfo(ArenaId arenaId)
		{
			_arenaId = arenaId;
			ResetText();
		}

		public void OnPasswordSubmit()
		{
			int result = 0;
			if (!int.TryParse(PasswordInputLabel.text, out result))
			{
				return;
			}
			if (result == PassWord)
			{
				FlagsStorage.IsPrivateRoom = true;
				ProceedToFreeDrive();
				return;
			}
			SetText(new LocalizedString("FriendsListItemUI.IncorrectPassword"));
			_showingOfflineMes = true;
			ShowMainWindow();
			Observable.Timer(TimeSpan.FromSeconds(2.0)).Subscribe((long _) =>
			{
				ResetText();
			}).AddTo(this);
			_lobbyHandler.ShowPasswordInput = false;
		}

		private void ShowPasswordWindow()
		{
			PasswordInputLabel.gameObject.SetActive(value: true);
			JoinButton.onClick.RemoveAllListeners();
			JoinButton.onClick.AddListener(OnPasswordSubmit);
		}

		private void ShowMainWindow()
		{
			PasswordInputLabel.gameObject.SetActive(value: false);
		}

		private void SetInRoom()
		{
			OnlineIndicator.color = new Color(0.64f, 0.73f, 0.32f);
			JoinIcon.gameObject.SetActive(value: true);
			JoinButton.interactable = true;
		}

		public void SetText(string text)
		{
			Text.text = text;
		}

		public void Init(string friendId, string friendName, MultiplayerLobbyHandler lobbyHandler)
		{
			_accountID = friendId;
			_friendIDInRoom = friendId;
			_name = friendName;
			_lobbyHandler = lobbyHandler;
			ResetText();
		}

		public void SetRoomInfo(string roomName, int maxPlayer)
		{
			RoomNameToJoin = roomName;
			_maxPlayerInRoom = maxPlayer;
			_friendInRoom = true;
			_currentPlayerInRoom = 1;
			ResetText();
			SetInRoom();
		}

		public void SetOnline()
		{
			OnlineIndicator.color = new Color(0.21f, 0.43f, 0.58f);
		}
	}
}
