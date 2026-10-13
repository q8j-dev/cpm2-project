using UnityEngine;

namespace Service.Delivery
{
	public class Test : MonoBehaviour
	{
		[SerializeField]
		private new Collider collider;

		[SerializeField]
		private GameObject prefab;

		[SerializeField]
		private Rigidbody rb;

		public GameObject trigger;

		public Transform triggerTransform
		{
			get
			{
				if (!trigger)
				{
					CreateTrigger();
				}
				return trigger.transform;
			}
		}

		private void Start()
		{
			rb = GetComponent<Rigidbody>();
		}

		public void CreateTrigger()
		{
			if (!(trigger != null))
			{
				prefab = (GameObject)Resources.Load("_Services/Delivery/CarTrigger");
				collider = GetComponent<VehicleData>().Collider;
				trigger = Object.Instantiate(prefab, Vector3.zero, base.transform.rotation, base.transform);
				trigger.transform.localPosition = -Vector3.forward * 5f;
				Vector3 position = collider.ClosestPoint(trigger.transform.position);
				trigger.transform.position = position;
			}
		}
	}
}
