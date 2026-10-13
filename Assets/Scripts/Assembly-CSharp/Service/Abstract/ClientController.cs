using GPS;
using Photon.Pun;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace Service.Abstract
{
	public abstract class ClientController : MonoBehaviourPunCallbacks, IClientController
	{
		[SerializeField]
		protected GameObject owner;

		[SerializeField]
		protected GameObject car;

		[SerializeField]
		protected CLIENT_STATE state;

		protected Quaternion initialRotation;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		public GameObject Car
		{
			get
			{
				return car;
			}
			set
			{
				car = value;
			}
		}

		public GameObject Owner => owner;

		[Inject]
		private void Construct(OnlineCollectionsProvider onlineCollectionsProvider)
		{
			_onlineCollectionsProvider = onlineCollectionsProvider;
		}

		public virtual void ChangeState(CLIENT_STATE state)
		{
			this.state = CLIENT_STATE.NONE;
			this.state = state;
		}

		public virtual void SetState(CLIENT_STATE state)
		{
			this.state = state;
		}

		public virtual CLIENT_STATE GetState()
		{
			return state;
		}

		public virtual void Set(GameObject owner, GameObject car)
		{
			this.owner = owner;
			this.car = car;
		}

		protected void SetCar(Transform carTransform)
		{
			Car = carTransform.gameObject;
		}

		protected void SetCar(int? carID = null)
		{
			if (!carID.HasValue)
			{
				if (!(car != null))
				{
					GameObject currentCar = GetPlayerByOwnerID(base.photonView.OwnerActorNr).GetComponent<MultiPersonControl>().currentCar;
					Car = currentCar;
				}
			}
			else
			{
				Car = PhotonView.Find(carID.GetValueOrDefault()).gameObject;
			}
		}

		protected GameObject GetPlayerByOwnerID(int ownerId)
		{
			MultiPersonControl playerByOwnerActorNr = _onlineCollectionsProvider.GetPlayerByOwnerActorNr(ownerId);
			if ((bool)playerByOwnerActorNr)
			{
				return playerByOwnerActorNr.gameObject;
			}
			if (V.DriverPerson.GetComponent<PhotonView>().OwnerActorNr == ownerId)
			{
				return V.DriverPerson;
			}
			return null;
		}

		public virtual void Attach(GameObject car)
		{
		}

		public virtual void Attach(Transform carTransform, Vector3? pos = null)
		{
		}

		public virtual void Detach(string message = "", bool showMessage = false)
		{
		}

		public void ResetRotation()
		{
			int num = GPS.GPSTracker.FindRandomClosestPoint(base.transform.localPosition);
			if (num != -1)
			{
				NodePosition position = GPS.GPSTracker.nodes[num].position;
				base.transform.position = new Vector3(position.x, position.y, position.z) + V.GlobalOffset + Vector3.up * 3f;
				uint[] next = GPS.GPSTracker.nodes[num].next;
				if (next != null && next.Length != 0)
				{
					position = GPS.GPSTracker.nodes[(int)next[0]].position;
					base.transform.LookAt(new Vector3(position.x, position.y, position.z) + V.GlobalOffset + Vector3.up * 3f);
				}
				else
				{
					base.transform.eulerAngles = Vector3.zero;
				}
			}
			else
			{
				base.transform.position += Vector3.up;
				base.transform.rotation = initialRotation;
			}
		}

		public void DestroyRPC()
		{
			Object.Destroy(base.gameObject);
		}
	}
}
