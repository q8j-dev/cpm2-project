using System;
using UniRx;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public class ChatOffer : Offer
	{
		private IDisposable _timerDisposable;

		public string Message { get; private set; }

		public string OfferCreatorId { get; private set; }

		public string ReceiverId { get; private set; }

		public event Action<string> OnTimeOutEvent;

		public ChatOffer(string tradeID, string message, string creatorId, string receiverId, float lifeTime)
		{
			base.TradeID = tradeID;
			Message = message;
			OfferCreatorId = creatorId;
			ReceiverId = receiverId;
			LifeTimeSeconds = lifeTime;
			base.State = TradeOfferState.Enabled;
			base.TimeCreated = DateTime.Now;
			base.OnStateChangedEvent += OnStateChanged;
			StartTimer();
		}

		private void StartTimer()
		{
			StopTimer();
			_timerDisposable = Observable.Timer(TimeSpan.FromSeconds(LifeTimeSeconds)).Subscribe((long _) =>
			{
				TimeOut();
			});
		}

		private void TimeOut()
		{
			base.State = TradeOfferState.Declined;
			OnTimeOutEvent?.Invoke(base.TradeID);
		}

		private void OnStateChanged(TradeOfferState state)
		{
			if (state != TradeOfferState.Enabled)
			{
				StopTimer();
			}
		}

		private void StopTimer()
		{
			_timerDisposable?.Dispose();
		}

		public void Dispose()
		{
			StopTimer();
			base.OnStateChangedEvent -= OnStateChanged;
		}
	}
}
