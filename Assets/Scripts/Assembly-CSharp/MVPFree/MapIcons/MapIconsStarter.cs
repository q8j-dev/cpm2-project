using System;
using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Presenters;
using MVPFree.MapIcons.Updaters;
using MVPFree.MapIcons.Views;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.MVPFree.MapIcons.Presenters;

namespace MVPFree.MapIcons
{
	public class MapIconsStarter : MvpStarter
	{
		[Inject]
		private IPrefabFactory _factory;

		[Inject]
		private MapIconsModel _mapIconsModel;

		[field: SerializeField]
		public IconsDescriptionView IconsDescriptionView { get; private set; }

		[field: SerializeField]
		public List<Transform> GaragesIconsPoint { get; private set; }

		[field: SerializeField]
		public List<Transform> RampPoints { get; private set; }

		[field: SerializeField]
		public EventZoneCollectiblesPosDatas EventZoneCollectiblesPosDatas { get; private set; }

		private void Awake()
		{
			presenters = new List<IPresenter>
			{
				_factory.Create<InitMapIconsPresenter>(Array.Empty<object>()),
				_factory.Create<StartCheckNearIconsPresenter>(Array.Empty<object>()),
				_factory.Create<CreateMapIconPresenter>(Array.Empty<object>()),
				_factory.Create<UpdateIconPresenter>(Array.Empty<object>()),
				_factory.Create<CreateIconsDescriptionPresenter>(Array.Empty<object>()),
				_factory.Create<UpdateIconsDescriptionPresenter>(Array.Empty<object>()),
				_factory.Create<OpenCloseIconDescriptionItemsPresenter>(Array.Empty<object>()),
				_factory.Create<ResetIconsDescriptionScrollViewPresenter>(Array.Empty<object>()),
				_factory.Create<SwitchIconsRotationPresenter>(Array.Empty<object>()),
				_factory.Create<CarMapIconPresenter>(Array.Empty<object>()),
				_factory.Create<EnableDisableCarIconPresenter>(Array.Empty<object>()),
				_factory.Create<ScrollDetectPresenter>(Array.Empty<object>()),
				_factory.Create<ResetFadeIconsPresenter>(Array.Empty<object>()),
				_factory.Create<MapIconShowHidePresenter>(Array.Empty<object>()),
				_factory.Create<SelectIconPresenter>(Array.Empty<object>()),
				_factory.Create<ClickToMapIconPresenter>(Array.Empty<object>()),
				_factory.Create<ReticleSelectMapIconPresenter>(Array.Empty<object>()),
				_factory.Create<RaceIconsPresenter>(Array.Empty<object>()),
				_factory.Create<ClickSnowMenIconPresenter>(Array.Empty<object>()),
				_factory.Create<ArenaIconClickPresenter>(Array.Empty<object>()),
				_factory.Create<PropertyIconsPresenter>(Array.Empty<object>())
			};
			updaters = new List<IUpdater>
			{
				_factory.Create<RotateIconOnMiniMapUpdater>(Array.Empty<object>()),
				_factory.Create<MapIconAlphaUpdater>(Array.Empty<object>())
			};
		}

		private void Start()
		{
			_mapIconsModel.Init(IconsDescriptionView);
		}
	}
}
