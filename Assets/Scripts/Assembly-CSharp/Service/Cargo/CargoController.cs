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

namespace Service.Cargo
{
	public class CargoController : TaskController
	{
		[SerializeField]
		private GameObject Destination;

		[SerializeField]
		private Order order;

		private CargoManager _cargoManager;

		private readonly Service.Manager.TaskManager _taskManager = Global.TaskManager;

		[SerializeField]
		private uint extraForDistance;

		[SerializeField]
		private uint extraForTime;

		[SerializeField]
		private uint reward;

		private FreeDriveProvider _freeDriveProvider;

		private MenuLevelParent _menuLevelParent;

		[Inject]
		public void Construct(FreeDriveProvider freeDriveProvider, MenuLevelParent menuLevelParent)
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
				_cargoManager = _taskManager.CargoManager;
			}
		}

		public override void UpdateTask(Service.Abstract.Order order)
		{
			this.order = (Order)order;
			_taskManager.Task = new Task(order);
			base.UpdateTask(order);
			_taskManager.OnClear += _cargoManager.Clear;
		}

		protected override void GiveMoney()
		{
			reward += order.Price;
			CurrencyManagerController.Instance.AddMoney(new AdvancedInt((int)order.Price));
			base.GiveMoney();
		}

		protected override void GiveExtraReward()
		{
			extraForDistance = (uint)((float)(order.Price / 5) * order.GetDistance());
			CurrencyManagerController.Instance.AddMoney(new AdvancedInt((int)extraForDistance));
		}

		private void GiveExtraReward(float time)
		{
			time /= 60f;
			extraForTime = (uint)(time * (float)order.Price / 5f);
			CurrencyManagerController.Instance.AddMoney(new AdvancedInt((int)extraForTime));
		}

		protected override void Finish()
		{
			float progressTime = _taskManager.ProgressTime;
			if (progressTime > 0f)
			{
				GiveExtraReward(progressTime);
			}
			FreeDriveDB freeDriveDB = _freeDriveProvider.FreeDriveDB;
			float? cargo = order.GetDistance();
			freeDriveDB.SendRatingData(new RatingData(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, cargo)).Forget();
			string text = "";
			if (progressTime > 0f)
			{
				text = new LocalizedString("ServiceController.YouGotAndExtraForDistanceAndFastDriving");
				text = string.Format(text, order.Price, extraForDistance, extraForTime);
			}
			else
			{
				text = new LocalizedString("ServiceController.YouGotAndExtraForDistance");
				text = string.Format(text, order.Price, extraForDistance);
			}
			_taskManager.DeliveryManager.ShowResult(new LocalizedString("TaxiManager.Complete!"), text, 5f);
			base.Finish();
		}

		private void OnDestroy()
		{
			if (playerView != null && playerView.ObservedComponents != null)
			{
				playerView.ObservedComponents.Clear();
			}
			playerView = null;
			Destination = null;
			_cargoManager = null;
			_freeDriveProvider = null;
			_menuLevelParent = null;
		}

		public override void Clear()
		{
			GameObject car = _cargoManager.clients[playerView.OwnerActorNr].Car;
			if ((bool)car)
			{
				Object.Destroy(car.GetComponent<ConfigurableJoint>());
			}
			_menuLevelParent.serviceLabels.Disable().Forget();
			TrailerController obj = (TrailerController)_cargoManager.clients[playerView.OwnerActorNr];
			_cargoManager.clients.Remove(playerView.OwnerActorNr);
			obj?.Detach();
			GPSTracker.CancelGPSDraw();
			obj?.Destroy(3f).Forget();
			Object.Destroy(Destination);
			extraForDistance = 0u;
			extraForTime = 0u;
			reward = 0u;
			base.Clear();
		}

		public void ShowAndHideDestination(TrailerController trailerController, Location destination)
		{
			GameObject currentCar = V.DriverPerson.GetComponent<MultiPersonControl>().currentCar;
			bool show = trailerController.GetState() == CLIENT_STATE.ATTACHED && currentCar == trailerController.Car;
			if (show)
			{
				GPSTracker.DrawGPS(destination.position, GPSTracker.GPS.DESTINATION, _freeDriveProvider.MapCamera);
			}
			else
			{
				GPSTracker.CancelGPSDraw();
			}
			if (Destination == null)
			{
				ResourceRequest rr = Resources.LoadAsync("_Services/Cargo/Finish");
				rr.completed += (AsyncOperation operation) =>
				{
					Destination = Object.Instantiate((GameObject)rr.asset, V.GlobalPoint);
					Destination.transform.position = destination.position + V.GlobalOffset + Vector3.up;
					Destination.SetActive(show);
				};
			}
			else
			{
				Destination.transform.position = destination.position + V.GlobalOffset + Vector3.up;
				Destination.transform.rotation = destination.rotation;
				Destination.SetActive(show);
			}
		}
	}
}
