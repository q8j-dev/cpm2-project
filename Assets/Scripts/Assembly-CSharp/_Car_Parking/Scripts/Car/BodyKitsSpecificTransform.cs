using UnityEngine;

namespace _Car_Parking.Scripts.Car
{
	public class BodyKitsSpecificTransform : MonoBehaviour
	{
		[SerializeField]
		private string[] ID = default;

		public bool ContainsID(string id)
		{
			return ID.Contains(id);
		}

		public Vector3 GetGlobalPosition(string id)
		{
			return base.transform.position;
		}

		public Quaternion GetGlobalRotation(string id)
		{
			return base.transform.rotation;
		}

		public Vector3 GetScale(string id)
		{
			return base.transform.localScale;
		}
	}
}
