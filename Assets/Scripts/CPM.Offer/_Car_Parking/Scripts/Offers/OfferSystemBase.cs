using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Offers
{
	public abstract class OfferSystemBase : IOfferSystem, IInitializable, IDisposable
	{
		private OfferToBuyCache _offerToBuyCache;

		private bool _isDataReady;

		private OfferBigBannerShowController _offerBigBannerShowController;

		public abstract OfferModel[] Offers { get; }

		public virtual bool IsDataReady
		{
			get
			{
				return _isDataReady;
			}
			protected set
			{
				_isDataReady = value;
			}
		}

		public bool HaveValidOffers
		{
			get
			{
				if (Offers == null)
				{
					return false;
				}
				for (int i = 0; i < Offers.Length; i++)
				{
					if (Offers[i].IsValid)
					{
						return true;
					}
				}
				return false;
			}
		}

		public event Action OnDataUpdatedEvent;

		protected OfferSystemBase()
		{
			_offerBigBannerShowController = new OfferBigBannerShowController();
		}

		public virtual void Initialize()
		{
			_offerToBuyCache = new OfferToBuyCache();
		}

		public virtual void Dispose()
		{
		}

		public abstract UniTask<bool> SyncOffers();

		public OfferModel GetOfferWithBiggestPriority()
		{
			OfferModel offerModel = null;
			if (Offers != null)
			{
				for (int i = 0; i < Offers.Length; i++)
				{
					if (offerModel == null || (Offers[i].Payload.Priority > offerModel.Payload.Priority && Offers[i].IsValid))
					{
						offerModel = Offers[i];
					}
				}
			}
			return offerModel;
		}

		public List<OfferModel> GetOffersForBanners()
		{
			List<OfferModel> list = new List<OfferModel>();
			List<OfferModel> list2 = new List<OfferModel>();
			if (Offers != null)
			{
				for (int i = 0; i < Offers.Length; i++)
				{
					list2.Add(Offers[i]);
				}
			}
			while (list2.Count > 0)
			{
				OfferModel offerModel = null;
				for (int j = 0; j < list2.Count; j++)
				{
					if (offerModel == null || list2[j].Payload.Priority > offerModel.Payload.Priority)
					{
						offerModel = list2[j];
					}
				}
				bool flag = true;
				if (offerModel.Skin.ID > 0)
				{
					foreach (OfferModel item in list)
					{
						if (offerModel.Skin.ID == item.Skin.ID)
						{
							flag = false;
							break;
						}
					}
				}
				if ((offerModel.IsValid & flag) && !offerModel.IsHiddenFromBannerList)
				{
					list.Add(offerModel);
				}
				list2.Remove(offerModel);
			}
			return list;
		}

		public OfferModel GetOfferForBigBanner()
		{
			if (!IsDataReady)
			{
				return null;
			}
			return _offerBigBannerShowController.GetOfferForBigBanner(Offers);
		}

		public void MarkShownBigBanner(int offerID)
		{
			_offerBigBannerShowController.MarkShown(offerID);
			OfferModel offer = GetOffer(offerID);
			if (offer == null || string.IsNullOrEmpty(offer.Skin.BannerPrefab) || Offers == null)
			{
				return;
			}
			for (int i = 0; i < Offers.Length; i++)
			{
				if (Offers[i].Skin.BannerPrefab == offer.Skin.BannerPrefab)
				{
					_offerBigBannerShowController.MarkShown(Offers[i].ID);
				}
			}
		}

		public void SetCurrentOfferID(int offerID)
		{
			OfferModel offer = GetOffer(offerID);
			if (offer != null)
			{
				List<int> list = new List<int>();
				RewardItemData[] rewardItems = offer.Payload.RewardItems;
				foreach (RewardItemData rewardItemData in rewardItems)
				{
					list.Add(rewardItemData.ID);
				}
				OfferToBuyCache.OfferCacheData offerCacheData = new OfferToBuyCache.OfferCacheData
				{
					Items = list.ToArray()
				};
				_offerToBuyCache.Set(offer.Payload.FirstBundleID, offerID, offerCacheData);
			}
		}

		public OfferModel GetOffer(int id)
		{
			if (Offers == null || Offers.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < Offers.Length; i++)
			{
				if (Offers[i].ID == id)
				{
					return Offers[i];
				}
			}
			return null;
		}

		public bool TryGetOfferFromCache(string bundleID, out int offerID)
		{
			return _offerToBuyCache.TryGetOffer(bundleID, out offerID);
		}

		public bool TryGetOfferCacheFromCache(string bundleID, out OfferToBuyCache.OfferCacheData cacheData)
		{
			return _offerToBuyCache.TryGetOfferData(bundleID, out cacheData);
		}

		public void RemoveOfferFromCache(string bundleID)
		{
			_offerToBuyCache.Remove(bundleID);
		}

		public OfferModel GetOffer(string productID)
		{
			if (Offers == null || Offers.Length == 0)
			{
				return null;
			}
			for (int i = 0; i < Offers.Length; i++)
			{
				if (Offers[i].Payload.FirstBundleID == productID)
				{
					return Offers[i];
				}
			}
			return null;
		}

		public List<OfferModel> GetOffersWithCarId(int carId)
		{
			List<OfferModel> list = new List<OfferModel>();
			if (Offers == null)
			{
				return list;
			}
			for (int i = 0; i < Offers.Length; i++)
			{
				OfferModel offerModel = Offers[i];
				if (offerModel.Payload != null && offerModel.IsValid && !offerModel.Payload.IsBundle && offerModel.Payload.HaveRewardCarId(carId))
				{
					list.Add(offerModel);
				}
			}
			return list;
		}

		public List<OfferModel> GetOffersWithCarIdAndHaveSuit(int carId)
		{
			List<OfferModel> list = new List<OfferModel>();
			if (Offers == null)
			{
				return list;
			}
			for (int i = 0; i < Offers.Length; i++)
			{
				OfferModel offerModel = Offers[i];
				if (offerModel.Payload != null && offerModel.IsValid && offerModel.Payload.HaveRewardCarIdAndHaveSuit(carId))
				{
					list.Add(offerModel);
				}
			}
			return list;
		}

		protected virtual void OnDataUpdated()
		{
			int num = UnityEngine.Random.Range(0, 9999);
			if (Offers != null)
			{
				for (int i = 0; i < Offers.Length; i++)
				{
					Offers[i].Init(num + i);
				}
			}
			_isDataReady = true;
		}

		protected void OnDataUpdatedSendEvent()
		{
			OnDataUpdatedEvent?.Invoke();
		}
	}
}
