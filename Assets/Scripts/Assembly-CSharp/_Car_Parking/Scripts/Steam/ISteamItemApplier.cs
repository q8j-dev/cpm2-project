using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.Steam
{
	public interface ISteamItemApplier
	{
		UniTask<bool> Apply(SteamItem item, SteamPurchaseData data);
	}
}
