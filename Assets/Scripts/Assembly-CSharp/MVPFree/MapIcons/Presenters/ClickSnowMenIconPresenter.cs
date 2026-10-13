using Cysharp.Threading.Tasks;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Views;
using VContainer;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.WindowSystem;

namespace MVPFree.MapIcons.Presenters
{
	public class ClickSnowMenIconPresenter : IPresenter
	{
		private MapIconsModel _mapIconsModel;

		private ManagerUI _managerUI;

		private SnowmenMapIconInfoWindow _popupWindow;

		[Inject]
		public ClickSnowMenIconPresenter(MapIconsModel mapIconsModel, ManagerUI managerUI)
		{
			_mapIconsModel = mapIconsModel;
			_managerUI = managerUI;
		}

		public void Subscribe()
		{
			_mapIconsModel.OnMapIconTypeSelectedAction += OpenSnoMenMapWindow;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.OnMapIconTypeSelectedAction -= OpenSnoMenMapWindow;
		}

		private void OpenSnoMenMapWindow(int index, IconMapView iconMapView)
		{
			if (iconMapView.IconType == IconType.SnowMen)
			{
				ShowWindow(index).Forget();
			}
		}

		private async UniTask ShowWindow(int index)
		{
			if ((bool)_popupWindow)
			{
				_popupWindow.Close();
			}
			_popupWindow = await _managerUI.CreateWindowGeneric<SnowmenMapIconInfoWindow>("SnowMenMapIconInfoWindow", onTop: true);
			SoundEventsDataHelper.PlayUISound(SoundEventsDataHelper.ShowUIMapPopupSwtch, _popupWindow.gameObject);
			_popupWindow.Init(index);
		}
	}
}
