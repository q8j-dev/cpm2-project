using System;
using System.Collections.Generic;
using Addler.Runtime.Core.LifetimeBinding;
using CarAnimationScripts;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Abstract;
using Service.Manager;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

namespace Service.Taxi
{
	public class TaxiManager : Service.Abstract.TaskManager
	{
		private Action OnDestroyClient;

		private Action OnDestroyGroundLight;

		private IPrefabFactory _prefabFactory;

		private Service.Manager.TaskManager _taskManager;

		private const int PantsNum = 8;

		private const int ShoesNum = 9;

		private Vector3 boundExtent = new Vector3(2f, 2f, 0f);

		private OnlineCollectionsProvider _onlineCollectionsProvider;

		private FreeDriveProvider _freeDriveProvider;

		private CarOutInControl _carOutInControl;

		private CoreGameManager _coreGameManager;

		public const string RPC_TAXI_SHOW_EMOJI = "ShowEmoji";

		public const string RPC_TAXI_INSTANTIATE_CLIENT = "InstantiateClient";

		public const string RPC_TAXI_GET_IN_CAR = "GetInCar";

		public const string RPC_TAXI_GET_OUT_CAR = "GetOutCar";

		public const string RPC_TAXI_CHANGE_STATE = "Taxi_ChangeState";

		private event Action OnCarEnable;

		public event Action OnCounter;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory, OnlineCollectionsProvider onlineCollectionsProvider, FreeDriveProvider freeDriveProvider, CoreGameManager coreGameManager)
		{
			_coreGameManager = coreGameManager;
			_freeDriveProvider = freeDriveProvider;
			_onlineCollectionsProvider = onlineCollectionsProvider;
			_prefabFactory = prefabFactory;
		}

		private void Start()
		{
			_taskManager = Global.TaskManager;
			_carOutInControl = _coreGameManager.CarOutInControl;
		}

		private void FixedUpdate()
		{
			OnCounter?.Invoke();
		}

		public override void OnUserDisconnect(int ownerID)
		{
			if (GetCarByOwnerID(ownerID) == V.Car && _taskManager.Task != null)
			{
				CancelTask(isPlayerLeft: true);
			}
			TaxiController component = clients[ownerID].Owner.GetComponent<TaxiController>();
			if ((bool)component)
			{
				UnityEngine.Object.Destroy(component.Shashka);
			}
			base.OnUserDisconnect(ownerID);
		}

		public void CancelTask(bool isPlayerLeft = false)
		{
			if (_taskManager.Task != null && (isPlayerLeft || _taskManager.Task.GetState() == TASK_STATE.NONE))
			{
				ShowResult(new LocalizedString("TaxiManager.Cancelled"), isPlayerLeft ? new LocalizedString("TaxiManager.OwnerOfTheCarLeftTheGame") : new LocalizedString("TaxiManager.TimeOutClientCancelledTheOrder"), 5f);
				_taskManager.Clear();
				PhotonView photonView = PhotonView.Get(_taskManager);
				PhotonView photonView2 = PhotonView.Get(V.DriverPerson);
				if (PhotonNetwork.IsConnected)
				{
					photonView.RpcToDefaultGroup("ChangeState", RpcTarget.Others, photonView2.ViewID, (byte)2);
				}
				((ServiceController)serviceController).CreateOrders();
			}
		}

		public void StopCounter()
		{
			base.OnCounterStopped?.Invoke();
			while (cancelToken.Count > 0)
			{
				cancelToken.Dequeue().Cancel();
			}
		}

		private void OnDestroy()
		{
			StopCounter();
			GPSTracker.CancelGPSDraw();
		}

		public void Clear()
		{
			StopCounter();
			clients.Remove(PhotonView.Get(V.DriverPerson).OwnerActorNr);
			base.ProgressTime = 0f;
			_taskManager.OnClear -= Clear;
		}

		[PunRPC]
		public void ShowEmoji(int ownerID, byte emojiID)
		{
			if (SceneManager.GetActiveScene().buildIndex != 13)
			{
				return;
			}
			PhotonView photonView = (PhotonNetwork.IsConnected ? PhotonView.Find(ownerID) : PhotonView.Get(V.DriverPerson));
			if (clients.ContainsKey(photonView.OwnerActorNr))
			{
				GameObject gameObject = clients[photonView.OwnerActorNr].gameObject;
				MainPerson component = gameObject.GetComponent<MainPerson>();
				if ((bool)component.ik)
				{
					GameObject gameObject2 = component.ik.references.head.gameObject;
					GameObject obj = UnityEngine.Object.Instantiate(V.DriverPerson.GetComponent<Emoji>().emojies[emojiID]);
					obj.transform.SetParent(gameObject.transform);
					obj.transform.localScale = new Vector3(0.65f, 0.65f, 0.65f);
					obj.transform.position = gameObject2.transform.position + new Vector3(0f, 0.75f, 0f);
					UnityEngine.Object.Destroy(obj, 2f);
				}
			}
		}

		[PunRPC]
		public void InstantiateClient(Vector3 position, int ownerID, int carID, byte stateIndex)
		{
			InstantiateClientInternal(position, ownerID, carID, stateIndex).Forget();
		}

		private async UniTaskVoid InstantiateClientInternal(Vector3 position, int ownerID, int carID, int stateIndex)
		{
			if (ObscuredPrefs.GetInt("taxi", 1) == 0 && PhotonNetwork.IsConnected)
			{
				return;
			}
			CLIENT_STATE state = (CLIENT_STATE)stateIndex;
			if (SceneManager.GetActiveScene().buildIndex != 13)
			{
				V.OnSceneLoaded += UniTask.Action(async () =>
				{
					await _taskManager.TaxiManager.CheckIfSceneLoaded(position, ownerID, carID, state);
				});
				_freeDriveProvider.Signal.Release();
			}
			else
			{
				await CheckIfSceneLoaded(position, ownerID, carID, state);
			}
		}

		private async UniTask CheckIfSceneLoaded(Vector3 position, int ownerID, int carID, CLIENT_STATE state)
		{
			CoreGameManager coreGameManager = V.CoreGameManager;
			while (coreGameManager.globalPoint == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
			}
			StartInstantiate(position, ownerID, carID, state);
			switch (state)
			{
			case CLIENT_STATE.SIT_IN:
				await GetInCarInternal(ownerID);
				break;
			case CLIENT_STATE.GO_OUT:
				await GetOutCarInternal(ownerID);
				break;
			}
		}

		private async UniTask<GameObject> InstantiateFromAddressables(string key, Transform parent)
		{
			AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(key);
			GameObject obj = await handle;
			GameObject result;
			try
			{
				if (!base.gameObject)
				{
					handle.Release();
					return null;
				}
				result = _prefabFactory.Instantiate(obj, parent);
			}
			catch
			{
				if (handle.IsValid())
				{
					handle.Release();
				}
				return null;
			}
			handle.BindTo(result);
			return result;
		}

		private void StartInstantiate(Vector3 position, int ownerID, int carID, CLIENT_STATE state)
		{
			PhotonView photonView = (PhotonNetwork.IsConnected ? PhotonView.Find(ownerID) : PhotonView.Get(V.DriverPerson));
			PhotonView photonView2 = (PhotonNetwork.IsConnected ? PhotonView.Find(carID) : PhotonView.Get(V.Car));
			GameObject obj = Resources.Load<GameObject>("FreeParts/Client");
			GameObject client = _prefabFactory.Instantiate(obj, V.GlobalPoint);
			SetPosition(client.transform, position);
			client.GetComponent<Animator>().applyRootMotion = false;
			ClientController component = client.GetComponent<ClientController>();
			clients[photonView.OwnerActorNr] = component;
			Transform transform = (PhotonNetwork.IsConnected ? photonView2.gameObject.transform : V.Car.transform);
			transform.GetComponent<VehicleData>().LightsComposition.IsTaxi = true;
			component.Set(photonView.gameObject, transform.gameObject);
			component.ChangeState(state).Forget();
			TaxiController component2 = photonView.GetComponent<TaxiController>();
			if (PhotonNetwork.IsConnected && !photonView.IsMine)
			{
				Transform policeTransform = GetPoliceTransform(transform);
				policeTransform.gameObject.SetActive(value: false);
				if ((bool)component2.Shashka)
				{
					component2.Shashka.SetActive(value: true);
					component2.Shashka.transform.SetPositionAndRotation(policeTransform.position, transform.rotation);
					return;
				}
				GameObject obj2 = Resources.Load<GameObject>("FreeParts/Shashka");
				component2.Shashka = _prefabFactory.Instantiate(obj2, transform);
				component2.Shashka.transform.SetPositionAndRotation(policeTransform.position, transform.rotation);
			}
			if (PhotonNetwork.IsConnected && !photonView.IsMine)
			{
				return;
			}
			if (OnDestroyClient != null)
			{
				_taskManager.OnClear -= OnDestroyClient;
			}
			OnDestroyClient = () =>
			{
				if (_taskManager.Task.GetState() != TASK_STATE.END)
				{
					UnityEngine.Object.Destroy(client);
				}
			};
			_taskManager.OnClear += OnDestroyClient;
		}

		[PunRPC]
		public void GetInCar(int ownerID)
		{
			GetInCarInternal(ownerID).Forget();
		}

		private async UniTask GetInCarInternal(int ownerID)
		{
			if (SceneManager.GetActiveScene().buildIndex != 13)
			{
				return;
			}
			PhotonView view = (PhotonNetwork.IsConnected ? PhotonView.Find(ownerID) : PhotonView.Get(V.DriverPerson));
			if (!PhotonNetwork.IsConnected || view.IsMine)
			{
				Order order = (Order)_taskManager.Task.Order;
				float num = GPSTracker.DistanceBetween(order.GetDeparture(), order.GetDestination());
				float taskTime = 3f * Mathf.Sqrt(((double)num > 0.01) ? num : Vector3.Distance(order.GetDeparture(), V.DriverPerson.transform.position - V.GlobalOffset));
				Counter(taskTime);
				GPSTracker.DrawGPS(order.GetDestination(), GPSTracker.GPS.DESTINATION, _freeDriveProvider.MapCamera);
				GameObject obj = Resources.Load<GameObject>("FreeParts/TriggerZone");
				GameObject finishZone = _prefabFactory.Instantiate(obj, V.GlobalPoint);
				Vector3 destination = order.GetDestination();
				SetPosition(finishZone.transform, destination);
				if (OnDestroyGroundLight != null)
				{
					_taskManager.OnClear -= OnDestroyGroundLight;
				}
				OnDestroyGroundLight = () =>
				{
					UnityEngine.Object.Destroy(finishZone);
				};
				_taskManager.OnClear += OnDestroyGroundLight;
			}
			GameObject client = clients[view.OwnerActorNr].gameObject;
			MultiPersonControl multiPersonControl = view.GetComponent<MultiPersonControl>();
			while (multiPersonControl.currentCar == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
			}
			SitInCar(client, view.gameObject, multiPersonControl.currentCar);
		}

		private void SitInCar(GameObject client, GameObject player, GameObject car)
		{
			client.transform.position = Vector3.zero;
			VehicleData component = car.GetComponent<VehicleData>();
			DriverController driverControl = car.GetComponent<DriverController>();
			MainPerson mainPerson = client.GetComponent<MainPerson>();
			_ = client.GetComponent<DriverActionController>().ik;
			int place = _carOutInControl.GetPlaceForPassenger(car);
			OnCarEnable = null;
			OnCarEnable += UniTask.Action(async () =>
			{
				client.GetComponent<Animator>().applyRootMotion = false;
				mainPerson.gameObject.SetActive(value: true);
				if (!mainPerson.skin)
				{
					mainPerson.LoadRandomSkin();
				}
				while (!mainPerson.ik)
				{
					await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
				}
				driverControl.PreSetPerson(client, place);
				if (!client.TryGetComponent<MultiPersonControl>(out var _))
				{
					CarAnimationsController component5 = car.GetComponent<CarAnimationsController>();
					List<Transform> list = ((!(component5 != null) || !(component5.CarAnimationReferences != null)) ? null : component5.CarAnimationsPoints?.SeatPointsToSit);
					if (list != null && place >= 0 && place < list.Count && (bool)list[place])
					{
						client.transform.rotation = list[place].rotation;
					}
					else
					{
						client.transform.localRotation = Quaternion.identity;
					}
				}
				mainPerson.ik.enabled = true;
			});
			OnCarEnable?.Invoke();
			component.VehicleMembersController.SetMember(place, client);
			client.transform.SetParent(car.transform);
			client.GetComponent<DriverActionController>().EnableDisableAllColliders(WillEnable: false);
			_carOutInControl.playCarCloseSound(car);
			client.GetComponent<Animator>().applyRootMotion = false;
			SkinnedMeshRenderer component2 = mainPerson.allKits[9].GetComponent<SkinnedMeshRenderer>();
			SkinnedMeshRenderer component3 = mainPerson.allKits[8].GetComponent<SkinnedMeshRenderer>();
			SetBounds(component2);
			SetBounds(component3);
		}

		private void SetBounds(SkinnedMeshRenderer skinnedMesh)
		{
			Bounds localBounds = skinnedMesh.localBounds;
			localBounds.center = Vector3.zero;
			localBounds.extents = boundExtent;
			skinnedMesh.localBounds = localBounds;
		}

		[PunRPC]
		public void GetOutCar(int ownerID)
		{
			GetOutCarInternal(ownerID).Forget((Exception exception) =>
			{
				Debug.LogError("Error in get out of car");
			});
		}

		public async UniTask GetOutCarInternal(int ownerID)
		{
			if (SceneManager.GetActiveScene().buildIndex != 13)
			{
				return;
			}
			PhotonView view = null;
			if (PhotonNetwork.IsConnected)
			{
				view = PhotonView.Find(ownerID);
			}
			else if ((bool)V.DriverPerson)
			{
				view = PhotonView.Get(V.DriverPerson);
			}
			if (!view)
			{
				return;
			}
			GameObject player = view.gameObject;
			MultiPersonControl multiPersonControl = player.GetComponent<MultiPersonControl>();
			if (!multiPersonControl)
			{
				return;
			}
			while (multiPersonControl.currentCar == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(1.0));
				if (!multiPersonControl || !this)
				{
					return;
				}
			}
			if (clients.TryGetValue(view.OwnerActorNr, out var value))
			{
				GameObject gameObject = value.gameObject;
				if ((bool)gameObject)
				{
					GoOutOfCar(gameObject, multiPersonControl.currentCar, player).Forget();
				}
			}
		}

		public async UniTaskVoid GoOutOfCar(GameObject client, GameObject car, GameObject player)
		{
			Rigidbody component = client.GetComponent<Rigidbody>();
			VehicleData component2 = car.GetComponent<VehicleData>();
			component.mass = 1f;
			OnCarEnable = null;
			Vector3 vector = car.transform.right * UnityEngine.Random.Range(2.6f, 3f);
			Vector3 localPosition = car.transform.localPosition;
			vector.y = (component2.isTruck ? 2 : 0);
			client.transform.localPosition = localPosition;
			FlagsStorage.KeepRow = false;
			client.transform.parent = null;
			client.transform.eulerAngles = Vector3.zero;
			Vector3 position = car.transform.position;
			position += (component2.RightHanded ? (-vector) : vector);
			float y = PersonOutHeight(position, car);
			position = new Vector3(position.x, y, position.z);
			client.transform.position = position;
			if (client.TryGetComponent<DriverActionController>(out var component3))
			{
				if ((bool)component3.ik)
				{
					component3.ik.solver.IKPositionWeight = 0f;
				}
				component3.EnableDisableAllColliders(WillEnable: true);
			}
			_carOutInControl.playCarCloseSound(car);
			await CreateParticleAndDestroy(client);
		}

		private float PersonOutHeight(Vector3 v3, GameObject car)
		{
			Vector3 direction = car.transform.TransformDirection(Vector3.down);
			LayerMask layerMask = -1;
			float maxDistance = 10f;
			if (Physics.Raycast(v3, direction, out var hitInfo, maxDistance, layerMask.value, QueryTriggerInteraction.Ignore))
			{
				return hitInfo.point.y;
			}
			return car.transform.localPosition.y;
		}

		private async UniTask CreateParticleAndDestroy(GameObject client)
		{
			GameObject original = Resources.Load<GameObject>("FreeParts/MoneyDollarbillBlast");
			GameObject particle = UnityEngine.Object.Instantiate(original);
			await UniTask.Delay(TimeSpan.FromMilliseconds(2000.0));
			UnityEngine.Object.Destroy(particle);
			UnityEngine.Object.Destroy(client);
		}

		[PunRPC]
		public void Taxi_ChangeState(int ownerID, byte data)
		{
			if (SceneManager.GetActiveScene().buildIndex == 13)
			{
				bool animate = (data & 1) != 0;
				CLIENT_STATE state = (CLIENT_STATE)(data >> 1);
				PhotonView photonView = (PhotonNetwork.IsConnected ? PhotonView.Find(ownerID) : PhotonView.Get(V.DriverPerson));
				if (clients.ContainsKey(photonView.OwnerActorNr))
				{
					(clients[photonView.OwnerActorNr] as ClientController)?.ChangeState(state, animate).Forget();
				}
			}
		}

		private void SetPosition(Transform trans, Vector3 position)
		{
			Vector3 position2 = V.GlobalPoint.position;
			Vector3 vector = position + position2;
			position = new Vector3(vector.x, vector.y - 0.5f, vector.z);
			if (Physics.Raycast(position + Vector3.up, Vector3.down, out var hitInfo, 5f))
			{
				position = hitInfo.point;
			}
			trans.position = position;
		}

		private Transform GetPoliceTransform(Transform car)
		{
			foreach (Transform item in car)
			{
				if (item.name.Contains("police"))
				{
					return item;
				}
			}
			return null;
		}

		private GameObject GetCarByOwnerID(int ownerId)
		{
			return _onlineCollectionsProvider.GetCarByOwnerID(ownerId);
		}
	}
}
