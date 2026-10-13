using UnityEngine;

namespace Service.Abstract
{
	public abstract class JointHandler : MonoBehaviour, IJointHandler
	{
		[SerializeField]
		protected Collider collider;

		[SerializeField]
		protected GameObject prefab;

		public GameObject trigger;

		public virtual void CreateTrigger()
		{
			if (!(trigger != null))
			{
				prefab = (GameObject)Resources.Load("_Services/Delivery/CarTrigger");
				collider = GetComponent<VehicleData>().Collider;
				trigger = Object.Instantiate(prefab, Vector3.zero, base.transform.rotation, base.transform);
			}
		}
	}
}
