using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class WheelTemplate
	{
		public const string MozaFamily = "<HID::Gudsen MOZA R5 Base>";

		public const string GenericSteering = "<Joystick>/stick/x";

		private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

		public static bool IsTemplateSlot(in InputBinding binding)
		{
			if (!binding.isComposite)
			{
				if (!BindingPaths.HasDevicePrefix(binding.path, "<HID::Gudsen MOZA R5 Base>"))
				{
					return IsGenericSteering(in binding);
				}
				return true;
			}
			return false;
		}

		public static bool IsGenericSteering(in InputBinding binding)
		{
			return string.Equals(binding.path, "<Joystick>/stick/x", StringComparison.OrdinalIgnoreCase);
		}

		public static HashSet<string> TemplateBindingIds(InputActionAsset asset)
		{
			HashSet<string> hashSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
			foreach (InputActionMap actionMap in asset.actionMaps)
			{
				foreach (InputBinding binding2 in actionMap.bindings)
				{
					InputBinding binding = binding2;
					if (IsTemplateSlot(in binding))
					{
						hashSet.Add(binding.id.ToString());
					}
				}
			}
			return hashSet;
		}

		public static List<BindingChange> DefaultChanges(InputActionAsset asset, WheelKind kind)
		{
			List<BindingChange> list = new List<BindingChange>();
			if (kind != WheelKind.Generic && kind != WheelKind.Moza)
			{
				return list;
			}
			foreach (InputActionMap actionMap in asset.actionMaps)
			{
				foreach (InputAction action in actionMap.actions)
				{
					for (int i = 0; i < action.bindings.Count; i++)
					{
						InputBinding binding = action.bindings[i];
						if (IsTemplateSlot(in binding))
						{
							bool flag = IsGenericSteering(in binding);
							if ((kind == WheelKind.Generic) ? (!flag) : flag)
							{
								list.Add(new BindingChange(action, i, string.Empty));
							}
						}
					}
				}
			}
			return list;
		}
	}
}
