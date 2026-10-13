using MVPFree.MapIcons.Models;
using VContainer;

namespace MVPFree.MapIcons.Presenters
{
	public class EnableDisableCarIconPresenter : IPresenter
	{
		private MapIconsModel _mapIconsModel;

		[Inject]
		public EnableDisableCarIconPresenter(MapIconsModel mapIconsModel)
		{
			_mapIconsModel = mapIconsModel;
		}

		public void Subscribe()
		{
			_mapIconsModel.OnGotOutOfCar += OnEnableDisablePlayerIcon;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.OnGotOutOfCar -= OnEnableDisablePlayerIcon;
		}

		private void OnEnableDisablePlayerIcon(bool getOutOfCar)
		{
			if ((bool)V.Car && V.Car.TryGetComponent<VehicleData>(out var component) && (bool)component.MapCarIcon)
			{
				component.MapCarIcon.SetActive(getOutOfCar);
			}
		}
	}
}
