using System;
using System.Collections.Generic;
using Persistence.Systems;

namespace _Car_Parking.Scripts.Offers
{
	public class OfferToBuyCache
	{
		public struct OfferCacheData
		{
			public int[] Items;
		}

		private const string CachePath = "Offers/Cache03";

		private const string CacheDataPath = "Offers/CacheData03";

		private readonly Dictionary<string, int> _offers = new Dictionary<string, int>();

		private readonly Dictionary<string, OfferCacheData> _offerCacheDatas = new Dictionary<string, OfferCacheData>();

		public OfferToBuyCache()
		{
			LoadData();
		}

		public bool TryGetOffer(string bundleID, out int offerID)
		{
			return _offers.TryGetValue(bundleID, out offerID);
		}

		public bool TryGetOfferData(string bundleID, out OfferCacheData offerCacheData)
		{
			return _offerCacheDatas.TryGetValue(bundleID, out offerCacheData);
		}

		public void Set(string bundleID, int offerID, OfferCacheData offerCacheData)
		{
			_offers[bundleID] = offerID;
			_offerCacheDatas[bundleID] = offerCacheData;
			EasySaveDataHandler.SaveValue(offerID, "Offers/Cache03", bundleID);
			EasySaveDataHandler.SaveValue(offerCacheData, "Offers/CacheData03", bundleID);
		}

		public void Remove(string bundleID)
		{
			if (_offers.Remove(bundleID))
			{
				EasySaveDataHandler.DeleteValue("Offers/Cache03", bundleID);
			}
			if (_offerCacheDatas.Remove(bundleID))
			{
				EasySaveDataHandler.DeleteValue("Offers/CacheData03", bundleID);
			}
		}

		private void LoadData()
		{
			try
			{
				if (!EasySaveDataHandler.TryGetFile("Offers/Cache03", out var file))
				{
					return;
				}
				string[] keys = file.GetKeys();
				foreach (string key in keys)
				{
					_offers.Add(key, file.Load<int>(key));
				}
				if (EasySaveDataHandler.TryGetFile("Offers/CacheData03", out file))
				{
					keys = file.GetKeys();
					foreach (string key2 in keys)
					{
						_offerCacheDatas.Add(key2, file.Load<OfferCacheData>(key2));
					}
				}
			}
			catch (Exception)
			{
			}
		}
	}
}
