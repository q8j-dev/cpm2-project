using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.Storage;
using Newtonsoft.Json;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Analyzers;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Scopes;

namespace _Car_Parking.Scripts.GooglePlayServices
{
	public static class GooglePlayAchievements
	{
		private const string CleanSlateMaskKey = "gpg_clean_slate_mask";

		private const string PendingUnlocksKey = "gpg_pending_unlocks";

		private const string DonePrefix = "gpg_done_";

		private const string LastUserKey = "gpg_last_user";

		private const string P2pBoughtCarsKey = "gpg_p2p_bought_cars";

		private const string PaintMaterialCarsKey = "gpg_paint_mat_cars";

		private const string PaintMaterialsMaskPrefix = "gpg_paint_mats_";

		private const char PendingSourceSeparator = '|';

		private const int CleanSlateParkingFlag = 1;

		private const int CleanSlateDragFlag = 2;

		private const int CleanSlateCircuitFlag = 4;

		private const int CleanSlateRallyFlag = 8;

		private const int CleanSlateAllFlags = 15;

		private static readonly (string Id, string Key, int Target, bool IsFloat)[] Incrementals = new (string, string, int, bool)[7]
		{
			("CgkIuKqW0PcZEAIQFQ", "gpg_ach_free_drive_km", 25, true),
			("CgkIuKqW0PcZEAIQFg", "gpg_ach_speed_banners", 8, false),
			("CgkIuKqW0PcZEAIQFw", "gpg_ach_offroad_km", 5, true),
			("CgkIuKqW0PcZEAIQHw", "gpg_ach_cars_acquired", 2, false),
			("CgkIuKqW0PcZEAIQIw", "gpg_ach_passenger_km", 10, true),
			("CgkIuKqW0PcZEAIQKg", "gpg_ach_taxi_completed", 10, false),
			("CgkIuKqW0PcZEAIQLA", "gpg_ach_dyno_pulls", 10, false)
		};

		private static readonly string[] ProgressKeys = new string[9] { "gpg_ach_free_drive_km", "gpg_ach_speed_banners", "gpg_ach_offroad_km", "gpg_ach_taxi_completed", "gpg_ach_dyno_pulls", "gpg_ach_cars_acquired", "gpg_ach_passenger_km", "gpg_postcard_env_mask", "gpg_garage_classes_mask" };

		private static readonly HashSet<string> Completed = new HashSet<string>();

		private static readonly Dictionary<string, int> MaskCache = new Dictionary<string, int>();

		private static readonly HashSet<string> SkipLogged = new HashSet<string>();

		private static readonly HashSet<string> InFlight = new HashSet<string>();

		private static readonly HashSet<string> InFlightSteps = new HashSet<string>();

		private static readonly HashSet<string> FailedFinalSteps = new HashSet<string>();

		private static IPlayGamesAchievementsService _service;

		private static AchievementAnalytic _analytic;

		private static string _platformPlayerId;

		private static bool _isSignedIn;

		private static bool _notSignedInLogged;

		private static Dictionary<string, string> _pendingUnlocks;

		private static HashSet<string> _p2pBoughtCarInstanceIds;

		private static HashSet<string> _paintMaterialCarInstanceIds;

		private static HashSet<string> P2pBoughtCarInstanceIds => _p2pBoughtCarInstanceIds ?? (_p2pBoughtCarInstanceIds = LoadCarInstanceIds("gpg_p2p_bought_cars"));

		private static HashSet<string> PaintMaterialCarInstanceIds => _paintMaterialCarInstanceIds ?? (_paintMaterialCarInstanceIds = LoadCarInstanceIds("gpg_paint_mat_cars"));

		private static bool IsSupportedPlatform => false;

		private static bool IsAnalyticsEnabled => false;

		private static Dictionary<string, string> PendingUnlocks => _pendingUnlocks ?? (_pendingUnlocks = LoadPendingUnlocks());

		public static void Unlock(string achievementId, string source = null)
		{
			if (!IsSupportedPlatform)
			{
				return;
			}
			if (_isSignedIn)
			{
				RetryFailedFinalSteps();
			}
			if (IsCompleted(achievementId))
			{
				LogSkip(achievementId, source);
				return;
			}
			if (!PendingUnlocks.ContainsKey(achievementId))
			{
				PendingUnlocks[achievementId] = source;
				SavePendingUnlocks();
			}
			SendUnlock(achievementId, source, isRetry: false);
		}

		public static void SyncCompletedFromServer(Action onDone)
		{
			if (!IsSupportedPlatform)
			{
				onDone?.Invoke();
				return;
			}
			try
			{
				IPlayGamesAchievementsService playGamesAchievementsService = ResolveService();
				ApplyUserScope(playGamesAchievementsService.GetUserId());
				playGamesAchievementsService.LoadState((IReadOnlyList<AchievementServerState> states) =>
				{
					if (states != null)
					{
						foreach (AchievementServerState state in states)
						{
							LogSyncedState(state);
							if (state.Completed)
							{
								MarkCompleted(state.Id);
							}
							else
							{
								RestoreServerProgress(state);
							}
						}
					}
					onDone?.Invoke();
				});
			}
			catch (Exception)
			{
				_service = null;
				onDone?.Invoke();
			}
		}

		private static void LogSyncedState(AchievementServerState state)
		{
			AchievementResult? result;
			if (state.Completed)
			{
				string id = state.Id;
				result = AchievementResult.COMPLETED;
				Log(AchievementStatus.SYNCED_FROM_PLATFORM, id, null, null, result);
				return;
			}
			if (state.PercentCompleted <= 0.0)
			{
				string id2 = state.Id;
				result = AchievementResult.NOT_STARTED;
				Log(AchievementStatus.SYNCED_FROM_PLATFORM, id2, null, null, result);
				return;
			}
			if (!TryGetIncrementalTarget(state.Id, out var target))
			{
				string id3 = state.Id;
				result = AchievementResult.IN_PROGRESS;
				Log(AchievementStatus.SYNCED_FROM_PLATFORM, id3, null, null, result);
				return;
			}
			int num = (int)Math.Round(state.PercentCompleted / 100.0 * (double)target);
			if (num > target)
			{
				num = target;
			}
			string id4 = state.Id;
			result = AchievementResult.IN_PROGRESS;
			int? progressCurrent = num;
			int? progressTarget = target;
			Log(AchievementStatus.SYNCED_FROM_PLATFORM, id4, null, null, result, progressCurrent, progressTarget);
		}

		private static bool TryGetIncrementalTarget(string achievementId, out int target)
		{
			(string, string, int, bool)[] incrementals = Incrementals;
			for (int i = 0; i < incrementals.Length; i++)
			{
				(string, string, int, bool) tuple = incrementals[i];
				if (!(tuple.Item1 != achievementId))
				{
					target = tuple.Item3;
					return true;
				}
			}
			target = 0;
			return false;
		}

		private static void ApplyUserScope(string userId)
		{
			_platformPlayerId = (string.IsNullOrEmpty(userId) ? null : userId);
			if (string.IsNullOrEmpty(userId))
			{
				return;
			}
			try
			{
				string text = ObscuredPrefs.GetString("gpg_last_user", string.Empty);
				if (!(text == userId))
				{
					if (!string.IsNullOrEmpty(text))
					{
						ClearLocalState();
						LogAccountSwitched(text);
					}
					ObscuredPrefs.SetString("gpg_last_user", userId);
				}
			}
			catch (Exception)
			{
			}
		}

		private static void LogAccountSwitched(string previousUserId)
		{
			string properties = SerializeProperties("prev_player_id", previousUserId);
			Log(AchievementStatus.ACCOUNT_SWITCHED, null, null, null, null, null, null, isRetry: false, null, properties);
		}

		private static void ClearLocalState()
		{
			Completed.Clear();
			MaskCache.Clear();
			SkipLogged.Clear();
			InFlight.Clear();
			_notSignedInLogged = false;
			_pendingUnlocks = null;
			_p2pBoughtCarInstanceIds = null;
			foreach (string item in GPGSIdNames.AllIds())
			{
				ObscuredPrefs.DeleteKey("gpg_done_" + item);
			}
			string[] progressKeys = ProgressKeys;
			for (int i = 0; i < progressKeys.Length; i++)
			{
				ObscuredPrefs.DeleteKey(progressKeys[i]);
			}
			foreach (string paintMaterialCarInstanceId in PaintMaterialCarInstanceIds)
			{
				ObscuredPrefs.DeleteKey("gpg_paint_mats_" + paintMaterialCarInstanceId);
			}
			ObscuredPrefs.DeleteKey("gpg_clean_slate_mask");
			ObscuredPrefs.DeleteKey("gpg_pending_unlocks");
			ObscuredPrefs.DeleteKey("gpg_p2p_bought_cars");
			ObscuredPrefs.DeleteKey("gpg_paint_mat_cars");
			_paintMaterialCarInstanceIds = null;
		}

		private static void RestoreServerProgress(AchievementServerState state)
		{
			(string, string, int, bool)[] incrementals = Incrementals;
			for (int i = 0; i < incrementals.Length; i++)
			{
				var (text, key, num, flag) = incrementals[i];
				if (text != state.Id)
				{
					continue;
				}
				int num2 = (int)Math.Round(state.PercentCompleted / 100.0 * (double)num);
				if (num2 <= 0)
				{
					break;
				}
				if (num2 > num)
				{
					num2 = num;
				}
				try
				{
					if (flag)
					{
						if (ObscuredPrefs.GetFloat(key) < (float)num2)
						{
							ObscuredPrefs.SetFloat(key, num2);
						}
					}
					else if (ObscuredPrefs.GetInt(key) < num2)
					{
						ObscuredPrefs.SetInt(key, num2);
					}
					break;
				}
				catch (Exception)
				{
					break;
				}
			}
		}

		public static void FlushPending()
		{
			if (!IsSupportedPlatform)
			{
				return;
			}
			foreach (KeyValuePair<string, string> item in new List<KeyValuePair<string, string>>(PendingUnlocks))
			{
				if (IsCompleted(item.Key))
				{
					PendingUnlocks.Remove(item.Key);
				}
				else
				{
					SendUnlock(item.Key, item.Value, isRetry: true);
				}
			}
			SavePendingUnlocks();
		}

		public static void SetSignedIn()
		{
			if (IsSupportedPlatform)
			{
				_isSignedIn = true;
				_notSignedInLogged = false;
			}
		}

		public static void SetSignedOut()
		{
			if (IsSupportedPlatform)
			{
				_isSignedIn = false;
			}
		}

		public static void OnSignedIn()
		{
			if (IsSupportedPlatform)
			{
				FailedFinalSteps.Clear();
				FlushPending();
				ResendIncrementals();
			}
		}

		public static void ReportProgress(string achievementId, string progressKey, int target, int increment, string source = null)
		{
			if (!IsSupportedPlatform)
			{
				return;
			}
			try
			{
				if (IsCompleted(achievementId))
				{
					LogSkip(achievementId, source);
					return;
				}
				int num = ObscuredPrefs.GetInt(progressKey);
				if (num >= target)
				{
					SendSteps(achievementId, num, target, isRetry: false, source);
					return;
				}
				int num2 = num + increment;
				if (num2 > target)
				{
					num2 = target;
				}
				ObscuredPrefs.SetInt(progressKey, num2);
				SendSteps(achievementId, num2, target, isRetry: false, source);
			}
			catch (Exception)
			{
			}
		}

		public static void ReportProgress(string achievementId, string progressKey, float target, float increment, string source = null)
		{
			if (!IsSupportedPlatform)
			{
				return;
			}
			try
			{
				if (IsCompleted(achievementId))
				{
					LogSkip(achievementId, source);
					return;
				}
				float num = ObscuredPrefs.GetFloat(progressKey);
				if (num >= target)
				{
					SendSteps(achievementId, (int)num, (int)target, isRetry: false, source);
					return;
				}
				float num2 = num + increment;
				if (num2 > target)
				{
					num2 = target;
				}
				ObscuredPrefs.SetFloat(progressKey, num2);
				SendSteps(achievementId, (int)num2, (int)target, isRetry: false, source);
			}
			catch (Exception)
			{
			}
		}

		public static void ReportMaskProgress(string achievementId, string maskKey, int bitIndex, int target, string source = null, string properties = null)
		{
			if (!IsSupportedPlatform || bitIndex < 0 || bitIndex >= 31)
			{
				return;
			}
			if (IsCompleted(achievementId))
			{
				LogSkip(achievementId, source);
				return;
			}
			try
			{
				if (!MaskCache.TryGetValue(maskKey, out var value))
				{
					value = ObscuredPrefs.GetInt(maskKey);
					MaskCache[maskKey] = value;
				}
				int num = value | (1 << bitIndex);
				if (num != value && CountSetBits(value) < target)
				{
					MaskCache[maskKey] = num;
					ObscuredPrefs.SetInt(maskKey, num);
					int num2 = CountSetBits(num);
					AchievementResult? result = AchievementResult.LOCAL;
					int? progressCurrent = num2;
					int? progressTarget = target;
					Log(AchievementStatus.PROGRESS_UPDATED, achievementId, source, null, result, progressCurrent, progressTarget, isRetry: false, null, properties);
					if (num2 >= target)
					{
						Unlock(achievementId, source);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		public static void MarkCleanSlateParking(string source = null)
		{
			MarkCleanSlate(1, source);
		}

		public static void MarkCleanSlateDrag(string source = null)
		{
			MarkCleanSlate(2, source);
		}

		public static void MarkCleanSlateCircuit(string source = null)
		{
			MarkCleanSlate(4, source);
		}

		public static void MarkCleanSlateRally(string source = null)
		{
			MarkCleanSlate(8, source);
		}

		public static void MarkP2pCarBought(string carInstanceId)
		{
			if (IsSupportedPlatform && !string.IsNullOrEmpty(carInstanceId) && P2pBoughtCarInstanceIds.Add(carInstanceId))
			{
				SaveCarInstanceIds("gpg_p2p_bought_cars", P2pBoughtCarInstanceIds);
			}
		}

		public static void ReportPaintMaterial(string carInstanceId, int matTypeIndex, string source = null)
		{
			if (!IsSupportedPlatform || string.IsNullOrEmpty(carInstanceId))
			{
				return;
			}
			if (IsCompleted("CgkIuKqW0PcZEAIQHQ"))
			{
				LogSkip("CgkIuKqW0PcZEAIQHQ", source);
				return;
			}
			if (PaintMaterialCarInstanceIds.Add(carInstanceId))
			{
				SaveCarInstanceIds("gpg_paint_mat_cars", PaintMaterialCarInstanceIds);
			}
			ReportMaskProgress("CgkIuKqW0PcZEAIQHQ", "gpg_paint_mats_" + carInstanceId, matTypeIndex, 3, source, SerializeProperties("car_instance_id", carInstanceId));
		}

		private static void SaveCarInstanceIds(string key, HashSet<string> carInstanceIds)
		{
			try
			{
				ObscuredPrefs.SetString(key, string.Join(",", carInstanceIds));
			}
			catch (Exception)
			{
			}
		}

		private static HashSet<string> LoadCarInstanceIds(string key)
		{
			HashSet<string> hashSet = new HashSet<string>();
			try
			{
				string text = ObscuredPrefs.GetString(key, string.Empty);
				if (string.IsNullOrEmpty(text))
				{
					return hashSet;
				}
				string[] array = text.Split(',');
				foreach (string text2 in array)
				{
					if (!string.IsNullOrEmpty(text2))
					{
						hashSet.Add(text2);
					}
				}
			}
			catch (Exception)
			{
			}
			return hashSet;
		}

		public static void TryUnlockDealerLuck(string carInstanceId, string source = null)
		{
			if (IsSupportedPlatform && !string.IsNullOrEmpty(carInstanceId) && P2pBoughtCarInstanceIds.Contains(carInstanceId))
			{
				Unlock("CgkIuKqW0PcZEAIQOg", source);
			}
		}

		private static void LogSkip(string achievementId, string source)
		{
			if (SkipLogged.Add(achievementId))
			{
				Log(AchievementStatus.ALREADY_UNLOCKED, achievementId, source);
			}
		}

		private static bool IsNotSignedIn(UnlockResult result)
		{
			if (!result.Success)
			{
				return result.FailReason == AchievementFailReason.NOT_SIGNED_IN;
			}
			return false;
		}

		private static bool ShouldLogNotSignedIn()
		{
			if (_notSignedInLogged)
			{
				return false;
			}
			_notSignedInLogged = true;
			return true;
		}

		private static void SendUnlock(string achievementId, string source, bool isRetry)
		{
			if (!InFlight.Add(achievementId))
			{
				return;
			}
			if (!_notSignedInLogged)
			{
				string achievementId2 = achievementId;
				string source2 = source;
				bool isRetry2 = isRetry;
				Log(AchievementStatus.UNLOCK_TRIGGERED, achievementId2, source2, null, null, null, null, isRetry2);
			}
			try
			{
				ResolveService().Unlock(achievementId, (UnlockResult result) =>
				{
					InFlight.Remove(achievementId);
					if (IsNotSignedIn(result))
					{
						if (ShouldLogNotSignedIn())
						{
							string achievementId4 = achievementId;
							string source4 = source;
							AchievementFailReason? failReason2 = result.FailReason;
							bool isRetry3 = isRetry;
							Log(AchievementStatus.UNLOCK_FAILED, achievementId4, source4, failReason2, null, null, null, isRetry3);
						}
					}
					else
					{
						_notSignedInLogged = false;
						if (!result.Success)
						{
							string achievementId5 = achievementId;
							string source5 = source;
							AchievementFailReason? failReason3 = result.FailReason;
							bool isRetry3 = isRetry;
							Log(AchievementStatus.UNLOCK_FAILED, achievementId5, source5, failReason3, null, null, null, isRetry3);
						}
						else
						{
							string achievementId6 = achievementId;
							string source6 = source;
							bool isRetry3 = isRetry;
							Log(AchievementStatus.UNLOCKED, achievementId6, source6, null, null, null, null, isRetry3);
							MarkCompleted(achievementId);
							if (PendingUnlocks.Remove(achievementId))
							{
								SavePendingUnlocks();
							}
						}
					}
				});
			}
			catch (Exception ex)
			{
				InFlight.Remove(achievementId);
				_service = null;
				string achievementId3 = achievementId;
				string source3 = source;
				AchievementFailReason? failReason = AchievementFailReason.API_ERROR;
				bool isRetry2 = isRetry;
				string errorMessage = ex.ToString();
				Log(AchievementStatus.UNLOCK_FAILED, achievementId3, source3, failReason, null, null, null, isRetry2, errorMessage);
			}
		}

		private static void SendSteps(string achievementId, int steps, int target, bool isRetry, string source)
		{
			if (steps <= 0 || !InFlightSteps.Add(achievementId))
			{
				return;
			}
			try
			{
				ResolveService().ReportProgress(achievementId, steps, (UnlockResult result) =>
				{
					InFlightSteps.Remove(achievementId);
					bool num = IsNotSignedIn(result);
					if (!num)
					{
						_notSignedInLogged = false;
					}
					if (!num || ShouldLogNotSignedIn())
					{
						string achievementId2 = achievementId;
						string source2 = source;
						AchievementResult? result2 = ((!result.Success) ? AchievementResult.FAILED : AchievementResult.SUCCESS);
						Log(AchievementStatus.PROGRESS_UPDATED, achievementId2, source2, result.FailReason, result2, steps, target, isRetry);
					}
					if (steps >= target)
					{
						if (!result.Success)
						{
							FailedFinalSteps.Add(achievementId);
						}
						else
						{
							FailedFinalSteps.Remove(achievementId);
							MarkCompleted(achievementId);
						}
					}
				});
			}
			catch (Exception ex)
			{
				InFlightSteps.Remove(achievementId);
				_service = null;
				Log(AchievementStatus.PROGRESS_UPDATED, achievementId, source, result: AchievementResult.FAILED, failReason: AchievementFailReason.API_ERROR, progressCurrent: steps, progressTarget: target, isRetry: isRetry, errorMessage: ex.ToString());
			}
		}

		private static void RetryFailedFinalSteps()
		{
			if (FailedFinalSteps.Count == 0)
			{
				return;
			}
			(string, string, int, bool)[] incrementals = Incrementals;
			for (int i = 0; i < incrementals.Length; i++)
			{
				var (text, key, num, flag) = incrementals[i];
				if (!FailedFinalSteps.Contains(text) || IsCompleted(text))
				{
					continue;
				}
				try
				{
					int num2 = (flag ? ((int)ObscuredPrefs.GetFloat(key)) : ObscuredPrefs.GetInt(key));
					if (num2 > num)
					{
						num2 = num;
					}
					SendSteps(text, num2, num, isRetry: true, null);
				}
				catch (Exception)
				{
				}
			}
		}

		private static void ResendIncrementals()
		{
			(string, string, int, bool)[] incrementals = Incrementals;
			for (int i = 0; i < incrementals.Length; i++)
			{
				var (achievementId, key, num, flag) = incrementals[i];
				if (!IsCompleted(achievementId))
				{
					int num2;
					try
					{
						num2 = (flag ? ((int)ObscuredPrefs.GetFloat(key)) : ObscuredPrefs.GetInt(key));
					}
					catch (Exception)
					{
						continue;
					}
					if (num2 > num)
					{
						num2 = num;
					}
					SendSteps(achievementId, num2, num, isRetry: true, null);
				}
			}
		}

		private static IPlayGamesAchievementsService ResolveService()
		{
			return _service ?? (_service = LifetimeScope.Find<A_PersistentScope>().Container.Resolve<IPlayGamesAchievementsService>());
		}

		private static AchievementAnalytic ResolveAnalytic()
		{
			try
			{
				return _analytic ?? (_analytic = LifetimeScope.Find<A_PersistentScope>().Container.Resolve<AchievementAnalytic>());
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static void Log(AchievementStatus status, string achievementId, string source = null, AchievementFailReason? failReason = null, AchievementResult? result = null, int? progressCurrent = null, int? progressTarget = null, bool isRetry = false, string errorMessage = null, string properties = null)
		{
			if (IsAnalyticsEnabled)
			{
				ResolveAnalytic()?.Log(new AchievementStatusContext
				{
					Status = status,
					Service = AchievementService.PLAY_GAMES,
					AchievementId = GPGSIdNames.CanonicalKeyOf(achievementId),
					PlatformPlayerId = _platformPlayerId,
					FailReason = failReason,
					Result = result,
					ProgressCurrent = progressCurrent,
					ProgressTarget = progressTarget,
					Source = source,
					IsRetry = (isRetry ? new bool?(true) : ((bool?)null)),
					ErrorMessage = errorMessage,
					Properties = properties
				});
			}
		}

		private static string SerializeProperties(string key, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				return null;
			}
			try
			{
				return JsonConvert.SerializeObject(new Dictionary<string, string> { { key, value } });
			}
			catch (Exception)
			{
				return null;
			}
		}

		private static void MarkCleanSlate(int flag, string source)
		{
			if (!IsSupportedPlatform)
			{
				return;
			}
			try
			{
				if (!IsCompleted("CgkIuKqW0PcZEAIQOw"))
				{
					int num = ObscuredPrefs.GetInt("gpg_clean_slate_mask") | flag;
					ObscuredPrefs.SetInt("gpg_clean_slate_mask", num);
					if ((num & 0xF) == 15)
					{
						Unlock("CgkIuKqW0PcZEAIQOw", source);
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private static bool IsCompleted(string achievementId)
		{
			if (Completed.Contains(achievementId))
			{
				return true;
			}
			try
			{
				if (!ObscuredPrefs.GetBool("gpg_done_" + achievementId))
				{
					return false;
				}
			}
			catch (Exception)
			{
				return false;
			}
			Completed.Add(achievementId);
			return true;
		}

		private static void MarkCompleted(string achievementId)
		{
			if (!Completed.Add(achievementId))
			{
				return;
			}
			try
			{
				ObscuredPrefs.SetBool("gpg_done_" + achievementId, value: true);
			}
			catch (Exception)
			{
			}
		}

		private static int CountSetBits(int mask)
		{
			uint num = (uint)mask;
			int num2 = 0;
			while (num != 0)
			{
				num2 += (int)(num & 1);
				num >>= 1;
			}
			return num2;
		}

		private static void SavePendingUnlocks()
		{
			try
			{
				List<string> list = new List<string>(PendingUnlocks.Count);
				foreach (KeyValuePair<string, string> pendingUnlock in PendingUnlocks)
				{
					list.Add(string.IsNullOrEmpty(pendingUnlock.Value) ? pendingUnlock.Key : (pendingUnlock.Key + "|" + pendingUnlock.Value));
				}
				ObscuredPrefs.SetString("gpg_pending_unlocks", string.Join(",", list));
			}
			catch (Exception)
			{
			}
		}

		private static Dictionary<string, string> LoadPendingUnlocks()
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			try
			{
				string text = ObscuredPrefs.GetString("gpg_pending_unlocks", string.Empty);
				if (string.IsNullOrEmpty(text))
				{
					return dictionary;
				}
				string[] array = text.Split(',');
				foreach (string text2 in array)
				{
					if (string.IsNullOrEmpty(text2))
					{
						continue;
					}
					int num = text2.IndexOf('|');
					if (num < 0)
					{
						dictionary[text2] = null;
						continue;
					}
					string text3 = text2.Substring(0, num);
					if (!string.IsNullOrEmpty(text3))
					{
						dictionary[text3] = text2.Substring(num + 1);
					}
				}
			}
			catch (Exception)
			{
			}
			return dictionary;
		}
	}
}
