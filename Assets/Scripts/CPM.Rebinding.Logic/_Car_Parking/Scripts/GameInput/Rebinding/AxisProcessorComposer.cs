using System;
using System.Collections.Generic;
using System.Text;

namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public static class AxisProcessorComposer
	{
		public const string Invert = "Invert";

		public const string SliderRemap = "SliderRemap";

		public static string Compose(string authored, bool invert, bool remap)
		{
			List<string> list = new List<string>();
			foreach (string item in SplitTopLevel(authored))
			{
				string a = NameOf(item);
				if (!Same(a, "Invert") && !Same(a, "SliderRemap"))
				{
					list.Add(item);
				}
			}
			StringBuilder stringBuilder = new StringBuilder();
			if (invert)
			{
				Append(stringBuilder, "Invert");
			}
			foreach (string item2 in list)
			{
				Append(stringBuilder, item2);
			}
			if (remap)
			{
				Append(stringBuilder, "SliderRemap");
			}
			return stringBuilder.ToString();
		}

		private static IEnumerable<string> SplitTopLevel(string list)
		{
			if (string.IsNullOrEmpty(list))
			{
				yield break;
			}
			int depth = 0;
			int num = 0;
			for (int i = 0; i <= list.Length; i++)
			{
				switch ((i < list.Length) ? list[i] : ',')
				{
				case '(':
					depth++;
					break;
				case ')':
					depth = Math.Max(0, depth - 1);
					break;
				case ',':
				case ';':
					if (depth == 0)
					{
						string text = list.Substring(num, i - num).Trim();
						if (text.Length > 0)
						{
							yield return text;
						}
						num = i + 1;
					}
					break;
				}
			}
		}

		private static string NameOf(string entry)
		{
			int num = entry.IndexOf('(');
			return ((num >= 0) ? entry.Substring(0, num) : entry).Trim();
		}

		private static void Append(StringBuilder builder, string entry)
		{
			if (builder.Length > 0)
			{
				builder.Append(',');
			}
			builder.Append(entry);
		}

		private static bool Same(string a, string b)
		{
			return string.Equals(a, b, StringComparison.OrdinalIgnoreCase);
		}
	}
}
