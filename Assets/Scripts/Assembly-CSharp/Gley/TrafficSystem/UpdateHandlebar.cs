using UnityEngine;

namespace Gley.TrafficSystem
{
	public class UpdateHandlebar : MonoBehaviour
	{
		public Transform handlebar;

		private int _listIndex;

		private void Start()
		{
			_listIndex = API.GetVehicleIndex(base.gameObject);
		}

		private void Update()
		{
			handlebar.localEulerAngles = new Vector3(0f, 0f, API.GetSteeringAngle(_listIndex));
		}
	}
}
