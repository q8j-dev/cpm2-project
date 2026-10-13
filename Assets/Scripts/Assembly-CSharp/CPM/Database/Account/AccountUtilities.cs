using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CPM.Database.Account
{
	public class AccountUtilities
	{
		public static bool IsValidEmail(string email)
		{
			if (string.IsNullOrWhiteSpace(email))
			{
				return false;
			}
			try
			{
				email = Regex.Replace(email, "(@)(.+)$", DomainMapper, RegexOptions.None, TimeSpan.FromMilliseconds(200.0));
			}
			catch (RegexMatchTimeoutException)
			{
				return false;
			}
			try
			{
				return Regex.IsMatch(email, "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250.0));
			}
			catch (RegexMatchTimeoutException)
			{
				return false;
			}
			static string DomainMapper(Match match)
			{
				try
				{
					string ascii = new IdnMapping().GetAscii(match.Groups[2].Value);
					return match.Groups[1].Value + ascii;
				}
				catch (Exception)
				{
					return "";
				}
			}
		}
	}
}
