using UnityEngine;

namespace TouchControlsKit
{
	public class Tilt : MonoBehaviour
	{
		[Range(1f, 10f)]
		public float sensitivity = 4f;

		[Range(0f, 90f)]
		public float fullTiltAngle = 25f;

		[Range(-50f, 50f)]
		public float centreAngleOffset;

		public static float forwardAxis { get; private set; }

		public static float sidewaysAxis { get; private set; }

		private void Update()
		{
			if (Input.acceleration != Vector3.zero)
			{
				float value = Mathf.Atan2(Input.acceleration.x, 0f - Input.acceleration.y) * 57.29578f + centreAngleOffset;
				float value2 = Mathf.Atan2(Input.acceleration.z, 0f - Input.acceleration.y) * 57.29578f + centreAngleOffset;
				forwardAxis = (Mathf.InverseLerp(0f - fullTiltAngle, fullTiltAngle, value) * 2f - 1f) * sensitivity;
				sidewaysAxis = (Mathf.InverseLerp(0f - fullTiltAngle, fullTiltAngle, value2) * 2f - 1f) * sensitivity;
			}
			else
			{
				forwardAxis = 0f;
				sidewaysAxis = 0f;
			}
		}
	}
}
