using UnityEngine;
using UnityEngine.Events;

namespace _Car_Parking.Scripts.Other
{
	public class CollisionChecker : MonoBehaviour
	{
		[SerializeField]
		private string NeedTag = default;

		[SerializeField]
		private UnityEvent OnTriggerEnterInvoke = default;

		[SerializeField]
		private UnityEvent OnTriggerExitInvoke = default;

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag(NeedTag) && other.name.Contains("car"))
			{
				OnTriggerEnterInvoke?.Invoke();
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if (other.CompareTag(NeedTag) && other.name.Contains("car"))
			{
				OnTriggerExitInvoke?.Invoke();
			}
		}
	}
}
