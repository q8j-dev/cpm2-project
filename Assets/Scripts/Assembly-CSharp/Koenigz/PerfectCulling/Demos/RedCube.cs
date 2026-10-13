using UnityEngine;

namespace Koenigz.PerfectCulling.Demos
{
	public class RedCube : MonoBehaviour
	{
		private void OnEnable()
		{
			Debug.Log("I'm the RedCube script and I was just enabled! Camera: " + ((Camera.current == null) ? "null" : Camera.current.name), base.gameObject);
		}

		private void OnDisable()
		{
			Debug.Log("I'm the RedCube script and I was just disabled! Camera: " + ((Camera.current == null) ? "null" : Camera.current.name), base.gameObject);
		}
	}
}
