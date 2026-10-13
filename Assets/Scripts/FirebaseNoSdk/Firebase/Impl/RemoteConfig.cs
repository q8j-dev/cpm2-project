using System;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine;
using UnityEngine.Networking;

namespace Firebase.Impl
{
	public class RemoteConfig : FirebaseRemoteConfig
	{
		private static readonly TimeSpan FetchInterval = TimeSpan.FromHours(8.0);

		private static readonly int[] AttemptTimeoutsSeconds = new int[3] { 15, 30, 60 };

		private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2.0);

		private string _value;

		private const string Project = "cpm2";

		private const string Env = "prod";

		private const string Platform = "steam";

		private static long NextTicks
		{
			get
			{
				return long.Parse(PlayerPrefs.GetString("NextTicks", "0"));
			}
			set
			{
				PlayerPrefs.SetString("NextTicks", value.ToString());
			}
		}

		private static string CachedValue
		{
			get
			{
				return PlayerPrefs.GetString("_RemoteCache_", "");
			}
			set
			{
				PlayerPrefs.SetString("_RemoteCache_", value);
			}
		}

		private static string CachedVersion
		{
			get
			{
				return PlayerPrefs.GetString("_RemoteCache_CachedVersion_", "");
			}
			set
			{
				PlayerPrefs.SetString("_RemoteCache_CachedVersion_", value);
			}
		}

		public override async UniTask FetchAsync()
		{
			string text = Application.version.Replace(".", "_");
			string combinedVersion = "cpm2/prod/steam/" + text;
			long ticks = DateTime.Now.Ticks;
			if (Math.Abs(NextTicks - ticks) > TimeSpan.FromDays(1.0).Ticks)
			{
				NextTicks = 0L;
			}
			if (CachedVersion != combinedVersion)
			{
				NextTicks = 0L;
			}
			if (NextTicks > ticks)
			{
				_value = CachedValue;
				return;
			}
			TimeSpan fetchInterval = FetchInterval;
			NextTicks = ticks + fetchInterval.Ticks;
			string uri = "https://olzhas1.b-cdn.net/configs/" + combinedVersion + ".json";
			for (int attempt = 0; attempt < AttemptTimeoutsSeconds.Length; attempt++)
			{
				int timeout = AttemptTimeoutsSeconds[attempt];
				using UnityWebRequest request = UnityWebRequest.Get(uri);
				request.timeout = timeout;
				request.SendWebRequest();
				while (!request.isDone)
				{
					await UniTask.Delay(TimeSpan.FromSeconds(0.10000000149011612), DelayType.UnscaledDeltaTime);
				}
				if (request.result == UnityWebRequest.Result.Success)
				{
					_ = CachedVersion != combinedVersion;
					_value = request.downloadHandler.text;
					CachedValue = _value;
					CachedVersion = combinedVersion;
					break;
				}
				if (attempt < AttemptTimeoutsSeconds.Length - 1)
				{
					await UniTask.Delay(RetryDelay, DelayType.UnscaledDeltaTime);
				}
			}
		}

		public override string GetRaw()
		{
			return _value;
		}
	}
}
