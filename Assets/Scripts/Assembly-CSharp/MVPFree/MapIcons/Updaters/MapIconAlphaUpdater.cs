using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace MVPFree.MapIcons.Updaters
{
	public class MapIconAlphaUpdater : IUpdater
	{
		private const float MinOrtSize = 2100f;

		[Inject]
		private FreeDriveProvider _freeDriveProvider;

		[Inject]
		private MapIconsModel _mapIconsModel;

		[Inject]
		private MapIconsStarter _mapIconsStarter;

		[Inject]
		private MapIconsDataSo _mapIconsDataSo;

		private bool _prevAlphaZoom;

		private IconStatusModel IconStatusModel => _mapIconsModel.IconStatusModel;

		public void Subscribe()
		{
			_mapIconsModel.OnStateUpdated += MapStateUpdatedCallback;
			_mapIconsModel.OnMapIconsUpdated += MapIconsUpdatedCallback;
			IconStatusModel.SelectedIconUpdated += SelectedIconUpdatedCallback;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.OnStateUpdated -= MapStateUpdatedCallback;
			_mapIconsModel.OnMapIconsUpdated -= MapIconsUpdatedCallback;
			IconStatusModel.SelectedIconUpdated -= SelectedIconUpdatedCallback;
		}

		public void Update(float deltaTime)
		{
			if (_freeDriveProvider != null && (bool)_freeDriveProvider.MapCamera && (bool)_freeDriveProvider.MapCamera.cam && (!(_freeDriveProvider.MapCamera.cam.orthographicSize < 2100f) || _prevAlphaZoom))
			{
				_prevAlphaZoom = _freeDriveProvider.MapCamera.cam.orthographicSize >= 2100f;
				UpdateViews();
			}
		}

		private void MapStateUpdatedCallback(MapIconsModel.State state)
		{
			UpdateViews();
		}

		private void SelectedIconUpdatedCallback(IconType iconType)
		{
			UpdateViews();
		}

		private void MapIconsUpdatedCallback()
		{
			UpdateViews();
		}

		private void UpdateViews()
		{
			if (_freeDriveProvider == null || !_freeDriveProvider.MapCamera)
			{
				return;
			}
			if (_mapIconsModel.StateMap != MapIconsModel.State.BigMap)
			{
				SetAllIconsAlphaOne();
				return;
			}
			float num = 1f;
			if (_freeDriveProvider.MapCamera.cam.orthographicSize >= 2100f)
			{
				float num2 = 2100f;
				float maxOrtSize = _freeDriveProvider.MapCamera.maxOrtSize;
				float orthographicSize = _freeDriveProvider.MapCamera.cam.orthographicSize;
				float num3 = (orthographicSize - num2) / (maxOrtSize - num2);
				if (orthographicSize < 2100f)
				{
					num3 = 0f;
				}
				num = 1f - num3;
			}
			float num4 = ((IconStatusModel.SelectedIcon == IconType.None) ? 1f : _mapIconsDataSo.NotSelectedAlpha);
			for (int i = 0; i < _mapIconsModel.MapIconModels.Count; i++)
			{
				if ((bool)_mapIconsModel.MapIconModels[i].IconMapView)
				{
					_mapIconsModel.MapIconModels[i].IconMapView.UpdateAlpha((_mapIconsModel.MapIconModels[i].IconType == IconStatusModel.SelectedIcon) ? 1f : (_mapIconsModel.IconsToDisableWhenZoomCam.Contains(_mapIconsModel.MapIconModels[i].IconMapView) ? (num * num4) : num4));
				}
			}
			if ((bool)_mapIconsModel.PlayerIcon)
			{
				_mapIconsModel.PlayerIcon.UpdateAlpha((IconType.Player == IconStatusModel.SelectedIcon) ? 1f : num4);
			}
		}

		private void SetAllIconsAlphaOne()
		{
			for (int i = 0; i < _mapIconsModel.MapIconModels.Count; i++)
			{
				if ((bool)_mapIconsModel.MapIconModels[i].IconMapView)
				{
					_mapIconsModel.MapIconModels[i].IconMapView.UpdateAlpha(1f);
				}
			}
			if ((bool)_mapIconsModel.PlayerIcon)
			{
				_mapIconsModel.PlayerIcon.UpdateAlpha(1f);
			}
		}
	}
}
