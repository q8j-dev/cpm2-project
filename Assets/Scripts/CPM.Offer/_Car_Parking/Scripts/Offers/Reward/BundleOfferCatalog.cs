using System.Collections.Generic;
using UnityEngine;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Offers.Reward
{
	public sealed class BundleOfferCatalog : IInitializable
	{
		private const int HeadwearTypeId = 3;

		private readonly IOfferSystem _offerSystem;

		private readonly ISuitPacksConfig _packs;

		private readonly Dictionary<(int gender, int typeId, int itemId), int> _packItemLookup = new Dictionary<(int, int, int), int>();

		public BundleOfferCatalog(IOfferSystem offerSystem, ISuitPacksConfig packs)
		{
			_offerSystem = offerSystem;
			_packs = packs;
		}

		public void Initialize()
		{
			_packItemLookup.Clear();
			foreach (int allPackId in _packs.GetAllPackIds())
			{
				if (_packs.TryGetPackItems(allPackId, out var items) && items != null)
				{
					for (int i = 0; i < items.Count; i++)
					{
						_packItemLookup[(items[i].BodyType, items[i].TypeId, items[i].ItemId)] = allPackId;
					}
				}
			}
		}

		public bool IsPackItem(int gender, int typeId, int itemId)
		{
			return _packItemLookup.ContainsKey((gender, typeId, itemId));
		}

		public List<OfferModel> GetActiveBundles()
		{
			List<OfferModel> list = new List<OfferModel>();
			OfferModel[] offers = _offerSystem.Offers;
			if (offers == null)
			{
				return list;
			}
			foreach (OfferModel offerModel in offers)
			{
				if (offerModel?.Payload != null && offerModel.IsValid && offerModel.Payload.IsBundle)
				{
					list.Add(offerModel);
				}
			}
			return list;
		}

		public OfferModel GetBundle(int offerId)
		{
			OfferModel[] offers = _offerSystem.Offers;
			if (offers == null)
			{
				return null;
			}
			foreach (OfferModel offerModel in offers)
			{
				if (offerModel?.Payload != null && offerModel.ID == offerId && offerModel.IsValid && offerModel.Payload.IsBundle)
				{
					return offerModel;
				}
			}
			return null;
		}

		public List<SuitPackItem> GetSuitItems(OfferModel offer, int gender)
		{
			List<SuitPackItem> list = new List<SuitPackItem>();
			RewardItemData[] array = offer?.Payload?.RewardItems;
			if (array == null)
			{
				return list;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Type != RewardType.SuitPack || !_packs.TryGetPackItems(array[i].ID, out var items) || items == null)
				{
					continue;
				}
				for (int j = 0; j < items.Count; j++)
				{
					if (items[j].BodyType == gender)
					{
						list.Add(items[j]);
					}
				}
			}
			return list;
		}

		public List<PackClothItem> GetAllPackItems(int gender)
		{
			List<PackClothItem> list = new List<PackClothItem>();
			foreach (int allPackId in _packs.GetAllPackIds())
			{
				if (!_packs.TryGetPackItems(allPackId, out var items) || items == null)
				{
					continue;
				}
				for (int i = 0; i < items.Count; i++)
				{
					if (items[i].BodyType == gender)
					{
						list.Add(new PackClothItem(allPackId, items[i].TypeId, items[i].ItemId));
					}
				}
			}
			return list;
		}

		public bool TryGetPackIdForItem(int gender, int typeId, int itemId, out int packId)
		{
			if (_packItemLookup.TryGetValue((gender, typeId, itemId), out packId))
			{
				return true;
			}
			packId = -1;
			return false;
		}

		public bool TryGetActiveOfferForIkBundle(int bundleId, out OfferModel offer)
		{
			foreach (OfferModel activeBundle in GetActiveBundles())
			{
				RewardItemData[] rewardItems = activeBundle.Payload.RewardItems;
				if (rewardItems == null)
				{
					continue;
				}
				for (int i = 0; i < rewardItems.Length; i++)
				{
					if (rewardItems[i].Type == RewardType.Poses && rewardItems[i].ID == bundleId)
					{
						offer = activeBundle;
						return true;
					}
				}
			}
			offer = null;
			return false;
		}

		public bool TryGetActiveOfferForAnimationBundle(int bundleId, out OfferModel offer)
		{
			foreach (OfferModel activeBundle in GetActiveBundles())
			{
				RewardItemData[] rewardItems = activeBundle.Payload.RewardItems;
				if (rewardItems == null)
				{
					continue;
				}
				for (int i = 0; i < rewardItems.Length; i++)
				{
					if (rewardItems[i].Type == RewardType.Animations && rewardItems[i].ID == bundleId)
					{
						offer = activeBundle;
						return true;
					}
				}
			}
			offer = null;
			return false;
		}

		public bool TryGetActiveOfferForPack(int packId, out OfferModel offer)
		{
			foreach (OfferModel activeBundle in GetActiveBundles())
			{
				RewardItemData[] rewardItems = activeBundle.Payload.RewardItems;
				if (rewardItems == null)
				{
					continue;
				}
				for (int i = 0; i < rewardItems.Length; i++)
				{
					if (rewardItems[i].Type == RewardType.SuitPack && rewardItems[i].ID == packId)
					{
						offer = activeBundle;
						return true;
					}
				}
			}
			offer = null;
			return false;
		}

		public bool HasSuitsForGender(OfferModel offer, int gender)
		{
			return GetSuitItems(offer, gender).Count > 0;
		}

		public bool HasHeadwearForGender(OfferModel offer, int gender)
		{
			RewardItemData[] array = offer?.Payload?.RewardItems;
			if (array == null)
			{
				return false;
			}
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Type != RewardType.SuitPack || !_packs.TryGetPackItems(array[i].ID, out var items) || items == null)
				{
					continue;
				}
				for (int j = 0; j < items.Count; j++)
				{
					if (items[j].TypeId == 3 && items[j].BodyType == gender)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool HasHeadwear(OfferModel offer)
		{
			if (!HasHeadwearForGender(offer, 0))
			{
				return HasHeadwearForGender(offer, 1);
			}
			return true;
		}

		public bool TryGetFirstPackIdForGender(OfferModel offer, int gender, out int packId)
		{
			RewardItemData[] array = offer?.Payload?.RewardItems;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Type != RewardType.SuitPack || !_packs.TryGetPackItems(array[i].ID, out var items) || items == null)
					{
						continue;
					}
					for (int j = 0; j < items.Count; j++)
					{
						if (items[j].BodyType == gender)
						{
							packId = array[i].ID;
							return true;
						}
					}
				}
			}
			packId = -1;
			return false;
		}

		public bool TryGetFirstPackForIcon(OfferModel offer, PackIconKind kind, out int packId, out Sprite icon)
		{
			RewardItemData[] array = offer?.Payload?.RewardItems;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Type == RewardType.SuitPack && _packs.TryGetPackIcon(array[i].ID, kind, out icon))
					{
						packId = array[i].ID;
						return true;
					}
				}
			}
			packId = -1;
			icon = null;
			return false;
		}

		public bool TryGetPackDescription(int packId, PackIconKind kind, out string description)
		{
			return _packs.TryGetPackDescription(packId, kind, out description);
		}

		public bool TryGetPackTitle(int packId, out string title)
		{
			return _packs.TryGetPackTitle(packId, out title);
		}

		public bool TryGetPackDescription(int packId, out string description)
		{
			return _packs.TryGetPackDescription(packId, out description);
		}

		public bool IsShadowEnabled(int packId)
		{
			return _packs.IsShadowEnabled(packId);
		}

		public bool TryGetFirstPackId(OfferModel offer, out int packId)
		{
			RewardItemData[] array = offer?.Payload?.RewardItems;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Type == RewardType.SuitPack)
					{
						packId = array[i].ID;
						return true;
					}
				}
			}
			packId = -1;
			return false;
		}

		public bool TryGetFirstPackIdWithHeadwear(OfferModel offer, int gender, out int packId)
		{
			RewardItemData[] array = offer?.Payload?.RewardItems;
			if (array != null)
			{
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].Type != RewardType.SuitPack || !_packs.TryGetPackItems(array[i].ID, out var items) || items == null)
					{
						continue;
					}
					for (int j = 0; j < items.Count; j++)
					{
						if (items[j].TypeId == 3 && items[j].BodyType == gender)
						{
							packId = array[i].ID;
							return true;
						}
					}
				}
			}
			packId = -1;
			return false;
		}
	}
}
