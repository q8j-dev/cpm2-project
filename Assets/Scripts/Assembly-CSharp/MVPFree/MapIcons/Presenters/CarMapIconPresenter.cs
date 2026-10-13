using System;
using Cysharp.Threading.Tasks;
using MVPFree.MapIcons.Enum;
using MVPFree.MapIcons.Models;
using MVPFree.MapIcons.Views;
using Photon.Pun;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;

namespace MVPFree.MapIcons.Presenters
{
	public class CarMapIconPresenter : IPresenter
	{
		private const string UI_MAPICON = "UI_mapiconcar";

		[Inject]
		private MapIconsModel _mapIconsModel;

		[Inject]
		private GlobalCollectionsProvider _collectionsProvider;

		[Inject]
		private FreeDriveProvider _freeDriveProvider;

		private readonly CompositeDisposable _disposables = new CompositeDisposable();

		public void Subscribe()
		{
			_mapIconsModel.CarRegistered += OnCarRegistered;
			_mapIconsModel.CarUnregistered += OnCarUnregistered;
			Observable.Timer(TimeSpan.FromSeconds(0.5), TimeSpan.FromSeconds(0.05000000074505806)).Subscribe((long _) =>
			{
				_freeDriveProvider.MapControl?.DrawIcons();
			}).AddTo(_disposables);
		}

		public void Unsubscribe()
		{
			_mapIconsModel.CarRegistered -= OnCarRegistered;
			_mapIconsModel.CarUnregistered -= OnCarUnregistered;
			_disposables.Clear();
		}

		private void OnCarRegistered(VehicleData vehicleData)
		{
			OnCarRegisteredAsync(vehicleData).Forget();
		}

		private async UniTask OnCarRegisteredAsync(VehicleData vehicleData)
		{
			if (!vehicleData || (bool)vehicleData.MapCarIcon)
			{
				return;
			}
			GameObject gameObject = await Addressables.InstantiateAsync("UI_mapiconcar");
			if (!gameObject || !vehicleData || vehicleData.IsBeingDestroyed)
			{
				return;
			}
			vehicleData.MapCarIcon = gameObject.GetComponent<IconMapView>();
			if ((bool)vehicleData.MapCarIcon)
			{
				_mapIconsModel.IconsToIgnoreRotate.Add(vehicleData.MapCarIcon);
				_mapIconsModel.AddMapIconModel(vehicleData.MapCarIcon, IconType.CarBlack);
				_collectionsProvider.RegisterMapIcon(vehicleData.MapCarIcon.transform);
				vehicleData.MapCarIcon.SpriteRenderer.color = GetCarColor(vehicleData);
				if (!vehicleData.isTraffiCar)
				{
					vehicleData.MapCarIcon.transform.parent = V.GlobalPoint.transform;
				}
				if (!FlagsStorage.IsMultiFree)
				{
					vehicleData.MapCarIcon.transform.parent = vehicleData.gameObject.transform;
				}
				if (V.Car == vehicleData.gameObject || !FlagsStorage.IsMultiFree)
				{
					vehicleData.MapCarIcon.SetActive(active: false);
				}
				if ((bool)_freeDriveProvider.MapCamera)
				{
					_freeDriveProvider.MapCamera.ZoomIcon();
				}
				vehicleData.MapCarIcon.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(90f, -90f, 0f));
			}
		}

		private void OnCarUnregistered(VehicleData vehicleData)
		{
			if ((bool)vehicleData && (bool)vehicleData.MapCarIcon)
			{
				if (!_mapIconsModel.IconsToIgnoreRotate.IsNullOrEmptyCollection() && _mapIconsModel.IconsToIgnoreRotate.Contains(vehicleData.MapCarIcon))
				{
					_mapIconsModel.IconsToIgnoreRotate.Remove(vehicleData.MapCarIcon);
				}
				_mapIconsModel.RemoveMapIconModel(vehicleData.MapCarIcon);
				UnityEngine.Object.Destroy(vehicleData.MapCarIcon.gameObject);
			}
		}

		private static Color GetCarColor(VehicleData vehicleData)
		{
			PhotonView component = vehicleData.GetComponent<PhotonView>();
			if ((bool)component && !component.IsMine)
			{
				object[] instantiationData = component.InstantiationData;
				if (instantiationData != null && instantiationData.Length > 0)
				{
					Vector3 vector = ColorsConverter.Get((object[])component.InstantiationData[4]).CarColor.Color.ToVector3();
					if (vector.x == 0.1f && vector.y == 0.2f && vector.z == 0.3f)
					{
						Color color = (vehicleData.StockMaterial ? vehicleData.StockMaterial.color : Color.black);
						vector = new Vector3(color.r, color.g, color.b);
					}
					return new Color(vector.x, vector.y, vector.z);
				}
			}
			Color defaultValue = (vehicleData.StockMaterial ? vehicleData.StockMaterial.color : Color.white);
			return PlayerDatas.GetColor("CarColor" + vehicleData.CarId, defaultValue);
		}
	}
}
