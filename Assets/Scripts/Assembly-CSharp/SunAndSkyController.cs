using System;
using System.Collections.Generic;
using CPM.NightLighting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Other.Helpers;

[ExecuteAlways]
[RequireComponent(typeof(Light), typeof(SkyAtmosphereController), typeof(WeatherController))]
[DefaultExecutionOrder(100)]
public sealed class SunAndSkyController : MonoBehaviour
{
	private const float StormThreshold = 0.8f;

	private const float ValueEpsilon = 0.0005f;

	private static readonly int GlobalAmbientEquator = Shader.PropertyToID("_GlobalAmbientEquator");

	private static readonly int WindDirection = Shader.PropertyToID("_WindDirection");

	private static readonly int CloudSpeed = Shader.PropertyToID("_CloudSpeed");

	[SerializeField]
	[Header("New sky and weather")]
	private SkyAtmosphereController skyAtmosphere;

	[SerializeField]
	private WeatherController weatherController;

	[SerializeField]
	private RainWetnessDriver rainWetness;

	[SerializeField]
	private Material cloudMaterial;

	[SerializeField]
	private LensFlareComponentSRP lensFlare;

	[SerializeField]
	[Header("Named presets")]
	private WeatherPreset clearPreset;

	[SerializeField]
	private WeatherPreset hazedPreset;

	[SerializeField]
	private WeatherPreset cloudyPreset;

	[SerializeField]
	private WeatherPreset overcastPreset;

	[SerializeField]
	private WeatherPreset rainyPreset;

	[SerializeField]
	private WeatherPreset stormyPreset;

	[Range(0f, 10f)]
	[SerializeField]
	[FormerlySerializedAs("CloudSequenceSlider")]
	[Header("Compatibility controls")]
	private float cloudiness = 1f;

	[SerializeField]
	[FormerlySerializedAs("UseDynamicNoise")]
	private bool useDynamicWeather;

	[Min(1f)]
	[SerializeField]
	private float dynamicWeatherInterval = 15f;

	[SerializeField]
	[FormerlySerializedAs("CloudsRotation")]
	private float windDirectionDegrees;

	[SerializeField]
	[Min(0f)]
	private float windSpeedMetersPerSecond = 8f;

	[FormerlySerializedAs("AmbientNightCompensation")]
	[Range(0f, 1f)]
	[SerializeField]
	private float ambientNightCompensation;

	[FormerlySerializedAs("IgnoreThunderSound")]
	[SerializeField]
	private bool ignoreThunderSound;

	private readonly List<DecalLight> decalLights = new List<DecalLight>(15);

	private Light sunLight;

	private WeatherPreset lastDryPreset;

	private Vector3 lastSunForward = new Vector3(float.NaN, float.NaN, float.NaN);

	private float lastWindDirection = float.NaN;

	private float lastWindSpeed = float.NaN;

	private float lastExposure = float.NaN;

	private Color lastGlobalAmbient = new Color(float.NaN, 0f, 0f, 0f);

	private float lastFlareIntensity = float.NaN;

	private float lastThunderRtpc = float.NaN;

	private bool nightInitialized;

	private bool thunderPlaying;

	private bool missingReferencesLogged;

	private GameObject thunderTarget;

	private float legacyStarsRotation;

	public float CloudSequenceSlider
	{
		get
		{
			return cloudiness;
		}
		set
		{
			ApplyCloudiness(value, instant: false);
		}
	}

	public float CurrentCloudiness => cloudiness;

	public float CurrentCloudLevel => cloudiness;

	public bool UseDynamicNoise
	{
		get
		{
			return useDynamicWeather;
		}
		set
		{
			useDynamicWeather = value;
		}
	}

	public float HorizonAngle { get; private set; }

	public bool IsNight { get; private set; }

	public bool IgnoreThunderSound
	{
		get
		{
			return ignoreThunderSound;
		}
		set
		{
			if (ignoreThunderSound != value)
			{
				ignoreThunderSound = value;
				UpdateThunder(force: true);
			}
		}
	}

	public float CloudsRotation
	{
		get
		{
			return windDirectionDegrees;
		}
		set
		{
			SetCloudWind(value, windSpeedMetersPerSecond);
		}
	}

	public float StarsRotation
	{
		get
		{
			return legacyStarsRotation;
		}
		set
		{
			legacyStarsRotation = value;
		}
	}

	public string CurrentWeatherPresetId
	{
		get
		{
			if (weatherController == null)
			{
				return null;
			}
			WeatherPreset weatherPreset = ((weatherController.SelectedPreset != null) ? weatherController.SelectedPreset : weatherController.ActivePreset);
			if (weatherPreset == null)
			{
				return null;
			}
			if (weatherPreset == clearPreset)
			{
				return "clear";
			}
			if (weatherPreset == hazedPreset)
			{
				return "hazed";
			}
			if (weatherPreset == cloudyPreset)
			{
				return "cloudy";
			}
			if (weatherPreset == overcastPreset)
			{
				return "overcast";
			}
			if (weatherPreset == rainyPreset)
			{
				return "rainy";
			}
			if (weatherPreset == stormyPreset)
			{
				return "stormy";
			}
			return null;
		}
	}

	public event Action<bool> OnDayTimeChanged;

	private void Awake()
	{
		ResolveReferences();
	}

	private void OnEnable()
	{
		ResolveReferences();
		missingReferencesLogged = false;
		if (ValidateRequiredReferences())
		{
			GlobalEventSystem.OnSunControllerRequested -= RequestInstance;
			GlobalEventSystem.OnSunControllerRequested += RequestInstance;
			ConfigureControllers();
			SubscribeRainDelivery();
			RefreshThunderTarget();
			nightInitialized = false;
			lastSunForward = new Vector3(float.NaN, float.NaN, float.NaN);
			lastWindDirection = (lastWindSpeed = float.NaN);
			lastExposure = float.NaN;
			lastGlobalAmbient.r = float.NaN;
			lastFlareIntensity = float.NaN;
			lastThunderRtpc = float.NaN;
			thunderPlaying = false;
			if (!Application.isPlaying || !RestoreSavedWeatherPreset())
			{
				ApplyCloudiness(cloudiness, instant: true);
			}
			SynchronizeOutputs(force: true);
		}
	}

	private void OnDisable()
	{
		GlobalEventSystem.OnSunControllerRequested -= RequestInstance;
		UnsubscribeRainDelivery();
		StopThunder();
	}

	private void OnDestroy()
	{
		GlobalEventSystem.OnSunControllerRequested -= RequestInstance;
		UnsubscribeRainDelivery();
		StopThunder();
	}

	private void OnValidate()
	{
		dynamicWeatherInterval = Mathf.Max(1f, dynamicWeatherInterval);
		windSpeedMetersPerSecond = Mathf.Max(0f, windSpeedMetersPerSecond);
		cloudiness = Mathf.Clamp(cloudiness, 0f, 10f);
		ResolveReferences();
		ConfigureControllers();
		lastWindDirection = (lastWindSpeed = float.NaN);
	}

	private void LateUpdate()
	{
		if (ValidateRequiredReferences())
		{
			SynchronizeOutputs(force: false);
		}
	}

	private void Update()
	{
		if (skyAtmosphere != null)
		{
			AlignMoon(force: false);
		}
	}

	public void SetWeather(WeatherPreset preset, bool instant = false)
	{
		if (!ValidateRequiredReferences() || preset == null)
		{
			if (preset == null)
			{
				Debug.LogWarning("SunAndSkyController rejected a null weather preset.", this);
			}
			return;
		}
		if (preset.rainAmount <= 0.0005f)
		{
			lastDryPreset = preset;
		}
		weatherController.SetWeather(preset, instant);
	}

	public void SetWeather(Weather weather, bool instant = false)
	{
		cloudiness = weather switch
		{
			Weather.Clear => 0f, 
			Weather.Cloudy => 4f, 
			Weather.Overcast => 9f, 
			Weather.Fog => 7f, 
			_ => cloudiness, 
		};
		SetDryWeather(weather switch
		{
			Weather.Clear => clearPreset, 
			Weather.Cloudy => cloudyPreset, 
			Weather.Overcast => overcastPreset, 
			Weather.Fog => hazedPreset, 
			_ => clearPreset, 
		}, instant, weather.ToString());
	}

	public WeatherPreset GetNamedPreset(string presetId)
	{
		if (string.IsNullOrEmpty(presetId))
		{
			return null;
		}
		return presetId.ToLowerInvariant() switch
		{
			"clear" => clearPreset, 
			"hazed" => hazedPreset, 
			"cloudy" => cloudyPreset, 
			"overcast" => overcastPreset, 
			"rainy" => rainyPreset, 
			"stormy" => stormyPreset, 
			_ => null, 
		};
	}

	public bool SetWeatherByName(string presetId, bool instant = false)
	{
		WeatherPreset namedPreset = GetNamedPreset(presetId);
		if (namedPreset == null)
		{
			Debug.LogWarning("SunAndSkyController has no weather preset for id '" + presetId + "'.", this);
			return false;
		}
		switch (presetId.ToLowerInvariant())
		{
		case "clear":
			cloudiness = 0f;
			break;
		case "cloudy":
			cloudiness = 4f;
			break;
		case "hazed":
			cloudiness = 7f;
			break;
		case "overcast":
			cloudiness = 9f;
			break;
		}
		SetWeather(namedPreset, instant);
		return true;
	}

	private bool RestoreSavedWeatherPreset()
	{
		if (!ValidateRequiredReferences())
		{
			return false;
		}
		string text = PlayerPrefs.GetString("weatherPreset", "");
		if (string.IsNullOrEmpty(text))
		{
			text = MigrateLegacySliderPrefs();
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			PlayerPrefs.SetString("weatherPreset", text);
		}
		WeatherPreset namedPreset = GetNamedPreset(text);
		if (namedPreset == null)
		{
			return false;
		}
		if (namedPreset.rainAmount > 0.0005f && !ScenesHelper.IsFreeDriveSceneActive())
		{
			return false;
		}
		return SetWeatherByName(text, instant: true);
	}

	private static string MigrateLegacySliderPrefs()
	{
		float num = PlayerPrefs.GetFloat("rainIntensity", 0f) * 0.1f;
		if (num >= 0.8f)
		{
			return "stormy";
		}
		if (num > 0.0005f)
		{
			return "rainy";
		}
		CPMGraphicsSettingsManager instance = CPMGraphicsSettingsManager.Instance;
		if (instance == null || !instance.ConfigLoaded)
		{
			return null;
		}
		float num2 = instance.Cloudiness;
		if (num2 < 2.5f)
		{
			return "clear";
		}
		if (!(num2 < 7.5f))
		{
			return "overcast";
		}
		return "cloudy";
	}

	public void SetRainIntensity(float intensity, bool instant = false)
	{
		if (!ValidateRequiredReferences())
		{
			return;
		}
		float num = Mathf.Clamp01(intensity);
		if (num <= 0.0005f)
		{
			WeatherPreset weatherPreset = ((lastDryPreset != null) ? lastDryPreset : DryPresetForCloudiness(cloudiness));
			if (weatherPreset == null)
			{
				weatherPreset = clearPreset;
			}
			SetDryWeather(weatherPreset, instant, "rain off");
		}
		else
		{
			RememberCurrentDryPreset();
			WeatherPreset weatherPreset2 = ((num >= 0.8f) ? stormyPreset : rainyPreset);
			if (weatherPreset2 == null)
			{
				Debug.LogWarning($"SunAndSkyController cannot apply rain {num:0.##}: the matching wet preset is not assigned.", this);
			}
			else
			{
				weatherController.SetWeatherWithRainOverride(weatherPreset2, num, instant);
			}
		}
	}

	public void SetCloudWind(float directionDegrees, float speedMetersPerSecond)
	{
		windDirectionDegrees = Mathf.Repeat(directionDegrees, 360f);
		windSpeedMetersPerSecond = Mathf.Max(0f, speedMetersPerSecond);
		ApplyWind(force: false);
	}

	public void SetAmbientNightCompensation(float value)
	{
		ambientNightCompensation = Mathf.Clamp01(value);
	}

	public void ApplyCloudSequenceImmediate(float value)
	{
		ApplyCloudiness(value, instant: true);
	}

	public void ForceUpdateAllGlobals()
	{
		if (base.isActiveAndEnabled)
		{
			ResolveReferences();
			if (ValidateRequiredReferences())
			{
				ConfigureControllers();
				SynchronizeOutputs(force: true);
			}
		}
	}

	public void SubscribeDecalLight(DecalLight decalLight)
	{
		if (!(decalLight == null))
		{
			decalLight.gameObject.SetActive(IsNight);
			if (!decalLights.Contains(decalLight))
			{
				decalLights.Add(decalLight);
			}
		}
	}

	public void UnsubscribeDecalLight(DecalLight decalLight)
	{
		decalLights.Remove(decalLight);
	}

	private SunAndSkyController RequestInstance()
	{
		return this;
	}

	private void ResolveReferences()
	{
		if (sunLight == null)
		{
			TryGetComponent<Light>(out sunLight);
		}
		if (skyAtmosphere == null)
		{
			skyAtmosphere = GetComponent<SkyAtmosphereController>();
		}
		if (weatherController == null)
		{
			weatherController = GetComponent<WeatherController>();
		}
		if (rainWetness == null)
		{
			rainWetness = GetComponentInChildren<RainWetnessDriver>(includeInactive: true);
		}
		if (lensFlare == null)
		{
			lensFlare = GetComponent<LensFlareComponentSRP>();
		}
		if (cloudMaterial == null && weatherController != null)
		{
			cloudMaterial = weatherController.cloudMaterial;
		}
		if (cloudMaterial == null && skyAtmosphere != null)
		{
			cloudMaterial = skyAtmosphere.cloudHemisphereMaterial;
		}
		ResolveNamedPresets();
	}

	private void ResolveNamedPresets()
	{
		if (weatherController == null || weatherController.presets == null)
		{
			return;
		}
		for (int i = 0; i < weatherController.presets.Count; i++)
		{
			WeatherPreset weatherPreset = weatherController.presets[i];
			if (weatherPreset == null)
			{
				continue;
			}
			switch (weatherPreset.name.ToLowerInvariant())
			{
			case "clear":
				if ((object)clearPreset == null)
				{
					clearPreset = weatherPreset;
				}
				break;
			case "hazed":
				if ((object)hazedPreset == null)
				{
					hazedPreset = weatherPreset;
				}
				break;
			case "cloudy":
				if ((object)cloudyPreset == null)
				{
					cloudyPreset = weatherPreset;
				}
				break;
			case "overcast":
				if ((object)overcastPreset == null)
				{
					overcastPreset = weatherPreset;
				}
				break;
			case "rainy":
				if ((object)rainyPreset == null)
				{
					rainyPreset = weatherPreset;
				}
				break;
			case "stormy":
				if ((object)stormyPreset == null)
				{
					stormyPreset = weatherPreset;
				}
				break;
			}
		}
	}

	private bool ValidateRequiredReferences()
	{
		int num;
		if (skyAtmosphere != null && weatherController != null)
		{
			num = ((sunLight != null) ? 1 : 0);
			if (num != 0)
			{
				goto IL_0048;
			}
		}
		else
		{
			num = 0;
		}
		if (!missingReferencesLogged)
		{
			missingReferencesLogged = true;
			Debug.LogError("SunAndSkyController requires Light, SkyAtmosphereController and WeatherController references on the composition root.", this);
		}
		goto IL_0048;
		IL_0048:
		return (byte)num != 0;
	}

	private void ConfigureControllers()
	{
		if (!(weatherController == null))
		{
			if (weatherController.skyAtmosphere == null)
			{
				weatherController.skyAtmosphere = skyAtmosphere;
			}
			if (weatherController.cloudMaterial == null)
			{
				weatherController.cloudMaterial = cloudMaterial;
			}
			if (weatherController.reflectionGlobalName != "_SkyboxProbe")
			{
				weatherController.reflectionGlobalName = "_SkyboxProbe";
			}
		}
	}

	private void SubscribeRainDelivery()
	{
		if (!(weatherController == null) && !(rainWetness == null) && weatherController.onRainAmountSent != null)
		{
			weatherController.onRainAmountSent.RemoveListener(rainWetness.SetWeatherIntensity);
			weatherController.onRainAmountSent.AddListener(rainWetness.SetWeatherIntensity);
		}
	}

	private void UnsubscribeRainDelivery()
	{
		if (weatherController != null && rainWetness != null && weatherController.onRainAmountSent != null)
		{
			weatherController.onRainAmountSent.RemoveListener(rainWetness.SetWeatherIntensity);
		}
	}

	private void ApplyCloudiness(float value, bool instant)
	{
		cloudiness = Mathf.Clamp(value, 0f, 10f);
		WeatherPreset weatherPreset = DryPresetForCloudiness(cloudiness);
		if (weatherPreset != null)
		{
			lastDryPreset = weatherPreset;
		}
		if (!(weatherController == null) && !(weatherController.TargetRainAmount > 0.0005f) && !(weatherController.CurrentRainAmount > 0.0005f) && !(weatherController.TargetPreset == weatherPreset))
		{
			SetDryWeather(weatherPreset, instant, $"cloudiness {cloudiness:0.##}");
		}
	}

	private WeatherPreset DryPresetForCloudiness(float value)
	{
		if (value < 2.5f)
		{
			return clearPreset;
		}
		if (!(value < 7.5f))
		{
			return overcastPreset;
		}
		return cloudyPreset;
	}

	private void SetDryWeather(WeatherPreset preset, bool instant, string source)
	{
		if (preset == null)
		{
			Debug.LogWarning("SunAndSkyController cannot apply " + source + ": the matching dry preset is not assigned.", this);
			return;
		}
		lastDryPreset = preset;
		weatherController.SetWeather(preset, instant);
	}

	private void RememberCurrentDryPreset()
	{
		WeatherPreset activePreset = weatherController.ActivePreset;
		if (activePreset != null && activePreset.rainAmount <= 0.0005f)
		{
			lastDryPreset = activePreset;
		}
		else if (lastDryPreset == null)
		{
			lastDryPreset = DryPresetForCloudiness(cloudiness);
		}
	}

	private void SynchronizeOutputs(bool force)
	{
		AlignMoon(force);
		UpdateNightState();
		ApplyExposure(force);
		ApplyAmbientCompensation(force);
		ApplyWind(force);
		ApplyLensFlare(force);
		UpdateThunder(force);
	}

	private void AlignMoon(bool force)
	{
		Vector3 forward = base.transform.forward;
		HorizonAngle = 0f - forward.y;
		if (!(skyAtmosphere.moonLight == null))
		{
			Vector3 vector = -forward;
			if (Vector3.Angle(skyAtmosphere.moonLight.transform.forward, vector) < 0.01f)
			{
				lastSunForward = forward;
			}
			else if (force || !(Vector3.Angle(lastSunForward, forward) < 0.01f))
			{
				skyAtmosphere.moonLight.transform.forward = vector;
				lastSunForward = forward;
			}
		}
	}

	private void UpdateNightState()
	{
		bool moonIsActive = skyAtmosphere.MoonIsActive;
		if (nightInitialized && moonIsActive == IsNight)
		{
			return;
		}
		bool flag = nightInitialized;
		nightInitialized = true;
		IsNight = moonIsActive;
		weatherController.additionalAmbientLightPower = (moonIsActive ? 1f : 0f);
		if (moonIsActive)
		{
			Shader.EnableKeyword("_LIGHTS_ON");
		}
		else
		{
			Shader.DisableKeyword("_LIGHTS_ON");
		}
		for (int num = decalLights.Count - 1; num >= 0; num--)
		{
			DecalLight decalLight = decalLights[num];
			if (decalLight == null)
			{
				decalLights.RemoveAt(num);
			}
			else
			{
				decalLight.gameObject.SetActive(moonIsActive);
			}
		}
		if (flag)
		{
			OnDayTimeChanged?.Invoke(moonIsActive);
		}
	}

	private void ApplyExposure(bool force)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		CPMGraphicsSettingsManager instance = CPMGraphicsSettingsManager.Instance;
		if (!(instance == null))
		{
			float currentExposure = weatherController.CurrentExposure;
			if (force || !(Mathf.Abs(lastExposure - currentExposure) < 0.0005f))
			{
				lastExposure = currentExposure;
				instance.SetExternalExposure(currentExposure);
			}
		}
	}

	private void ApplyAmbientCompensation(bool force)
	{
		float num = (IsNight ? (1f + weatherController.CurrentExposure * ambientNightCompensation * 0.25f) : 1f);
		if (num > 1.0005f)
		{
			RenderSettings.ambientSkyColor *= num;
			RenderSettings.ambientEquatorColor *= num;
			RenderSettings.ambientGroundColor *= num;
		}
		Color linear = RenderSettings.ambientEquatorColor.linear;
		if (force || !Approximately(lastGlobalAmbient, linear))
		{
			lastGlobalAmbient = linear;
			Shader.SetGlobalColor(GlobalAmbientEquator, linear);
		}
	}

	private void ApplyWind(bool force)
	{
		if (!(cloudMaterial == null))
		{
			float num = Mathf.Repeat(windDirectionDegrees, 360f);
			float num2 = Mathf.Max(0f, windSpeedMetersPerSecond);
			if (force || !(Mathf.Abs(Mathf.DeltaAngle(lastWindDirection, num)) < 0.01f) || !(Mathf.Abs(lastWindSpeed - num2) < 0.0005f))
			{
				float f = num * ((float)Math.PI / 180f);
				cloudMaterial.SetVector(WindDirection, new Vector4(Mathf.Cos(f), 0f, Mathf.Sin(f), 0f));
				cloudMaterial.SetFloat(CloudSpeed, num2);
				lastWindDirection = num;
				lastWindSpeed = num2;
			}
		}
	}

	private void ApplyLensFlare(bool force)
	{
		if (Application.isPlaying && !(lensFlare == null) && !(sunLight == null))
		{
			float num = (sunLight.enabled ? (sunLight.intensity * sunLight.color.grayscale * 0.5f) : 0f);
			if (force || !(Mathf.Abs(lastFlareIntensity - num) < 0.0005f))
			{
				lastFlareIntensity = num;
				lensFlare.intensity = num;
			}
		}
	}

	private void RefreshThunderTarget()
	{
		thunderTarget = ((Application.isPlaying && V.CoreGameManager != null && V.CoreGameManager.MainCamera != null) ? V.CoreGameManager.MainCamera.gameObject : null);
	}

	private void UpdateThunder(bool force)
	{
		if (!Application.isPlaying)
		{
			return;
		}
		float num = ((weatherController != null) ? weatherController.CurrentRainAmount : 0f);
		if (ignoreThunderSound)
		{
			StopThunder();
			return;
		}
		if (force || Mathf.Abs(lastThunderRtpc - num) >= 0.0005f)
		{
			lastThunderRtpc = num;
		}
		if (num >= 0.8f)
		{
			if (!thunderPlaying)
			{
				if (thunderTarget == null)
				{
					RefreshThunderTarget();
				}
				if (thunderTarget != null)
				{
					thunderPlaying = true;
				}
			}
		}
		else if (thunderPlaying)
		{
			thunderPlaying = false;
		}
	}

	private void StopThunder()
	{
		if (thunderPlaying)
		{
			thunderPlaying = false;
		}
	}

	private static bool Approximately(Color a, Color b)
	{
		if (Mathf.Abs(a.r - b.r) < 0.0005f && Mathf.Abs(a.g - b.g) < 0.0005f && Mathf.Abs(a.b - b.b) < 0.0005f)
		{
			return Mathf.Abs(a.a - b.a) < 0.0005f;
		}
		return false;
	}
}
