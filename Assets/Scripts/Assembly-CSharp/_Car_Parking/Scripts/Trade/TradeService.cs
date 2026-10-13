using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.GameFacts.Base;
using _Car_Parking.Scripts.Multiplayer.RpcMethods.RpcObjects;

namespace _Car_Parking.Scripts.Trade
{
	[Preserve]
	public class TradeService : ITradeService, IDisposable, IInitializable
	{
		private const float DelayToCheckOfferTime = 1f;

		private readonly Dictionary<string, TradeOffer> _tradeOffers = new Dictionary<string, TradeOffer>();

		private readonly TradeOfferHandlersHolder _tradeOfferHandlersHolder;

		private readonly ManagerUI _managerUI;

		private readonly TradeAnalytic _tradeAnalytic;

		private readonly IGameFactManager _gameFactManager;

		private CancellationTokenSource _mainCTS;

		public IReadOnlyCollection<TradeOffer> TradeOffers => _tradeOffers.Values;

		public bool Enabled => !_gameFactManager.IsTrue(GameFact.ArenaActive);

		private MultiPlayer MultiPlayer => (V.CoreGameManager ? V.CoreGameManager.FreeDriveProvider : null)?.MultiPlayer;

		public TradeService(IEnumerable<ITradeOfferHandler> handlers, ManagerUI managerUI, TradeAnalytic tradeAnalytic, IGameFactManager gameFactManager)
		{
			_tradeOfferHandlersHolder = new TradeOfferHandlersHolder(handlers.ToArray());
			_managerUI = managerUI;
			_tradeAnalytic = tradeAnalytic;
			_gameFactManager = gameFactManager;
			_mainCTS = new CancellationTokenSource();
		}

		public void Initialize()
		{
			V.OnInGarageFreeChanged = (Action<bool>)Delegate.Combine(V.OnInGarageFreeChanged, new Action<bool>(OnInGarageFreeChanged));
			V.OnPlayerDisconnected += OnPlayerDisconnected;
			CancelOffersByTimeAsync(_mainCTS.Token).Forget();
		}

		public void Dispose()
		{
			V.OnInGarageFreeChanged = (Action<bool>)Delegate.Remove(V.OnInGarageFreeChanged, new Action<bool>(OnInGarageFreeChanged));
			V.OnPlayerDisconnected -= OnPlayerDisconnected;
			_tradeOfferHandlersHolder.Dispose();
			CommonExtensions.CancelAndDisposeToken(ref _mainCTS);
		}

		public TradeOffer GetOffer(string tradeId)
		{
			if (string.IsNullOrEmpty(tradeId))
			{
				return null;
			}
			return _tradeOffers.GetValueOrDefault(tradeId);
		}

		public bool AddOffer(TradeOffer offer)
		{
			if (offer == null)
			{
				return false;
			}
			if (GetOffer(offer.TradeID) != null)
			{
				return false;
			}
			_tradeOffers.Add(offer.TradeID, offer);
			return true;
		}

		public bool CancelOrDeclineOffer(string tradeId, TradeOfferCancelReason cancelReason, bool notifyOtherPlayer = true)
		{
			if (string.IsNullOrEmpty(tradeId))
			{
				return false;
			}
			if (!_tradeOffers.TryGetValue(tradeId, out var value))
			{
				return false;
			}
			return CancelOffer(tradeId, cancelReason, notifyOtherPlayer, value.OfferActorNr == PhotonNetwork.LocalPlayer.ActorNumber);
		}

		public void CancelOrDeclineAllOffers(TradeOfferCancelReason cancelReason, bool notifyOtherPlayer, TradeOfferVisibleFilter visibleFilter = TradeOfferVisibleFilter.All)
		{
			List<string> list = new List<string>();
			foreach (TradeOffer value in _tradeOffers.Values)
			{
				if (value.CanCancel && (visibleFilter == TradeOfferVisibleFilter.All || (visibleFilter == TradeOfferVisibleFilter.OnlyPrivate && value.VisibleType == TradeOfferVisibleType.Private) || (visibleFilter == TradeOfferVisibleFilter.OnlyPublic && value.VisibleType == TradeOfferVisibleType.Public)))
				{
					list.Add(value.TradeID);
				}
			}
			foreach (string item in list)
			{
				CancelOrDeclineOffer(item, cancelReason, notifyOtherPlayer);
			}
		}

		public void CancelOrDeclineAllOffersByActorNr(int actorNr, TradeOfferCancelReason cancelReason, bool notifyOtherPlayer)
		{
			List<string> list = new List<string>();
			foreach (TradeOffer value in _tradeOffers.Values)
			{
				if (value.CanCancel && (value.OfferActorNr == actorNr || value.RequestActorNr == actorNr))
				{
					list.Add(value.TradeID);
				}
			}
			foreach (string item in list)
			{
				CancelOrDeclineOffer(item, cancelReason, notifyOtherPlayer);
			}
		}

		public bool CancelOffer(string tradeId, TradeOfferCancelReason cancelReason, bool notifyOtherPlayer = true, bool isCanceled = true)
		{
			if (string.IsNullOrEmpty(tradeId))
			{
				return false;
			}
			if (_tradeOffers.TryGetValue(tradeId, out var value))
			{
				if (!value.CanCancel)
				{
					return false;
				}
				value.State = (isCanceled ? TradeOfferState.Cancelled : TradeOfferState.Declined);
				AnalyticsCancelLog(value, cancelReason);
				if ((((value.RequestActorNr == PhotonNetwork.LocalPlayer.ActorNumber || value.OfferActorNr == PhotonNetwork.LocalPlayer.ActorNumber) && value.VisibleType == TradeOfferVisibleType.Private) & notifyOtherPlayer) && (bool)MultiPlayer)
				{
					int actorNumber = ((value.RequestActorNr == PhotonNetwork.LocalPlayer.ActorNumber) ? value.OfferActorNr : value.RequestActorNr);
					MultiPlayer.SendValueStrToPlayer<RpcTradeOfferCancelled>(RpcTradeOfferCancelled.GetRequestData(tradeId, isCanceled, cancelReason), string.Empty, actorNumber);
				}
				return true;
			}
			return false;
		}

		public void CompleteOffer(string tradeId)
		{
			if (!string.IsNullOrEmpty(tradeId) && _tradeOffers.TryGetValue(tradeId, out var value) && value.CanComplete)
			{
				value.State = TradeOfferState.Completed;
			}
		}

		public async UniTask<bool> TryTradeAsync(string tradeId)
		{
			TradeOffer tradeOffer = GetOffer(tradeId);
			if (tradeOffer == null)
			{
				return false;
			}
			if (!tradeOffer.CanComplete)
			{
				return false;
			}
			ITradeOfferHandler handler = _tradeOfferHandlersHolder.GetHandler(tradeOffer);
			if (handler == null)
			{
				return false;
			}
			if (tradeOffer.State != TradeOfferState.Pending)
			{
				tradeOffer.State = TradeOfferState.Pending;
				if (tradeOffer.VisibleType == TradeOfferVisibleType.Private && tradeOffer.OfferActorNr != PhotonNetwork.LocalPlayer.ActorNumber && tradeOffer.OfferActorNr >= 0 && (bool)MultiPlayer)
				{
					MultiPlayer.SendValueStrToPlayer<RpcTradeOfferPending>(tradeId, string.Empty, tradeOffer.OfferActorNr);
				}
			}
			PreTradeResultActionType preTradeResultActionType = await handler.PreTradeAction(tradeOffer);
			if (preTradeResultActionType != PreTradeResultActionType.Accept)
			{
				if (tradeOffer.VisibleType == TradeOfferVisibleType.Private)
				{
					TradeOfferCancelReason cancelReason = TradeOfferCancelReason.None;
					switch (preTradeResultActionType)
					{
					case PreTradeResultActionType.NotEnoughMoney:
						cancelReason = TradeOfferCancelReason.NotEnoughMoney;
						break;
					case PreTradeResultActionType.Reject:
						cancelReason = TradeOfferCancelReason.Manual;
						break;
					}
					CancelOrDeclineOffer(tradeOffer.TradeID, cancelReason);
				}
				return false;
			}
			_managerUI.ShowWaitingWindow(set: true).Forget();
			bool result = false;
			try
			{
				result = await handler.TradeAction(tradeOffer);
				if (result)
				{
					_managerUI.CloseWaitingWindow();
					await handler.PostTradeAction(tradeOffer);
					CompleteOffer(tradeOffer.TradeID);
				}
				else
				{
					CancelOrDeclineOffer(tradeOffer.TradeID, TradeOfferCancelReason.None);
				}
			}
			catch (Exception)
			{
			}
			finally
			{
				_managerUI.CloseWaitingWindow();
			}
			return result;
		}

		public List<TradeOffer> GetMyActiveOffers()
		{
			List<TradeOffer> list = new List<TradeOffer>();
			foreach (TradeOffer value in _tradeOffers.Values)
			{
				if (value.CanComplete && value.OfferActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					list.Add(value);
				}
			}
			return list;
		}

		public void ClearOffers()
		{
			_tradeOffers.Clear();
		}

		private async UniTaskVoid CancelOffersByTimeAsync(CancellationToken cancellationToken)
		{
			List<string> offersToCancel = new List<string>();
			while (!cancellationToken.IsCancellationRequested)
			{
				foreach (TradeOffer value in _tradeOffers.Values)
				{
					if (value.State == TradeOfferState.Enabled && value.SecondUntilExpiration < 0f)
					{
						offersToCancel.Add(value.TradeID);
					}
				}
				foreach (string item in offersToCancel)
				{
					CancelOffer(item, TradeOfferCancelReason.TimeOut, notifyOtherPlayer: false, isCanceled: false);
				}
				offersToCancel.Clear();
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
		}

		private void OnInGarageFreeChanged(bool isInGarage)
		{
			if (isInGarage)
			{
				CancelOrDeclineAllOffers(TradeOfferCancelReason.PlayerInGarage, notifyOtherPlayer: false, TradeOfferVisibleFilter.OnlyPrivate);
			}
		}

		private void OnPlayerDisconnected(Player player)
		{
			CancelOrDeclineAllOffersByActorNr(player.ActorNumber, TradeOfferCancelReason.PlayerOffline, notifyOtherPlayer: false);
		}

		private void AnalyticsCancelLog(TradeOffer offer, TradeOfferCancelReason cancelReason)
		{
			if (offer == null)
			{
				return;
			}
			if (cancelReason == TradeOfferCancelReason.TimeOut)
			{
				_tradeAnalytic.LogCarTradeRequestStatus(offer.TradeID, CarTradeRequestStatusType.Timeout);
				if (offer.RequestActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
				{
					_tradeAnalytic.LogP2pDealStatusEvent(offer.TradeID, P2pDealStatusType.BuyerTimeout);
				}
			}
			if (cancelReason == TradeOfferCancelReason.PlayerOffline && offer.OfferActorNr == PhotonNetwork.LocalPlayer.ActorNumber)
			{
				_tradeAnalytic.LogCarTradeRequestStatus(offer.TradeID, CarTradeRequestStatusType.Timeout);
			}
			if (cancelReason == TradeOfferCancelReason.Manual && offer.State == TradeOfferState.Declined)
			{
				_tradeAnalytic.LogCarTradeRequestStatus(offer.TradeID, CarTradeRequestStatusType.Declined);
				_tradeAnalytic.LogP2pDealStatusEvent(offer.TradeID, P2pDealStatusType.BuyerDeclined);
			}
			if (cancelReason == TradeOfferCancelReason.Manual && offer.State == TradeOfferState.Cancelled)
			{
				_tradeAnalytic.LogCarTradeRequestStatus(offer.TradeID, CarTradeRequestStatusType.CanceledBySender);
			}
			if (cancelReason == TradeOfferCancelReason.NotEnoughMoney && offer.State == TradeOfferState.Declined)
			{
				_tradeAnalytic.LogP2pDealStatusEvent(offer.TradeID, P2pDealStatusType.BuyerConfirmed);
				_tradeAnalytic.LogP2pDealStatusEvent(offer.TradeID, P2pDealStatusType.InsufficientFunds);
			}
		}
	}
}
