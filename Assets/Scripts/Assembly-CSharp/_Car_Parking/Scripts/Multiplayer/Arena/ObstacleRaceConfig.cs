using System;
using System.Collections.Generic;
using CPM.RemoteConfig;
using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	[CreateAssetMenu(fileName = "ObstacleRaceConfig", menuName = "Configs/Arena/ObstacleRaceConfig")]
	public sealed class ObstacleRaceConfig : ArenaConfig
	{
		[Serializable]
		public class ZonePartData
		{
			[Tooltip("Play this block exactly as written instead of shuffling it. Use it where the order rules leave only one legal layout anyway.")]
			public bool KeepOrder;

			public ObstacleRaceZoneDifficulty[] Block;
		}

		[SerializeField]
		private CombinedPrice[] Rewards = default;

		[Header("Track")]
		[Tooltip("How many random zones sit between the start and the finish.")]
		public ZonePartData[] ZoneParts;

		[Header("Survival")]
		[Tooltip("Seconds from the first arm-trigger touch until the course and collapse objects vanish.")]
		public float CollapseTimer = 15f;

		[Tooltip("Seconds left once every remaining player has touched the arm trigger. Only shortens the timer.")]
		public float AllTouchedCollapseTimer = 3f;

		public FallingZoneConfig FallingZone;

		public override int MaxPlayers
		{
			get
			{
				if (RemoteConfigSnapshot.MaxPlayers <= 0)
				{
					return base.MaxPlayers;
				}
				return Mathf.Clamp(RemoteConfigSnapshot.MaxPlayers, MinPlayers, 32);
			}
		}

		public int RewardsCount => Rewards.Length;

		public ObstacleRaceBlock[] GetBlocks()
		{
			if (ZoneParts == null)
			{
				return Array.Empty<ObstacleRaceBlock>();
			}
			ObstacleRaceBlock[] array = new ObstacleRaceBlock[ZoneParts.Length];
			for (int i = 0; i < ZoneParts.Length; i++)
			{
				ZonePartData zonePartData = ZoneParts[i];
				array[i] = new ObstacleRaceBlock(zonePartData?.Block ?? Array.Empty<ObstacleRaceZoneDifficulty>(), zonePartData?.KeepOrder ?? false);
			}
			return array;
		}

		public CombinedPrice GetReward(int index)
		{
			if (index < 0 || index >= RewardsCount)
			{
				return null;
			}
			return Rewards[index];
		}

		public IReadOnlyCollection<CombinedPrice> GetRewards()
		{
			return Rewards;
		}
	}
}
