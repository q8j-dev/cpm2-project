using System;

namespace _Car_Parking.Scripts.Offers
{
	[Serializable]
	public class OfferPayloadModel
	{
		public string ItemID;

		public string OldBundleID;

		public OfferType OfferType;

		public PurchaseOptionData[] PurchaseOptions;

		public RewardItemData[] RewardItems;

		public int Discount;

		public int Priority;

		public float OldPriceModifier;

		public float OldPrice = -1f;

		public string OldPriceText = "";

		public bool HiddenFromList;

		public bool HavePurchaseOptionCoins
		{
			get
			{
				if (PurchaseOptions != null)
				{
					for (int i = 0; i < PurchaseOptions.Length; i++)
					{
						if (PurchaseOptions[i].Type == PurchaseType.Coins)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public bool HaveOldBundleID => !string.IsNullOrEmpty(OldBundleID);

		public string FirstBundleID
		{
			get
			{
				if (PurchaseOptions == null)
				{
					return "";
				}
				for (int i = 0; i < PurchaseOptions.Length; i++)
				{
					if (PurchaseOptions[i].Type == PurchaseType.IAP)
					{
						return PurchaseOptions[i].BundleID;
					}
				}
				return "";
			}
		}

		public int FirstSteamItemDefId
		{
			get
			{
				if (PurchaseOptions == null)
				{
					return 0;
				}
				for (int i = 0; i < PurchaseOptions.Length; i++)
				{
					if (PurchaseOptions[i].Type == PurchaseType.IAP)
					{
						return PurchaseOptions[i].SteamItemDefId;
					}
				}
				return 0;
			}
		}

		public bool HaveOldPrice => OldPrice > 0f;

		public int RewardCount
		{
			get
			{
				RewardItemData[] rewardItems = RewardItems;
				if (rewardItems == null)
				{
					return 0;
				}
				return rewardItems.Length;
			}
		}

		public bool RewardIsCar
		{
			get
			{
				if (RewardCount == 1)
				{
					return RewardItems[0].Type == RewardType.Car;
				}
				return false;
			}
		}

		public bool HaveRewardSuitPack
		{
			get
			{
				if (RewardItems == null)
				{
					return false;
				}
				for (int i = 0; i < RewardItems.Length; i++)
				{
					if (RewardItems[i].Type == RewardType.SuitPack)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool HaveRewardIkPoses
		{
			get
			{
				if (RewardItems == null)
				{
					return false;
				}
				for (int i = 0; i < RewardItems.Length; i++)
				{
					if (RewardItems[i].Type == RewardType.Poses)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool HaveRewardAnimations
		{
			get
			{
				if (RewardItems == null)
				{
					return false;
				}
				for (int i = 0; i < RewardItems.Length; i++)
				{
					if (RewardItems[i].Type == RewardType.Animations)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool IsBundle
		{
			get
			{
				if (!HaveRewardSuitPack && !HaveRewardIkPoses)
				{
					return HaveRewardAnimations;
				}
				return true;
			}
		}

		public int FirstRewardCarId
		{
			get
			{
				if (RewardItems == null)
				{
					return -1;
				}
				for (int i = 0; i < RewardItems.Length; i++)
				{
					if (RewardItems[i].Type == RewardType.Car)
					{
						return RewardItems[i].ID;
					}
				}
				return -1;
			}
		}

		public void Init()
		{
			if (RewardItems == null)
			{
				RewardItems = Array.Empty<RewardItemData>();
			}
			if (PurchaseOptions == null)
			{
				PurchaseOptions = Array.Empty<PurchaseOptionData>();
			}
		}

		public bool HaveRewardCarId(int carId)
		{
			if (RewardItems == null)
			{
				return false;
			}
			for (int i = 0; i < RewardItems.Length; i++)
			{
				if (RewardItems[i].Type == RewardType.Car && RewardItems[i].ID == carId)
				{
					return true;
				}
			}
			return false;
		}

		public bool HaveRewardCarIdAndHaveSuit(int carId)
		{
			if (HaveRewardCarId(carId))
			{
				return HaveRewardSuitPack;
			}
			return false;
		}
	}
}
