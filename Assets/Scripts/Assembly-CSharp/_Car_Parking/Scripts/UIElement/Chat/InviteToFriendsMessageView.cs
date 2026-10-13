using Cysharp.Threading.Tasks;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Trade;
using _Car_Parking.Scripts.WindowSystem.Confirmation;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class InviteToFriendsMessageView : InteractableMessageView, IConfirmWindowSubscriber
	{
		private AddFriendService _addFriendService;

		private ChatOffer _offer;

		[Inject]
		public void Construct(ManagerUI managerUI, AddFriendService addFriendService)
		{
			_addFriendService = addFriendService;
			ManagerUI = managerUI;
		}

		protected void OnEnable()
		{
			if (CurrentMessage != null)
			{
				OfferId = CurrentMessage.EnteredText;
				_offer = _addFriendService.GetOffer(OfferId);
				_offer.OnStateChangedEvent += base.ChangeMessageState;
				switch (_offer.State)
				{
				case TradeOfferState.Enabled:
				{
					OfferInProgressState();
					float startValue = CalculateProgress(_offer.SecondUntilExpiration, 30f);
					StartProgress(_offer.SecondUntilExpiration, startValue);
					break;
				}
				case TradeOfferState.Pending:
				{
					float fillAmount = CalculateProgress(_offer.SecondUntilExpiration, 30f);
					ProgressBar.SetFillAmount(fillAmount);
					OfferPendingState();
					break;
				}
				case TradeOfferState.Cancelled:
					OfferCancelledState();
					break;
				case TradeOfferState.Declined:
					OfferDeclinedState();
					break;
				case TradeOfferState.Completed:
					OfferCompletedState();
					break;
				}
			}
		}

		protected void OnDisable()
		{
			if (_offer != null)
			{
				_offer.OnStateChangedEvent -= base.ChangeMessageState;
			}
		}

		protected override void ShowDetails()
		{
			ManagerUI.ShowSimpleConfirmWindow(new LocalizedString("Chat.InviteToFriends"), new LocalizedString("Chat.ThisPlayerInvitesYouToAFriend"), OnConfirmed, OnCanceled).Forget();
			PauseProgress();
			_addFriendService.SetPending(OfferId);
		}

		protected override void Cancel()
		{
			ManagerUI.ShowSimpleConfirmWindow(new LocalizedString("Chat.ConfirmDecline"), new LocalizedString("Chat.AreYouSureYouWantToDeclineThisInvite"), OnConfirmed, OnCanceled).Forget();
		}

		public override void OnTimeOut()
		{
		}

		public void OnConfirmed()
		{
			if (IsLocalMessage)
			{
				_addFriendService.CancelRequest(OfferId);
			}
			else
			{
				_addFriendService.Confirm(OfferId);
			}
		}

		public void OnCanceled()
		{
			if (!IsLocalMessage)
			{
				_addFriendService.Decline(OfferId);
			}
		}
	}
}
