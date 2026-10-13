using Service.Cargo;
using UnityEngine;

namespace Service
{
	public class FakeWheel : MonoBehaviour
	{
		public Transform referenceWheel;

		private Transform fakeWheel;

		private WheelCollider wheelCollider;

		private float rotation;

		private Vector3 rotationV3;

		private WheelController wheelController;

		private void Awake()
		{
			fakeWheel = base.transform;
			wheelCollider = GetComponentInParent<WheelCollider>();
			wheelController = GetComponentInParent<WheelController>();
		}

		private void Update()
		{
			if (wheelCollider.enabled)
			{
				wheelCollider.motorTorque = 1E-05f;
				wheelCollider.GetWorldPose(out var pos, out var _);
				Vector3 position = fakeWheel.position;
				fakeWheel.position = new Vector3(position.x, pos.y, position.z);
				if (Time.frameCount % wheelController.frame == 0)
				{
					rotation += wheelController.rotateModelByTransformMovement() * Time.deltaTime;
					if (float.IsNaN(rotation))
					{
						rotation = 0f;
					}
					else
					{
						fakeWheel.localRotation = Quaternion.Euler(57.29578f * rotation, 0f, 0f);
					}
				}
			}
			else if ((bool)referenceWheel)
			{
				rotationV3 = referenceWheel.rotation.eulerAngles;
				rotationV3 = new Vector3(rotationV3.x, 0f, 0f);
				fakeWheel.transform.localRotation = Quaternion.Euler(rotationV3);
			}
		}
	}
}
