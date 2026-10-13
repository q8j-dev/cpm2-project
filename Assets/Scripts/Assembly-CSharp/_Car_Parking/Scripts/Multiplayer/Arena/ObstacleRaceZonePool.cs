using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[CreateAssetMenu(fileName = "ObstacleRaceZonePool", menuName = "Configs/Arena/ObstacleRaceZonePool")]
	public sealed class ObstacleRaceZonePool : ScriptableObject
	{
		public ObstacleRaceZone[] Zones;

		public int Count
		{
			get
			{
				ObstacleRaceZone[] zones = Zones;
				if (zones == null)
				{
					return 0;
				}
				return zones.Length;
			}
		}

		public ObstacleRaceZone Get(int index)
		{
			if (index < 0 || index >= Count)
			{
				return null;
			}
			return Zones[index];
		}

		public ObstacleRaceZoneInfo[] BuildZoneInfos()
		{
			if (Zones == null)
			{
				return Array.Empty<ObstacleRaceZoneInfo>();
			}
			List<ObstacleRaceZoneInfo> list = new List<ObstacleRaceZoneInfo>(Zones.Length);
			for (int i = 0; i < Zones.Length; i++)
			{
				if ((bool)Zones[i])
				{
					list.Add(new ObstacleRaceZoneInfo(i, Zones[i].Difficulty, Zones[i].Mechanics, Zones[i].Traits));
				}
			}
			return list.ToArray();
		}

		private void OnValidate()
		{
			if (Zones == null)
			{
				return;
			}
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < Zones.Length; i++)
			{
				if ((bool)Zones[i])
				{
					hashSet.Add(Zones[i].AnalyticsId);
				}
			}
		}
	}
}
