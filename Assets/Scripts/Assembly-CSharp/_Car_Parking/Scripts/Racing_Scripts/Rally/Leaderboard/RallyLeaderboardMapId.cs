using CPM.RacingRecords;
using _Car_Parking.Scripts.Backend;

namespace _Car_Parking.Scripts.Racing_Scripts.Rally.Leaderboard
{
	public static class RallyLeaderboardMapId
	{
		public static bool TryBuild(RallyController controller, out string mapId)
		{
			mapId = null;
			RallyMapDataSO rallyMapDataSO = (controller ? controller.RallyMapDataSO : null);
			if (!rallyMapDataSO || rallyMapDataSO.RallyLocationDatas == null)
			{
				return false;
			}
			int locationIndex = RallyController.LocationIndex;
			if (locationIndex < 0 || locationIndex >= rallyMapDataSO.RallyLocationDatas.Count)
			{
				return false;
			}
			Result<string> result = RacingRecordsMapId.Build(rallyMapDataSO.RallyLocationDatas[locationIndex].AnalyticsId, RallyController.TrajectoryIndex);
			if (result.IsFail)
			{
				return false;
			}
			mapId = result.Value;
			return true;
		}

		public static string LocalKey()
		{
			return $"{RallyController.LocationIndex}_{RallyController.TrajectoryIndex}";
		}
	}
}
