using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI.Extensions;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.EventSystem.View
{
	[Serializable]
	public class SnapController : IDisposable
	{
		[SerializeField]
		private HorizontalScrollSnap HorizontalScrollSnap = default;

		[SerializeField]
		private UI_InfiniteScroll InfiniteScroll = default;

		[SerializeField]
		private TabIconsController TabIconsController = default;

		[SerializeField]
		private List<GameObject> EventViewPrefabsV2 = default;

		[SerializeField]
		private Transform ContentTransform = default;

		[SerializeField]
		private List<GameObject> NotEventViewPrefabs = default;

		private IPrefabFactory _prefabFactory;

		private IEventService _eventService;

		private IOfferSystem _offerSystem;

		public void Inject(IEventService eventService, IPrefabFactory prefabFactory, IOfferSystem offerSystem)
		{
			_eventService = eventService;
			_prefabFactory = prefabFactory;
			_offerSystem = offerSystem;
			HorizontalScrollSnap.OnSelectionPageChangedEvent.AddListener(SelectionPageChangedCallback);
		}

		public void Dispose()
		{
			HorizontalScrollSnap.OnSelectionPageChangedEvent.RemoveListener(SelectionPageChangedCallback);
		}

		private void Test(int dummy)
		{
			GameObject[] childObjects = HorizontalScrollSnap.ChildObjects;
			for (int i = 0; i < childObjects.Length; i++)
			{
				childObjects[i].SetActive(value: false);
			}
			HorizontalScrollSnap.GoToScreen(dummy, pagination: false, instant: true);
			childObjects = HorizontalScrollSnap.ChildObjects;
			for (int i = 0; i < childObjects.Length; i++)
			{
				childObjects[i].SetActive(value: true);
			}
		}

		public async UniTask Initialize(IBigBannerView.BannerData bannerData, bool singleOfferOnly = false)
		{
			while (!HorizontalScrollSnap.IsInitialized)
			{
				await UniTask.Yield();
			}
			await UniTask.Delay(1);
			if (!ContentTransform)
			{
				return;
			}
			bool flag = false;
			if (singleOfferOnly && bannerData != null && bannerData.OfferID >= 0)
			{
				int childCount = ContentTransform.childCount;
				CreateBanner(bannerData);
				flag = ContentTransform.childCount > childCount;
			}
			if (!flag)
			{
				List<_Car_Parking.Scripts.EventSystem.Data.Event> list = await _eventService.RetrieveCurrentEvents();
				if (!ContentTransform)
				{
					return;
				}
				foreach (_Car_Parking.Scripts.EventSystem.Data.Event item in list)
				{
					CreateBanner(new IBigBannerView.BannerData(item.Type));
				}
				foreach (OfferModel offersForBanner in _offerSystem.GetOffersForBanners())
				{
					CreateBanner(new IBigBannerView.BannerData(offersForBanner));
				}
				for (int i = 0; i < NotEventViewPrefabs.Count; i++)
				{
					CreateBanner(i);
				}
			}
			foreach (GameObject item2 in ContentTransform.Children())
			{
				item2.SetActive(value: false);
			}
			HorizontalScrollSnap.enabled = true;
			InfiniteScroll.enabled = false;
			InfiniteScroll.Init();
			int initialIndex = 0;
			for (int j = 0; j < HorizontalScrollSnap.ChildObjects.Length; j++)
			{
				GameObject gameObject = HorizontalScrollSnap.ChildObjects[j];
				if ((bool)gameObject && gameObject.TryGetComponent<IBigBannerView>(out var component) && component.IsCorrectPrefab(bannerData))
				{
					initialIndex = j;
				}
			}
			TabIconsController.Init((HorizontalScrollSnap.ChildObjects.Length > 1) ? HorizontalScrollSnap.ChildObjects.Length : 0);
			await UniTask.Delay(1);
			HorizontalScrollSnap.GoToScreen(initialIndex, pagination: false, instant: true);
			foreach (GameObject item3 in ContentTransform.Children())
			{
				item3.SetActive(value: true);
			}
			if (initialIndex == 0)
			{
				SelectionPageChangedCallback(0);
			}
		}

		private void CreateBanner(IBigBannerView.BannerData bannerData)
		{
			GameObject gameObject = null;
			foreach (GameObject item in EventViewPrefabsV2)
			{
				if ((bool)item && item.TryGetComponent<IBigBannerView>(out var component) && component.IsCorrectPrefab(bannerData))
				{
					gameObject = item;
					break;
				}
			}
			if ((bool)gameObject)
			{
				GameObject gameObject2 = _prefabFactory.Instantiate(gameObject, ContentTransform.position, ContentTransform.rotation, ContentTransform);
				gameObject2.name = gameObject.name;
				gameObject2.GetComponent<IBigBannerView>().InitBanner(bannerData);
			}
		}

		private void CreateBanner(int index)
		{
			GameObject gameObject = NotEventViewPrefabs[index];
			if ((bool)gameObject)
			{
				GameObject gameObject2 = _prefabFactory.Instantiate(gameObject, ContentTransform.position, ContentTransform.rotation, ContentTransform);
				gameObject2.name = gameObject.name;
				gameObject2.GetComponent<IBigBannerView>().InitBanner(new IBigBannerView.BannerData(index));
			}
		}

		private void SelectionPageChangedCallback(int page)
		{
			if (page < HorizontalScrollSnap.ChildObjects.Length)
			{
				GameObject gameObject = HorizontalScrollSnap.ChildObjects[page];
				if ((bool)gameObject && gameObject.TryGetComponent<IBigBannerView>(out var component))
				{
					component.Shown();
				}
			}
		}
	}
}
