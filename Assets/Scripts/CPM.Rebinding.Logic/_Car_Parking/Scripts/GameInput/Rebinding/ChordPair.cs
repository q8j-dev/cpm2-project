using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public readonly struct ChordPair
	{
		public readonly InputAction Action;

		public readonly int Plain;

		public readonly int Header;

		public readonly int Modifier;

		public readonly int Main;

		public readonly CaptureKind Kind;

		public bool ComboActive => !string.IsNullOrEmpty(Action.bindings[Main].effectivePath);

		public string RowPart
		{
			get
			{
				if (!ChordSlots.IsRuntime(Action.bindings[Plain]))
				{
					return string.Empty;
				}
				return Action.bindings[Main].name;
			}
		}

		public int DisplayedIndex
		{
			get
			{
				if (!ComboActive)
				{
					return Plain;
				}
				return Main;
			}
		}

		public ChordPair(InputAction action, int plain, int header, int modifier, int main, CaptureKind kind)
		{
			Action = action;
			Plain = plain;
			Header = header;
			Modifier = modifier;
			Main = main;
			Kind = kind;
		}

		public bool Contains(int index)
		{
			if (index != Plain && index != Header && index != Modifier)
			{
				return index == Main;
			}
			return true;
		}
	}
}
