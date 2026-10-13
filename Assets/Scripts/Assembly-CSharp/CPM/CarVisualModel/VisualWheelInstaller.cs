using System.Collections.Generic;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Infrastructure;

namespace CPM.CarVisualModel
{
	public static class VisualWheelInstaller
	{
		public sealed class MaterialCache
		{
			internal readonly Dictionary<WheelData, Material> Primary = new Dictionary<WheelData, Material>();

			internal readonly Dictionary<WheelData, Material> Secondary = new Dictionary<WheelData, Material>();

			public readonly List<Material> Created = new List<Material>();
		}

		private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

		private static readonly int NormalMask = Shader.PropertyToID("_NormalMask");

		public static float TyreRadius(WheelData data)
		{
			if (data == null)
			{
				return 0.33f;
			}
			float num = (float)data.tyreWidth * 0.001f;
			float num2 = (float)data.tyreProfile;
			return (float)data.wheelSize * 0.0127f + num * num2 * 0.01f;
		}

		public static void Install(WheelProvider wheel, WheelData wheelData, Dictionary<string, GameObject> resourceCache, MaterialCache materialCache)
		{
			if (!wheel || wheelData == null)
			{
				return;
			}
			CreateRim(wheel, wheelData, resourceCache);
			if ((bool)wheel.WheelModel)
			{
				CreateTyre(wheel.WheelModel, wheelData, resourceCache);
				InstallCaliper(wheel.WheelModel, wheel, wheelData.BrakesData, resourceCache, materialCache);
				PaintRims(wheel.WheelModel, wheelData, materialCache);
				wheel.SetRimSize(wheelData.wheelSize, wheelData.rimWidth);
				wheel.SetTyre(wheelData.tyreType, wheelData.tyreWidth, wheelData.tyreProfile, wheelData.wheelSize);
				wheel.SetSpacers(wheelData.SpacerWidth);
				wheel.UpdateWheelVisual();
				if ((bool)wheel.WheelModel.BrakeDisk)
				{
					wheel.WheelModel.BrakeDisk.Resize(wheelData.BrakesData.GetBrakeDiskDiameter());
				}
				wheel.WheelModel.InstallSideDisk();
			}
		}

		private static GameObject LoadCached(Dictionary<string, GameObject> cache, string path)
		{
			if (cache != null && cache.TryGetValue(path, out var value))
			{
				return value;
			}
			GameObject gameObject = Resources.Load<GameObject>(path);
			if (cache != null)
			{
				cache[path] = gameObject;
			}
			return gameObject;
		}

		private static void CreateRim(WheelProvider wheel, WheelData data, Dictionary<string, GameObject> cache)
		{
			GameObject gameObject = LoadCached(cache, $"Rims/w{data.RimModelId}");
			if (!gameObject)
			{
				gameObject = LoadCached(cache, "Rims/w0");
				if (!gameObject)
				{
					return;
				}
			}
			if ((bool)wheel.WheelModel)
			{
				Object.Destroy(wheel.WheelModel.gameObject);
			}
			wheel.Model = Object.Instantiate(gameObject, wheel.transform);
			wheel.WheelModel.transform.localScale = ((!wheel.IsLeft) ? new Vector3(-1f, 1f, 1f) : Vector3.one);
			wheel.Model.name = $"rim{data.RimModelId}";
		}

		private static void PaintRims(WheelModel wheelModel, WheelData data, MaterialCache cache)
		{
			if (cache == null || !wheelModel)
			{
				return;
			}
			if ((bool)wheelModel.Rim && wheelModel.Rim.TryGetComponent<MeshRenderer>(out var component))
			{
				if (!cache.Primary.TryGetValue(data, out var value) || !value)
				{
					value = CreateRimMaterial(component.sharedMaterial, data.Color1);
					if ((bool)value)
					{
						cache.Primary[data] = value;
						cache.Created.Add(value);
					}
				}
				if ((bool)value)
				{
					component.sharedMaterial = value;
				}
			}
			if (!wheelModel.Rim2 || !wheelModel.Rim2.TryGetComponent<MeshRenderer>(out var component2))
			{
				return;
			}
			if (!cache.Secondary.TryGetValue(data, out var value2) || !value2)
			{
				value2 = CreateRimMaterial(component2.sharedMaterial, data.Color2);
				if ((bool)value2)
				{
					cache.Secondary[data] = value2;
					cache.Created.Add(value2);
				}
			}
			if ((bool)value2)
			{
				component2.sharedMaterial = value2;
			}
		}

		private static Material CreateRimMaterial(Material baseline, CarPaintColor color)
		{
			Shader shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/CarPaint");
			if (!shader)
			{
				return null;
			}
			Texture value = null;
			if ((bool)baseline)
			{
				value = (baseline.HasTexture(BaseMap) ? baseline.GetTexture(BaseMap) : baseline.mainTexture);
			}
			Material material = new Material(shader)
			{
				enableInstancing = true,
				name = "CarModelRim"
			};
			if (material.HasTexture(BaseMap))
			{
				material.SetTexture(BaseMap, value);
			}
			ColorsService.SetCarPaintColor(material, color);
			material.SetFloat(ShaderPropertyIDs.RainPower, 1f);
			material.SetFloat(ShaderPropertyIDs.RainStreaksPower, 0f);
			material.mainTexture = null;
			return material;
		}

		private static void CreateTyre(WheelModel wheelModel, WheelData data, Dictionary<string, GameObject> cache)
		{
			if (!wheelModel)
			{
				return;
			}
			GameObject gameObject = LoadCached(cache, $"Tires/DynamicTire_{data.tyreType}") ?? LoadCached(cache, $"Tires/DynamicTire_{TyreType.Touring}");
			if (!gameObject)
			{
				return;
			}
			if ((bool)wheelModel.Tire)
			{
				Object.Destroy(wheelModel.Tire);
			}
			wheelModel.Tire = Object.Instantiate(gameObject, wheelModel.transform);
			if (wheelModel.DualTyre)
			{
				if ((bool)wheelModel.Tire2)
				{
					Object.Destroy(wheelModel.Tire2);
				}
				wheelModel.Tire2 = Object.Instantiate(gameObject, wheelModel.transform);
				float x = (float)data.tyreWidth * 0.001f;
				wheelModel.Tire2.transform.localPosition = new Vector3(x, 0f, 0f);
			}
		}

		private static void InstallCaliper(WheelModel wheelModel, WheelProvider wheel, BrakesData brakes, Dictionary<string, GameObject> cache, MaterialCache materialCache)
		{
			DynamicBrakeDisk component;
			if (!wheelModel.BrakeDisk)
			{
				GameObject gameObject = LoadCached(cache, "rotorr");
				if (!gameObject)
				{
					return;
				}
				GameObject gameObject2 = Object.Instantiate(gameObject, wheelModel.transform);
				if (!gameObject2.TryGetComponent<DynamicBrakeDisk>(out component))
				{
					Object.Destroy(gameObject2);
					return;
				}
				component.Init(wheel);
				wheelModel.BrakeDisk = component;
				wheelModel.BrakeDiskGm = gameObject2;
			}
			else
			{
				component = wheelModel.BrakeDisk;
				component.Init(wheel);
			}
			if ((bool)component.CaliperModel)
			{
				Object.Destroy(component.CaliperModel);
			}
			GameObject gameObject3 = LoadCached(cache, $"Calipers/caliper_{brakes.CaliperMeshIndex}");
			if ((bool)gameObject3)
			{
				GameObject gameObject4 = Object.Instantiate(gameObject3, component.transform);
				gameObject4.transform.localScale = Vector3.one;
				component.CaliperModel = gameObject4;
			}
			wheelModel.BrakeDiskMaterial = component.transform.GetChild(0).GetComponent<Renderer>().material;
			Color value = new Color(190f, 0f, 0f, 255f) * Mathf.LinearToGammaSpace(-8f);
			wheelModel.BrakeDiskMaterial.SetColor(ShaderPropertyIDs.EmissionColor, value);
			InstallBrakeDiskTexture(wheelModel, brakes, materialCache);
		}

		private static void InstallBrakeDiskTexture(WheelModel wheelModel, BrakesData brakes, MaterialCache materialCache)
		{
			if ((bool)wheelModel.BrakeDisk && (bool)wheelModel.BrakeDisk.DiskModel)
			{
				Material material = wheelModel.BrakeDisk.DiskModel.material;
				if (materialCache != null && material != wheelModel.BrakeDiskMaterial && !materialCache.Created.Contains(material))
				{
					materialCache.Created.Add(material);
				}
				material.SetTexture(BaseMap, Resources.Load<Texture>($"Brakes/{brakes.DiscType}/texture"));
				material.SetTexture(NormalMask, Resources.Load<Texture>($"Brakes/{brakes.DiscType}/normalMap"));
			}
		}
	}
}
