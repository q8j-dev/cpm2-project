using UnityEngine.InputSystem;
using UnityEngine.UI;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public readonly struct RebindRow
	{
		public readonly int Index;

		public readonly InputHintElement Element;

		public readonly Button Button;

		public readonly InputAction Action;

		public readonly string Part;

		public readonly int SlotIndex;

		public RebindRow(int index, InputHintElement element, Button button, InputAction action, string part, int slotIndex = -1)
		{
			Index = index;
			Element = element;
			Button = button;
			Action = action;
			Part = part;
			SlotIndex = slotIndex;
		}
	}
}
