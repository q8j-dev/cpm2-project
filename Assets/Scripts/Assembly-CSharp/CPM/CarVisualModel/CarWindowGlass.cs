using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using _Car_Parking.Scripts.Car;

namespace CPM.CarVisualModel
{
	public static class CarWindowGlass
	{
		private static readonly int MaskAddTex = Shader.PropertyToID("_MaskAddTex");

		private static readonly int CracksNormal = Shader.PropertyToID("_CracksNormal");

		private static readonly int ShatterTileOffset = Shader.PropertyToID("_ShatterTileOffset");

		private static readonly int WipersValue = Shader.PropertyToID("_WipersValue");

		private static readonly int RainOverlayValue = Shader.PropertyToID("_RainOverlayValue");

		private static readonly int InvertWipers = Shader.PropertyToID("_InvertWipers");

		public static Material Apply(GameObject body, Material windowBaseMaterial)
		{
			if (!body)
			{
				return null;
			}
			List<Renderer> list = CollectGlassRenderers(body);
			if (list.Count == 0)
			{
				return null;
			}
			Material sharedMaterial = list[0].sharedMaterial;
			Material material;
			if ((bool)sharedMaterial)
			{
				material = new Material(sharedMaterial)
				{
					name = "CarWindowMat"
				};
			}
			else
			{
				if (!windowBaseMaterial)
				{
					return null;
				}
				material = new Material(windowBaseMaterial)
				{
					name = "CarWindowMat"
				};
				material.EnableKeyword(new LocalKeyword(material.shader, "_USEDIRT"));
				material.SetTextureScale(MaskAddTex, new Vector2(10f, 10f));
				material.SetTextureScale(CracksNormal, new Vector2(15f, 15f));
				material.SetVector(ShatterTileOffset, Vector4.zero);
			}
			material.mainTexture = null;
			material.SetFloat(WipersValue, 0f);
			material.SetFloat(RainOverlayValue, 0f);
			material.SetFloat(InvertWipers, 0f);
			for (int i = 0; i < list.Count; i++)
			{
				list[i].sharedMaterial = material;
			}
			return material;
		}

		private static List<Renderer> CollectGlassRenderers(GameObject body)
		{
			List<Renderer> list = new List<Renderer>();
			if (body.TryGetComponent<CarBodyReferences>(out var component) && (bool)component.GlassWindow && component.GlassWindow.TryGetComponent<Renderer>(out var component2))
			{
				list.Add(component2);
			}
			if (body.TryGetComponent<BodyMarker>(out var component3) && component3.AdditionalWindowElements != null)
			{
				GameObject[] additionalWindowElements = component3.AdditionalWindowElements;
				foreach (GameObject gameObject in additionalWindowElements)
				{
					if ((bool)gameObject && gameObject.TryGetComponent<Renderer>(out var component4) && !list.Contains(component4))
					{
						list.Add(component4);
					}
				}
			}
			return list;
		}
	}
}
