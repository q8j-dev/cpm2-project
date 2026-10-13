using System;
using System.Collections.Generic;
using System.Globalization;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Abstract;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Providers;

namespace Service.Delivery
{
	public class ServiceController : Service.Abstract.ServiceController
	{
		private List<int> closePositions = new List<int>();

		private DeliveryManager deliveryManager;

		private float offset = 320f;

		private NamesScriptables namesScriptables;

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
			deliveryManager = Global.TaskManager.DeliveryManager;
			deliveryManager.serviceController = this;
			orders = new Dictionary<int, Transform>();
		}

		private void OnEnable()
		{
			SetMap();
			GPSTracker.ClearPath(forget: true);
			currentTask = null;
		}

		public override void CreateOrders()
		{
			if (!_freeDriveProvider.MapCamera.serviceMapIsOpen)
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

		private void GetClosePositions()
		{
			for (int i = 0; i < locations.Count; i++)
			{
				Transform obj = V.DriverPerson.transform;
				Vector2 a = new Vector2(locations[i].position.x, locations[i].position.z);
				Vector3 localPosition = obj.localPosition;
				Vector2 b = new Vector2(localPosition.x, localPosition.z);
				if (Vector2.Distance(a, b) < 1000f && Vector2.Distance(a, b) > 10f)
				{
					closePositions.Add(i);
				}
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
			object[] data = new object[1] { base.PhoneWindow.CurrentTask };
			Global.TaskManager.OnClear += () =>
			{
				GPSTracker.CancelGPSDraw();
				base.PhoneWindow.EnableAcceptButton(isEnabled: true);
			};
			V.DriverPerson.GetComponent<DeliveryController>().UpdateTask(base.CurrentOrder);
			if (PhotonNetwork.IsConnected)
			{
				PhotonNetwork.Instantiate("_Services/Delivery/Trailer_1x1", base.CurrentOrder.GetDeparture(), Quaternion.identity, 0, data);
			}
			else
			{
				ResourceRequest rr = Resources.LoadAsync("_Services/Delivery/Trailer_1x1");
				rr.completed += (AsyncOperation operation) =>
				{
					UnityEngine.Object.Instantiate((GameObject)rr.asset, base.CurrentOrder.GetDeparture(), Quaternion.identity);
				};
			}
			RemoveTask(taskID.GetValueOrDefault());
			base.PhoneWindow.Close();
			Camera.main.GetComponent<FreeDriveControl>().ShowMainDrivingUI();
		}

		public void SetMap()
		{
			_freeDriveProvider.MapCamera.serviceMap = base.PhoneWindow.Map;
			_freeDriveProvider.MapCamera.ShowServiceMap();
		}

		public void AddTask(Location x, Location y)
		{
			float num = GPSTracker.DistanceBetween(x.position, y.position);
			if (!(num.ToString(CultureInfo.InvariantCulture) == "0"))
			{
				int typeId = UnityEngine.Random.Range(0, deliveryManager.scriptablesCount);
				base.PhoneWindow.AddOrder(x, y, 0u, num / 1000f, y.city, namesScriptables.GetName(), index, typeId);
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

		public override void RefreshList()
		{
			base.RefreshList();
			closePositions.Clear();
		}
	}
}
