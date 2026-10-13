using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Abstract;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace Service.Cargo
{
	public class ServiceController : Service.Abstract.ServiceController
	{
		private List<int> closePositions = new List<int>();

		private CargoManager cargoManager;

		private float offset = 320f;

		private NamesScriptables namesScriptables;

		[SerializeField]
		private TrailerScriptable scriptable;

		private FreeDriveProvider _freeDriveProvider;

		private void Awake()
		{
			if (locations.Count == 0)
			{
				Init().Forget();
			}
		}

		[Inject]
		public void Construct(NamesScriptables namesScriptables, FreeDriveProvider freeDriveProvider)
		{
			_freeDriveProvider = freeDriveProvider;
			this.namesScriptables = namesScriptables;
		}

		private async UniTaskVoid Init()
		{
			locations = positionsSO.locations;
			while (Global.TaskManager == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(10.0));
			}
			cargoManager = Global.TaskManager.CargoManager;
			cargoManager.serviceController = this;
			orders = new Dictionary<int, Transform>();
		}

		private void OnEnable()
		{
			SetMap();
			currentTask = null;
		}

		public override void CreateOrders()
		{
			if (!_freeDriveProvider.MapCamera.serviceMapIsOpen || locations == null)
			{
				return;
			}
			if (locations.Count == 0)
			{
				Init().Forget();
			}
			base.CreateOrders();
			GetClosePositions();
			index = 0;
			int num = Mathf.Min(UnityEngine.Random.Range(5, 7), closePositions.Count);
			for (int i = 0; i < num; i++)
			{
				int num2 = UnityEngine.Random.Range(0, closePositions.Count - 1);
				int hashCode = num2.GetHashCode();
				int num3;
				do
				{
					num3 = UnityEngine.Random.Range(0, locations.Count - 1);
				}
				while (hashCode == num3.GetHashCode() || !(Vector3.Distance(locations[closePositions[num2]].position, locations[num3].position) > 200f));
				AddTask(locations[closePositions[num2]], locations[num3]);
			}
		}

		public override void ClickStartButton(int? taskID)
		{
			if (!taskID.HasValue)
			{
				return;
			}
			base.ClickStartButton(taskID);
			base.CurrentOrder = base.PhoneWindow.GetCurrentOrder();
			Order order = (Order)base.CurrentOrder;
			Global.TaskManager.OnClear += () =>
			{
				GPSTracker.CancelGPSDraw();
				base.PhoneWindow.EnableAcceptButton(isEnabled: true);
			};
			V.DriverPerson.GetComponent<CargoController>().UpdateTask(order);
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.Instantiate("_Services/Cargo/" + scriptable.trailers[order.id], order.GetDeparture(), order.departure.rotation, 0);
			}
			else
			{
				ResourceRequest rr = Resources.LoadAsync("_Services/Cargo/" + scriptable.trailers[order.id]);
				rr.completed += (AsyncOperation operation) =>
				{
					UnityEngine.Object.Instantiate((GameObject)rr.asset, order.departure.position, order.departure.rotation);
				};
			}
			RemoveTask(taskID.GetValueOrDefault());
			base.PhoneWindow.Close();
			Camera.main.GetComponent<FreeDriveControl>().ShowMainDrivingUI();
		}

		public void AddTask(Location x, Location y)
		{
			float num = GPSTracker.DistanceBetween(x.position, y.position);
			if (!(num.ToString(CultureInfo.InvariantCulture) == "0"))
			{
				int typeId = UnityEngine.Random.Range(0, scriptable.trailers.Count);
				base.PhoneWindow.AddOrder(x, y, (uint)UnityEngine.Random.Range(2500, 5000), num / 1000f, y.city, namesScriptables.GetName(), index, typeId);
				index++;
			}
		}

		public override void RefreshList()
		{
			base.RefreshList();
			closePositions.Clear();
		}

		private void GetClosePositions()
		{
			for (int i = 0; i < locations.Count; i++)
			{
				Vector3 localPosition = V.DriverPerson.transform.localPosition;
				Vector2 a = new Vector2(locations[i].position.x, locations[i].position.z);
				Vector2 b = new Vector2(localPosition.x, localPosition.z);
				if (Vector2.Distance(a, b) < 1000f && Vector2.Distance(a, b) > 10f)
				{
					closePositions.Add(i);
				}
			}
		}

		public void SetMap()
		{
			_freeDriveProvider.MapCamera.serviceMap = base.PhoneWindow.Map;
			_freeDriveProvider.MapCamera.ShowServiceMap();
			GPSTracker.ClearPath(forget: true);
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
	}
}
