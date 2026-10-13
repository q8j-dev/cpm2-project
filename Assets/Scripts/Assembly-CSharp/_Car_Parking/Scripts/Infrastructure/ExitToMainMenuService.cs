using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Multiplayer.Arena;
using _Car_Parking.Scripts.Trade;

namespace _Car_Parking.Scripts.Infrastructure
{
	public sealed class ExitToMainMenuService
	{
		private readonly CommonProvider _commonProvider;

		private readonly ArenaFlowController _arenaFlow;

		private readonly OpenWorldAnalytic _openWorldAnalytic;

		public ExitToMainMenuService(CommonProvider commonProvider, ArenaFlowController arenaFlow, OpenWorldAnalytic openWorldAnalytic)
		{
			_commonProvider = commonProvider;
			_arenaFlow = arenaFlow;
			_openWorldAnalytic = openWorldAnalytic;
		}

		public void Exit(MenuLevelParent menu, string exitReason)
		{
			_commonProvider.TradeService?.CancelOrDeclineAllOffers(TradeOfferCancelReason.PlayerOffline, notifyOtherPlayer: false);
			_arenaFlow?.LeaveArena(ArenaLeaveReason.UserLeft, null, !menu);
			FlagsStorage.IsPrivateRoom = false;
			if ((bool)menu)
			{
				menu.OpenMainMenu();
			}
			_openWorldAnalytic.FinishEvent(exitReason);
		}
	}
}
