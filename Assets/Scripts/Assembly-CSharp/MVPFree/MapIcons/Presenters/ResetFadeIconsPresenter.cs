using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Views;
using UnityEngine;

namespace MVPFree.MapIcons.Presenters
{
	public class ResetFadeIconsPresenter : IPresenter
	{
		private readonly MapIconsStarter _mapIconsStarter;

		private readonly MapIconsModel _mapIconsModel;

		public ResetFadeIconsPresenter(MapIconsStarter mapIconsStarter, MapIconsModel mapIconsModel)
		{
			_mapIconsStarter = mapIconsStarter;
			_mapIconsModel = mapIconsModel;
		}

		public void Subscribe()
		{
			_mapIconsStarter.IconsDescriptionView.OnEnabled += ResetFade;
			_mapIconsModel.OnOpenedServiceMap += ResetFade;
		}

		public void Unsubscribe()
		{
			_mapIconsStarter.IconsDescriptionView.OnEnabled -= ResetFade;
			_mapIconsModel.OnOpenedServiceMap -= ResetFade;
		}

		private void ResetFade(bool isOpen)
		{
			if (isOpen)
			{
				return;
			}
			foreach (IconMapView item in _mapIconsModel.IconsToDisableWhenZoomCam)
			{
				if ((bool)item && (bool)item.SpriteRenderer)
				{
					SetColor(item.SpriteRenderer);
					if ((bool)item.SpriteRendererBackground)
					{
						SetColor(item.SpriteRendererBackground);
					}
				}
			}
		}

		private void SetColor(SpriteRenderer spriteRenderer)
		{
			Color color = spriteRenderer.color;
			color.a = 1f;
			spriteRenderer.color = color;
		}
	}
}
