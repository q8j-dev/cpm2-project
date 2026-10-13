using UnityEngine;

namespace Obi
{
	[RequireComponent(typeof(Camera))]
	public class LookAroundCamera : MonoBehaviour
	{
		public struct CameraShot
		{
			public Vector3 position;

			public Quaternion rotation;

			public Vector3 up;

			public float fieldOfView;

			public CameraShot(Vector3 position, Quaternion rotation, Vector3 up, float fieldOfView)
			{
				this.position = position;
				this.rotation = rotation;
				this.up = up;
				this.fieldOfView = fieldOfView;
			}
		}

		private Camera cam;

		private CameraShot currentShot;

		public float movementSpeed = 5f;

		public float rotationSpeed = 8f;

		public float translationResponse = 10f;

		public float rotationResponse = 10f;

		public float fovResponse = 10f;

		private void Awake()
		{
			cam = GetComponent<Camera>();
			currentShot = new CameraShot(base.transform.position, base.transform.rotation, base.transform.up, cam.fieldOfView);
		}

		private void LookAt(Vector3 position, Vector3 up)
		{
			currentShot.up = up;
			currentShot.rotation = Quaternion.LookRotation(position - currentShot.position, currentShot.up);
		}

		private void UpdateShot()
		{
			base.transform.position = Vector3.Lerp(base.transform.position, currentShot.position, translationResponse * Time.deltaTime);
			base.transform.rotation = Quaternion.Slerp(base.transform.rotation, currentShot.rotation, rotationResponse * Time.deltaTime);
			cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, currentShot.fieldOfView, fovResponse * Time.deltaTime);
		}

		private void LateUpdate()
		{
			_ = Vector3.zero;
		}
	}
}
