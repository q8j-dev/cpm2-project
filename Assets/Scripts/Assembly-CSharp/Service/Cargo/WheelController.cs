using UnityEngine;

namespace Service.Cargo
{
	public class WheelController : MonoBehaviour
	{
		public Transform wheelTransform;

		[SerializeField]
		private WheelCollider wheelCollider = default;

		[SerializeField]
		private MeshCollider meshCollider = default;

		public int frame = 1;

		private Vector3 prevPos;

		private void OnEnable()
		{
			wheelCollider.enabled = true;
			meshCollider.enabled = false;
		}

		private void OnDisable()
		{
			wheelCollider.enabled = false;
			meshCollider.enabled = true;
		}

		public float rotateModelByTransformMovement()
		{
			float num = Vector2.Distance(new Vector2(base.transform.position.x, base.transform.position.z), new Vector2(prevPos.x, prevPos.z)) / Time.deltaTime;
			int num2 = 1;
			Vector3 vector = base.transform.position + base.transform.forward;
			float num3 = Vector3.Distance(vector, base.transform.position);
			float num4 = Vector3.Distance(prevPos, vector);
			if (num3 - num4 > 0f)
			{
				num2 = -1;
			}
			prevPos = base.transform.position;
			return num * 2f * (float)num2;
		}
	}
}
