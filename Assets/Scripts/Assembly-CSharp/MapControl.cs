using System;
using MVPFree.MapIcons.Views;
using UniRx;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Providers;

public class MapControl : MonoBehaviour
{
	public Texture2D pointTexureCar;

	public float ClampLenthMax;

	public Vector3 customPointPos;

	public Transform CustomPointIcon;

	public Transform treasure;

	public bool showTreasure;

	public Transform taxi;

	public bool showCustomPoint;

	private OnlineCollectionsProvider _onlineCollectionsProvider;

	private GlobalCollectionsProvider _globalCollectionsProvider;

	private FreeDriveProvider _freeDriveProvider;

	private IDisposable _zoomIconDisposable;

	[Inject]
	public void Construct(OnlineCollectionsProvider onlineCollectionsProvider, GlobalCollectionsProvider globalCollectionsProvider, FreeDriveProvider freeDriveProvider)
	{
		_freeDriveProvider = freeDriveProvider;
		_globalCollectionsProvider = globalCollectionsProvider;
		_onlineCollectionsProvider = onlineCollectionsProvider;
	}

	private void Awake()
	{
		InitAwake();
	}

	private void OnDestroy()
	{
		_freeDriveProvider.MapControl = null;
	}

	public void InitAwake()
	{
		_freeDriveProvider.MapControl = this;
		if ((bool)CustomPointIcon)
		{
			CustomPointIcon.transform.gameObject.SetActive(value: false);
			_globalCollectionsProvider.RegisterMapIcon(CustomPointIcon);
		}
	}

	public void InitStart()
	{
		PlayerDatas.poitnTexture = pointTexureCar;
	}

	private void CalcCarIconPos(Transform tr, Transform car)
	{
		if (!V.DriverPerson && !V.Car)
		{
			return;
		}
		Vector3 vector = car.transform.position;
		Vector3 eulerAngles = car.transform.eulerAngles;
		if (!car.gameObject.activeSelf)
		{
			MultiPlayerPosControl component = car.GetComponent<MultiPlayerPosControl>();
			vector = component.correctPlayerPos + V.GlobalOffset;
			eulerAngles = component.correctPlayerRot.eulerAngles;
		}
		if ((bool)V.DriverPerson)
		{
			if (Vector3.Distance(vector, V.DriverPerson.transform.position) > ClampLenthMax && !_freeDriveProvider.MapCamera.ShowingBigMap && !_freeDriveProvider.MapCamera.serviceMapIsOpen)
			{
				Vector3 vector2 = Vector3.Normalize(vector - V.DriverPerson.transform.position);
				tr.transform.position = V.DriverPerson.transform.position + ClampLenthMax * vector2;
			}
			else
			{
				tr.transform.position = car.transform.position;
				tr.transform.eulerAngles = new Vector3(90f, eulerAngles.y - 90f, 0f);
			}
		}
	}

	private void CalIconPos(Transform icon, Vector3 pos)
	{
		if ((bool)V.DriverPerson || (bool)V.Car)
		{
			Vector3 position = icon.position;
			icon.position = new Vector3(position.x, -10f, position.z);
		}
	}

	private void CalcIconPosTreasure(Transform treasureIcon)
	{
		if ((bool)V.DriverPerson || (bool)V.Car)
		{
			Transform parent = treasureIcon.parent;
			Vector3 position = parent.position;
			if (Vector3.Distance(position, V.DriverPerson.transform.position) > ClampLenthMax && !_freeDriveProvider.MapCamera.ShowingBigMap && showTreasure)
			{
				Vector3 vector = Vector3.Normalize(position - V.DriverPerson.transform.position);
				treasureIcon.position = V.DriverPerson.transform.position + ClampLenthMax * vector;
			}
			else
			{
				Vector3 position2 = parent.position;
				treasureIcon.position = new Vector3(position2.x, position2.y + 75f, position2.z);
			}
		}
	}

	private void CalIconPosTaxi(Transform icon, Vector3 pos)
	{
		if ((bool)V.DriverPerson || (bool)V.Car)
		{
			if (Vector3.Distance(pos, V.DriverPerson.transform.position) > ClampLenthMax && !_freeDriveProvider.MapCamera.ShowingBigMap)
			{
				Vector3 vector = Vector3.Normalize(pos - V.DriverPerson.transform.position);
				icon.transform.position = V.DriverPerson.transform.position + ClampLenthMax * vector;
			}
			else
			{
				icon.transform.position = pos;
			}
			icon.transform.position = new Vector3(icon.transform.position.x, -10f, icon.transform.position.z);
		}
	}

	public void DrawIcons()
	{
		DrawCars();
		if (showCustomPoint)
		{
			CalIconPos(CustomPointIcon, customPointPos);
		}
		if ((bool)V.DriverPerson)
		{
			if ((bool)treasure)
			{
				CalcIconPosTreasure(treasure);
			}
			if ((bool)taxi)
			{
				CalIconPosTaxi(taxi, taxi.transform.parent.position);
			}
		}
	}

	private void DrawCars()
	{
		MultiPlayerPosControl[] carsOnlineNow = _onlineCollectionsProvider.CarsOnlineNow;
		foreach (MultiPlayerPosControl multiPlayerPosControl in carsOnlineNow)
		{
			if (!multiPlayerPosControl)
			{
				continue;
			}
			IconMapView mapCarIcon = multiPlayerPosControl.GetComponent<VehicleData>().MapCarIcon;
			if ((bool)mapCarIcon)
			{
				if (!FlagsStorage.ChasingRaceModeStarted)
				{
					CalcCarIconPos(mapCarIcon.transform, multiPlayerPosControl.transform);
				}
				else if (multiPlayerPosControl.gameObject == _freeDriveProvider.RacingCar2 || multiPlayerPosControl.gameObject == V.Car)
				{
					CalcCarIconPos(mapCarIcon.transform, multiPlayerPosControl.transform);
				}
			}
		}
	}

	public void CopyAndSetColor(IconMapView icon, Color c)
	{
		if ((bool)_freeDriveProvider.MapControl)
		{
			icon.SpriteRenderer.color = c;
			Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				_freeDriveProvider.MapCamera.ZoomIcon();
			}).AddTo(this);
		}
	}
}
