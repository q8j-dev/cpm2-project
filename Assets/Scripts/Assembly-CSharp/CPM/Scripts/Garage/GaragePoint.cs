using UnityEngine;

namespace CPM.Scripts.Garage
{
	public class GaragePoint : MonoBehaviour
	{
		[SerializeField]
		private float CheckDistance = 100f;

		[SerializeField]
		private GarageController Controller;

		[SerializeField]
		private Transform CheckPoint;
	}
}
