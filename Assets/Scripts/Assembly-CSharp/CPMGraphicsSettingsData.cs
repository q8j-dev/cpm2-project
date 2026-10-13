using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class CPMGraphicsSettingsData : ICloneable
{
	[Header("Resolution and Clarity")]
	[Range(0.25f, 1f)]
	public float m_RenderScale = 1f;

	public CPMGraphicsSettings.AAQuality m_AAQuality = CPMGraphicsSettings.AAQuality.MSAA4x;

	[Tooltip("Used instead of m_AAQuality when running on Steam Deck (Van Gogh APU detected).\nTAA produces severe ghosting on Deck's Mesa Vulkan + low FPS pipeline, so per-preset override is provided.")]
	public CPMGraphicsSettings.AAQuality m_SteamDeckAAQuality = CPMGraphicsSettings.AAQuality.FXAA;

	public CPMGraphicsSettings.TextureResolution m_TextureResolution;

	[Range(30f, 240f)]
	public int m_MaxFramerate = 240;

	[Tooltip("VSync on/off. When on, frame rate is synced to display refresh; m_MaxFramerate is ignored.")]
	public bool m_VSync = true;

	[HideInInspector]
	public bool m_HighFpsDefaultsApplied;

	public bool BatterySaveMode;

	[Range(60f, 3000f)]
	[Header("Objects and Rendering")]
	public float m_CameraRenderDistance = 2000f;

	[Range(0.3f, 1f)]
	public float m_ObjectsLodQuality = 1f;

	[Range(1f, 16f)]
	[Tooltip("Steam desktop LOD bias. Higher = high-detail LODs (LOD0) persist further before switching down. Overrides m_ObjectsLodQuality on Standalone (Win/Linux).")]
	public float m_SteamLodBias = 10f;

	public int m_MaximumLodLevel;

	[Range(15f, 3000f)]
	public float ClipDistance = 15f;

	[Range(0f, 100f)]
	public float CarDetailDistance = 40f;

	[Header("Lighting and Shading")]
	public CPMGraphicsSettings.FogQuality m_FogQuality = CPMGraphicsSettings.FogQuality.On;

	public CPMGraphicsSettings.ShadersQuality m_ShadingQuality = CPMGraphicsSettings.ShadersQuality.High;

	public CPMGraphicsSettings.ShadowsQuality m_ShadowsQuality;

	public CPMGraphicsSettings.AdditionalEffects m_AdditionalEffects;

	public bool m_UseDecalShadows = true;

	public bool m_UseTyreDeformation;

	public CPMGraphicsSettings.RainQuality m_RainQuality = CPMGraphicsSettings.RainQuality.All;

	public bool m_UseSky = true;

	public bool m_UseBlur;

	public bool m_UseLensFlares;

	public float m_NightAmbientLightingCompensation;

	public float m_Cloudiness = 1f;

	[Header("Reflections")]
	public CPMGraphicsSettings.RealtimeRefections m_RealtimeReflections;

	public bool m_UseStaticReflectons = true;

	[Header("Post Processing")]
	public TonemappingMode m_Tonemapping = TonemappingMode.ACES;

	public bool m_UseBloom;

	public bool m_UseVignette;

	public bool m_UseMotionBlur;

	public CPMGraphicsSettings.ColorCorrectionLut m_ColorCorrectionLut;

	public float m_EVCompensation;

	public float m_WBCompensation;

	public object Clone()
	{
		return MemberwiseClone();
	}
}
