using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Car_Parking.Scripts.GameInput.Rebinding;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public sealed class LayoutRowDetails
	{
		private readonly ControlsLayoutFilterConfig _config;

		private readonly Color _lockedTint;

		private readonly Dictionary<InputHintElement, (Graphic graphic, Color color)[]> _originals = new Dictionary<InputHintElement, (Graphic, Color)[]>();

		private readonly HashSet<InputHintElement> _tinted = new HashSet<InputHintElement>();

		public LayoutRowDetails(ControlsLayoutFilterConfig config, Color lockedTint)
		{
			_config = config;
			_lockedTint = lockedTint;
		}

		public bool ShowsHoldArrow(InputAction action, string part, int index, ForcedDeviceType page)
		{
			string overrideInteractions = action.bindings[InteractionRules.TriggerIndexOf(action.bindings, index)].overrideInteractions;
			if (overrideInteractions != null)
			{
				return InteractionRules.Classify(InteractionRules.Join(overrideInteractions, action.interactions)) == InteractionClass.Hold;
			}
			if ((bool)_config && _config.TryGetHoldArrow(action, part, page, out var show))
			{
				return show;
			}
			return InteractionRules.ClassOf(action, index) == InteractionClass.Hold;
		}

		public void SetLocked(InputHintElement row, bool locked)
		{
			if (!row || locked == _tinted.Contains(row))
			{
				return;
			}
			(Graphic, Color)[] array = OriginalsOf(row);
			for (int i = 0; i < array.Length; i++)
			{
				var (graphic, color) = array[i];
				if ((bool)graphic)
				{
					graphic.color = (locked ? (color * _lockedTint) : color);
				}
			}
			if (locked)
			{
				_tinted.Add(row);
			}
			else
			{
				_tinted.Remove(row);
			}
		}

		private (Graphic graphic, Color color)[] OriginalsOf(InputHintElement row)
		{
			if (_originals.TryGetValue(row, out (Graphic, Color)[] value))
			{
				return value;
			}
			List<(Graphic, Color)> list = new List<(Graphic, Color)>();
			Graphic[] componentsInChildren = row.GetComponentsInChildren<Graphic>(includeInactive: true);
			foreach (Graphic graphic in componentsInChildren)
			{
				if (graphic.gameObject.name != "RebindHighlight")
				{
					list.Add((graphic, graphic.color));
				}
			}
			value = list.ToArray();
			_originals[row] = value;
			return value;
		}
	}
}
