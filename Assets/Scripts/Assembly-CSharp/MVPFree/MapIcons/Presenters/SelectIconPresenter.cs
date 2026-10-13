using System.Collections.Generic;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace MVPFree.MapIcons.Presenters
{
	public class SelectIconPresenter : IPresenter
	{
		private MapIconsModel _mapIconsModel;

		private MapIconsStarter _mapIconsStarter;

		private MapIconsDataSo _mapIconsDataSo;

		private FreeDriveProvider _freeDriveProvider;

		private IconStatusModel IconStatusModel => _mapIconsModel.IconStatusModel;

		[Inject]
		public SelectIconPresenter(MapIconsModel mapIconsModel, MapIconsStarter mapIconsStarter, MapIconsDataSo mapIconsDataSo, FreeDriveProvider freeDriveProvider)
		{
			_mapIconsModel = mapIconsModel;
			_mapIconsStarter = mapIconsStarter;
			_mapIconsDataSo = mapIconsDataSo;
			_freeDriveProvider = freeDriveProvider;
		}

		public void Subscribe()
		{
			_mapIconsModel.Initialized += InitializedCallback;
			_mapIconsModel.OnStateUpdated += MapStateUpdatedCallback;
			_mapIconsModel.OnMapIconsUpdated += MapIconsUpdatedCallback;
			IconStatusModel.SelectedIconUpdated += SelectedIconUpdatedCallback;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.Initialized -= InitializedCallback;
			_mapIconsModel.OnMapIconsUpdated -= MapIconsUpdatedCallback;
			_mapIconsModel.OnStateUpdated -= MapStateUpdatedCallback;
			IconStatusModel.SelectedIconUpdated -= SelectedIconUpdatedCallback;
			foreach (KeyValuePair<IconType, IconDescriptionItemView> iconDescriptionItem in _mapIconsStarter.IconsDescriptionView.IconDescriptionItems)
			{
				iconDescriptionItem.Value.MainButtonClicked -= MainButtonClickedCallback;
			}
		}

		private void InitializedCallback()
		{
			foreach (KeyValuePair<IconType, IconDescriptionItemView> iconDescriptionItem in _mapIconsStarter.IconsDescriptionView.IconDescriptionItems)
			{
				iconDescriptionItem.Value.MainButtonClicked += MainButtonClickedCallback;
			}
			UpdateViews();
		}

		private void MapStateUpdatedCallback(MapIconsModel.State state)
		{
			UpdateViews();
		}

		private void SelectedIconUpdatedCallback(IconType iconType)
		{
			UpdateViews();
			if (iconType == IconType.None)
			{
				return;
			}
			IconMapView iconMapView = null;
			for (int i = 0; i < _mapIconsModel.MapIconModels.Count; i++)
			{
				if ((bool)_mapIconsModel.MapIconModels[i].IconMapView && _mapIconsModel.MapIconModels[i].IconType == iconType)
				{
					if ((bool)iconMapView)
					{
						return;
					}
					iconMapView = _mapIconsModel.MapIconModels[i].IconMapView;
				}
			}
			if ((bool)_mapIconsModel.PlayerIcon && IconType.Player == iconType)
			{
				iconMapView = _mapIconsModel.PlayerIcon;
			}
			if ((bool)iconMapView)
			{
				_freeDriveProvider.MapCamera.MoveCameraByVelocity(iconMapView.transform.position, _mapIconsDataSo.SelectIcon.moveVelocity, _mapIconsDataSo.SelectIcon.maxMoveDuration);
			}
		}

		private void MapIconsUpdatedCallback()
		{
			UpdateViews();
		}

		private void MainButtonClickedCallback(IconDescriptionItemView iconDescriptionItemView)
		{
			IconType iconType = _mapIconsStarter.IconsDescriptionView.GetIconType(iconDescriptionItemView);
			if (IconStatusModel.SelectedIcon == iconType)
			{
				IconStatusModel.SelectedIcon = IconType.None;
				return;
			}
			IconStatusModel.SelectedIcon = iconType;
			IconStatusModel.SetIconVisible(iconType, visible: true);
		}

		private void UpdateViews()
		{
			if (_mapIconsModel.StateMap == MapIconsModel.State.BigMap)
			{
				for (int i = 0; i < _mapIconsModel.MapIconModels.Count; i++)
				{
					if ((bool)_mapIconsModel.MapIconModels[i].IconMapView)
					{
						_mapIconsModel.MapIconModels[i].IconMapView.UpdateIsSelected(_mapIconsModel.MapIconModels[i].IconType == IconStatusModel.SelectedIcon);
					}
				}
				if ((bool)_mapIconsModel.PlayerIcon)
				{
					_mapIconsModel.PlayerIcon.UpdateIsSelected(IconType.Player == IconStatusModel.SelectedIcon);
				}
				{
					foreach (KeyValuePair<IconType, IconDescriptionItemView> iconDescriptionItem in _mapIconsStarter.IconsDescriptionView.IconDescriptionItems)
					{
						if (_mapIconsStarter.IconsDescriptionView.GetIconType(iconDescriptionItem.Value) == IconStatusModel.SelectedIcon)
						{
							iconDescriptionItem.Value.SetBGColor(_mapIconsDataSo.DescriptionSelectColor);
						}
						else
						{
							iconDescriptionItem.Value.SetDefaultBGColor();
						}
					}
					return;
				}
			}
			for (int j = 0; j < _mapIconsModel.MapIconModels.Count; j++)
			{
				if ((bool)_mapIconsModel.MapIconModels[j].IconMapView)
				{
					_mapIconsModel.MapIconModels[j].IconMapView.UpdateIsSelected(isSelected: false);
				}
			}
			if ((bool)_mapIconsModel.PlayerIcon)
			{
				_mapIconsModel.PlayerIcon.UpdateIsSelected(isSelected: false);
			}
			foreach (KeyValuePair<IconType, IconDescriptionItemView> iconDescriptionItem2 in _mapIconsStarter.IconsDescriptionView.IconDescriptionItems)
			{
				iconDescriptionItem2.Value.SetDefaultBGColor();
			}
		}
	}
}
