using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Service.Abstract;
using UnityEngine;

namespace Service.Delivery
{
	public class ConnectionTrigger : CollisionDetector
	{
		[SerializeField]
		private Transform trailer;

		public void Init()
		{
			serviceType = TYPE.DELIVERY;
			trailer = base.transform.parent.parent;
		}

		private void OnTriggerEnter(Collider other)
		{
			TriggerEnter(other);
		}

		private void OnTriggerExit(Collider other)
		{
			TriggerExit(other);
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
			if ((bool)component && component.connectedBody != null)
			{
				return false;
			}
			TrailerController component2 = trailer.GetComponent<TrailerController>();
			Player owner = component2.Owner.GetComponent<PhotonView>().Owner;
			component2.SetTriggerPosition(transform.localPosition);
			component2.photonView.TransferOwnership(owner);
			component2.Attach(parent);
			return true;
		}
	}
}
