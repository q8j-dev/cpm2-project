using Cysharp.Threading.Tasks;
using UnityEngine;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class JoinDragRacingTrigger : MonoBehaviour
	{
		public int id;

		public NewRacingMultiplayerController newRacingMultiplayerController;

		public DragRacingMultiplayerTree.PositionOnTree PositionOnTree = DragRacingMultiplayerTree.PositionOnTree.None;

		public GameObject currentCar;

		public DragRacingMultiplayerTree tree;

		public MeshRenderer render;

		private bool stopedCar;

		private void Start()
		{
			render = GetComponent<MeshRenderer>();
		}

		public async UniTask CarEnteredRacingTrigger(GameObject car)
		{
			currentCar = car;
			stopedCar = false;
			Rigidbody rb = car.GetComponent<Rigidbody>();
			while (rb.velocity.magnitude > 0.1f)
			{
				await UniTask.Delay(500);
			}
			stopedCar = true;
			if (!(currentCar == null))
			{
				newRacingMultiplayerController.OnCarEnterRacingTrigger(car.gameObject, this);
				if ((bool)render)
				{
					render.enabled = false;
				}
			}
		}

		public bool CheckCarInsideTrigger(GameObject car)
		{
			Collider collider = car.GetComponent<VehicleData>().Collider;
			Collider component = GetComponent<Collider>();
			Collider[] array = Physics.OverlapBox(collider.bounds.center, collider.bounds.extents, collider.transform.rotation);
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == component)
				{
					return true;
				}
			}
			return false;
		}

		public void CarExitRacingTrigger(GameObject car)
		{
		}
	}
}
