using DG.Tweening;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using UnityEngine;

namespace MVPFree.MapIcons.Presenters
{
	public class OpenCloseIconDescriptionItemsPresenter : IPresenter
	{
		private const float DurationOpenClose = 1f;

		private MapIconsStarter _mapIconsStarter;

		private MapIconsModel _mapIconsModel;

		private MapIconsDataSo _mapIconsDataSo;

		public OpenCloseIconDescriptionItemsPresenter(MapIconsStarter mapIconsStarter, MapIconsModel mapIconsModel, MapIconsDataSo mapIconsDataSo)
		{
			_mapIconsStarter = mapIconsStarter;
			_mapIconsModel = mapIconsModel;
			_mapIconsDataSo = mapIconsDataSo;
		}

		public void Subscribe()
		{
			_mapIconsStarter.IconsDescriptionView.ExpandButton.onClick.AddListener(OnOpenCloseIconDescriptionItems);
		}

		public void Unsubscribe()
		{
			_mapIconsStarter.IconsDescriptionView.ExpandButton.onClick.RemoveListener(OnOpenCloseIconDescriptionItems);
		}

		private void OnOpenCloseIconDescriptionItems()
		{
			if (!_mapIconsModel.IsMovingIconsDescription)
			{
				float y = (_mapIconsModel.IsOpenedIconsDescription ? _mapIconsModel.ClosedScrollViewHeight : _mapIconsModel.OpenedScrollViewHeight);
				_mapIconsModel.IsMovingIconsDescription = true;
				_mapIconsModel.OpeningIconsDescriptionTween = _mapIconsStarter.IconsDescriptionView.ScrollViewRect.DOSizeDelta(new Vector2(_mapIconsStarter.IconsDescriptionView.ScrollViewRect.sizeDelta.x, y), 1f).OnComplete(() =>
				{
					Vector3 euler = (_mapIconsModel.IsOpenedIconsDescription ? _mapIconsDataSo.RotationImageArrowDown : _mapIconsDataSo.RotationImageArrowUp);
					_mapIconsStarter.IconsDescriptionView.ImageArrowRectTransform.rotation = Quaternion.Euler(euler);
					_mapIconsModel.IsMovingIconsDescription = false;
					_mapIconsModel.IsOpenedIconsDescription = !_mapIconsModel.IsOpenedIconsDescription;
				}).SetTarget(_mapIconsStarter.gameObject);
			}
		}
	}
}
