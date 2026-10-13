using System;
using System.Collections.Generic;
using CPM.PlayerData;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public class MoneySendService : IChatService
	{
		public const float OfferLifeTime = 30f;

		public Action<ChatOffer> OnMoneySend;

		public Action<ChatOffer> OnNewOfferReceived;

		private Dictionary<string, ChatOffer> _offers = new Dictionary<string, ChatOffer>();

		private CurrencyManagerController _currencyManagerController;

		private MultiPlayer _multiPlayer;

		private PlayerDataStorage _playerDataStorage;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private ManagerUI _managerUI;

		private FreeDriveProvider _freeDriveProvider;

		private ChatControl _chatControl;

		[Inject]
		public void Construct(CurrencyManagerController currencyManagerController, MultiPlayer multiPlayer, PlayerDataStorage playerDataStorage, OnlineCollectionsProvider onlineCollectionsProvider, ManagerUI managerUI, FreeDriveProvider freeDriveProvider, ChatControl chatControl)
		{
			_chatControl = chatControl;
			_freeDriveProvider = freeDriveProvider;
			_managerUI = managerUI;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_playerDataStorage = playerDataStorage;
			_multiPlayer = multiPlayer;
			_currencyManagerController = currencyManagerController;
			_freeDriveProvider.MoneySendService = this;
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

		public void Send(ObscuredInt moneySend, string playerId)
		{
			string text = _chatControl.GetNewRandomOfferID();
			if (_offers.ContainsKey(text))
			{
				text += playerId;
			}
			string message = $"{moneySend}?{text}";
			ChatOffer chatOffer = new ChatOffer(text, message, _playerDataStorage.PlayerID, playerId, 30f);
			SendRpc(chatOffer);
			OnMoneySend?.Invoke(chatOffer);
			_offers.Add(text, chatOffer);
		}

		public void CancelRequest(string offerId)
		{
			if (!_offers.TryGetValue(offerId, out var value))
			{
				return;
			}
			if (value.State == TradeOfferState.Declined)
			{
				_managerUI.ShowNotificationWindow("Request already declined", 3f).Forget();
				return;
			}
			if (value.State == TradeOfferState.Completed)
			{
				_managerUI.ShowNotificationWindow("Request already completed", 3f).Forget();
				return;
			}
			MultiPersonControl playerByPlayerID = _onlineCollectionsProvider.GetPlayerByPlayerID(value.ReceiverId);
			if ((bool)playerByPlayerID)
			{
				playerByPlayerID.moneySend = false;
			}
			value.State = TradeOfferState.Cancelled;
			SendRpc(value);
		}

		public void Confirm(string offerId)
		{
			if (_offers.TryGetValue(offerId, out var value))
			{
				if (value.State == TradeOfferState.Cancelled)
				{
					_managerUI.ShowNotificationWindow("Request already canceled", 3f).Forget();
					return;
				}
				value.State = TradeOfferState.Processing;
				SendRpc(value);
			}
		}

		public void Decline(string offerId)
		{
			if (_offers.TryGetValue(offerId, out var value))
			{
				if (value.State == TradeOfferState.Cancelled)
				{
					_managerUI.ShowNotificationWindow("Request already canceled", 3f).Forget();
					return;
				}
				value.State = TradeOfferState.Declined;
				SendRpc(value);
			}
		}

		public void SetPending(string offerId)
		{
			if (_offers.TryGetValue(offerId, out var value))
			{
				value.State = TradeOfferState.Pending;
				SendRpc(value);
			}
		}

		public void NewOfferReceived(string playerId, ObscuredInt moneyReceived, string offerId)
		{
			string message = $"{moneyReceived}?{offerId}";
			ChatOffer chatOffer = new ChatOffer(offerId, message, playerId, _playerDataStorage.PlayerID, 30f);
			_offers.Add(offerId, chatOffer);
			OnNewOfferReceived?.Invoke(chatOffer);
			chatOffer.OnTimeOutEvent += Decline;
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
			if (_offers.TryGetValue(offerId, out var value) && value.State != TradeOfferState.Cancelled)
			{
				bool flag = TrySpendMoney(value);
				value.State = ((!flag) ? TradeOfferState.Cancelled : TradeOfferState.Completed);
				SendRpc(value);
			}
		}

		public void OfferDeclined(string offerId)
		{
			if (_offers.TryGetValue(offerId, out var value))
			{
				value.State = TradeOfferState.Declined;
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
				ReceiveMoney(value);
			}
		}

		private void SendRpc(ChatOffer offer)
		{
			string playerID = (((ObscuredString)offer.OfferCreatorId == _playerDataStorage.PlayerID) ? offer.ReceiverId : offer.OfferCreatorId);
			PhotonView photonView = _onlineCollectionsProvider.GetPlayerByPlayerID(playerID).photonView;
			int ownerActorNr = photonView.OwnerActorNr;
			string userId = photonView.Controller.UserId;
			string name = $"{offer.Message}?{(int)offer.State}?{_playerDataStorage.PlayerID}";
			_multiPlayer.SendValueStrToPlayer<RpcSendMoney>(ownerActorNr.ToString(), name, userId);
		}

		private void ReceiveMoney(ChatOffer offer)
		{
			if (int.TryParse(offer.Message.Split("?"[0])[0], out var result))
			{
				MultiPersonControl playerByPlayerID = _onlineCollectionsProvider.GetPlayerByPlayerID(offer.OfferCreatorId);
				_currencyManagerController.AddMoney(new AdvancedInt(result));
				string arg = PlayerDatas.CheckNameValid(playerByPlayerID.ThisPlayerName);
				string format = new LocalizedString("RpcSendMoney.ReceivedMoneyreceivedN0MoneyFr");
				V.CoreGameManager.SetMessage(string.Format(format, result, arg), 5f);
			}
		}

		private bool TrySpendMoney(ChatOffer offer)
		{
			if (int.TryParse(offer.Message.Split("?"[0])[0], out var result))
			{
				MultiPersonControl playerByPlayerID = _onlineCollectionsProvider.GetPlayerByPlayerID(offer.ReceiverId);
				if (!playerByPlayerID)
				{
					return false;
				}
				_currencyManagerController.TryMoneySpend(out var result2, new AdvancedInt(result), null, null, sendNotEnoughMoneyIfNeed: false);
				if (!result2)
				{
					return false;
				}
				string format = new LocalizedString("PlayerProfileWindow.moneywassenttoaplayer");
				_managerUI.ShowNotificationWindow(string.Format(format, $"{result:n0}", playerByPlayerID.ThisPlayerName), 3f).Forget();
				int num = ObscuredPrefs.GetInt("moneySendDay" + V.PassedDay);
				ObscuredPrefs.SetInt("moneySendDay" + V.PassedDay, result + num);
				playerByPlayerID.moneySend = true;
				return true;
			}
			return false;
		}
	}
}
