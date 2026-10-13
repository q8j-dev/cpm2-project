using UnityEngine;
using _Car_Parking;
using _Car_Parking.Scripts.Person;

public class RagdollRestrictTrigger : MonoBehaviour
{
	private RagdollCPM _ragdoll;

	public void Init(RagdollCPM ragdoll)
	{
		_ragdoll = ragdoll;
	}

	private void OnTriggerEnter(Collider other)
	{
		if ((bool)_ragdoll && _ragdoll.isRagdoll && !_ragdoll.onlinePerson && !FlagsStorage.IsPhotoMode && other.gameObject.CompareTag("restrict"))
		{
			DriverActionController driverActionController = _ragdoll.driverActionController;
			if ((bool)driverActionController && !(driverActionController.gameObject != V.DriverPerson))
			{
				_ragdoll.ReleaseForTeleport();
				driverActionController.OnTriggerMutual(other).Forget();
			}
		}
	}
}
