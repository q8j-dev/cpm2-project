using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.UIElement.Chat
{
	public abstract class InteractableMessageView : ChatMessageView
	{
		private const float CancelledAlpha = 0.4f;

		private static readonly Color ActiveColor = Color.green;

		private static readonly Color CancelledColor = Color.red;

		[SerializeField]
		protected bool IsLocalMessage;

		[SerializeField]
		protected GameObject CompleteSign;

		[SerializeField]
		protected TextMeshProUGUI StatusText;

		[SerializeField]
		protected CanvasGroup ContentCanvasGroup;

		[SerializeField]
		protected ProgressbarImage ProgressBar;

		[SerializeField]
		protected Button ActionButton;

		[SerializeField]
		protected RectTransform MainRect;

		[SerializeField]
		protected GameObject CancelledImage;

		protected string OfferId;

		[Inject]
		public void Construct(ManagerUI managerUI)
		{
			ManagerUI = managerUI;
		}

		protected virtual void Start()
		{
			if (IsLocalMessage)
			{
				ActionButton.onClick.AddListener(Cancel);
			}
			else
			{
				ActionButton.onClick.AddListener(ShowDetails);
			}
		}

		protected virtual void OnDestroy()
		{
			ActionButton.onClick.RemoveAllListeners();
			ProgressBar.Stop();
		}

		protected void ChangeMessageState(TradeOfferState state)
		{
			switch (state)
			{
			case TradeOfferState.Cancelled:
				OfferCancelledState();
				break;
			case TradeOfferState.Completed:
				OfferCompletedState();
				break;
			case TradeOfferState.Pending:
				ProgressBar.Pause();
				OfferPendingState();
				break;
			case TradeOfferState.Declined:
				OfferDeclinedState();
				break;
			}
		}

		protected float CalculateProgress(float timeLeft, float totalTime)
		{
			return timeLeft / totalTime * 100f / 100f;
		}

		protected void StartProgress(float duration, float startValue = 1f)
		{
			ProgressBar.StartProgress(startValue, 0f, duration, OnTimeOut);
		}

		protected void PauseProgress()
		{
			ProgressBar.Pause();
		}

		protected void ContinueProgress()
		{
			ProgressBar.Continue();
		}

		protected void StopProgress()
		{
			ProgressBar.Stop();
			ProgressBar.SetActive(active: false);
		}

		protected void OfferInProgressState()
		{
			ProgressBar.SetActive(active: true);
			StatusText.SetText(new LocalizedString("Chat.InProgress"));
			StatusText.color = new Color(255f, 165f, 0f, 255f);
			ActionButton.SetActive(active: true);
			CompleteSign.SetActive(value: false);
			ContentCanvasGroup.alpha = 1f;
			UIHelper.DelayedRebuildLayout(MainRect).Forget();
		}

		protected void OfferCancelledState()
		{
			StopProgress();
			ActionButton.SetActive(active: false);
			ContentCanvasGroup.alpha = 0.4f;
			StatusText.color = CancelledColor;
			StatusText.SetText(new LocalizedString("Chat.Canceled"));
			if (IsLocalMessage)
			{
				CancelledImage.SetActive(value: true);
			}
			UIHelper.DelayedRebuildLayout(MainRect).Forget();
		}

		protected void OfferCompletedState()
		{
			StopProgress();
			ActionButton.SetActive(active: false);
			CompleteSign.SetActive(value: true);
			StatusText.color = ActiveColor;
			StatusText.SetText(new LocalizedString("DataBase.Complete"));
			OnCompleted();
			UIHelper.DelayedRebuildLayout(MainRect).Forget();
		}

		protected void OfferDeclinedState()
		{
			StopProgress();
			ActionButton.SetActive(active: false);
			ContentCanvasGroup.alpha = 0.4f;
			StatusText.color = CancelledColor;
			StatusText.SetText(new LocalizedString("Chat.Declined"));
			if (IsLocalMessage)
			{
				CancelledImage.SetActive(value: true);
			}
			UIHelper.DelayedRebuildLayout(MainRect).Forget();
		}

		protected void OfferPendingState()
		{
			StatusText.color = ActiveColor;
			StatusText.SetText(new LocalizedString("Chat.Pending"));
			ActionButton.SetActive(active: false);
		}

		protected abstract void ShowDetails();

		protected abstract void Cancel();

		public abstract void OnTimeOut();

		public virtual void OnCompleted()
		{
		}
	}
}
