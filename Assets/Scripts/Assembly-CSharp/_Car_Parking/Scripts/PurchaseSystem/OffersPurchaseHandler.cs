using System;
using System.Collections.Generic;
using CPM.Connection;
using Cysharp.Threading.Tasks;
using Firebase.Functions;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.Analytics.Data;
using _Car_Parking.Scripts.Animations.Emotes.Locker;
using _Car_Parking.Scripts.GrantService;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.MenuUI.Person;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.PurchaseSystem.Base.SteamPayload;
using _Car_Parking.Scripts.Steam;
using _Car_Parking.Scripts.Wallet;
using _Car_Parking.Scripts.Wallet.PurchaseItemProviders;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	[Preserve]
	public class OffersPurchaseHandler : IPurchaseHandler, IDisposable
	{
		public const string GetOfferByOfferIdFn = "GetOfferByOfferId24_1";

		private readonly List<IPurchaseCheckProductToBuy> _purchaseChecksProductToBuy = new List<IPurchaseCheckProductToBuy>();

		private readonly IOfferSystem _offerSystem;

		private readonly CommonProvider _commonProvider;

		private readonly IWalletService _walletService;

		private readonly RewardGranter _rewardGranter;

		private readonly AnimationShopOpenRequest _animationShopOpenRequest;

		private readonly EmoteLockerOpenRequest _emoteLockerOpenRequest;

		public OffersPurchaseHandler(IConnectionManager connectionManager, IOfferSystem offerSystem, CommonProvider commonProvider, IWalletService walletService, RewardGranter rewardGranter, AnimationShopOpenRequest animationShopOpenRequest, EmoteLockerOpenRequest emoteLockerOpenRequest)
		{
			_offerSystem = offerSystem;
			_commonProvider = commonProvider;
			_walletService = walletService;
			_rewardGranter = rewardGranter;
			_animationShopOpenRequest = animationShopOpenRequest;
			_emoteLockerOpenRequest = emoteLockerOpenRequest;
			_purchaseChecksProductToBuy.Add(new PurchaseCheckerIkPosesGrantable(_offerSystem));
			_purchaseChecksProductToBuy.Add(new PurchaseCheckerAnimationsGrantable(_offerSystem));
			_purchaseChecksProductToBuy.Add(new NewCarPurchaseCheckerGarageIsFully(showMessageBeforeBuySlot: true));
			_purchaseChecksProductToBuy.Add(new PurchaseCheckerHashCheck(connectionManager));
			_purchaseChecksProductToBuy.Add(new PurchaseCheckerServerAvailable());
			_purchaseChecksProductToBuy.Add(new PurchaseCheckerOfferVersion(_offerSystem));
		}

		public void Dispose()
		{
		}

		public bool IsHandlerForProduct(string productID)
		{
			if (_offerSystem.GetOffer(productID) != null)
			{
				return true;
			}
			int offerID;
			return _offerSystem.TryGetOfferFromCache(productID, out offerID);
		}

		public async UniTask<PreBuyActionResult> PreBuyAction(string productID)
		{
			bool offerHasCar = OfferHasCar(ResolveOffer(productID));
			for (int i = 0; i < _purchaseChecksProductToBuy.Count; i++)
			{
				if (!offerHasCar && _purchaseChecksProductToBuy[i] is NewCarPurchaseCheckerGarageIsFully)
				{
					continue;
				}
				PreBuyActionResult preBuyActionResult = await _purchaseChecksProductToBuy[i].CheckProductToBuyAsync(productID);
				if (preBuyActionResult == PreBuyActionResult.Success)
				{
					continue;
				}
				if (preBuyActionResult == PreBuyActionResult.OfferIsOld)
				{
					OfferModel offer = _offerSystem.GetOffer(productID);
					if (offer != null)
					{
						offer.Enabled = false;
					}
					ManagerUIHelper.SetMessage(new LocalizedString("Offers.OfferIsNoValid"), 3f);
				}
				return preBuyActionResult;
			}
			return PreBuyActionResult.Success;
		}

		private static bool OfferHasCar(OfferModel offer)
		{
			if (offer == null || offer.Payload == null)
			{
				return true;
			}
			return offer.Payload.FirstRewardCarId >= 0;
		}

		private static RewardItemData[] FilterRegrantableRewards(RewardItemData[] rewards)
		{
			if (rewards == null)
			{
				return Array.Empty<RewardItemData>();
			}
			List<RewardItemData> list = new List<RewardItemData>();
			for (int i = 0; i < rewards.Length; i++)
			{
				if (rewards[i] != null && (rewards[i].Type == RewardType.Animations || rewards[i].Type == RewardType.Poses))
				{
					list.Add(rewards[i]);
				}
			}
			return list.ToArray();
		}

		private OfferModel ResolveOffer(string productID)
		{
			OfferModel offer = _offerSystem.GetOffer(productID);
			if (offer != null)
			{
				return offer;
			}
			if (_offerSystem.TryGetOfferFromCache(productID, out var offerID))
			{
				return _offerSystem.GetOffer(offerID);
			}
			return null;
		}

		public async UniTask<PurchaseItemResultStatus> ProcessPurchaseAsync(PurchaseEventArgs args, PurchaseProduct purchaseProduct, string attemptId)
		{
			OfferModel offer = _offerSystem.GetOffer(purchaseProduct.ProductID);
			OfferModel offerModelCurrent = null;
			if (_offerSystem.TryGetOfferFromCache(purchaseProduct.ProductID, out var offerID))
			{
				offerModelCurrent = _offerSystem.GetOffer(offerID);
				if (offerModelCurrent == null)
				{
					FunctionCallResult<OfferModel> functionCallResult = await new FirebaseWrappedFunctionFactory().Create("GetOfferByOfferId24_1").CallAsyncResult<OfferModel>(offerID);
					if (functionCallResult.IsSuccess)
					{
						offerModelCurrent = functionCallResult.Data;
					}
				}
			}
			if (offerModelCurrent != null && offerModelCurrent.Payload.FirstBundleID == purchaseProduct.ProductID)
			{
				offer = offerModelCurrent;
			}
			if (offer == null)
			{
				return FailWithMessage(PurchaseItemResultStatus.Error);
			}
			bool offerHasCar = OfferHasCar(offer);
			int newSlotGarageIndex = -1;
			if (offerHasCar && !_commonProvider.SlotService.TryGetEmptySlot(out var _))
			{
				newSlotGarageIndex = _commonProvider.SlotService.GetFirstGarageIndexNotFull();
			}
			PurchasePayloadOffer payLoad = new PurchasePayloadOffer(args.purchasedProduct.definition.id, args.purchasedProduct.transactionID, attemptId, offer.ID, newSlotGarageIndex);
			ItemData data = new ItemData(offer.Payload.FirstBundleID, payLoad);
			WalletOperationResult walletOperationResult;
			try
			{
				walletOperationResult = await _walletService.PurchaseItem(data);
			}
			catch (Exception exception)
			{
				walletOperationResult = new WalletOperationResult(0, 0);
				Debug.LogException(exception);
			}
			if (walletOperationResult == null || !walletOperationResult.IsSuccess())
			{
				PurchaseItemResultStatus purchaseItemResultStatus = ((walletOperationResult != null) ? ServerPurchaseStatusMapper.ToResultStatus(walletOperationResult.Success) : PurchaseItemResultStatus.Error);
				if (purchaseItemResultStatus == PurchaseItemResultStatus.ItemWasBought)
				{
					if (offer.Payload.HaveRewardAnimations || offer.Payload.HaveRewardIkPoses)
					{
						if (!(await _rewardGranter.GrantAsync(FilterRegrantableRewards(offer.Payload.RewardItems))))
						{
							return FailWithMessage(PurchaseItemResultStatus.Error);
						}
						offer.Enabled = false;
						CompletePurchaseUI(offer);
					}
					_offerSystem.RemoveOfferFromCache(offer.Payload.FirstBundleID);
					return PurchaseItemResultStatus.ItemWasBought;
				}
				return FailWithMessage(purchaseItemResultStatus);
			}
			if (offerHasCar && !_commonProvider.SlotService.TryGetEmptySlot(out var _) && (!_commonProvider.SlotService.TryGetFreeSlot(newSlotGarageIndex) || newSlotGarageIndex == -1))
			{
				return FailWithMessage(PurchaseItemResultStatus.Error);
			}
			RewardItemData[] rewardItems = offer.Payload.RewardItems;
			RewardItemData[] array = new RewardItemData[(rewardItems != null) ? rewardItems.Length : 0];
			for (int i = 0; i < array.Length; i++)
			{
				RewardItemData rewardItemData = rewardItems[i];
				if (rewardItemData != null)
				{
					array[i] = ((rewardItemData.Type == RewardType.Car) ? new RewardItemCarData
					{
						Type = rewardItemData.Type,
						ID = rewardItemData.ID,
						URL = rewardItemData.URL,
						CarInstanceId = walletOperationResult.Data
					} : rewardItemData);
				}
			}
			if (!(await _rewardGranter.GrantAsync(array)))
			{
				return FailWithMessage(PurchaseItemResultStatus.Error);
			}
			offer.Enabled = false;
			_offerSystem.RemoveOfferFromCache(offer.Payload.FirstBundleID);
			CompletePurchaseUI(offer);
			return PurchaseItemResultStatus.Success;
		}

		public async UniTask<bool> ProcessSteamPurchaseAsync(OfferModel offer, SteamPurchaseData steamData)
		{
			if (offer == null)
			{
				return false;
			}
			bool offerHasCar = OfferHasCar(offer);
			int freeSlotGarageIndex = -1;
			if (offerHasCar && !_commonProvider.SlotService.TryGetEmptySlot(out var _))
			{
				freeSlotGarageIndex = _commonProvider.SlotService.GetFirstGarageIndexNotFull();
			}
			SteamPurchasePayloadOffer payLoad = new SteamPurchasePayloadOffer(offer.Payload.FirstBundleID, steamData.TransactionID, offer.ID, freeSlotGarageIndex, steamData.Itemdefid, steamData.Name);
			ItemData data = new ItemData(offer.Payload.FirstBundleID, payLoad);
			WalletOperationResult walletOperationResult;
			try
			{
				walletOperationResult = await _walletService.PurchaseItemSteam(data);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return false;
			}
			if (walletOperationResult == null || !walletOperationResult.IsSuccess())
			{
				if (walletOperationResult != null && walletOperationResult.Success == 2)
				{
					_offerSystem.RemoveOfferFromCache(offer.Payload.FirstBundleID);
				}
				return false;
			}
			if (offerHasCar && !_commonProvider.SlotService.TryGetEmptySlot(out var _) && (!_commonProvider.SlotService.TryGetFreeSlot(freeSlotGarageIndex) || freeSlotGarageIndex == -1))
			{
				return false;
			}
			RewardItemData[] rewardItems = offer.Payload.RewardItems;
			RewardItemData[] array = new RewardItemData[(rewardItems != null) ? rewardItems.Length : 0];
			for (int i = 0; i < array.Length; i++)
			{
				RewardItemData rewardItemData = rewardItems[i];
				if (rewardItemData != null)
				{
					array[i] = ((rewardItemData.Type == RewardType.Car) ? new RewardItemCarData
					{
						Type = rewardItemData.Type,
						ID = rewardItemData.ID,
						URL = rewardItemData.URL,
						CarInstanceId = walletOperationResult.Data
					} : rewardItemData);
				}
			}
			if (!(await _rewardGranter.GrantAsync(array)))
			{
				return false;
			}
			offer.Enabled = false;
			_offerSystem.RemoveOfferFromCache(offer.Payload.FirstBundleID);
			return true;
		}

		private void CompletePurchaseUI(OfferModel offer)
		{
			if (offer.Payload.HaveRewardIkPoses)
			{
				OpenAnimationShop(AnimationShopTab.Poses, FirstRewardBundleId(offer, RewardType.Poses));
				ManagerUIHelper.SetMessage(new LocalizedString("AnimationShop.Success"), 3f);
			}
			else if (offer.Payload.HaveRewardAnimations)
			{
				OpenAnimationShop(AnimationShopTab.Animations, FirstRewardBundleId(offer, RewardType.Animations));
				ManagerUIHelper.SetMessage(new LocalizedString("AnimationShop.Success"), 3f);
			}
			else
			{
				ManagerUIHelper.SetMessage(new LocalizedString("PurchaserContent.ThankYouForPurchase"), 3f);
			}
		}

		private static int FirstRewardBundleId(OfferModel offer, RewardType type)
		{
			RewardItemData[] rewardItems = offer.Payload.RewardItems;
			if (rewardItems == null)
			{
				return -1;
			}
			for (int i = 0; i < rewardItems.Length; i++)
			{
				if (rewardItems[i] != null && rewardItems[i].Type == type)
				{
					return rewardItems[i].ID;
				}
			}
			return -1;
		}

		private void OpenAnimationShop(AnimationShopTab tab, int focusBundleId)
		{
			CoreGameManager coreGameManager = V.CoreGameManager;
			if ((bool)coreGameManager && (bool)coreGameManager.ManagerUI)
			{
				if (FlagsStorage.IsMainMenu)
				{
					_animationShopOpenRequest.Set(tab, focusBundleId);
					coreGameManager.menuControl.ShowSelectDriver(JsonConvert.SerializeObject(new PersonWindowAdditionalData
					{
						ParentWindow = PersonWindowAdditionalData.ParentWindowType.MainMenu,
						OpenCategory = AnimationShopRouting.CategoryFor(tab)
					}));
					coreGameManager.ManagerUI.ShowUserBalanceWindow();
				}
				else
				{
					_emoteLockerOpenRequest.Set(new EmoteLockerContext
					{
						OpenPickerTab = tab,
						FocusBundleId = focusBundleId
					});
					coreGameManager.ManagerUI.ShowEmoteLockerWindow();
				}
			}
		}

		public string GetOfferId(string productID)
		{
			OfferModel offer = _offerSystem.GetOffer(productID);
			if (offer != null)
			{
				return offer.ID.ToString();
			}
			if (_offerSystem.TryGetOfferFromCache(productID, out var offerID))
			{
				return offerID.ToString();
			}
			return "-1";
		}

		public List<ItemIdData> GetItemIds(string productID)
		{
			List<ItemIdData> list = new List<ItemIdData>();
			OfferModel offer = _offerSystem.GetOffer(productID);
			OfferToBuyCache.OfferCacheData cacheData;
			if (offer != null)
			{
				for (int i = 0; i < offer.Payload.RewardItems.Length; i++)
				{
					list.Add(new ItemIdData
					{
						ItemId = offer.Payload.RewardItems[i].ID.ToString(),
						Amount = 1
					});
				}
			}
			else if (_offerSystem.TryGetOfferCacheFromCache(productID, out cacheData))
			{
				for (int j = 0; j < cacheData.Items.Length; j++)
				{
					list.Add(new ItemIdData
					{
						ItemId = cacheData.Items[j].ToString(),
						Amount = 1
					});
				}
			}
			return list;
		}

		public async UniTask PurchaseFailedAsync(PurchaseProduct purchaseProduct, Product product, PurchaseFailureReason failureReason)
		{
		}

		private static PurchaseItemResultStatus FailWithMessage(PurchaseItemResultStatus status)
		{
			ManagerUIHelper.SetMessage(new LocalizedString("DebugAccountData.SomethingWentWrong"), 3f, NotificationType.Error);
			return status;
		}
	}
}
