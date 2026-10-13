using System;
using UnityEngine;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Steam.InputHints;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public static class LayoutRowChips
	{
		private const string UnboundKey = "SteamControls.RebindUnbound";

		private const string Separator = " · ";

		public static bool IsUnbound(InputHintElement row)
		{
			if ((bool)row)
			{
				return row.DisplayedBindingPath == string.Empty;
			}
			return false;
		}

		public static string WithChip(string label, InputHintElement row, bool wheelPage, Color warning, bool modifierUnbound)
		{
			if (IsUnbound(row) | modifierUnbound)
			{
				return string.Concat(label, " · <color=#", ColorUtility.ToHtmlStringRGBA(warning), ">", new LocalizedString("SteamControls.RebindUnbound"), "</color>");
			}
			string text = (row ? row.DisplayedBindingPath : null);
			if (!wheelPage || string.IsNullOrEmpty(text))
			{
				return label;
			}
			InputHintSpriteMap spriteMapAsset = row.SpriteMapAsset;
			if ((bool)spriteMapAsset && spriteMapAsset.HasSprite(text, InputDeviceType.Wheel))
			{
				return label;
			}
			return label + " · " + ShortControlName(text);
		}

		public static string ShortControlName(string path)
		{
			int num = path.IndexOf('>');
			string text = ((num >= 0 && num + 2 <= path.Length) ? path.Substring(num + 2) : path);
			if (text.StartsWith("button", StringComparison.OrdinalIgnoreCase) && text.Length > "button".Length)
			{
				return "B" + text.Substring("button".Length);
			}
			if (string.Equals(text, "trigger", StringComparison.OrdinalIgnoreCase))
			{
				return "TRG";
			}
			return text.ToUpperInvariant();
		}
	}
}
