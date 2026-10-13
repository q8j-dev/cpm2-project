using System;
using System.Collections.Generic;
using CPM.Account;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using Steamworks;
using UnityEngine;
using UnityEngine.Device;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Root;
using _Car_Parking.Scripts.PrefsKeys;
using _Car_Parking.Scripts.Steam;
using Application = UnityEngine.Application;

namespace _Car_Parking.Scripts.Analytics.Event.CustomEvent
{
	public class LoginEvent : DefaultEvent<DefaultContext>
	{
		private static bool? _isSteamDeckCached;

		private string Rank => Utils.GetRankNameByIndex(ObscuredPrefs.GetInt("rank" + DateTime.Now.Day));

		protected override string Name => "login";

		protected override Dictionary<string, object> CustomParameters
		{
			get
			{
				Dictionary<string, object> obj = new Dictionary<string, object>
				{
					{
						"lang",
						Application.systemLanguage.ToString()
					},
					{
						"battery_level",
						Mathf.Clamp(Mathf.RoundToInt(UnityEngine.SystemInfo.batteryLevel * 100f), -1, 100)
					},
					{
						"screen_resolution_w",
						UnityEngine.Device.Screen.width
					},
					{
						"screen_resolution_h",
						UnityEngine.Device.Screen.height
					},
					{
						"graphics_quality",
						QualitySettings.names[QualitySettings.GetQualityLevel()]
					},
					{
						"frame_rate",
						Application.targetFrameRate
					},
					{
						"ram",
						UnityEngine.SystemInfo.systemMemorySize
					},
					{
						"device_model",
						UnityEngine.SystemInfo.deviceModel
					},
					{
						"cpu_cores",
						UnityEngine.SystemInfo.processorCount
					},
					{
						"timezone",
						TimeZoneInfo.Local.BaseUtcOffset.TotalHours
					},
					{
						"device_type",
						GetDeviceType()
					},
					{ "player_rank", Rank },
					{
						"cpu_model",
						UnityEngine.SystemInfo.processorType
					},
					{
						"gpu_renderer",
						UnityEngine.SystemInfo.graphicsDeviceName
					},
					{
						"os_version",
						GetOsVersion()
					},
					{
						"os_major",
						GetOsMajor(GetOsVersion())
					}
				};
				AppendSteamParameters(obj);
				return obj;
			}
		}

		public LoginEvent(CPMAnalytics analytics, DefaultContext defaultContext)
			: base(analytics, defaultContext)
		{
		}

		private string GetDeviceType()
		{
			float dpi = UnityEngine.Device.Screen.dpi;
			float num = (float)UnityEngine.Device.Screen.width / dpi;
			float num2 = (float)UnityEngine.Device.Screen.height / dpi;
			float num3 = Mathf.Sqrt(num * num + num2 * num2);
			return "desktop";
		}

		private string GetOsVersion()
		{
			string operatingSystem = UnityEngine.SystemInfo.operatingSystem;
			if (string.IsNullOrWhiteSpace(operatingSystem))
			{
				return null;
			}
			if (operatingSystem.ToLowerInvariant() == "unknown")
			{
				return null;
			}
			return operatingSystem;
		}

		private int GetOsMajor(string osVersion)
		{
			if (string.IsNullOrEmpty(osVersion))
			{
				return -1;
			}
			for (int i = 0; i < osVersion.Length; i++)
			{
				if (char.IsDigit(osVersion[i]))
				{
					int num = i;
					for (; i < osVersion.Length && char.IsDigit(osVersion[i]); i++)
					{
					}
					if (!int.TryParse(osVersion.Substring(num, i - num), out var result) || result <= 0)
					{
						break;
					}
					return result;
				}
			}
			return -1;
		}

		private static void AppendSteamParameters(Dictionary<string, object> parameters)
		{
			if (SteamManager.IsInitialized())
			{
				parameters["steam_id"] = SteamManager.GetUserIdString();
				parameters["steam_account_id"] = SteamManager.GetUserAccountId();
				try
				{
					parameters["steam_branch"] = SteamApps.CurrentBetaName ?? "default";
				}
				catch
				{
					parameters["steam_branch"] = "default";
				}
				try
				{
					parameters["is_family_share"] = SteamApps.IsSubscribedFromFamilySharing;
				}
				catch
				{
					parameters["is_family_share"] = false;
				}
				parameters["steam_language"] = SteamManager.GetSteamLanguage();
				parameters["steam_level"] = SteamManager.GetSteamLevel();
			}
			parameters["is_steam_deck"] = IsSteamDeck();
			parameters["is_steam_linked"] = ManagerKeysForPrefs.IsSteamLinked;
			string text = AnalyticsDependencies.Get<IAccountController>()?.Email;
			parameters["is_steam_only_account"] = !string.IsNullOrEmpty(text) && AccountSystemV2Extension.IsSteamOnlyAccount(text);
		}

		private static bool IsSteamDeck()
		{
			if (_isSteamDeckCached.HasValue)
			{
				return _isSteamDeckCached.Value;
			}
			string text = UnityEngine.SystemInfo.graphicsDeviceName ?? string.Empty;
			string text2 = UnityEngine.SystemInfo.processorType ?? string.Empty;
			_isSteamDeckCached = text.Contains("AMD Custom GPU 0405") || text2.Contains("AMD Custom APU 0405");
			return _isSteamDeckCached.Value;
		}
	}
}
