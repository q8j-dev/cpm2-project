using System.Collections.Generic;
using CPM.Database;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;

namespace CPM.CarVisualModel
{
	public static class CarVisualPaint
	{
		public static Material Apply(GameObject body, CarInfo carInfo, Material carBodyBaseMaterial)
		{
			if (!body)
			{
				return null;
			}
			if (!body.TryGetComponent<BodyMarker>(out var component))
			{
				return null;
			}
			Material material = (component.mainMat ? component.mainMat : carBodyBaseMaterial);
			if (!material)
			{
				return null;
			}
			Material material2 = new Material(material)
			{
				name = "CarModelPaint"
			};
			material2.mainTexture = null;
			if (carInfo != null)
			{
				material2.SetColor(ShaderPropertyIDs.DirtColor, carInfo.Colors.DirtColor);
				ColorsService.SetCarPaintColor(material2, carInfo.Colors.CarColor);
			}
			material2.SetFloat(ShaderPropertyIDs.DirtAmount, 0f);
			ReplaceSharedMaterial(body, material, material2);
			AssignPaint(component.frontLightRight, material2);
			AssignPaint(component.frontLightLeft, material2);
			AssignPaint(component.hood, material2);
			if (component.AdditionalElements != null)
			{
				GameObject[] additionalElements = component.AdditionalElements;
				for (int i = 0; i < additionalElements.Length; i++)
				{
					AssignPaint(additionalElements[i], material2);
				}
			}
			if ((bool)component.spoiler && component.spoiler.TryGetComponent<MeshRenderer>(out var component2))
			{
				component2.sharedMaterial = material2;
			}
			return material2;
		}

		public static void PaintKit(GameObject kit, Material paintMaterial)
		{
			if ((bool)kit && (bool)paintMaterial)
			{
				Renderer renderer = KitMainRenderer(kit, out var values);
				if ((bool)renderer)
				{
					renderer.sharedMaterial = paintMaterial;
				}
				if ((bool)values)
				{
					PaintElements(values.AdditionalElements, paintMaterial);
				}
			}
		}

		public static void TintNoVinylKit(GameObject kit, BodyKitType type, CarInfo carInfo, List<Material> ownedMaterials)
		{
			if (!kit)
			{
				return;
			}
			BodyKitsColors bodyKitsColors = carInfo?.Colors?.BodyKits;
			if (bodyKitsColors == null || !bodyKitsColors.TryGet(type, out var color))
			{
				return;
			}
			Renderer renderer = KitMainRenderer(kit, out var values);
			if ((bool)renderer && (bool)renderer.sharedMaterial)
			{
				if ((bool)values && (bool)values.MainMaterial && values.render == renderer)
				{
					ColorsService.SetCarPaintColor(values.MainMaterial, color);
					return;
				}
				Material material = new Material(renderer.sharedMaterial)
				{
					name = "CarModelKitPaint"
				};
				ColorsService.SetCarPaintColor(material, color);
				renderer.sharedMaterial = material;
				ownedMaterials?.Add(material);
			}
		}

		private static Renderer KitMainRenderer(GameObject kit, out bodyKitValues values)
		{
			if (kit.TryGetComponent<bodyKitValues>(out values) && (bool)values.render)
			{
				return values.render;
			}
			return kit.GetComponentInChildren<MeshRenderer>(includeInactive: true);
		}

		private static void ReplaceSharedMaterial(GameObject root, Material from, Material to)
		{
			Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>(includeInactive: true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
				bool flag = false;
				for (int j = 0; j < sharedMaterials.Length; j++)
				{
					if (sharedMaterials[j] == from)
					{
						sharedMaterials[j] = to;
						flag = true;
					}
				}
				if (flag)
				{
					componentsInChildren[i].sharedMaterials = sharedMaterials;
				}
			}
		}

		private static void PaintElements(GameObject[] elements, Material mat)
		{
			if (elements == null)
			{
				return;
			}
			for (int i = 0; i < elements.Length; i++)
			{
				if ((bool)elements[i] && elements[i].TryGetComponent<Renderer>(out var component))
				{
					component.sharedMaterial = mat;
				}
			}
		}

		private static void AssignPaint(GameObject go, Material mat)
		{
			if ((bool)go && go.TryGetComponent<MeshRenderer>(out var component))
			{
				component.sharedMaterial = mat;
			}
		}
	}
}
