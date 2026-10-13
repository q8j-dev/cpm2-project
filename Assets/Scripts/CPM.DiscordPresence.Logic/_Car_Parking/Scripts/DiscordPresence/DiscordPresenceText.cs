using System;
using System.Globalization;

namespace _Car_Parking.Scripts.DiscordPresence
{
	public static class DiscordPresenceText
	{
		public const int FieldCharacterLimit = 128;

		public static string FormatDetails(string plain, string withLocation, string location)
		{
			if (!string.IsNullOrEmpty(location) && !string.IsNullOrEmpty(withLocation))
			{
				return TryFormat(withLocation, location) ?? plain ?? string.Empty;
			}
			return plain ?? string.Empty;
		}

		public static string FormatState(string plain, string withLaps, int lap, int totalLaps)
		{
			if (lap > 0 && totalLaps > 0 && !string.IsNullOrEmpty(withLaps))
			{
				return TryFormat(withLaps, lap, totalLaps) ?? plain ?? string.Empty;
			}
			return plain ?? string.Empty;
		}

		public static string ToField(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return null;
			}
			if (value.Length <= 128)
			{
				return value;
			}
			int num = 128;
			if (char.IsHighSurrogate(value[num - 1]))
			{
				num--;
			}
			return value.Substring(0, num);
		}

		public static bool TryParseApplicationId(string raw, out ulong applicationId)
		{
			applicationId = 0uL;
			if (string.IsNullOrWhiteSpace(raw))
			{
				return false;
			}
			if (!ulong.TryParse(raw.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var result) || result == 0L)
			{
				return false;
			}
			applicationId = result;
			return true;
		}

		private static string TryFormat(string format, params object[] args)
		{
			try
			{
				return string.Format(CultureInfo.InvariantCulture, format, args);
			}
			catch (FormatException)
			{
				return null;
			}
		}
	}
}
