using Cysharp.Threading.Tasks;
using Steamworks;

namespace _Car_Parking.Scripts.Steam
{
	public interface ISteamInventoryService
	{
		UniTask<InventoryResult?> Refresh();

		UniTask DeleteAllInventoryItems();

		InventoryDef GetInventoryDef(int itemDefId);
	}
}
