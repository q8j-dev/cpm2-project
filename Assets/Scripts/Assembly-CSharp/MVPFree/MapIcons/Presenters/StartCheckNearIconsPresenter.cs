using System;
using System.Collections.Generic;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.ScrObjs.Scripts;
using MVPFree.MapIcons.Views;
using UniRx;
using UnityEngine;
using VContainer;

namespace MVPFree.MapIcons.Presenters
{
	public class StartCheckNearIconsPresenter : IPresenter
	{
		private readonly MapIconsModel _mapIconsModel;

		private readonly MapIconsStarter _mapIconsStarter;

		private readonly MapIconsDataSo _mapIconsDataSo;

		[Inject]
		public StartCheckNearIconsPresenter(MapIconsModel mapIconsModel, MapIconsStarter mapIconsStarter, MapIconsDataSo mapIconsDataSo)
		{
			_mapIconsModel = mapIconsModel;
			_mapIconsStarter = mapIconsStarter;
			_mapIconsDataSo = mapIconsDataSo;
		}

		public void Subscribe()
		{
			_mapIconsModel.Initialized += StartCheckNearIcons;
		}

		public void Unsubscribe()
		{
			_mapIconsModel.Initialized -= StartCheckNearIcons;
			_mapIconsModel.CheckNearIconsToRotateDisposable?.Dispose();
			_mapIconsModel.CheckNearIconsToRotateDisposable = null;
		}

		private void StartCheckNearIcons()
		{
			_mapIconsModel.CheckNearIconsToRotateDisposable = Observable.Interval(TimeSpan.FromSeconds(_mapIconsDataSo.TimeIntervalCheckNearIcons)).Subscribe((long _) =>
			{
				CheckNearIcons();
			}).AddTo(_mapIconsStarter);
		}

		private void CheckNearIcons()
		{
			if (_mapIconsModel.IsBigMapOpening || !_mapIconsModel.PlayerIcon)
			{
				return;
			}
			Vector3 position = _mapIconsModel.PlayerIcon.transform.position;
			List<IconMapView> list = new List<IconMapView>();
			foreach (IconMapView icon in _mapIconsModel.Icons)
			{
				if ((icon.transform.position - position).sqrMagnitude <= _mapIconsDataSo.DistanceToCheckNearIcons * _mapIconsDataSo.DistanceToCheckNearIcons)
				{
					list.Add(icon);
				}
			}
			_mapIconsModel.IconsToRotate = list;
		}
	}
}
