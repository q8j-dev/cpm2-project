using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Taxi;
using UniRx;
using UnityEngine;

namespace Service.Abstract
{
	public abstract class CollisionDetector : MonoBehaviour, ICollisionDetector
	{
		[SerializeField]
		protected PhotonView photonView;

		[SerializeField]
		protected PhotonView playerView;

		[SerializeField]
		protected TYPE serviceType;

		private CancellationTokenSource token;

		public virtual void Start()
		{
			photonView = PhotonView.Get(Global.TaskManager);
			(from _ in Observable.Interval(TimeSpan.FromMilliseconds(100.0))
				where V.DriverPerson != null
				select _).Take(1).Subscribe((long _) =>
			{
				playerView = PhotonView.Get(V.DriverPerson);
			}).AddTo(this);
		}

		public virtual async UniTask<bool> TriggerEnter(Collider other)
		{
			if (!isValid(other))
			{
				return false;
			}
			if (Global.TaskManager.Task == null || Global.TaskManager.Task.GetState() == TASK_STATE.END)
			{
				return false;
			}
			if (PhotonNetwork.IsConnected && !other.transform.parent.GetComponent<PhotonView>().IsMine)
			{
				return false;
			}
			token = new CancellationTokenSource();
			CancellationToken cancellationToken = token.Token;
			while (V.Car.GetComponent<Rigidbody>().velocity.magnitude > 2f)
			{
				if (cancellationToken.IsCancellationRequested)
				{
					return false;
				}
				await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
			}
			if (!this || cancellationToken.IsCancellationRequested)
			{
				return false;
			}
			return true;
		}

		public virtual bool TriggerExit(Collider other)
		{
			if (!isCar(other) || !isMine(other))
			{
				return false;
			}
			token?.Cancel();
			return true;
		}

		private void OnDestroy()
		{
			token?.Cancel();
		}

		protected virtual bool isValid(Collider other)
		{
			return isCar(other) & isMine(other);
		}

		protected virtual bool isCar(Collider other)
		{
			if (serviceType == TYPE.TAXI)
			{
				if (!other.CompareTag("Car") && !other.CompareTag("OnlineCar"))
				{
					return false;
				}
				PhotonView photonView = PhotonView.Get(V.DriverPerson);
				return other.transform.parent.gameObject == Global.TaskManager.TaxiManager.clients[photonView.OwnerActorNr].Car;
			}
			if (!other.CompareTag("Joint"))
			{
				return false;
			}
			return true;
		}

		protected virtual bool isMine(Collider other)
		{
			if (!PhotonNetwork.IsConnected)
			{
				return true;
			}
			if (other.transform.parent.TryGetComponent<VehicleData>(out var component) && (bool)component.Driver)
			{
				return PhotonView.Get(component.Driver).IsMine;
			}
			return false;
		}
	}
}
