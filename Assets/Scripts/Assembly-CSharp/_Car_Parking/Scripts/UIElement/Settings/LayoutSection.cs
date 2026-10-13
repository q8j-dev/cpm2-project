using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public sealed class LayoutSection
	{
		public string Title;

		public readonly List<(InputAction action, string part, int slot)> Rows = new List<(InputAction, string, int)>();
	}
}
