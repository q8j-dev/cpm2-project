using System;
using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Service.Abstract;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace Service.Cargo
{
	public class TrailerController : ClientController, IPunObservable, IPunOwnershipCallbacks
	{
		public TRUCK_TYPE type;

		public GameObject connectionTrigger;

		public WheelController[] wheels;

		public List<Collider> colliders = new List<Collider>();

		[SerializeField]
		private Collider jointCollider = default;

		private Order order;

		private Vector3 carJointPosition;

		private ConfigurableJoint joint;

		private PhotonView globalView;

		private Rigidbody rb;

		private bool started;

		private CancellationTokenSource _checkDistanceCts;

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private FreeDriveProvider _freeDriveProvider;

		private MenuLevelParent _menuLevelParent;

		[Inject]
		public void Construct(OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveProvider freeDriveProvider, MenuLevelParent menuLevelParent)
		{
			_menuLevelParent = menuLevelParent;
			_freeDriveProvider = freeDriveProvider;
			_onlineCollectionsProvider = onlineCollectionsProvider;
		}

		private void OnDestroy()
		{
			_checkDistanceCts?.Cancel();
			_checkDistanceCts?.Dispose();
		}

		private void Awake()
		{
			UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		}

		private async void Start()
		{
			while (SceneManager.GetActiveScene().buildIndex != 13 || !V.GlobalPoint || !V.DriverPerson)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(100.0));
			}
			if ((bool)this)
			{
				base.transform.SetParent(V.GlobalPoint);
				base.transform.position += V.GlobalOffset;
				initialRotation = base.transform.rotation;
				globalView = PhotonView.Get(Global.TaskManager.gameObject);
				started = true;
				if (!PhotonNetwork.IsConnected || base.photonView.IsMine)
				{
					Set(V.DriverPerson, null);
					base.transform.position += Vector3.up;
					connectionTrigger.AddComponent<ConnectionTrigger>().Init();
				}
				else
				{
					PhotonView component = V.DriverPerson.GetComponent<PhotonView>();
					base.photonView.RPC("RequestData", base.photonView.Owner, component.ViewID);
				}
				Init();
				ChangeState(CLIENT_STATE.DEATTACHED);
			}
		}

		private void FixedUpdate()
		{
			if (started && PhotonNetwork.IsConnected && base.photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
			{
				ResetPhysics();
				if (state != CLIENT_STATE.DEATTACHED)
				{
					base.transform.localPosition = carJointPosition;
				}
			}
		}

		private void Init()
		{
			rb = GetComponent<Rigidbody>();
			if (!(owner != V.DriverPerson))
			{
				order = (Order)Global.TaskManager.Task.Order;
			}
		}

		public override void Set(GameObject owner, GameObject car)
		{
			base.Set(owner, car);
			if (owner != V.DriverPerson)
			{
				DisablePhysics();
			}
			PhotonView photonView = PhotonView.Get(owner);
			Global.TaskManager.CargoManager.AddTrailer(photonView.OwnerActorNr, this);
		}

		public override void Attach(GameObject car)
		{
			Attach(car.transform);
		}

		public override void Attach(Transform carTransform, Vector3? pos = null)
		{
			joint = carTransform.gameObject.GetComponent<ConfigurableJoint>();
			if (!joint)
			{
				joint = carTransform.gameObject.AddComponent<ConfigurableJoint>();
			}
			base.transform.SetParent(carTransform);
			base.transform.localPosition = carJointPosition;
			joint.breakForce = 1000000f;
			joint.enableCollision = true;
			joint.anchor = carJointPosition;
			joint.axis = new Vector3(0f, 1f, 0f);
			joint.autoConfigureConnectedAnchor = false;
			joint.secondaryAxis = new Vector3(0f, 1f, 0f);
			joint.autoConfigureConnectedAnchor = true;
			joint.xMotion = ConfigurableJointMotion.Locked;
			joint.yMotion = ConfigurableJointMotion.Locked;
			joint.zMotion = ConfigurableJointMotion.Locked;
			joint.angularXMotion = ConfigurableJointMotion.Limited;
			joint.angularYMotion = ConfigurableJointMotion.Limited;
			joint.angularZMotion = ConfigurableJointMotion.Limited;
			joint.highAngularXLimit = new SoftJointLimit
			{
				limit = 180f
			};
			joint.lowAngularXLimit = new SoftJointLimit
			{
				limit = -180f
			};
			joint.angularYLimit = new SoftJointLimit
			{
				limit = 10f
			};
			joint.angularZLimit = new SoftJointLimit
			{
				limit = 5f
			};
			joint.connectedBody = base.transform.GetComponent<Rigidbody>();
			ChangeState(CLIENT_STATE.ATTACHED);
			SendAttach();
			if (!(owner != V.DriverPerson))
			{
				_menuLevelParent.serviceLabels.Enable(TYPE.CARGO);
				Task task = (Task)Global.TaskManager.Task;
				if (task.GetState() == TASK_STATE.NONE)
				{
					Global.TaskManager.CargoManager.Counter(task.Order.GetTime());
				}
				task.SetState(TASK_STATE.START);
			}
		}

		private void SendAttach()
		{
			if (PhotonNetwork.IsConnected)
			{
				JointHandler component = car.GetComponent<JointHandler>();
				component.CreateTrigger(TYPE.CARGO);
				Vector3 localPosition = component.trigger.transform.localPosition;
				globalView.RpcToDefaultGroup("AttachTrailer", RpcTarget.Others, owner.GetComponent<PhotonView>().ViewID, (byte)4, localPosition);
			}
		}

		public override void Detach(string message = "", bool showMessage = false)
		{
			ChangeState(CLIENT_STATE.DEATTACHED);
			if (base.Owner == V.DriverPerson)
			{
				if (showMessage)
				{
					V.CoreGameManager.SetMessage(message, 3f);
				}
			}
			else
			{
				base.photonView.TransferOwnership(base.Owner.GetComponent<PhotonView>().Owner);
			}
			SendDetach();
		}

		private void SendDetach()
		{
			if (PhotonNetwork.IsConnected)
			{
				Vector3 localPosition = base.transform.localPosition;
				globalView.RpcToDefaultGroup("DetachTrailer", RpcTarget.Others, owner.GetComponent<PhotonView>().ViewID, (byte)4, localPosition);
			}
		}

		public void SetTriggerPosition(Vector3 pos)
		{
			carJointPosition = pos;
		}

		[PunRPC]
		private async void RequestData(int viewID)
		{
			PhotonView photonView = null;
			while (photonView == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(100.0));
				photonView = PhotonView.Find(viewID);
			}
			Player targetPlayer = photonView.Owner;
			PhotonView component = owner.GetComponent<PhotonView>();
			Vector3 vector = ((base.transform.parent == V.GlobalPoint) ? base.transform.localPosition : (base.transform.localPosition + base.transform.parent.localPosition));
			base.photonView.RPC("SentData", targetPlayer, vector, carJointPosition, component.ViewID, car?.GetComponent<PhotonView>()?.ViewID, (byte)state);
		}

		[PunRPC]
		private void SentData(Vector3 position, Vector3 triggerPos, int viewID, int carID, byte new_state)
		{
			PhotonView photonView = PhotonView.Find(viewID);
			SetTriggerPosition(triggerPos);
			base.transform.localPosition = position;
			Set(photonView.gameObject, null);
			if (carID == 0)
			{
				SetCar();
			}
			else
			{
				SetCar(carID);
			}
			ChangeState((new_state == 5) ? CLIENT_STATE.ATTACHED : CLIENT_STATE.DEATTACHED);
		}

		public override void ChangeState(CLIENT_STATE state)
		{
			base.ChangeState(state);
			GetComponent<PhotonTransformViewClassic>().m_PositionModel.SynchronizeEnabled = state == CLIENT_STATE.DEATTACHED;
			switch (state)
			{
			case CLIENT_STATE.ATTACHED:
				SetCar();
				base.transform.SetParent(car.transform);
				jointCollider.enabled = false;
				DisableCollisions();
				_checkDistanceCts?.Cancel();
				if (!(owner != V.DriverPerson))
				{
					order = (Order)Global.TaskManager.Task.Order;
					if (base.photonView.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber || !PhotonNetwork.IsConnected)
					{
						owner.GetComponent<CargoController>().ShowAndHideDestination(this, order.destination);
					}
					GPSTracker.CancelGPSDraw();
					GPSTracker.DrawGPS(order.GetDestination(), GPSTracker.GPS.DESTINATION, _freeDriveProvider.MapCamera);
				}
				break;
			case CLIENT_STATE.DEATTACHED:
				car = null;
				base.transform.SetParent(V.GlobalPoint);
				jointCollider.enabled = true;
				if (PhotonNetwork.IsConnected && base.photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
				{
					DisablePhysics();
				}
				else
				{
					EnablePhysics(null);
				}
				if (PhotonNetwork.IsConnected)
				{
					ResetPhysics();
				}
				_checkDistanceCts?.Cancel();
				_checkDistanceCts?.Dispose();
				_checkDistanceCts = new CancellationTokenSource();
				CheckDistance(_checkDistanceCts.Token).Forget();
				EnableCollisions();
				if (!(owner != V.DriverPerson))
				{
					owner.GetComponent<CargoController>().ShowAndHideDestination(this, order.destination);
					GPSTracker.CancelGPSDraw();
					GPSTracker.DrawGPS(base.transform, GPSTracker.GPS.DEPARTURE, _freeDriveProvider.MapCamera).Forget();
				}
				break;
			}
			SetFakeWheel();
		}

		private async UniTaskVoid CheckDistance(CancellationToken cancellationToken)
		{
			while (!cancellationToken.IsCancellationRequested)
			{
				rb.isKinematic = Vector3.Distance(V.DriverPerson.transform.position, base.transform.position) > 150f;
				await UniTask.Delay(500, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
		}

		private void SetFakeWheel()
		{
			if (state == CLIENT_STATE.ATTACHED && !base.photonView.IsMine && PhotonNetwork.IsConnected)
			{
				WheelController[] array = wheels;
				foreach (WheelController obj in array)
				{
					Transform transform = car.GetComponentInChildren<WheelModel>()?.transform;
					if (!transform)
					{
						transform = car.GetComponentInChildren<WheelProvider>().ModelTransform;
					}
					obj.transform.GetChild(0).GetComponent<FakeWheel>().referenceWheel = transform;
				}
			}
			else
			{
				WheelController[] array = wheels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].transform.GetChild(0).GetComponent<FakeWheel>().referenceWheel = null;
				}
			}
		}

		public async UniTaskVoid Destroy(float time = 0f)
		{
			if (!PhotonNetwork.IsConnected)
			{
				UnityEngine.Object.Destroy(base.gameObject, time);
				return;
			}
			await UniTask.Delay(TimeSpan.FromMilliseconds((int)time * 1000));
			PhotonNetwork.Destroy(base.photonView);
		}

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				for (int i = 0; i < wheels.Length; i++)
				{
					stream.SendNext(wheels[i].wheelTransform.localPosition.y);
				}
				return;
			}
			for (int j = 0; j < wheels.Length; j++)
			{
				SetWheelPosition(j, (float)stream.ReceiveNext());
			}
			if (car != null)
			{
				base.transform.localPosition = carJointPosition;
			}
		}

		public void SetWheelPosition(int i, float yPos)
		{
			Transform wheelTransform = wheels[i].wheelTransform;
			Vector3 localPosition = wheelTransform.localPosition;
			localPosition = new Vector3(localPosition.x, yPos, localPosition.z);
			wheelTransform.localPosition = localPosition;
		}

		public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
		{
			throw new NotImplementedException();
		}

		public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
		{
			if (targetView != base.photonView)
			{
				return;
			}
			if (object.Equals(previousOwner, PhotonNetwork.LocalPlayer) && !object.Equals(previousOwner, targetView.Owner))
			{
				if ((bool)car)
				{
					UnityEngine.Object.Destroy(car.GetComponent<ConfigurableJoint>());
					ChangeState(CLIENT_STATE.DEATTACHED);
					DisablePhysics();
				}
			}
			else if (object.Equals(previousOwner, targetView.Owner))
			{
				EnablePhysics(null);
			}
		}

		public async UniTaskVoid TransferOwnership(GameObject _car, CancellationToken token)
		{
			try
			{
				while (!object.Equals(base.photonView.Owner, PhotonNetwork.LocalPlayer))
				{
					await UniTask.Delay(50, ignoreTimeScale: false, PlayerLoopTiming.Update, token);
				}
			}
			catch (OperationCanceledException)
			{
				return;
			}
			car = _car;
			ChangeState(CLIENT_STATE.DEATTACHED);
			Attach((_car == null) ? car.transform : _car.transform, carJointPosition);
			EnablePhysics(_car);
		}

		private void DisablePhysics()
		{
			WheelController[] array = wheels;
			for (int i = 0; i < array.Length; i++)
			{
				array[i].enabled = false;
			}
		}

		private void EnablePhysics(GameObject _car)
		{
			if (base.photonView.IsMine)
			{
				WheelController[] array = wheels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].enabled = true;
				}
			}
		}

		public void ResetPhysics()
		{
			if (state == CLIENT_STATE.ATTACHED)
			{
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rb.Sleep();
			}
			rb.useGravity = base.photonView.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber || !PhotonNetwork.IsConnected || state == CLIENT_STATE.DEATTACHED;
		}

		private void DisableCollisions()
		{
			foreach (Collider collider in colliders)
			{
				_onlineCollectionsProvider.DisableOnlineCarsCollision(car, collider);
			}
		}

		private void EnableCollisions()
		{
			foreach (Collider collider in colliders)
			{
				_onlineCollectionsProvider.EnableOnlineCarsCollision(collider);
			}
		}

		public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
		{
			throw new NotImplementedException();
		}
	}
}
