using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class PersonalChatPool : MonoBehaviour
	{
		[SerializeField]
		private ChatWindow ChatWindow;

		[SerializeField]
		private RectTransform PersonalChatContainer;

		[Header("Prefabs")]
		[SerializeField]
		private PersonalTextMessageView TextRightPrefab;

		[SerializeField]
		private PersonalTextMessageView TextLeftPrefab;

		[SerializeField]
		private PersonalSmileMessageView SmileRightPrefab;

		[SerializeField]
		private PersonalSmileMessageView SmileLeftPrefab;

		[SerializeField]
		private CarExchangeMessageView CarExchangeRightPrefab;

		[SerializeField]
		private CarExchangeMessageView CarExchangeLeftPrefab;

		[SerializeField]
		private CarSellMessageView CarSellRightPrefab;

		[SerializeField]
		private CarSellMessageView CarSellLeftPrefab;

		[SerializeField]
		private SendMoneyMessageView SendMoneyRightPrefab;

		[SerializeField]
		private SendMoneyMessageView SendMoneyLeftPrefab;

		[SerializeField]
		private InviteToFriendsMessageView InviteToFriendsRightPrefab;

		[SerializeField]
		private InviteToFriendsMessageView InviteToFriendsLeftPrefab;

		[SerializeField]
		private ConnectionMessageView ConnectionPrefab;

		[SerializeField]
		private MuteMessageView MutePrefab;

		[SerializeField]
		private BlockMessageView BlockPrefab;

		[Header("Pool Settings")]
		[SerializeField]
		private int InitialPoolSize = 5;

		private ObjectPool<PersonalTextMessageView> _rightTextPool;

		private ObjectPool<PersonalTextMessageView> _leftTextPool;

		private ObjectPool<PersonalSmileMessageView> _rightSmilePool;

		private ObjectPool<PersonalSmileMessageView> _leftSmilePool;

		private ObjectPool<CarExchangeMessageView> _rightCarExchangePool;

		private ObjectPool<CarExchangeMessageView> _leftCarExchangePool;

		private ObjectPool<CarSellMessageView> _rightCarSellPool;

		private ObjectPool<CarSellMessageView> _leftCarSellPool;

		private ObjectPool<SendMoneyMessageView> _rightSendMoneyPool;

		private ObjectPool<SendMoneyMessageView> _leftSendMoneyPool;

		private ObjectPool<InviteToFriendsMessageView> _rightInviteToFriendsPool;

		private ObjectPool<InviteToFriendsMessageView> _leftInviteToFriendsPool;

		private ObjectPool<ConnectionMessageView> _connectionPool;

		private ObjectPool<MuteMessageView> _mutePool;

		private ObjectPool<BlockMessageView> _blockPool;

		private IPrefabFactory _prefabFactory;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory)
		{
			_prefabFactory = prefabFactory;
		}

		private void Awake()
		{
			InitializePools();
		}

		private void InitializePools()
		{
			_rightTextPool = new ObjectPool<PersonalTextMessageView>(() => CreateInstance(TextRightPrefab), InitialPoolSize);
			_leftTextPool = new ObjectPool<PersonalTextMessageView>(() => CreateInstance(TextLeftPrefab), InitialPoolSize);
			_rightSmilePool = new ObjectPool<PersonalSmileMessageView>(() => CreateInstance(SmileRightPrefab), InitialPoolSize);
			_leftSmilePool = new ObjectPool<PersonalSmileMessageView>(() => CreateInstance(SmileLeftPrefab), InitialPoolSize);
			_rightCarExchangePool = new ObjectPool<CarExchangeMessageView>(() => CreateInstance(CarExchangeRightPrefab), InitialPoolSize);
			_leftCarExchangePool = new ObjectPool<CarExchangeMessageView>(() => CreateInstance(CarExchangeLeftPrefab), InitialPoolSize);
			_rightCarSellPool = new ObjectPool<CarSellMessageView>(() => CreateInstance(CarSellRightPrefab), InitialPoolSize);
			_leftCarSellPool = new ObjectPool<CarSellMessageView>(() => CreateInstance(CarSellLeftPrefab), InitialPoolSize);
			_rightSendMoneyPool = new ObjectPool<SendMoneyMessageView>(() => CreateInstance(SendMoneyRightPrefab), InitialPoolSize);
			_leftSendMoneyPool = new ObjectPool<SendMoneyMessageView>(() => CreateInstance(SendMoneyLeftPrefab), InitialPoolSize);
			_rightInviteToFriendsPool = new ObjectPool<InviteToFriendsMessageView>(() => CreateInstance(InviteToFriendsRightPrefab), InitialPoolSize);
			_leftInviteToFriendsPool = new ObjectPool<InviteToFriendsMessageView>(() => CreateInstance(InviteToFriendsLeftPrefab), InitialPoolSize);
			_connectionPool = new ObjectPool<ConnectionMessageView>(() => CreateInstance(ConnectionPrefab), InitialPoolSize);
			_mutePool = new ObjectPool<MuteMessageView>(() => CreateInstance(MutePrefab), InitialPoolSize);
			_blockPool = new ObjectPool<BlockMessageView>(() => CreateInstance(BlockPrefab), InitialPoolSize);
		}

		private T CreateInstance<T>(T prefab) where T : MonoBehaviour
		{
			T val = _prefabFactory.Instantiate(prefab, PersonalChatContainer);
			val.gameObject.SetActive(value: false);
			return val;
		}

		public ChatMessageView AddNewMessage(ChatMessage message, string chatId)
		{
			ChatMessageView result = null;
			bool flag = ChatWindow.IsOwnMessage(message);
			switch (message.MessageType)
			{
			case ChatMessageType.Text:
			{
				PersonalTextMessageView personalTextMessageView = (flag ? _rightTextPool : _leftTextPool).Get();
				personalTextMessageView.transform.SetAsLastSibling();
				personalTextMessageView.Init(ChatWindow, message, chatId);
				personalTextMessageView.gameObject.SetActive(value: true);
				result = personalTextMessageView;
				break;
			}
			case ChatMessageType.Smile:
			{
				PersonalSmileMessageView personalSmileMessageView = (flag ? _rightSmilePool : _leftSmilePool).Get();
				personalSmileMessageView.transform.SetAsLastSibling();
				personalSmileMessageView.Init(ChatWindow, message, chatId);
				personalSmileMessageView.gameObject.SetActive(value: true);
				result = personalSmileMessageView;
				break;
			}
			case ChatMessageType.CarExchange:
			{
				CarExchangeMessageView carExchangeMessageView = (flag ? _rightCarExchangePool : _leftCarExchangePool).Get();
				carExchangeMessageView.transform.SetAsLastSibling();
				carExchangeMessageView.Init(ChatWindow, message, chatId);
				carExchangeMessageView.gameObject.SetActive(value: true);
				result = carExchangeMessageView;
				break;
			}
			case ChatMessageType.CarSell:
			{
				CarSellMessageView carSellMessageView = (flag ? _rightCarSellPool : _leftCarSellPool).Get();
				carSellMessageView.transform.SetAsLastSibling();
				carSellMessageView.Init(ChatWindow, message, chatId);
				carSellMessageView.gameObject.SetActive(value: true);
				result = carSellMessageView;
				break;
			}
			case ChatMessageType.SendMoney:
			{
				SendMoneyMessageView sendMoneyMessageView = (flag ? _rightSendMoneyPool : _leftSendMoneyPool).Get();
				sendMoneyMessageView.transform.SetAsLastSibling();
				sendMoneyMessageView.Init(ChatWindow, message, chatId);
				sendMoneyMessageView.gameObject.SetActive(value: true);
				result = sendMoneyMessageView;
				break;
			}
			case ChatMessageType.InviteToFriends:
			{
				InviteToFriendsMessageView inviteToFriendsMessageView = (flag ? _rightInviteToFriendsPool : _leftInviteToFriendsPool).Get();
				inviteToFriendsMessageView.transform.SetAsLastSibling();
				inviteToFriendsMessageView.Init(ChatWindow, message, chatId);
				inviteToFriendsMessageView.gameObject.SetActive(value: true);
				result = inviteToFriendsMessageView;
				break;
			}
			case ChatMessageType.ConnectionState:
			{
				ConnectionMessageView connectionMessageView = _connectionPool.Get();
				connectionMessageView.transform.SetAsLastSibling();
				connectionMessageView.Init(ChatWindow, message, chatId);
				connectionMessageView.gameObject.SetActive(value: true);
				result = connectionMessageView;
				break;
			}
			case ChatMessageType.Mute:
			{
				MuteMessageView muteMessageView = _mutePool.Get();
				muteMessageView.transform.SetAsLastSibling();
				muteMessageView.Init(ChatWindow, message, chatId);
				muteMessageView.gameObject.SetActive(value: true);
				result = muteMessageView;
				break;
			}
			case ChatMessageType.Block:
			{
				BlockMessageView blockMessageView = _blockPool.Get();
				blockMessageView.transform.SetAsLastSibling();
				blockMessageView.Init(ChatWindow, message, chatId);
				blockMessageView.gameObject.SetActive(value: true);
				result = blockMessageView;
				break;
			}
			}
			return result;
		}

		public void ReturnMessage(ChatMessageView message)
		{
			if (!message)
			{
				return;
			}
			message.gameObject.SetActive(value: false);
			if (message is PersonalTextMessageView obj)
			{
				if (_rightTextPool.Contains(obj))
				{
					_rightTextPool.Return(obj);
				}
				else if (_leftTextPool.Contains(obj))
				{
					_leftTextPool.Return(obj);
				}
			}
			else if (message is PersonalSmileMessageView obj2)
			{
				if (_rightSmilePool.Contains(obj2))
				{
					_rightSmilePool.Return(obj2);
				}
				else if (_leftSmilePool.Contains(obj2))
				{
					_leftSmilePool.Return(obj2);
				}
			}
			else if (message is CarExchangeMessageView obj3)
			{
				if (_rightCarExchangePool.Contains(obj3))
				{
					_rightCarExchangePool.Return(obj3);
				}
				else if (_leftCarExchangePool.Contains(obj3))
				{
					_leftCarExchangePool.Return(obj3);
				}
			}
			else if (message is CarSellMessageView obj4)
			{
				if (_rightCarSellPool.Contains(obj4))
				{
					_rightCarSellPool.Return(obj4);
				}
				else if (_leftCarSellPool.Contains(obj4))
				{
					_leftCarSellPool.Return(obj4);
				}
			}
			else if (message is SendMoneyMessageView obj5)
			{
				if (_rightSendMoneyPool.Contains(obj5))
				{
					_rightSendMoneyPool.Return(obj5);
				}
				else if (_leftSendMoneyPool.Contains(obj5))
				{
					_leftSendMoneyPool.Return(obj5);
				}
			}
			else if (message is InviteToFriendsMessageView obj6)
			{
				if (_rightInviteToFriendsPool.Contains(obj6))
				{
					_rightInviteToFriendsPool.Return(obj6);
				}
				else if (_leftInviteToFriendsPool.Contains(obj6))
				{
					_leftInviteToFriendsPool.Return(obj6);
				}
			}
			else if (message is ConnectionMessageView obj7)
			{
				_connectionPool.Return(obj7);
			}
			else if (message is MuteMessageView obj8)
			{
				_mutePool.Return(obj8);
			}
			else if (message is BlockMessageView obj9)
			{
				_blockPool.Return(obj9);
			}
		}

		public void ClearAllMessages()
		{
			_rightTextPool.ReturnAll();
			_leftTextPool.ReturnAll();
			_rightSmilePool.ReturnAll();
			_leftSmilePool.ReturnAll();
			_rightCarExchangePool.ReturnAll();
			_leftCarExchangePool.ReturnAll();
			_rightCarSellPool.ReturnAll();
			_leftCarSellPool.ReturnAll();
			_rightSendMoneyPool.ReturnAll();
			_leftSendMoneyPool.ReturnAll();
			_rightInviteToFriendsPool.ReturnAll();
			_leftInviteToFriendsPool.ReturnAll();
			_connectionPool.ReturnAll();
			_mutePool.ReturnAll();
			_blockPool.ReturnAll();
		}
	}
}
