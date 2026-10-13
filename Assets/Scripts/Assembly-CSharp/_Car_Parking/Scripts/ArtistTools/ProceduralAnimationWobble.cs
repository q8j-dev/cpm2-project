using UnityEngine;
using _Car_Parking.Scripts.ArtistTools.Trackables;

namespace _Car_Parking.Scripts.ArtistTools
{
	public class ProceduralAnimationWobble : TrackableWithAspects
	{
		[Header("Params for X")]
		[SerializeField]
		[Range(0f, 10f)]
		[Space(10f)]
		private float WobbleSpeedX = 1.5f;

		[SerializeField]
		[Range(0f, 10f)]
		private float AmplitudeX = 1f;

		[SerializeField]
		[Range(0f, 100f)]
		private float PhaseOffsetX;

		[SerializeField]
		[Range(0f, 10f)]
		[Header("Params for Z")]
		[Space(10f)]
		private float WobbleSpeedZ = 1.5f;

		[SerializeField]
		[Range(0f, 10f)]
		private float AmplitudeZ = 1f;

		[Range(0f, 100f)]
		[SerializeField]
		private float PhaseOffsetZ;

		private Quaternion _initRotation;

		private void Awake()
		{
			_initRotation = base.transform.rotation;
		}

		private void Update()
		{
			float time = Time.time;
			float x = Mathf.Sin((time + PhaseOffsetX) * WobbleSpeedX) * AmplitudeX;
			float z = Mathf.Sin((time + PhaseOffsetZ) * WobbleSpeedZ) * AmplitudeZ;
			Quaternion quaternion = Quaternion.Euler(x, 0f, z);
			base.transform.localRotation = _initRotation * quaternion;
		}
	}
}
