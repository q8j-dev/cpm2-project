namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public interface IChatService
	{
		void CancelRequest(string offerId);

		void Confirm(string offerId);

		void Decline(string offerId);

		void SetPending(string offerId);

		void OfferCanceled(string offerId);

		void OfferConfirmed(string offerId);

		void OfferDeclined(string offerId);

		void OfferPending(string offerId);

		void OnOfferCompleted(string offerId);
	}
}
