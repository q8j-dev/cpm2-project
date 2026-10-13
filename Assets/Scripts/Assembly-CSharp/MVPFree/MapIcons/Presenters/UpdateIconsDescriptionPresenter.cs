using Cysharp.Threading.Tasks;
using MVPFree.MapIcons.Models;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Drag;

namespace MVPFree.MapIcons.Presenters
{
	public class UpdateIconsDescriptionPresenter : IPresenter
	{
		private ManagerUI _managerUI;

		private MapIconsStarter _mapIconsStarter;

		private MapIconsModel _mapIconsModel;

		[Inject]
		public UpdateIconsDescriptionPresenter(ManagerUI managerUI, MapIconsStarter mapIconsStarter, MapIconsModel mapIconsModel)
		{
			_managerUI = managerUI;
			_mapIconsStarter = mapIconsStarter;
			_mapIconsModel = mapIconsModel;
		}

		public void Subscribe()
		{
			_mapIconsStarter.IconsDescriptionView.OnEnabled += OnUpdateHeight;
		}

		public void Unsubscribe()
		{
			_mapIconsStarter.IconsDescriptionView.OnEnabled -= OnUpdateHeight;
		}

		private void OnUpdateHeight(bool enable)
		{
			if (enable)
			{
				OnUpdateHeightAsync().Forget();
			}
		}

		private async UniTask OnUpdateHeightAsync()
		{
			await UniTask.DelayFrame(2);
			if ((bool)_managerUI && _mapIconsModel != null)
			{
				CanvasScaler component = _managerUI.GetComponent<CanvasScaler>();
				RectTransform component2 = _managerUI.GetComponent<RectTransform>();
				RectTransform component3 = _mapIconsStarter.IconsDescriptionView.ContentParent.GetComponent<RectTransform>();
				float x = component.referenceResolution.x;
				float y = component.referenceResolution.y;
				float width = component2.rect.width;
				float height = component2.rect.height;
				float height2 = component3.rect.height;
				float num = Mathf.Min(width / x, height / y);
				float num2 = height / num;
				float num3 = height2 / num;
				float num4 = _mapIconsStarter.IconsDescriptionView.ExpandButtonRect.rect.height * 2f;
				_mapIconsModel.OpenedScrollViewHeight = ((num2 - num4 > num3) ? num3 : (num2 - num4));
				_mapIconsModel.ClosedScrollViewHeight = _mapIconsStarter.IconsDescriptionView.ScrollViewRect.rect.height;
			}
		}
	}
}
