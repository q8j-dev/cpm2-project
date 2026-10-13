using System.Collections.Generic;
using Steamworks;
using Steamworks.Data;
using _Car_Parking.Scripts.Steam.Base;

namespace _Car_Parking.Scripts.Steam.Achievements
{
	public class SteamAchievementService : ISteamAchievementService
	{
		private bool _cleanSlateParking;

		private bool _cleanSlateDrag;

		private bool _cleanSlateCircuit;

		private bool _cleanSlateRally;

		private readonly HashSet<int> _p2pBoughtCarIds = new HashSet<int>();

		public void Unlock(string achievementApiName)
		{
			if (SteamManager.IsInitialized() && !string.IsNullOrEmpty(achievementApiName))
			{
				Achievement achievement = new Achievement(achievementApiName);
				if (!achievement.State)
				{
					achievement.Trigger();
				}
			}
		}

		public void SetStat(string statApiName, int value)
		{
			if (SteamManager.IsInitialized() && !string.IsNullOrEmpty(statApiName))
			{
				SteamUserStats.SetStat(statApiName, value);
			}
		}

		public void SetStat(string statApiName, float value)
		{
			if (SteamManager.IsInitialized() && !string.IsNullOrEmpty(statApiName))
			{
				SteamUserStats.SetStat(statApiName, value);
			}
		}

		public int GetStatInt(string statApiName)
		{
			if (!SteamManager.IsInitialized())
			{
				return 0;
			}
			if (string.IsNullOrEmpty(statApiName))
			{
				return 0;
			}
			return SteamUserStats.GetStatInt(statApiName);
		}

		public float GetStatFloat(string statApiName)
		{
			if (!SteamManager.IsInitialized())
			{
				return 0f;
			}
			if (string.IsNullOrEmpty(statApiName))
			{
				return 0f;
			}
			return SteamUserStats.GetStatFloat(statApiName);
		}

		public void ReportProgress(string achievementApiName, string statApiName, int target, int current)
		{
			if (SteamManager.IsInitialized())
			{
				SetStat(statApiName, current);
				if (current >= target)
				{
					Unlock(achievementApiName);
				}
			}
		}

		public void ReportProgress(string achievementApiName, string statApiName, float target, float current)
		{
			if (SteamManager.IsInitialized())
			{
				SetStat(statApiName, current);
				if (current >= target)
				{
					Unlock(achievementApiName);
				}
			}
		}

		public IEnumerable<Achievement> GetAchievements()
		{
			if (!SteamManager.IsInitialized())
			{
				return null;
			}
			return SteamUserStats.Achievements;
		}

		public void MarkCleanSlateParking()
		{
			_cleanSlateParking = true;
			CheckCleanSlate();
		}

		public void MarkCleanSlateDrag()
		{
			_cleanSlateDrag = true;
			CheckCleanSlate();
		}

		public void MarkCleanSlateCircuit()
		{
			_cleanSlateCircuit = true;
			CheckCleanSlate();
		}

		public void MarkCleanSlateRally()
		{
			_cleanSlateRally = true;
			CheckCleanSlate();
		}

		private void CheckCleanSlate()
		{
			if (_cleanSlateParking && _cleanSlateDrag && _cleanSlateCircuit && _cleanSlateRally)
			{
				Unlock("ACH_CLEAN_SLATE");
			}
		}

		public void MarkP2pCarBought(int carId)
		{
			_p2pBoughtCarIds.Add(carId);
		}

		public void TryUnlockDealerLuck(int carId)
		{
			if (_p2pBoughtCarIds.Contains(carId))
			{
				Unlock("ACH_DEALER_LUCK");
			}
		}
	}
}
