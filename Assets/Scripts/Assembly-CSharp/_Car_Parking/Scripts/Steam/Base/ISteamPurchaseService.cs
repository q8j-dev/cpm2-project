using System;
using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Steam.Base
{
	public interface ISteamPurchaseService
	{
		bool IsPurchasing { get; }

		UniTask StartPurchase(int itemDefId, Func<SteamPurchaseData, UniTask<bool>> onApply);
	}
}
