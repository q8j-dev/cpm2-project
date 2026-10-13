using System.Text.RegularExpressions;

namespace _Car_Parking.Scripts.MenuUI.LocalizationUI
{
	public static class LocalizedTextStyles
	{
		private static readonly (string token, string replacement)[] Styles = new (string, string)[7]
		{
			("[bco]", "<b><color=#FFBA00>"),
			("[/bc]", "</color></b>"),
			("[b]", "<b>"),
			("[/b]", "</b>"),
			("[co]", "<color=#FFBA00>"),
			("[cr]", "<color=#FC563E>"),
			("[/c]", "</color>")
		};

		public static string Apply(string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return text;
			}
			(string, string)[] styles = Styles;
			for (int i = 0; i < styles.Length; i++)
			{
				(string, string) tuple = styles[i];
				string item = tuple.Item1;
				string item2 = tuple.Item2;
				text = Regex.Replace(text, Regex.Escape(item), item2.Replace("$", "$$"), RegexOptions.IgnoreCase);
			}
			return text;
		}
	}
}
