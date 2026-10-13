using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using _Car_Parking.Scripts.GameInput.Rebinding;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public static class LayoutWheelSlots
	{
		private const string G25Family = "<HID::G25 Racing Wheel>";

		private const string MenuAccept = "UIAcceptCancel/Submit";

		private const string MenuBack = "UIAcceptCancel/Cancel";

		public static Predicate<InputBinding> FilterFor(ActiveWheelService wheels)
		{
			if (wheels?.ActiveWheel == null)
			{
				return null;
			}
			return wheels.ActiveKind switch
			{
				WheelKind.G25 => (InputBinding binding) => BindingPaths.HasDevicePrefix(binding.path, "<HID::G25 Racing Wheel>"), 
				WheelKind.Moza => (InputBinding binding) => BindingPaths.HasDevicePrefix(binding.path, "<HID::Gudsen MOZA R5 Base>"), 
				WheelKind.Generic => (InputBinding binding) => WheelTemplate.IsTemplateSlot(in binding), 
				_ => null, 
			};
		}

		public static RebindDeviceProfile ProfileFor(ActiveWheelService wheels, InputActionAsset asset)
		{
			InputDevice inputDevice = wheels?.ActiveWheel;
			if (inputDevice == null || !WheelClassifier.IsWheel(wheels.ActiveKind))
			{
				return null;
			}
			string family = "<" + inputDevice.layout + ">";
			return RebindDeviceProfile.CreateWheel(inputDevice.layout, wheels.ActiveKind != WheelKind.G25, WheelPathsOf(asset, "UIAcceptCancel/Submit", family), WheelPathsOf(asset, "UIAcceptCancel/Cancel", family));
		}

		private static List<string> WheelPathsOf(InputActionAsset asset, string actionPath, string family)
		{
			List<string> list = new List<string>();
			InputAction inputAction = (asset ? asset.FindAction(actionPath) : null);
			if (inputAction == null)
			{
				return list;
			}
			foreach (InputBinding binding in inputAction.bindings)
			{
				if (!binding.isComposite && BindingPaths.HasDevicePrefix(binding.effectivePath, family))
				{
					list.Add(binding.effectivePath);
				}
			}
			return list;
		}

		public static bool Reset(BindingOverridesService overrides, ActiveWheelService wheels)
		{
			if (overrides == null || wheels?.ActiveWheel == null)
			{
				return false;
			}
			if (wheels.ActiveKind == WheelKind.G25)
			{
				return overrides.ResetOverridesWhere((InputBinding binding) => BindingPaths.HasDevicePrefix(binding.path, "<HID::G25 Racing Wheel>"));
			}
			return overrides.ResetActiveWheel();
		}
	}
}
