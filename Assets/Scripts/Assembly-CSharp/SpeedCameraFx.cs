using UnityEngine;
using _Car_Parking.Scripts;

public static class SpeedCameraFx
{
	private const float DefaultMaxFovAdd = 25f;

	private const float DefaultStartKmh = 60f;

	private const float DefaultMaxKmh = 200f;

	public static float Speed01(float speedMps, SteamSpeedCameraConfig cfg)
	{
		float num = (cfg ? cfg.StartSpeedKmh : 60f);
		float num2 = (cfg ? cfg.SpeedAtMaxKmh : 200f);
		return Mathf.Clamp01((Mathf.Abs(speedMps) * 3.6f - num) / Mathf.Max(1f, num2 - num));
	}

	public static float FovAdd(float speedMps, SteamSpeedCameraConfig cfg)
	{
		if ((bool)cfg && !cfg.EnableFov)
		{
			return 0f;
		}
		return (cfg ? cfg.MaxFovAdd : 25f) * Speed01(speedMps, cfg);
	}
}
