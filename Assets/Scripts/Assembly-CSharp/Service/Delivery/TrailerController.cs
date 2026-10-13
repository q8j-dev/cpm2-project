using System;
using System.Collections.Generic;
using System.Threading;
using CPMEngine;
using Cysharp.Threading.Tasks;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using Service.Abstract;
using Service.Manager;
using UnityEngine;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace Service.Delivery
{
	public class TrailerController : ClientController, IPunObservable, IPunOwnershipCallbacks
	{
		public TRUCK_TYPE type;

		public GameObject trigger;

		[SerializeField]
		private Collider jointCollider = default;

		public ContainerController containerController;

		public new PhotonView photonView;

		[SerializeField]
		private Rigidbody rb;

		public WheelCollider[] wheels;

		public List<Collider> colliders = new List<Collider>();

		private Order order;

		private Vector3 triggerPosition;

		private PhotonView view;

		private bool started;

		private ConfigurableJoint joint;

		private CancellationTokenSource _checkDistanceCts;

		private const float TrailerVelocityThreshold = 1f;

		private const int TrailerVisibilityThreshold = 225000;

		private const float InitialDrag = 1.2f;

		private const float DetachedDrag = 22f;

		private readonly Service.Manager.TaskManager _taskManager = Global.TaskManager;

		private Vector3 _initPos;

		private bool _wasAttached;

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
			_initPos = Vector3.zero;
			_wasAttached = false;
		}

		private void Update()
		{
			if (!(_initPos == Vector3.zero) && !_wasAttached && state == CLIENT_STATE.DEATTACHED && Vector3.Distance(base.transform.localPosition, _initPos) > 10f)
			{
				base.transform.localPosition = _initPos;
			}
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
				photonView = GetComponent<PhotonView>();
				base.transform.position += V.GlobalOffset;
				_initPos = base.transform.localPosition;
				initialRotation = base.transform.rotation;
				started = true;
				view = PhotonView.Get(_taskManager);
				if (!PhotonNetwork.IsConnected || photonView.IsMine)
				{
					Set(V.DriverPerson, null);
					base.transform.position += Vector3.up;
					trigger.AddComponent<ConnectionTrigger>().Init();
				}
				else
				{
					PhotonView component = V.DriverPerson.GetComponent<PhotonView>();
					photonView.RPC("RequestData", photonView.Owner, component.ViewID);
				}
				Init();
				ChangeState(CLIENT_STATE.DEATTACHED);
			}
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

		public override void OnEnable()
		{
			base.OnEnable();
			if (owner == V.DriverPerson)
			{
				_taskManager.Task.CurrentItemsCount = containerController.onBoard.Count;
				_taskManager.Task.TotalItemsCount = containerController.items.Count;
				_taskManager.Task.OnItemsCountChanged?.Invoke();
			}
		}

		private void Init()
		{
			rb = GetComponent<Rigidbody>();
			int num = (PhotonNetwork.IsConnected ? ((int)photonView.InstantiationData[0]) : ((Order)_taskManager.Task.Order).id);
			GameObject gameObject = UnityEngine.Object.Instantiate(((ItemScriptables)Resources.Load($"_Services/Delivery/Scriptables/{num}")).GetItems(), trigger.transform.parent.parent);
			containerController = gameObject.GetComponent<ContainerController>();
			containerController.trailer = this;
			containerController.Init(this);
			if (!(owner != V.DriverPerson))
			{
				order = (Order)_taskManager.Task.Order;
			}
		}

		private void DisablePhysics()
		{
			WheelCollider[] array = wheels;
			foreach (WheelCollider obj in array)
			{
				obj.enabled = false;
				obj.transform.GetChild(0).GetComponent<Collider>().enabled = true;
			}
			rb.useGravity = true;
			containerController?.DisablePhysics();
		}

		private void EnablePhysics()
		{
			if (photonView.IsMine)
			{
				WheelCollider[] array = wheels;
				foreach (WheelCollider obj in array)
				{
					obj.enabled = true;
					obj.transform.GetChild(0).GetComponent<Collider>().enabled = false;
				}
				rb.useGravity = true;
				containerController?.EnablePhysics();
			}
		}

		private void ResetPhysics()
		{
			if (state == CLIENT_STATE.ATTACHED)
			{
				rb.velocity = Vector3.zero;
				rb.angularVelocity = Vector3.zero;
				rb.Sleep();
			}
			rb.useGravity = !PhotonNetwork.IsConnected || state == CLIENT_STATE.DEATTACHED || photonView.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber;
		}

		public override void Set(GameObject owner, GameObject car)
		{
			base.Set(owner, car);
			if (owner != V.DriverPerson)
			{
				DisablePhysics();
			}
			PhotonView photonView = PhotonView.Get(owner);
			_taskManager.DeliveryManager.AddTrailer(photonView.OwnerActorNr, this);
		}

		public void Reattach()
		{
			ConfigurableJoint component = car.GetComponent<ConfigurableJoint>();
			base.transform.localPosition = triggerPosition;
			component.connectedBody = GetComponent<Rigidbody>();
		}

		public override void Attach(GameObject car)
		{
			Attach(car.transform);
		}

		public override void Attach(Transform carTransform, Vector3? pos = null)
		{
			if (owner == V.DriverPerson)
			{
				_taskManager.Task.CurrentItemsCount = containerController.onBoard.Count;
				_taskManager.Task.TotalItemsCount = containerController.items.Count;
				_taskManager.Task.OnItemsCountChanged?.Invoke();
			}
			SetCar(carTransform);
			joint = carTransform.gameObject.GetComponent<ConfigurableJoint>();
			if (!joint)
			{
				joint = carTransform.gameObject.AddComponent<ConfigurableJoint>();
			}
			base.transform.SetParent(carTransform);
			base.transform.localPosition = pos ?? triggerPosition;
			joint.breakForce = 1000000f;
			joint.enableCollision = true;
			joint.anchor = pos ?? triggerPosition;
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
			float num = carTransform.eulerAngles.y - base.transform.eulerAngles.y;
			num = ((num < -180f) ? (360f + num) : num);
			num = ((num > 180f) ? (360f - num) : num);
			SoftJointLimit highAngularXLimit = new SoftJointLimit
			{
				limit = 180f
			};
			joint.highAngularXLimit = highAngularXLimit;
			SoftJointLimit lowAngularXLimit = new SoftJointLimit
			{
				limit = -180f
			};
			joint.lowAngularXLimit = lowAngularXLimit;
			if (num < 0f)
			{
				joint.highAngularXLimit = highAngularXLimit;
			}
			SoftJointLimit angularYLimit = new SoftJointLimit
			{
				limit = 15f
			};
			joint.angularYLimit = angularYLimit;
			SoftJointLimit angularZLimit = new SoftJointLimit
			{
				limit = 10f
			};
			joint.angularZLimit = angularZLimit;
			joint.connectedBody = base.transform.GetComponent<Rigidbody>();
			ChangeState(CLIENT_STATE.ATTACHED);
			SendAttach();
			containerController.EnableItems();
			new Hashtable();
			if (!(owner != V.DriverPerson))
			{
				_menuLevelParent.serviceLabels.Enable(TYPE.DELIVERY);
				Task task = (Task)_taskManager.Task;
				if (task.GetState() == TASK_STATE.NONE)
				{
					_taskManager.DeliveryManager.Counter(task.Order.GetTime());
				}
				task.SetState(TASK_STATE.START);
			}
		}

		public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion rotation)
		{
			return rotation * (point - pivot) + pivot;
		}

		public Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Vector3 angle)
		{
			return Quaternion.Euler(angle) * (point - pivot) + pivot;
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
				photonView.TransferOwnership(base.Owner.GetComponent<PhotonView>().Owner);
			}
			containerController.DisableItems();
			SendDetach();
		}

		private void SendAttach()
		{
			if (PhotonNetwork.IsConnected)
			{
				Service.JointHandler component = car.GetComponent<Service.JointHandler>();
				component.CreateTrigger(TYPE.DELIVERY);
				Vector3 localPosition = component.trigger.transform.localPosition;
				view.RpcToDefaultGroup("AttachTrailer", RpcTarget.Others, owner.GetComponent<PhotonView>().ViewID, (byte)2, localPosition);
			}
		}

		private void SendDetach()
		{
			if (PhotonNetwork.IsConnected)
			{
				Vector3 localPosition = base.transform.localPosition;
				view.RpcToDefaultGroup("DetachTrailer", RpcTarget.Others, owner.GetComponent<PhotonView>().ViewID, (byte)2, localPosition);
			}
		}

		public override void ChangeState(CLIENT_STATE state)
		{
			base.ChangeState(state);
			GetComponent<PhotonTransformViewClassic>().m_PositionModel.SynchronizeEnabled = state == CLIENT_STATE.DEATTACHED;
			switch (state)
			{
			case CLIENT_STATE.ATTACHED:
				_wasAttached = true;
				SetCar();
				base.transform.SetParent(car.transform);
				jointCollider.enabled = false;
				DisableCollisions();
				_checkDistanceCts?.Cancel();
				if (!(owner != V.DriverPerson))
				{
					order = (Order)_taskManager.Task.Order;
					if (photonView.OwnerActorNr == PhotonNetwork.LocalPlayer.ActorNumber || !PhotonNetwork.IsConnected)
					{
						owner.GetComponent<DeliveryController>().ShowAndHideDestination(this, order.GetDestination());
					}
					rb.drag = 1.2f;
					rb.angularDrag = 1.2f;
					GPSTracker.CancelGPSDraw();
					GPSTracker.DrawGPS(order.GetDestination(), GPSTracker.GPS.DESTINATION, _freeDriveProvider.MapCamera);
				}
				break;
			case CLIENT_STATE.DEATTACHED:
				car = null;
				base.transform.SetParent(V.GlobalPoint);
				jointCollider.enabled = true;
				if (PhotonNetwork.IsConnected && photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
				{
					DisablePhysics();
				}
				else
				{
					EnablePhysics();
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
					owner.GetComponent<DeliveryController>().ShowAndHideDestination(this, order.GetDestination());
					rb.drag = 22f;
					rb.angularDrag = 22f;
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
				bool flag = Vector3.SqrMagnitude(V.DriverPerson.transform.position - base.transform.position) > 225000f;
				bool flag2 = rb.velocity.sqrMagnitude < 1f;
				rb.isKinematic = flag & flag2;
				containerController.UpdatePhysics();
				await UniTask.Delay(1000, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			}
		}

		private void SetFakeWheel()
		{
			if (state == CLIENT_STATE.ATTACHED && !photonView.IsMine && PhotonNetwork.IsConnected)
			{
				WheelCollider[] array = wheels;
				foreach (WheelCollider obj in array)
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
				WheelCollider[] array = wheels;
				for (int i = 0; i < array.Length; i++)
				{
					array[i].transform.GetChild(0).GetComponent<FakeWheel>().referenceWheel = null;
				}
			}
		}

		public void SetTriggerPosition(Vector3 pos)
		{
			triggerPosition = pos;
		}

		public void SetWheelPosition(int i, float ypos)
		{
			Transform obj = wheels[i].transform;
			Vector3 localPosition = obj.localPosition;
			localPosition = new Vector3(localPosition.x, ypos, localPosition.z);
			obj.localPosition = localPosition;
		}

		public async UniTaskVoid Destroy(float time = 0f)
		{
			time = 0f;
			if (PhotonNetwork.IsConnected)
			{
				containerController.ResetCustomProperties();
			}
			if (!PhotonNetwork.IsConnected)
			{
				UnityEngine.Object.Destroy(base.gameObject, time);
				return;
			}
			await UniTask.Delay(TimeSpan.FromMilliseconds((int)time * 1000));
			PhotonNetwork.Destroy(photonView);
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
			int? num = null;
			if ((bool)car)
			{
				num = car.GetComponent<PhotonView>().ViewID;
			}
			this.photonView.RPC("SentData", targetPlayer, vector, triggerPosition, component.ViewID, num, (byte)state);
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

		public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.IsWriting)
			{
				for (int i = 0; i < wheels.Length; i++)
				{
					stream.SendNext(wheels[i].transform.localPosition.y);
				}
				return;
			}
			for (int j = 0; j < wheels.Length; j++)
			{
				SetWheelPosition(j, (float)stream.ReceiveNext());
			}
			if (car != null)
			{
				base.transform.localPosition = triggerPosition;
			}
		}

		private void FixedUpdate()
		{
			if (started && PhotonNetwork.IsConnected && photonView.OwnerActorNr != PhotonNetwork.LocalPlayer.ActorNumber && state != CLIENT_STATE.DEATTACHED)
			{
				base.transform.localPosition = triggerPosition;
			}
		}

		public void OnOwnershipRequest(PhotonView targetView, Player requestingPlayer)
		{
		}

		public void OnOwnershipTransfered(PhotonView targetView, Player previousOwner)
		{
			if (targetView != photonView)
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
				EnablePhysics();
			}
		}

		public async UniTaskVoid TransferOwnership(GameObject _car, CancellationToken cancelationToken)
		{
			try
			{
				while (!object.Equals(photonView.Owner, PhotonNetwork.LocalPlayer))
				{
					await UniTask.Delay(50, ignoreTimeScale: false, PlayerLoopTiming.Update, cancelationToken);
				}
			}
			catch (OperationCanceledException)
			{
				return;
			}
			car = _car;
			ChangeState(CLIENT_STATE.DEATTACHED);
			Attach((_car == null) ? car.transform : _car.transform, triggerPosition);
			EnablePhysics();
		}

		public void OnOwnershipTransferFailed(PhotonView targetView, Player senderOfFailedRequest)
		{
			Debug.LogWarning("Trailer ownership transfer failed");
		}
	}
}
