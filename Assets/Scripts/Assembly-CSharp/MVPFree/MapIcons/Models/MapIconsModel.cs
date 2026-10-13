using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DG.Tweening;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using _Car_Parking.Scripts.EventSystem;

namespace MVPFree.MapIcons.Models
{
	public class MapIconsModel
	{
		public enum State
		{
			MiniMap = 0,
			BigMap = 1,
			ServiceMap = 2
		}

		public IconStatusModel IconStatusModel = new IconStatusModel();

		public float OpenedScrollViewHeight;

		public float ClosedScrollViewHeight;

		public bool IsOpenedIconsDescription;

		public bool IsMovingIconsDescription;

		public Tween OpeningIconsDescriptionTween;

		public bool IsBigMapOpening;

		public bool IsScrollingIconsDescription;

		public IDisposable CheckNearIconsToRotateDisposable;

		private List<MapIconModel> _mapIconModels = new List<MapIconModel>();

		private State _state;

		public Transform ParentOfIcons { get; set; }

		public Dictionary<int, IconMapView> HomeIcons { get; private set; }

		public Dictionary<int, IconMapView> RestaurantsIcons { get; private set; }

		public Dictionary<int, IconMapView> MotelIcons { get; private set; }

		public Dictionary<int, IconMapView> GasStationIcons { get; private set; }

		public Dictionary<int, IconMapView> GaragesIcons { get; private set; }

		public Dictionary<_Car_Parking.Scripts.EventSystem.EventType, Dictionary<int, IconMapView>> EventsIcons { get; private set; }

		public List<IconMapView> Icons { get; private set; } = new List<IconMapView>();

		public List<IconMapView> IconsToRotate { get; set; } = new List<IconMapView>();

		public List<IconMapView> IconsToDisableWhenZoomCam { get; private set; }

		public List<IconMapView> IconsToIgnoreRotate { get; private set; } = new List<IconMapView>();

		public ReadOnlyCollection<MapIconModel> MapIconModels => _mapIconModels.AsReadOnly();

		public IconMapView PlayerIcon { get; set; }

		public State StateMap
		{
			get
			{
				return _state;
			}
			set
			{
				_state = value;
				OnStateUpdated?.Invoke(value);
			}
		}

		public event Action Initialized;

		public event Action OnMapIconsUpdated;

		public event Action<bool> OnGotOutOfCar;

		public event Action<bool> OnOpenedServiceMap;

		public event Action<bool> OnOpenedBigMap;

		public event Action<State> OnStateUpdated;

		public event Action<PointerEventData> BigMapPointerDown;

		public event Action<PointerEventData> BigMapPointerUp;

		public event Action<int, IconMapView> OnMapIconTypeSelectedAction;

		public event Func<GameObject, IconType, int, Vector3, IconMapView> CreatedMapIcon;

		public event Action<VehicleData> CarRegistered;

		public event Action<VehicleData> CarUnregistered;

		public void Init(IconsDescriptionView iconsDescriptionView)
		{
			IconStatusModel.Init(iconsDescriptionView);
			HomeIcons = new Dictionary<int, IconMapView>();
			RestaurantsIcons = new Dictionary<int, IconMapView>();
			MotelIcons = new Dictionary<int, IconMapView>();
			GasStationIcons = new Dictionary<int, IconMapView>();
			GaragesIcons = new Dictionary<int, IconMapView>();
			EventsIcons = new Dictionary<_Car_Parking.Scripts.EventSystem.EventType, Dictionary<int, IconMapView>>();
			IconsToDisableWhenZoomCam = new List<IconMapView>();
			Initialized?.Invoke();
		}

		public IconMapView CreateMapIcon(GameObject parent, IconType iconType, int id, Vector3 position = default(Vector3))
		{
			return CreatedMapIcon?.Invoke(parent, iconType, id, position);
		}

		public void RegisterCar(VehicleData vehicle)
		{
			CarRegistered?.Invoke(vehicle);
		}

		public void UnregisterCar(VehicleData vehicle)
		{
			CarUnregistered?.Invoke(vehicle);
		}

		public void GotOutOfCar(bool gotOut)
		{
			OnGotOutOfCar?.Invoke(gotOut);
		}

		public void OpenServiceMap(bool enabled)
		{
			StateMap = (enabled ? State.ServiceMap : State.MiniMap);
			OnOpenedServiceMap?.Invoke(enabled);
		}

		public void OpenedBigMap(bool enabled)
		{
			StateMap = (enabled ? State.BigMap : State.MiniMap);
			OnOpenedBigMap?.Invoke(enabled);
		}

		public void RemoveMapIconModel(IconMapView mapIcon)
		{
			if (!mapIcon)
			{
				return;
			}
			for (int i = 0; i < _mapIconModels.Count; i++)
			{
				if ((bool)_mapIconModels[i].IconMapView && _mapIconModels[i].IconMapView == mapIcon)
				{
					_mapIconModels.RemoveAt(i);
					break;
				}
			}
			OnMapIconsUpdated?.Invoke();
		}

		public void AddMapIconModel(IconMapView iconMapView, IconType iconType, int id = -1)
		{
			AddMapIconModel(new MapIconModel(iconMapView, iconType, id));
		}

		public void AddMapIconModel(MapIconModel mapIconModel)
		{
			_mapIconModels.Add(mapIconModel);
			OnMapIconsUpdated?.Invoke();
		}

		public void OnBigMapPointerDown(PointerEventData eventData)
		{
			BigMapPointerDown?.Invoke(eventData);
		}

		public void OnBigMapPointerUp(PointerEventData eventData)
		{
			BigMapPointerUp?.Invoke(eventData);
		}

		public void OnMapIconTypeSelected(IconMapView iconMapView)
		{
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.AcceptSwitch, iconMapView.gameObject);
			OnMapIconTypeSelectedAction?.Invoke(iconMapView.Index, iconMapView);
		}
	}
}
