using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class DeviceRowSlots
	{
		public const int Unpinned = -1;

		private const string ModifierPrefix = "modifier";

		public static void Collect(InputAction action, RebindDeviceProfile profile, List<(string part, int slot)> results)
		{
			results.Clear();
			if (action == null || profile == null)
			{
				return;
			}
			bool flag = ChordSlots.TryGetPair(action, profile, out var pair);
			if (flag)
			{
				results.Add((pair.RowPart, pair.DisplayedIndex));
			}
			for (int i = 0; i < action.bindings.Count; i++)
			{
				InputBinding inputBinding = action.bindings[i];
				if (inputBinding.isComposite || (flag && pair.Contains(i)))
				{
					continue;
				}
				string text = (inputBinding.isPartOfComposite ? (inputBinding.name ?? string.Empty) : string.Empty);
				if (inputBinding.isPartOfComposite && (text.Length == 0 || text.StartsWith("modifier", StringComparison.OrdinalIgnoreCase)))
				{
					continue;
				}
				string effectivePath = inputBinding.effectivePath;
				int num;
				int num2;
				if (!string.IsNullOrEmpty(effectivePath))
				{
					num = (profile.Owns(effectivePath) ? 1 : 0);
					if (num != 0)
					{
						num2 = (ModifierComposites.HasUnboundModifier(action, i, profile) ? 1 : 0);
						goto IL_00db;
					}
				}
				else
				{
					num = 0;
				}
				num2 = 0;
				goto IL_00db;
				IL_00db:
				bool flag2 = (byte)num2 != 0;
				bool flag3 = num != 0 && !flag2;
				bool flag4 = flag2 || (string.IsNullOrEmpty(effectivePath) && profile.Owns(inputBinding.path));
				if (!flag3 && !flag4)
				{
					continue;
				}
				int num3 = IndexOfPart(results, text);
				if (num3 < 0)
				{
					(string, int) item = (text, flag3 ? (-1) : i);
					if (text.Length == 0)
					{
						results.Insert(0, item);
					}
					else
					{
						results.Add(item);
					}
				}
				else if (flag3)
				{
					results[num3] = (text, -1);
				}
			}
		}

		private static int IndexOfPart(List<(string part, int slot)> rows, string part)
		{
			for (int i = 0; i < rows.Count; i++)
			{
				if (string.Equals(rows[i].part, part, StringComparison.OrdinalIgnoreCase))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
