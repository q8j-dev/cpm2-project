using VContainer.Unity;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Steam
{
	public class SteamDebugBootstrap : IInitializable
	{
		private readonly ISteamPurchaseService _steamPurchaseService;

		private readonly ISteamInventoryService _steamInventoryService;

		public SteamDebugBootstrap(ISteamPurchaseService steamPurchaseService, ISteamInventoryService steamInventoryService)
		{
			_steamPurchaseService = steamPurchaseService;
			_steamInventoryService = steamInventoryService;
		}

		public void Initialize()
		{
			SteamDebug.Init(_steamPurchaseService, _steamInventoryService);
		}
	}
}
