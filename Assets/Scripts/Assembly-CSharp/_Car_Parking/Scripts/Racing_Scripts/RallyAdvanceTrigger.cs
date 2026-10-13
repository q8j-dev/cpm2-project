using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallyAdvanceTrigger : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			RallyCar rallyCar = ((other.attachedRigidbody != null) ? other.attachedRigidbody.GetComponent<RallyCar>() : other.GetComponent<RallyCar>());
			if (rallyCar != null)
			{
				rallyCar.TeleportToNextTrigger();
			}
		}
	}
}
