using System;
using System.Collections.Generic;
using CPMEngine;
using CodeStage.AntiCheat.Storage;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;

namespace CPM.Database.Constructors
{
	public class WheelConstructor
	{
		public static DynamicCanvasTire DynamicCanvasTire;

		public static DynamicCanvasNormalTire DynamicCanvasNormal;

		private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

		private static readonly int NormalMask = Shader.PropertyToID("_NormalMask");

		public static async UniTaskVoid SetWheel(VehicleData vehicleData, CarTuningData carTuningData, int waitTime = 0, bool randomWheelColor = false)
		{
			if ((bool)vehicleData && (!vehicleData.mppc || !vehicleData.mppc.WheelsLoaded || !vehicleData.mppc.isOnlineCar))
			{
				if ((bool)vehicleData.mppc)
				{
					vehicleData.mppc.WheelsLoaded = true;
				}
				Debug.Log("SetWheel  started" + vehicleData);
				if (waitTime > 0)
				{
					await UniTask.Delay(waitTime);
				}
				if (randomWheelColor)
				{
					CarPaintColor carPaintColor = new CarPaintColor
					{
						Color = UnityEngine.Random.ColorHSV(),
						CarMatType = (CarMatType)UnityEngine.Random.Range(0, 6)
					};
					carTuningData.FrontWheelData.Color1 = carPaintColor;
					carTuningData.FrontWheelData.Color2 = carPaintColor;
					carTuningData.RearWheelData.Color1 = carPaintColor;
					carTuningData.RearWheelData.Color2 = carPaintColor;
				}
				SetWheelModel(vehicleData.gameObject, AxleType.Front, carTuningData.FrontWheelData, waitTime).Forget();
				if (waitTime > 0)
				{
					await UniTask.Delay(waitTime);
				}
				SetWheelModel(vehicleData.gameObject, AxleType.Rear, carTuningData.RearWheelData, waitTime).Forget();
				SetWheelModel(vehicleData.gameObject, AxleType.Rear1, carTuningData.RearWheelData, waitTime).Forget();
				SetWheelModel(vehicleData.gameObject, AxleType.Rear2, carTuningData.RearWheelData, waitTime).Forget();
			}
		}

		public static async UniTaskVoid SetWheelModel(GameObject car, AxleType axleType, WheelData wheelData, int waitTime = 0)
		{
			if ((bool)car && car.TryGetComponent<Axles>(out var component) && (component.Rear1Axle.Wheels.Count != 0 || axleType != AxleType.Rear1) && (component.Rear2Axle.Wheels.Count != 0 || axleType != AxleType.Rear2))
			{
				await InstallWheelModel(car, axleType switch
				{
					AxleType.Front => component.FrontAxle, 
					AxleType.Rear => component.RearAxle, 
					AxleType.Rear1 => component.Rear1Axle, 
					AxleType.Rear2 => component.Rear2Axle, 
					_ => throw new ArgumentOutOfRangeException("axleType", axleType, "Axle is null"), 
				}, wheelData, axleType, waitTime);
			}
		}

		public static void SetTrailerWheelModel(GameObject car, WheelData wheelData)
		{
			if (car.TryGetComponent<Axles>(out var component))
			{
				if (component.RearAxle.Wheels.Count > 0)
				{
					InstallTrailerWheelModel(car, component.RearAxle, wheelData);
				}
				if (component.Rear1Axle.Wheels.Count > 0)
				{
					InstallTrailerWheelModel(car, component.Rear1Axle, wheelData);
				}
				if (component.Rear2Axle.Wheels.Count > 0)
				{
					InstallTrailerWheelModel(car, component.Rear2Axle, wheelData);
				}
			}
		}

		public static void SetBrakesTexture(AxleType axleType, GameObject car, BrakesData brakesData)
		{
			if (!car.TryGetComponent<Axles>(out var component))
			{
				return;
			}
			Axle obj = ((axleType == AxleType.Front) ? component.FrontAxle : component.RearAxle);
			Texture diskTexture = Resources.Load<Texture>($"Brakes/{brakesData.DiscType}/texture");
			Texture diskNormal = Resources.Load<Texture>($"Brakes/{brakesData.DiscType}/normalMap");
			foreach (WheelProvider wheel in obj.Wheels)
			{
				InstallBrakeDiskTexture(wheel.WheelModel, brakesData, diskTexture, diskNormal);
			}
			if (!component.DualAxles || axleType != AxleType.Rear)
			{
				return;
			}
			if (component.Rear1Axle.Wheels.Count > 0)
			{
				foreach (WheelProvider wheel2 in component.Rear1Axle.Wheels)
				{
					InstallBrakeDiskTexture(wheel2.WheelModel, brakesData, diskTexture, diskNormal);
				}
			}
			if (component.Rear2Axle.Wheels.Count <= 0)
			{
				return;
			}
			foreach (WheelProvider wheel3 in component.Rear2Axle.Wheels)
			{
				InstallBrakeDiskTexture(wheel3.WheelModel, brakesData, diskTexture, diskNormal);
			}
		}

		public static void SetCalipersMesh(AxleType axleType, GameObject car, BrakesData brakesData)
		{
			if (!car.TryGetComponent<Axles>(out var component))
			{
				return;
			}
			Axle obj = ((axleType == AxleType.Front) ? component.FrontAxle : component.RearAxle);
			GameObject caliperPrefab = Resources.Load<GameObject>($"Calipers/caliper_{brakesData.CaliperMeshIndex}");
			foreach (WheelProvider wheel in obj.Wheels)
			{
				InstallBrakeCaliperModel(wheel.WheelModel, wheel, brakesData, caliperPrefab);
			}
			if (!component.DualAxles || axleType != AxleType.Rear)
			{
				return;
			}
			if (component.Rear1Axle.Wheels.Count > 0)
			{
				foreach (WheelProvider wheel2 in component.Rear1Axle.Wheels)
				{
					InstallBrakeCaliperModel(wheel2.WheelModel, wheel2, brakesData, caliperPrefab);
				}
			}
			if (component.Rear2Axle.Wheels.Count <= 0)
			{
				return;
			}
			foreach (WheelProvider wheel3 in component.Rear2Axle.Wheels)
			{
				InstallBrakeCaliperModel(wheel3.WheelModel, wheel3, brakesData, caliperPrefab);
			}
		}

		private static float CalculateBrakesSize(WheelData wheelData)
		{
			float num = Mathf.Floor((float)wheelData.wheelSize * 25.4f * 0.95f);
			if (!(wheelData.BrakesData.Diameter > num))
			{
				return wheelData.BrakesData.Diameter;
			}
			return num;
		}

		public static void SetWheelVisual(AxleType axleType, GameObject car, WheelData wheelData, bool updateTyreType = false)
		{
			if (!car.TryGetComponent<Axles>(out var component))
			{
				return;
			}
			Axle obj = ((axleType == AxleType.Front) ? component.FrontAxle : component.RearAxle);
			GameObject tyrePrefab = (updateTyreType ? LoadTyrePrefab(wheelData.tyreType) : null);
			foreach (WheelProvider wheel in obj.Wheels)
			{
				UpdateWheelVisual(wheel, wheelData, updateTyreType, tyrePrefab);
			}
			if (component.DualAxles && axleType == AxleType.Rear)
			{
				if (component.Rear1Axle.Wheels.Count > 0)
				{
					UpdateWheelVisual(component.Rear1Axle.LeftWheel, wheelData, updateTyreType, tyrePrefab);
					UpdateWheelVisual(component.Rear1Axle.RightWheel, wheelData, updateTyreType, tyrePrefab);
				}
				if (component.Rear2Axle.Wheels.Count > 0)
				{
					UpdateWheelVisual(component.Rear2Axle.LeftWheel, wheelData, updateTyreType, tyrePrefab);
					UpdateWheelVisual(component.Rear2Axle.RightWheel, wheelData, updateTyreType, tyrePrefab);
				}
			}
		}

		private static void UpdateWheelVisual(WheelProvider wheel, WheelData wheelData, bool updateTyreType, GameObject tyrePrefab = null)
		{
			if (updateTyreType)
			{
				CreateTyreModel(wheel.WheelModel, wheelData, tyrePrefab);
			}
			wheel.SetRimSize(wheelData.wheelSize, wheelData.rimWidth);
			wheel.SetTyre(wheelData.tyreType, wheelData.tyreWidth, wheelData.tyreProfile, wheelData.wheelSize);
			wheelData.BrakesData.Diameter = CalculateBrakesSize(wheelData);
			wheel.WheelModel.BrakeDisk.Resize(wheelData.BrakesData.GetBrakeDiskDiameter());
			wheel.SetSpacers(wheelData.SpacerWidth);
			wheel.UpdateWheelVisual();
		}

		public static void ChangeBrakeSize(AxleType axleType, GameObject car, float value, BrakesData brakesData)
		{
			if (!car.TryGetComponent<Axles>(out var component))
			{
				return;
			}
			foreach (WheelProvider wheel in ((axleType == AxleType.Front) ? component.FrontAxle : component.RearAxle).Wheels)
			{
				brakesData.Diameter = value;
				wheel.WheelModel.BrakeDisk.Resize(brakesData.GetBrakeDiskDiameter());
			}
			if (component.DualAxles && axleType == AxleType.Rear)
			{
				if (component.Rear1Axle.Wheels.Count > 0)
				{
					component.Rear1Axle.LeftWheel.WheelModel.BrakeDisk.Resize(brakesData.GetBrakeDiskDiameter());
					component.Rear1Axle.RightWheel.WheelModel.BrakeDisk.Resize(brakesData.GetBrakeDiskDiameter());
				}
				if (component.Rear2Axle.Wheels.Count > 0)
				{
					component.Rear2Axle.LeftWheel.WheelModel.BrakeDisk.Resize(brakesData.GetBrakeDiskDiameter());
					component.Rear2Axle.RightWheel.WheelModel.BrakeDisk.Resize(brakesData.GetBrakeDiskDiameter());
				}
			}
		}

		private static GameObject LoadRimPrefab(int rimModelId)
		{
			GameObject gameObject = Resources.Load<GameObject>($"Rims/w{rimModelId}");
			if (!gameObject)
			{
				Debug.LogWarning($"[WheelConstructor] Missing rim prefab Rims/w{rimModelId} — falling back to w0");
				gameObject = Resources.Load<GameObject>("Rims/w0");
			}
			return gameObject;
		}

		private static GameObject LoadTyrePrefab(TyreType tyreType)
		{
			return Resources.Load<GameObject>($"Tires/DynamicTire_{tyreType}") ?? Resources.Load<GameObject>($"Tires/DynamicTire_{TyreType.Touring}");
		}

		private static void CreateSellRimModel(SellCarWheel wheel, WheelData wheelMainData)
		{
			if ((bool)wheel.WheelModel)
			{
				UnityEngine.Object.Destroy(wheel.WheelModel.gameObject);
			}
			wheel.Model = UnityEngine.Object.Instantiate(LoadRimPrefab(wheelMainData.RimModelId), wheel.transform);
			wheel.WheelModel = wheel.Model.GetComponent<WheelModel>();
			wheel.WheelModel.transform.localScale = ((!wheel.IsLeft) ? new Vector3(-1f, 1f, 1f) : Vector3.one);
			wheel.Model.name = $"rim{wheelMainData.RimModelId}";
		}

		private static GameObject CreateRimModel(WheelProvider wheel, WheelData wheelMainData, GameObject rimPrefab = null)
		{
			GameObject gameObject = (rimPrefab ? rimPrefab : LoadRimPrefab(wheelMainData.RimModelId));
			if (!gameObject)
			{
				return null;
			}
			if ((bool)wheel.WheelModel)
			{
				UnityEngine.Object.Destroy(wheel.WheelModel.gameObject);
			}
			wheel.Model = UnityEngine.Object.Instantiate(gameObject, wheel.transform);
			wheel.WheelModel.transform.localScale = ((!wheel.IsLeft) ? new Vector3(-1f, 1f, 1f) : Vector3.one);
			wheel.Model.name = $"rim{wheelMainData.RimModelId}";
			return wheel.Model;
		}

		private static void InstallTrailerWheelModel(GameObject car, Axle axle, WheelData wheelMainData)
		{
			Material material = null;
			Material material2 = null;
			Material wheelMatLow = null;
			GameObject rimPrefab = LoadRimPrefab(wheelMainData.RimModelId);
			GameObject tyrePrefab = LoadTyrePrefab(wheelMainData.tyreType);
			GameObject caliperPrefab = Resources.Load<GameObject>($"Calipers/caliper_{wheelMainData.BrakesData.CaliperMeshIndex}");
			Texture diskTexture = Resources.Load<Texture>($"Brakes/{wheelMainData.BrakesData.DiscType}/texture");
			Texture diskNormal = Resources.Load<Texture>($"Brakes/{wheelMainData.BrakesData.DiscType}/normalMap");
			GameObject lodPrefab = Resources.Load<GameObject>($"WheelLow/wheelLow {wheelMainData.RimModelId}");
			foreach (WheelProvider wheel in axle.Wheels)
			{
				wheel.transform.localScale = Vector3.one;
				GameObject newWheel = CreateRimModel(wheel, wheelMainData, rimPrefab);
				CreateTyreModel(wheel.WheelModel, wheelMainData, tyrePrefab);
				InstallBrakeCaliperModel(wheel.WheelModel, wheel, wheelMainData.BrakesData, caliperPrefab);
				InstallBrakeDiskTexture(wheel.WheelModel, wheelMainData.BrakesData, diskTexture, diskNormal);
				if ((bool)wheel.WheelModel.Rim)
				{
					MeshRenderer component = wheel.WheelModel.Rim.GetComponent<MeshRenderer>();
					if (!material)
					{
						material = CreateWheelMat(component.material, wheelMainData.Color1);
						material.name = $"firstWheel{axle}";
						component.material = material;
						wheelMatLow = CreateLowWheelMaterial(wheelMatLow, material);
					}
				}
				SetLodWheel(newWheel, wheelMainData.RimModelId, wheelMatLow, lodPrefab);
				if ((bool)wheel.WheelModel.Rim2)
				{
					MeshRenderer component2 = wheel.WheelModel.Rim.GetComponent<MeshRenderer>();
					if (!material2)
					{
						material2 = CreateWheelMat(component2.material, wheelMainData.Color2);
						material2.name = $"secondWheel{axle}";
						component2.material = material2;
					}
				}
				wheel.WheelModel.InstallSideDisk();
				wheel.SetRimSize(wheelMainData.wheelSize, wheelMainData.rimWidth);
				wheel.SetTyre(wheelMainData.tyreType, wheelMainData.tyreWidth, wheelMainData.tyreProfile, wheelMainData.wheelSize);
				wheel.WheelModel.BrakeDisk.Resize(wheelMainData.BrakesData.GetBrakeDiskDiameter());
				wheel.UpdateWheelVisual();
				Utils.SetLpAnchor(car.transform, wheel.transform.gameObject);
			}
			if ((bool)material)
			{
				material.mainTexture = null;
			}
			if ((bool)material2)
			{
				material2.mainTexture = null;
			}
		}

		private static Material CreateLowWheelMaterial(Material wheelMatLow, Material wheelMat)
		{
			if (!wheelMatLow)
			{
				wheelMatLow = new Material(ColorsService.LowWheelMaterial);
				wheelMatLow.color = wheelMat.color;
				wheelMatLow.name = wheelMat.name + " LOW";
				Texture value = (wheelMat.HasTexture(BaseMap) ? wheelMat.GetTexture(BaseMap) : wheelMat.mainTexture);
				if (wheelMatLow.HasTexture(BaseMap))
				{
					wheelMatLow.SetTexture(BaseMap, value);
				}
				Color color = wheelMatLow.color;
				color.a = 1f;
				wheelMatLow.color = color;
			}
			return wheelMatLow;
		}

		private static void CreateTyreModel(WheelModel wheelModel, WheelData wheelMainData, GameObject tyrePrefab = null)
		{
			if (!wheelModel)
			{
				return;
			}
			GameObject gameObject = (tyrePrefab ? tyrePrefab : LoadTyrePrefab(wheelMainData.tyreType));
			if (!gameObject)
			{
				Debug.LogWarning($"[WheelConstructor] Missing tyre prefab for TyreType={wheelMainData.tyreType}");
				return;
			}
			if ((bool)wheelModel.Tire)
			{
				UnityEngine.Object.Destroy(wheelModel.Tire);
			}
			wheelModel.Tire = UnityEngine.Object.Instantiate(gameObject, wheelModel.transform);
			if (wheelModel.DualTyre)
			{
				if ((bool)wheelModel.Tire2)
				{
					UnityEngine.Object.Destroy(wheelModel.Tire2);
				}
				wheelModel.Tire2 = UnityEngine.Object.Instantiate(gameObject, wheelModel.transform);
				float x = (float)wheelMainData.tyreWidth * 0.001f;
				wheelModel.Tire2.transform.localPosition = new Vector3(x, 0f, 0f);
			}
		}

		private static async UniTask InstallWheelModel(GameObject car, Axle axle, WheelData wheelMainData, AxleType axleType, int waitTime = 0, Material wheelMat = null, Material wheelMat2 = null, Material wheelMatLow = null)
		{
			bool ignoreSetCarDataMaterial = wheelMat;
			if (car.TryGetComponent<VehicleData>(out var component) && !TyreClassRules.IsTyreAllowed(wheelMainData.tyreType, component.CarId))
			{
				TyreType tyreType = TyreClassRules.Sanitize(wheelMainData.tyreType, component.CarId);
				Debug.LogWarning($"[WheelConstructor] {wheelMainData.tyreType} not allowed on car {component.CarId} — falling back to {tyreType}");
				wheelMainData.tyreType = tyreType;
			}
			GameObject rimPrefab = LoadRimPrefab(wheelMainData.RimModelId);
			GameObject tyrePrefab = LoadTyrePrefab(wheelMainData.tyreType);
			GameObject caliperPrefab = Resources.Load<GameObject>($"Calipers/caliper_{wheelMainData.BrakesData.CaliperMeshIndex}");
			Texture brakeTexture = Resources.Load<Texture>($"Brakes/{wheelMainData.BrakesData.DiscType}/texture");
			Texture brakeNormal = Resources.Load<Texture>($"Brakes/{wheelMainData.BrakesData.DiscType}/normalMap");
			GameObject wheelLodPrefab = Resources.Load<GameObject>($"WheelLow/wheelLow {wheelMainData.RimModelId}");
			foreach (WheelProvider wheel in axle.Wheels)
			{
				wheel.transform.localScale = Vector3.one;
				GameObject newWheel = CreateRimModel(wheel, wheelMainData, rimPrefab);
				CreateTyreModel(wheel.WheelModel, wheelMainData, tyrePrefab);
				InstallBrakeCaliperModel(wheel.WheelModel, wheel, wheelMainData.BrakesData, caliperPrefab);
				InstallBrakeDiskTexture(wheel.WheelModel, wheelMainData.BrakesData, brakeTexture, brakeNormal);
				if ((bool)wheel.WheelModel.Rim)
				{
					MeshRenderer component2 = wheel.WheelModel.Rim.GetComponent<MeshRenderer>();
					if (!wheelMat)
					{
						wheelMat = CreateWheelMat(component2.material, wheelMainData.Color1);
						wheelMat.name = $"firstWheel{axleType}";
						wheelMatLow = CreateLowWheelMaterial(wheelMatLow, wheelMat);
					}
					component2.material = wheelMat;
				}
				SetLodWheel(newWheel, wheelMainData.RimModelId, wheelMatLow, wheelLodPrefab);
				if ((bool)wheel.WheelModel.Rim2)
				{
					MeshRenderer component3 = wheel.WheelModel.Rim2.GetComponent<MeshRenderer>();
					if (!wheelMat2)
					{
						wheelMat2 = CreateWheelMat(component3.material, wheelMainData.Color2);
						wheelMat2.name = $"secondWheel{axleType}";
					}
					component3.material = wheelMat2;
				}
				if ((bool)wheel.WheelModel.SpinRimEffect && wheel.WheelModel.SpinRimEffect.TryGetComponent<WheelSpinEffect>(out var component4))
				{
					component4.Init(wheel);
				}
				if ((bool)wheel.WheelModel.Tire && wheel.WheelModel.Tire.TryGetComponent<DynamicTire>(out var component5))
				{
					component5.Init(wheel);
				}
				wheel.SetRimSize(wheelMainData.wheelSize, wheelMainData.rimWidth);
				wheel.SetTyre(wheelMainData.tyreType, wheelMainData.tyreWidth, wheelMainData.tyreProfile, wheelMainData.wheelSize);
				wheel.WheelModel.BrakeDisk.Resize(wheelMainData.BrakesData.GetBrakeDiskDiameter());
				wheel.UpdateWheelVisual();
				wheel.SetSpacers(wheelMainData.SpacerWidth);
				Utils.SetLpAnchor(car.transform, wheel.transform.gameObject);
				wheel.WheelModel.InstallSideDisk();
				if (waitTime != 0)
				{
					await UniTask.Delay(waitTime);
				}
			}
			if ((bool)wheelMat)
			{
				wheelMat.mainTexture = null;
			}
			if ((bool)wheelMat2)
			{
				wheelMat2.mainTexture = null;
			}
			if (!car || !car.TryGetComponent<VehicleData>(out var component6))
			{
				return;
			}
			GlobalEventSystem.InvokeCarWheelsInstalled(component6);
			if (!ignoreSetCarDataMaterial)
			{
				component6.WheelComposition.RegisterRuntimeWheelMaterial(wheelMat);
				component6.WheelComposition.RegisterRuntimeWheelMaterial(wheelMat2);
				component6.WheelComposition.RegisterRuntimeWheelMaterial(wheelMatLow);
			}
			component6.WheelComposition.WheelMatLod = wheelMatLow;
			if (!ignoreSetCarDataMaterial)
			{
				switch (axleType)
				{
				case AxleType.Front:
					component6.WheelComposition.WheelMatFront1 = wheelMat;
					component6.WheelComposition.WheelMatFront2 = wheelMat2;
					break;
				case AxleType.Rear:
					component6.WheelComposition.WheelMatRear1 = wheelMat;
					component6.WheelComposition.WheelMatRear2 = wheelMat2;
					break;
				case AxleType.Rear1:
				case AxleType.Rear2:
					component6.WheelComposition.SetExtraRearMaterials(axleType, wheelMat, wheelMat2);
					break;
				}
			}
		}

		private static void InstallBrakeCaliperModel(WheelModel wheelModel, SellCarWheel wheel, BrakesData brakesData, GameObject caliperPrefab = null)
		{
			InstallBrakeCaliperModelCore(wheelModel, (DynamicBrakeDisk disk) =>
			{
				disk.Init(wheel);
			}, brakesData, caliperPrefab);
		}

		private static void InstallBrakeCaliperModel(WheelModel wheelModel, WheelProvider wheel, BrakesData brakesData, GameObject caliperPrefab = null)
		{
			InstallBrakeCaliperModelCore(wheelModel, (DynamicBrakeDisk disk) =>
			{
				disk.Init(wheel);
			}, brakesData, caliperPrefab);
		}

		private static void InstallBrakeCaliperModelCore(WheelModel wheelModel, Action<DynamicBrakeDisk> init, BrakesData brakesData, GameObject caliperPrefab = null)
		{
			DynamicBrakeDisk component;
			if (!wheelModel.BrakeDisk)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load<GameObject>("rotorr"), wheelModel.transform);
				if (gameObject.TryGetComponent<DynamicBrakeDisk>(out component))
				{
					init(component);
					wheelModel.BrakeDisk = component;
				}
				wheelModel.BrakeDiskGm = gameObject;
			}
			else
			{
				component = wheelModel.BrakeDisk;
				init(component);
				component.Resize(brakesData.GetBrakeDiskDiameter());
			}
			if ((bool)component.CaliperModel)
			{
				UnityEngine.Object.Destroy(component.CaliperModel);
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(caliperPrefab ? caliperPrefab : Resources.Load<GameObject>($"Calipers/caliper_{brakesData.CaliperMeshIndex}"), component.transform);
			gameObject2.transform.localScale = Vector3.one;
			component.CaliperModel = gameObject2;
			wheelModel.BrakeDiskMaterial = component.transform.GetChild(0).GetComponent<Renderer>().material;
			Color value = new Color(190f, 0f, 0f, 255f) * Mathf.LinearToGammaSpace(-8f);
			wheelModel.BrakeDiskMaterial.SetColor(ShaderPropertyIDs.EmissionColor, value);
		}

		private static void InstallBrakeDiskTexture(WheelModel wheelModel, BrakesData brakesData, Texture diskTexture = null, Texture diskNormal = null)
		{
			if (!wheelModel || !wheelModel.BrakeDisk || !wheelModel.BrakeDisk.DiskModel)
			{
				return;
			}
			Material material = wheelModel.BrakeDisk.DiskModel.material;
			if (material != wheelModel.BrakeDiskMaterial)
			{
				if (wheelModel.RuntimeMaterials == null)
				{
					wheelModel.RuntimeMaterials = new List<Material>();
				}
				if (!wheelModel.RuntimeMaterials.Contains(material))
				{
					wheelModel.RuntimeMaterials.Add(material);
				}
			}
			material.SetTexture(BaseMap, diskTexture ? diskTexture : Resources.Load<Texture>($"Brakes/{brakesData.DiscType}/texture"));
			material.SetTexture(NormalMask, diskNormal ? diskNormal : Resources.Load<Texture>($"Brakes/{brakesData.DiscType}/normalMap"));
		}

		public static void InstallSellCarWheel(SellCarWheel wheel, WheelData wheelMainData, SellCarValues sellCarValues)
		{
			wheel.transform.localScale = Vector3.one;
			CreateSellRimModel(wheel, wheelMainData);
			CreateTyreModel(wheel.WheelModel, wheelMainData);
			InstallBrakeCaliperModel(wheel.WheelModel, wheel, wheelMainData.BrakesData);
			InstallBrakeDiskTexture(wheel.WheelModel, wheelMainData.BrakesData);
			wheel.WheelModel.SpacerWidth = wheelMainData.SpacerWidth;
			if ((bool)wheel.WheelModel.Rim)
			{
				MeshRenderer component = wheel.WheelModel.Rim.GetComponent<MeshRenderer>();
				if (!wheel.RimMaterial1)
				{
					wheel.RimMaterial1 = CreateWheelMat(component.material, wheelMainData.Color1);
					wheel.RimMaterial1.name = $"{wheelMainData}";
					component.material = wheel.RimMaterial1;
				}
			}
			if ((bool)wheel.WheelModel.Rim2)
			{
				MeshRenderer component2 = wheel.WheelModel.Rim2.GetComponent<MeshRenderer>();
				if (!wheel.RimMaterial2)
				{
					wheel.RimMaterial2 = CreateWheelMat(component2.material, wheelMainData.Color2);
					wheel.RimMaterial2.name = $"{wheelMainData}_2";
					component2.material = wheel.RimMaterial2;
				}
			}
			wheel.WheelModel.BrakeDisk.Resize(wheelMainData.BrakesData.GetBrakeDiskDiameter());
			wheel.SetSize(wheelMainData.tyreWidth, wheelMainData.tyreProfile, wheelMainData.wheelSize, wheelMainData.rimWidth, sellCarValues);
			wheel.WheelModel.InstallSideDisk();
		}

		private static Material CreateWheelMat(Material mat, CarPaintColor carPaintColor)
		{
			Shader shader = GlobalEventSystem.RequireShader("CPM/URP/Automotive/CarPaint");
			Texture value = (mat.HasTexture(BaseMap) ? mat.GetTexture(BaseMap) : mat.mainTexture);
			Material material = new Material(shader)
			{
				enableInstancing = true
			};
			if (material.HasTexture(BaseMap))
			{
				material.SetTexture(BaseMap, value);
			}
			ColorsService.SetCarPaintColor(material, carPaintColor);
			material.SetFloat(ShaderPropertyIDs.RainPower, 1f);
			material.SetFloat(ShaderPropertyIDs.RainStreaksPower, 0f);
			return material;
		}

		private static void ResetChildrenRotation(Transform parentTransform)
		{
			foreach (Transform item in parentTransform)
			{
				item.localEulerAngles = Vector3.zero;
			}
		}

		private static void SetLodWheel(GameObject newWheel, int wheelID, Material wheelMatLow, GameObject lodPrefab = null)
		{
			GameObject gameObject = (lodPrefab ? lodPrefab : Resources.Load<GameObject>($"WheelLow/wheelLow {wheelID}"));
			if (!gameObject)
			{
				Debug.LogError("Can't find lod wheel: " + wheelID);
				return;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate(gameObject, newWheel.transform);
			MeshRenderer component = gameObject2.GetComponent<MeshRenderer>();
			gameObject2.SetActive(value: false);
			wheelMatLow.mainTexture = component.material.mainTexture;
			Color color = wheelMatLow.color;
			color.a = 1f;
			wheelMatLow.color = color;
			component.material = wheelMatLow;
			if (newWheel.TryGetComponent<WheelModel>(out var component2))
			{
				component2.LodModel = gameObject2;
			}
		}

		public static void SetWheelScaleTrafficCar(GameObject carInput, float s)
		{
			if (carInput.TryGetComponent<Axles>(out var component))
			{
				foreach (WheelProvider allWheel in component.AllWheels)
				{
					if ((bool)allWheel.Model)
					{
						allWheel.Model.transform.localScale = new Vector3(s, s, s);
					}
				}
				return;
			}
			if (carInput.TryGetComponent<TrafficWaypointControl>(out var component2))
			{
				Transform[] wheels = component2.wheels;
				for (int i = 0; i < wheels.Length; i++)
				{
					wheels[i].localScale = new Vector3(s, s, s);
				}
			}
		}

		public static void SetWheel(VehicleData carData, int wheelId)
		{
			if (!carData)
			{
				return;
			}
			int carId = carData.CarId;
			if (!carData.isTruck && carId != 155 && carId != 165)
			{
				if (wheelId == -1)
				{
					wheelId = ObscuredPrefs.GetInt($"wheelToSet{V.CarId}", UnityEngine.Random.Range(0, 49));
				}
				SetWheelScaleTrafficCar(carData.gameObject, 1f);
			}
		}
	}
}
