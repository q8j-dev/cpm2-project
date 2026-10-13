using System;
using System.Collections.Generic;
using CPM.PlayerData;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public class AddFriendService : IChatService
	{
		public const float OfferLifeTime = 30f;

		public Action<ChatOffer> OnSendRequestToAddFriend;

		public Action<ChatOffer> OnReceiveRequestToAddFriend;

		private Dictionary<string, ChatOffer> _offers = new Dictionary<string, ChatOffer>();

		private ManagerUI _managerUI;

		private MultiPlayer _multiplayer;

		private FreeDriveControl _freeDriveControl;

		private PlayerDataStorage _playerDataStorage;

		private FreeDriveProvider _freeDriveProvider;

		private PlayerDatas.FriendsUtility _friendsUtility;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private ChatControl _chatControl;

		[Inject]
		public void Construct(FreeDriveControl freeDriveControl, ManagerUI managerUI, MultiPlayer multiplayer, PlayerDatas.FriendsUtility friendsUtility, PlayerDataStorage playerDataStorage, FreeDriveProvider freeDriveProvider, OnlineCollectionsProvider onlineCollectionsProvider, ChatControl chatControl)
		{
			_chatControl = chatControl;
			_playerDataStorage = playerDataStorage;
			_multiplayer = multiplayer;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_freeDriveProvider = freeDriveProvider;
			_friendsUtility = friendsUtility;
			_managerUI = managerUI;
			_freeDriveControl = freeDriveControl;
			_freeDriveProvider.AddFriendService = this;
		}

		public ChatOffer GetOffer(string offerId)
		{
			if (string.IsNullOrEmpty(offerId))
			{
				return null;
			}
			return _offers.GetValueOrDefault(offerId);
		}

		public void ClearOffers()
		{
			foreach (ChatOffer value in _offers.Values)
			{
				value.OnTimeOutEvent -= Decline;
				value.Dispose();
			}
			_offers.Clear();
		}

		public void SendRequestToAddFriendList(GameObject person)
		{
			MultiPersonControl component = person.GetComponent<MultiPersonControl>();
			int ownerActorNr = component.photonView.OwnerActorNr;
			string thisPlayerID = component.ThisPlayerID;
			if (!_friendsUtility.HasFriendWithID(thisPlayerID))
			{
				string text = _chatControl.GetNewRandomOfferID();
				if (_offers.ContainsKey(text))
				{
					text += thisPlayerID;
				}
				ChatOffer chatOffer = new ChatOffer(text, text, _playerDataStorage.PlayerID, thisPlayerID, 30f);
				SendRpc(chatOffer, ownerActorNr, thisPlayerID);
				_offers.Add(text, chatOffer);
				OnSendRequestToAddFriend?.Invoke(chatOffer);
			}
			else
			{
				Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612)).Subscribe((long _) =>
				{
					PlayerExistMessage();
				});
			}
		}

		private void PlayerExistMessage()
		{
			_managerUI.ShowNotificationWindow(new LocalizedString("FreeDriveControl.ThisPlayerAlreadyInYourFriendList"), 3f).Forget();
		}

		public void RequestToFriendReceived(string playerId, string offerId)
		{
			foreach (string item in _freeDriveControl.playerToIgnore)
			{
				if (item == playerId)
				{
					Decline(playerId);
					return;
				}
			}
			ChatOffer chatOffer = new ChatOffer(offerId, offerId, playerId, _playerDataStorage.PlayerID, 30f);
			_offers.Add(offerId, chatOffer);
			OnReceiveRequestToAddFriend?.Invoke(chatOffer);
			chatOffer.OnTimeOutEvent += Decline;
		}

		public void CancelRequest(string offerId)
		{
			if (_offers.TryGetValue(offerId, out var value))
			{
				if (value.State == TradeOfferState.Declined)
				{
					_managerUI.ShowNotificationWindow("Request already declined", 3f).Forget();
					return;
				}
				value.State = TradeOfferState.Cancelled;
				SendRpc(value, value.ReceiverId);
			}
		}

		public void Confirm(string offerId)
		{
			if (_friendsUtility.CanAddFriend())
			{
				if (!_offers.TryGetValue(offerId, out var value))
				{
					return;
				}
				if (value.State == TradeOfferState.Cancelled)
				{
					_managerUI.ShowNotificationWindow("Request already canceled", 3f).Forget();
					return;
				}
				value.State = TradeOfferState.Processing;
				MultiPersonControl playerByPlayerID = _onlineCollectionsProvider.GetPlayerByPlayerID(value.OfferCreatorId);
				if ((bool)playerByPlayerID)
				{
					string target = playerByPlayerID.ThisPlayerID.ToUpper();
					string b = playerByPlayerID.photonView.OwnerActorNr.ToString();
					_multiplayer.SendValueStrToPlayer(27, b, offerId, target);
				}
			}
			else
			{
				Decline(offerId);
				_managerUI.ShowNotificationWindow(new LocalizedString("FreeDriveControl.FriendListIsFull"), 3f).Forget();
			}
		}

		public void Decline(string offerId)
		{
			if (!_offers.TryGetValue(offerId, out var value))
			{
				return;
			}
			if (value.State == TradeOfferState.Cancelled)
			{
				_managerUI.ShowNotificationWindow("Request already canceled", 3f).Forget();
				return;
			}
			value.State = TradeOfferState.Declined;
			MultiPersonControl playerByPlayerID = _onlineCollectionsProvider.GetPlayerByPlayerID(value.OfferCreatorId);
			if ((bool)playerByPlayerID)
			{
				PhotonView component = playerByPlayerID.GetComponent<PhotonView>();
				_ = (string)_playerDataStorage.PlayerID;
				_multiplayer.SendValueStrToPlayer(28, component.OwnerActorNr.ToString(), offerId, component.Owner);
			}
		}

		public void SetPending(string offerId)
		{
			if (_offers.TryGetValue(offerId, out var value))
			{
				value.State = TradeOfferState.Pending;
				SendRpc(value, value.OfferCreatorId);
			}
		}

		public void OfferCanceled(string offerId)
		{
			if (_offers.TryGetValue(offerId, out var value))
			{
				value.State = TradeOfferState.Cancelled;
			}
		}

		public void OfferConfirmed(string offerId)
		{
			if (!_offers.TryGetValue(offerId, out var value) || value.State == TradeOfferState.Cancelled)
			{
				return;
			}
			value.State = TradeOfferState.Completed;
			if (_friendsUtility.CanAddFriend())
			{
				MultiPersonControl playerByPlayerID = _onlineCollectionsProvider.GetPlayerByPlayerID(value.ReceiverId);
				string thisPlayerName = playerByPlayerID.ThisPlayerName;
				_friendsUtility.AddFriend(playerByPlayerID.ThisPlayerID.ToUpper(), thisPlayerName, playerByPlayerID.ThisPlayerAccountID, fromRoom: true);
				if ((bool)_freeDriveProvider.PlayerListControl)
				{
					_freeDriveProvider.PlayerListControl.UpdatePlayersList();
				}
				SendRpc(value, value.ReceiverId);
				string text = new LocalizedString("FreeDriveControl.VLastreveivedfriendnametoaddIs");
				_managerUI.ShowNotificationWindow(text.Replace("{0}", thisPlayerName), 3f, Color.green);
			}
			else
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("FreeDriveControl.FriendListIsFull"), 3f, Color.red);
				CancelRequest(offerId);
			}
		}

		public void OfferDeclined(string offerId)
		{
			if (_offers.TryGetValue(offerId, out var value))
			{
				value.State = TradeOfferState.Declined;
				string thisPlayerName = _onlineCollectionsProvider.GetPlayerByPlayerID(value.ReceiverId).ThisPlayerName;
				string text = new LocalizedString("FreeDriveControl.IsNotFriend");
				_managerUI.ShowNotificationWindow(text.Replace("{0}", thisPlayerName), 3f, Color.red);
			}
		}

		public void OfferPending(string offerId)
		{
			if (_offers.TryGetValue(offerId, out var value))
			{
				value.State = TradeOfferState.Pending;
			}
		}

		public void OnOfferCompleted(string offerId)
		{
			if (_offers.TryGetValue(offerId, out var value))
			{
				value.State = TradeOfferState.Completed;
				AddToFriends(value);
			}
		}

		private void AddToFriends(ChatOffer offer)
		{
			MultiPersonControl playerByPlayerID = _onlineCollectionsProvider.GetPlayerByPlayerID(offer.OfferCreatorId);
			if ((bool)playerByPlayerID)
			{
				string thisPlayerName = playerByPlayerID.ThisPlayerName;
				string localID = playerByPlayerID.ThisPlayerID.ToUpper();
				_friendsUtility.AddFriend(localID, thisPlayerName, playerByPlayerID.ThisPlayerAccountID, fromRoom: true);
				string text = new LocalizedString("FreeDriveControl.VLastreveivedfriendnametoaddAn");
				_managerUI.ShowNotificationWindow(text.Replace("{0}", thisPlayerName), 3f).Forget();
			}
		}

		private void SendRpc(ChatOffer offer, string rpcReceiver)
		{
			MultiPersonControl playerByPlayerID = _onlineCollectionsProvider.GetPlayerByPlayerID(rpcReceiver);
			int ownerActorNr = playerByPlayerID.photonView.OwnerActorNr;
			string thisPlayerID = playerByPlayerID.ThisPlayerID;
			SendRpc(offer, ownerActorNr, thisPlayerID);
		}

		private void SendRpc(ChatOffer offer, int idToRequest, string playerId)
		{
			string name = $"{_playerDataStorage.PlayerID}?{(int)offer.State}?{offer.TradeID}";
			_multiplayer.SendValueStrToPlayer(26, idToRequest.ToString(), name, playerId);
		}
	}
}
