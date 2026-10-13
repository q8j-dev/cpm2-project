using System;
using System.Threading;
using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public class ChatWorldMessagesCreator : MonoBehaviour
	{
		private const int MaxMessageCharCount = 50;

		[SerializeField]
		private ChatMessagesConfig chatMessagesConfig;

		private MultiPersonControl _multiPersonControl;

		private DriverActionController _driverActionController;

		private Transform _pointToSpawn;

		private Sequence _fadeAndDestroyAnimation;

		private GameObject _messageInstance;

		private CanvasGroup _messageInstanceCanvasGroup;

		private bool _wasInitialized;

		private PlayerDataStorage _playerDataStorage;

		private FreeDriveProvider _freeDriveProvider;

		[Inject]
		public void Construct(PlayerDataStorage playerDataStorage, FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
			_playerDataStorage = playerDataStorage;
		}

		private void Awake()
		{
			TryGetComponent<MultiPersonControl>(out _multiPersonControl);
			TryGetComponent<DriverActionController>(out _driverActionController);
		}

		private void Start()
		{
			TryToInitLoop(this.GetCancellationTokenOnDestroy()).Forget();
		}

		private async UniTaskVoid TryToInitLoop(CancellationToken ct)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(3.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			while (!ct.IsCancellationRequested)
			{
				if ((bool)_freeDriveProvider.FreeDriveControl && (bool)_freeDriveProvider.FreeDriveControl.chatControl)
				{
					Init();
					break;
				}
				await UniTask.Delay(TimeSpan.FromSeconds(2.0), ignoreTimeScale: false, PlayerLoopTiming.Update, ct);
			}
		}

		private void Init()
		{
			_wasInitialized = true;
			ChatControl chatControl = _freeDriveProvider.FreeDriveControl.chatControl;
			chatControl.GlobalMessageEvent = (Action<ChatMessage>)Delegate.Combine(chatControl.GlobalMessageEvent, new Action<ChatMessage>(WorldMessageHandle));
		}

		private void OnDestroy()
		{
			if (_wasInitialized && FlagsStorage.IsFreeDriveMode && (bool)_freeDriveProvider.FreeDriveControl && (bool)_freeDriveProvider.FreeDriveControl.chatControl)
			{
				ChatControl chatControl = _freeDriveProvider.FreeDriveControl.chatControl;
				chatControl.GlobalMessageEvent = (Action<ChatMessage>)Delegate.Remove(chatControl.GlobalMessageEvent, new Action<ChatMessage>(WorldMessageHandle));
			}
		}

		private void WorldMessageHandle(ChatMessage chatMessage)
		{
			if (PlayerPrefs.GetInt("hideAllChat", 0) != 1 && !FlagsStorage.IsPhotoMode && CheckConditions(chatMessage))
			{
				TryCreateMessage(chatMessage);
			}
		}

		private bool CheckConditions(ChatMessage chatMessage)
		{
			if (chatMessage == null)
			{
				return false;
			}
			bool flag = chatMessage.MessageType != ChatMessageType.ConnectionState;
			bool flag2 = chatMessage.MessageType == ChatMessageType.Text;
			bool flag3 = chatMessage.FromPlayerId == _multiPersonControl.ThisPlayerID;
			if (_playerDataStorage == null)
			{
				Debug.LogError("PlayerDataStorage is null." + base.gameObject);
				return false;
			}
			ObscuredString playerID = _playerDataStorage.PlayerID;
			bool flag4 = !((ObscuredString)_multiPersonControl.ThisPlayerID == playerID) || chatMessagesConfig.showMeMyMessages;
			bool num = chatMessage.ChatType == ChatType.FullChat && chatMessagesConfig.showGlobalMessages;
			bool flag5 = chatMessage.ChatType == ChatType.FriendsChat && chatMessagesConfig.showFriendsMessages;
			bool flag6 = chatMessage.ChatType == ChatType.Personal && chatMessagesConfig.showPersonalMessages;
			bool flag7 = num | flag5 | flag6;
			return flag & flag3 & flag7 & flag4 & flag2;
		}

		private void TryCreateMessage(ChatMessage chatMessage)
		{
			if (_messageInstance != null)
			{
				_fadeAndDestroyAnimation.Kill();
				_fadeAndDestroyAnimation = DOTween.Sequence();
				_fadeAndDestroyAnimation.Append(_messageInstanceCanvasGroup.DOFade(1f, chatMessagesConfig.worldMessageFadeTime));
				_fadeAndDestroyAnimation.OnComplete(() =>
				{
					DestroyMessage(_messageInstance);
					ShowMessage(chatMessage, chatMessagesConfig.timeForMessage, GetPersonWorldStatus());
				});
			}
			else
			{
				ShowMessage(chatMessage, chatMessagesConfig.timeForMessage, GetPersonWorldStatus());
			}
		}

		private void ShowMessage(ChatMessage message, float timeForMessage, PersonStateInWorld personStateInWorld)
		{
			if (_pointToSpawn == null)
			{
				TryFindPoint();
			}
			if (!(_pointToSpawn == null) && base.gameObject.activeInHierarchy)
			{
				_messageInstance = UnityEngine.Object.Instantiate(chatMessagesConfig.messagePrefab, base.transform, worldPositionStays: true);
				Text componentInChildren = _messageInstance.GetComponentInChildren<Text>();
				string text = message.EnteredText;
				if (text.Length > 50)
				{
					text = text.Substring(0, 50);
				}
				componentInChildren.text = text;
				SetMessageGraphicSettings(message, componentInChildren, _messageInstance);
				_messageInstance.transform.position = CalculatePosition(personStateInWorld);
				_messageInstance.GetComponentInChildren<LayoutGroup>().ChangeUpdateAsync(this.GetCancellationTokenOnDestroy()).Forget();
				SetFadeAnimationToMessage(_messageInstance, timeForMessage, chatMessagesConfig.worldMessageFadeTime);
			}
		}

		private void SetMessageGraphicSettings(ChatMessage message, Text unityText, GameObject messageInstance)
		{
			Image[] componentsInChildren = messageInstance.GetComponentsInChildren<Image>(includeInactive: true);
			Image image = componentsInChildren[0];
			Image image2 = componentsInChildren[1];
			ChatWorldMessage component = messageInstance.GetComponent<ChatWorldMessage>();
			component.openChatOnClick = chatMessagesConfig.openChatOnClickMessage;
			component.playerID = _playerDataStorage.PlayerID;
			component.ChatControl = _freeDriveProvider.FreeDriveControl.chatControl;
			if (message.EnteredText.Length < chatMessagesConfig.maxNextLineIndex)
			{
				component.messageButton.GetComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
			}
			switch (message.ChatType)
			{
			case ChatType.FullChat:
				image2.color = chatMessagesConfig.globalMessageColor;
				image.sprite = chatMessagesConfig.globalChatSprite;
				component.chatTypeString = "all";
				break;
			case ChatType.FriendsChat:
				image2.color = chatMessagesConfig.friendsMessageColor;
				image.sprite = chatMessagesConfig.friendsChatSprite;
				component.chatTypeString = "friends";
				break;
			case ChatType.Personal:
				unityText.fontStyle = FontStyle.BoldAndItalic;
				image2.color = chatMessagesConfig.personalMessageColor;
				image.sprite = chatMessagesConfig.personalChatSprite;
				component.chatTypeString = message.FromPlayerId;
				component.playerName = message.FromPlayerName;
				break;
			}
			if (image.sprite == null)
			{
				image.color = new Color(0f, 0f, 0f, 0f);
			}
		}

		private Vector3 CalculatePosition(PersonStateInWorld personStateInWorld)
		{
			Vector3 position = _pointToSpawn.transform.position;
			Vector3 result;
			switch (personStateInWorld)
			{
			case PersonStateInWorld.InCar:
			{
				_messageInstance.transform.position = position;
				int carId = GetComponent<DriverActionController>().Car.GetComponent<VehicleData>().CarId;
				float num = (chatMessagesConfig.highCarIds.Contains(carId) ? chatMessagesConfig.highCarWorldMessageHeight : chatMessagesConfig.classicCarWorldMessageHeight);
				result = new Vector3(position.x, position.y + num, position.z);
				break;
			}
			case PersonStateInWorld.Passenger:
				result = new Vector3(position.x, position.y + chatMessagesConfig.classicCarWorldMessageHeight, position.z);
				break;
			case PersonStateInWorld.OutOfCar:
			{
				Vector3 position2 = base.transform.position;
				result = new Vector3(position2.x, position2.y + chatMessagesConfig.personCarWorldMessageHeight, position2.z);
				break;
			}
			default:
				return position;
			}
			return result;
		}

		private void SetFadeAnimationToMessage(GameObject messageInstance, float timeForMessage, float fadeTime)
		{
			_messageInstanceCanvasGroup = messageInstance.GetComponentInChildren<CanvasGroup>();
			_messageInstanceCanvasGroup.alpha = 0f;
			_fadeAndDestroyAnimation = DOTween.Sequence().SetLink(messageInstance, LinkBehaviour.KillOnDestroy);
			_fadeAndDestroyAnimation.Append(_messageInstanceCanvasGroup.DOFade(1f, fadeTime));
			_fadeAndDestroyAnimation.AppendInterval(timeForMessage - 2f * fadeTime);
			_fadeAndDestroyAnimation.Append(_messageInstanceCanvasGroup.DOFade(0f, fadeTime));
			_fadeAndDestroyAnimation.OnComplete(() =>
			{
				DestroyMessage(messageInstance);
			});
		}

		private void DestroyMessage(GameObject messageInstance)
		{
			UnityEngine.Object.Destroy(messageInstance);
		}

		private void TryFindPoint()
		{
			if (TryGetComponent<MainPerson>(out var component) && component.ik != null)
			{
				_pointToSpawn = GetComponent<MainPerson>().ik.references.head.gameObject.transform;
			}
		}

		private PersonStateInWorld GetPersonWorldStatus()
		{
			if (_driverActionController.Car != null)
			{
				return PersonStateInWorld.InCar;
			}
			if (_driverActionController.isPassenger)
			{
				return PersonStateInWorld.Passenger;
			}
			return PersonStateInWorld.OutOfCar;
		}
	}
}
