using UnityEngine;

namespace _Car_Parking.WwisePrefabs.WiseSoundScripts
{
	public class WwiseAmbiencePart : MonoBehaviour
	{
		private WwiseAmbienceStateWithParts _wwiseAmbienceStateWithParts;

		public GameObject MainCamera { get; private set; }

		private void Start()
		{
			_wwiseAmbienceStateWithParts = (base.transform.parent ? base.transform.parent.GetComponent<WwiseAmbienceStateWithParts>() : null);
		}

		private void OnTriggerEnter(Collider other)
		{
			if ((bool)_wwiseAmbienceStateWithParts && other.CompareTag(_wwiseAmbienceStateWithParts.MainCameraTag))
			{
				MainCamera = other.gameObject;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			if ((bool)_wwiseAmbienceStateWithParts && other.CompareTag(_wwiseAmbienceStateWithParts.MainCameraTag))
			{
				MainCamera = null;
			}
		}
	}
}
