using CPMEngine;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace MVPFree.MapIcons.Presenters
{
	public class ClickToMapIconPresenter : IPresenter
	{
		private MapIconsDataSo _mapIconsDataSo;

		private MapIconsModel _mapIconsModel;

		private FreeDriveProvider _freeDriveProvider;

		private GPSNavigationCrossView _gpsNavigationCrossView;

		private IconMapView _closeIconView;

		private Vector2 _tapDownPosition;

		private int _pointerId;

		private float _tapDownTime;

		private bool _isTapDown;

		private SelectIconData SelectIconData => _mapIconsDataSo.SelectIcon;

		[Inject]
		public ClickToMapIconPresenter(MapIconsDataSo mapIconsDataSo, MapIconsModel mapIconsModel, FreeDriveProvider freeDriveProvider, GPSNavigationCrossView gpsNavigationCrossView)
		{
			_mapIconsDataSo = mapIconsDataSo;
			_mapIconsModel = mapIconsModel;
			_freeDriveProvider = freeDriveProvider;
			_gpsNavigationCrossView = gpsNavigationCrossView;
		}

		public void Subscribe()
		{
			_mapIconsModel.BigMapPointerDown += BigMapPointerDownCallback;
			_mapIconsModel.BigMapPointerUp += BigMapPointerUpCallback;
			_mapIconsModel.OnStateUpdated += MapStateUpdatedCallback;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.BigMapPointerDown -= BigMapPointerDownCallback;
			_mapIconsModel.BigMapPointerUp -= BigMapPointerUpCallback;
			_mapIconsModel.OnStateUpdated -= MapStateUpdatedCallback;
		}

		private void BigMapPointerDownCallback(PointerEventData eventData)
		{
			Deselect();
			_isTapDown = true;
			_pointerId = eventData.pointerId;
			_tapDownTime = Time.time;
			_tapDownPosition = eventData.position;
		}

		private void MapStateUpdatedCallback(MapIconsModel.State state)
		{
			if (state != MapIconsModel.State.BigMap)
			{
				Deselect();
			}
		}

		private void BigMapPointerUpCallback(PointerEventData eventData)
		{
			if (!_isTapDown || Time.time - _tapDownTime > SelectIconData.clickDuration || Vector2.Distance(eventData.position, _tapDownPosition) > SelectIconData.clickDistance || _pointerId != eventData.pointerId)
			{
				_isTapDown = false;
				return;
			}
			_isTapDown = false;
			if (_freeDriveProvider == null || !_freeDriveProvider.MapCamera)
			{
				return;
			}
			float x = eventData.position.x / (float)Screen.width;
			float y = eventData.position.y / (float)Screen.height;
			Ray ray = _freeDriveProvider.MapCamera.cam.ViewportPointToRay(new Vector3(x, y, 0f));
			IconMapView iconMapView = null;
			float num = float.MaxValue;
			for (int i = 0; i < _mapIconsModel.MapIconModels.Count; i++)
			{
				if ((bool)_mapIconsModel.MapIconModels[i].IconMapView && _mapIconsModel.MapIconModels[i].IconMapView.IsVisible)
				{
					float distanceSqr = GetDistanceSqr(ray.origin, _mapIconsModel.MapIconModels[i].IconMapView.transform.position);
					if (distanceSqr < num)
					{
						iconMapView = _mapIconsModel.MapIconModels[i].IconMapView;
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
			if ((bool)iconMapView && !(Mathf.Sqrt(num) > SelectIconData.selectDistance * (_freeDriveProvider.MapCamera.cam.orthographicSize / 400f)))
			{
				_mapIconsModel.OnMapIconTypeSelected(iconMapView);
				_freeDriveProvider.MapCamera.MoveCameraByVelocity(iconMapView.transform.position, SelectIconData.moveVelocity, SelectIconData.maxMoveDuration);
				if (SelectIconData.hapticsEnabled)
				{
					Utils.Vibrate(SelectIconData.hapticsDuration, SelectIconData.hapticsIntensity, unscaledTime: true);
				}
				_closeIconView = iconMapView;
				_closeIconView.UpdateIsSelected(isSelected: true, updateBGCircle: false);
				_gpsNavigationCrossView.SetCentreCircleVisible(isVisible: false);
				SelectIconData.wwiseSound?.SetValue(_freeDriveProvider.MapControl.gameObject);
				SelectIconData.uiEvent?.Post(_freeDriveProvider.MapControl.gameObject);
			}
		}

		private float GetDistanceSqr(Vector3 posA, Vector3 posB)
		{
			posA.y = posB.y;
			return (posA - posB).sqrMagnitude;
		}

		private void Deselect()
		{
			_gpsNavigationCrossView.SetCentreCircleVisible(isVisible: true);
			if ((bool)_closeIconView)
			{
				_closeIconView.UpdateIsSelected(isSelected: false);
				_closeIconView = null;
			}
		}
	}
}
