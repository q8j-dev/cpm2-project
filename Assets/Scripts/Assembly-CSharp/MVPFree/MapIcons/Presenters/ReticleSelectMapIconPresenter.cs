using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.SOAP;

namespace MVPFree.MapIcons.Presenters
{
	public class ReticleSelectMapIconPresenter : IPresenter
	{
		private readonly MapIconsDataSo _mapIconsDataSo;

		private readonly MapIconsModel _mapIconsModel;

		private readonly FreeDriveProvider _freeDriveProvider;

		private readonly GPSNavigationCrossView _gpsNavigationCrossView;

		private readonly EventRegistry _eventRegistry;

		private RegistryHandle<ScriptableEvent> _showPopUpHandle = new RegistryHandle<ScriptableEvent>(EventRegistryIDs.ShowPupUpOnMap);

		private SelectIconData SelectIconData => _mapIconsDataSo.SelectIcon;

		[Inject]
		public ReticleSelectMapIconPresenter(MapIconsDataSo mapIconsDataSo, MapIconsModel mapIconsModel, FreeDriveProvider freeDriveProvider, GPSNavigationCrossView gpsNavigationCrossView, EventRegistry eventRegistry)
		{
			_mapIconsDataSo = mapIconsDataSo;
			_mapIconsModel = mapIconsModel;
			_freeDriveProvider = freeDriveProvider;
			_gpsNavigationCrossView = gpsNavigationCrossView;
			_eventRegistry = eventRegistry;
		}

		public void Subscribe()
		{
			_showPopUpHandle.Get(_eventRegistry).Subscribe(OnShowPopUpRequested);
		}

		public void Unsubscribe()
		{
			_showPopUpHandle.Get(_eventRegistry)?.Unsubscribe(OnShowPopUpRequested);
		}

		private void OnShowPopUpRequested()
		{
			if (_mapIconsModel.StateMap != MapIconsModel.State.BigMap || _freeDriveProvider == null || !_freeDriveProvider.MapCamera)
			{
				return;
			}
			Vector2 centreCircleScreenPosition = _gpsNavigationCrossView.CentreCircleScreenPosition;
			float x = centreCircleScreenPosition.x / (float)Screen.width;
			float y = centreCircleScreenPosition.y / (float)Screen.height;
			Ray ray = _freeDriveProvider.MapCamera.cam.ViewportPointToRay(new Vector3(x, y, 0f));
			IconMapView iconMapView = null;
			float num = float.MaxValue;
			for (int i = 0; i < _mapIconsModel.MapIconModels.Count; i++)
			{
				IconMapView iconMapView2 = _mapIconsModel.MapIconModels[i].IconMapView;
				if ((bool)iconMapView2 && iconMapView2.IsVisible)
				{
					float distanceSqr = GetDistanceSqr(ray.origin, iconMapView2.transform.position);
					if (distanceSqr < num)
					{
						iconMapView = iconMapView2;
						num = distanceSqr;
					}
				}
			}
			if ((bool)_mapIconsModel.PlayerIcon && _mapIconsModel.PlayerIcon.IsVisible)
			{
				float distanceSqr2 = GetDistanceSqr(ray.origin, _mapIconsModel.PlayerIcon.transform.position);
				if (distanceSqr2 < num)
				{
					iconMapView = _mapIconsModel.PlayerIcon;
					num = distanceSqr2;
				}
			}
			float num2 = SelectIconData.selectDistance * (_freeDriveProvider.MapCamera.cam.orthographicSize / 400f);
			if ((bool)iconMapView && !(Mathf.Sqrt(num) > num2))
			{
				_mapIconsModel.OnMapIconTypeSelected(iconMapView);
			}
		}

		private float GetDistanceSqr(Vector3 posA, Vector3 posB)
		{
			posA.y = posB.y;
			return (posA - posB).sqrMagnitude;
		}
	}
}
