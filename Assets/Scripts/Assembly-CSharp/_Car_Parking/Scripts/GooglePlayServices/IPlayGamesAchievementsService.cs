using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.GooglePlayServices
{
	public interface IPlayGamesAchievementsService
	{
		string GetUserId();

		void Unlock(string achievementId, Action<UnlockResult> onResult);

		void ReportProgress(string achievementId, int steps, Action<UnlockResult> onResult);

		void LoadState(Action<IReadOnlyList<AchievementServerState>> onLoaded);
	}
}
