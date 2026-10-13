using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Car.Materials
{
	[DefaultExecutionOrder(-1000)]
	public class AddressableMaterialDescriptor : MonoBehaviour
	{
		[Serializable]
		public struct FloatProperty
		{
			public string name;

			public float value;
		}

		[Serializable]
		public struct IntProperty
		{
			public string name;

			public int value;
		}

		[Serializable]
		public struct ColorProperty
		{
			public string name;

			public Color value;
		}

		[Serializable]
		public struct VectorProperty
		{
			public string name;

			public Vector4 value;
		}

		[Serializable]
		public struct TextureProperty
		{
			public string name;

			public Texture texture;

			public Vector2 scale;

			public Vector2 offset;
		}

		[Serializable]
		public struct RendererSlot
		{
			public Renderer Renderer;

			public int Slot;
		}

		[Serializable]
		public class MaterialEntry
		{
			public string ShaderName;

			public string[] Keywords = Array.Empty<string>();

			public FloatProperty[] FloatProperties = Array.Empty<FloatProperty>();

			public IntProperty[] IntProperties = Array.Empty<IntProperty>();

			public ColorProperty[] ColorProperties = Array.Empty<ColorProperty>();

			public VectorProperty[] VectorProperties = Array.Empty<VectorProperty>();

			public TextureProperty[] TextureProperties = Array.Empty<TextureProperty>();

			public RendererSlot[] Targets = Array.Empty<RendererSlot>();

			public BodyMarker AssignToBodyMarker;
		}

		public MaterialEntry[] Entries = Array.Empty<MaterialEntry>();

		private readonly List<Material> _createdMaterials = new List<Material>();

		private void Awake()
		{
			Apply();
		}

		public void Apply()
		{
			foreach (Material createdMaterial in _createdMaterials)
			{
				if ((bool)createdMaterial)
				{
					UnityEngine.Object.Destroy(createdMaterial);
				}
			}
			_createdMaterials.Clear();
			Dictionary<string, Material> cache = new Dictionary<string, Material>();
			MaterialEntry[] entries = Entries;
			foreach (MaterialEntry materialEntry in entries)
			{
				if (materialEntry == null)
				{
					continue;
				}
				Material orCreateMaterial = GetOrCreateMaterial(materialEntry, cache);
				if ((bool)orCreateMaterial)
				{
					AssignToTargets(materialEntry, orCreateMaterial);
					if ((bool)materialEntry.AssignToBodyMarker)
					{
						materialEntry.AssignToBodyMarker.mainMat = orCreateMaterial;
					}
				}
			}
		}

		private Material GetOrCreateMaterial(MaterialEntry entry, Dictionary<string, Material> cache)
		{
			string key = BuildSignature(entry);
			if (cache.TryGetValue(key, out var value))
			{
				return value;
			}
			Shader shader = GlobalEventSystem.RequireShader(entry.ShaderName);
			if (!shader)
			{
				Debug.LogWarning("[AddressableMaterialDescriptor] Shader not found: '" + entry.ShaderName + "' on " + base.gameObject.name, base.gameObject);
				return null;
			}
			Material material = new Material(shader)
			{
				name = "[AMD] " + entry.ShaderName
			};
			string[] keywords = entry.Keywords;
			foreach (string text in keywords)
			{
				material.EnableKeyword(new LocalKeyword(shader, text));
			}
			FloatProperty[] floatProperties = entry.FloatProperties;
			for (int i = 0; i < floatProperties.Length; i++)
			{
				FloatProperty floatProperty = floatProperties[i];
				material.SetFloat(floatProperty.name, floatProperty.value);
			}
			IntProperty[] intProperties = entry.IntProperties;
			for (int i = 0; i < intProperties.Length; i++)
			{
				IntProperty intProperty = intProperties[i];
				material.SetInt(intProperty.name, intProperty.value);
			}
			ColorProperty[] colorProperties = entry.ColorProperties;
			for (int i = 0; i < colorProperties.Length; i++)
			{
				ColorProperty colorProperty = colorProperties[i];
				material.SetColor(colorProperty.name, colorProperty.value);
			}
			VectorProperty[] vectorProperties = entry.VectorProperties;
			for (int i = 0; i < vectorProperties.Length; i++)
			{
				VectorProperty vectorProperty = vectorProperties[i];
				material.SetVector(vectorProperty.name, vectorProperty.value);
			}
			TextureProperty[] textureProperties = entry.TextureProperties;
			for (int i = 0; i < textureProperties.Length; i++)
			{
				TextureProperty textureProperty = textureProperties[i];
				material.SetTexture(textureProperty.name, textureProperty.texture);
				if (textureProperty.scale != Vector2.zero)
				{
					material.SetTextureScale(textureProperty.name, textureProperty.scale);
				}
				if (textureProperty.offset != Vector2.zero)
				{
					material.SetTextureOffset(textureProperty.name, textureProperty.offset);
				}
			}
			cache[key] = material;
			_createdMaterials.Add(material);
			return material;
		}

		private static void AssignToTargets(MaterialEntry entry, Material mat)
		{
			RendererSlot[] targets = entry.Targets;
			for (int i = 0; i < targets.Length; i++)
			{
				RendererSlot rendererSlot = targets[i];
				Renderer renderer = rendererSlot.Renderer;
				if ((bool)renderer)
				{
					Material[] sharedMaterials = renderer.sharedMaterials;
					if (rendererSlot.Slot < 0 || rendererSlot.Slot >= sharedMaterials.Length)
					{
						Debug.LogWarning($"[AddressableMaterialDescriptor] Slot {rendererSlot.Slot} out of range (length {sharedMaterials.Length}) on {renderer.name}", renderer);
					}
					else if (!sharedMaterials[rendererSlot.Slot])
					{
						sharedMaterials[rendererSlot.Slot] = mat;
						renderer.sharedMaterials = sharedMaterials;
					}
				}
			}
		}

		private static string BuildSignature(MaterialEntry entry)
		{
			CultureInfo invariantCulture = CultureInfo.InvariantCulture;
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(entry.ShaderName).Append('|');
			string[] obj = (string[])entry.Keywords.Clone();
			Array.Sort(obj, StringComparer.Ordinal);
			string[] array = obj;
			foreach (string value in array)
			{
				stringBuilder.Append(value).Append(',');
			}
			stringBuilder.Append('|');
			FloatProperty[] floatProperties = entry.FloatProperties;
			for (int i = 0; i < floatProperties.Length; i++)
			{
				FloatProperty floatProperty = floatProperties[i];
				StringBuilder stringBuilder2 = stringBuilder.Append(floatProperty.name).Append('=');
				float value2 = floatProperty.value;
				stringBuilder2.Append(value2.ToString("R", invariantCulture)).Append(';');
			}
			IntProperty[] intProperties = entry.IntProperties;
			for (int i = 0; i < intProperties.Length; i++)
			{
				IntProperty intProperty = intProperties[i];
				StringBuilder stringBuilder3 = stringBuilder.Append(intProperty.name).Append('=');
				int value3 = intProperty.value;
				stringBuilder3.Append(value3.ToString(invariantCulture)).Append(';');
			}
			ColorProperty[] colorProperties = entry.ColorProperties;
			for (int i = 0; i < colorProperties.Length; i++)
			{
				ColorProperty colorProperty = colorProperties[i];
				StringBuilder stringBuilder4 = stringBuilder.Append(colorProperty.name).Append('=');
				Color value4 = colorProperty.value;
				stringBuilder4.Append(value4.ToString("R", invariantCulture)).Append(';');
			}
			VectorProperty[] vectorProperties = entry.VectorProperties;
			for (int i = 0; i < vectorProperties.Length; i++)
			{
				VectorProperty vectorProperty = vectorProperties[i];
				StringBuilder stringBuilder5 = stringBuilder.Append(vectorProperty.name).Append('=');
				Vector4 value5 = vectorProperty.value;
				stringBuilder5.Append(value5.ToString("R", invariantCulture)).Append(';');
			}
			TextureProperty[] textureProperties = entry.TextureProperties;
			for (int i = 0; i < textureProperties.Length; i++)
			{
				TextureProperty textureProperty = textureProperties[i];
				int value6 = (textureProperty.texture ? textureProperty.texture.GetInstanceID() : 0);
				StringBuilder stringBuilder6 = stringBuilder.Append(textureProperty.name).Append('=').Append(value6)
					.Append('@');
				Vector2 scale = textureProperty.scale;
				StringBuilder stringBuilder7 = stringBuilder6.Append(scale.ToString("R", invariantCulture)).Append('/');
				scale = textureProperty.offset;
				stringBuilder7.Append(scale.ToString("R", invariantCulture)).Append(';');
			}
			return stringBuilder.ToString();
		}

		private void OnDestroy()
		{
			foreach (Material createdMaterial in _createdMaterials)
			{
				if ((bool)createdMaterial)
				{
					UnityEngine.Object.Destroy(createdMaterial);
				}
			}
			_createdMaterials.Clear();
		}
	}
}
