using UnityEngine;

namespace Service.Cargo
{
	public class ParkingTrigger : MonoBehaviour
	{
		[SerializeField]
		private FinishCollisionDetector parent;

		private int triggerEntered;

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Trailer"))
			{
				if (triggerEntered == 0)
				{
					parent.ParkingTriggers++;
				}
				triggerEntered++;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag("Trailer"))
			{
				triggerEntered--;
				if (triggerEntered == 0)
				{
					parent.ParkingTriggers--;
				}
			}
		}
	}
}
