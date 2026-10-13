using DG.Tweening;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using UnityEngine;
using VContainer;

namespace MVPFree.MapIcons.Presenters
{
	public class ResetIconsDescriptionScrollViewPresenter : IPresenter
	{
		private MapIconsStarter _mapIconsStarter;

		private MapIconsModel _mapIconsModel;

		private MapIconsDataSo _mapIconsDataSo;

		[Inject]
		public ResetIconsDescriptionScrollViewPresenter(MapIconsStarter mapIconsStarter, MapIconsModel mapIconsModel, MapIconsDataSo mapIconsDataSo)
		{
			_mapIconsStarter = mapIconsStarter;
			_mapIconsModel = mapIconsModel;
			_mapIconsDataSo = mapIconsDataSo;
		}

		public void Subscribe()
		{
			_mapIconsStarter.IconsDescriptionView.OnEnabled += OnResetScrollView;
		}

		public void Unsubscribe()
		{
			_mapIconsStarter.IconsDescriptionView.OnEnabled -= OnResetScrollView;
		}

		private void OnResetScrollView(bool enabled)
		{
			if (!enabled)
			{
				_mapIconsModel.OpeningIconsDescriptionTween?.Kill();
				_mapIconsModel.OpeningIconsDescriptionTween = null;
				_mapIconsModel.IsMovingIconsDescription = false;
				_mapIconsModel.IsOpenedIconsDescription = false;
				_mapIconsStarter.IconsDescriptionView.ScrollView.verticalNormalizedPosition = 1f;
				_mapIconsStarter.IconsDescriptionView.ImageArrowRectTransform.rotation = Quaternion.Euler(_mapIconsDataSo.RotationImageArrowDown);
				_mapIconsStarter.IconsDescriptionView.ScrollViewRect.sizeDelta = new Vector2(_mapIconsStarter.IconsDescriptionView.ScrollViewRect.sizeDelta.x, _mapIconsModel.ClosedScrollViewHeight);
			}
		}
	}
}
