using UnityEngine;

namespace _Car_Parking.Scripts.Police
{
	public class PoliceRoobarSpecificTransform : MonoBehaviour
	{
		[SerializeField]
		private Transform Main = default;

		public Vector3 GetGlobalPosition(int index)
		{
			return Main.position;
		}

		public Quaternion GetGlobalRotation(int index)
		{
			return Main.rotation;
		}

		public Vector3 GetScale(int index)
		{
			return Main.localScale;
		}
	}
}
