using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Animations.Emotes;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Offers
{
	public class PurchaseCheckerIkPosesGrantable : IPurchaseCheckProductToBuy
	{
		private readonly IOfferSystem _offerSystem;

		private PoseCatalogSO _poseCatalog;

		public PurchaseCheckerIkPosesGrantable(IOfferSystem offerSystem)
		{
			_offerSystem = offerSystem;
		}

		public UniTask<PreBuyActionResult> CheckProductToBuyAsync(string productID)
		{
			OfferModel offer = _offerSystem.GetOffer(productID);
			if (!HasPoseRewards(offer))
			{
				return UniTask.FromResult(PreBuyActionResult.Success);
			}
			return UniTask.FromResult(Check(offer, ResolvePoseCatalog()));
		}

		public static PreBuyActionResult Check(OfferModel offer, PoseCatalogSO poseCatalog)
		{
			RewardItemData[] array = offer?.Payload?.RewardItems;
			if (array == null)
			{
				return PreBuyActionResult.Success;
			}
			foreach (RewardItemData rewardItemData in array)
			{
				if (rewardItemData != null && rewardItemData.Type == RewardType.Poses)
				{
					if (!poseCatalog)
					{
						return PreBuyActionResult.OfferIsOld;
					}
					if (!HasIkPosesForBundle(poseCatalog, rewardItemData.ID))
					{
						return PreBuyActionResult.OfferIsOld;
					}
				}
			}
			return PreBuyActionResult.Success;
		}

		public static bool HasIkPosesForBundle(PoseCatalogSO catalog, int bundleId)
		{
			return catalog.CollectIdsForBundle(bundleId).Count > 0;
		}

		private static bool HasPoseRewards(OfferModel offer)
		{
			RewardItemData[] array = offer?.Payload?.RewardItems;
			if (array == null)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].Type == RewardType.Poses)
				{
					return true;
				}
			}
			return false;
		}

		private PoseCatalogSO ResolvePoseCatalog()
		{
			if (!_poseCatalog)
			{
				_poseCatalog = Resources.Load<PoseCatalogSO>("Configs/PoseCatalog");
			}
			return _poseCatalog;
		}
	}
}
