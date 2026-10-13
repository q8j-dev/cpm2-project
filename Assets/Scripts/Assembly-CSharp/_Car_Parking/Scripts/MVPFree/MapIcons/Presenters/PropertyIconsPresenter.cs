using Cysharp.Threading.Tasks;
using MVPFree;
using MVPFree.MapIcons;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.WindowSystem.PropertyPopupWindow;

namespace _Car_Parking.Scripts.MVPFree.MapIcons.Presenters
{
	public class PropertyIconsPresenter : IPresenter
	{
		private MapIconsModel _mapIconsModel;

		private MapIconsDataSo _mapIconsDataSo;

		private ManagerUI _managerUI;

		private FreeDriveProvider _freeDriveProvider;

		private IPrefabFactory _prefabFactory;

		private MapIconsStarter _mapIconsStarter;

		private WindowScript _popupWindow;

		[Inject]
		public PropertyIconsPresenter(MapIconsModel mapIconsModel, MapIconsDataSo mapIconsDataSo, ManagerUI managerUI, FreeDriveProvider freeDriveProvider, IPrefabFactory prefabFactory, MapIconsStarter mapIconsStarter)
		{
			_mapIconsModel = mapIconsModel;
			_mapIconsDataSo = mapIconsDataSo;
			_managerUI = managerUI;
			_freeDriveProvider = freeDriveProvider;
			_prefabFactory = prefabFactory;
			_mapIconsStarter = mapIconsStarter;
		}

		public void Subscribe()
		{
			_mapIconsModel.OnMapIconTypeSelectedAction += OnMapIconSelect;
		}

		private void OnMapIconSelect(int index, IconMapView iconMapView)
		{
			switch (iconMapView.IconType)
			{
			case IconType.YourHome:
			case IconType.BuyHome:
				ShowPopupHouse(index, iconMapView.IconType).Forget();
				break;
			case IconType.YourMotel:
			case IconType.BuyMotel:
			case IconType.YourRestaurant:
			case IconType.BuyRestaurant:
			case IconType.YourGasStation:
			case IconType.BuyGasStation:
				ShowPopupBusiness(index, iconMapView.IconType).Forget();
				break;
			case IconType.YourGarage:
			case IconType.BuyGarage:
				ShowPopupGarage(index, iconMapView.IconType).Forget();
				break;
			}
		}

		private async UniTaskVoid ShowPopupHouse(int index, IconType iconType)
		{
			if ((bool)_popupWindow)
			{
				_popupWindow.Close();
			}
			HouseMapPopupWindow houseMapPopupWindow = (HouseMapPopupWindow)(_popupWindow = await _managerUI.CreateWindowGeneric<HouseMapPopupWindow>("HouseMapPopupWindow", onTop: true));
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ShowUIMapPopupSwtch, houseMapPopupWindow.gameObject);
			houseMapPopupWindow.Initialize(index, iconType);
		}

		private async UniTaskVoid ShowPopupBusiness(int index, IconType iconType)
		{
			if ((bool)_popupWindow)
			{
				_popupWindow.Close();
			}
			BusinessMapPopupWindow businessMapPopupWindow = (BusinessMapPopupWindow)(_popupWindow = await _managerUI.CreateWindowGeneric<BusinessMapPopupWindow>("BusinessMapPopupWindow", onTop: true));
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ShowUIMapPopupSwtch, businessMapPopupWindow.gameObject);
			businessMapPopupWindow.Initialize(index, iconType);
		}

		private async UniTaskVoid ShowPopupGarage(int index, IconType iconType)
		{
			if ((bool)_popupWindow)
			{
				_popupWindow.Close();
			}
			GarageMapPopupWindow garageMapPopupWindow = (GarageMapPopupWindow)(_popupWindow = await _managerUI.CreateWindowGeneric<GarageMapPopupWindow>("GarageMapPopupWindow", onTop: true));
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ShowUIMapPopupSwtch, garageMapPopupWindow.gameObject);
			garageMapPopupWindow.Initialize(index, iconType);
		}

		public void Unsubscribe()
		{
			_mapIconsModel.OnMapIconTypeSelectedAction -= OnMapIconSelect;
		}
	}
}
