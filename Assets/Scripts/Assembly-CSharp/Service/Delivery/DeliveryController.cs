using CPM.Database;
using CPM.Rating.Entities;
using Photon.Pun;
using Service.Abstract;
using Service.Manager;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;

namespace Service.Delivery
{
	public class DeliveryController : TaskController
	{
		[SerializeField]
		private GameObject Destination;

		[SerializeField]
		private Service.Abstract.Order _order;

		[SerializeField]
		private uint extraForDistance;

		[SerializeField]
		private uint extraForTime;

		[SerializeField]
		private uint reward;

		private DeliveryManager _deliveryManager;

		private readonly Service.Manager.TaskManager _taskManager = Global.TaskManager;

		private FreeDriveProvider _freeDriveProvider;

		private MenuLevelParent _menuLevelParent;

		[Inject]
		private void Construct(FreeDriveProvider freeDriveProvider, MenuLevelParent menuLevelParent)
		{
			_menuLevelParent = menuLevelParent;
			_freeDriveProvider = freeDriveProvider;
		}

		private void Start()
		{
			if (!playerView)
			{
				playerView = PhotonView.Get(this);
			}
			if ((bool)_taskManager)
			{
				_deliveryManager = _taskManager.DeliveryManager;
			}
		}

		public override void Clear()
		{
			GameObject car = _deliveryManager.clients[playerView.OwnerActorNr].Car;
			if ((bool)car)
			{
				Object.Destroy(car.GetComponent<ConfigurableJoint>());
			}
			_menuLevelParent.serviceLabels.Disable().Forget();
			TrailerController obj = (TrailerController)_deliveryManager.clients[playerView.OwnerActorNr];
			_deliveryManager.clients.Remove(playerView.OwnerActorNr);
			obj?.Detach();
			GPSTracker.CancelGPSDraw();
			obj?.Destroy(3f).Forget();
			Object.Destroy(Destination);
			extraForDistance = 0u;
			extraForTime = 0u;
			reward = 0u;
			base.Clear();
		}

		public override void UpdateTask(Service.Abstract.Order order)
		{
			_order = order;
			_taskManager.Task = new Task(order);
			base.UpdateTask(order);
			_taskManager.OnClear += _deliveryManager.Clear;
		}

		protected override void GiveMoney()
		{
			reward += _order.Price;
			CurrencyManagerController.Instance.AddMoney(new AdvancedInt((int)_order.Price));
			base.GiveMoney();
		}

		protected override void GiveExtraReward()
		{
			extraForDistance = (uint)((float)(_order.Price / 5) * _order.GetDistance());
			CurrencyManagerController.Instance.AddMoney(new AdvancedInt((int)extraForDistance));
		}

		private void GiveExtraReward(float time)
		{
			time /= 60f;
			extraForTime = (uint)(time * (float)_order.Price / 5f);
			CurrencyManagerController.Instance.AddMoney(new AdvancedInt((int)extraForTime));
		}

		public void ShowAndHideDestination(TrailerController trailerController, Vector3 destination)
		{
			GameObject currentCar = V.DriverPerson.GetComponent<MultiPersonControl>().currentCar;
			bool show = trailerController.GetState() == CLIENT_STATE.ATTACHED && currentCar == trailerController.Car;
			if (show)
			{
				GPSTracker.DrawGPS(destination, GPSTracker.GPS.DESTINATION, _freeDriveProvider.MapCamera);
			}
			else
			{
				GPSTracker.CancelGPSDraw();
			}
			if (Destination == null)
			{
				ResourceRequest rr = Resources.LoadAsync("_Services/Delivery/TriggerZone");
				rr.completed += (AsyncOperation operation) =>
				{
					Destination = Object.Instantiate((GameObject)rr.asset, V.GlobalPoint);
					Destination.transform.position = destination + V.GlobalOffset;
					Destination.SetActive(show);
				};
			}
			else
			{
				Destination.transform.position = destination + V.GlobalOffset;
				Destination.SetActive(show);
			}
		}

		protected override void Finish()
		{
			float num = 0f - _taskManager.ProgressTime;
			if (num > 0f)
			{
				GiveExtraReward(num);
			}
			FreeDriveDB freeDriveDB = _freeDriveProvider.FreeDriveDB;
			float? delivery = _order.GetDistance();
			freeDriveDB.SendRatingData(new RatingData(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, delivery), forceSend: true).Forget();
			string text = "";
			if (num > 0f)
			{
				text = new LocalizedString("ServiceController.YouGotAndExtraForDistanceAndFastDriving");
				text = string.Format(text, _order.Price, extraForDistance, extraForTime);
			}
			else
			{
				text = new LocalizedString("ServiceController.YouGotAndExtraForDistance");
				text = string.Format(text, _order.Price, extraForDistance);
			}
			_deliveryManager.ShowResult(new LocalizedString("TaxiManager.Complete!"), text, 5f);
			base.Finish();
		}
	}
}
