using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.GooglePlayServices
{
	public class NullAchievementsService : IPlayGamesAchievementsService
	{
		public string GetUserId()
		{
			return string.Empty;
		}

		public void Unlock(string achievementId, Action<UnlockResult> onResult)
		{
			onResult(UnlockResult.Succeeded());
		}

		public void ReportProgress(string achievementId, int steps, Action<UnlockResult> onResult)
		{
			onResult(UnlockResult.Succeeded());
		}

		public void LoadState(Action<IReadOnlyList<AchievementServerState>> onLoaded)
		{
			onLoaded(Array.Empty<AchievementServerState>());
		}
	}
}
