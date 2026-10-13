using CarInteractionAnimationsScripts;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using UnityEngine;
using _Car_Parking;

namespace MVPFree.MapIcons.Updaters
{
	public class RotateIconOnMiniMapUpdater : IUpdater
	{
		private MapIconsModel _mapIconsModel;

		private MapIconsDataSo _mapIconsDataSo;

		private MenuLevelParent _menuLevelParent;

		private MainPerson _mainPerson;

		private Quaternion _previousPlayerIconRotation;

		private bool _missingDependencyReported;

		public RotateIconOnMiniMapUpdater(MapIconsModel mapIconsModel, MapIconsDataSo mapIconsDataSo, MenuLevelParent menuLevelParent)
		{
			_mapIconsModel = mapIconsModel;
			_mapIconsDataSo = mapIconsDataSo;
			_menuLevelParent = menuLevelParent;
		}

		public void Subscribe()
		{
		}

		public void Unsubscribe()
		{
		}

		public void Update(float deltaTime)
		{
			if (_mapIconsModel == null || !_mapIconsDataSo || !_menuLevelParent)
			{
				ReportMissingDependencyOnce();
			}
			else
			{
				if (_mapIconsModel.IsBigMapOpening || !_mapIconsModel.PlayerIcon || !V.DriverPerson || (_previousPlayerIconRotation == _mapIconsModel.PlayerIcon.transform.rotation && !_menuLevelParent.isDroneEnabled))
				{
					return;
				}
				foreach (IconMapView item in _mapIconsModel.IconsToRotate)
				{
					if (!item)
					{
						continue;
					}
					bool flag = false;
					bool flag2 = false;
					if (!_mainPerson)
					{
						V.DriverPerson.TryGetComponent<MainPerson>(out _mainPerson);
					}
					if ((bool)_mainPerson && (bool)_mainPerson.PersonAnimationsController)
					{
						SitGetOutData sitGetOutData = _mainPerson.PersonAnimationsController.SitGetOutCarSystem?.SitGetOutData;
						if (sitGetOutData != null)
						{
							flag = sitGetOutData.IsSittingIntoCarAnimate;
							flag2 = sitGetOutData.IsGettingOutOfTheCarAnimate;
						}
					}
					float y;
					if ((flag | flag2) || _menuLevelParent.isDroneEnabled)
					{
						if (!V.CoreGameManager)
						{
							return;
						}
						y = V.CoreGameManager.transform.eulerAngles.y;
					}
					else if (!FlagsStorage.OutOfCar)
					{
						if (!V.Car)
						{
							return;
						}
						y = V.Car.transform.eulerAngles.y;
					}
					else
					{
						y = V.DriverPerson.transform.eulerAngles.y;
					}
					item.transform.rotation = Quaternion.Euler(_mapIconsDataSo.InitIconRotation + new Vector3(0f, 0f, 0f - y));
				}
				_previousPlayerIconRotation = _mapIconsModel.PlayerIcon.transform.rotation;
			}
		}

		private void ReportMissingDependencyOnce()
		{
			if (!_missingDependencyReported)
			{
				_missingDependencyReported = true;
			}
		}
	}
}
