using System;
using System.Collections.Generic;
using System.Linq;
using CPM.Account;
using CPM.RemoteConfig;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.CommonUtility;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;

namespace CPM.Scripts.Garage.GarageBuyer
{
	[Preserve]
	public class GarageBuyService : IGarageBuyService, IInitializable, IDeleteUserData
	{
		public static bool BlockGarage2Purchase;

		private readonly IWalletService _walletService;

		private Dictionary<int, bool> _buyingGaragesStatus = new Dictionary<int, bool>();

		private PurchaseItemService _purchaseItemService;

		private IRemoteConfigManager _remoteConfigManager;

		private IGuestAccountCreator _guestAccountCreator;

		public Dictionary<int, bool> BuyingGaragesStatus => _buyingGaragesStatus;

		public int LastBuyingGarageId { get; private set; } = -1;

		public event Action<int> GarageBought;

		public GarageBuyService(IWalletService walletService, PurchaseItemService purchaseItemService, IGuestAccountCreator guestAccountCreator, IDeleterUserDataGetter deleterUserDataGetter, IRemoteConfigManager remoteConfigManager)
		{
			_guestAccountCreator = guestAccountCreator;
			_purchaseItemService = purchaseItemService;
			_walletService = walletService;
			_remoteConfigManager = remoteConfigManager;
			deleterUserDataGetter.Add(this);
		}

		public void Initialize()
		{
			Init();
		}

		public void Init()
		{
			SetBlockGarage2Purchase(value: true).Forget();
			string defaultValue = JsonConvert.SerializeObject(new Dictionary<int, bool>
			{
				{ 0, true },
				{ 1, false }
			});
			string value = ObscuredPrefs.GetString("BuyingGarages", defaultValue);
			_buyingGaragesStatus = JsonConvert.DeserializeObject<Dictionary<int, bool>>(value);
		}

		public void ChangeToDefault()
		{
			foreach (int item in _buyingGaragesStatus.Keys.ToList())
			{
				_buyingGaragesStatus[item] = false;
			}
			_buyingGaragesStatus[0] = true;
			SaveBuyingProgress();
		}

		public void GarageBuySuccess(int garageIndex)
		{
			_buyingGaragesStatus[garageIndex] = true;
			SaveBuyingProgress();
			GarageBought?.Invoke(garageIndex);
		}

		public async UniTask<PurchaseItemResult> ProcessGarageRestore(int garageIndex, PurchaseData purchaseData)
		{
			if (!(await _remoteConfigManager.GetConfigValuesStorage()).legacyGarage)
			{
				V.CoreGameManager.ManagerUI.CloseWaitingWindow();
				return null;
			}
			bool isSettingsWindowActive = V.CoreGameManager.ManagerUI.IsSettingsWindowActive;
			V.CoreGameManager.ManagerUI.ShowWaitingWindow(set: true, isSettingsWindowActive).Forget();
			object payLoad = new
			{
				platform = purchaseData.Platform,
				paymentProductId = purchaseData.ProductId,
				transactionId = purchaseData.TransactionId
			};
			PurchaseItemResult purchaseItemResult = await _purchaseItemService.RestorePurchase(PurchaseItemType.Garage1, payLoad);
			if (purchaseItemResult.IsSuccess)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("PurchaserContent.ThankYouForPurchase"), 3f);
				_buyingGaragesStatus[garageIndex] = true;
				SaveBuyingProgress();
				GarageBought?.Invoke(garageIndex);
				SetBlockGarage2Purchase(value: true).Forget();
			}
			else if (purchaseItemResult.StatusCode == -1)
			{
				V.CoreGameManager.SetMessage(new LocalizedString("DataBase.CanTSaveProgress"), 3f, NotificationType.Error);
			}
			else if (purchaseItemResult.StatusCode == -10)
			{
				V.CoreGameManager.SetMessage("Purchase failed. Please contact game support.", 3f, NotificationType.Error);
			}
			else
			{
				V.CoreGameManager.SetMessage(new LocalizedString("Purchase.RestoreCompleted!"), 2f);
			}
			V.CoreGameManager.ManagerUI.RestoreButtonPressed = false;
			V.CoreGameManager.ManagerUI.CloseWaitingWindow();
			if ((bool)GameOverlayWindow.instance)
			{
				GameOverlayWindow.instance.BackButton.gameObject.SetActive(value: true);
			}
			return purchaseItemResult;
		}

		public AdvancedBoolean GetGarageStatus(int garageIndex)
		{
			return new AdvancedBoolean(_buyingGaragesStatus[garageIndex]);
		}

		public void SaveBuyingProgress()
		{
			string value = JsonConvert.SerializeObject(_buyingGaragesStatus);
			ObscuredPrefs.SetString("BuyingGarages", value);
		}

		public void DeleteData()
		{
			ChangeToDefault();
		}

		public async UniTask SetBlockGarage2Purchase(bool value)
		{
		}
	}
}
