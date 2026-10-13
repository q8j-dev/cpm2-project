using System.Collections.Generic;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using UniRx;
using VContainer;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.Localization;

namespace MVPFree.MapIcons.Presenters
{
	public class MapIconShowHidePresenter : IPresenter
	{
		private MapIconsModel _mapIconsModel;

		private MapIconsStarter _mapIconsStarter;

		private MapIconsDataSo _mapIconsDataSo;

		private TimedActivityZoneController _timedActivityZoneController;

		private bool _isBigMap;

		private IconStatusModel IconStatusModel => _mapIconsModel.IconStatusModel;

		[Inject]
		public MapIconShowHidePresenter(MapIconsModel mapIconsModel, MapIconsStarter mapIconsStarter, MapIconsDataSo mapIconsDataSo, TimedActivityZoneController timedActivityZoneController)
		{
			_mapIconsModel = mapIconsModel;
			_mapIconsStarter = mapIconsStarter;
			_mapIconsDataSo = mapIconsDataSo;
			_timedActivityZoneController = timedActivityZoneController;
		}

		public void Subscribe()
		{
			_mapIconsModel.Initialized += InitializedCallback;
			_mapIconsModel.OnMapIconsUpdated += MapIconsUpdatedCallback;
			_mapIconsModel.OnOpenedBigMap += OpenedBigMapCallback;
			_mapIconsModel.OnOpenedServiceMap += OpenedServiceMapCallback;
			_mapIconsStarter.IconsDescriptionView.HideShowAllButton.onClick.AddListener(HideShowAllButtonClickedCallback);
			IconStatusModel.StatusUpdated += VisibleStatusUpdatedCallback;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.Initialized -= InitializedCallback;
			_mapIconsModel.OnMapIconsUpdated -= MapIconsUpdatedCallback;
			_mapIconsModel.OnOpenedBigMap -= OpenedBigMapCallback;
			_mapIconsModel.OnOpenedServiceMap -= OpenedServiceMapCallback;
			_mapIconsStarter.IconsDescriptionView.HideShowAllButton.onClick.RemoveListener(HideShowAllButtonClickedCallback);
			IconStatusModel.StatusUpdated -= VisibleStatusUpdatedCallback;
			foreach (KeyValuePair<IconType, IconDescriptionItemView> iconDescriptionItem in _mapIconsStarter.IconsDescriptionView.IconDescriptionItems)
			{
				iconDescriptionItem.Value.VisibleButtonClicked -= VisibleButtonClickedCallback;
			}
		}

		private void VisibleStatusUpdatedCallback()
		{
			UpdateViews();
		}

		private void OpenedBigMapCallback(bool value)
		{
			_isBigMap = value;
			if (value)
			{
				UpdateViews();
			}
			else
			{
				ShowAllViews();
			}
		}

		private void MapIconsUpdatedCallback()
		{
			if (_isBigMap)
			{
				UpdateViews();
			}
		}

		private void OpenedServiceMapCallback(bool value)
		{
			ShowAllViews();
		}

		private void InitializedCallback()
		{
			foreach (KeyValuePair<IconType, IconDescriptionItemView> iconDescriptionItem in _mapIconsStarter.IconsDescriptionView.IconDescriptionItems)
			{
				iconDescriptionItem.Value.VisibleButtonClicked += VisibleButtonClickedCallback;
			}
			Observable.NextFrame().Subscribe((Unit _) =>
			{
				UpdateViews();
			}).AddTo(_mapIconsStarter);
		}

		private void HideShowAllButtonClickedCallback()
		{
			IconStatusModel.SetAllVisible(IconStatusModel.AllIconsHided);
		}

		private void VisibleButtonClickedCallback(IconDescriptionItemView iconDescriptionItemView)
		{
			IconStatusModel.RevertIconVisible(_mapIconsStarter.IconsDescriptionView.GetIconType(iconDescriptionItemView));
		}

		private void UpdateViews()
		{
			for (int i = 0; i < _mapIconsModel.MapIconModels.Count; i++)
			{
				IconMapView iconMapView = _mapIconsModel.MapIconModels[i].IconMapView;
				if (!iconMapView)
				{
					continue;
				}
				if (_mapIconsDataSo.CurrentEvents.Contains(iconMapView.EventType) && _timedActivityZoneController != null && _timedActivityZoneController.AllEventActivityModel.TryGetValue(iconMapView.Index, out var value))
				{
					EventZoneMapElement mapZone = value.MapZone;
					if ((bool)mapZone)
					{
						mapZone.SwitchMapZoneVisible(IconStatusModel.GetIconVisibleStatus(_mapIconsModel.MapIconModels[i].IconType));
					}
				}
				else
				{
					_mapIconsModel.MapIconModels[i].IconMapView.UpdateRootVisible(IconStatusModel.GetIconVisibleStatus(_mapIconsModel.MapIconModels[i].IconType));
				}
			}
			if ((bool)_mapIconsModel.PlayerIcon)
			{
				_mapIconsModel.PlayerIcon.UpdateRootVisible(IconStatusModel.GetIconVisibleStatus(IconType.Player));
			}
			foreach (KeyValuePair<IconType, IconDescriptionItemView> iconDescriptionItem in _mapIconsStarter.IconsDescriptionView.IconDescriptionItems)
			{
				iconDescriptionItem.Value.VisibleImage.sprite = (IconStatusModel.GetIconVisibleStatus(iconDescriptionItem.Key) ? _mapIconsDataSo.IconShow : _mapIconsDataSo.IconHide);
			}
			if (IconStatusModel.AllIconsHided)
			{
				_mapIconsStarter.IconsDescriptionView.UpdateShowHideView(_mapIconsDataSo.IconShow, new LocalizedString("Map.ShowAll"));
			}
			else
			{
				_mapIconsStarter.IconsDescriptionView.UpdateShowHideView(_mapIconsDataSo.IconHide, new LocalizedString("Map.HideAll"));
			}
		}

		private void ShowAllViews()
		{
			for (int i = 0; i < _mapIconsModel.MapIconModels.Count; i++)
			{
				if ((bool)_mapIconsModel.MapIconModels[i].IconMapView)
				{
					_mapIconsModel.MapIconModels[i].IconMapView.UpdateRootVisible(visible: true);
				}
			}
			if ((bool)_mapIconsModel.PlayerIcon)
			{
				_mapIconsModel.PlayerIcon.UpdateRootVisible(visible: true);
			}
		}
	}
}
