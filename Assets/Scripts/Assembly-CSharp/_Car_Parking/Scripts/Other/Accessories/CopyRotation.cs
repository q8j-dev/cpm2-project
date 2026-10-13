using UnityEngine;

namespace _Car_Parking.Scripts.Other.Accessories
{
	public class CopyRotation : MonoBehaviour
	{
		public Transform objectToCopyRotation;

		public void Update()
		{
			base.transform.rotation = objectToCopyRotation.rotation;
		}
	}
}
