using UnityEngine;
using UnityEngine.Rendering;

public class CPMGraphicsSettings : MonoBehaviour
{
	public enum FogQuality
	{
		Off = 0,
		On = 1
	}

	public enum ShadersQuality
	{
		Ultra = 0,
		High = 1,
		Potato = 2
	}

	public enum AAQuality
	{
		MSAA8x = 0,
		MSAA4x = 1,
		MSAA2x = 2,
		TAA = 3,
		SMAA = 4,
		FXAA = 5,
		Off = 6
	}

	public enum ShadowsQuality
	{
		Ultra = 4,
		High = 0,
		Mid = 1,
		Low = 2,
		Off = 3
	}

	public enum StaticReflectionsQuality
	{
		High = 0,
		Mid = 1,
		Low = 2
	}

	public enum TextureResolution
	{
		High = 0,
		Mid = 1,
		Low = 2,
		Lowest = 3
	}

	public enum AdditionalEffects
	{
		High = 0,
		Mid = 1,
		Low = 2
	}

	public enum DecalShadowQuality
	{
		Ultra = 0,
		High = 1,
		Off = 2
	}

	public enum ColorCorrectionLut
	{
		Off = 0,
		Williams = 1,
		Carter = 2,
		Vance = 3,
		Bullock = 4
	}

	public enum RealtimeRefections
	{
		Off = 0,
		Medium = 1,
		High = 2
	}

	public enum RainQuality
	{
		Off = 0,
		Minimal = 1,
		All = 2
	}

	public GlobalKeyword RealtimeReflectionKeyword;

	public GlobalKeyword ShaderQualityKeyword;

	public static void GenerateShaderKeywords()
	{
	}
}
