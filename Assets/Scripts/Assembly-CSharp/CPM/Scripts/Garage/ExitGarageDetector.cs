using System;
using System.Linq;
using Photon.Pun;
using UnityEngine;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Scopes;

namespace CPM.Scripts.Garage
{
	[RequireComponent(typeof(BoxCollider))]
	public class ExitGarageDetector : MonoBehaviour
	{
		[SerializeField]
		private GarageDoor Door = default;

		private GarageController _garageController;

		private MultiPlayerMenu _multiPlayerMenu;

		private BoxCollider _collider;

		private readonly Collider[] _intersectColliders = new Collider[10];

		private int _carLayerMask;

		private FreeDriveProvider _freeDriveProvider;

		[Inject]
		public void Construct(GarageController garageController, FreeDriveProvider freeDriveProvider, OptionalDependency<MultiPlayerMenu> multiPlayerMenu)
		{
			_freeDriveProvider = freeDriveProvider;
			_multiPlayerMenu = multiPlayerMenu.Value;
			_garageController = garageController;
		}

		private void Awake()
		{
			_collider = GetComponent<BoxCollider>();
			_carLayerMask = LayerMask.GetMask("car", "CarBody", "Trailer");
		}

		private void OnTriggerEnter(Collider other)
		{
			if (!FlagsStorage.IsPhotoMode && (other.CompareTag("Car") || other.CompareTag("OnlineCar")))
			{
				Door.Open();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (FlagsStorage.IsPhotoMode || (!other.CompareTag("Car") && !other.CompareTag("OnlineCar")) || FlagsStorage.OutOfCar || DoCollidersIntersect(_collider, other) || FlagsStorage.IsLoadingGarageCars)
			{
				return;
			}
			GameObject gameObject = other.transform.parent.gameObject;
			if (gameObject != V.Car)
			{
				return;
			}
			Transform transform = base.transform;
			Vector3 rhs = gameObject.transform.position - transform.position;
			float magnitude = rhs.magnitude;
			if (Vector3.Dot(transform.forward, rhs) < 0f && magnitude <= 10f)
			{
				if (PhotonNetwork.IsConnected && gameObject != _freeDriveProvider.FirstCarMultiPlayer)
				{
					Door.Close();
					return;
				}
				if (!FlagsStorage.CanEnterGarage)
				{
					V.CoreGameManager.SetMessage(new LocalizedString("Detector.LeaveTunnelFirst"), 3f);
					return;
				}
				FlagsStorage.EnteredForwardInGarageTunnel = Vector3.Dot(_garageController.InnerGarageTunnelChild.transform.forward, gameObject.transform.forward) > 0f;
				_garageController.OnGarageClose();
				if (ScenesHelper.IsMenuSceneActive())
				{
					V.PlaceToSpawn = V.placeToSpawn.GarageEntrance;
					V.CurrentEnvironment = V.placeToSpawn.MainCity;
					_multiPlayerMenu.StartSingleFreeDrive();
				}
				FlagsStorage.CanEnterGarage = false;
			}
			else
			{
				FlagsStorage.CanEnterGarage = true;
			}
			Door.Close();
		}

		private bool DoCollidersIntersect(Collider a, Collider b)
		{
			bool result = Physics.OverlapBoxNonAlloc(a.bounds.center, a.bounds.extents, _intersectColliders, a.transform.rotation, _carLayerMask, QueryTriggerInteraction.Ignore) != 0 && Enumerable.Contains(_intersectColliders, b);
			Array.Clear(_intersectColliders, 0, _intersectColliders.Length);
			return result;
		}
	}
}
