using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EventSystem.View;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.UIElement;

namespace _Car_Parking.Scripts.WindowSystem
{
	public class SingleEventViewWindow : WindowScript
	{
		[SerializeField]
		private Button CloseButton;

		[SerializeField]
		private List<GameObject> EventViewPrefabsV2;

		[SerializeField]
		private Transform Parent;

		private IPrefabFactory _prefabFactory;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory)
		{
			_prefabFactory = prefabFactory;
		}

		protected override void Start()
		{
			base.Start();
			CloseButton.onClick.AddListener(Close);
		}

		public override void SetAdditionalData(string data)
		{
			base.SetAdditionalData(data);
			Debug.Log(data);
			CreateBanner(JsonConvert.DeserializeObject<IBigBannerView.BannerData>(data));
		}

		private void CreateBanner(IBigBannerView.BannerData bannerData)
		{
			if (bannerData == null)
			{
				return;
			}
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
				GameObject obj = _prefabFactory.Instantiate(gameObject, Parent);
				obj.name = gameObject.name;
				obj.GetComponent<IBigBannerView>().InitBanner(bannerData);
			}
		}

		protected override void Awake()
		{
			base.Awake();
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ShortNotifySwitchState, base.gameObject);
		}

		protected override void OnEnable()
		{
			EventView.Claimed = (Action)Delegate.Combine(EventView.Claimed, new Action(CloseEvents));
			EventView.Closed = (Action)Delegate.Combine(EventView.Closed, new Action(CloseEvents));
			base.OnEnable();
		}

		private void OnDisable()
		{
			EventView.Claimed = (Action)Delegate.Remove(EventView.Claimed, new Action(CloseEvents));
			EventView.Closed = (Action)Delegate.Remove(EventView.Closed, new Action(CloseEvents));
		}

		private void CloseEvents()
		{
			Close();
		}
	}
}
