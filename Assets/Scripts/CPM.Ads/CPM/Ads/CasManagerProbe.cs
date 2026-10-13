using System;
using System.Text;
using CAS;
using UnityEngine;

namespace CPM.Ads
{
	public static class CasManagerProbe
	{
		public struct Report
		{
			public bool ManagerExistedBefore;

			public string UserIdBefore;

			public string FirstManagerId;

			public string UserIdAfterFirst;

			public string FirstError;

			public string SecondManagerId;

			public string UserIdAfterSecond;

			public string SecondError;

			public bool SameInstance;

			public bool CacheReleased;

			public string ThirdManagerId;

			public string UserIdAfterThird;

			public string ThirdError;

			public bool ThirdIsNewInstance;

			public bool RebuildAfterReleaseTookEffect => UserIdAfterThird == "cas_probe_user_C";

			public bool NativeReadAvailable
			{
				get
				{
					if (UserIdAfterSecond != null)
					{
						return !UserIdAfterSecond.StartsWith("<");
					}
					return false;
				}
			}

			private bool NativeReadAvailableAfterThird
			{
				get
				{
					if (UserIdAfterThird != null)
					{
						return !UserIdAfterThird.StartsWith("<");
					}
					return false;
				}
			}

			public bool SecondBuildTookEffect => UserIdAfterSecond == "cas_probe_user_B";

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendLine("========== [CasProbe] BuildManager twice with different user ids ==========");
				stringBuilder.AppendLine("manager existed before probe : " + ManagerExistedBefore);
				stringBuilder.AppendLine("native userID before         : " + Quote(UserIdBefore));
				stringBuilder.AppendLine("--- build 1 with cas_probe_user_A");
				stringBuilder.AppendLine("  managerID                  : " + Quote(FirstManagerId));
				stringBuilder.AppendLine("  native userID after        : " + Quote(UserIdAfterFirst));
				if (!string.IsNullOrEmpty(FirstError))
				{
					stringBuilder.AppendLine("  build threw                : " + FirstError);
				}
				stringBuilder.AppendLine("--- build 2 with cas_probe_user_B");
				stringBuilder.AppendLine("  managerID                  : " + Quote(SecondManagerId));
				stringBuilder.AppendLine("  native userID after        : " + Quote(UserIdAfterSecond));
				if (!string.IsNullOrEmpty(SecondError))
				{
					stringBuilder.AppendLine("  build threw                : " + SecondError);
				}
				stringBuilder.AppendLine("--- build 3 with cas_probe_user_C, after releasing CAS's cached manager");
				stringBuilder.AppendLine("  cache released               : " + CacheReleased);
				if (CacheReleased)
				{
					stringBuilder.AppendLine("  managerID                  : " + Quote(ThirdManagerId));
					stringBuilder.AppendLine("  new instance               : " + ThirdIsNewInstance);
					stringBuilder.AppendLine("  native userID after        : " + Quote(UserIdAfterThird));
					if (!string.IsNullOrEmpty(ThirdError))
					{
						stringBuilder.AppendLine("  build threw                : " + ThirdError);
					}
				}
				stringBuilder.AppendLine("--- verdict");
				stringBuilder.AppendLine("  same manager instance      : " + SameInstance + (SameInstance ? "  (CAS returned the cached manager)" : "  (a new manager was built)"));
				if (NativeReadAvailable)
				{
					stringBuilder.AppendLine("  2nd user id reached native : " + SecondBuildTookEffect);
					stringBuilder.AppendLine("  => rebuilding to change the user id " + (SecondBuildTookEffect ? "WORKS" : "DOES NOT WORK") + " on this platform");
				}
				else
				{
					stringBuilder.AppendLine("  2nd user id reached native : unknown — no native getter here");
					stringBuilder.AppendLine("  => run this on a device for the native half. The C# half says " + (SameInstance ? "the second build was a cache hit, so Init() never re-ran." : "a second manager really was constructed."));
				}
				if (CacheReleased)
				{
					stringBuilder.AppendLine("  release+rebuild            : " + ((!NativeReadAvailableAfterThird) ? ("native id unknown here; new C# instance: " + ThirdIsNewInstance) : (RebuildAfterReleaseTookEffect ? "WORKS" : "DID NOT change the native id")));
				}
				stringBuilder.Append("==========================================================================");
				return stringBuilder.ToString();
			}

			private static string Quote(string value)
			{
				if (value == null)
				{
					return "<null>";
				}
				if (value.Length != 0)
				{
					return "\"" + value + "\"";
				}
				return "<empty>";
			}
		}

		public const string UserIdA = "cas_probe_user_A";

		public const string UserIdB = "cas_probe_user_B";

		public const string UserIdC = "cas_probe_user_C";

		public static Report Run()
		{
			Report result = new Report
			{
				ManagerExistedBefore = ManagerExists(),
				UserIdBefore = ReadNativeUserId()
			};
			IMediationManager mediationManager = Build("cas_probe_user_A", out var error);
			result.FirstError = error;
			result.FirstManagerId = mediationManager?.managerID;
			result.UserIdAfterFirst = ReadNativeUserId();
			IMediationManager mediationManager2 = Build("cas_probe_user_B", out var error2);
			result.SecondError = error2;
			result.SecondManagerId = mediationManager2?.managerID;
			result.UserIdAfterSecond = ReadNativeUserId();
			result.SameInstance = mediationManager != null && mediationManager == mediationManager2;
			result.CacheReleased = CasManagerReset.TryReleaseCachedManagers();
			if (result.CacheReleased)
			{
				IMediationManager mediationManager3 = Build("cas_probe_user_C", out var error3);
				result.ThirdError = error3;
				result.ThirdManagerId = mediationManager3?.managerID;
				result.UserIdAfterThird = ReadNativeUserId();
				result.ThirdIsNewInstance = mediationManager3 != null && mediationManager2 != mediationManager3;
			}
			Debug.Log(result.ToString());
			return result;
		}

		private static IMediationManager Build(string userId, out string error)
		{
			error = null;
			try
			{
				return MobileAds.BuildManager().WithUserID(userId).WithCompletionListener((InitialConfiguration config) =>
				{
					Debug.Log("[CasProbe] init listener fired for " + userId + ", error: " + (string.IsNullOrEmpty(config.error) ? "<none>" : config.error));
				})
					.Build();
			}
			catch (Exception ex)
			{
				error = ex.Message;
				Debug.LogException(ex);
				return null;
			}
		}

		private static bool ManagerExists()
		{
			try
			{
				return MobileAds.manager != null;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static string ReadNativeUserId()
		{
			return "<no native getter in the editor>";
		}
	}
}
