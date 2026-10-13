using System;
using Cysharp.Threading.Tasks;

namespace CPM.Ads.Policy
{
	public interface IAdOfferService
	{
		event Action Changed;

		OfferState CanOffer(AdPlacement placement, string contextId);

		UniTask<OfferResult> TryShowAsync(AdPlacement placement, string contextId);
	}
}
