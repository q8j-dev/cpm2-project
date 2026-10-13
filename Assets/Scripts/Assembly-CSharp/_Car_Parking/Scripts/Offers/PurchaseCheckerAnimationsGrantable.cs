using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.PurchaseSystem;

namespace _Car_Parking.Scripts.Offers
{
	public class PurchaseCheckerAnimationsGrantable : IPurchaseCheckProductToBuy
	{
		private readonly IOfferSystem _offerSystem;

		private AnimationDataSO _animationData;

		public PurchaseCheckerAnimationsGrantable(IOfferSystem offerSystem)
		{
			_offerSystem = offerSystem;
		}

		public UniTask<PreBuyActionResult> CheckProductToBuyAsync(string productID)
		{
			OfferModel offer = _offerSystem.GetOffer(productID);
			if (!HasAnimationRewards(offer))
			{
				return UniTask.FromResult(PreBuyActionResult.Success);
			}
			return UniTask.FromResult(Check(offer, ResolveAnimationData()));
		}

		public static PreBuyActionResult Check(OfferModel offer, AnimationDataSO animationData)
		{
			RewardItemData[] array = offer?.Payload?.RewardItems;
			if (array == null)
			{
				return PreBuyActionResult.Success;
			}
			foreach (RewardItemData rewardItemData in array)
			{
				if (rewardItemData != null && rewardItemData.Type == RewardType.Animations)
				{
					if (!animationData)
					{
						return PreBuyActionResult.OfferIsOld;
					}
					if (animationData.CollectYsForBundle(rewardItemData.ID).Count == 0)
					{
						return PreBuyActionResult.OfferIsOld;
					}
				}
			}
			return PreBuyActionResult.Success;
		}

		private static bool HasAnimationRewards(OfferModel offer)
		{
			RewardItemData[] array = offer?.Payload?.RewardItems;
			if (array == null)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] != null && array[i].Type == RewardType.Animations)
				{
					return true;
				}
			}
			return false;
		}

		private AnimationDataSO ResolveAnimationData()
		{
			if (!_animationData)
			{
				_animationData = Resources.Load<AnimationDataSO>("Configs/AnimationData");
			}
			return _animationData;
		}
	}
}
