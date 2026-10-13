using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.VisualTuning;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.Steam.Base;
using _Car_Parking.Scripts.Wallet;

namespace _Car_Parking.Scripts.Other.FsoStrobeLights
{
	public class FsoTuningScope : VisualTuningScope
	{
		public FsoStrobeLightsConfig fsoStrobeLightsConfig;

		private int _lastSelectedItemIndex;

		private FsoStrobeLights _fsoStrobeLights;

		private ExteriorTuning _exteriorTuning;

		private CoinPurchaseWindow _coinPurchaseWindow;

		private IPrefabFactory _prefabFactory;

		private FsoDataFileMediator _fsoDataFileMediator;

		private ISlotService _slotService;

		private CurrencyManagerController _currencyManagerController;

		private ICarInfoService _carInfoService;

		private ManagerUI _managerUI;

		private ISteamAchievementService _steamAchievementService;

		public bool IsSameColor { get; private set; } = true;

		public Color LeftColor => _fsoStrobeLights.ColorLeft;

		public Color RightColor => _fsoStrobeLights.ColorRight;

		[Inject]
		public void Construct(ExteriorTuning exteriorTuning, CoinPurchaseWindow coinPurchaseWindow, IPrefabFactory prefabFactory, ISlotService slotService, CurrencyManagerController currencyManagerController, ICarInfoService carInfoService, ManagerUI managerUI, ISteamAchievementService steamAchievementService)
		{
			_steamAchievementService = steamAchievementService;
			_managerUI = managerUI;
			_carInfoService = carInfoService;
			_currencyManagerController = currencyManagerController;
			_slotService = slotService;
			_prefabFactory = prefabFactory;
			_coinPurchaseWindow = coinPurchaseWindow;
			_exteriorTuning = exteriorTuning;
		}

		public override void Init()
		{
			CarInfo fromSlot = _carInfoService.GetFromSlot(V.SlotID);
			_fsoDataFileMediator = new FsoDataFileMediator(fromSlot);
		}

		public override void Select(int index)
		{
			Color fsoColorLeft = _fsoDataFileMediator.GetFsoColorLeft();
			Color fsoColorRight = _fsoDataFileMediator.GetFsoColorRight();
			SpawnStrobeLightByIndex(index, fsoColorLeft, fsoColorRight);
		}

		public override void Apply()
		{
			int lastSelectedIndex = _lastSelectedItemIndex;
			int coinPrice = fsoStrobeLightsConfig.GetPrice(lastSelectedIndex);
			if (lastSelectedIndex == _fsoDataFileMediator.GetFsoIndex() || FsoStrobeLightsConfig.GetFsoBoughtPrefs(lastSelectedIndex))
			{
				_fsoStrobeLights.SetFsoData(lastSelectedIndex);
				CheckGlowUpForSteam(lastSelectedIndex);
				_managerUI.ShowNotificationWindow(new LocalizedString("FsoTuningScope.Detailinstalled!"), 3f).Forget();
			}
			else
			{
				VisualItemPurchase.Purchase(_currencyManagerController, SpendCoinsType.Fso, lastSelectedIndex, coinPrice, alreadyOwned: false, () =>
				{
					CompleteBuyFso(lastSelectedIndex, coinPrice);
				});
			}
		}

		public override void Revert()
		{
			int fsoIndex = _fsoDataFileMediator.GetFsoIndex();
			int fsoPosition = _fsoDataFileMediator.GetFsoPosition();
			Color fsoColorLeft = _fsoDataFileMediator.GetFsoColorLeft();
			Color fsoColorRight = _fsoDataFileMediator.GetFsoColorRight();
			SpawnStrobeLightByIndex(fsoIndex, fsoColorLeft, fsoColorRight, fsoPosition);
		}

		public void NextPosition()
		{
			if (TryGetFsoStrobeLight())
			{
				_fsoStrobeLights.ChangePositionPoints(Direction.Right);
			}
		}

		public void PrevPosition()
		{
			if (TryGetFsoStrobeLight())
			{
				_fsoStrobeLights.ChangePositionPoints(Direction.Left);
			}
		}

		public void SetDifferentColors(bool value)
		{
			IsSameColor = value;
		}

		private void OnEnable()
		{
			TryGetFsoStrobeLight();
		}

		private void OnDisable()
		{
			Revert();
		}

		private void SpawnStrobeLightByIndex(int index, Color32 colorLeft, Color32 colorRight, int indexPosition = -1)
		{
			if (TryGetFsoStrobeLight())
			{
				_fsoStrobeLights.CurrentPrefab = fsoStrobeLightsConfig.data.Find((OneFsoItemData x) => x.index == index).prefab;
				if (indexPosition != -1)
				{
					_fsoStrobeLights.CurrentSpawnPointIndex = indexPosition;
				}
				_fsoStrobeLights.SetColors(colorLeft, colorRight);
				_fsoStrobeLights.UpdateSpawn();
				_lastSelectedItemIndex = index;
			}
		}

		private bool TryGetFsoStrobeLight()
		{
			return V.Car.TryGetComponentInChildren<FsoStrobeLights>(out _fsoStrobeLights);
		}

		private void CompleteBuyFso(int lastSelectedIndex, int coinPrice)
		{
			FsoStrobeLightsConfig.SetFsoBoughtPrefs(lastSelectedIndex, value: true);
			_fsoStrobeLights.SetFsoData(lastSelectedIndex);
			CheckGlowUpForSteam(lastSelectedIndex);
			_managerUI.ShowNotificationWindow(new LocalizedString("FsoTuningScope.Detailinstalled!"), 3f).Forget();
		}

		private void CheckGlowUpForSteam(int fsoIndex)
		{
		}
	}
}
