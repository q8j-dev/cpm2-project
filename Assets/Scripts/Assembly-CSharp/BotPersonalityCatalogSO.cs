using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BotPersonalityCatalog", menuName = "Scriptable/Racing/Bot Personality Catalog", order = 8)]
public class BotPersonalityCatalogSO : ScriptableObject
{
	[Serializable]
	public struct ClassEntry
	{
		[Tooltip("Race class id as returned by CircuitRaceControlller.CurrentClassId.")]
		public int ClassId;

		public BotPersonalitySO Personality;
	}

	[Serializable]
	public struct SlotEntry
	{
		[Tooltip("Starting-grid slot index (spawn index passed to CreateTestRacerCar).")]
		public int SlotIndex;

		public BotPersonalitySO Personality;
	}

	[Tooltip("Assigned to the bot spawned next to the human player (dynamic rival). Empty → that bot falls through to slot/class/default like any other.")]
	public BotPersonalitySO RivalPersonality;

	[Tooltip("Per-grid-slot overrides (priority after Rival). First matching SlotIndex wins.")]
	public List<SlotEntry> PerGridSlot = new List<SlotEntry>();

	[Tooltip("Per-class overrides. First matching ClassId wins.")]
	public List<ClassEntry> PerClass = new List<ClassEntry>();

	[Tooltip("Used when no Rival/slot/class entry matches. Empty → BotPersonality.Neutral.")]
	public BotPersonalitySO DefaultPersonality;

	public void Shuffle()
	{
		if (PerGridSlot == null || PerGridSlot.Count < 2)
		{
			return;
		}
		for (int num = PerGridSlot.Count - 1; num > 0; num--)
		{
			int num2 = UnityEngine.Random.Range(0, num + 1);
			if (num2 != num)
			{
				SlotEntry value = PerGridSlot[num];
				SlotEntry value2 = PerGridSlot[num2];
				ref BotPersonalitySO personality = ref value.Personality;
				ref BotPersonalitySO personality2 = ref value2.Personality;
				BotPersonalitySO personality3 = value2.Personality;
				BotPersonalitySO personality4 = value.Personality;
				personality = personality3;
				personality2 = personality4;
				PerGridSlot[num] = value;
				PerGridSlot[num2] = value2;
			}
		}
	}

	public BotPersonality Resolve(int slotIndex, int classId, bool isRivalSlot)
	{
		if (isRivalSlot && RivalPersonality != null)
		{
			return RivalPersonality.Resolve();
		}
		if (PerGridSlot != null)
		{
			for (int i = 0; i < PerGridSlot.Count; i++)
			{
				if (PerGridSlot[i].SlotIndex == slotIndex && PerGridSlot[i].Personality != null)
				{
					return PerGridSlot[i].Personality.Resolve();
				}
			}
		}
		if (PerClass != null)
		{
			for (int j = 0; j < PerClass.Count; j++)
			{
				if (PerClass[j].ClassId == classId && PerClass[j].Personality != null)
				{
					return PerClass[j].Personality.Resolve();
				}
			}
		}
		if (!(DefaultPersonality != null))
		{
			return BotPersonality.Neutral;
		}
		return DefaultPersonality.Resolve();
	}
}
