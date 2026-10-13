using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Service.Abstract;
using UnityEngine;

namespace Service.Cargo
{
	public class ConnectionTrigger : CollisionDetector
	{
		[SerializeField]
		private Transform trailer;

		private void OnTriggerEnter(Collider other)
		{
			_ = TriggerEnter(other);
		}

		private void OnTriggerExit(Collider other)
		{
			base.TriggerExit(other);
		}

		public override async UniTask<bool> TriggerEnter(Collider other)
		{
			if (!(await base.TriggerEnter(other)))
			{
				return false;
			}
			Transform transform = other.transform;
			Transform parent = transform.parent;
			ConfigurableJoint component = parent.gameObject.GetComponent<ConfigurableJoint>();
			VehicleData component2 = parent.gameObject.GetComponent<VehicleData>();
			if ((bool)component && component.connectedBody != null)
			{
				return false;
			}
			TrailerController component3 = trailer.GetComponent<TrailerController>();
			if (!component2.isTruck && component3.type == TRUCK_TYPE.SEMI_TRAILER)
			{
				return false;
			}
			Player owner = component3.Owner.GetComponent<PhotonView>().Owner;
			component3.SetTriggerPosition(transform.localPosition);
			component3.photonView.TransferOwnership(owner);
			component3.Attach(parent);
			return true;
		}

		public void Init()
		{
			serviceType = TYPE.CARGO;
			trailer = base.transform.parent.parent;
		}
	}
}
