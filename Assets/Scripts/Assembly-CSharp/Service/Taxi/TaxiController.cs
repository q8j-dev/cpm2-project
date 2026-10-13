using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CPM.Database;
using CPM.Rating.Entities;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using Service.Abstract;
using Service.Manager;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.GooglePlayServices;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Providers;
using _Car_Parking.Scripts.Steam.Base;

namespace Service.Taxi
{
	public class TaxiController : TaskController
	{
		public List<int> emojis;

		[Header("monitoring")]
		[SerializeField]
		private GameObject shashka;

		[SerializeField]
		private PhotonView photonView;

		[SerializeField]
		private uint extraForSafety;

		[SerializeField]
		private uint reward;

		[SerializeField]
		private uint extraForTime;

		[SerializeField]
		private uint extra;

		private readonly Service.Manager.TaskManager _taskManager = Global.TaskManager;

		private CurrencyManagerController _currencyManagerController;

		private IEarnMoneyService _earnMoneyService;

		private FreeDriveProvider _freeDriveProvider;

		private ISteamAchievementService _steamAchievementService;

		public GameObject Shashka
		{
			get
			{
				return shashka;
			}
			set
			{
				shashka = value;
			}
		}

		public int AllOrders { get; private set; }

		public int AllRewards { get; private set; }

		[Inject]
		private void Construct(CurrencyManagerController currencyManagerController, IEarnMoneyService earnMoneyService, FreeDriveProvider freeDriveProvider, ISteamAchievementService steamAchievementService)
		{
			_freeDriveProvider = freeDriveProvider;
			_earnMoneyService = earnMoneyService;
			_currencyManagerController = currencyManagerController;
			_steamAchievementService = steamAchievementService;
		}

		public override void Clear()
		{
			if (!playerView)
			{
				playerView = PhotonView.Get(this);
			}
			if ((bool)_taskManager.TaxiManager && _taskManager.TaxiManager.clients.ContainsKey(playerView.OwnerActorNr))
			{
				if ((bool)shashka)
				{
					shashka.SetActive(value: false);
				}
				GameObject car = _taskManager.TaxiManager.clients[playerView.OwnerActorNr].Car;
				if ((bool)car && car.TryGetComponent<VehicleData>(out var component) && component.LightsComposition.IsPolice)
				{
					GetPoliceTransform(car.transform).gameObject.SetActive(value: true);
				}
				if (!PhotonNetwork.IsConnected || playerView.IsMine)
				{
					extraForSafety = 0u;
					reward = 0u;
					extraForTime = 0u;
					extra = 0u;
					base.Clear();
				}
			}
		}

		public new async UniTask UpdateTask(Service.Abstract.Order order)
		{
			await EnsureInit();
			shashka.SetActive(value: true);
			if (!PhotonNetwork.IsConnected || playerView.IsMine)
			{
				_taskManager.Task = new Task(order);
				base.UpdateTask(order);
				_taskManager.OnClear += _taskManager.TaxiManager.Clear;
			}
		}

		private async System.Threading.Tasks.Task EnsureInit()
		{
			if ((object)playerView == null)
			{
				playerView = PhotonView.Get(this);
			}
			GameObject currentCar = GetComponent<MultiPersonControl>().currentCar;
			while (currentCar == null)
			{
				await UniTask.Delay(TimeSpan.FromMilliseconds(100.0));
				currentCar = GetComponent<MultiPersonControl>().currentCar;
			}
			if (shashka == null)
			{
				GameObject original = Resources.Load<GameObject>("FreeParts/Shashka");
				shashka = UnityEngine.Object.Instantiate(original);
			}
			Transform transform = currentCar.transform;
			Transform policeTransform = GetPoliceTransform(transform);
			policeTransform.gameObject.SetActive(value: false);
			shashka.transform.SetParent(transform);
			shashka.transform.SetPositionAndRotation(policeTransform.position, transform.transform.rotation);
		}

		public Transform GetPoliceTransform(Transform car)
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

		protected override void GiveMoney()
		{
			Order order = (Order)_taskManager.Task.Order;
			reward = (uint)((float)order.GetPrice() * order.GetDistance());
			_currencyManagerController.AddMoney(new AdvancedInt((int)reward));
			AllRewards += (int)reward;
			base.GiveMoney();
		}

		protected override void GiveExtraReward()
		{
			Order order = (Order)_taskManager.Task.Order;
			extraForSafety = (uint)((float)order.GetPrice() * order.GetDistance() * (float)_earnMoneyService.TaxiRewardExtraSafetyCoef);
			extra += extraForSafety;
			_currencyManagerController.AddMoney(new AdvancedInt((int)extraForSafety));
			AllRewards += (int)extraForSafety;
			base.GiveExtraReward();
		}

		private void GiveExtraReward(float time)
		{
			Order order = (_taskManager.Task as Task)?.Order as Order;
			extraForTime = (uint)(time * order.GetDistance() * (float)_earnMoneyService.TaxiRewardExtraTimeCoef);
			extra += extraForTime;
			_currencyManagerController.AddMoney(new AdvancedInt((int)extraForTime));
		}

		public override void CancelExtraReward()
		{
			_taskManager.Task.AddCollision();
			if ((object)photonView == null)
			{
				photonView = PhotonView.Get(_taskManager);
			}
			byte b = (byte)emojis[UnityEngine.Random.Range(0, emojis.Count - 1)];
			if (PhotonNetwork.IsConnected)
			{
				photonView.InterestGroupRPC("ShowEmoji", photonView.Group, RpcTarget.All, playerView.ViewID, b);
			}
			else
			{
				_taskManager.TaxiManager.ShowEmoji(playerView.ViewID, b);
			}
			base.CancelExtraReward();
		}

		protected override void Finish()
		{
			float progressTime = _taskManager.ProgressTime;
			if (progressTime > 0f)
			{
				GiveExtraReward(progressTime);
			}
			Task obj = _taskManager.Task as Task;
			Order order = obj?.Order as Order;
			uint num = 5u;
			uint collisionCount = obj.GetCollisionCount();
			if (collisionCount != 0)
			{
				num--;
			}
			if (collisionCount >= 5)
			{
				num--;
			}
			if (progressTime <= 0f)
			{
				num--;
			}
			if (num >= 5)
			{
				GooglePlayAchievements.Unlock("CgkIuKqW0PcZEAIQGA", "taxi_order");
			}
			if (num >= 5)
			{
				_steamAchievementService?.Unlock("ACH_FIVE_STAR_FARE");
			}
			FreeDriveDB freeDriveDB = _freeDriveProvider.FreeDriveDB;
			float? taxi = order.GetDistance();
			freeDriveDB.SendRatingData(new RatingData(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, taxi), forceSend: true).Forget();
			string format = "";
			if (extra != 0)
			{
				if (collisionCount == 0 && progressTime > 0f)
				{
					format = new LocalizedString("TaxiManager.YouGotExtraForSafetyAndFastDriving");
				}
				else if (collisionCount == 0)
				{
					format = new LocalizedString("TaxiManager.YouGotExtraForSafetyDriving");
				}
				else if (progressTime > 0f)
				{
					format = new LocalizedString("TaxiManager.YouGotExtraForFastDriving");
				}
				format = string.Format(format, reward, extra);
			}
			else
			{
				format = new LocalizedString("TaxiManager.YouGotForTaxi");
				format = string.Format(format, reward);
			}
			_taskManager.TaxiManager.ShowResult(new LocalizedString("TaxiManager.Complete!"), format, 5f);
			extra = 0u;
			reward = 0u;
			AllOrders++;
			GooglePlayAchievements.ReportProgress("CgkIuKqW0PcZEAIQKg", "gpg_ach_taxi_completed", AchievementProgress.EffectiveTaxiCompletedTarget, 1, "taxi_order");
			int current = _steamAchievementService.GetStatInt("steam_taxi_completed") + 1;
			_steamAchievementService.ReportProgress("ACH_NIGHT_SHIFT", "steam_taxi_completed", 10, current);
			base.Finish();
		}
	}
}
