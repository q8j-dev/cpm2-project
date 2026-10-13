using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.BannerSystem;
using _Car_Parking.Scripts.DeepLink;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.UIElement;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.EventSystem
{
	public class BannerShowController : MonoBehaviour, IDeepLinkConsumer
	{
		private IEventService _eventService;

		private IBannerService _bannerService;

		private ManagerUI _managerUI;

		private DeepLinkProvider _deepLinkProvider;

		private IOfferSystem _offerSystem;

		private List<_Car_Parking.Scripts.EventSystem.Data.Event> _eventsForShow;

		private bool _wasInitialize;

		private bool _wasProvideOffer;

		[Inject]
		public void Construct(IEventService eventService, IBannerService bannerService, ManagerUI managerUI, DeepLinkProvider deepLinkProvider, IOfferSystem offerSystem)
		{
			_eventService = eventService;
			_bannerService = bannerService;
			_managerUI = managerUI;
			_deepLinkProvider = deepLinkProvider;
			_offerSystem = offerSystem;
		}

		public void Initialize()
		{
			_wasInitialize = true;
			_deepLinkProvider.Subscribe(this);
			_deepLinkProvider.RequestDeepLink();
			_bannerService.ProvideShowBanner(BannerType.Event, () =>
			{
				Show(false).Forget();
			}, base.destroyCancellationToken);
			TryToProvideOfferBanners();
		}

		public bool TryConsumeDeepLink(DeepLinkData linkData)
		{
			if (linkData.Path == "/event" && linkData.TryGetParam("name", out var _))
			{
				Show(byLink: true).Forget();
				return true;
			}
			return false;
		}

		private async UniTask Show(bool byLink = false)
		{
			List<_Car_Parking.Scripts.EventSystem.Data.Event> list = ((!byLink) ? (await _eventService.CheckForUncachedEvents()) : (await _eventService.RetrieveCurrentEvents()));
			List<_Car_Parking.Scripts.EventSystem.Data.Event> list2 = list;
			if (!base.destroyCancellationToken.IsCancellationRequested && list2 != null && list2.Count != 0 && _managerUI.IsMainMenuActive())
			{
				_eventService.CacheEventByName(list2[0].Type);
				Show(list2);
			}
		}

		private void Show(List<_Car_Parking.Scripts.EventSystem.Data.Event> eventsForShow = null)
		{
			_managerUI.CloseWindow("UserBalanceWindow");
			_managerUI.CloseWindow("MainMenuWindow");
			_managerUI.ShowEventViewWindow(eventsForShow[0].Type);
		}

		private void OnEnable()
		{
			_offerSystem.OnDataUpdatedEvent += OffersDataUpdatedCallback;
		}

		private void OnDisable()
		{
			_offerSystem.OnDataUpdatedEvent -= OffersDataUpdatedCallback;
		}

		private void OnDestroy()
		{
			_deepLinkProvider.Unsubscribe(this);
		}

		private void OffersDataUpdatedCallback()
		{
			TryToProvideOfferBanners();
		}

		private void TryToProvideOfferBanners()
		{
			if (_wasInitialize && _offerSystem.Offers != null && _offerSystem.Offers.Length != 0 && _offerSystem.HaveValidOffers && _offerSystem.IsDataReady && !_wasProvideOffer)
			{
				_wasProvideOffer = true;
				_bannerService.ProvideShowBanner(BannerType.Offer, ShowOfferWindow, base.destroyCancellationToken);
			}
		}

		private void ShowOfferWindow()
		{
			OfferModel offerForBigBanner = _offerSystem.GetOfferForBigBanner();
			if (offerForBigBanner != null && _managerUI.IsMainMenuActive())
			{
				_managerUI.CloseWindow("UserBalanceWindow");
				EventViewWindow.AdditionalData value = new EventViewWindow.AdditionalData(new IBigBannerView.BannerData(offerForBigBanner));
				_managerUI.ShowWindow("EventViewWindow", alwaysOnTop: false, show: true, null, fadeBlack: false, enableBackButton: false, cleanScreen: true, JsonConvert.SerializeObject(value)).Forget();
			}
		}
	}
}
