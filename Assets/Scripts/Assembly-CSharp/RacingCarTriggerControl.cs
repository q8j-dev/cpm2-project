using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Car_Parking.Scripts;

public class RacingCarTriggerControl : MonoBehaviour
{
	public Collider boxCollider;

	public CarAIController CarAIController;

	private Rigidbody _rb;

	private VehicleData _vehicle;

	private Collider _col;

	private Vector3 originPosition;

	public float speedSizeK = 1f;

	public float speedSizePos = 0.5f;

	public List<GameObject> carsOnTrigger;

	private List<int> _layerToCheck;

	private void Start()
	{
		GetComponent<MeshRenderer>().enabled = false;
		boxCollider = GetComponent<BoxCollider>();
		boxCollider.isTrigger = true;
		carsOnTrigger = new List<GameObject>();
		_layerToCheck = new List<int>
		{
			LayerViaName.CarBody,
			LayerViaName.CarLayer
		};
	}

	private void UpdateTransform()
	{
		base.transform.localScale = new Vector3(2f, 1f, _rb.velocity.magnitude * speedSizeK + 2f);
		base.transform.localPosition = originPosition + Vector3.forward * (_rb.velocity.magnitude * speedSizePos);
	}

	private void CheckManuallyTrigger()
	{
		if (Physics.OverlapBox(base.transform.position, base.transform.localScale / 2f, base.transform.rotation).Count((Collider col) => _layerToCheck.Contains(col.gameObject.layer)) == 0)
		{
			carsOnTrigger.Clear();
			CarAIController.CarOnFront = false;
		}
	}

	public void Initialize(VehicleData vehicleData)
	{
		_vehicle = vehicleData;
		_col = _vehicle.Collider;
		if (!_col)
		{
			_col = vehicleData.transform.Find("Collider").GetComponent<Collider>();
		}
		base.transform.parent = vehicleData.transform;
		originPosition = _col.transform.localPosition + Vector3.up * 0.5f + Vector3.forward * (_col.bounds.extents.z * 2f);
		_rb = vehicleData.GetComponent<Rigidbody>();
		CarAIController = _vehicle.GetComponent<CarAIController>();
		base.gameObject.name = $"RacingCarTriggerControl_{_vehicle.CarId}";
		base.gameObject.layer = LayerMask.NameToLayer("parktronic");
		UpdateTransform();
	}

	private void Update()
	{
		if (Time.frameCount % 10 == 0 && (bool)_rb)
		{
			UpdateTransform();
			if (CarAIController.CarOnFront && Time.frameCount % 100 != 0)
			{
				CheckManuallyTrigger();
			}
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (_layerToCheck.Contains(other.gameObject.layer))
		{
			if (!carsOnTrigger.Contains(other.gameObject))
			{
				carsOnTrigger.Add(other.gameObject);
			}
			if (carsOnTrigger.Count > 0 && (bool)CarAIController)
			{
				CarAIController.CarOnFront = true;
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (_layerToCheck.Contains(other.gameObject.layer) && carsOnTrigger.Contains(other.gameObject))
		{
			carsOnTrigger.Remove(other.gameObject);
			if (carsOnTrigger.Count == 0 && (bool)CarAIController)
			{
				CarAIController.CarOnFront = false;
			}
		}
	}
}
