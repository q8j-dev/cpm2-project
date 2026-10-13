using Cysharp.Threading.Tasks;
using Service.Abstract;
using UnityEngine;

namespace Service.Taxi
{
	public class MidCollisionDetector : CollisionDetector
	{
		private void OnTriggerEnter(Collider other)
		{
			TriggerEnter(other);
		}

		public override async UniTask<bool> TriggerEnter(Collider other)
		{
			if (!(await base.TriggerEnter(other)))
			{
				return false;
			}
			if (!other.transform.parent.GetComponent<VehicleData>().VehicleMembersController.HasPlaceForPassenger)
			{
				return false;
			}
			GetComponentInParent<ClientController>().Walk();
			return true;
		}

		private void OnTriggerExit(Collider other)
		{
			TriggerExit(other);
		}

		public override bool TriggerExit(Collider other)
		{
			if (!base.TriggerExit(other))
			{
				return false;
			}
			GetComponentInParent<ClientController>().ChangeState(CLIENT_STATE.WAIT).Forget();
			return true;
		}
	}
}
