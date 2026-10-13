using CPMEngine;
using Photon.Pun;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer
{
	public class CarComponentsMultiplayer
	{
		public GameObject GameObject;

		public Transform Transform;

		public Axles Axles;

		public VehicleData VehicleData;

		public Dragy Dragy;

		public Rigidbody Rigidbody;

		public CarAIController CarAIController;

		public Powertrain Powertrain;

		public PhotonView PhotonView;

		public CarComponentsMultiplayer(GameObject car)
		{
			if (!(car == null))
			{
				GameObject = car;
				Transform = car.transform;
				Axles = car.GetComponent<Axles>();
				VehicleData = car.GetComponent<VehicleData>();
				Dragy = car.GetComponent<Dragy>();
				Rigidbody = car.GetComponent<Rigidbody>();
				CarAIController = car.GetComponent<CarAIController>();
				Powertrain = car.GetComponent<Powertrain>();
				PhotonView = car.GetComponent<PhotonView>();
			}
		}
	}
}
