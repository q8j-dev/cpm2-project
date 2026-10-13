using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Common;
using CPM.PlayerData;
using CPM.Scripts.Plugins.Multiplayer.Player;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SpamFilter;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public class ChatControl : MonoBehaviour
	{
		public const string White = "#ffffff";

		public const string Green = "#6aa84f";

		public const string Red = "#ff0000";

		public const string FullChatButtonName = "all";

		public const string FriendsChatButtonName = "friends";

		public const int MaxLinesCount = 100;

		public const int MaxMessageLength = 100;

		public Action<ChatMessage> GlobalMessageEvent;

		public Action<string, bool> OnPlayerMuted;

		public Action<MultiPersonControl> OnPlayerBlocked;

		public Action<RectTransform, bool> OnChatOpened;

		public Action OnOpenChatWithProfileWindow;

		public Action OnProfileWindowClosedFromChat;

		public Action OnProfileWindowCloseClicked;

		public ChatMessageType MessageType;

		public string ActivePersonalChatId;

		private List<MultiPersonControl> _onlinePlayersMpcs = new List<MultiPersonControl>();

		private PlayerDatas.FriendsUtility _friendsUtility;

		private PlayerDataStorage _playerDataStorage;

		private IChatFilterUtility _chatFilterUtility;

		private ISpamFilter _spamFilter;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private FreeDriveProvider _freeDriveProvider;

		private ManagerUI _managerUI;

		private MultiPlayer _multiPlayer;

		public ChatWindow ChatWindow { get; set; }

		private ChatType ActiveChatType { get; set; }

		private string GetMyPlayerName
		{
			get
			{
				string text = _playerDataStorage.PlayerName;
				if (text == "")
				{
					text = "No Nick";
				}
				return ConvertName(text);
			}
		}

		public event Action<string> OnPersonalChatOpen;

		[Inject]
		public void Construct(PlayerDataStorage playerDataStorage, IChatFilterUtility chatFilterUtility, OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveProvider freeDriveProvider, ManagerUI managerUI, MultiPlayer multiPlayer)
		{
			_multiPlayer = multiPlayer;
			_managerUI = managerUI;
			_freeDriveProvider = freeDriveProvider;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_playerDataStorage = playerDataStorage;
			_chatFilterUtility = chatFilterUtility;
		}

		private string ConvertName(string playerName)
		{
			playerName = ChatHelper.RemoveRichText(playerName);
			return playerName;
		}

		public string GetNewRandomOfferID()
		{
			return Guid.NewGuid().ToString();
		}

		private void Start()
		{
			Init();
		}

		[Inject]
		public void Initialize(PlayerDatas.FriendsUtility friendsUtility)
		{
			_friendsUtility = friendsUtility;
		}

		private void Init()
		{
			ActiveChatType = ChatType.FullChat;
			_freeDriveProvider.MultiPlayer.OnPlayerConnectedWithDataEvent += OnPlayerConnectedWithDataEventHandler;
			_freeDriveProvider.MultiPlayer.OnPlayerDisconnectedWithDataEvent += OnPlayerDisconnectedWithDataEventHandler;
			UpdatePlayersMpcs();
			LoadSpamFilterSettings();
		}

		private void LoadSpamFilterSettings()
		{
			_spamFilter = new LevenshteinSpamFilter(new IncrementalSpamPunishmentStrategy());
		}

		private void UpdatePlayersMpcs()
		{
			if (_onlineCollectionsProvider.PlayersOnlineNow != null)
			{
				_onlinePlayersMpcs = (from x in _onlineCollectionsProvider.PlayersOnlineNow.ToList()
					where x != null
					select x).ToList();
			}
		}

		private void OnDestroy()
		{
			if ((bool)_freeDriveProvider.MultiPlayer)
			{
				_freeDriveProvider.MultiPlayer.OnPlayerConnectedWithDataEvent -= OnPlayerConnectedWithDataEventHandler;
				_freeDriveProvider.MultiPlayer.OnPlayerDisconnectedWithDataEvent -= OnPlayerDisconnectedWithDataEventHandler;
			}
		}

		public void CreateChatWindow()
		{
			if (!ChatWindow)
			{
				_managerUI.ShowChatWindow().Forget();
			}
		}

		public void ShowChatFull()
		{
			UpdatePlayersMpcs();
			if (FlagsStorage.IsChatAllowed && (bool)ChatWindow)
			{
				ChatWindow.ShowWindow(isShow: true);
				ChatWindow.MaximizeChat();
				ChatWindow.InitBackButton();
			}
		}

		public void HideAllChat()
		{
			if ((bool)ChatWindow)
			{
				ChatWindow.ShowWindow(isShow: false);
			}
		}

		public void ShowPartiallyChat()
		{
			if (FlagsStorage.IsChatAllowed && (bool)ChatWindow)
			{
				if (!_multiPlayer.hideAllChat)
				{
					ChatWindow.ShowMinimizedChat(isShow: true);
				}
				ChatWindow.MinimizeChat(immediate: true);
			}
		}

		public void OnChatEntered(string text, ChatType chatType)
		{
			ActiveChatType = chatType;
			string censuredEnteredText = _chatFilterUtility.Filter(text);
			censuredEnteredText = ChatHelper.RemoveRichText(censuredEnteredText);
			censuredEnteredText = ChatMessage.RemoveSeparators(censuredEnteredText);
			_spamFilter.RegisterMessage(censuredEnteredText);
			bool shouldSend = !_spamFilter.Spamming && !_spamFilter.IsMessageBanned(censuredEnteredText);
			string text2 = CreateMessageDependsOfChatType(censuredEnteredText, shouldSend);
			if (!(text2 == ""))
			{
				SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ChatSendingSwitch, base.gameObject);
				OnChatMessageReceived(text2);
			}
		}

		private string CreateMessageDependsOfChatType(string censuredEnteredString, bool shouldSend = true)
		{
			string text;
			switch (ActiveChatType)
			{
			case ChatType.FullChat:
			{
				string additionData2 = ChatMessage.AdditionalDataForFullAndFriendChat(_playerDataStorage.PlayerID, GetMyPlayerName);
				text = new ChatMessage(ChatType.FullChat, MessageType, censuredEnteredString, additionData2).ToString();
				if (shouldSend)
				{
					_multiPlayer.SendValueStr<RpcChatMessageReceived>(text, GetMyPlayerName);
				}
				break;
			}
			case ChatType.FriendsChat:
			{
				string additionData = ChatMessage.AdditionalDataForFullAndFriendChat(_playerDataStorage.PlayerID, GetMyPlayerName);
				text = new ChatMessage(ChatType.FriendsChat, MessageType, censuredEnteredString, additionData).ToString();
				if (shouldSend)
				{
					_multiPlayer.SendValueStr<RpcChatMessageReceived>(text, GetMyPlayerName);
				}
				break;
			}
			case ChatType.Personal:
				text = SendPersonalMessage(censuredEnteredString, ActivePersonalChatId, shouldSend);
				break;
			default:
				text = "";
				break;
			}
			return text;
		}

		public string SendPersonalMessage(string censuredEnteredString, string toPlayerId, bool shouldSend = true)
		{
			string additionData = ChatMessage.AdditionalDataForPersonalChat(_playerDataStorage.PlayerID, GetMyPlayerName, toPlayerId, "");
			string text = new ChatMessage(ChatType.Personal, MessageType, censuredEnteredString, additionData).ToString();
			if (shouldSend)
			{
				_multiPlayer.SendValueStrToPlayer<RpcChatMessageReceived>(text, GetMyPlayerName, toPlayerId);
			}
			return text;
		}

		public void OnChatMessageReceived(string input)
		{
			ChatMessage chatMessage = new ChatMessage(input);
			UpdateChatText(chatMessage);
		}

		private void UpdateChatText(ChatMessage chatMessage)
		{
			ActiveChatType = chatMessage.ChatType;
			if (TryParseMessage(chatMessage))
			{
				GlobalMessageEvent?.Invoke(chatMessage);
			}
		}

		private bool TryParseMessage(ChatMessage chatMessage)
		{
			if (IsPlayerBlocked(chatMessage.FromPlayerId))
			{
				return false;
			}
			if (chatMessage.ChatType == ChatType.FriendsChat)
			{
				if ((ObscuredString)chatMessage.FromPlayerId != _playerDataStorage.PlayerID && !_friendsUtility.HasFriendWithID(chatMessage.FromPlayerId))
				{
					return false;
				}
				return true;
			}
			return true;
		}

		private bool IsPlayerBlocked(string playerId)
		{
			MultiPersonControl multiPersonControl = _onlinePlayersMpcs.Find((MultiPersonControl x) => x.ThisPlayerID == playerId);
			if ((bool)multiPersonControl)
			{
				return multiPersonControl.IsBlocked;
			}
			return false;
		}

		public bool IsPlayerMuted(string playerId)
		{
			MultiPersonControl multiPersonControl = _onlinePlayersMpcs.Find((MultiPersonControl x) => x.ThisPlayerID == playerId);
			if ((bool)multiPersonControl)
			{
				return multiPersonControl.isMuted;
			}
			return false;
		}

		public void SetActiveChatType(string chatTypeString, string playerName = "")
		{
			if (!(chatTypeString == "all"))
			{
				if (chatTypeString == "friends")
				{
					ActiveChatType = ChatType.FriendsChat;
					return;
				}
				ActiveChatType = ChatType.Personal;
				ActivePersonalChatId = chatTypeString;
			}
			else
			{
				ActiveChatType = ChatType.FullChat;
			}
		}

		public void OpenPersonalChat(string playerId, string playerName)
		{
			ActiveChatType = ChatType.Personal;
			OnPersonalChatOpen?.Invoke(playerId);
		}

		private void OnPlayerConnectedWithDataEventHandler(PlayerMainData playerMainData)
		{
			UpdatePlayersMpcs();
			if (!string.IsNullOrEmpty(playerMainData.PlayerId))
			{
				string additionData = ChatMessage.AdditionalDataForPersonalChat(playerMainData.PlayerId, "", _playerDataStorage.PlayerID, GetMyPlayerName);
				ChatMessage chatMessage = new ChatMessage(ChatType.Personal, ChatMessageType.ConnectionState, "1", additionData);
				OnChatMessageReceived(chatMessage.ToString());
			}
		}

		private void OnPlayerDisconnectedWithDataEventHandler(PlayerMainData playerMainData)
		{
			UpdatePlayersMpcs();
			if (!string.IsNullOrEmpty(playerMainData.PlayerId))
			{
				string additionData = ChatMessage.AdditionalDataForPersonalChat(playerMainData.PlayerId, "", _playerDataStorage.PlayerID, GetMyPlayerName);
				ChatMessage chatMessage = new ChatMessage(ChatType.Personal, ChatMessageType.ConnectionState, "0", additionData);
				OnChatMessageReceived(chatMessage.ToString());
			}
		}

		public void OnClickLink(string playerIdInLink, string nickName)
		{
			if (nickName != null && nickName.EndsWith("[-]"))
			{
				nickName = nickName.Substring(0, nickName.Length - 3);
			}
			if ((playerIdInLink != null || nickName != null) && (!(playerIdInLink == "") || !(nickName == "")))
			{
				OpenPlayerProfile(playerIdInLink);
			}
		}

		private void OpenPlayerProfile(string playerIdInLink)
		{
			_freeDriveProvider.FreeDriveControl.ShowProfile(playerIdInLink).Forget();
		}
	}
}
