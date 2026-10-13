using System.Collections.Generic;

namespace _Car_Parking.Scripts.Steam.Base
{
	public interface ISteamItemCatalog
	{
		IReadOnlyList<SteamItem> AllItems { get; }

		SteamItem GetSteamItemByDefId(int defId);
	}
}
