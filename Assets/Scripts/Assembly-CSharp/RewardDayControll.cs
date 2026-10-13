using System;
using System.Collections.Generic;
using CPM.DailyTasks;
using CodeStage.AntiCheat.ObscuredTypes;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;

public class RewardDayControll : MonoBehaviour
{
	private static DateTime startDate;

	private ObscuredDouble SecondsLeftPrize;

	public TextMeshProUGUI timeLeftLabelPrize;

	public float overallTime;

	public Image giftIcon;

	public bool PrizeTimeReached;

	public bool ShowingSelling;

	private int offeredSaleID = -1;

	public bool NoAdsAndMoneySale;

	public bool Money1000kSale;

	public bool Money500kSale;

	public bool GetAllCarSale;

	public List<int> offeringIDs;

	private int lastSetIndex;

	private bool incrementCountOffer = true;

	public GameObject giftButton;

	private MultiPlayerMenu multiPlayerMenu;

	private DailyTasksManager dailyTasksManager;

	private IDisposable _tryGetPrizeObservable;

	private IDisposable _offerSaleDisposable;

	private IDisposable _closeAfterSaleWindowDisposable;

	private IDisposable _stopBlinDisposable;

	private IDisposable _incrementOfferTick;

	private IDisposable _blinkLeftLabelTick;

	private CurrencyManagerController _currencyManagerController;

	[Inject]
	public void Construct(MultiPlayerMenu multiPlayerMenu, DailyTasksManager dailyTasksManager, CurrencyManagerController currencyManagerController)
	{
		_currencyManagerController = currencyManagerController;
		this.multiPlayerMenu = multiPlayerMenu;
		this.dailyTasksManager = dailyTasksManager;
	}

	private void Awake()
	{
		incrementCountOffer = true;
		SetStartDate();
		giftButton.SetActive(value: false);
	}

	private async UniTaskVoid WaitForPricePurchase()
	{
		if (!FlagsStorage.StoreInitialized)
		{
			await UniTask.WaitUntil(() => FlagsStorage.StoreInitialized);
		}
		if (offeringIDs.Count > 0 && (V.MenuIndex == 0 || V.MenuIndex == 19))
		{
			_offerSaleDisposable = Observable.Timer(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
			{
				OfferSale();
			}).AddTo(this);
			_incrementOfferTick?.Dispose();
			_incrementOfferTick = Observable.Timer(TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(10.0)).Subscribe((long _) =>
			{
				IncrementOffer();
			}).AddTo(this);
		}
	}

	private void IncrementOffer()
	{
		if (offeringIDs.Count == 0)
		{
			CloseOfferSale();
			return;
		}
		if (lastSetIndex >= offeringIDs.Count)
		{
			lastSetIndex = 0;
			incrementCountOffer = false;
		}
		int offerByID = offeringIDs[lastSetIndex];
		SetOfferByID(offerByID);
		lastSetIndex++;
	}

	private void OfferSale()
	{
		GameOverlayWindow.OfferSale(state: true);
		ShowingSelling = true;
	}

	private void SetOfferByID(int id)
	{
		string textOfferSale = "";
		switch (id)
		{
		case 0:
			Debug.LogError("Remove ads dont work anymore!");
			break;
		case 1:
			if (ObscuredPrefs.GetInt("AllCarGet") == 0)
			{
				textOfferSale = "Get All Cars[S]\nV.getAllCarPrice[/S]\nV.getAllCarPricesale";
				Debug.LogError("Buy call cars dont work anymore!");
				GetAllCarSale = true;
				offeredSaleID = id;
				GameOverlayWindow.IncrementCountOffers(incrementCountOffer);
			}
			else
			{
				offeringIDs.Remove(1);
				IncrementOffer();
			}
			break;
		}
		GameOverlayWindow.SetTextOfferSale(textOfferSale);
	}

	public void OnSalePressed()
	{
		if (offeredSaleID == 0)
		{
			Debug.LogError("Remove ads dont work anymore!");
			offeringIDs.Remove(0);
		}
		IncrementOffer();
	}

	public void CloseOfferSale()
	{
		ShowingSelling = false;
		_closeAfterSaleWindowDisposable = Observable.Timer(TimeSpan.FromSeconds(1.7999999523162842)).Subscribe((long _) =>
		{
			CloseAfterSaleWindow();
		}).AddTo(this);
	}

	private void CloseAfterSaleWindow()
	{
		GameOverlayWindow.OfferSale(state: false);
	}

	private void CheckForReward()
	{
	}

	private void SetStartDate()
	{
		GetInternetTime().Forget();
	}

	public async UniTaskVoid GetInternetTime()
	{
		try
		{
			await UniTask.WaitUntil(() => dailyTasksManager.timeMilliseconds.HasValue).Timeout(TimeSpan.FromMinutes(2.0));
		}
		catch (Exception)
		{
		}
		startDate = UnixTimeStampToDateTime(dailyTasksManager.timeMilliseconds.GetValueOrDefault());
		V.PassedDay = int.Parse(GetDaysPassed()) + 1;
	}

	public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
	{
		return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(unixTimeStamp).ToLocalTime();
	}

	public string GetDaysPassed()
	{
		DateTime value = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
		return startDate.Subtract(value).TotalDays.ToString("0");
	}

	private void UpdatePrice()
	{
		if (!ObscuredPrefs.HasKey("remainingPrize"))
		{
			return;
		}
		if (!DateTime.TryParse(ObscuredPrefs.GetString("remainingPrize"), out var result))
		{
			SetNewTimePrize();
			return;
		}
		TimeSpan timeSpan = result.Subtract(startDate);
		overallTime = ObscuredPrefs.GetInt("overallTime");
		SecondsLeftPrize = timeSpan.TotalSeconds;
		_tryGetPrizeObservable?.Dispose();
		_tryGetPrizeObservable = Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			UpdatePrizeLeftTime();
		}).AddTo(this);
	}

	private void UpdatePrizeLeftTime()
	{
		--SecondsLeftPrize;
		int num = (int)((double)SecondsLeftPrize / 3600.0);
		string text = num.ToString();
		if (num < 10)
		{
			text = "0" + num;
		}
		int num2 = (int)((double)SecondsLeftPrize % 3600.0) / 60;
		string text2 = num2.ToString();
		if (num2 < 10)
		{
			text2 = "0" + num2;
		}
		int num3 = (int)((double)SecondsLeftPrize % 3600.0) % 60;
		string text3 = num3.ToString();
		if (num3 < 10)
		{
			text3 = "0" + num3;
		}
		timeLeftLabelPrize.text = text + ":" + text2 + ":" + text3;
		giftIcon.fillAmount = 1f - (float)(double)SecondsLeftPrize / overallTime;
		if ((double)SecondsLeftPrize < 0.0)
		{
			OnPrizeTimeReached();
		}
	}

	private void OnPrizeTimeReached()
	{
		_tryGetPrizeObservable?.Dispose();
		timeLeftLabelPrize.text = "Get Gift!";
		PrizeTimeReached = true;
	}

	private void SetNewTimePrize()
	{
		int num = UnityEngine.Random.Range(15, 24);
		int num2 = 60 * num * 60;
		DateTime dateTime = DateTime.Now.AddSeconds(num2);
		ObscuredPrefs.SetInt("overallTime", num2);
		overallTime = num2;
		TimeSpan timeSpan = dateTime.Subtract(startDate);
		ObscuredPrefs.SetString("remainingPrize", dateTime.ToString());
		SecondsLeftPrize = timeSpan.TotalSeconds;
		_tryGetPrizeObservable?.Dispose();
		_tryGetPrizeObservable = Observable.Interval(TimeSpan.FromSeconds(1.0)).Subscribe((long _) =>
		{
			UpdatePrizeLeftTime();
		}).AddTo(this);
		PrizeTimeReached = false;
	}

	public void OnGetGiftPressed()
	{
		if (PrizeTimeReached)
		{
			GeneratePrize();
			SetNewTimePrize();
			return;
		}
		_blinkLeftLabelTick?.Dispose();
		_stopBlinDisposable?.Dispose();
		_blinkLeftLabelTick = Observable.Timer(TimeSpan.FromSeconds(0.10000000149011612), TimeSpan.FromSeconds(0.4000000059604645)).Subscribe((long _) =>
		{
			BlinkLeftLabel();
		}).AddTo(this);
		_stopBlinDisposable = Observable.Timer(TimeSpan.FromSeconds(3.0)).Subscribe((long _) =>
		{
			StopBlin();
		}).AddTo(this);
	}

	private void GeneratePrize()
	{
		int num = UnityEngine.Random.Range(0, 2);
		if (V.PassedDay > 5)
		{
			num = UnityEngine.Random.Range(0, 10);
		}
		if (num > 2)
		{
			int num2 = UnityEngine.Random.Range(0, 4);
			if (num2 == 0)
			{
				num = 0;
			}
			if (num2 == 1)
			{
				num = 1;
			}
		}
		switch (num)
		{
		case 0:
			GivRandomMoney();
			return;
		case 1:
		{
			List<int> list = new List<int>();
			for (int i = 0; i <= 30; i++)
			{
				ObscuredInt obscuredInt = V.CoreGameManager.CarOrder[i];
				if (ObscuredPrefs.GetInt("car" + obscuredInt.ToString()) == 0)
				{
					list.Add(V.CoreGameManager.CarOrder[i]);
				}
			}
			if (list.Count > 0)
			{
				int index = UnityEngine.Random.Range(0, list.Count);
				ObscuredPrefs.SetInt("car" + list[index], 1);
				V.CoreGameManager.SetMessage(new LocalizedString("RewardDayControll.CongratulationsYourGiftIsNewCar"), 5f);
				return;
			}
			break;
		}
		case 2:
			if (ObscuredPrefs.GetInt("horn" + 2) == 0)
			{
				ObscuredPrefs.SetInt("horn" + 2, 1);
				ObscuredPrefs.SetInt("hornID" + V.CarId.ToString(), 2);
				ObscuredPrefs.Save();
				V.CoreGameManager.SetMessage(new LocalizedString("RewardDayControll.CongratulationsYourGiftIsNewHornSound"), 5f);
				return;
			}
			break;
		case 3:
			if (ObscuredPrefs.GetInt("horn" + 3) == 0)
			{
				ObscuredPrefs.SetInt("horn" + 3, 1);
				ObscuredPrefs.SetInt("hornID" + V.CarId.ToString(), 3);
				ObscuredPrefs.Save();
				V.CoreGameManager.SetMessage(new LocalizedString("RewardDayControll.CongratulationsYourGiftIsNewHornSound"), 5f);
				return;
			}
			break;
		case 4:
			if (ObscuredPrefs.GetInt("wheelPurchased" + 84) == 0)
			{
				ObscuredPrefs.SetInt("wheelPurchased" + 84, 1);
				ObscuredPrefs.Save();
				V.CoreGameManager.SetMessage(new LocalizedString("RewardDayControll.CongratulationsYourGiftIsNewRim"), 5f);
				return;
			}
			break;
		case 5:
			if (ObscuredPrefs.GetInt("DiodColor") == 0)
			{
				ObscuredPrefs.SetInt("DiodColor", 1);
				V.CoreGameManager.SetMessage(new LocalizedString("RewardDayControll.CongratulationsNowYouCanChangeHeadlightColor"), 5f);
				return;
			}
			break;
		case 6:
			if (ObscuredPrefs.GetInt("wheelPurchased" + 87) == 0)
			{
				ObscuredPrefs.SetInt("wheelPurchased" + 87, 1);
				ObscuredPrefs.Save();
				V.CoreGameManager.SetMessage(new LocalizedString("RewardDayControll.CongratulationsYourGiftIsNewRim"), 5f);
				return;
			}
			break;
		case 7:
			if (ObscuredPrefs.GetInt("wheelPurchased" + 88) == 0)
			{
				ObscuredPrefs.SetInt("wheelPurchased" + 88, 1);
				ObscuredPrefs.Save();
				V.CoreGameManager.SetMessage(new LocalizedString("RewardDayControll.CongratulationsYourGiftIsNewRim"), 5f);
				return;
			}
			break;
		case 8:
			if (ObscuredPrefs.GetInt("wheelPurchased" + 97) == 0)
			{
				ObscuredPrefs.SetInt("wheelPurchased" + 97, 1);
				ObscuredPrefs.Save();
				V.CoreGameManager.SetMessage(new LocalizedString("RewardDayControll.CongratulationsYourGiftIsNewRim"), 5f);
				return;
			}
			break;
		case 9:
			if (ObscuredPrefs.GetInt("wheelPurchased" + 98) == 0)
			{
				ObscuredPrefs.SetInt("wheelPurchased" + 98, 1);
				ObscuredPrefs.Save();
				V.CoreGameManager.SetMessage(new LocalizedString("RewardDayControll.CongratulationsYourGiftIsNewRim"), 5f);
				return;
			}
			break;
		}
		GivRandomMoney();
	}

	private void GivRandomMoney()
	{
		ObscuredInt obscuredInt = UnityEngine.Random.Range(70, 130);
		obscuredInt = (int)obscuredInt * 1000;
		_currencyManagerController.AddMoney(new AdvancedInt(obscuredInt));
		CoreGameManager coreGameManager = V.CoreGameManager;
		ObscuredInt obscuredInt2 = obscuredInt;
		coreGameManager.SetMessage("Your gift is game money!\n+" + obscuredInt2.ToString(), 4f);
	}

	private void BlinkLeftLabel()
	{
		timeLeftLabelPrize.gameObject.SetActive(!timeLeftLabelPrize.gameObject.activeSelf);
	}

	private void StopBlin()
	{
		_stopBlinDisposable?.Dispose();
		_blinkLeftLabelTick?.Dispose();
		_blinkLeftLabelTick = null;
		timeLeftLabelPrize.gameObject.SetActive(value: true);
	}

	private void SaveRemaining()
	{
	}
}
