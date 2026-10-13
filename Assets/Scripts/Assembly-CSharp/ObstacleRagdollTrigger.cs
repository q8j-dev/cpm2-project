using CPM.Obstacles;
using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Person;

[RequireComponent(typeof(Collider))]
[DisallowMultipleComponent]
public class ObstacleRagdollTrigger : MonoBehaviour
{
	private IRagdollImpact _impact;

	private void Awake()
	{
		_impact = GetComponentInParent<IRagdollImpact>();
		if (_impact == null)
		{
			Debug.LogError("[ObstacleRagdollTrigger] " + base.name + ": no obstacle implementing IRagdollImpact on this object or above it - nothing can decide the force.", this);
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		TryRagdoll(other);
	}

	private void OnTriggerStay(Collider other)
	{
		TryRagdoll(other);
	}

	private void TryRagdoll(Collider other)
	{
		if (_impact != null && IsLocalPlayerOnFoot(other))
		{
			GameObject driverPerson = V.DriverPerson;
			if (_impact.TryGetRagdollImpact(other.bounds.center, out var impulse))
			{
				Vector3 to = Vector3.ProjectOnPlane(impulse, Vector3.up);
				float x = ((to.sqrMagnitude > 1E-06f) ? Vector3.SignedAngle(driverPerson.transform.forward, to, Vector3.up) : 0f);
				NetworkedRagdoll.Enable(driverPerson, new Vector3(x, 0f, impulse.magnitude), autoStandUp: false);
			}
		}
	}

	private static bool IsLocalPlayerOnFoot(Collider other)
	{
		Rigidbody attachedRigidbody = other.attachedRigidbody;
		if ((bool)attachedRigidbody && (bool)V.DriverPerson && attachedRigidbody.gameObject == V.DriverPerson)
		{
			return FlagsStorage.OutOfCar;
		}
		return false;
	}
}
