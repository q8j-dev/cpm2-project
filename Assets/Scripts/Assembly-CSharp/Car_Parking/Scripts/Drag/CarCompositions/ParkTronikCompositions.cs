using System.Collections.Generic;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Infrastructure;

namespace Car_Parking.Scripts.Drag.CarCompositions
{
	public class ParkTronikCompositions
	{
		private readonly VehicleData _vehicleData;

		private readonly Powertrain _powertrain;

		private readonly IPrefabFactory _prefabFactory;

		public List<ParkTronicTrigger> ParkTronicTriggers { get; } = new List<ParkTronicTrigger>();

		public GameObject ParktronikParent { get; set; }

		public GameObject FrontMiddle { get; set; }

		public GameObject RearMiddle { get; set; }

		public ParkTronikCompositions(VehicleData vehicleData, Powertrain powertrain, IPrefabFactory prefabFactory)
		{
			_vehicleData = vehicleData;
			_powertrain = powertrain;
			_prefabFactory = prefabFactory;
		}

		public void Init()
		{
			TryCreateParkingTriggers();
			TryDisableParktronikParent();
		}

		private void TryCreateParkingTriggers()
		{
			ParkTronicTriggers.Clear();
			if (!_vehicleData.isTraffiCar && !_vehicleData.isTruck && (bool)_vehicleData.Collider && _prefabFactory != null)
			{
				GameObject gameObject = new GameObject();
				gameObject.name = "ParkTronic";
				gameObject.transform.parent = _vehicleData.Collider.transform;
				ParktronikParent = gameObject;
				ParktronikParent.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
				Bounds getWeightBounds = _powertrain.GetWeightBounds;
				Vector3 localPosition = new Vector3(getWeightBounds.center.x, getWeightBounds.min.y - _vehicleData.Collider.transform.localPosition.y, getWeightBounds.max.z);
				Vector3 localPosition2 = new Vector3(getWeightBounds.center.x, getWeightBounds.min.y - _vehicleData.Collider.transform.localPosition.y, getWeightBounds.min.z);
				FrontMiddle = _prefabFactory.Instantiate(_vehicleData.FrontMiddlePark, ParktronikParent.transform);
				FrontMiddle.transform.SetLocalPositionAndRotation(localPosition, Quaternion.identity);
				if (FrontMiddle.TryGetComponent<ParkTronicEntity>(out var component))
				{
					ParkTronicTriggers.AddRange(component.Triggers);
				}
				RearMiddle = _prefabFactory.Instantiate(_vehicleData.RearMiddlePark, ParktronikParent.transform);
				RearMiddle.transform.SetLocalPositionAndRotation(localPosition2, Quaternion.Euler(0f, 180f, 0f));
				if (RearMiddle.TryGetComponent<ParkTronicEntity>(out var component2))
				{
					ParkTronicTriggers.AddRange(component2.Triggers);
				}
			}
		}

		public void TrySetActiveParktronikParent(bool active)
		{
			if ((bool)ParktronikParent)
			{
				ParktronikParent.SetActive(active);
			}
		}

		private void TryDisableParktronikParent()
		{
			if (!_vehicleData.isTraffiCar && !_vehicleData.isTruck && !(V.Car == _vehicleData.gameObject))
			{
				TrySetActiveParktronikParent(active: false);
			}
		}
	}
}
