using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace _Car_Parking.Scripts.AccountSystemNew
{
	public static class AccountSystemV2Extension
	{
		public const string RandomDeviceIdPrefs = "RandomDeviceId";

		private const int MaxEmailLength = 200;

		private const int MaxPasswordLength = 100;

		private const string GuestDomain = "@ogames.guest";

		private const string NoDeviceGuestDomain = "@ogames.noDeviceGuest";

		private const string SteamGuestDomain = "@ogames.steamguest";

		public static string GuestEmail
		{
			get
			{
				string text = GetDeviceID();
				if (string.IsNullOrEmpty(text))
				{
					text = "1234567890";
				}
				while (text.Length < 15)
				{
					text += "1";
				}
				text = GetSalt(text, 2, 3, 4, 1);
				text = "v47" + text;
				text = GetMd5Hash(text);
				text = text.Substring(0, Mathf.Min(200, text.Length));
				if (SystemInfo.deviceUniqueIdentifier == GetDeviceID())
				{
					return text + "@ogames.guest";
				}
				return text + "@ogames.noDeviceGuest";
			}
		}

		public static string GuestPassword
		{
			get
			{
				string text = GetDeviceID();
				if (string.IsNullOrEmpty(text))
				{
					text = "1234567890";
				}
				while (text.Length < 15)
				{
					text += "1";
				}
				text = GetSalt(text, 5, 4, 2, 3);
				text = GetMd5Hash(text);
				return text.Substring(0, Mathf.Min(100, text.Length));
			}
		}

		public static bool IsGuestAccount(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return false;
			}
			if (!email.Contains("@ogames.guest"))
			{
				return email.Contains("@ogames.noDeviceGuest");
			}
			return true;
		}

		public static bool IsReservedGuestEmail(string email)
		{
			if (string.IsNullOrEmpty(email))
			{
				return false;
			}
			if (email.IndexOf("@ogames.guest", StringComparison.OrdinalIgnoreCase) < 0)
			{
				return email.IndexOf("@ogames.noDeviceGuest", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			return true;
		}

		public static string GetMd5Hash(string input)
		{
			using MD5 mD = MD5.Create();
			byte[] bytes = Encoding.UTF8.GetBytes(input);
			byte[] array = mD.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			byte[] array2 = array;
			foreach (byte b in array2)
			{
				stringBuilder.Append(b.ToString("x2"));
			}
			return stringBuilder.ToString();
		}

		private static string GetSalt(string value, int switchKey1, int switchKey2, int removeKey1, int addKey1)
		{
			List<char> list = new List<char>(value.ToCharArray());
			if (switchKey1 < list.Count && switchKey2 < list.Count)
			{
				char value2 = list[switchKey1];
				char value3 = list[switchKey2];
				list[switchKey1] = value3;
				list[switchKey2] = value2;
			}
			if (removeKey1 < list.Count)
			{
				list.RemoveAt(removeKey1);
			}
			if (addKey1 < list.Count)
			{
				list.Add(list[addKey1]);
			}
			return new string(list.ToArray());
		}

		public static string GetDeviceID()
		{
			if (!string.IsNullOrEmpty(SystemInfo.deviceUniqueIdentifier) && SystemInfo.deviceUniqueIdentifier.Length > 2)
			{
				return SystemInfo.deviceUniqueIdentifier;
			}
			if (!PlayerPrefs.HasKey("RandomDeviceId"))
			{
				PlayerPrefs.SetString("RandomDeviceId", Guid.NewGuid().ToString());
			}
			return PlayerPrefs.GetString("RandomDeviceId");
		}

		public static bool IsSteamOnlyAccount(string email)
		{
			if (!(email == "steam_authorized"))
			{
				if (!string.IsNullOrEmpty(email))
				{
					return email.Contains("@ogames.steamguest");
				}
				return false;
			}
			return true;
		}
	}
}
