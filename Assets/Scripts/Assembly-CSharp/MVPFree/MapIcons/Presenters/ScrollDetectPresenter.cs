using MVPFree.MapIcons.Models;
using VContainer;

namespace MVPFree.MapIcons.Presenters
{
	public class ScrollDetectPresenter : IPresenter
	{
		private readonly MapIconsStarter _mapIconsStarter;

		private readonly MapIconsModel _mapIconModel;

		[Inject]
		public ScrollDetectPresenter(MapIconsStarter mapIconsStarter, MapIconsModel mapIconModel)
		{
			_mapIconsStarter = mapIconsStarter;
			_mapIconModel = mapIconModel;
		}

		public void Subscribe()
		{
			_mapIconsStarter.IconsDescriptionView.ClickDetector.OnPressed += OnDetectScroll;
		}

		public void Unsubscribe()
		{
			_mapIconsStarter.IconsDescriptionView.ClickDetector.OnPressed -= OnDetectScroll;
		}

		private void OnDetectScroll(bool isScroll)
		{
			_mapIconModel.IsScrollingIconsDescription = isScroll;
		}
	}
}
