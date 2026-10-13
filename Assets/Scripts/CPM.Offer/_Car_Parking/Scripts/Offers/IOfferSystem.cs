using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Offers
{
	public interface IOfferSystem
	{
		bool IsDataReady { get; }

		bool HaveValidOffers { get; }

		OfferModel[] Offers { get; }

		event Action OnDataUpdatedEvent;

		UniTask<bool> SyncOffers();

		OfferModel GetOffer(int id);

		OfferModel GetOffer(string productID);

		List<OfferModel> GetOffersWithCarId(int carId);

		List<OfferModel> GetOffersWithCarIdAndHaveSuit(int carId);

		void SetCurrentOfferID(int offerID);

		OfferModel GetOfferWithBiggestPriority();

		List<OfferModel> GetOffersForBanners();

		OfferModel GetOfferForBigBanner();

		void MarkShownBigBanner(int offerID);

		bool TryGetOfferFromCache(string bundleID, out int offerID);

		bool TryGetOfferCacheFromCache(string bundleID, out OfferToBuyCache.OfferCacheData cacheData);

		void RemoveOfferFromCache(string bundleID);
	}
}
