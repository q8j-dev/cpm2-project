using System;
using System.Text;

namespace _Car_Parking.Scripts.Analytics.Types
{
	public static class LobbyErrorCode
	{
		public const string Other = "OTHER";

		private const string ExceptionSuffix = "Exception";

		private const int MaxMessageLength = 200;

		private static readonly char[] LineBreaks = new char[2] { '\r', '\n' };

		public static string From(Exception error)
		{
			if (error == null)
			{
				return null;
			}
			if (error is TimeoutException)
			{
				return "TIMEOUT";
			}
			if (error is NullReferenceException)
			{
				return "NULL_REF";
			}
			return FromTypeName(error.GetType().Name);
		}

		public static string MessageOf(Exception error)
		{
			string text = error?.Message;
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			int num = text.IndexOfAny(LineBreaks);
			if (num >= 0)
			{
				text = text.Substring(0, num);
			}
			text = text.Trim();
			if (text.Length > 200)
			{
				text = text.Substring(0, 200);
			}
			if (text.Length <= 0)
			{
				return null;
			}
			return text;
		}

		private static string FromTypeName(string typeName)
		{
			if (string.IsNullOrEmpty(typeName))
			{
				return "OTHER";
			}
			if (typeName.Length > "Exception".Length && typeName.EndsWith("Exception", StringComparison.Ordinal))
			{
				typeName = typeName.Substring(0, typeName.Length - "Exception".Length);
			}
			StringBuilder stringBuilder = new StringBuilder(typeName.Length + 8);
			for (int i = 0; i < typeName.Length; i++)
			{
				char c = typeName[i];
				if (char.IsLetterOrDigit(c))
				{
					if (char.IsUpper(c) && stringBuilder.Length > 0 && (char.IsLower(typeName[i - 1]) || (i + 1 < typeName.Length && char.IsLower(typeName[i + 1]))))
					{
						stringBuilder.Append('_');
					}
					stringBuilder.Append(char.ToUpperInvariant(c));
				}
			}
			if (stringBuilder.Length <= 0)
			{
				return "OTHER";
			}
			return stringBuilder.ToString();
		}
	}
}
