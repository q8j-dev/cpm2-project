using UnityEngine;

public static class CloudWeatherProps
{
	public static readonly int CoverageMultiplier = Shader.PropertyToID("_CoverageMultiplier");

	public static readonly int CloudTypeBias = Shader.PropertyToID("_CloudTypeBias");

	public static readonly int DensityScale = Shader.PropertyToID("_DensityScale");

	public static readonly int PrecipDensityBoost = Shader.PropertyToID("_PrecipDensityBoost");
}
