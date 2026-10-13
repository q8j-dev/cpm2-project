using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "WeatherPreset", menuName = "SkyAtmosphere/Weather Preset")]
public class WeatherPreset : ScriptableObject
{
	[Header("Sky Atmosphere - Mie / Fog")]
	public float mieScaleHeightKm = 1.2f;

	[Range(0f, 0.5f)]
	[Tooltip("Mie scattering in 1/km. Clear ~0.004, hazy 0.02-0.05, foggy 0.1+.")]
	public float mieScattering = 0.003996f;

	[Range(0f, 0.5f)]
	[Tooltip("Mie extinction in 1/km (kept >= scattering by the sky controller).")]
	public float mieExtinction = 0.00444f;

	[Range(0f, 0.999f)]
	public float miePhaseG = 0.8f;

	[Header("Sky Atmosphere - Overcast")]
	[Range(0f, 1f)]
	public float overcastAmount;

	public float overcastAltitudeKm = 5.5f;

	[Min(0.1f)]
	public float overcastThicknessKm = 2f;

	[Min(0f)]
	public float overcastMaxAbsorption = 2.5f;

	[Header("Sky Atmosphere - Aerial Perspective")]
	[Range(0.1f, 10f)]
	public float aerialPerspectiveDistanceScale = 1f;

	[Header("Cloud Hemisphere Material")]
	public float coverageMultiplier = 1f;

	[Range(-1f, 1f)]
	public float cloudTypeBias;

	public float densityScale = 1f;

	public float precipDensityBoost = 2.5f;

	[Header("Lighting - Reflections")]
	[Tooltip("Streamed (addressable) environment reflection cubemaps ordered by angular sun position: index 0 = sun at nadir, last = sun at zenith, middle = horizon (denser near the horizon, see SunTime01). Loaded on demand by the WeatherController so only the active cubemap stays in RAM. Filled by the baker; presets baked before streaming existed convert via SkyAtmosphere > Migrate Weather Presets To Streamed Cubemaps. Leave empty (along with the direct list) to fall back to the cubemaps of a lower-index preset in the WeatherController list.")]
	public List<AssetReferenceT<Cubemap>> reflectionCubemapRefs = new List<AssetReferenceT<Cubemap>>();

	[Tooltip("Streamed (addressable) moon-lit cubemaps covering above-horizon moon angles only: index 0 = moon at the horizon, last = moon at zenith (denser near the horizon, see MoonTime01). Used while the moon lights the sky and is up; when it sets (or the preset has no moon set) the sun cubemaps take over - at night the sun indexes its darkest captures.")]
	public List<AssetReferenceT<Cubemap>> moonReflectionCubemapRefs = new List<AssetReferenceT<Cubemap>>();

	[Tooltip("Direct (always-resident) sun cubemap references - the pre-streaming storage, kept for un-migrated presets and in-memory cubemaps. Ignored while the streamed list above has any entry.")]
	public List<Cubemap> reflectionCubemaps = new List<Cubemap>();

	[Tooltip("Direct (always-resident) moon cubemap references. Ignored while the streamed moon list above has any entry.")]
	public List<Cubemap> moonReflectionCubemaps = new List<Cubemap>();

	[GradientUsage(true)]
	[Tooltip("Ambient sky color over angular sun position (0 = nadir, 0.5 = horizon, 1 = zenith).")]
	[Header("Lighting - Ambient (trilight)")]
	public Gradient ambientSkyColor = new Gradient();

	[GradientUsage(true)]
	public Gradient ambientEquatorColor = new Gradient();

	[GradientUsage(true)]
	public Gradient ambientGroundColor = new Gradient();

	[Tooltip("Exposure over angular sun position (0 = nadir, 0.5 = horizon, 1 = zenith - same warped axis as the ambient gradients). Blended by WeatherController and exposed as CurrentExposure; applying it (e.g. to a post volume) is up to your own script.")]
	[Header("Lighting - Exposure / White Balance (hand-authored)")]
	public AnimationCurve exposureSun = AnimationCurve.Constant(0f, 1f, 1f);

	[Tooltip("Exposure over angular MOON position, used instead of Exposure Sun while the moon lights the sky (SkyAtmosphereController.MoonIsActive).")]
	public AnimationCurve exposureMoon = AnimationCurve.Constant(0f, 1f, 1f);

	[Tooltip("White balance over angular sun position. Units are yours (e.g. URP White Balance temperature). Exposed as CurrentWhiteBalance.")]
	public AnimationCurve whiteBalanceSun = AnimationCurve.Constant(0f, 1f, 0f);

	[Tooltip("White balance over angular moon position, used while the moon lights the sky.")]
	public AnimationCurve whiteBalanceMoon = AnimationCurve.Constant(0f, 1f, 0f);

	[Header("Rain Settings")]
	[Range(0f, 1f)]
	[Tooltip("Placeholder for the future rain system. Blended by WeatherController (CurrentRainAmount) but not applied to anything.")]
	public float rainAmount;

	public bool HasReflectionCubemaps => HasAny(reflectionCubemaps);

	public bool HasMoonReflectionCubemaps => HasAny(moonReflectionCubemaps);

	public bool HasReflectionCubemapRefs => HasAnyRef(reflectionCubemapRefs);

	public bool HasMoonReflectionCubemapRefs => HasAnyRef(moonReflectionCubemapRefs);

	public static float SunTime01(float sinElevation)
	{
		float num = Mathf.Clamp(sinElevation, -1f, 1f);
		return Mathf.Clamp01(((num >= 0f) ? Mathf.Sqrt(num) : (0f - Mathf.Sqrt(0f - num))) * 0.5f + 0.5f);
	}

	public static float SinElevationFromSunTime(float t)
	{
		float num = Mathf.Clamp01(t) * 2f - 1f;
		if (!(num >= 0f))
		{
			return 0f - num * num;
		}
		return num * num;
	}

	public static float MoonTime01(float sinElevation)
	{
		return Mathf.Sqrt(Mathf.Clamp01(sinElevation));
	}

	public Cubemap GetReflectionCubemap(float sunTime01)
	{
		return Nearest(reflectionCubemaps, sunTime01);
	}

	public Cubemap GetMoonReflectionCubemap(float moonTime01)
	{
		return Nearest(moonReflectionCubemaps, moonTime01);
	}

	public AssetReferenceT<Cubemap> GetReflectionCubemapRef(float sunTime01)
	{
		int num = NearestRefIndex(reflectionCubemapRefs, sunTime01);
		if (num < 0)
		{
			return null;
		}
		return reflectionCubemapRefs[num];
	}

	public AssetReferenceT<Cubemap> GetMoonReflectionCubemapRef(float moonTime01)
	{
		int num = NearestRefIndex(moonReflectionCubemapRefs, moonTime01);
		if (num < 0)
		{
			return null;
		}
		return moonReflectionCubemapRefs[num];
	}

	public static bool IsValidRef(AssetReferenceT<Cubemap> reference)
	{
		return reference?.RuntimeKeyIsValid() ?? false;
	}

	public static int NearestRefIndex(List<AssetReferenceT<Cubemap>> refs, float t01)
	{
		if (refs == null || refs.Count == 0)
		{
			return -1;
		}
		int count = refs.Count;
		int num = Mathf.Clamp(Mathf.RoundToInt(Mathf.Clamp01(t01) * (float)(count - 1)), 0, count - 1);
		for (int i = 0; i < count; i++)
		{
			int num2 = num - i;
			int num3 = num + i;
			if (num2 >= 0 && IsValidRef(refs[num2]))
			{
				return num2;
			}
			if (num3 < count && IsValidRef(refs[num3]))
			{
				return num3;
			}
		}
		return -1;
	}

	private static bool HasAnyRef(List<AssetReferenceT<Cubemap>> refs)
	{
		if (refs == null)
		{
			return false;
		}
		for (int i = 0; i < refs.Count; i++)
		{
			if (IsValidRef(refs[i]))
			{
				return true;
			}
		}
		return false;
	}

	private static bool HasAny(List<Cubemap> cubemaps)
	{
		if (cubemaps == null)
		{
			return false;
		}
		for (int i = 0; i < cubemaps.Count; i++)
		{
			if (cubemaps[i] != null)
			{
				return true;
			}
		}
		return false;
	}

	private static Cubemap Nearest(List<Cubemap> cubemaps, float t01)
	{
		if (cubemaps == null || cubemaps.Count == 0)
		{
			return null;
		}
		int count = cubemaps.Count;
		int num = Mathf.Clamp(Mathf.RoundToInt(Mathf.Clamp01(t01) * (float)(count - 1)), 0, count - 1);
		for (int i = 0; i < count; i++)
		{
			int num2 = num - i;
			int num3 = num + i;
			if (num2 >= 0 && cubemaps[num2] != null)
			{
				return cubemaps[num2];
			}
			if (num3 < count && cubemaps[num3] != null)
			{
				return cubemaps[num3];
			}
		}
		return null;
	}
}
