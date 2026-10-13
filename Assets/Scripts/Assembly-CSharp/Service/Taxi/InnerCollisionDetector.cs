using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Abstract;
using UnityEngine;

namespace Service.Taxi
{
	public class InnerCollisionDetector : CollisionDetector
	{
		[SerializeField]
		private List<CollisionDetector> triggers;

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
			if (!other.transform.parent.GetComponent<VehicleData>().VehicleMembersController.HasPlaceForPassenger)
			{
				return false;
			}
			foreach (CollisionDetector trigger in triggers)
			{
				if ((bool)trigger.gameObject)
				{
					Object.Destroy(trigger.gameObject);
				}
			}
			GetComponentInParent<ClientController>().ChangeState(CLIENT_STATE.SIT_IN).Forget();
			if (PhotonNetwork.IsConnected)
			{
				photonView.RpcToDefaultGroup("GetInCar", RpcTarget.All, playerView.ViewID);
			}
			else
			{
				Global.TaskManager.TaxiManager.GetInCar(playerView.ViewID);
			}
			base.gameObject.SetActive(value: false);
			return true;
		}

		public override bool TriggerExit(Collider other)
		{
			if (!base.TriggerExit(other))
			{
				return false;
			}
			base.gameObject.SetActive(value: true);
			return true;
		}

		protected override bool isCar(Collider other)
		{
			ClientController componentInParent = GetComponentInParent<ClientController>();
			if ((bool)componentInParent && componentInParent.Car != other.transform.parent.gameObject)
			{
				return false;
			}
			return base.isCar(other);
		}
	}
}
