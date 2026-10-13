using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Abstract;
using UnityEngine;

namespace Service.Cargo
{
	public class OuterCollisionDetector : CollisionDetector
	{
		[SerializeField]
		private Transform trailer;

		[SerializeField]
		private TrailerController trailerController;

		public override void Start()
		{
			base.Start();
			trailer = base.transform.parent.parent;
			trailerController = trailer.GetComponent<TrailerController>();
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
			if (trailerController.photonView.IsMine && other.gameObject == trailerController.Owner)
			{
				V.CoreGameManager.MenuLevelParent.serviceLabels.resetTrailer.enabled = true;
			}
			if (!isValid(other))
			{
				return false;
			}
			if (trailerController.GetState() == CLIENT_STATE.ATTACHED)
			{
				return false;
			}
			JointHandler componentInParent = other.GetComponentInParent<JointHandler>();
			if (componentInParent != null)
			{
				componentInParent.CreateTrigger(TYPE.CARGO);
			}
			return true;
		}

		protected override bool isCar(Collider other)
		{
			if (!other.CompareTag("Car") && !other.CompareTag("OnlineCar"))
			{
				return false;
			}
			return true;
		}

		protected override bool isMine(Collider other)
		{
			if (!PhotonNetwork.IsConnected)
			{
				return true;
			}
			GameObject currentCar = V.DriverPerson.GetComponent<MultiPersonControl>().currentCar;
			GameObject gameObject = other.transform.parent.gameObject;
			if (trailerController.Owner != V.DriverPerson)
			{
				return false;
			}
			if (currentCar != gameObject)
			{
				return false;
			}
			return true;
		}

		protected override bool isValid(Collider other)
		{
			bool flag = isCar(other);
			if (!flag)
			{
				return flag;
			}
			flag &= isMine(other);
			return flag & (Global.TaskManager.Task != null);
		}

		public override bool TriggerExit(Collider other)
		{
			if (!isValid(other))
			{
				return false;
			}
			V.CoreGameManager.MenuLevelParent.serviceLabels.resetTrailer.enabled = true;
			return true;
		}
	}
}
