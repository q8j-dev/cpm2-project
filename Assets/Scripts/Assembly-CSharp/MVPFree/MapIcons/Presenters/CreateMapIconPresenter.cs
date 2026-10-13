using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace MVPFree.MapIcons.Presenters
{
	public class CreateMapIconPresenter : IPresenter
	{
		private const string Iconpoint = "IconPoint";

		private const string ParentOfIconsName = "parent of icons";

		private MapIconsModel _mapIconsModel;

		private SceneControl _sceneControl;

		private MapIconsDataSo _mapIconsDataSo;

		private GlobalCollectionsProvider _collectionsProvider;

		[Inject]
		public CreateMapIconPresenter(MapIconsModel mapIconsModel, SceneControl sceneControl, MapIconsDataSo mapIconsDataSo, GlobalCollectionsProvider collectionsProvider)
		{
			_mapIconsModel = mapIconsModel;
			_sceneControl = sceneControl;
			_mapIconsDataSo = mapIconsDataSo;
			_collectionsProvider = collectionsProvider;
		}

		public void Subscribe()
		{
			_mapIconsModel.CreatedMapIcon += OnCreateMapIcon;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.CreatedMapIcon -= OnCreateMapIcon;
		}

		private IconMapView OnCreateMapIcon(GameObject parent, IconType iconType, int id, Vector3 position)
		{
			Transform transform = parent?.transform.Find("IconPoint");
			if ((bool)transform && !transform.gameObject.activeSelf)
			{
				return null;
			}
			if (_mapIconsModel.ParentOfIcons == null)
			{
				CreateParentOfIcons();
			}
			IconMapView iconMapView = Object.Instantiate(_mapIconsDataSo.MapIconPrefab, _mapIconsModel.ParentOfIcons.transform);
			iconMapView.Init(id, iconType);
			Vector3 vector = (parent ? parent.transform.position : position);
			iconMapView.transform.position = (transform ? transform.position : vector);
			iconMapView.transform.eulerAngles = _mapIconsDataSo.InitIconRotation;
			iconMapView.transform.position += _mapIconsDataSo.InitIconPositionY;
			_collectionsProvider.RegisterMapIcon(iconMapView.transform);
			if ((bool)iconMapView.SpriteRenderer)
			{
				IconData iconData = _mapIconsDataSo.GetIconData(iconType);
				iconMapView.EventType = iconData.EventType;
				iconMapView.SpriteRenderer.sprite = iconData.SpriteIcon;
				iconMapView.transform.localScale = iconData.ScaleIcon;
				iconMapView.SpriteRenderer.sortingOrder = iconData.SortingOrder;
			}
			_mapIconsModel.Icons.Add(iconMapView);
			if (_mapIconsDataSo.IconsToDisableWhenZoomCam.Contains(iconType))
			{
				_mapIconsModel.IconsToDisableWhenZoomCam.Add(iconMapView);
			}
			_mapIconsModel.AddMapIconModel(iconMapView, iconType, id);
			return iconMapView;
		}

		private void CreateParentOfIcons()
		{
			_mapIconsModel.ParentOfIcons = new GameObject("parent of icons").transform;
			_mapIconsModel.ParentOfIcons.position = Vector3.zero;
			_mapIconsModel.ParentOfIcons.parent = V.GlobalPoint;
		}
	}
}
