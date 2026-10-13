using Cysharp.Threading.Tasks;

namespace _Car_Parking.Scripts.PurchaseSystem
{
	public interface IPurchaseActionsBeforeToBuyProduct
	{
		UniTask<bool> ActionsBeforeToBuyProductAsync(string productID);
	}
}
