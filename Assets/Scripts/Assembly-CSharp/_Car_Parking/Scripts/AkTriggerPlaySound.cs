using AK.Wwise;
using UnityEngine;

namespace _Car_Parking.Scripts
{
	public class AkTriggerPlaySound : MonoBehaviour
	{
		private const string MainCameraTag = "MainCamera";

		public AK.Wwise.Event PlayEvent = new AK.Wwise.Event();

		public AK.Wwise.Event StopEvent = new AK.Wwise.Event();

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.CompareTag("MainCamera"))
			{
				PlayEvent.Post(base.gameObject);
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.gameObject.CompareTag("MainCamera"))
			{
				StopEvent.Post(base.gameObject);
			}
		}

		private void OnDisable()
		{
			StopEvent.Post(base.gameObject);
		}

		private void OnDestroy()
		{
			StopEvent.Post(base.gameObject);
		}
	}
}
