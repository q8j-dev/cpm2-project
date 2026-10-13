using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Scopes;
using _Car_Parking.Scripts.Trade;
using _Car_Parking.Scripts.WindowSystem.Confirmation;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public class CarTradeMessageView : InteractableMessageView, IConfirmWindowSubscriber
	{
		[SerializeField]
		protected Image OfferCarImage;

		[SerializeField]
		protected Button GoToGarageButton;

		protected TradeOffer TradeOffer;

		protected TradeService TradeService;

		private MenuLevelParent _menuLevelParent;

		private OptionalDependency<MenuLevelParent> _menuLevelParentDep;

		[Inject]
		public void Construct(TradeService tradeService, OptionalDependency<MenuLevelParent> menuLevelParent)
		{
			_menuLevelParentDep = menuLevelParent;
			TradeService = tradeService;
		}

		protected override void Start()
		{
			_menuLevelParent = _menuLevelParentDep?.Value;
			base.Start();
			if (!IsLocalMessage)
			{
				GoToGarageButton.onClick.AddListener(GoToGarage);
			}
		}

		protected void OnEnable()
		{
			if (CurrentMessage == null)
			{
				return;
			}
			OfferId = CurrentMessage.EnteredText;
			TradeOffer = TradeService.GetOffer(OfferId);
			if (TradeOffer != null)
			{
				SetTradeCar();
				SetRequestedItem();
				switch (TradeOffer.State)
				{
				case TradeOfferState.Enabled:
				{
					OfferInProgressState();
					float startValue = CalculateProgress(TradeOffer.SecondUntilExpiration, 30f);
					StartProgress(TradeOffer.SecondUntilExpiration, startValue);
					break;
				}
				case TradeOfferState.Cancelled:
					OfferCancelledState();
					break;
				case TradeOfferState.Pending:
				{
					float fillAmount = CalculateProgress(TradeOffer.SecondUntilExpiration, 30f);
					ProgressBar.SetFillAmount(fillAmount);
					OfferPendingState();
					break;
				}
				case TradeOfferState.Declined:
					OfferDeclinedState();
					break;
				case TradeOfferState.Completed:
					OfferCompletedState();
					break;
				}
				TradeOffer.OnStateChangedEvent += base.ChangeMessageState;
			}
		}

		protected void OnDisable()
		{
			if (TradeOffer != null)
			{
				TradeOffer.OnStateChangedEvent -= base.ChangeMessageState;
			}
		}

		protected override void Cancel()
		{
			MainChatWindow.ShowUI(state: false);
			ManagerUI.ShowSimpleConfirmWindow(new LocalizedString("Chat.ConfirmDecline"), new LocalizedString("Chat.DeclineThisTradeOffer"), OnConfirmed, OnCanceled).Forget();
		}

		public override void OnTimeOut()
		{
		}

		protected override void ShowDetails()
		{
			MainChatWindow.OnBuyingCar(TradeOffer);
		}

		public override void OnCompleted()
		{
			if (!IsLocalMessage)
			{
				GoToGarageButton.SetActive(active: true);
			}
			OfferCompleteText();
		}

		private void SetTradeCar()
		{
			if (TradeOffer.OfferItem is TradeItemCar tradeItemCar)
			{
				Sprite sprite = Resources.Load(PathGetterHelper.GetCarPreviewPath(tradeItemCar.CarID), typeof(Sprite)) as Sprite;
				if ((bool)sprite)
				{
					OfferCarImage.sprite = sprite;
				}
				else
				{
					OfferCarImage.SetActive(active: false);
				}
			}
		}

		private void GoToGarage()
		{
			if ((bool)_menuLevelParent)
			{
				_menuLevelParent.OpenMainMenu();
			}
		}

		protected virtual void SetRequestedItem()
		{
		}

		protected virtual void OfferCompleteText()
		{
		}

		public void OnConfirmed()
		{
			TradeService.CancelOrDeclineOffer(TradeOffer.TradeID, TradeOfferCancelReason.Manual);
		}

		public void OnCanceled()
		{
			MainChatWindow.ShowUI(state: true);
		}
	}
}
