using UnityEngine;

namespace _Car_Parking.Scripts.Drag
{
	[DefaultExecutionOrder(-1)]
	public class CarMiniMapTransformHelper : MonoBehaviour
	{
		private void LateUpdate()
		{
			base.transform.rotation = Quaternion.LookRotation(new Vector3(base.transform.parent.forward.x, 0f, base.transform.parent.forward.z), Vector3.up);
		}
	}
}
