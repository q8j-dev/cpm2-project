using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

public class ParkingFreeSpaceSensor : MonoBehaviour
{
	private Material _currentMaterial;

	private GlobalCollectionsProvider _collectionsProvider;

	[Inject]
	public void Construct(GlobalCollectionsProvider collectionsProvider)
	{
		_collectionsProvider = collectionsProvider;
	}

	private void Start()
	{
		MeshRenderer component = base.gameObject.GetComponent<MeshRenderer>();
		_currentMaterial = new Material(component.material.shader);
		component.material = _currentMaterial;
		_collectionsProvider.RegisterFreeSpaceSensors(this);
		CheckForSensor();
	}

	private void OnDestroy()
	{
		Object.Destroy(_currentMaterial);
		_collectionsProvider.UnregisterFreeSpaceSensors(this);
	}

	public void CheckForSensor()
	{
		Vector3 direction = base.transform.TransformDirection(Vector3.right);
		Vector3 position = base.transform.position;
		LayerMask layerMask = -1;
		SetLampColor(Color.green);
		if (Physics.Raycast(position, direction, out var hitInfo, 5f, layerMask.value, QueryTriggerInteraction.Ignore) && hitInfo.distance < 2f)
		{
			SetLampColor(Color.red);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.isTrigger)
		{
			SetLampColor(Color.red);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (!other.isTrigger)
		{
			SetLampColor(Color.green);
		}
	}

	public void SetLampColor(Color color)
	{
		if ((bool)_currentMaterial)
		{
			_currentMaterial.color = color;
		}
	}
}
