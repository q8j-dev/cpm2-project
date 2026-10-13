using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;

[ExecuteAlways]
[DefaultExecutionOrder(-50)]
public class WeatherController : MonoBehaviour
{
	private struct WeatherValues
	{
		public float mieScaleHeightKm;

		public float mieScattering;

		public float mieExtinction;

		public float miePhaseG;

		public float overcastAmount;

		public float overcastAltitudeKm;

		public float overcastThicknessKm;

		public float overcastMaxAbsorption;

		public float aerialPerspectiveDistanceScale;

		public float coverageMultiplier;

		public float cloudTypeBias;

		public float densityScale;

		public float precipDensityBoost;

		public Color ambientSky;

		public Color ambientEquator;

		public Color ambientGround;

		public float exposure;

		public float whiteBalance;

		public float rainAmount;

		public static WeatherValues FromPreset(WeatherPreset p, float sunTime01, float bodyTime01, bool moonActive, float rainAmount)
		{
			WeatherValues result = default;
			result.mieScaleHeightKm = p.mieScaleHeightKm;
			result.mieScattering = p.mieScattering;
			result.mieExtinction = p.mieExtinction;
			result.miePhaseG = p.miePhaseG;
			result.overcastAmount = p.overcastAmount;
			result.overcastAltitudeKm = p.overcastAltitudeKm;
			result.overcastThicknessKm = p.overcastThicknessKm;
			result.overcastMaxAbsorption = p.overcastMaxAbsorption;
			result.aerialPerspectiveDistanceScale = p.aerialPerspectiveDistanceScale;
			result.coverageMultiplier = p.coverageMultiplier;
			result.cloudTypeBias = p.cloudTypeBias;
			result.densityScale = p.densityScale;
			result.precipDensityBoost = p.precipDensityBoost;
			result.ambientSky = Opaque(p.ambientSkyColor.Evaluate(sunTime01));
			result.ambientEquator = Opaque(p.ambientEquatorColor.Evaluate(sunTime01));
			result.ambientGround = Opaque(p.ambientGroundColor.Evaluate(sunTime01));
			result.exposure = (moonActive ? Eval(p.exposureMoon, bodyTime01, 1f) : Eval(p.exposureSun, bodyTime01, 1f));
			result.whiteBalance = (moonActive ? Eval(p.whiteBalanceMoon, bodyTime01, 0f) : Eval(p.whiteBalanceSun, bodyTime01, 0f));
			result.rainAmount = Mathf.Clamp01(rainAmount);
			return result;
		}

		private static Color Opaque(Color c)
		{
			c.a = 1f;
			return c;
		}

		private static float Eval(AnimationCurve curve, float t, float fallback)
		{
			if (curve == null || curve.length <= 0)
			{
				return fallback;
			}
			return curve.Evaluate(t);
		}

		public static WeatherValues Lerp(in WeatherValues a, in WeatherValues b, float t)
		{
			WeatherValues result = default;
			result.mieScaleHeightKm = Mathf.Lerp(a.mieScaleHeightKm, b.mieScaleHeightKm, t);
			result.mieScattering = Mathf.Lerp(a.mieScattering, b.mieScattering, t);
			result.mieExtinction = Mathf.Lerp(a.mieExtinction, b.mieExtinction, t);
			result.miePhaseG = Mathf.Lerp(a.miePhaseG, b.miePhaseG, t);
			result.overcastAmount = Mathf.Lerp(a.overcastAmount, b.overcastAmount, t);
			result.overcastAltitudeKm = Mathf.Lerp(a.overcastAltitudeKm, b.overcastAltitudeKm, t);
			result.overcastThicknessKm = Mathf.Lerp(a.overcastThicknessKm, b.overcastThicknessKm, t);
			result.overcastMaxAbsorption = Mathf.Lerp(a.overcastMaxAbsorption, b.overcastMaxAbsorption, t);
			result.aerialPerspectiveDistanceScale = Mathf.Lerp(a.aerialPerspectiveDistanceScale, b.aerialPerspectiveDistanceScale, t);
			result.coverageMultiplier = Mathf.Lerp(a.coverageMultiplier, b.coverageMultiplier, t);
			result.cloudTypeBias = Mathf.Lerp(a.cloudTypeBias, b.cloudTypeBias, t);
			result.densityScale = Mathf.Lerp(a.densityScale, b.densityScale, t);
			result.precipDensityBoost = Mathf.Lerp(a.precipDensityBoost, b.precipDensityBoost, t);
			result.ambientSky = Color.Lerp(a.ambientSky, b.ambientSky, t);
			result.ambientEquator = Color.Lerp(a.ambientEquator, b.ambientEquator, t);
			result.ambientGround = Color.Lerp(a.ambientGround, b.ambientGround, t);
			result.exposure = Mathf.Lerp(a.exposure, b.exposure, t);
			result.whiteBalance = Mathf.Lerp(a.whiteBalance, b.whiteBalance, t);
			result.rainAmount = Mathf.Lerp(a.rainAmount, b.rainAmount, t);
			return result;
		}
	}

	[Tooltip("Sky atmosphere controller driven by the presets. Found automatically when empty.")]
	[Header("Targets")]
	public SkyAtmosphereController skyAtmosphere;

	[Tooltip("Cloud hemisphere raymarch material (Sky_CloudHemisphere). Defaults to the material assigned on the sky controller.")]
	public Material cloudMaterial;

	[Tooltip("Fallback Skybox Material. For use in fallback cameras whre we don't have a full atmospheric stack (Reflection, rear-view mirrors)")]
	public Material skyboxFallbackMaterial;

	[Header("Weather")]
	[Tooltip("Baked presets. Order matters: a preset without reflection cubemaps borrows the set of the closest LOWER index preset that has one.")]
	public List<WeatherPreset> presets = new List<WeatherPreset>();

	[Tooltip("Index of the preset to fade towards. Change it (or call SetWeather) to start a transition.")]
	[Min(0f)]
	public int selectedPreset;

	[Tooltip("Seconds a full transition takes. 0 = instant.")]
	[Min(0f)]
	public float fadeSeconds = 5f;

	[Header("Lighting")]
	[Tooltip("Drive RenderSettings ambient trilight (sky/equator/ground colors, sampled from the preset gradients at the current angular sun position).")]
	public bool driveAmbientLight = true;

	[Tooltip("Additional Ambient Light Color")]
	public Color additionalAmbientLight = Color.black;

	[Tooltip("Additional Ambient Light Contribution (Top, Equator, Bottom)")]
	public Vector3 additionalAmbientLightContribution = Vector3.one;

	[Range(0f, 1f)]
	[Tooltip("Additional Ambient Light Power")]
	public float additionalAmbientLightPower;

	[Range(0f, 2f)]
	[Tooltip("Fake bounce of the direct light added to the driven ambient: the ground color gains the light color tinted by the sky controller's Ground Albedo (strongest with a high sun), the equator ring gains the light color directly (strongest with a low sun). Follows the active body (moon at night) and uses the cloud-dimmed light. 0 = off.")]
	public float sunAmbientBouncePower = 1f;

	[Range(0f, 1f)]
	[Tooltip("Fake bounce of the direct light added to the driven ambient on the side")]
	public float sunAmbientBounceOnSide = 0.5f;

	[Range(0f, 2f)]
	[Tooltip("Scatters the light blocked by the sky controller's Cloud Light Dimming back into the driven ambient: the amount removed from the direct light is added, fully desaturated, to the sky and equator colors. 1 = the dimmed amount is compensated exactly. 0 = off.")]
	public float cloudScatterAmbientPower = 1f;

	[Tooltip("Drive RenderSettings.customReflectionTexture with the preset cubemaps.")]
	public bool driveReflection = true;

	[Tooltip("Optional global shader texture name that also receives the active reflection cubemap (e.g. _SkyboxProbe). Empty = skip.")]
	public string reflectionGlobalName = "";

	[Tooltip("Optional local shader texture name that also receives the active reflection cubemap. Empty = skip.")]
	public string FallbackSkyboxReflectionTextureName = "";

	public ExposureWBEvent onExposureWBSent;

	public RainAmountEvent onRainAmountSent = new RainAmountEvent();

	private readonly MaterialAssetStateGuard _materialGuard = new MaterialAssetStateGuard();

	private WeatherPreset activePreset;

	private int activeIndex = -1;

	private int reflectionSetIndex = -1;

	private WeatherValues fadeFrom;

	private WeatherValues current;

	private float fadeT = 1f;

	private bool snapRequested;

	private bool hasRainOverride;

	private float rainOverride;

	private uint requestRevision;

	private uint appliedRequestRevision;

	private float lastAppliedRain = float.NaN;

	private bool missingSkyLogged;

	private double lastEditorTime;

	private string cachedGlobalName;

	private string cachedLocalName;

	private int cachedGlobalId;

	private int cachedLocalId;

	[NonSerialized]
	private float debugDeltaTime;

	private ReflectionCubemapLoader reflectionLoader;

	private int prefetchSetKey;

	private float prefetchPrevT;

	private int prefetchDir;

	public WeatherPreset ActivePreset => activePreset;

	public WeatherPreset SelectedPreset
	{
		get
		{
			if (presets == null || selectedPreset < 0 || selectedPreset >= presets.Count)
			{
				return null;
			}
			return presets[selectedPreset];
		}
	}

	public int ActivePresetIndex => activeIndex;

	public float FadeProgress => fadeT;

	public Texture CurrentReflection { get; private set; }

	public bool CurrentReflectionIsMoon { get; private set; }

	public float CurrentExposure => current.exposure;

	public float CurrentWhiteBalance => current.whiteBalance;

	public float CurrentRainAmount => current.rainAmount;

	public WeatherPreset TargetPreset
	{
		get
		{
			if (presets == null || presets.Count == 0)
			{
				return null;
			}
			return presets[Mathf.Clamp(selectedPreset, 0, presets.Count - 1)];
		}
	}

	public float TargetRainAmount
	{
		get
		{
			WeatherPreset targetPreset = TargetPreset;
			if (!(targetPreset == null))
			{
				return Mathf.Clamp01(hasRainOverride ? rainOverride : targetPreset.rainAmount);
			}
			return 0f;
		}
	}

	public void SetWeather(int index, bool instant = false)
	{
		selectedPreset = Mathf.Max(0, index);
		hasRainOverride = false;
		snapRequested = instant;
		requestRevision++;
	}

	public void SetWeather(WeatherPreset preset, bool instant = false)
	{
		int num = ((presets != null) ? presets.IndexOf(preset) : (-1));
		if (num < 0)
		{
			Debug.LogWarning("WeatherController: preset '" + ((preset != null) ? preset.name : "null") + "' is not in the presets list.", this);
			return;
		}
		selectedPreset = num;
		hasRainOverride = false;
		snapRequested = instant;
		requestRevision++;
	}

	public void SetWeatherWithRainOverride(WeatherPreset preset, float intensity, bool instant = false)
	{
		int num = ((presets != null) ? presets.IndexOf(preset) : (-1));
		if (num < 0)
		{
			Debug.LogWarning("WeatherController: preset '" + ((preset != null) ? preset.name : "null") + "' is not in the presets list.", this);
			return;
		}
		selectedPreset = num;
		hasRainOverride = true;
		rainOverride = Mathf.Clamp01(intensity);
		snapRequested = instant;
		requestRevision++;
	}

	public void ClearRainOverride()
	{
		if (hasRainOverride)
		{
			hasRainOverride = false;
			requestRevision++;
		}
	}

	private void OnEnable()
	{
		_materialGuard.Capture(cloudMaterial);
		_materialGuard.Capture(skyboxFallbackMaterial);
		activePreset = null;
		activeIndex = -1;
		reflectionSetIndex = -1;
		fadeT = 1f;
		appliedRequestRevision = uint.MaxValue;
		lastAppliedRain = float.NaN;
		missingSkyLogged = false;
		lastEditorTime = 0.0;
		if (skyAtmosphere == null)
		{
			skyAtmosphere = GetComponent<SkyAtmosphereController>();
		}
		prefetchSetKey = 0;
		prefetchPrevT = -1f;
		prefetchDir = 0;
	}

	private void OnDisable()
	{
		reflectionLoader?.ReleaseTransient();
	}

	private void OnValidate()
	{
		if (skyAtmosphere == null)
		{
			skyAtmosphere = GetComponent<SkyAtmosphereController>();
		}
	}

	private void OnDestroy()
	{
		if (reflectionLoader != null)
		{
			Texture activeTexture = reflectionLoader.ActiveTexture;
			if (activeTexture != null)
			{
				if (RenderSettings.customReflectionTexture == activeTexture)
				{
					RenderSettings.customReflectionTexture = null;
					RenderSettings.defaultReflectionMode = DefaultReflectionMode.Skybox;
				}
				if (!string.IsNullOrEmpty(cachedGlobalName) && Shader.GetGlobalTexture(cachedGlobalId) == activeTexture)
				{
					Shader.SetGlobalTexture(cachedGlobalId, null);
				}
				if (skyboxFallbackMaterial != null && !string.IsNullOrEmpty(cachedLocalName) && skyboxFallbackMaterial.GetTexture(cachedLocalId) == activeTexture)
				{
					skyboxFallbackMaterial.SetTexture(cachedLocalId, null);
				}
				if (CurrentReflection == activeTexture)
				{
					CurrentReflection = null;
					CurrentReflectionIsMoon = false;
				}
			}
			reflectionLoader.ReleaseAll();
		}
		_materialGuard.Restore();
	}

	private void LateUpdate()
	{
		float num = TickDeltaTime();
		if (skyAtmosphere == null)
		{
			if (!missingSkyLogged)
			{
				missingSkyLogged = true;
				Debug.LogError("WeatherController requires SkyAtmosphereController on the same GameObject or an explicit reference.", this);
			}
		}
		else
		{
			if (presets == null || presets.Count == 0)
			{
				return;
			}
			int num2 = Mathf.Clamp(selectedPreset, 0, presets.Count - 1);
			WeatherPreset weatherPreset = presets[num2];
			if (weatherPreset == null)
			{
				return;
			}
			if (hasRainOverride && activeIndex >= 0 && num2 != activeIndex && appliedRequestRevision == requestRevision)
			{
				hasRainOverride = false;
				requestRevision++;
			}
			float num3 = CurrentSunTime01();
			bool flag = skyAtmosphere.moonLight != null && skyAtmosphere.MoonIsActive;
			float bodyTime = (flag ? WeatherPreset.SunTime01(0f - skyAtmosphere.moonLight.transform.forward.y) : num3);
			if (activePreset != weatherPreset || activeIndex != num2 || appliedRequestRevision != requestRevision)
			{
				bool flag2 = activePreset == null;
				fadeFrom = (flag2 ? WeatherValues.FromPreset(weatherPreset, num3, bodyTime, flag, EffectiveRain(weatherPreset)) : current);
				activePreset = weatherPreset;
				activeIndex = num2;
				fadeT = ((flag2 || snapRequested || fadeSeconds <= 0f) ? 1f : 0f);
				snapRequested = false;
				appliedRequestRevision = requestRevision;
			}
			else
			{
				if (snapRequested)
				{
					fadeT = 1f;
					snapRequested = false;
				}
				if (fadeT < 1f)
				{
					fadeT = ((fadeSeconds <= 0f) ? 1f : Mathf.Min(1f, fadeT + num / fadeSeconds));
				}
			}
			if (fadeT >= 0.5f)
			{
				reflectionSetIndex = activeIndex;
			}
			else if (reflectionSetIndex < 0)
			{
				reflectionSetIndex = activeIndex;
			}
			WeatherValues b = WeatherValues.FromPreset(activePreset, num3, bodyTime, flag, EffectiveRain(activePreset));
			current = ((fadeT >= 1f) ? b : WeatherValues.Lerp(in fadeFrom, in b, Mathf.SmoothStep(0f, 1f, fadeT)));
			ApplyValues(in current);
			ApplyReflection(num3);
		}
	}

	private float EffectiveRain(WeatherPreset preset)
	{
		if (!hasRainOverride)
		{
			return preset.rainAmount;
		}
		return rainOverride;
	}

	private float CurrentSunTime01()
	{
		return WeatherPreset.SunTime01((-skyAtmosphere.transform.forward).y);
	}

	private void ApplyValues(in WeatherValues v)
	{
		bool num = CanWriteSerializedTargets();
		if (num)
		{
			skyAtmosphere.mieScaleHeightKm = v.mieScaleHeightKm;
			skyAtmosphere.mieScattering = v.mieScattering;
			skyAtmosphere.mieExtinction = v.mieExtinction;
			skyAtmosphere.miePhaseG = v.miePhaseG;
			skyAtmosphere.overcastAmount = v.overcastAmount;
			skyAtmosphere.overcastAltitudeKm = v.overcastAltitudeKm;
			skyAtmosphere.overcastThicknessKm = v.overcastThicknessKm;
			skyAtmosphere.overcastMaxAbsorption = v.overcastMaxAbsorption;
			skyAtmosphere.aerialPerspectiveDistanceScale = v.aerialPerspectiveDistanceScale;
		}
		Material material = ((cloudMaterial != null) ? cloudMaterial : skyAtmosphere.cloudHemisphereMaterial);
		if (material != null)
		{
			material.SetFloat(CloudWeatherProps.CoverageMultiplier, v.coverageMultiplier);
			material.SetFloat(CloudWeatherProps.CloudTypeBias, v.cloudTypeBias);
			material.SetFloat(CloudWeatherProps.DensityScale, v.densityScale);
			material.SetFloat(CloudWeatherProps.PrecipDensityBoost, v.precipDensityBoost);
		}
		if (driveAmbientLight)
		{
			Color sky = v.ambientSky;
			Color equator = v.ambientEquator;
			Color ground = v.ambientGround;
			AddSunAmbientContributions(ref sky, ref equator, ref ground);
			AddAdditionalAmbientContributions(ref sky, ref equator, ref ground);
			RenderSettings.ambientMode = AmbientMode.Trilight;
			RenderSettings.ambientSkyColor = sky.gamma;
			RenderSettings.ambientEquatorColor = equator.gamma;
			RenderSettings.ambientGroundColor = ground.gamma;
		}
		if (num && (float.IsNaN(lastAppliedRain) || Mathf.Abs(lastAppliedRain - v.rainAmount) >= 0.0005f || (fadeT >= 1f && lastAppliedRain != v.rainAmount)))
		{
			lastAppliedRain = v.rainAmount;
			onRainAmountSent.Invoke(v.rainAmount);
		}
		onExposureWBSent.Invoke(new Vector2(v.exposure, v.whiteBalance));
	}

	private bool CanWriteSerializedTargets()
	{
		return true;
	}

	private void AddSunAmbientContributions(ref Color sky, ref Color equator, ref Color ground)
	{
		if (sunAmbientBouncePower <= 0f && cloudScatterAmbientPower <= 0f)
		{
			return;
		}
		int num;
		Light light;
		if (skyAtmosphere.moonLight != null)
		{
			num = (skyAtmosphere.MoonIsActive ? 1 : 0);
			if (num != 0)
			{
				light = skyAtmosphere.moonLight;
				goto IL_0057;
			}
		}
		else
		{
			num = 0;
		}
		light = skyAtmosphere.SunLight;
		goto IL_0057;
		IL_0057:
		Light light2 = light;
		Vector3 vector = ((num != 0) ? (-skyAtmosphere.moonLight.transform.forward) : (-skyAtmosphere.transform.forward));
		Color linear = ((num != 0) ? skyAtmosphere.spaceMoonColor : skyAtmosphere.spaceSunColor).linear;
		float num2 = ((light2 != null) ? light2.intensity : 1f);
		Color color = linear * skyAtmosphere.SampleSunTransmittance(vector.y) * num2;
		float num3 = Mathf.Clamp01(skyAtmosphere.CloudLightOcclusion * skyAtmosphere.cloudLightDimming);
		float num4 = Mathf.Max(0f, vector.y);
		float num5 = Mathf.Sqrt(Mathf.Max(0f, 1f - vector.y * vector.y)) * sunAmbientBounceOnSide;
		Color color2 = color * (1f - num3);
		ground += color2 * skyAtmosphere.groundAlbedo.linear * (sunAmbientBouncePower * num4);
		equator += color2 * (sunAmbientBouncePower * num5);
		float num6 = Luminance(color) * num3 * cloudScatterAmbientPower;
		sky += new Color(num6 * num4, num6 * num4, num6 * num4);
		equator += new Color(num6 * num5, num6 * num5, num6 * num5);
		sky.a = 1f;
		equator.a = 1f;
		ground.a = 1f;
	}

	private void AddAdditionalAmbientContributions(ref Color sky, ref Color equator, ref Color ground)
	{
		if (!(additionalAmbientLightPower <= 0f))
		{
			sky += additionalAmbientLight * additionalAmbientLightContribution.x * additionalAmbientLightPower;
			equator += additionalAmbientLight * additionalAmbientLightContribution.y * additionalAmbientLightPower;
			ground += additionalAmbientLight * additionalAmbientLightContribution.z * additionalAmbientLightPower;
			sky.a = 1f;
			equator.a = 1f;
			ground.a = 1f;
		}
	}

	private static float Luminance(Color c)
	{
		return 0.2126f * c.r + 0.7152f * c.g + 0.0722f * c.b;
	}

	private void ApplyReflection(float sunTime)
	{
		if (!driveReflection)
		{
			return;
		}
		bool moonUp = false;
		float num = 0f;
		Light moonLight = skyAtmosphere.moonLight;
		float num2 = ((moonLight != null) ? (0f - moonLight.transform.forward.y) : (-1f));
		if (moonLight != null && skyAtmosphere.MoonIsActive && num2 > 0f)
		{
			moonUp = true;
			num = WeatherPreset.MoonTime01(num2);
		}
		if (!TryPickReflectionSet(reflectionSetIndex, moonUp, out var setPreset, out var fromMoon))
		{
			return;
		}
		Texture texture = ResolveReflection(setPreset, fromMoon, fromMoon ? num : sunTime, out var isRequested);
		PrefetchUpcomingReflection(sunTime, moonUp, num, num2, setPreset, fromMoon);
		if (texture == null)
		{
			return;
		}
		RenderSettings.defaultReflectionMode = DefaultReflectionMode.Custom;
		if (RenderSettings.customReflectionTexture != texture)
		{
			RenderSettings.customReflectionTexture = texture;
		}
		CurrentReflection = texture;
		if (isRequested)
		{
			CurrentReflectionIsMoon = fromMoon;
		}
		if (!string.IsNullOrEmpty(reflectionGlobalName))
		{
			if (cachedGlobalName != reflectionGlobalName)
			{
				cachedGlobalName = reflectionGlobalName;
				cachedGlobalId = Shader.PropertyToID(reflectionGlobalName);
			}
			Shader.SetGlobalTexture(cachedGlobalId, texture);
		}
		if (!string.IsNullOrEmpty(FallbackSkyboxReflectionTextureName) && skyboxFallbackMaterial != null)
		{
			if (cachedLocalName != FallbackSkyboxReflectionTextureName)
			{
				cachedLocalName = FallbackSkyboxReflectionTextureName;
				cachedLocalId = Shader.PropertyToID(FallbackSkyboxReflectionTextureName);
			}
			skyboxFallbackMaterial.SetTexture(cachedLocalId, texture);
		}
	}

	private bool TryPickReflectionSet(int setIndex, bool moonUp, out WeatherPreset setPreset, out bool fromMoon)
	{
		for (int num = Mathf.Min(setIndex, presets.Count - 1); num >= 0; num--)
		{
			WeatherPreset weatherPreset = presets[num];
			if (!(weatherPreset == null))
			{
				if (moonUp && (weatherPreset.HasMoonReflectionCubemapRefs || weatherPreset.HasMoonReflectionCubemaps))
				{
					setPreset = weatherPreset;
					fromMoon = true;
					return true;
				}
				if (weatherPreset.HasReflectionCubemapRefs || weatherPreset.HasReflectionCubemaps)
				{
					setPreset = weatherPreset;
					fromMoon = false;
					return true;
				}
			}
		}
		setPreset = null;
		fromMoon = false;
		return false;
	}

	private Texture ResolveReflection(WeatherPreset p, bool fromMoon, float t, out bool isRequested)
	{
		if (fromMoon ? p.HasMoonReflectionCubemapRefs : p.HasReflectionCubemapRefs)
		{
			AssetReferenceT<Cubemap> reference = (fromMoon ? p.GetMoonReflectionCubemapRef(t) : p.GetReflectionCubemapRef(t));
			if (reflectionLoader == null)
			{
				reflectionLoader = new ReflectionCubemapLoader();
			}
			return reflectionLoader.Request(reference, out isRequested);
		}
		Cubemap obj = (fromMoon ? p.GetMoonReflectionCubemap(t) : p.GetReflectionCubemap(t));
		if (obj != null)
		{
			reflectionLoader?.ReleaseActive();
		}
		isRequested = true;
		return obj;
	}

	private void PrefetchUpcomingReflection(float sunTime, bool moonUp, float moonTime, float sinMoonElevation, WeatherPreset currentSetPreset, bool currentFromMoon)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (fadeT < 0.5f && reflectionSetIndex != activeIndex && TryPickReflectionSet(activeIndex, moonUp, out var setPreset, out var fromMoon))
		{
			PrefetchFromSet(setPreset, fromMoon, fromMoon ? moonTime : sunTime);
			return;
		}
		if (skyAtmosphere.moonLight != null)
		{
			float num = 0f - skyAtmosphere.transform.forward.y;
			float num2 = Mathf.Sin(skyAtmosphere.moonSwitchSunElevationDegrees * ((float)Math.PI / 180f));
			bool num3 = skyAtmosphere.lightSource == SkyAtmosphereController.CelestialSource.Auto && sinMoonElevation > 0f && Mathf.Abs(num - num2) < 0.03f;
			bool flag = skyAtmosphere.MoonIsActive && Mathf.Abs(sinMoonElevation) < 0.05f;
			if ((num3 | flag) && TryPickReflectionSet(reflectionSetIndex, !moonUp, out var setPreset2, out var fromMoon2))
			{
				PrefetchFromSet(setPreset2, fromMoon2, fromMoon2 ? WeatherPreset.MoonTime01(sinMoonElevation) : sunTime);
				return;
			}
		}
		if (!(currentFromMoon ? currentSetPreset.HasMoonReflectionCubemapRefs : currentSetPreset.HasReflectionCubemapRefs))
		{
			return;
		}
		List<AssetReferenceT<Cubemap>> list = (currentFromMoon ? currentSetPreset.moonReflectionCubemapRefs : currentSetPreset.reflectionCubemapRefs);
		float num4 = (currentFromMoon ? moonTime : sunTime);
		int num5 = currentSetPreset.GetInstanceID() * 2 + (currentFromMoon ? 1 : 0);
		if (num5 != prefetchSetKey)
		{
			prefetchSetKey = num5;
			prefetchPrevT = num4;
			prefetchDir = 0;
			return;
		}
		if (Mathf.Abs(num4 - prefetchPrevT) > 0.0001f)
		{
			prefetchDir = ((num4 > prefetchPrevT) ? 1 : (-1));
			prefetchPrevT = num4;
		}
		if (prefetchDir == 0)
		{
			return;
		}
		for (int i = WeatherPreset.NearestRefIndex(list, num4) + prefetchDir; i >= 0 && i < list.Count; i += prefetchDir)
		{
			if (WeatherPreset.IsValidRef(list[i]))
			{
				if (reflectionLoader == null)
				{
					reflectionLoader = new ReflectionCubemapLoader();
				}
				reflectionLoader.Prefetch(list[i]);
				break;
			}
		}
	}

	private void PrefetchFromSet(WeatherPreset p, bool fromMoon, float t)
	{
		if (!(fromMoon ? p.HasMoonReflectionCubemapRefs : p.HasReflectionCubemapRefs))
		{
			return;
		}
		AssetReferenceT<Cubemap> assetReferenceT = (fromMoon ? p.GetMoonReflectionCubemapRef(t) : p.GetReflectionCubemapRef(t));
		if (assetReferenceT != null)
		{
			if (reflectionLoader == null)
			{
				reflectionLoader = new ReflectionCubemapLoader();
			}
			reflectionLoader.Prefetch(assetReferenceT);
		}
	}

	private float TickDeltaTime()
	{
		if (debugDeltaTime > 0f)
		{
			return debugDeltaTime;
		}
		_ = Application.isPlaying;
		return Time.deltaTime;
	}
}
