using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.WindowSystem
{
	public static class HudSlotRegistry
	{
		private static readonly Dictionary<HudSlot, List<RectTransform>> Anchors = new Dictionary<HudSlot, List<RectTransform>>();

		public static event Action<HudSlot> OnChanged;

		public static void Register(HudSlot slot, RectTransform anchor)
		{
			if ((bool)anchor)
			{
				if (!Anchors.TryGetValue(slot, out var value))
				{
					value = (Anchors[slot] = new List<RectTransform>());
				}
				if (!value.Contains(anchor))
				{
					value.Add(anchor);
					OnChanged?.Invoke(slot);
				}
			}
		}

		public static void Unregister(HudSlot slot, RectTransform anchor)
		{
			if (Anchors.TryGetValue(slot, out var value) && value.Remove(anchor))
			{
				OnChanged?.Invoke(slot);
			}
		}

		public static bool TryGet(HudSlot slot, out RectTransform anchor)
		{
			anchor = null;
			if (!Anchors.TryGetValue(slot, out var value))
			{
				return false;
			}
			int num = value.Count - 1;
			while (num >= 0)
			{
				if (!value[num])
				{
					value.RemoveAt(num);
					num--;
					continue;
				}
				anchor = value[num];
				return true;
			}
			return false;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		private static void ResetState()
		{
			Anchors.Clear();
			OnChanged = null;
		}
	}
}
