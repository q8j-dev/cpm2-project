using System.Collections.Generic;
using Steamworks.Data;

namespace _Car_Parking.Scripts.Steam.Base
{
	public interface ISteamAchievementService
	{
		void Unlock(string achievementApiName);

		void SetStat(string statApiName, int value);

		void SetStat(string statApiName, float value);

		int GetStatInt(string statApiName);

		float GetStatFloat(string statApiName);

		void MarkCleanSlateParking();

		void MarkCleanSlateDrag();

		void MarkCleanSlateCircuit();

		void MarkCleanSlateRally();

		void MarkP2pCarBought(int carId);

		void TryUnlockDealerLuck(int carId);

		void ReportProgress(string achievementApiName, string statApiName, int target, int current);

		void ReportProgress(string achievementApiName, string statApiName, float target, float current);

		IEnumerable<Achievement> GetAchievements();
	}
}
