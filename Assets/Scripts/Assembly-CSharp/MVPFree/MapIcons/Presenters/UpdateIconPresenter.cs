using System.Collections.Generic;
using CPM.Scripts.Garage.GarageBuyer;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using UnityEngine;
using VContainer;

namespace MVPFree.MapIcons.Presenters
{
	public class UpdateIconPresenter : IPresenter
	{
		private static readonly int _baseMap = Shader.PropertyToID("_BaseMap");

		private SceneControl _sceneControl;

		private FreeDriveControl _freeDriveControl;

		private MapIconsModel _mapIconsModel;

		private MapIconsDataSo _mapIconsDataSo;

		private IGarageBuyService _garageBuyService;

		[Inject]
		public UpdateIconPresenter(SceneControl sceneControl, FreeDriveControl freeDriveControl, MapIconsModel mapIconsModel, MapIconsDataSo mapIconsDataSo, IGarageBuyService garageBuyService)
		{
			_sceneControl = sceneControl;
			_freeDriveControl = freeDriveControl;
			_mapIconsModel = mapIconsModel;
			_mapIconsDataSo = mapIconsDataSo;
			_garageBuyService = garageBuyService;
		}

		public void Subscribe()
		{
			_sceneControl.HomeBought += OnUpdateHomeIcon;
			_freeDriveControl.OnFastFoodBuildBought += OnUpdateFastFoodBuildIcon;
			_freeDriveControl.OnFastFoodBuildBought += OnUpdateMotelIcon;
			_freeDriveControl.OnFastFoodBuildBought += OnUpdateGasStationIcon;
			_garageBuyService.GarageBought += OnUpdateGarageIcon;
		}

		public void Unsubscribe()
		{
			_sceneControl.HomeBought -= OnUpdateHomeIcon;
			_freeDriveControl.OnFastFoodBuildBought -= OnUpdateFastFoodBuildIcon;
			_freeDriveControl.OnFastFoodBuildBought -= OnUpdateMotelIcon;
			_freeDriveControl.OnFastFoodBuildBought -= OnUpdateGasStationIcon;
			_garageBuyService.GarageBought -= OnUpdateGarageIcon;
		}

		private void OnUpdateHomeIcon(int id, bool isBought)
		{
			IconData iconData = _mapIconsDataSo.GetIconData(isBought ? IconType.YourHome : IconType.BuyHome);
			UpdateData(_mapIconsModel.HomeIcons, id, iconData);
		}

		private void OnUpdateFastFoodBuildIcon(int id, bool isBought)
		{
			if (id != 6 && id != 4)
			{
				IconData iconData = _mapIconsDataSo.GetIconData(isBought ? IconType.YourRestaurant : IconType.BuyRestaurant);
				Dictionary<int, IconMapView> restaurantsIcons = _mapIconsModel.RestaurantsIcons;
				UpdateData(restaurantsIcons, id, iconData);
			}
		}

		private void OnUpdateMotelIcon(int id, bool isBought)
		{
			if (id == 6)
			{
				IconData iconData = _mapIconsDataSo.GetIconData(isBought ? IconType.YourMotel : IconType.BuyMotel);
				Dictionary<int, IconMapView> motelIcons = _mapIconsModel.MotelIcons;
				UpdateData(motelIcons, id, iconData);
			}
		}

		private void OnUpdateGasStationIcon(int id, bool isBought)
		{
			if (id == 4)
			{
				IconData iconData = _mapIconsDataSo.GetIconData(isBought ? IconType.YourGasStation : IconType.BuyGasStation);
				Dictionary<int, IconMapView> gasStationIcons = _mapIconsModel.GasStationIcons;
				UpdateData(gasStationIcons, id, iconData);
			}
		}

		private void OnUpdateGarageIcon(int id)
		{
			IconData iconData = _mapIconsDataSo.GetIconData(IconType.YourGarage);
			UpdateData(_mapIconsModel.GaragesIcons, id, iconData);
		}

		private void UpdateData(Dictionary<int, IconMapView> iconsData, int id, IconData iconData)
		{
			if (iconsData.ContainsKey(id) && iconsData.TryGetValue(id, out var value) && (bool)value.SpriteRenderer && iconData != null)
			{
				value.SpriteRenderer.sprite = iconData.SpriteIcon;
				iconsData[id].IconType = iconData.IconType;
			}
		}
	}
}
