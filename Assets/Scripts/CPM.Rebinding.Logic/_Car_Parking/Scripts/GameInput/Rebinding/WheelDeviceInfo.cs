using System;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.HID;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public readonly struct WheelDeviceInfo
	{
		public readonly string Layout;

		public readonly string Product;

		public readonly string Manufacturer;

		public readonly bool IsGamepad;

		public readonly bool IsJoystick;

		public readonly int VendorId;

		public readonly int ProductId;

		public WheelDeviceInfo(string layout, string product, string manufacturer, bool isGamepad, bool isJoystick, int vendorId, int productId)
		{
			Layout = layout ?? string.Empty;
			Product = product ?? string.Empty;
			Manufacturer = manufacturer ?? string.Empty;
			IsGamepad = isGamepad;
			IsJoystick = isJoystick;
			VendorId = vendorId;
			ProductId = productId;
		}

		public static WheelDeviceInfo From(InputDevice device)
		{
			int vendorId = 0;
			int productId = 0;
			string capabilities = device.description.capabilities;
			if (device.description.interfaceName == "HID" && !string.IsNullOrEmpty(capabilities))
			{
				try
				{
					HID.HIDDeviceDescriptor hIDDeviceDescriptor = HID.HIDDeviceDescriptor.FromJson(capabilities);
					vendorId = hIDDeviceDescriptor.vendorId;
					productId = hIDDeviceDescriptor.productId;
				}
				catch (Exception)
				{
				}
			}
			return new WheelDeviceInfo(device.layout, device.description.product, device.description.manufacturer, device is Gamepad, device is Joystick, vendorId, productId);
		}
	}
}
