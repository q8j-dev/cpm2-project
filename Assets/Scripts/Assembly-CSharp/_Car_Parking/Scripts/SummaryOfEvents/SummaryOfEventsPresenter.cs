using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CPM.Account;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Infrastructure.Start;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.PurchaseSystem;
using _Car_Parking.Scripts.Steam;
using _Car_Parking.Scripts.SummaryOfEvents.ItemViews.Event;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.SummaryOfEvents
{
	[Preserve]
	public class SummaryOfEventsPresenter : IDisposable, IInitializable
	{
		private const float ConfirmWindowBackgroundAlpha = 0.925f;

		private readonly SummaryOfEventsView _summaryOfEventsView;

		private readonly IOfferSystem _offerSystem;

		private readonly SummaryOfEventsConfig _config;

		private readonly ManagerUI _managerUI;

		private readonly IAccountController _accountController;

		private readonly AutoScrollController _autoScrollController;

		private readonly IPurchaseSystem _purchaseSystem;

		private readonly IPrefabFactory _prefabFactory;

		private readonly IEventService _eventService;

		private readonly ServerTimeService _serverTimeService;

		private CancellationTokenSource _cancellationTokenSourceInitItems;

		private CancellationTokenSource _cancellationTokenSourceCheckOffersTime;

		private bool OffersIsReady
		{
			get
			{
				if (_purchaseSystem.IsInitialized)
				{
					return _offerSystem.IsDataReady;
				}
				return false;
			}
		}

		public SummaryOfEventsPresenter(SummaryOfEventsView view, IOfferSystem offerSystem, SummaryOfEventsConfig config, ManagerUI managerUI, IAccountController accountController, IPurchaseSystem purchaseSystem, IPrefabFactory prefabFactory, IEventService eventService, ServerTimeService serverTimeService)
		{
			_summaryOfEventsView = view;
			_offerSystem = offerSystem;
			_config = config;
			_managerUI = managerUI;
			_accountController = accountController;
			_purchaseSystem = purchaseSystem;
			_prefabFactory = prefabFactory;
			_autoScrollController = new AutoScrollController(_summaryOfEventsView.ScrollSnap, _config.DelayToAutoScroll);
			_eventService = eventService;
			_serverTimeService = serverTimeService;
			_offerSystem.OnDataUpdatedEvent += OfferSystemDataUpdatedCallback;
			_purchaseSystem.OnInitializedCompletedEvent += PurchaseSystemInitializedCallback;
			_eventService.OnEventsRetrieved += EventsRetrievedCallback;
			_summaryOfEventsView.ScrollSnap.OnSelectionChangeStartEvent.AddListener(ScrollSelectionChangeStartCallback);
			_summaryOfEventsView.ScrollSnap.OnSelectionChangeEndEvent.AddListener(ScrollSelectionChangeEndCallback);
		}

		public void Initialize()
		{
			InitItems();
		}

		public void Dispose()
		{
			_offerSystem.OnDataUpdatedEvent -= OfferSystemDataUpdatedCallback;
			_purchaseSystem.OnInitializedCompletedEvent -= PurchaseSystemInitializedCallback;
			_eventService.OnEventsRetrieved -= EventsRetrievedCallback;
			_summaryOfEventsView.ScrollSnap.OnSelectionChangeStartEvent.RemoveListener(ScrollSelectionChangeStartCallback);
			_summaryOfEventsView.ScrollSnap.OnSelectionChangeEndEvent.RemoveListener(ScrollSelectionChangeEndCallback);
			_autoScrollController.Dispose();
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceInitItems);
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheckOffersTime);
		}

		private void OfferSystemDataUpdatedCallback()
		{
			if (OffersIsReady)
			{
				InitItems();
			}
		}

		private void PurchaseSystemInitializedCallback()
		{
			if (OffersIsReady)
			{
				InitItems();
			}
		}

		private void EventsRetrievedCallback()
		{
			if (OffersIsReady)
			{
				InitItems();
			}
		}

		private void ScrollSelectionChangeStartCallback()
		{
			_autoScrollController.Stop();
		}

		private void ScrollSelectionChangeEndCallback(int page)
		{
			_autoScrollController.Restart();
		}

		private void InitItems()
		{
			CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceInitItems);
			_cancellationTokenSourceInitItems = new CancellationTokenSource();
			InitItemsAsync(_cancellationTokenSourceInitItems.Token).Forget();
		}

		private async UniTaskVoid InitItemsAsync(CancellationToken cancellationToken)
		{
			while (!_summaryOfEventsView.ScrollSnap.IsInitialized)
			{
				_summaryOfEventsView.UpdateRootVisible(isVisible: true);
				await UniTask.Yield();
				if (cancellationToken.IsCancellationRequested)
				{
					return;
				}
			}
			_autoScrollController.Stop();
			_summaryOfEventsView.InfiniteScroll.Disable();
			_summaryOfEventsView.ScrollSnap.enabled = false;
			_summaryOfEventsView.ContentTransform.RemoveAllChild();
			if (_summaryOfEventsView.ContentTransform.TryGetComponent<RectTransform>(out var component))
			{
				component.anchoredPosition = Vector2.zero;
			}
			await UniTask.Yield();
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			List<_Car_Parking.Scripts.EventSystem.Data.Event> list = await _eventService.RetrieveCurrentEvents();
			if (cancellationToken.IsCancellationRequested)
			{
				return;
			}
			if (list != null && !list.IsNullOrEmptyCollection())
			{
				foreach (_Car_Parking.Scripts.EventSystem.Data.Event @event in list)
				{
					ItemEvent itemEvent = _config.ItemEventPrefab.FirstOrDefault((ItemEvent x) => x.EventType == @event.Type);
					if ((bool)itemEvent)
					{
						InstanceGenericItem(itemEvent, (ItemEvent item) =>
						{
							_managerUI.ShowEventViewWindow(@event.Type);
						}).Initialize(@event, _serverTimeService);
					}
				}
			}
			List<OfferModel> activeOffers = _offerSystem.GetOffersForBanners();
			for (int num = 0; num < activeOffers.Count; num++)
			{
				InstanceItem(_config.GetItemOfferPrefab(activeOffers[num].Skin.SmallPrefab), ItemOfferClickCallback).GetComponent<OfferViewBase>().Init(activeOffers[num]);
			}
			OtherSmallBannerItemBase[] otherUpdateItemPrefabs = _config.OtherUpdateItemPrefabs;
			for (int num2 = 0; num2 < otherUpdateItemPrefabs.Length; num2++)
			{
				if (!(otherUpdateItemPrefabs[num2] == null))
				{
					CreatePrefab(otherUpdateItemPrefabs[num2]).Init(num2);
				}
			}
			if (!_accountController.HasEmail || AccountSystemV2Extension.IsGuestAccount(_accountController.Email))
			{
				InstanceItem(_config.ItemLogInPrefab, (SummaryOfEventItem item) =>
				{
					UpdateWindowShowUI(state: false);
					ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(new Color(0f, 0f, 0f, 0.925f));
					_ = _managerUI.ShowConfirmWindow(() =>
					{
						UpdateWindowShowUI(state: true);
						AuthorizationWindow.AdditionalData value = new AuthorizationWindow.AdditionalData
						{
							BackWindowKey = WindowScriptType.MainMenuWindow.ToString()
						};
						_ = _managerUI.ShowWindow(WindowScriptType.AuthorizationWindow.ToString(), alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: true, cleanScreen: true, JsonConvert.SerializeObject(value));
					}, new LocalizedString("DailyRewards.SignInToGetRewards.Title"), new LocalizedString("DailyRewards.SignInToGetRewards.Content"), () =>
					{
						UpdateWindowShowUI(state: true);
					}, "", null, additionalData);
				});
			}
			if (SteamManager.IsInitialized() && !ManagerKeysForPrefs.IsSteamLinked && (bool)_config.ItemSteamConnectPrefab)
			{
				InstanceItem(_config.ItemSteamConnectPrefab, (SummaryOfEventItem item) =>
				{
					_managerUI.ShowSettingsWindow("Account");
				});
			}
			int itemCount = _summaryOfEventsView.ContentTransform.childCount;
			_summaryOfEventsView.TabIconsController.Init((itemCount > 1) ? itemCount : 0);
			_summaryOfEventsView.UpdateRootVisible(isVisible: false);
			await UniTask.Yield();
			if (!cancellationToken.IsCancellationRequested)
			{
				_summaryOfEventsView.UpdateRootVisible(itemCount > 0);
				_summaryOfEventsView.ScrollSnap.enabled = true;
				_summaryOfEventsView.InfiniteScroll.enabled = false;
				_summaryOfEventsView.InfiniteScroll.Init();
				if (itemCount > 0)
				{
					_summaryOfEventsView.ScrollSnap.GoToScreen(0);
				}
				_summaryOfEventsView.ScrollRect.horizontal = itemCount > 1;
				_autoScrollController.Restart();
				CommonExtensions.CancelAndDisposeToken(ref _cancellationTokenSourceCheckOffersTime);
				if (activeOffers.Count > 0)
				{
					_cancellationTokenSourceCheckOffersTime = new CancellationTokenSource();
					CheckOfferTimesInitItemsAsync(activeOffers, _cancellationTokenSourceCheckOffersTime.Token).Forget();
				}
			}
		}

		private SummaryOfEventItem InstanceItem(SummaryOfEventItem prefab, Action<SummaryOfEventItem> callback)
		{
			SummaryOfEventItem summaryOfEventItem = CreatePrefab(prefab);
			summaryOfEventItem.OnClickEvent += callback;
			return summaryOfEventItem;
		}

		private T InstanceGenericItem<T>(T prefab, Action<T> callback) where T : SummaryOfEventItem
		{
			T item = CreatePrefab(prefab);
			item.OnClickEvent += (SummaryOfEventItem eventItem) =>
			{
				callback?.Invoke(item);
			};
			return item;
		}

		private T CreatePrefab<T>(T prefab) where T : MonoBehaviour
		{
			T val = _prefabFactory.Instantiate(prefab, _summaryOfEventsView.ContentTransform.position, _summaryOfEventsView.ContentTransform.rotation, _summaryOfEventsView.ContentTransform);
			RectTransform component = val.GetComponent<RectTransform>();
			component.anchoredPosition = new Vector2(component.rect.width * (float)(_summaryOfEventsView.ContentTransform.childCount - 1), component.anchoredPosition.y);
			component.transform.localScale = Vector3.one;
			return val;
		}

		private void UpdateWindowShowUI(bool state)
		{
			WindowScript componentInParent = _summaryOfEventsView.GetComponentInParent<WindowScript>();
			if ((bool)componentInParent)
			{
				componentInParent.ShowUI(state);
			}
		}

		private void ItemOfferClickCallback(SummaryOfEventItem item)
		{
			if (item.TryGetComponent<OfferViewBase>(out var component) && component.OfferId >= 0)
			{
				OfferModel offer = _offerSystem.GetOffer(component.OfferId);
				if (offer != null)
				{
					EventViewWindow.AdditionalData value = new EventViewWindow.AdditionalData(new IBigBannerView.BannerData(offer));
					_managerUI.ShowWindow("EventViewWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, JsonConvert.SerializeObject(value)).Forget();
				}
			}
		}

		private async UniTaskVoid CheckOfferTimesInitItemsAsync(List<OfferModel> offers, CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				for (int i = 0; i < offers.Count; i++)
				{
					if (!offers[i].IsValid)
					{
						InitItems();
						return;
					}
				}
				await UniTask.Delay(TimeSpan.FromSeconds(1.0), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
		}
	}
}
