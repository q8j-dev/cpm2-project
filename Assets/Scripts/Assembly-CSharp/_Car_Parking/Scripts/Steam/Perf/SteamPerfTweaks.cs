using Traffic;
using UnityEngine;

namespace _Car_Parking.Scripts.Steam.Perf
{
	public static class SteamPerfTweaks
	{
		private const float MaxDeltaTime = 0.1f;

		private static float _defaultMaxDeltaTime = -1f;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static void Init()
		{
			_defaultMaxDeltaTime = Time.maximumDeltaTime;
			ApplyMaxDelta();
			SteamPerfFlags.Changed += OnFlagsChanged;
		}

		public static float SteamLodBias(float presetValue, int quality)
		{
			if (!SteamPerfFlags.IsOn("lodBias"))
			{
				return presetValue;
			}
			return Mathf.Min(presetValue, quality switch
			{
				0 => 1f, 
				1 => 1.5f, 
				2 => 2f, 
				3 => 2.5f, 
				_ => 3f, 
			});
		}

		private static void OnFlagsChanged()
		{
			ApplyMaxDelta();
			PeopleTraffic instance = PeopleTraffic.Instance;
			if ((bool)instance)
			{
				instance.SteamRefreshAvoidance();
			}
			CPMGraphicsSettingsManager instance2 = CPMGraphicsSettingsManager.Instance;
			if ((bool)instance2 && instance2._graphicsSettings != null)
			{
				instance2.ObjectsLodQuality = instance2.ObjectsLodQuality;
			}
		}

		private static void ApplyMaxDelta()
		{
			if (_defaultMaxDeltaTime > 0f)
			{
				Time.maximumDeltaTime = (SteamPerfFlags.IsOn("maxDelta") ? 0.1f : _defaultMaxDeltaTime);
			}
		}
	}
}
