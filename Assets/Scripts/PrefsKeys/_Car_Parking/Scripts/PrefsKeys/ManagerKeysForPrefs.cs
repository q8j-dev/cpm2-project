using CodeStage.AntiCheat.Storage;

namespace _Car_Parking.Scripts.PrefsKeys
{
	public class ManagerKeysForPrefs
	{
		private const string LocalIdPrefKey = "PlayerID_2";

		private const string AccountIdKey = "accountID_2";

		private const string EmailKey = "userEmail_2";

		private const string PasswordAccountKey = "PasswordAccount";

		private const string DefaultAccountId = "0";

		private const string DefaultEmail = "0";

		private const string DefaultLocalId = "";

		public const string EsPath = "es3";

		private const string CreationtimestampKey = "CreationTimestamp";

		private const string PlayerNameKey = "PlayerName";

		public const string LoginTimeUnixDataKey = "loginTime";

		private const string IsSteamLinkedKey = "IsSteamLinked";

		private const string LinkedSteamIdKey = "LinkedSteamId";

		private const string LastLoginMethodKey = "LastLoginMethod";

		public const string KickedByAnotherDeviceKey = "KickedByAnotherDevice";

		public static string PlayerName
		{
			get
			{
				return ObscuredPrefs.GetString("PlayerName", "Player123");
			}
			set
			{
				ObscuredPrefs.SetString("PlayerName", value);
			}
		}

		public static string AccountId
		{
			get
			{
				return ObscuredPrefs.GetString("accountID_2", "0");
			}
			set
			{
				ObscuredPrefs.SetString("accountID_2", value);
			}
		}

		public static bool HasAccountId => ObscuredPrefs.HasKey("accountID_2");

		public static string Email
		{
			get
			{
				return ObscuredPrefs.GetString("userEmail_2", "0");
			}
			set
			{
				ObscuredPrefs.SetString("userEmail_2", value);
			}
		}

		public static ulong CreationTimestamp
		{
			get
			{
				return ObscuredPrefs.GetULong("CreationTimestamp", 0uL);
			}
			set
			{
				ObscuredPrefs.SetULong("CreationTimestamp", value);
			}
		}

		public static bool HasEmail => ObscuredPrefs.HasKey("userEmail_2");

		public static string PasswordAccount
		{
			get
			{
				return ObscuredPrefs.GetString("PasswordAccount", "123");
			}
			set
			{
				ObscuredPrefs.SetString("PasswordAccount", value);
			}
		}

		public static bool HasPasswordAccount => ObscuredPrefs.HasKey("PasswordAccount");

		public static string LocalId
		{
			get
			{
				return ObscuredPrefs.GetString("PlayerID_2");
			}
			set
			{
				ObscuredPrefs.SetString("PlayerID_2", value);
			}
		}

		public static bool IsSteamLinked
		{
			get
			{
				return ObscuredPrefs.GetBool("IsSteamLinked");
			}
			set
			{
				ObscuredPrefs.SetBool("IsSteamLinked", value);
				ObscuredPrefs.Save();
			}
		}

		public static bool KickedByAnotherDevice
		{
			get
			{
				return ObscuredPrefs.GetBool("KickedByAnotherDevice");
			}
			set
			{
				ObscuredPrefs.SetBool("KickedByAnotherDevice", value);
			}
		}

		public static string LinkedSteamId
		{
			get
			{
				return ObscuredPrefs.GetString("LinkedSteamId", "0");
			}
			set
			{
				ObscuredPrefs.SetString("LinkedSteamId", value);
				ObscuredPrefs.Save();
			}
		}

		public static bool HasLinkedSteamId => ObscuredPrefs.HasKey("LinkedSteamId");

		public static LoginMethod LastLoginMethod
		{
			get
			{
				return (LoginMethod)ObscuredPrefs.GetInt("LastLoginMethod");
			}
			set
			{
				ObscuredPrefs.SetInt("LastLoginMethod", (int)value);
				ObscuredPrefs.Save();
			}
		}
	}
}
