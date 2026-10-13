using System;
using System.Collections.Generic;
using System.Threading;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using VContainer;
using _Car_Parking.Scripts.Car.Configs;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Police;

namespace CPM.CarVisualModel
{
	public sealed class CarVisualModelBuilder : ICarVisualModelBuilder
	{
		private const string LibraryResourcePath = "Configs/CarModels/CarModelLibrary";

		public const string RootPrefix = "CarVisualModel_";

		public const string ContentName = "Content";

		public const float WheelRestDropMeters = 0.2f;

		public const float GroundSquashMeters = 0.015f;

		private const float VinylWaitTimeoutSeconds = 30f;

		private readonly ICarInfoService _carInfoService;

		private readonly DefaultMaterialsConfig _materialsConfig;

		private readonly VinylDrawer _vinylDrawer;

		private readonly PlayerDatas.VinylsDataUtility _vinylsDataUtility;

		private readonly DrawQueue _drawQueue;

		private CarModelLibrary _library;

		private PoliceSystemConfig _policeConfig;

		public static string RootName(int carId)
		{
			return "CarVisualModel_" + carId;
		}

		public static float ContentLift(float groundY, float floorGapMeters)
		{
			return floorGapMeters - 0.015f - groundY;
		}

		[Inject]
		public CarVisualModelBuilder(ICarInfoService carInfoService, DefaultMaterialsConfig materialsConfig, VinylDrawer vinylDrawer, PlayerDatas.VinylsDataUtility vinylsDataUtility, DrawQueue drawQueue)
		{
			_carInfoService = carInfoService;
			_materialsConfig = materialsConfig;
			_vinylDrawer = vinylDrawer;
			_vinylsDataUtility = vinylsDataUtility;
			_drawQueue = drawQueue;
		}

		public UniTask<CarVisualModelHandle> BuildAsync(int carId, CancellationToken token)
		{
			CarInfo carInfo = _carInfoService.CreateDefault(carId);
			return BuildAsync(carInfo, token);
		}

		public async UniTask<CarVisualModelHandle> BuildAsync(CarInfo carInfo, CancellationToken token)
		{
			int carId = carInfo.CarID;
			CarModelLibrary carModelLibrary = (_library ? _library : (_library = Resources.Load<CarModelLibrary>("Configs/CarModels/CarModelLibrary")));
			if (!carModelLibrary || !carModelLibrary.TryGet(carId, out var cfg) || cfg.Wheels == null)
			{
				return null;
			}
			GameObject holder = new GameObject("__CarVisualModelHolder");
			holder.SetActive(value: false);
			GameObject root = new GameObject(RootName(carId));
			root.transform.SetParent(holder.transform, worldPositionStays: false);
			Transform content = new GameObject("Content").transform;
			content.SetParent(root.transform, worldPositionStays: false);
			List<WheelProvider> providers = new List<WheelProvider>();
			WheelMount[] wheels = cfg.Wheels;
			for (int i = 0; i < wheels.Length; i++)
			{
				WheelMount wheelMount = wheels[i];
				GameObject gameObject = new GameObject($"Wheel_{wheelMount.WheelIndex}");
				gameObject.transform.SetParent(content, worldPositionStays: false);
				gameObject.transform.localPosition = wheelMount.LocalPosition;
				gameObject.transform.localRotation = Quaternion.Euler(wheelMount.LocalEuler);
				WheelProvider wheelProvider = gameObject.AddComponent<WheelProvider>();
				wheelProvider.WheelIndex = wheelMount.WheelIndex;
				providers.Add(wheelProvider);
			}
			LoadedBody body = default;
			LoadedKits kits = default;
			Material paintMat = null;
			RenderTexture vinylRT = null;
			Material windowMat = null;
			RenderTexture windowRT = null;
			VisualWheelInstaller.MaterialCache wheelMaterials = new VisualWheelInstaller.MaterialCache();
			try
			{
				body = await CarBodyVisualLoader.LoadAsync(carId, content, cfg.BodyAnchorLocal, token);
				root.transform.SetParent(null, worldPositionStays: false);
				FrontWheelData frontWheelData = carInfo.TuningData.FrontWheelData;
				RearWheelData rearWheelData = carInfo.TuningData.RearWheelData;
				Dictionary<string, GameObject> resourceCache = new Dictionary<string, GameObject>();
				float num = float.MaxValue;
				for (int j = 0; j < providers.Count; j++)
				{
					WheelProvider wheelProvider2 = providers[j];
					WheelData wheelData = (wheelProvider2.IsFront ? ((WheelData)frontWheelData) : ((WheelData)rearWheelData));
					VisualWheelInstaller.Install(wheelProvider2, wheelData, resourceCache, wheelMaterials);
					Vector3 localPosition = wheelProvider2.transform.localPosition;
					localPosition.y -= 0.2f;
					wheelProvider2.transform.localPosition = localPosition;
					num = Mathf.Min(num, localPosition.y - VisualWheelInstaller.TyreRadius(wheelData));
				}
				if (num < float.MaxValue)
				{
					content.localPosition = new Vector3(0f, ContentLift(num, cfg.FloorGapMeters), 0f);
				}
				if ((bool)body.Body)
				{
					paintMat = CarVisualPaint.Apply(body.Body, carInfo, _materialsConfig.CarBody);
					kits = await InstalledBodyKitLoader.LoadAsync(carId, content.gameObject, body.Body, cfg, carInfo.InstalledBodyKits, token);
					if ((bool)paintMat && kits.Kits != null)
					{
						for (int k = 0; k < kits.Kits.Count; k++)
						{
							LoadedKitInstance loadedKitInstance = kits.Kits[k];
							if (IsNoVinylKit(loadedKitInstance.Type))
							{
								CarVisualPaint.TintNoVinylKit(loadedKitInstance.Go, loadedKitInstance.Type, carInfo, wheelMaterials.Created);
							}
							else
							{
								CarVisualPaint.PaintKit(loadedKitInstance.Go, paintMat);
							}
						}
					}
					PoliceSystemConfig config = (_policeConfig ? _policeConfig : (_policeConfig = Resources.Load<PoliceSystemConfig>("Configs/PoliceSystemConfig")));
					List<AsyncOperationHandle> list = await PoliceKitLoader.LoadAsync(carId, content.gameObject, carInfo.AdditionalData?.Police, config, token);
					if (kits.Handles != null && list.Count > 0)
					{
						kits.Handles.AddRange(list);
					}
					if ((bool)paintMat && (bool)_vinylDrawer && _vinylsDataUtility != null && _drawQueue != null && CarVinylRenderer.TryLoadDefaultPreset(carId, out var preset))
					{
						int num2 = CarVinylRenderer.BodyVinylCount(preset);
						int num3 = CarVinylRenderer.WindowVinylCount(preset);
						if (num3 > 0)
						{
							windowMat = CarWindowGlass.Apply(body.Body, _materialsConfig.Window);
						}
						if (num2 > 0 || ((bool)windowMat && num3 > 0))
						{
							CarModelVinylPerformer performer = root.AddComponent<CarModelVinylPerformer>();
							performer.Init(carId, preset, paintMat, windowMat, _vinylDrawer, _vinylsDataUtility);
							_drawQueue.Enqueue(performer);
							await WaitForVinylsAsync(performer, carId, token);
							performer.TakeTextures(out vinylRT, out windowRT);
						}
					}
				}
				Bounds bounds = CalculateBounds(root);
				return new CarVisualModelHandle(root, bounds, carId, body.Handle, body.Valid, paintMat, kits.Handles, vinylRT, windowMat, windowRT, wheelMaterials.Created);
			}
			catch (OperationCanceledException)
			{
				CleanupBuild(root, body, kits, paintMat, vinylRT, windowMat, windowRT, wheelMaterials.Created);
				throw;
			}
			catch (Exception)
			{
				CleanupBuild(root, body, kits, paintMat, vinylRT, windowMat, windowRT, wheelMaterials.Created);
				return null;
			}
			finally
			{
				if ((bool)holder)
				{
					UnityEngine.Object.Destroy(holder);
				}
			}
		}

		private static bool IsNoVinylKit(BodyKitType type)
		{
			if (type != BodyKitType.Spoiler && type != BodyKitType.AirIntakeHood)
			{
				return type == BodyKitType.AirIntakeRoof;
			}
			return true;
		}

		private static async UniTask WaitForVinylsAsync(CarModelVinylPerformer performer, int carId, CancellationToken token)
		{
			if (await UniTask.WhenAny(performer.Completion, UniTask.Delay(TimeSpan.FromSeconds(30.0), DelayType.Realtime, PlayerLoopTiming.Update, token)) != 0)
			{
				performer.Cancel();
			}
		}

		private static void CleanupBuild(GameObject root, LoadedBody body, LoadedKits kits, Material paintMat, RenderTexture vinylRT, Material windowMat, RenderTexture windowRT, List<Material> wheelMaterials)
		{
			if ((bool)root)
			{
				UnityEngine.Object.Destroy(root);
			}
			if (body.Valid && body.Handle.IsValid())
			{
				Addressables.Release(body.Handle);
			}
			if (kits.Handles != null)
			{
				for (int i = 0; i < kits.Handles.Count; i++)
				{
					if (kits.Handles[i].IsValid())
					{
						Addressables.Release(kits.Handles[i]);
					}
				}
			}
			if ((bool)paintMat)
			{
				UnityEngine.Object.Destroy(paintMat);
			}
			if ((bool)vinylRT)
			{
				CommonExtensions.DeleteRenderTex(vinylRT);
			}
			if ((bool)windowMat)
			{
				UnityEngine.Object.Destroy(windowMat);
			}
			if ((bool)windowRT)
			{
				CommonExtensions.DeleteRenderTex(windowRT);
			}
			if (wheelMaterials == null)
			{
				return;
			}
			for (int j = 0; j < wheelMaterials.Count; j++)
			{
				if ((bool)wheelMaterials[j])
				{
					UnityEngine.Object.Destroy(wheelMaterials[j]);
				}
			}
		}

		private static Bounds CalculateBounds(GameObject root)
		{
			Renderer[] componentsInChildren = root.GetComponentsInChildren<Renderer>(includeInactive: true);
			bool flag = false;
			Bounds result = default;
			foreach (Renderer renderer in componentsInChildren)
			{
				if ((bool)renderer && renderer.enabled && renderer.gameObject.activeInHierarchy)
				{
					if (!flag)
					{
						result = renderer.bounds;
						flag = true;
					}
					else
					{
						result.Encapsulate(renderer.bounds);
					}
				}
			}
			if (!flag)
			{
				return new Bounds(root.transform.position, Vector3.one);
			}
			return result;
		}
	}
}
