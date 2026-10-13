using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Chat;
using _Car_Parking.Scripts.Trade;
using _Car_Parking.Scripts.WindowSystem.Confirmation;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class SendMoneyMessageView : InteractableMessageView, IConfirmWindowSubscriber
	{
		[SerializeField]
		private TextMeshProUGUI MoneyText;

		private MoneySendService _moneySendService;

		private ChatOffer _offer;

		[Inject]
		private void AdditionalConstruct(MoneySendService moneySendService)
		{
			_moneySendService = moneySendService;
		}

		protected void OnEnable()
		{
			if (CurrentMessage != null)
			{
				string[] array = CurrentMessage.EnteredText.Split("?"[0]);
				OfferId = array[1];
				if (int.TryParse(array[0], out var result))
				{
					result = Mathf.Clamp(result, 0, 500000);
					SetMoney(result);
				}
				_offer = _moneySendService.GetOffer(OfferId);
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

		public void SetMoney(int money)
		{
			MoneyText.SetText(TextExtensions.IntFormatThousandSpace(money));
		}

		protected override void ShowDetails()
		{
			PauseProgress();
			_moneySendService.SetPending(OfferId);
			string format = new LocalizedString("Chat.ThisPlayerWantsToSendYouMoney");
			ManagerUI.ShowSimpleConfirmWindow(new LocalizedString("Chat.ConfirmTransaction"), string.Format(format, MoneyText.text), OnConfirmed, OnCanceled).Forget();
		}

		protected override void Cancel()
		{
			ManagerUI.ShowSimpleConfirmWindow(new LocalizedString("Chat.ConfirmDecline"), new LocalizedString("Chat.DeclineThisMoneyOffer"), OnConfirmed, OnCanceled).Forget();
		}

		public override void OnTimeOut()
		{
		}

		public void OnConfirmed()
		{
			if (IsLocalMessage)
			{
				_moneySendService.CancelRequest(OfferId);
			}
			else
			{
				_moneySendService.Confirm(OfferId);
			}
		}

		public void OnCanceled()
		{
			if (IsLocalMessage)
			{
				ContinueProgress();
			}
			else
			{
				_moneySendService.Decline(OfferId);
			}
		}
	}
}
