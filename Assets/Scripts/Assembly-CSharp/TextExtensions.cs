using System;
using System.Collections.Generic;
using System.Globalization;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Localization;

public static class TextExtensions
{
	private static readonly Dictionary<string, string> IsoCurrencyToCulture = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
	{
		{ "USD", "en-US" },
		{ "EUR", "fr-FR" },
		{ "GBP", "en-GB" },
		{ "JPY", "ja-JP" },
		{ "RUB", "ru-RU" },
		{ "CNY", "zh-CN" },
		{ "CHF", "de-CH" }
	};

	private static readonly string[] IsoCurrencyFirst = new string[1] { "KZT" };

	public static TimeUnitSuffixes CreateTimeUnitSuffixes()
	{
		return new TimeUnitSuffixes(new LocalizedString("EventViewWindow.D"), new LocalizedString("EventViewWindow.H"), new LocalizedString("EventViewWindow.M"), new LocalizedString("EventViewWindow.S"));
	}

	public static string IntFormatThousandSpace(int value)
	{
		NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
		numberFormatInfo.NumberGroupSeparator = " ";
		return value.ToString("N0", numberFormatInfo);
	}

	public static string FormatPrice(float amount, string isoCurrencyCode)
	{
		if (string.IsNullOrEmpty(isoCurrencyCode))
		{
			return amount.ToString("0.##", CultureInfo.InvariantCulture);
		}
		if (!IsoCurrencyToCulture.TryGetValue(isoCurrencyCode, out var value))
		{
			if (IsoCurrencyFirst.Contains(isoCurrencyCode))
			{
				return isoCurrencyCode + " " + amount.ToString("0.##", CultureInfo.InvariantCulture);
			}
			return amount.ToString("0.##", CultureInfo.InvariantCulture) + " " + isoCurrencyCode;
		}
		CultureInfo obj = (CultureInfo)new CultureInfo(value).Clone();
		obj.NumberFormat.CurrencySymbol = new RegionInfo(value).CurrencySymbol;
		obj.NumberFormat.CurrencyDecimalDigits = 2;
		return string.Format(obj, "{0:C}", amount);
	}
}
