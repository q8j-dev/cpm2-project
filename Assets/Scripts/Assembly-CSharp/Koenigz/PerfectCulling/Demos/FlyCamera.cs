using UnityEngine;

namespace Koenigz.PerfectCulling.Demos
{
	public class FlyCamera : MonoBehaviour
	{
		[SerializeField]
		[Range(30f, 150f)]
		private float MouseSensitivity = 90f;

		private float m_rotationX = default;

		private float m_rotationY;

		private void LateUpdate()
		{
			_ = Time.deltaTime;
			m_rotationY = Mathf.Clamp(m_rotationY, -90f, 90f);
			base.transform.localRotation = Quaternion.AngleAxis(m_rotationX, Vector3.up) * Quaternion.AngleAxis(m_rotationY, Vector3.left);
		}
	}
}
