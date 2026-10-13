using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ObstacleRaceTrackPlan
	{
		public const int MaxPickAttempts = 20;

		public const int MaxShuffleAttempts = 200;

		public static int[] Pick(int seed, IReadOnlyList<ObstacleRaceZoneInfo> pool, IReadOnlyList<ObstacleRaceBlock> blocks, IReadOnlyList<IObstacleRaceZoneRule> rules = null)
		{
			if (pool == null || pool.Count == 0)
			{
				return Array.Empty<int>();
			}
			if (rules == null)
			{
				rules = ObstacleRaceZoneRules.Default;
			}
			Random random = new Random(seed);
			int[] array = Array.Empty<int>();
			IObstacleRaceZoneRule blocker = null;
			for (int i = 0; i < 20; i++)
			{
				array = FillSlots(ShuffleWithinBlocks(blocks, random), pool, random, rules, ref blocker);
				if (blocker == null)
				{
					break;
				}
			}
			int num = CountSlots(blocks);
			_ = array.Length;
			return array;
		}

		private static int CountSlots(IReadOnlyList<ObstacleRaceBlock> blocks)
		{
			if (blocks == null)
			{
				return 0;
			}
			int num = 0;
			for (int i = 0; i < blocks.Count; i++)
			{
				num += blocks[i].Count;
			}
			return num;
		}

		private static int[] FillSlots(ObstacleRaceZoneDifficulty[] slotDifficulties, IReadOnlyList<ObstacleRaceZoneInfo> pool, Random random, IReadOnlyList<IObstacleRaceZoneRule> rules, ref IObstacleRaceZoneRule blocker)
		{
			blocker = null;
			List<ObstacleRaceZoneInfo> list = new List<ObstacleRaceZoneInfo>(pool);
			List<ObstacleRaceZoneInfo> list2 = new List<ObstacleRaceZoneInfo>(slotDifficulties.Length);
			for (int i = 0; i < slotDifficulties.Length; i++)
			{
				if (list.Count == 0)
				{
					break;
				}
				List<ObstacleRaceZoneInfo> list3 = Narrow(list, new ObstacleRaceSlotContext(i, slotDifficulties.Length, slotDifficulties[i], list2), rules, ref blocker);
				ObstacleRaceZoneInfo item = ((list3.Count > 0) ? list3[random.Next(list3.Count)] : list[random.Next(list.Count)]);
				list2.Add(item);
				Drop(list, item.Id);
			}
			int[] array = new int[list2.Count];
			for (int j = 0; j < list2.Count; j++)
			{
				array[j] = list2[j].Id;
			}
			return array;
		}

		private static List<ObstacleRaceZoneInfo> Narrow(List<ObstacleRaceZoneInfo> unused, in ObstacleRaceSlotContext slot, IReadOnlyList<IObstacleRaceZoneRule> rules, ref IObstacleRaceZoneRule blocker)
		{
			List<ObstacleRaceZoneInfo> list = new List<ObstacleRaceZoneInfo>(unused);
			for (int i = 0; i < rules.Count; i++)
			{
				if (rules[i] != null)
				{
					bool num = list.Count > 0;
					list = rules[i].Filter(list, in slot) ?? new List<ObstacleRaceZoneInfo>();
					if (num && list.Count == 0 && blocker == null)
					{
						blocker = rules[i];
					}
				}
			}
			return list;
		}

		private static void Drop(List<ObstacleRaceZoneInfo> zones, int id)
		{
			for (int i = 0; i < zones.Count; i++)
			{
				if (zones[i].Id == id)
				{
					zones.RemoveAt(i);
					break;
				}
			}
		}

		public static ObstacleRaceZoneDifficulty[] ShuffleWithinBlocks(IReadOnlyList<ObstacleRaceBlock> blocks, Random random, IReadOnlyList<IObstacleRaceOrderRule> rules = null)
		{
			if (blocks == null || random == null)
			{
				return Array.Empty<ObstacleRaceZoneDifficulty>();
			}
			if (rules == null)
			{
				rules = ObstacleRaceOrderRules.Default;
			}
			ObstacleRaceZoneDifficulty[] array = Array.Empty<ObstacleRaceZoneDifficulty>();
			for (int i = 0; i < 200; i++)
			{
				array = ShuffleOnce(blocks, random);
				if (FindBlocker(array, rules) == null)
				{
					return array;
				}
			}
			return array;
		}

		private static IObstacleRaceOrderRule FindBlocker(IReadOnlyList<ObstacleRaceZoneDifficulty> slots, IReadOnlyList<IObstacleRaceOrderRule> rules)
		{
			for (int i = 0; i < rules.Count; i++)
			{
				if (rules[i] != null && !rules[i].IsAllowed(slots))
				{
					return rules[i];
				}
			}
			return null;
		}

		private static ObstacleRaceZoneDifficulty[] ShuffleOnce(IReadOnlyList<ObstacleRaceBlock> blocks, Random random)
		{
			List<ObstacleRaceZoneDifficulty> list = new List<ObstacleRaceZoneDifficulty>();
			for (int i = 0; i < blocks.Count; i++)
			{
				ObstacleRaceBlock obstacleRaceBlock = blocks[i];
				if (obstacleRaceBlock.Count == 0)
				{
					continue;
				}
				int count = list.Count;
				for (int j = 0; j < obstacleRaceBlock.Count; j++)
				{
					list.Add(obstacleRaceBlock.Slots[j]);
				}
				if (!obstacleRaceBlock.KeepOrder)
				{
					for (int num = list.Count - 1; num > count; num--)
					{
						int num2 = random.Next(count, num + 1);
						List<ObstacleRaceZoneDifficulty> list2 = list;
						int index = num;
						List<ObstacleRaceZoneDifficulty> list3 = list;
						int index2 = num2;
						ObstacleRaceZoneDifficulty obstacleRaceZoneDifficulty = list[num2];
						ObstacleRaceZoneDifficulty obstacleRaceZoneDifficulty2 = list[num];
						ObstacleRaceZoneDifficulty obstacleRaceZoneDifficulty3 = (list2[index] = obstacleRaceZoneDifficulty);
						obstacleRaceZoneDifficulty3 = (list3[index2] = obstacleRaceZoneDifficulty2);
					}
				}
			}
			return list.ToArray();
		}
	}
}
