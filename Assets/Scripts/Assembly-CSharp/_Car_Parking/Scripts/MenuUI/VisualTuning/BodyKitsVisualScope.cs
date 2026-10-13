using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using Cysharp.Threading.Tasks;
using Racing.CameraManagement;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Economics;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.MenuUI.VisualTuning
{
	public class BodyKitsVisualScope : VisualTuningScope
	{
		private const float DelaySelectBodyKits = 0.1f;

		private CurrencyManagerController _currencyManagerController;

		private VehicleData _vehicleData;

		private BodyKits _bodyKits;

		private VisualTuning _visualTuning;

		private CarInfo _carInfo;

		private ICarInfoService _carInfoService;

		private CameraManager _cameraManager;

		private ManagerUI _managerUI;

		private IPricesService _pricesService;

		private CancellationTokenSource _cancellationTokenSourceDelay;

		private BodyKitsCartData _cartData;

		private readonly Dictionary<BodyKitType, CancellationTokenSource> _loadBodyCancellations = new Dictionary<BodyKitType, CancellationTokenSource>();

		public BodyKitsCartData Cart => _cartData;

		private Array AllBodyKitTypes => Enum.GetValues(typeof(BodyKitType));

		[Inject]
		public void Construct(CurrencyManagerController currencyManagerController, ICarInfoService carInfoService, CameraManager cameraManager, ManagerUI managerUI, IPricesService pricesService, BodyKitsTrimConfigs trimConfigs)
		{
			_pricesService = pricesService;
			_managerUI = managerUI;
			_cameraManager = cameraManager;
			_carInfoService = carInfoService;
			_currencyManagerController = currencyManagerController;
			_cartData = new BodyKitsCartData(trimConfigs);
		}

		public void Register(VisualTuning visualTuning)
		{
			_visualTuning = visualTuning;
		}

		public void InitCarInfo()
		{
			_carInfo = _carInfoService.GetFromSlot(V.SlotID);
			_vehicleData = V.Car.GetComponent<VehicleData>();
			_bodyKits = V.Car.GetComponent<BodyKits>();
			_cartData.Init(_carInfo);
		}

		public override void Init()
		{
			InitCarInfo();
			GlobalEventSystem.InvokeTesterCollisionIgnore(TesterType.Exterior, 0.5f);
			_cameraManager.ActivateCamera(RacingCameraType.GarageLookAround, _vehicleData.transform);
		}

		public override void Apply()
		{
			if (!_cartData.IsFilled)
			{
				Debug.Log("No selected BodyTypes. Don't save.");
			}
			else
			{
				TryBuy();
			}
		}

		public override CombinedPrice GetBoughtItemPrice()
		{
			return _cartData.GetPrice(_pricesService);
		}

		private async UniTaskVoid TryBuy()
		{
			bool canBuy = false;
			CombinedPrice price = _cartData.GetPrice(_pricesService);
			if ((int)price.Coin > 0 || (int)price.Money > 0)
			{
				bool num = (int)price.Coin == 0 || (int)CurrencyManagerController.Coins >= (int)price.Coin;
				bool flag = (int)price.Money == 0 || (int)CurrencyManagerController.Money >= (int)price.Money;
				if (!num)
				{
					_managerUI.NotEnoughCoins();
				}
				else if (!flag)
				{
					_managerUI.NotEnoughMoney();
				}
				if (num & flag)
				{
					if ((int)price.Money > 0)
					{
						AdvancedInt price2 = new AdvancedInt(price.Money);
						_currencyManagerController.TryMoneySpend(out var _, price2, () =>
						{
							canBuy = true;
						});
					}
					if ((int)price.Coin > 0)
					{
						AdvancedInt price3 = new AdvancedInt(price.Coin);
						await _currencyManagerController.TryCoinsSpend(SpendCoinsType.BodyKits, price3, () =>
						{
							canBuy = true;
						}, () =>
						{
							canBuy = false;
						}, allowZero: false);
					}
				}
			}
			else
			{
				canBuy = true;
			}
			if (canBuy)
			{
				for (int num2 = 0; num2 < _cartData.Trims.Count; num2++)
				{
					_carInfo.BoughtBodyKits.AddBoughtBodyKits(BodyKitType.Trim, _cartData.Trims[num2]);
				}
				if (_cartData.BodyKitType != BodyKitType.None)
				{
					_carInfo.BoughtBodyKits.AddBoughtBodyKits(_cartData.BodyKitType, _cartData.BodyKitIndex);
					_carInfo.InstalledBodyKits.GetInstalledBodyKitIndexRef(_cartData.BodyKitType) = _cartData.BodyKitIndex;
				}
				_carInfo.InstalledBodyKits.Trims = _cartData.Trims.ToArray();
				if (FlagsStorage.IsMainMenu)
				{
					_bodyKits.TypesToIntall = _carInfo.InstalledBodyKits.GetInstalledKitIds();
					_bodyKits.TrimsToIntall = _carInfo.InstalledBodyKits.GetInstalledTrims();
				}
				_carInfo.Save();
				_managerUI.ShowNotificationWindow(new LocalizedString("BodyKitsVisualScope.Detailinstalled!"), 3f, NotificationType.Success, SoundEventsDataHelper.InstallVisualTuningSwitch).Forget();
				_cartData.Clear();
			}
			InvokeOnValuesChanged();
			if ((bool)V.Car.GetComponent<HDHeadlighsController>())
			{
				V.Car.GetComponent<HDHeadlighsController>().Init();
			}
		}

		public override void Revert()
		{
			if (!_bodyKits)
			{
				return;
			}
			SetAndLoadDefaultTrims();
			CancelDelayLoadBodyKit();
			foreach (BodyKitType allBodyKitType in AllBodyKitTypes)
			{
				LoadDefaultBodyKitIfNeed(allBodyKitType);
			}
		}

		public override void Discard()
		{
			if ((bool)_bodyKits)
			{
				if (_carInfo.InstalledBodyKits.GetInstalledBodyKitId(_cartData.BodyKitType) == _cartData.BodyKitIndex && _cartData.BodyKitType != BodyKitType.None)
				{
					_carInfo.InstalledBodyKits.GetInstalledBodyKitIndexRef(_cartData.BodyKitType) = 0;
					_carInfo.Save();
				}
				SetAndLoadDefaultTrims();
				InvokeOnValuesChanged();
				base.Discard();
			}
		}

		public override void Select(int index)
		{
			Debug.LogError("Need 2 parameters");
		}

		public void Select(BodyKitType bodyType, int modelIndex)
		{
			Cart.AddSmart(bodyType, modelIndex);
			CancelDelayLoadBodyKit();
			_cancellationTokenSourceDelay = new CancellationTokenSource();
			DelayAndLoadBodyAsync(bodyType, modelIndex, _cancellationTokenSourceDelay.Token).Forget();
			InvokeOnValuesChanged();
		}

		private async UniTaskVoid DelayAndLoadBodyAsync(BodyKitType bodyType, int modelIndex, CancellationToken cancellationToken)
		{
			await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken);
			if (!cancellationToken.IsCancellationRequested)
			{
				LoadBodyAsync(bodyType, modelIndex).Forget();
			}
		}

		private async UniTaskVoid LoadBodyAsync(BodyKitType bodyType, int id)
		{
			BodyKits bodyKits = _bodyKits;
			foreach (BodyKitType allBodyKitType in AllBodyKitTypes)
			{
				if (allBodyKitType != bodyType)
				{
					LoadDefaultBodyKitIfNeed(allBodyKitType);
				}
			}
			CancelLoadBodyKitToken(bodyType);
			CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
			_loadBodyCancellations.Add(bodyType, cancellationTokenSource);
			bodyKits.BodyKitsTrim.SetTrims(_cartData.Trims.ToArray());
			if ((bool)(await bodyKits.LoadBodyKit((int)bodyType, id, cancellationTokenSource.Token)))
			{
				switch (bodyType)
				{
				case BodyKitType.RearBumper:
					_visualTuning.PlaceCamera(2);
					break;
				case BodyKitType.FrontBumper:
				case BodyKitType.Hood:
					_visualTuning.PlaceCamera(0);
					break;
				case BodyKitType.Skirt:
					_visualTuning.PlaceCamera(1);
					break;
				}
			}
		}

		private void CancelLoadBodyKitToken(BodyKitType bodyType)
		{
			if (_loadBodyCancellations.TryGetValue(bodyType, out var value))
			{
				value?.Cancel();
				value?.Dispose();
				_loadBodyCancellations.Remove(bodyType);
			}
		}

		private void CancelDelayLoadBodyKit()
		{
			_cancellationTokenSourceDelay?.Cancel();
			_cancellationTokenSourceDelay?.Dispose();
			_cancellationTokenSourceDelay = null;
		}

		private void LoadDefaultBodyKitIfNeed(BodyKitType bodyKitType)
		{
			if (bodyKitType == BodyKitType.None)
			{
				return;
			}
			CancelLoadBodyKitToken(bodyKitType);
			bodyKitValues installedBodyKitValues = _bodyKits.GetInstalledBodyKitValues(bodyKitType);
			int num = (installedBodyKitValues ? installedBodyKitValues.Index : (-1));
			int installedBodyKitId = _carInfo.InstalledBodyKits.GetInstalledBodyKitId(bodyKitType);
			if (num != installedBodyKitId)
			{
				if (installedBodyKitId >= 0)
				{
					CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
					_loadBodyCancellations.Add(bodyKitType, cancellationTokenSource);
					_bodyKits.LoadBodyKit((int)bodyKitType, installedBodyKitId, cancellationTokenSource.Token).Forget();
				}
				else
				{
					_bodyKits.UnloadPrevKit(bodyKitType, fadeOut: true);
				}
			}
		}

		private void SetAndLoadDefaultTrims()
		{
			_cartData.Clear();
			_bodyKits.BodyKitsTrim.SetTrims(_cartData.Trims.ToArray());
		}
	}
}
