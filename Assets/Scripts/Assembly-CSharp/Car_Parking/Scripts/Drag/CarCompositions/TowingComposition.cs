using System;
using UniRx;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.Physics;
using _Car_Parking.Scripts.Localization;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class TowingComposition
	{
		private static readonly Vector3 TowingCarRotation = new Vector3(-3f, 0f, 0f);

		private readonly VehicleData _vehicle;

		private IDisposable _resetTowTriggerDispose;

		public GameObject LoadCar { get; set; }

		public GameObject TowingTruck { get; set; }

		public TowingComposition(VehicleData vehicle)
		{
			_vehicle = vehicle;
		}

		public void TowMultiplayer(GameObject loadCarGo)
		{
			if (!_vehicle || !loadCarGo)
			{
				return;
			}
			VehicleData component = loadCarGo.GetComponent<VehicleData>();
			MultiPlayerPosControl component2 = _vehicle.GetComponent<MultiPlayerPosControl>();
			MultiPlayerPosControl component3 = loadCarGo.GetComponent<MultiPlayerPosControl>();
			if (LoadCar == loadCarGo)
			{
				return;
			}
			Rigidbody component4 = loadCarGo.GetComponent<Rigidbody>();
			Rigidbody component5 = _vehicle.GetComponent<Rigidbody>();
			component4.isKinematic = true;
			component5.mass += component4.mass;
			component.TowingComposition.TowingTruck = _vehicle.gameObject;
			LoadCar = loadCarGo;
			component4.velocity = Vector3.zero;
			component4.angularVelocity = Vector3.zero;
			loadCarGo.transform.parent = _vehicle.transform;
			component3.loadCarLocalTarget = component2.loadCarLocalTarget;
			if ((bool)component.Collider)
			{
				component.Collider.transform.tag = "Car";
				component.Collider.gameObject.layer = LayerViaName.CarLayer;
			}
			float num = 0.05f;
			int carId = component.CarId;
			if (carId == 62 || carId == 59 || carId == 88)
			{
				num = 0.2f;
			}
			float num2 = 0f;
			if ((bool)_vehicle.CarChildsReferences && (bool)_vehicle.CarChildsReferences.PointWheel)
			{
				num2 = _vehicle.CarChildsReferences.PointWheel.position.y;
			}
			float num3 = num2 - component.Collider.transform.position.y;
			float y = loadCarGo.transform.position.y + num3 - num;
			loadCarGo.transform.SetPositionAndRotation(new Vector3(0f, y, -3f), Quaternion.Euler(TowingCarRotation));
			if (!component2.loadCarLocalTarget)
			{
				component2.loadCarLocalTarget = new GameObject("local target for load").transform;
				component2.loadCarLocalTarget.transform.parent = component.transform;
				component2.loadCarLocalTarget.transform.localPosition = Vector3.zero;
			}
			component2.loadCarLocalTarget.transform.localPosition = loadCarGo.transform.localPosition;
			component2.loadCarLocalTarget.transform.localEulerAngles = component.transform.localEulerAngles;
			component3.loadCarLocalTarget = component2.loadCarLocalTarget.transform;
			if (FlagsStorage.IsFreeDriveMode)
			{
				loadCarGo.transform.parent = V.GlobalPoint;
			}
			if (!FlagsStorage.IsPassenger && _vehicle.gameObject == V.Car)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("PlayerDatas.CarLoaded"), 2f);
				if (_vehicle.SpacerOpen)
				{
					_vehicle.FreeDriveProvider.FreeDriveControl.UnloadFromTowCar();
				}
			}
			if (!FlagsStorage.IsPassenger && loadCarGo == V.Car)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("PlayerDatas.CarLoaded"), 2f);
			}
			CarColliders.SetIgnore(_vehicle, component, ignore: true);
			Collider[] towTruckRearCollider = _vehicle.towTruckRearCollider;
			foreach (Collider other in towTruckRearCollider)
			{
				CarColliders.SetIgnore(component, other, ignore: true);
			}
		}

		public void UnTowMultiplayer(GameObject loadCarGo)
		{
			if ((bool)loadCarGo)
			{
				if (FlagsStorage.IsFreeDriveMode)
				{
					loadCarGo.transform.parent = V.GlobalPoint;
				}
				Rigidbody component = loadCarGo.GetComponent<Rigidbody>();
				loadCarGo.GetComponent<VehicleData>().TowingComposition.TowingTruck = null;
				component.isKinematic = false;
				component.ResetInertiaTensor();
				component.ResetCenterOfMass();
				MultiPlayerPosControl component2 = loadCarGo.GetComponent<MultiPlayerPosControl>();
				component2.correctPlayerPos = loadCarGo.transform.position;
				component2.correctPlayerRot = loadCarGo.transform.rotation;
			}
			_resetTowTriggerDispose?.Dispose();
			ConfigurableJoint configurableJoint = (loadCarGo ? loadCarGo.GetComponent<ConfigurableJoint>() : null);
			if ((bool)configurableJoint)
			{
				UnityEngine.Object.Destroy(configurableJoint);
			}
			LoadCar = null;
			Rigidbody component3 = _vehicle.GetComponent<Rigidbody>();
			component3.isKinematic = false;
			component3.velocity = Vector3.zero;
			component3.angularVelocity = Vector3.zero;
			_vehicle.resetingTowTrigger = true;
			_resetTowTriggerDispose = Observable.Timer(TimeSpan.FromSeconds(5.0)).Subscribe((long _) =>
			{
				_vehicle.ResetTowTruckTrigger();
			}).AddTo(_vehicle);
			if ((bool)loadCarGo && (bool)_vehicle)
			{
				VehicleData component4 = loadCarGo.GetComponent<VehicleData>();
				CarColliders.SetIgnore(_vehicle, component4, ignore: false);
				Collider[] towTruckRearCollider = _vehicle.towTruckRearCollider;
				foreach (Collider other in towTruckRearCollider)
				{
					CarColliders.SetIgnore(component4, other, ignore: false);
				}
				ConfigurableJoint component5 = loadCarGo.GetComponent<ConfigurableJoint>();
				if ((bool)component5)
				{
					component5.connectedBody = null;
					UnityEngine.Object.Destroy(component5);
				}
			}
		}
	}
}
