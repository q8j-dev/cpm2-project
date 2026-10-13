using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.Data.Response;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.GameActivity;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class DetailEventView : MonoBehaviour
	{
		[SerializeField]
		private RemainingTimeView TimeView;

		[SerializeField]
		private Button ClaimButton;

		[SerializeField]
		private GameObject ClaimButtonDisabled;

		[SerializeField]
		private GameObject ClaimButtonFinished;

		protected GameActivityEntry _entry;

		protected _Car_Parking.Scripts.EventSystem.Data.Event _event;

		private CancellationTokenSource _refreshCts;

		private IEventService _eventService;

		private bool _claimProcess;

		[Inject]
		public void Construct(IEventService eventService)
		{
			_eventService = eventService;
		}

		protected virtual void OnEnable()
		{
			ClaimButton.onClick.AddListener(OnClaimButtonClicked);
		}

		protected virtual void OnDisable()
		{
			ClaimButton.onClick.RemoveListener(OnClaimButtonClicked);
		}

		public void Init(GameActivityEntry entry)
		{
			_entry = entry;
			if (EventActivityMap.TryGetEvent(entry.ActivityType, out var eventType))
			{
				RefreshEvents(eventType).Forget();
			}
		}

		protected virtual void OnRender()
		{
		}

		private async UniTaskVoid RefreshEvents(_Car_Parking.Scripts.EventSystem.EventType eventType)
		{
			CommonExtensions.CancelAndDisposeToken(ref _refreshCts);
			_refreshCts = CancellationTokenSource.CreateLinkedTokenSource(base.destroyCancellationToken);
			CancellationToken token = _refreshCts.Token;
			List<_Car_Parking.Scripts.EventSystem.Data.Event> list = await _eventService.RetrieveCurrentEvents();
			if (token.IsCancellationRequested)
			{
				return;
			}
			foreach (_Car_Parking.Scripts.EventSystem.Data.Event item in list)
			{
				if (item != null && item.Type == eventType)
				{
					_event = item;
					Render();
					break;
				}
			}
		}

		private void Render()
		{
			if (_event != null)
			{
				base.gameObject.SetActive(value: true);
				if ((bool)TimeView)
				{
					TimeView.Init(_event.EndTime);
				}
				if ((bool)ClaimButton)
				{
					ClaimButton.interactable = _event.Status == EventStatus.Finished;
				}
				if ((bool)ClaimButtonDisabled)
				{
					ClaimButtonDisabled.SetActive(_event.Status != EventStatus.Finished);
				}
				if ((bool)ClaimButtonFinished)
				{
					ClaimButtonFinished.SetActive(_event.Status == EventStatus.Finished);
				}
				OnRender();
			}
		}

		private void OnClaimButtonClicked()
		{
			if (_event != null && _event.Status == EventStatus.Finished)
			{
				ClaimRewardAsync(base.destroyCancellationToken).Forget();
			}
		}

		private async UniTask ClaimRewardAsync(CancellationToken cancellationToken)
		{
			if (_claimProcess)
			{
				return;
			}
			try
			{
				_claimProcess = true;
				ClaimRewardResult claimRewardResult = await _eventService.ClaimReward(_event.Type);
				if (!cancellationToken.IsCancellationRequested && claimRewardResult.Success)
				{
					_event.Status = EventStatus.Claimed;
					Render();
				}
			}
			finally
			{
				_claimProcess = false;
			}
		}
	}
}
