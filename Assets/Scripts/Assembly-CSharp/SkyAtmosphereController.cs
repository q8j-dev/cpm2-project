using System;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using _Car_Parking.Scripts.Steam.Perf;

[RequireComponent(typeof(Light))]
[ExecuteAlways]
public class SkyAtmosphereController : MonoBehaviour
{
	public enum CelestialSource
	{
		Auto = 0,
		Sun = 1,
		Moon = 2
	}

	private static class ShaderIds
	{
		public static readonly int RayleighScattering = Shader.PropertyToID("_SA_RayleighScattering");

		public static readonly int RayleighDensityExpScale = Shader.PropertyToID("_SA_RayleighDensityExpScale");

		public static readonly int MieScattering = Shader.PropertyToID("_SA_MieScattering");

		public static readonly int MieDensityExpScale = Shader.PropertyToID("_SA_MieDensityExpScale");

		public static readonly int MieExtinction = Shader.PropertyToID("_SA_MieExtinction");

		public static readonly int MiePhaseG = Shader.PropertyToID("_SA_MiePhaseG");

		public static readonly int MieAbsorption = Shader.PropertyToID("_SA_MieAbsorption");

		public static readonly int AbsorptionExtinction = Shader.PropertyToID("_SA_AbsorptionExtinction");

		public static readonly int OzoneLayerWidth = Shader.PropertyToID("_SA_OzoneLayerWidth");

		public static readonly int OzoneLayer0ConstantTerm = Shader.PropertyToID("_SA_OzoneLayer0ConstantTerm");

		public static readonly int OzoneLayer0LinearTerm = Shader.PropertyToID("_SA_OzoneLayer0LinearTerm");

		public static readonly int OzoneLayer1ConstantTerm = Shader.PropertyToID("_SA_OzoneLayer1ConstantTerm");

		public static readonly int OzoneLayer1LinearTerm = Shader.PropertyToID("_SA_OzoneLayer1LinearTerm");

		public static readonly int OvercastAbsorption = Shader.PropertyToID("_SA_OvercastAbsorption");

		public static readonly int OvercastCenter = Shader.PropertyToID("_SA_OvercastCenter");

		public static readonly int OvercastHalfWidth = Shader.PropertyToID("_SA_OvercastHalfWidth");

		public static readonly int GroundAlbedo = Shader.PropertyToID("_SA_GroundAlbedo");

		public static readonly int BottomRadius = Shader.PropertyToID("_SA_BottomRadius");

		public static readonly int TopRadius = Shader.PropertyToID("_SA_TopRadius");

		public static readonly int SunDirection = Shader.PropertyToID("_SA_SunDirection");

		public static readonly int SunIlluminance = Shader.PropertyToID("_SA_SunIlluminance");

		public static readonly int LightPollutionLuminance = Shader.PropertyToID("_SA_LightPollutionLuminance");

		public static readonly int HorizonFillEnabled = Shader.PropertyToID("_SA_HorizonFillEnabled");

		public static readonly int HorizonFillStart = Shader.PropertyToID("_SA_HorizonFillStart");

		public static readonly int HorizonFadeAngle = Shader.PropertyToID("_SA_HorizonFadeAngle");

		public static readonly int HorizonFadeDarkening = Shader.PropertyToID("_SA_HorizonFadeDarkening");

		public static readonly int HorizonFillTint = Shader.PropertyToID("_SA_HorizonFillTint");

		public static readonly int CloudHemisphereLut = Shader.PropertyToID("_SA_CloudHemisphereLut");

		public static readonly int CloudsEnabled = Shader.PropertyToID("_SA_CloudsEnabled");

		public static readonly int CloudSunIntensity = Shader.PropertyToID("_SA_CloudSunIntensity");

		public static readonly int CloudAmbientIntensity = Shader.PropertyToID("_SA_CloudAmbientIntensity");

		public static readonly int CloudMaterialSunDirection = Shader.PropertyToID("_SunDirection");

		public static readonly int SunDiskAngularRadius = Shader.PropertyToID("_SA_SunDiskAngularRadius");

		public static readonly int SunDiskLuminanceScale = Shader.PropertyToID("_SA_SunDiskLuminanceScale");

		public static readonly int MultipleScatteringFactor = Shader.PropertyToID("_SA_MultipleScatteringFactor");

		public static readonly int MultiScatteringLUTRes = Shader.PropertyToID("_SA_MultiScatteringLUTRes");

		public static readonly int RayMarchMinMaxSPP = Shader.PropertyToID("_SA_RayMarchMinMaxSPP");

		public static readonly int SkyViewLutSize = Shader.PropertyToID("_SA_SkyViewLutSize");

		public static readonly int CameraPlanetPos = Shader.PropertyToID("_SA_CameraPlanetPos");

		public static readonly int CameraVhorizon = Shader.PropertyToID("_SA_CameraVhorizon");

		public static readonly int CameraCBottom = Shader.PropertyToID("_SA_CameraCBottom");

		public static readonly int APSliceCount = Shader.PropertyToID("_SA_APSliceCount");

		public static readonly int APKmPerSlice = Shader.PropertyToID("_SA_APKmPerSlice");

		public static readonly int APDistanceScale = Shader.PropertyToID("_SA_APDistanceScale");

		public static readonly int APSliceIndex = Shader.PropertyToID("_SA_APSliceIndex");

		public static readonly int CloudShadowStrength = Shader.PropertyToID("_SA_CloudShadowStrength");

		public static readonly int CloudShadowRadius = Shader.PropertyToID("_SA_CloudShadowRadius");

		public static readonly int CloudLayerBottom = Shader.PropertyToID("_CloudLayerBottom");

		public static readonly int CloudLayerTop = Shader.PropertyToID("_CloudLayerTop");

		public static readonly int TransmittanceLut = Shader.PropertyToID("_SA_TransmittanceLut");

		public static readonly int MultiScatteringLut = Shader.PropertyToID("_SA_MultiScatteringLut");

		public static readonly int SkyViewLut = Shader.PropertyToID("_SA_SkyViewLut");

		public static readonly int AerialPerspectiveLut = Shader.PropertyToID("_SA_AerialPerspectiveLut");
	}

	private const int TransmittanceWidth = 256;

	private const int TransmittanceHeight = 64;

	private const int MultiScatteringRes = 32;

	private const int SkyViewWidth = 96;

	private const int SkyViewHeight = 50;

	private const int AerialPerspectiveWidth = 96;

	private const int AerialPerspectiveHeight = 50;

	private const int AerialPerspectiveSlices = 16;

	private const int CloudDimSampleSize = 3;

	private const float CloudDimSampleIntervalSeconds = 0.15f;

	private const float CloudDimMinY = 0.005f;

	private const float CloudDimHorizonFadeY = 0.05f;

	[Tooltip("Sun color in outer space, before atmosphere absorption. The Light color is overwritten every frame with this multiplied by the atmosphere transmittance, so edit this instead of the Light color.")]
	[Header("Sun")]
	public Color spaceSunColor = Color.white;

	[Tooltip("Angular radius of the sun disk in degrees (real sun: ~0.268).")]
	public float sunDiskAngularRadiusDegrees = 0.268f;

	[Tooltip("Scale on the physically derived sun disk luminance. 0 disables the disk.")]
	[Min(0f)]
	public float sunDiskLuminanceScale = 1f;

	[Tooltip("Drive the light colors (sun and moon) with the transmittance sampled from the LUT.")]
	public bool updateLightColor = true;

	[Header("Moon / Night")]
	[Tooltip("Optional directional light used as the celestial light source when the sun is down. The sky is lit by one body at a time.")]
	public Light moonLight;

	[Tooltip("Moon color in outer space. The moon Light color is overwritten with this multiplied by the atmosphere transmittance.")]
	public Color spaceMoonColor = new Color(0.62f, 0.75f, 1f);

	[Tooltip("Which body lights the sky. Auto switches to the moon once the sun sinks below the configured elevation.")]
	public CelestialSource lightSource;

	[Range(-18f, 5f)]
	[Tooltip("Auto mode: sun elevation (degrees) below which the moon takes over the sky. Slightly negative keeps the twilight glow through sunset.")]
	public float moonSwitchSunElevationDegrees = -2f;

	[Tooltip("Keep only the active body's Light component enabled (URP renders a single main directional light). Only applies while a moon light is assigned.")]
	public bool switchLightComponents = true;

	[Tooltip("Isotropic night emission scattered by the atmosphere (city light pollution). Affects the sky and the aerial perspective; strongest near the horizon.")]
	[ColorUsage(false, true)]
	public Color lightPollution = Color.black;

	[Tooltip("Fill the sky below the horizon with the per-azimuth horizon color instead of the (nearly black) physical result.")]
	[Header("Horizon")]
	public bool extendSkyBelowHorizon = true;

	[Range(-5f, 5f)]
	[Tooltip("Elevation in degrees where the fill starts. Positive starts above the true horizon (hides the thin LUT seam line), negative keeps some physical sky below the horizon before the fill takes over.")]
	public float horizonFillStartDegrees = 0.2f;

	[Range(0.1f, 90f)]
	[Tooltip("Angle in degrees below the fill start over which the fill fades to its darkened value.")]
	public float horizonFadeAngleDegrees = 10f;

	[Tooltip("Brightness multiplier reached at the end of the fade. Relative to the horizon color, so it reacts to time of day and never glows at night.")]
	[Range(0f, 1f)]
	public float horizonFadeDarkening = 0.4f;

	[Tooltip("Tint applied to the below-horizon fill.")]
	public Color horizonFillTint = Color.white;

	[Header("Clouds")]
	[Tooltip("Composed hemisphere cloud texture (assign the CloudCompose CustomRenderTexture). None = no clouds.")]
	public Texture cloudHemisphereTexture;

	[Tooltip("Cloud raymarch material (Sky_CloudHemisphere). The active body's light direction is pushed into its _SunDirection every frame.")]
	public Material cloudHemisphereMaterial;

	[Min(0f)]
	[Tooltip("Scale on the sun scattering term of the clouds.")]
	public float cloudSunIntensity = 1f;

	[Min(0f)]
	[Tooltip("Scale on the ambient (sky) scattering term of the clouds.")]
	public float cloudAmbientIntensity = 1f;

	[Tooltip("Dim the active directional light (sun, or moon at night) when the hemisphere clouds cover it. The composed cloud texture is sampled in the light's direction (small neighborhood average, async readback), so the dim always matches the cloud the player sees over the sun disk. Only the analytic light dims - the sky already shows the clouds blocking the sun. 0 disables.")]
	[Range(0f, 1f)]
	public float cloudLightDimming = 0.8f;

	[Min(0f)]
	[Tooltip("Seconds a change in cloud occlusion takes to settle in the light color (hides the amortized cloud texture updates). 0 = instant.")]
	public float cloudDimSmoothSeconds = 0.75f;

	[Range(0f, 1f)]
	[Tooltip("GT7-style cloud shadows in the aerial perspective: every fog sample is projected along the sun direction onto the cloud layer and its direct sun in-scattering is dimmed by the cloud opacity found there (multi-scattering keeps an ambient floor). Uses the hemisphere cloud texture; 0 disables.")]
	public float cloudAerialShadow = 1f;

	[Tooltip("Where between the cloud layer bottom (0) and top (1) the representative shadow-casting altitude sits. The layer bounds are read from the cloud raymarch material.")]
	[Range(0f, 1f)]
	public float cloudAerialShadowHeight = 0.35f;

	[Header("Planet")]
	[Tooltip("Planet radius in kilometers (Earth: 6360).")]
	public float planetRadiusKm = 6360f;

	[Tooltip("Atmosphere layer thickness in kilometers (Earth: 100).")]
	public float atmosphereHeightKm = 100f;

	[Tooltip("Assumed view height above sea level, in meters. The sky is always rendered from this height.")]
	[Min(0.1f)]
	public float viewHeightMeters = 2f;

	[Tooltip("Ground bounce albedo used by the multi-scattering LUT.")]
	public Color groundAlbedo = Color.black;

	[Header("Rayleigh")]
	public float rayleighScaleHeightKm = 8f;

	[Tooltip("Rayleigh scattering coefficients in 1/km.")]
	public Vector3 rayleighScattering = new Vector3(0.005802f, 0.013558f, 0.0331f);

	[Header("Mie / Fog")]
	public float mieScaleHeightKm = 1.2f;

	[Range(0f, 0.5f)]
	[Tooltip("Mie scattering in 1/km (monochromatic). This is the haze/fog density dial: clear sky ~0.004, hazy ~0.02-0.05, misty/foggy 0.1+.")]
	public float mieScattering = 0.003996f;

	[Tooltip("Mie extinction in 1/km (monochromatic, kept >= scattering; the difference is absorption). Raise together with scattering; a larger gap darkens the haze.")]
	[Range(0f, 0.5f)]
	public float mieExtinction = 0.00444f;

	[Range(0f, 0.999f)]
	public float miePhaseG = 0.8f;

	[Tooltip("Ozone absorption coefficients in 1/km.")]
	[Header("Ozone")]
	public Vector3 ozoneAbsorption = new Vector3(0.00065f, 0.001881f, 8.5E-05f);

	[Range(0f, 1f)]
	[Tooltip("Strength of the grey light-absorbing cloud deck used as the overcast backdrop. Dims the sun, the sky, the aerial perspective and the hemisphere clouds' sun light consistently. 0 = clear.")]
	[Header("Overcast")]
	public float overcastAmount;

	[Tooltip("Altitude of the deck center in km above sea level.")]
	public float overcastAltitudeKm = 5.5f;

	[Tooltip("Full vertical thickness of the deck in km (tent density profile).")]
	[Min(0.1f)]
	public float overcastThicknessKm = 2f;

	[Tooltip("Absorption coefficient in 1/km at Overcast Amount = 1. Vertical optical depth = this x half thickness (2.5 x 1 km cuts the vertical sun to ~8%).")]
	[Min(0f)]
	public float overcastMaxAbsorption = 2.5f;

	[Header("Quality")]
	[Tooltip("Contribution of the multiple scattering approximation (paper section 3.2).")]
	[Range(0f, 1f)]
	public float multipleScatteringFactor = 1f;

	[Tooltip("Min/max ray marching samples per pixel for the sky-view LUT.")]
	public Vector2 rayMarchMinMaxSPP = new Vector2(4f, 14f);

	[Tooltip("Play mode: minimum seconds between re-renders of the transmittance and multi-scattering LUTs while the atmosphere parameters animate (e.g. weather fades). They settle on the final values at most this long after the change stops. 0 = re-render every frame. Edit mode and the first render are always immediate.")]
	[Min(0f)]
	public float staticLutUpdateIntervalSeconds = 0.1f;

	[Tooltip("Kilometers covered by one aerial perspective depth slice (16 slices, squared distribution).")]
	[Min(0.5f)]
	public float aerialPerspectiveKmPerSlice = 4f;

	[Tooltip("Play mode: aerial perspective depth slices re-rendered per frame, cycling back to front (16 = the whole volume every frame). 1 spreads the volume over 16 frames. Edit mode always renders all slices, and the volume is fully primed on the first frame.")]
	[Range(1f, 16f)]
	public int aerialPerspectiveSlicesPerFrame = 1;

	[Range(0.1f, 10f)]
	[Tooltip("Artistic aerial perspective thickness: scales the distance used when sampling the fog volume. 2 = surfaces fog up as if twice as far away (the fog ramp completes at half the volume range).")]
	public float aerialPerspectiveDistanceScale = 1f;

	[Header("Output")]
	[Tooltip("Optional. Skybox material using the Skybox/SkyAtmosphere shader. Created automatically when empty.")]
	public Material skyboxMaterial;

	[Tooltip("Assign the skybox material to RenderSettings.skybox automatically.")]
	public bool assignSkyboxToRenderSettings = true;

	[SerializeField]
	[HideInInspector]
	private Shader lutShader;

	[SerializeField]
	[HideInInspector]
	private Shader skyboxShader;

	private RenderTexture transmittanceLut;

	private RenderTexture multiScatteringLut;

	private RenderTexture skyViewLut;

	private RenderTexture aerialPerspectiveLut;

	private Texture2D transmittanceCpu;

	private Material lutMaterial;

	private Material ownedSkyboxMaterial;

	private Light sunLight;

	private int lastStaticLutHash;

	private double lastStaticLutRenderTime;

	private bool transmittanceCpuValid;

	private int apSliceCursor = 15;

	private bool apPrimePending;

	private float cloudOcclusionTarget;

	private float cloudOcclusionSmoothed;

	private float cloudReadbackHorizonFade;

	private bool cloudReadbackPending;

	private double lastCloudSampleTime;

	private double lastCloudSmoothTime;

	private Texture lastCloudSampleTexture;

	private Texture2D cloudDimScratch;

	public Color SunTransmittance { get; private set; } = Color.white;

	public Light SunLight => sunLight;

	public float CloudLightOcclusion => cloudOcclusionSmoothed;

	private Vector3 SunDirectionPlanetSpace
	{
		get
		{
			Vector3 vector = -base.transform.forward;
			return new Vector3(vector.x, vector.z, vector.y);
		}
	}

	private Vector3 MoonDirectionPlanetSpace
	{
		get
		{
			Vector3 vector = -moonLight.transform.forward;
			return new Vector3(vector.x, vector.z, vector.y);
		}
	}

	public bool MoonIsActive
	{
		get
		{
			if (moonLight == null)
			{
				return false;
			}
			if (lightSource == CelestialSource.Moon)
			{
				return true;
			}
			if (lightSource == CelestialSource.Sun)
			{
				return false;
			}
			return SunDirectionPlanetSpace.z < Mathf.Sin(moonSwitchSunElevationDegrees * ((float)Math.PI / 180f));
		}
	}

	public Color SampleSunTransmittance(float sunZenithCosAngle)
	{
		if (!transmittanceCpuValid)
		{
			return Color.white;
		}
		double num = planetRadiusKm;
		double num2 = planetRadiusKm + atmosphereHeightKm;
		double num3 = (double)Math.Max(viewHeightMeters, 0.1f) * 0.001;
		double num4 = num + num3;
		double num5 = Math.Sqrt(Math.Max(0.0, num2 * num2 - num * num));
		double num6 = Math.Sqrt(num3 * (2.0 * num + num3));
		double num7 = Math.Min(1.0, Math.Max(-1.0, sunZenithCosAngle));
		double val = num4 * num4 * (num7 * num7 - 1.0) + num2 * num2;
		double num8 = Math.Max(0.0, (0.0 - num4) * num7 + Math.Sqrt(Math.Max(0.0, val)));
		double num9 = num2 - num4;
		double num10 = num6 + num5;
		float u = Mathf.Clamp01((float)((num8 - num9) / (num10 - num9)));
		float v = Mathf.Clamp01((float)(num6 / num5));
		Color pixelBilinear = transmittanceCpu.GetPixelBilinear(u, v);
		double num11 = (0.0 - num6) / num4;
		float num12 = Mathf.Max(sunDiskAngularRadiusDegrees * ((float)Math.PI / 180f), 0.0001f);
		float num13 = Mathf.Clamp01((float)((num7 - num11) / (2.0 * (double)num12)) + 0.5f);
		return pixelBilinear * num13;
	}

	private void Reset()
	{
		FindShaders();
	}

	private void OnValidate()
	{
		FindShaders();
	}

	private void FindShaders()
	{
		if (lutShader == null)
		{
			lutShader = Shader.Find("Hidden/SkyAtmosphere/Luts");
		}
		if (skyboxShader == null)
		{
			skyboxShader = Shader.Find("Skybox/SkyAtmosphere");
		}
	}

	private void OnEnable()
	{
		sunLight = GetComponent<Light>();
		if (sunLight.type != LightType.Directional)
		{
			Debug.LogWarning("SkyAtmosphereController expects a directional light.", this);
		}
		lastStaticLutHash = 0;
		lastStaticLutRenderTime = 0.0;
		transmittanceCpuValid = false;
	}

	private void OnDisable()
	{
		ReleaseRt(ref transmittanceLut);
		ReleaseRt(ref multiScatteringLut);
		ReleaseRt(ref skyViewLut);
		ReleaseRt(ref aerialPerspectiveLut);
		DestroyOwned(ref transmittanceCpu);
		DestroyOwned(ref lutMaterial);
		DestroyOwned(ref cloudDimScratch);
		cloudReadbackPending = false;
	}

	private void OnDestroy()
	{
		DestroyOwned(ref ownedSkyboxMaterial);
	}

	private void LateUpdate()
	{
		FindShaders();
		if (lutShader == null || skyboxShader == null)
		{
			Debug.LogError("SkyAtmosphereController: shaders not found. Make sure the SkyAtmosphere shaders are in the project.", this);
			base.enabled = false;
			return;
		}
		EnsureResources();
		SetGlobalUniforms();
		int num = ComputeStaticLutHash();
		RenderTexture active = RenderTexture.active;
		if (num != lastStaticLutHash && StaticLutUpdateDue())
		{
			lastStaticLutHash = num;
			lastStaticLutRenderTime = Time.realtimeSinceStartupAsDouble;
			RenderLutPass(transmittanceLut, 0);
			RenderLutPass(multiScatteringLut, 1);
			ReadbackTransmittance();
		}
		RenderLutPass(skyViewLut, 2);
		RenderAerialPerspective(!Application.isPlaying || apPrimePending);
		apPrimePending = false;
		RenderTexture.active = active;
		UpdateCloudLightOcclusion();
		UpdateSunLightColor();
		UpdateLightEnabledStates();
		UpdateSkyboxMaterial();
	}

	private void UpdateLightEnabledStates()
	{
		if (!switchLightComponents || sunLight == null)
		{
			return;
		}
		if (moonLight == null)
		{
			if (!sunLight.enabled)
			{
				sunLight.enabled = true;
			}
			return;
		}
		bool moonIsActive = MoonIsActive;
		if (sunLight.enabled == moonIsActive)
		{
			sunLight.enabled = !moonIsActive;
		}
		if (moonLight.enabled != moonIsActive)
		{
			moonLight.enabled = moonIsActive;
		}
	}

	private void EnsureResources()
	{
		if (lutMaterial == null)
		{
			lutMaterial = new Material(lutShader)
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
		if (transmittanceLut == null)
		{
			transmittanceLut = CreateLutRt("SkyAtmosphere Transmittance", 256, 64);
		}
		if (multiScatteringLut == null)
		{
			multiScatteringLut = CreateLutRt("SkyAtmosphere MultiScattering", 32, 32);
		}
		if (skyViewLut == null)
		{
			skyViewLut = CreateLutRt("SkyAtmosphere SkyView", 96, 50);
		}
		if (aerialPerspectiveLut == null)
		{
			aerialPerspectiveLut = new RenderTexture(96, 50, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear)
			{
				name = "SkyAtmosphere AerialPerspective",
				dimension = TextureDimension.Tex3D,
				volumeDepth = 16,
				filterMode = FilterMode.Bilinear,
				wrapModeU = TextureWrapMode.Repeat,
				wrapModeV = TextureWrapMode.Clamp,
				wrapModeW = TextureWrapMode.Clamp,
				useMipMap = false
			};
			aerialPerspectiveLut.Create();
			ClearAerialPerspective();
			apPrimePending = true;
			apSliceCursor = 15;
		}
		else if (!aerialPerspectiveLut.IsCreated())
		{
			aerialPerspectiveLut.Create();
			ClearAerialPerspective();
			apPrimePending = true;
			apSliceCursor = 15;
		}
		if (transmittanceCpu == null)
		{
			transmittanceCpu = new Texture2D(256, 64, TextureFormat.RGBAHalf, mipChain: false, linear: true)
			{
				name = "SkyAtmosphere Transmittance CPU",
				hideFlags = HideFlags.HideAndDontSave,
				wrapMode = TextureWrapMode.Clamp
			};
			transmittanceCpuValid = false;
		}
	}

	private static RenderTexture CreateLutRt(string name, int width, int height)
	{
		RenderTexture renderTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBHalf, RenderTextureReadWrite.Linear);
		renderTexture.name = name;
		renderTexture.filterMode = FilterMode.Bilinear;
		renderTexture.wrapMode = TextureWrapMode.Clamp;
		renderTexture.useMipMap = false;
		renderTexture.Create();
		return renderTexture;
	}

	private void ClearAerialPerspective()
	{
		RenderTexture active = RenderTexture.active;
		for (int i = 0; i < 16; i++)
		{
			Graphics.SetRenderTarget(aerialPerspectiveLut, 0, CubemapFace.Unknown, i);
			GL.Clear(clearDepth: false, clearColor: true, Color.clear);
		}
		RenderTexture.active = active;
	}

	private static void ReleaseRt(ref RenderTexture rt)
	{
		if (rt != null)
		{
			rt.Release();
			DestroyImmediateSafe(rt);
			rt = null;
		}
	}

	private static void DestroyOwned<T>(ref T obj) where T : UnityEngine.Object
	{
		if (obj != null)
		{
			DestroyImmediateSafe(obj);
			obj = null;
		}
	}

	private static void DestroyImmediateSafe(UnityEngine.Object obj)
	{
		if (Application.isPlaying)
		{
			UnityEngine.Object.Destroy(obj);
		}
		else
		{
			UnityEngine.Object.DestroyImmediate(obj);
		}
	}

	private void SetGlobalUniforms()
	{
		float num = planetRadiusKm;
		float value = planetRadiusKm + atmosphereHeightKm;
		double num2 = planetRadiusKm;
		double num3 = (double)Math.Max(viewHeightMeters, 0.1f) * 0.001;
		double num4 = num3 * (2.0 * num2 + num3);
		float z = (float)(num2 + num3);
		float num5 = Mathf.Max(mieScattering, mieExtinction);
		Vector3 vector = Vector3.one * mieScattering;
		Vector3 vector2 = Vector3.one * num5;
		Vector3 vector3 = Vector3.one * (num5 - mieScattering);
		bool moonIsActive = MoonIsActive;
		Vector3 vector4 = (moonIsActive ? MoonDirectionPlanetSpace : SunDirectionPlanetSpace);
		Color linear = (moonIsActive ? spaceMoonColor : spaceSunColor).linear;
		float num6 = (moonIsActive ? moonLight.intensity : ((sunLight != null) ? sunLight.intensity : 1f));
		Color linear2 = lightPollution.linear;
		Color linear3 = horizonFillTint.linear;
		Shader.SetGlobalVector(ShaderIds.RayleighScattering, rayleighScattering);
		Shader.SetGlobalFloat(ShaderIds.RayleighDensityExpScale, -1f / Mathf.Max(rayleighScaleHeightKm, 0.001f));
		Shader.SetGlobalVector(ShaderIds.MieScattering, vector);
		Shader.SetGlobalFloat(ShaderIds.MieDensityExpScale, -1f / Mathf.Max(mieScaleHeightKm, 0.001f));
		Shader.SetGlobalVector(ShaderIds.MieExtinction, vector2);
		Shader.SetGlobalFloat(ShaderIds.MiePhaseG, miePhaseG);
		Shader.SetGlobalVector(ShaderIds.MieAbsorption, vector3);
		Shader.SetGlobalVector(ShaderIds.AbsorptionExtinction, ozoneAbsorption);
		Shader.SetGlobalFloat(ShaderIds.OzoneLayerWidth, 25f);
		Shader.SetGlobalFloat(ShaderIds.OzoneLayer0ConstantTerm, -2f / 3f);
		Shader.SetGlobalFloat(ShaderIds.OzoneLayer0LinearTerm, 1f / 15f);
		Shader.SetGlobalFloat(ShaderIds.OzoneLayer1ConstantTerm, 2.6666667f);
		Shader.SetGlobalFloat(ShaderIds.OzoneLayer1LinearTerm, -1f / 15f);
		Shader.SetGlobalFloat(ShaderIds.OvercastAbsorption, overcastAmount * overcastMaxAbsorption);
		Shader.SetGlobalFloat(ShaderIds.OvercastCenter, overcastAltitudeKm);
		Shader.SetGlobalFloat(ShaderIds.OvercastHalfWidth, overcastThicknessKm * 0.5f);
		Shader.SetGlobalVector(ShaderIds.GroundAlbedo, new Vector3(groundAlbedo.linear.r, groundAlbedo.linear.g, groundAlbedo.linear.b));
		Shader.SetGlobalFloat(ShaderIds.BottomRadius, num);
		Shader.SetGlobalFloat(ShaderIds.TopRadius, value);
		Shader.SetGlobalVector(ShaderIds.SunDirection, vector4);
		Shader.SetGlobalVector(ShaderIds.SunIlluminance, new Vector3(linear.r, linear.g, linear.b) * num6);
		Shader.SetGlobalFloat(ShaderIds.SunDiskAngularRadius, sunDiskAngularRadiusDegrees * ((float)Math.PI / 180f));
		Shader.SetGlobalFloat(ShaderIds.SunDiskLuminanceScale, sunDiskLuminanceScale);
		Shader.SetGlobalVector(ShaderIds.LightPollutionLuminance, new Vector3(linear2.r, linear2.g, linear2.b));
		Shader.SetGlobalFloat(ShaderIds.CloudsEnabled, (cloudHemisphereTexture != null) ? 1f : 0f);
		Shader.SetGlobalTexture(ShaderIds.CloudHemisphereLut, (cloudHemisphereTexture != null) ? cloudHemisphereTexture : Texture2D.blackTexture);
		Shader.SetGlobalFloat(ShaderIds.CloudSunIntensity, cloudSunIntensity);
		Shader.SetGlobalFloat(ShaderIds.CloudAmbientIntensity, cloudAmbientIntensity);
		if (cloudHemisphereMaterial != null)
		{
			Vector3 vector5 = (moonIsActive ? moonLight.transform.forward : base.transform.forward);
			cloudHemisphereMaterial.SetVector(ShaderIds.CloudMaterialSunDirection, vector5);
		}
		float a = 1500f;
		float b = 4000f;
		if (cloudHemisphereMaterial != null && cloudHemisphereMaterial.HasProperty(ShaderIds.CloudLayerBottom))
		{
			a = cloudHemisphereMaterial.GetFloat(ShaderIds.CloudLayerBottom);
		}
		if (cloudHemisphereMaterial != null && cloudHemisphereMaterial.HasProperty(ShaderIds.CloudLayerTop))
		{
			b = cloudHemisphereMaterial.GetFloat(ShaderIds.CloudLayerTop);
		}
		float num7 = Mathf.Lerp(a, b, cloudAerialShadowHeight) * 0.001f;
		Shader.SetGlobalFloat(ShaderIds.CloudShadowRadius, num + num7);
		float value2 = ((cloudHemisphereTexture != null) ? cloudAerialShadow : 0f);
		if (!SteamPerfFlags.IsOn("cloudShadow"))
		{
			value2 = 0f;
		}
		Shader.SetGlobalFloat(ShaderIds.CloudShadowStrength, value2);
		Shader.SetGlobalFloat(ShaderIds.HorizonFillEnabled, extendSkyBelowHorizon ? 1f : 0f);
		Shader.SetGlobalFloat(ShaderIds.HorizonFillStart, horizonFillStartDegrees * ((float)Math.PI / 180f));
		Shader.SetGlobalFloat(ShaderIds.HorizonFadeAngle, horizonFadeAngleDegrees * ((float)Math.PI / 180f));
		Shader.SetGlobalFloat(ShaderIds.HorizonFadeDarkening, horizonFadeDarkening);
		Shader.SetGlobalVector(ShaderIds.HorizonFillTint, new Vector3(linear3.r, linear3.g, linear3.b));
		Shader.SetGlobalFloat(ShaderIds.MultipleScatteringFactor, multipleScatteringFactor);
		Shader.SetGlobalFloat(ShaderIds.MultiScatteringLUTRes, 32f);
		Shader.SetGlobalVector(ShaderIds.RayMarchMinMaxSPP, rayMarchMinMaxSPP);
		Shader.SetGlobalVector(ShaderIds.SkyViewLutSize, new Vector4(96f, 50f, 1f / 96f, 0.02f));
		Shader.SetGlobalVector(ShaderIds.CameraPlanetPos, new Vector3(0f, 0f, z));
		Shader.SetGlobalFloat(ShaderIds.CameraVhorizon, (float)Math.Sqrt(num4));
		Shader.SetGlobalFloat(ShaderIds.CameraCBottom, (float)num4);
		Shader.SetGlobalFloat(ShaderIds.APSliceCount, 16f);
		Shader.SetGlobalFloat(ShaderIds.APKmPerSlice, aerialPerspectiveKmPerSlice);
		Shader.SetGlobalFloat(ShaderIds.APDistanceScale, Mathf.Max(aerialPerspectiveDistanceScale, 0.01f));
		Shader.SetGlobalTexture(ShaderIds.TransmittanceLut, transmittanceLut);
		Shader.SetGlobalTexture(ShaderIds.MultiScatteringLut, multiScatteringLut);
		Shader.SetGlobalTexture(ShaderIds.SkyViewLut, skyViewLut);
		Shader.SetGlobalTexture(ShaderIds.AerialPerspectiveLut, aerialPerspectiveLut);
	}

	private bool StaticLutUpdateDue()
	{
		if (!Application.isPlaying || !transmittanceCpuValid)
		{
			return true;
		}
		return Time.realtimeSinceStartupAsDouble - lastStaticLutRenderTime >= (double)staticLutUpdateIntervalSeconds;
	}

	private int ComputeStaticLutHash()
	{
		return HashCode.Combine(HashCode.Combine(HashCode.Combine(planetRadiusKm, atmosphereHeightKm, rayleighScaleHeightKm, rayleighScattering, mieScaleHeightKm, mieScattering, mieExtinction, miePhaseG), ozoneAbsorption, groundAlbedo, multipleScatteringFactor), overcastAmount, overcastAltitudeKm, overcastThicknessKm, overcastMaxAbsorption);
	}

	private void RenderLutPass(RenderTexture target, int pass, int depthSlice = -1)
	{
		if (depthSlice >= 0)
		{
			Graphics.SetRenderTarget(target, 0, CubemapFace.Unknown, depthSlice);
		}
		else
		{
			Graphics.SetRenderTarget(target);
		}
		lutMaterial.SetPass(pass);
		Graphics.DrawProceduralNow(MeshTopology.Triangles, 3);
	}

	private void RenderAerialPerspective(bool allSlices)
	{
		int num = (allSlices ? 16 : Mathf.Clamp(aerialPerspectiveSlicesPerFrame, 1, 16));
		for (int i = 0; i < num; i++)
		{
			int num2 = (allSlices ? i : apSliceCursor);
			lutMaterial.SetFloat(ShaderIds.APSliceIndex, num2);
			RenderLutPass(aerialPerspectiveLut, 3, num2);
			if (!allSlices && --apSliceCursor < 0)
			{
				apSliceCursor = 15;
			}
		}
		if (allSlices)
		{
			apSliceCursor = 15;
		}
	}

	private void ReadbackTransmittance()
	{
		if (Application.isPlaying && transmittanceCpuValid && SystemInfo.supportsAsyncGPUReadback)
		{
			AsyncGPUReadback.Request(transmittanceLut, 0, TextureFormat.RGBAHalf, OnTransmittanceReadback);
			return;
		}
		RenderTexture.active = transmittanceLut;
		transmittanceCpu.ReadPixels(new Rect(0f, 0f, 256f, 64f), 0, 0, recalculateMipMaps: false);
		transmittanceCpuValid = true;
	}

	private void OnTransmittanceReadback(AsyncGPUReadbackRequest request)
	{
		if (!(this == null) && !(transmittanceCpu == null) && !request.hasError)
		{
			ApplyTransmittanceReadback(request, transmittanceCpu);
			transmittanceCpuValid = true;
		}
	}

	public static void ApplyTransmittanceReadback(AsyncGPUReadbackRequest request, Texture2D target)
	{
		target.SetPixelData(request.GetData<byte>(), 0);
	}

	private void UpdateSunLightColor()
	{
		Color color = (SunTransmittance = SampleSunTransmittance(SunDirectionPlanetSpace.z));
		if (updateLightColor)
		{
			bool moonIsActive = MoonIsActive;
			float num = 1f - cloudOcclusionSmoothed * cloudLightDimming;
			if (sunLight != null)
			{
				sunLight.color = AttenuatedLightColor(spaceSunColor, color * (moonIsActive ? 1f : num));
			}
			if (moonLight != null)
			{
				moonLight.color = AttenuatedLightColor(spaceMoonColor, SampleSunTransmittance(MoonDirectionPlanetSpace.z) * (moonIsActive ? num : 1f));
			}
		}
	}

	private static Color AttenuatedLightColor(Color spaceColor, Color transmittanceLinear)
	{
		Color linear = spaceColor.linear;
		return new Color(linear.r * transmittanceLinear.r, linear.g * transmittanceLinear.g, linear.b * transmittanceLinear.b, 1f).gamma;
	}

	private void UpdateCloudLightOcclusion()
	{
		double realtimeSinceStartupAsDouble = Time.realtimeSinceStartupAsDouble;
		float num = ((lastCloudSmoothTime > 0.0) ? Mathf.Clamp((float)(realtimeSinceStartupAsDouble - lastCloudSmoothTime), 0f, 0.25f) : 0f);
		lastCloudSmoothTime = realtimeSinceStartupAsDouble;
		Vector3 toBodyUnity = (MoonIsActive ? (-moonLight.transform.forward) : (-base.transform.forward));
		if (!(cloudLightDimming > 0f) || !updateLightColor || !(cloudHemisphereTexture != null) || !(toBodyUnity.y >= 0.005f))
		{
			cloudOcclusionTarget = 0f;
		}
		else if (cloudHemisphereTexture != lastCloudSampleTexture || realtimeSinceStartupAsDouble - lastCloudSampleTime >= 0.15000000596046448)
		{
			lastCloudSampleTime = realtimeSinceStartupAsDouble;
			lastCloudSampleTexture = cloudHemisphereTexture;
			SampleCloudOcclusion(toBodyUnity);
		}
		if (cloudDimSmoothSeconds <= 0f)
		{
			cloudOcclusionSmoothed = cloudOcclusionTarget;
		}
		else
		{
			cloudOcclusionSmoothed = Mathf.MoveTowards(cloudOcclusionSmoothed, cloudOcclusionTarget, num / cloudDimSmoothSeconds);
		}
	}

	private void SampleCloudOcclusion(Vector3 toBodyUnity)
	{
		float num = Mathf.Clamp01((toBodyUnity.y - 0.005f) / 0.05f);
		Vector2 uv = WorldDirToHemisphereUV(toBodyUnity);
		if (cloudHemisphereTexture is RenderTexture renderTexture)
		{
			if (SystemInfo.supportsAsyncGPUReadback)
			{
				if (!cloudReadbackPending)
				{
					cloudReadbackPending = true;
					cloudReadbackHorizonFade = num;
					RectInt rectInt = CloudSampleRegion(renderTexture.width, renderTexture.height, uv, 3);
					AsyncGPUReadback.Request(renderTexture, 0, rectInt.x, rectInt.width, rectInt.y, rectInt.height, 0, 1, TextureFormat.RGBAFloat, OnCloudReadback);
				}
			}
			else
			{
				cloudOcclusionTarget = SampleAverageR(renderTexture, uv, 3, ref cloudDimScratch) * num;
			}
		}
		else if (cloudHemisphereTexture is Texture2D { isReadable: not false } texture2D)
		{
			cloudOcclusionTarget = Mathf.Clamp01(texture2D.GetPixelBilinear(uv.x, uv.y).r) * num;
		}
	}

	private void OnCloudReadback(AsyncGPUReadbackRequest request)
	{
		cloudReadbackPending = false;
		if (!(this == null) && !request.hasError)
		{
			NativeArray<Color> data = request.GetData<Color>();
			float num = 0f;
			for (int i = 0; i < data.Length; i++)
			{
				num += data[i].r;
			}
			cloudOcclusionTarget = Mathf.Clamp01(num / (float)Mathf.Max(data.Length, 1)) * cloudReadbackHorizonFade;
		}
	}

	public static Vector2 WorldDirToHemisphereUV(Vector3 dir)
	{
		float num = Mathf.Acos(Mathf.Clamp01(dir.y));
		float num2 = Mathf.Atan2(dir.z, dir.x);
		float num3 = 0.5f / Mathf.Cos(Mathf.Repeat(num2 + (float)Math.PI / 4f, (float)Math.PI / 2f) - (float)Math.PI / 4f);
		float num4 = num / ((float)Math.PI / 2f);
		float num5 = num4 * num4 * num3;
		return new Vector2(Mathf.Cos(num2) * num5 + 0.5f, Mathf.Sin(num2) * num5 + 0.5f);
	}

	public static RectInt CloudSampleRegion(int width, int height, Vector2 uv, int size)
	{
		int xMin = Mathf.Clamp(Mathf.RoundToInt(uv.x * (float)width - 0.5f) - size / 2, 0, Mathf.Max(width - size, 0));
		int yMin = Mathf.Clamp(Mathf.RoundToInt(uv.y * (float)height - 0.5f) - size / 2, 0, Mathf.Max(height - size, 0));
		return new RectInt(xMin, yMin, size, size);
	}

	public static float SampleAverageR(RenderTexture rt, Vector2 uv, int size, ref Texture2D scratch)
	{
		RectInt rectInt = CloudSampleRegion(rt.width, rt.height, uv, size);
		if (scratch == null || scratch.width != size || scratch.height != size)
		{
			if (scratch != null)
			{
				DestroyImmediateSafe(scratch);
			}
			scratch = new Texture2D(size, size, TextureFormat.RGBAFloat, mipChain: false, linear: true)
			{
				hideFlags = HideFlags.HideAndDontSave
			};
		}
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rt;
		scratch.ReadPixels(new Rect(rectInt.x, rectInt.y, size, size), 0, 0, recalculateMipMaps: false);
		RenderTexture.active = active;
		Color[] pixels = scratch.GetPixels();
		float num = 0f;
		for (int i = 0; i < pixels.Length; i++)
		{
			num += pixels[i].r;
		}
		return Mathf.Clamp01(num / (float)Mathf.Max(pixels.Length, 1));
	}

	private void UpdateSkyboxMaterial()
	{
		Material material = skyboxMaterial;
		if (material == null)
		{
			if (ownedSkyboxMaterial == null)
			{
				ownedSkyboxMaterial = new Material(skyboxShader)
				{
					name = "SkyAtmosphere Skybox (auto)",
					hideFlags = HideFlags.DontSave
				};
			}
			material = ownedSkyboxMaterial;
		}
		if (assignSkyboxToRenderSettings && RenderSettings.skybox != material)
		{
			RenderSettings.skybox = material;
		}
	}
}
