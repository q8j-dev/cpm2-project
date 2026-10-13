using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public static class UnityGraphicsBullshit
{
	private static FieldInfo MainLightCastShadows_FieldInfo;

	private static FieldInfo AdditionalLightCastShadows_FieldInfo;

	private static FieldInfo MainLightShadowmapResolution_FieldInfo;

	private static FieldInfo AdditionalLightShadowmapResolution_FieldInfo;

	private static FieldInfo Cascade2Split_FieldInfo;

	private static FieldInfo Cascade4Split_FieldInfo;

	private static FieldInfo SoftShadowsEnabled_FieldInfo;

	private static FieldInfo RenderScale_FieldInfo;

	private static FieldInfo Msaa_FieldInfo;

	private static FieldInfo ShadowCascadeCount_FieldInfo;

	private static FieldInfo SupportDataDrivenLensFlare_FieldInfo;

	private static FieldInfo ReflectionProbeBlending_FieldInfo;

	private static FieldInfo ReflectionProbeBoxProjection_FieldInfo;

	private static FieldInfo ShadowDistance_FieldInfo;

	private static FieldInfo SupportsHDR_FieldInfo;

	private static FieldInfo ColorGradingMode_FieldInfo;

	private static FieldInfo RequireOpaqueTexture_FieldInfo;

	private static FieldInfo OpaqueDownsampling_FieldInfo;

	public static bool MainLightCastShadows
	{
		get
		{
			return (bool)MainLightCastShadows_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			MainLightCastShadows_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static bool AdditionalLightCastShadows
	{
		get
		{
			return (bool)AdditionalLightCastShadows_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			AdditionalLightCastShadows_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static ShadowResolution MainLightShadowResolution
	{
		get
		{
			return (ShadowResolution)MainLightShadowmapResolution_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			MainLightShadowmapResolution_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static ShadowResolution AdditionalLightShadowResolution
	{
		get
		{
			return (ShadowResolution)AdditionalLightShadowmapResolution_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			AdditionalLightShadowmapResolution_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static float Cascade2Split
	{
		get
		{
			return (float)Cascade2Split_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			Cascade2Split_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static Vector3 Cascade4Split
	{
		get
		{
			return (Vector3)Cascade4Split_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			Cascade4Split_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static bool SoftShadowsEnabled
	{
		get
		{
			return (bool)SoftShadowsEnabled_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			SoftShadowsEnabled_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static float RenderScale
	{
		get
		{
			return (float)RenderScale_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			RenderScale_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static float ShadowDistance
	{
		get
		{
			return (float)ShadowDistance_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			ShadowDistance_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static MsaaQuality Msaa
	{
		get
		{
			return (MsaaQuality)Msaa_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			Msaa_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static int ShadowCascadeCount
	{
		get
		{
			return (int)ShadowCascadeCount_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			ShadowCascadeCount_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static bool SupportDataDrivenLensFlare
	{
		get
		{
			return (bool)SupportDataDrivenLensFlare_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			SupportDataDrivenLensFlare_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static bool ReflectionProbeBlending
	{
		get
		{
			return (bool)ReflectionProbeBlending_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			ReflectionProbeBlending_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static bool ReflectionProbeBoxProjection
	{
		get
		{
			return (bool)ReflectionProbeBoxProjection_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			ReflectionProbeBoxProjection_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static bool SupportsHDR
	{
		get
		{
			return (bool)SupportsHDR_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			SupportsHDR_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static ColorGradingMode colorGradingMode
	{
		get
		{
			return (ColorGradingMode)ColorGradingMode_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			ColorGradingMode_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static bool RequireOpaqueTexture
	{
		get
		{
			return (bool)RequireOpaqueTexture_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			RequireOpaqueTexture_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	public static Downsampling OpaqueDownsampling
	{
		get
		{
			return (Downsampling)OpaqueDownsampling_FieldInfo.GetValue(QualitySettings.renderPipeline);
		}
		set
		{
			OpaqueDownsampling_FieldInfo.SetValue(QualitySettings.renderPipeline, value);
		}
	}

	static UnityGraphicsBullshit()
	{
		Type typeFromHandle = typeof(UniversalRenderPipelineAsset);
		BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
		MainLightCastShadows_FieldInfo = typeFromHandle.GetField("m_MainLightShadowsSupported", bindingAttr);
		AdditionalLightCastShadows_FieldInfo = typeFromHandle.GetField("m_AdditionalLightShadowsSupported", bindingAttr);
		MainLightShadowmapResolution_FieldInfo = typeFromHandle.GetField("m_MainLightShadowmapResolution", bindingAttr);
		AdditionalLightShadowmapResolution_FieldInfo = typeFromHandle.GetField("m_AdditionalLightsShadowmapResolution", bindingAttr);
		Cascade2Split_FieldInfo = typeFromHandle.GetField("m_Cascade2Split", bindingAttr);
		Cascade4Split_FieldInfo = typeFromHandle.GetField("m_Cascade4Split", bindingAttr);
		SoftShadowsEnabled_FieldInfo = typeFromHandle.GetField("m_SoftShadowsSupported", bindingAttr);
		RenderScale_FieldInfo = typeFromHandle.GetField("m_RenderScale", bindingAttr);
		Msaa_FieldInfo = typeFromHandle.GetField("m_MSAA", bindingAttr);
		ShadowCascadeCount_FieldInfo = typeFromHandle.GetField("m_ShadowCascadeCount", bindingAttr);
		SupportDataDrivenLensFlare_FieldInfo = typeFromHandle.GetField("m_SupportDataDrivenLensFlare", bindingAttr);
		ReflectionProbeBlending_FieldInfo = typeFromHandle.GetField("m_ReflectionProbeBlending", bindingAttr);
		ReflectionProbeBoxProjection_FieldInfo = typeFromHandle.GetField("m_ReflectionProbeBoxProjection", bindingAttr);
		ShadowDistance_FieldInfo = typeFromHandle.GetField("m_ShadowDistance", bindingAttr);
		SupportsHDR_FieldInfo = typeFromHandle.GetField("m_SupportsHDR", bindingAttr);
		ColorGradingMode_FieldInfo = typeFromHandle.GetField("m_ColorGradingMode", bindingAttr);
		RequireOpaqueTexture_FieldInfo = typeFromHandle.GetField("m_RequireOpaqueTexture", bindingAttr);
		OpaqueDownsampling_FieldInfo = typeFromHandle.GetField("m_OpaqueDownsampling", bindingAttr);
	}
}
