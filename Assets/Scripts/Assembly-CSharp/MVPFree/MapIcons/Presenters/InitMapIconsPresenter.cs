using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using Newtonsoft.Json;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.EventSystem.Data;
using _Car_Parking.Scripts.EventSystem.EventService;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace MVPFree.MapIcons.Presenters
{
	public class InitMapIconsPresenter : IPresenter
	{
		private const string BuildingPrefKey = "FastFoodBuilding";

		private MapIconsModel _mapIconsModel;

		private SceneControl _sceneControl;

		private MapIconsDataSo _mapIconsDataSo;

		private MapIconsStarter _mapIconsStarter;

		private IEventService _eventService;

		private TimeAttackDataSO _timeAttackDataSo;

		private MapIconPositionsDataSo _mapIconPositionsDataSo;

		private ArenaModeRegistry _arenaRegistry;

		private Transform _mapIconParent;

		[Inject]
		public InitMapIconsPresenter(MapIconsModel mapIconsModel, SceneControl sceneControl, MapIconsDataSo mapIconsDataSo, MapIconsStarter mapIconsStarter, IEventService eventService, TimeAttackDataSO timeAttackDataSo, MapIconPositionsDataSo mapIconPositionsDataSo, ArenaModeRegistry arenaRegistry)
		{
			_mapIconsModel = mapIconsModel;
			_sceneControl = sceneControl;
			_mapIconsDataSo = mapIconsDataSo;
			_mapIconsStarter = mapIconsStarter;
			_eventService = eventService;
			_timeAttackDataSo = timeAttackDataSo;
			_mapIconPositionsDataSo = mapIconPositionsDataSo;
			_arenaRegistry = arenaRegistry;
		}

		public void Subscribe()
		{
			_mapIconsModel.Initialized += OnInit;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.Initialized -= OnInit;
		}

		private void OnInit()
		{
			_mapIconsDataSo.Init();
			_mapIconsStarter.EventZoneCollectiblesPosDatas.Init();
			TryCreateEventIcons();
			string defaultValue = JsonConvert.SerializeObject(new Dictionary<int, bool>
			{
				{ 0, true },
				{ 1, false }
			});
			Dictionary<int, bool> dictionary = JsonConvert.DeserializeObject<Dictionary<int, bool>>(ObscuredPrefs.GetString("BuyingGarages", defaultValue));
			for (int i = 0; i < _mapIconsStarter.GaragesIconsPoint.Count; i++)
			{
				if ((bool)_mapIconsStarter.GaragesIconsPoint[i])
				{
					IconType iconType = (dictionary[i] ? IconType.YourGarage : IconType.BuyGarage);
					IconMapView value = _mapIconsModel.CreateMapIcon(_mapIconsStarter.GaragesIconsPoint[i].gameObject, iconType, i);
					_mapIconsModel.GaragesIcons.Add(i, value);
				}
			}
			for (int j = 0; j < _sceneControl.RaceTrigers.Length; j++)
			{
				if ((bool)_sceneControl.RaceTrigers[j])
				{
					_Car_Parking.Scripts.EventSystem.Data.Event eventByName = _eventService.GetEventByName(_timeAttackDataSo.TimeAttackDatas[j].EventType);
					IconMapView iconMapView = _mapIconsModel.CreateMapIcon(_sceneControl.RaceTrigers[j].gameObject, _timeAttackDataSo.TimeAttackDatas[j].IconType, _timeAttackDataSo.TimeAttackDatas[j].TrackIndex);
					bool flag = PlayerPrefs.GetInt($"newRaceIcon_{j}", 0) == 0;
					iconMapView.ShowNewIcon(flag);
					iconMapView.ShowTimeIcon(eventByName != null && !flag);
				}
			}
			GameObject[] hospitalTrigers = _sceneControl.HospitalTrigers;
			foreach (GameObject point in hospitalTrigers)
			{
				CreateDefaultIcon(point, IconType.Hospital, -1);
			}
			hospitalTrigers = _sceneControl.PoliceTrigers;
			foreach (GameObject point2 in hospitalTrigers)
			{
				CreateDefaultIcon(point2, IconType.Police, -1);
			}
			hospitalTrigers = _sceneControl.FireStationTrigers;
			foreach (GameObject point3 in hospitalTrigers)
			{
				CreateDefaultIcon(point3, IconType.FireStation, -1);
			}
			hospitalTrigers = _sceneControl.AirPortTrigers;
			foreach (GameObject point4 in hospitalTrigers)
			{
				CreateDefaultIcon(point4, IconType.AirPort, -1);
			}
			hospitalTrigers = _sceneControl.CarWashTrigers;
			foreach (GameObject point5 in hospitalTrigers)
			{
				CreateDefaultIcon(point5, IconType.CarWash, -1);
			}
			hospitalTrigers = _sceneControl.SellCarTrigers;
			foreach (GameObject point6 in hospitalTrigers)
			{
				CreateDefaultIcon(point6, IconType.SellCar, -1);
			}
			foreach (Transform rampPoint in _mapIconsStarter.RampPoints)
			{
				CreateDefaultIcon(rampPoint.gameObject, IconType.Ramp, -1);
			}
			MapIconPositionsDataSo.IconPositionData[] iconPositions = _mapIconPositionsDataSo.IconPositions;
			foreach (MapIconPositionsDataSo.IconPositionData iconPositionData in iconPositions)
			{
				if (ArenaMapIcons.IsArenaIcon(iconPositionData.IconType))
				{
					if (_arenaRegistry.TryGetConfig((ArenaId)iconPositionData.Id, out var _))
					{
						CreateIcon(iconPositionData.WorldPosition, iconPositionData.IconType, iconPositionData.Id);
					}
				}
				else
				{
					CreateIcon(iconPositionData.WorldPosition, iconPositionData.IconType, iconPositionData.Id);
				}
			}
		}

		private void CreateIcon(Vector3 position, IconType iconType, int id)
		{
			if (!_mapIconParent)
			{
				GameObject gameObject = new GameObject("MapIconsParent");
				_mapIconParent = gameObject.transform;
				_mapIconParent.SetParent(V.GlobalPoint);
				_mapIconParent.localPosition = Vector3.zero;
			}
			switch (iconType)
			{
			case IconType.YourHome:
			case IconType.BuyHome:
				iconType = ((ObscuredPrefs.GetInt("homeOpen" + id) == 0) ? IconType.BuyHome : IconType.YourHome);
				break;
			case IconType.YourRestaurant:
			case IconType.BuyRestaurant:
				iconType = ((ObscuredPrefs.GetInt("FastFoodBuilding" + id) == 0) ? IconType.BuyRestaurant : IconType.YourRestaurant);
				break;
			case IconType.YourGasStation:
			case IconType.BuyGasStation:
				iconType = ((ObscuredPrefs.GetInt("FastFoodBuilding" + id) == 0) ? IconType.BuyGasStation : IconType.YourGasStation);
				break;
			case IconType.YourMotel:
			case IconType.BuyMotel:
				iconType = ((ObscuredPrefs.GetInt("FastFoodBuilding" + id) == 0) ? IconType.BuyMotel : IconType.YourMotel);
				break;
			}
			GameObject gameObject2 = new GameObject("MapIcon " + iconType);
			gameObject2.transform.SetParent(_mapIconParent);
			gameObject2.transform.localPosition = position;
			IconMapView value = _mapIconsModel.CreateMapIcon(gameObject2, iconType, id);
			if (iconType == IconType.BuyHome || iconType == IconType.YourHome)
			{
				_mapIconsModel.HomeIcons[id] = value;
			}
			if (iconType == IconType.BuyRestaurant || iconType == IconType.YourRestaurant)
			{
				_mapIconsModel.RestaurantsIcons[id] = value;
			}
			if (iconType == IconType.BuyGasStation || iconType == IconType.YourGasStation)
			{
				_mapIconsModel.GasStationIcons[id] = value;
			}
			if (iconType == IconType.BuyMotel || iconType == IconType.YourMotel)
			{
				_mapIconsModel.MotelIcons[id] = value;
			}
		}

		private void CreateDefaultIcon(GameObject point, IconType iconType, int id)
		{
			if ((bool)point)
			{
				_mapIconsModel.CreateMapIcon(point, iconType, id);
			}
		}

		private void TryCreateEventIcons()
		{
			foreach (IconData currentEventIcon in _mapIconsDataSo.GetCurrentEventIcons(_eventService))
			{
				CreateEventIcons(currentEventIcon);
			}
		}

		private void CreateEventIcons(IconData iconData)
		{
			foreach (EventLocationData collectibleZone in _mapIconsStarter.EventZoneCollectiblesPosDatas.CollectibleZones)
			{
				if (collectibleZone.EventType == _Car_Parking.Scripts.EventSystem.EventType.None)
				{
					continue;
				}
				foreach (ZoneConfigData collectiblePosition in collectibleZone.CollectiblePositions)
				{
					IconMapView iconMapView = _mapIconsModel.CreateMapIcon(null, iconData.IconType, collectiblePosition.Index, collectiblePosition.ZonePosition);
					if (!_mapIconsModel.EventsIcons.ContainsKey(iconData.EventType))
					{
						Dictionary<int, IconMapView> value = new Dictionary<int, IconMapView>();
						_mapIconsModel.EventsIcons.Add(iconData.EventType, value);
					}
					_mapIconsModel.EventsIcons[iconData.EventType].Add(collectiblePosition.Index, iconMapView);
					iconMapView.TimeIcon.transform.localPosition = new Vector3(0.561f, 0.664f, 0f);
				}
			}
		}
	}
}
