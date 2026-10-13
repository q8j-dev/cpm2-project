using CPMEngine;
using UnityEngine;

public struct CarComponents
{
	public GameObject GameObject;

	public Transform Transform;

	public Axles Axles;

	public VehicleData VehicleData;

	public Dragy Dragy;

	public Rigidbody Rigidbody;

	public CarAIController CarAIController;

	public Powertrain Powertrain;

	public CarComponents(GameObject car)
	{
		if (!car)
		{
			GameObject = null;
			Transform = null;
			Axles = null;
			VehicleData = null;
			Dragy = null;
			Rigidbody = null;
			CarAIController = null;
			Powertrain = null;
		}
		else
		{
			GameObject = car;
			Transform = car.transform;
			Axles = car.GetComponent<Axles>();
			VehicleData = car.GetComponent<VehicleData>();
			Dragy = car.GetComponent<Dragy>();
			Rigidbody = car.GetComponent<Rigidbody>();
			CarAIController = car.GetComponent<CarAIController>();
			Powertrain = car.GetComponent<Powertrain>();
		}
	}
}
