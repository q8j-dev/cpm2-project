using System;
using System.Collections.Generic;
using System.Globalization;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Photon.Realtime;
using Service.Abstract;
using Service.Manager;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Providers;

namespace Service.Taxi
{
	public class ServiceController : Service.Abstract.ServiceController
	{
		private PhotonView photonView;

		private float offset = 320f;

		private GameObject clientPrefab;

		private GameObject finishPrefab;

		private List<Location> closePositions = new List<Location>();

		private Action<Player> SendInstantiateClient;

		private NamesScriptables namesScriptables;

		private Service.Manager.TaskManager _taskManager;

		private IEarnMoneyService _earnMoneyService;

		private FreeDriveProvider _freeDriveProvider;

		private void Awake()
		{
			if (locations.Count == 0)
			{
				Init().Forget();
			}
			if (!_taskManager)
			{
				_taskManager = Global.TaskManager;
			}
		}

		[Inject]
		public void Construct(NamesScriptables namesScriptables, IEarnMoneyService earnMoneyService, FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
			_earnMoneyService = earnMoneyService;
			this.namesScriptables = namesScriptables;
		}

		private async UniTaskVoid Init()
		{
			locations = positionsSO.locations;
			while (_taskManager == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
			}
			_taskManager.TaxiManager.serviceController = this;
		}

		private void OnEnable()
		{
			SetMap();
			GPSTracker.ClearPath(forget: true);
			currentTask = null;
		}

		private void OnDisable()
		{
		}

		public override void RefreshList()
		{
			base.RefreshList();
			closePositions.Clear();
		}

		public override void OnSceneLoaded()
		{
			time = -1;
		}

		public override void CreateOrders()
		{
			if (ObscuredPrefs.GetInt("taxi", 1) == 0)
			{
				return;
			}
			if (locations.Count == 0)
			{
				Init().Forget();
			}
			if (!_freeDriveProvider.MapCamera.serviceMapIsOpen || locations == null)
			{
				return;
			}
			base.CreateOrders();
			foreach (Location location in locations)
			{
				Transform transform = V.DriverPerson.transform;
				Vector2 a = new Vector2(location.position.x, location.position.z);
				Vector2 b = new Vector2(transform.localPosition.x, transform.localPosition.z);
				if (Vector2.Distance(a, b) < 1000f && Vector2.Distance(a, b) > 100f)
				{
					closePositions.Add(location);
				}
			}
			SetMap();
			if (closePositions.Count == 0)
			{
				return;
			}
			int num = UnityEngine.Random.Range(5, 15);
			index = 0;
			for (int i = 0; i < num; i++)
			{
				int num2 = UnityEngine.Random.Range(0, closePositions.Count - 1);
				int hashCode = num2.GetHashCode();
				int num3;
				do
				{
					num3 = UnityEngine.Random.Range(0, locations.Count - 1);
				}
				while (hashCode == num3.GetHashCode() || !(Vector3.Distance(closePositions[num2].position, locations[num3].position) > 200f));
				AddTask(closePositions[num2], locations[num3]);
			}
		}

		public void AddTask(Location x, Location y)
		{
			float num = GPSTracker.DistanceBetween(x.position, y.position);
			if (!(num.ToString(CultureInfo.InvariantCulture) == "0"))
			{
				base.PhoneWindow.AddOrder(x, y, (uint)UnityEngine.Random.Range(_earnMoneyService.TaxiRewardMin, _earnMoneyService.TaxiRewardMax), num / 1000f, y.city, namesScriptables.GetName(), index);
				index++;
			}
		}

		protected override void LocateOrders()
		{
			int num = 0;
			foreach (KeyValuePair<int, Transform> order in orders)
			{
				order.Value.localPosition = new Vector3(0f, (float)(num * -220) + offset, 0f);
				num++;
			}
		}

		public override void ClickStartButton(int? taskID)
		{
			ClickStartButtonInternal(taskID).Forget();
		}

		private async UniTaskVoid ClickStartButtonInternal(int? taskID)
		{
			photonView = PhotonView.Get(_taskManager);
			PhotonView playerView = PhotonView.Get(V.DriverPerson);
			if (!taskID.HasValue)
			{
				return;
			}
			base.ClickStartButton(taskID);
			base.CurrentOrder = base.PhoneWindow.GetCurrentOrder();
			base.CurrentOrder.CorrectDepartureAndDestination();
			RemoveTask(taskID.GetValueOrDefault());
			await V.DriverPerson.GetComponent<TaxiController>().UpdateTask(base.CurrentOrder);
			_taskManager.OnClear += () =>
			{
				GPSTracker.CancelGPSDraw();
				base.PhoneWindow.EnableAcceptButton(isEnabled: true);
			};
			TaxiManager taxiManager = _taskManager.TaxiManager;
			Vector3 position = base.CurrentOrder.GetDeparture();
			if (!V.DriverPerson)
			{
				Debug.LogWarning("[Taxi] Водитель исчез, пока оформлялся заказ — приём отменён.", this);
				return;
			}
			float num = GPSTracker.DistanceBetween(base.CurrentOrder.GetDeparture(), V.DriverPerson.transform.position - V.GlobalOffset);
			float taskTime = 3f * Mathf.Sqrt(((double)num > 0.01) ? num : Vector3.Distance(base.CurrentOrder.GetDeparture(), V.DriverPerson.transform.position - V.GlobalOffset));
			taxiManager.Counter(taskTime);
			GPSTracker.DrawGPS(base.CurrentOrder.GetDeparture(), GPSTracker.GPS.DEPARTURE, _freeDriveProvider.MapCamera);
			PhotonView carView = V.DriverPerson.GetComponent<MultiPersonControl>().currentCar.GetPhotonView();
			base.PhoneWindow.Close();
			Camera.main.GetComponent<FreeDriveControl>().ShowMainDrivingUI();
			taxiManager.InstantiateClient(position, playerView.ViewID, carView.ViewID, 0);
			if (PhotonNetwork.IsConnected)
			{
				photonView.RpcToDefaultGroup("InstantiateClient", RpcTarget.Others, position, playerView.ViewID, carView.ViewID, (byte)0);
				if (SendInstantiateClient != null)
				{
					V.OnPlayerConnected -= SendInstantiateClient;
				}
				SendInstantiateClient = (Player target) =>
				{
					byte b = (byte)_taskManager.TaxiManager.clients[playerView.OwnerActorNr].GetState();
					photonView.RPC("InstantiateClient", target, position, playerView.ViewID, carView.ViewID, b);
				};
				V.OnPlayerConnected += SendInstantiateClient;
			}
		}

		public override void OnTaskFinished()
		{
			V.OnPlayerConnected -= SendInstantiateClient;
		}

		public void SetMap()
		{
			_freeDriveProvider.MapCamera.serviceMap = base.PhoneWindow.Map;
			_freeDriveProvider.MapCamera.ShowServiceMap();
		}
	}
}
