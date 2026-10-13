using System;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class WheelClassifier
	{
		public const string MozaLayout = "HID::Gudsen MOZA R5 Base";

		public const string G25Layout = "HID::G25 Racing Wheel";

		private const int VendorLogitech = 1133;

		private const int VendorThrustmaster = 1103;

		private const int VendorFanatec = 3767;

		private const int VendorMoza = 13422;

		private static readonly int[] LogitechWheelProducts = new int[9] { 49817, 49819, 49743, 49762, 49766, 49774, 49818, 49816, 49812 };

		private static readonly string[] FlightWords = new string[10] { "flight", "hotas", "16000", "warthog", "twcs", "throttle", "airbus", "yoke", "rudder", "sidestick" };

		private static readonly string[] AccessoryWords = new string[7] { "handbrake", "hand brake", "pedal", "shifter", "button box", "hbp", "hgp" };

		private static readonly string[] WheelWords = new string[14]
		{
			"wheel", "racing", "driving", "g27", "g29", "g920", "g923", "thrustmaster", "fanatec", "simagic",
			"simucube", "cammus", "asetek", "moza"
		};

		private static readonly string[] MozaBaseWords = new string[6] { "base", "r5", "r9", "r12", "r16", "r21" };

		public static WheelKind Classify(in WheelDeviceInfo info)
		{
			if (info.IsGamepad || !info.IsJoystick)
			{
				return WheelKind.None;
			}
			string text = (info.Manufacturer + " " + info.Product + " " + info.Layout).ToLowerInvariant();
			if (ContainsAny(text, FlightWords))
			{
				return WheelKind.None;
			}
			if (ContainsAny(text, AccessoryWords))
			{
				return WheelKind.Accessory;
			}
			if (string.Equals(info.Layout, "HID::G25 Racing Wheel", StringComparison.OrdinalIgnoreCase))
			{
				return WheelKind.G25;
			}
			if (info.VendorId == 13422 || text.Contains("moza"))
			{
				if (!ContainsAny(text, MozaBaseWords))
				{
					return WheelKind.Accessory;
				}
				return WheelKind.Moza;
			}
			if (IsKnownWheelId(info.VendorId, info.ProductId) || ContainsAny(text, WheelWords))
			{
				return WheelKind.Generic;
			}
			return WheelKind.None;
		}

		public static bool IsWheel(WheelKind kind)
		{
			if (kind != WheelKind.Moza && kind != WheelKind.G25)
			{
				return kind == WheelKind.Generic;
			}
			return true;
		}

		private static bool IsKnownWheelId(int vendor, int product)
		{
			switch (vendor)
			{
			case 1103:
			case 3767:
				return true;
			case 1133:
				return Array.IndexOf(LogitechWheelProducts, product) >= 0;
			default:
				return false;
			}
		}

		private static bool ContainsAny(string text, string[] words)
		{
			foreach (string value in words)
			{
				if (text.Contains(value))
				{
					return true;
				}
			}
			return false;
		}
	}
}
