using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public interface IPurchaseCheckProductToBuy
	{
		UniTask<PreBuyActionResult> CheckProductToBuyAsync(string productID);
	}
}
