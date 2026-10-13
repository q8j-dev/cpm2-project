using CPM.Cdn;
using UnityEngine;

namespace CPM.Balance
{
	public static class BalanceBootSeeder
	{
		public enum Source
		{
			Cache = 0,
			Bundled = 1,
			Empty = 2
		}

		public static Source Seed(EtagFileCache cache, string cacheKey, byte[] bundledBytes, BalanceDocumentStore store)
		{
			if (cache != null && cache.TryRead(cacheKey, out var bytes, out var _))
			{
				BalanceDocument document = BalanceDocumentService.Parse(bytes);
				if (CarPriceProvider.TryInit(document))
				{
					store.Set(document);
					return Source.Cache;
				}
				cache.Invalidate();
			}
			BalanceDocument document2 = BalanceDocumentService.Parse(bundledBytes);
			if (CarPriceProvider.TryInit(document2))
			{
				store.Set(document2);
				return Source.Bundled;
			}
			CarPriceProvider.Init(new BalanceDocument());
			store.Set(new BalanceDocument());
			Debug.LogError("BalanceBootSeeder: balance document indexed 0 cars (cache and bundled both unusable). Check Assets/link.xml preserve (CPM.Balance / CPM.Economy.Primitives) and the JSON schema.");
			return Source.Empty;
		}
	}
}
