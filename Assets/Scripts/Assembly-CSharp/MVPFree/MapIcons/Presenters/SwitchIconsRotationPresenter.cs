using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using UnityEngine;

namespace MVPFree.MapIcons.Presenters
{
	public class SwitchIconsRotationPresenter : IPresenter
	{
		private readonly MapIconsStarter _mapIconsStarter;

		private readonly MapIconsModel _mapIconsModel;

		private readonly MapIconsDataSo _mapIconsDataSo;

		public SwitchIconsRotationPresenter(MapIconsStarter mapIconsStarter, MapIconsModel mapIconsModel, MapIconsDataSo mapIconsDataSo)
		{
			_mapIconsStarter = mapIconsStarter;
			_mapIconsModel = mapIconsModel;
			_mapIconsDataSo = mapIconsDataSo;
		}

		public void Subscribe()
		{
			_mapIconsStarter.IconsDescriptionView.OnEnabled += SwitchIconsRotation;
			_mapIconsModel.OnOpenedServiceMap += SwitchIconsRotation;
		}

		public void Unsubscribe()
		{
			_mapIconsStarter.IconsDescriptionView.OnEnabled -= SwitchIconsRotation;
			_mapIconsModel.OnOpenedServiceMap -= SwitchIconsRotation;
		}

		private void SwitchIconsRotation(bool isOpen)
		{
			if (isOpen)
			{
				foreach (IconMapView icon in _mapIconsModel.Icons)
				{
					icon.transform.rotation = Quaternion.Euler(_mapIconsDataSo.InitIconRotation);
				}
				_mapIconsStarter.IconsDescriptionView.RebuildIconDescriptionItems();
			}
			_mapIconsModel.IsBigMapOpening = isOpen;
		}
	}
}
