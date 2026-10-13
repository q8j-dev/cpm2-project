using UnityEngine;

namespace CPM.Ghost.Core
{
	[CreateAssetMenu(menuName = "CPM/Ghost/Material Preset", fileName = "GhostMaterialPreset")]
	public sealed class GhostMaterialPreset : ScriptableObject
	{
		public Color Tint = new Color(0.5f, 0.75f, 1f, 1f);

		public Shader OverrideShader;

		public Material OverrideMaterial;

		[Range(0f, 1f)]
		public float MinAlpha = 0.1f;

		[Range(0f, 1f)]
		public float MaxAlpha = 0.35f;

		public float MinDistance = 5f;

		public float MaxDistance = 30f;

		public float AlphaForDistance(float distance)
		{
			float num = MaxDistance - MinDistance;
			float t = ((num > 0.0001f) ? Mathf.Clamp01((distance - MinDistance) / num) : 0f);
			return Mathf.Lerp(MinAlpha, MaxAlpha, t);
		}
	}
}
