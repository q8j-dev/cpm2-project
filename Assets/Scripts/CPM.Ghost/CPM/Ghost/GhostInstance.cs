using System.Collections.Generic;
using CPM.Ghost.Core;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Scripting;

namespace CPM.Ghost
{
	[Preserve]
	public sealed class GhostInstance : MonoBehaviour
	{
		private static readonly int ColorId = Shader.PropertyToID("_Color");

		private Transform _observer;

		private GhostMaterialPreset _preset;

		private readonly List<Material> _colorMaterials = new List<Material>();

		public void Setup(Transform observer, GhostMaterialPreset preset)
		{
			_observer = observer;
			_preset = preset;
			_colorMaterials.Clear();
			Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>(includeInactive: true);
			foreach (Renderer renderer in componentsInChildren)
			{
				renderer.shadowCastingMode = ShadowCastingMode.Off;
				renderer.receiveShadows = false;
				Material[] materials = renderer.materials;
				foreach (Material material in materials)
				{
					_colorMaterials.Add(material);
					if ((bool)preset.OverrideShader)
					{
						material.shader = preset.OverrideShader;
					}
					MakeURPMaterialTransparent(material);
					if (material.HasProperty(ColorId))
					{
						Color tint = preset.Tint;
						tint.a = preset.MinAlpha;
						material.SetColor(ColorId, tint);
					}
				}
				renderer.sharedMaterials = materials;
			}
			UpdateAlpha();
		}

		private void MakeURPMaterialTransparent(Material material)
		{
			material.SetFloat("_Surface", 1f);
			material.SetInt("_SrcBlend", 5);
			material.SetInt("_DstBlend", 10);
			material.SetInt("_ZWrite", 0);
			material.DisableKeyword("_ALPHATEST_ON");
			material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
			material.renderQueue = 3000;
		}

		public void UpdateAlpha()
		{
			if (!_preset || !_observer || _colorMaterials.Count == 0)
			{
				return;
			}
			float distance = Vector3.Distance(base.transform.position, _observer.position);
			float a = _preset.AlphaForDistance(distance);
			foreach (Material colorMaterial in _colorMaterials)
			{
				if ((bool)colorMaterial && colorMaterial.HasProperty(ColorId))
				{
					Color color = colorMaterial.GetColor(ColorId);
					color.a = a;
					colorMaterial.SetColor(ColorId, color);
				}
			}
		}

		private void OnDestroy()
		{
			foreach (Material colorMaterial in _colorMaterials)
			{
				if ((bool)colorMaterial)
				{
					Object.Destroy(colorMaterial);
				}
			}
		}
	}
}
