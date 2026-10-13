using System;
using System.Collections.Generic;
using CPM.Account;
using CPM.PlayerData;
using CPM.RemoteConfig;
using Cysharp.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using _Car_Parking.Scripts.AccountSystemNew;
using _Car_Parking.Scripts.Analytics.Root;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.Analytics.Event
{
	public abstract class DefaultEvent<TContext> where TContext : IEventContext
	{
		private readonly CPMAnalytics _analytics;

		private ConfigValuesStorage _cachedConfig;

		private HashSet<string> _cachedDisabledEvents;

		protected virtual Dictionary<string, object> CustomParameters => new Dictionary<string, object>();

		protected TContext Context { get; private set; }

		protected abstract string Name { get; }

		protected virtual bool CanSend => true;

		protected string AccountID
		{
			get
			{
				string text = (AnalyticsDependencies.TryGet<PlayerDataStorage>(out var service) ? service.AccountID.ToString() : null);
				if (text == "1" || text.IsNullOrEmpty())
				{
					text = "anonymous";
				}
				return text;
			}
		}

		protected DefaultEvent(CPMAnalytics analytics, TContext context)
		{
			_analytics = analytics;
			Context = context;
		}

		public void Send()
		{
			SendAsync().Forget();
		}

		private async UniTask SendAsync()
		{
			if (!AnalyticsDependencies.TryGet<ServerTimeService>(out var service) || !CanSend)
			{
				return;
			}
			long timestamp = service.CurrentServerUnixTimeSec;
			if (!(await ProhibitionCheck(Name)) || !AnalyticsDependencies.TryGet<ServerTimeService>(out var _))
			{
				return;
			}
			Dictionary<string, object> defaultParameters = GetDefaultParameters(timestamp);
			defaultParameters["params"] = CustomParameters;
			List<Parameter> list = new List<Parameter>();
			foreach (KeyValuePair<string, object> item in defaultParameters)
			{
				list.Add(new Parameter(item.Key, item.Value));
			}
			list.Add(new Parameter("event_type", Name));
			_analytics.LogEvent(list.ToArray());
			LogEvent(defaultParameters);
		}

		private Dictionary<string, object> GetDefaultParameters(long timestamp)
		{
			if (AnalyticStorage.LastSessionId.IsNullOrEmpty())
			{
				AnalyticStorage.LastSessionId = Guid.NewGuid().ToString();
			}
			int num = PlayerPrefs.GetInt("languageIndex", 42);
			string text = AnalyticsDependencies.Get<IAccountController>()?.Email;
			if (text == "0")
			{
				text = null;
			}
			return new Dictionary<string, object>
			{
				{
					"event_id",
					Guid.NewGuid().ToString()
				},
				{ "user_fb_id", AccountID },
				{
					"user_id",
					AnalyticsDependencies.Get<PlayerDataStorage>()?.PlayerID?.ToString()
				},
				{
					"device_id",
					SystemInfo.deviceUniqueIdentifier
				},
				{
					"session_id",
					AnalyticStorage.LastSessionId
				},
				{ "timestamp", timestamp },
				{
					"build_version",
					Application.version
				},
				{
					"build_id",
					Application.buildGUID
				},
				{
					"hard_currency",
					(int)CurrencyManagerController.Coins
				},
				{
					"soft_currency",
					(int)CurrencyManagerController.Money
				},
				{
					"connection",
					Application.internetReachability != NetworkReachability.NotReachable
				},
				{
					"internet_type",
					GetInternetType()
				},
				{ "ab_test_id", null },
				{
					"ab_test_group",
					AbTestGroup.NONE
				},
				{
					"platform",
					GetCustomPlatform()
				},
				{
					"environment",
					AnalyticsEnvironmentResolver.Current.ToString()
				},
				{
					"referral",
					ReferalType.Organic.ToString()
				},
				{
					"country",
					AnalyticsDependencies.Get<GeoGetterService>()?.GetGeoData()?.countryCode
				},
				{
					"photon_version",
					_cachedConfig?.photonGameVersion ?? (-1)
				},
				{
					"photon_shard",
					MatchmakingGroup.MAIN
				},
				{ "photon_lang", num },
				{
					"account_type",
					GetAccountType()
				},
				{ "email", text },
				{
					"mp_in_room",
					PhotonNetwork.InRoom
				},
				{
					"mp_photon_room_name",
					PhotonNetwork.InRoom ? PhotonNetwork.CurrentRoom.Name : null
				},
				{
					"session_duration_ms",
					AnalyticStorage.GetSessionDurationMs()
				}
			};
		}

		private AnalyticAccountType GetAccountType()
		{
			return AnalyticsDependencies.Get<IAccountController>()?.AccountType switch
			{
				AccountType.Email => AnalyticAccountType.NORMAL, 
				AccountType.NewSystemGuest => AnalyticAccountType.GUEST, 
				AccountType.OldGuest => AnalyticAccountType.OLD_GUEST, 
				AccountType.OldAnonymous => AnalyticAccountType.OLD_ANONYMOUS, 
				_ => AnalyticAccountType.UNKNOWN, 
			};
		}

		private static Platform GetCustomPlatform()
		{
			switch (Application.platform)
			{
			case RuntimePlatform.IPhonePlayer:
				return Platform.IOS;
			case RuntimePlatform.Android:
				return Platform.ANDROID;
			case RuntimePlatform.WindowsPlayer:
			case RuntimePlatform.WindowsEditor:
			case RuntimePlatform.LinuxPlayer:
			case RuntimePlatform.LinuxEditor:
				return Platform.STEAM;
			case RuntimePlatform.Switch:
				return Platform.NINTENDO;
			default:
				return Platform.UNKNOWN;
			}
		}

		private InternetType GetInternetType()
		{
			return Application.internetReachability switch
			{
				NetworkReachability.ReachableViaLocalAreaNetwork => InternetType.WIFI, 
				NetworkReachability.ReachableViaCarrierDataNetwork => InternetType.MOBILE, 
				_ => InternetType.NONE, 
			};
		}

		private void LogEvent(Dictionary<string, object> dictionary)
		{
		}

		private string GetType(object value)
		{
			if (!(value is int))
			{
				if (!(value is long))
				{
					if (!(value is float))
					{
						if (!(value is double))
						{
							if (!(value is bool))
							{
								if (!(value is string))
								{
									if (!(value is DateTime))
									{
										if (value != null && value.GetType().IsEnum)
										{
											return "enum";
										}
										return "WRONG TYPE";
									}
									return "timestamp";
								}
								return "string";
							}
							return "boolean";
						}
						return "double";
					}
					return "float";
				}
				return "long";
			}
			return "int";
		}

		private async UniTask<bool> ProhibitionCheck(string eventName)
		{
			if (_cachedConfig == null)
			{
				if (!AnalyticsDependencies.TryGet<IRemoteConfigManager>(out var service))
				{
					return false;
				}
				_cachedConfig = await service.GetConfigValuesStorage();
				_cachedDisabledEvents = new HashSet<string>(_cachedConfig.analytics.disableEvents ?? new List<string>(), StringComparer.OrdinalIgnoreCase);
			}
			if (!_cachedConfig.analytics.enabled)
			{
				return false;
			}
			return !_cachedDisabledEvents.Contains(eventName);
		}
	}
}
