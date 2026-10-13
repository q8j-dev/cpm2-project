using UnityEngine;

namespace _Car_Parking.Scripts
{
	public static class ShaderPropertyIDs
	{
		public static readonly int PaintType = Shader.PropertyToID("_PAINT_TYPE");

		public static readonly int IridescenceThickness = Shader.PropertyToID("_IridescenceThickness");

		public static readonly int IridescencePower = Shader.PropertyToID("_Iridescence_Power");

		public static readonly int DirtColor = Shader.PropertyToID("_MaskColor");

		public static readonly int DirtAmount = Shader.PropertyToID("_MaskValue");

		public static readonly int Shattered = Shader.PropertyToID("_Shattered");

		public static readonly int ShatterHole = Shader.PropertyToID("_ShatterHole");

		public static readonly int Shininess = Shader.PropertyToID("_Shininess");

		public static readonly int Smoothness = Shader.PropertyToID("_Smoothness");

		public static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

		public static readonly int WipersValue = Shader.PropertyToID("_WipersValue");

		public static readonly int RainOverlayValue = Shader.PropertyToID("_RainOverlayValue");

		public static readonly int RainPower = Shader.PropertyToID("_RainPower");

		public static readonly int RainStreaksPower = Shader.PropertyToID("_RainStreaksPower");

		public static readonly int InvertWipers = Shader.PropertyToID("_InvertWipers");

		public static readonly int WipersTilingOffset = Shader.PropertyToID("_WipersTilingOffset");

		public static readonly int WipersScale = Shader.PropertyToID("_WipersScale");

		public static readonly int Cull = Shader.PropertyToID("_Cull");

		public static readonly int FPRainBlend = Shader.PropertyToID("_FPRainBlend");
	}
}
