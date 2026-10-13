using System;
using System.Collections.Generic;
using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Offers;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class CarExtension
	{
		public const string ResetSmoothMatSubstring = "_ResetSmooth";

		private static readonly int SmoothnessProperty = Shader.PropertyToID("_Smoothness");

		private static readonly int RainPowerProperty = Shader.PropertyToID("_RainPower");

		private static readonly Dictionary<Material, Material> RainFreeInteriorMats = new Dictionary<Material, Material>();

		private static readonly List<Renderer> InteriorRendererBuffer = new List<Renderer>();

		private static readonly HashSet<Renderer> ProcessedInteriorRenderers = new HashSet<Renderer>();

		private static readonly List<Material> DeadRainFreeSources = new List<Material>();

		public static void UpdateInteriorMaterialSmooth0(CarInteriorReferences interior)
		{
			if (!interior)
			{
				return;
			}
			MeshRenderer[] componentsInChildren = interior.GetComponentsInChildren<MeshRenderer>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] sharedMaterials = componentsInChildren[i].sharedMaterials;
				foreach (Material material in sharedMaterials)
				{
					if ((bool)material && material.name.Contains("_ResetSmooth") && material.HasProperty(SmoothnessProperty))
					{
						material.SetFloat(SmoothnessProperty, 0f);
					}
				}
				componentsInChildren[i].sharedMaterials = sharedMaterials;
			}
		}

		public static void DisableRainOnInteriorMaterials(CarInteriorReferences interior)
		{
			if ((bool)interior)
			{
				PruneDeadRainFreeSources();
				ProcessedInteriorRenderers.Clear();
				DisableRainInSubtree(interior.transform);
				DisableRainInSubtree(interior.FrontLeftDoorInteriorPart);
				DisableRainInSubtree(interior.FrontRightDoorInteriorPart);
				DisableRainInSubtree(interior.BackLeftDoorInteriorPart);
				DisableRainInSubtree(interior.BackRightDoorInteriorPart);
				DisableRainInSubtree(interior.TrunkFirstDoorInteriorPart);
				DisableRainInSubtree(interior.TrunkSecondDoorInteriorPart);
				ProcessedInteriorRenderers.Clear();
			}
		}

		private static void DisableRainInSubtree(Transform root)
		{
			if (!root)
			{
				return;
			}
			root.GetComponentsInChildren(includeInactive: true, InteriorRendererBuffer);
			foreach (Renderer item in InteriorRendererBuffer)
			{
				if (!ProcessedInteriorRenderers.Add(item))
				{
					continue;
				}
				Material[] sharedMaterials = item.sharedMaterials;
				bool flag = false;
				for (int i = 0; i < sharedMaterials.Length; i++)
				{
					Material material = sharedMaterials[i];
					if ((bool)material && material.HasProperty(RainPowerProperty) && material.GetFloat(RainPowerProperty) != 0f)
					{
						sharedMaterials[i] = GetRainFreeCopy(material);
						flag = true;
					}
				}
				if (flag)
				{
					item.sharedMaterials = sharedMaterials;
				}
			}
		}

		private static Material GetRainFreeCopy(Material source)
		{
			if (RainFreeInteriorMats.TryGetValue(source, out var value) && (bool)value)
			{
				return value;
			}
			value = new Material(source)
			{
				name = source.name + " (NoRain)"
			};
			value.SetFloat(RainPowerProperty, 0f);
			RainFreeInteriorMats[source] = value;
			return value;
		}

		private static void PruneDeadRainFreeSources()
		{
			foreach (KeyValuePair<Material, Material> rainFreeInteriorMat in RainFreeInteriorMats)
			{
				if (!rainFreeInteriorMat.Key)
				{
					DeadRainFreeSources.Add(rainFreeInteriorMat.Key);
				}
			}
			foreach (Material deadRainFreeSource in DeadRainFreeSources)
			{
				Material material = RainFreeInteriorMats[deadRainFreeSource];
				RainFreeInteriorMats.Remove(deadRainFreeSource);
				if ((bool)material)
				{
					UnityEngine.Object.Destroy(material);
				}
			}
			DeadRainFreeSources.Clear();
		}

		public static BuyCarFromPurchaseResult BuyCarFromPurchase(int carId, Action<CarInfo> addVinylsCallbackNew = null, Action<CarInfo> addBodyKitsCallbackNew = null, string carInstanceID = null)
		{
			try
			{
				Action<CarInfo> addVinylsCallback = null;
				Action<CarInfo> addBodyKitsCallback = null;
				if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.dealerWindow)
				{
					addVinylsCallback = V.CoreGameManager.dealerWindow.RecordStyleForNewPurchasedCar;
				}
				if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.SellCarValues && (bool)V.CoreGameManager.SellCarValues.BuyCarBodyKits)
				{
					addBodyKitsCallback = V.CoreGameManager.SellCarValues.BuyCarBodyKits.RecordBodyKitsForNewPurchasedCar;
				}
				if (addVinylsCallbackNew != null)
				{
					addVinylsCallback = addVinylsCallbackNew;
				}
				if (addBodyKitsCallbackNew != null)
				{
					addBodyKitsCallback = addBodyKitsCallbackNew;
				}
				if (!V.CoreGameManager.SlotService.SlotCollection.TryGetEmptySlot(out var res))
				{
					Debug.LogError("No slot");
					return BuyCarFromPurchaseResult.NoFreeSlot;
				}
				V.SlotID = res.ID;
				if (V.CoreGameManager.AddPurchasedCarToSlot(res, carId, addVinylsCallback, addBodyKitsCallback, carInstanceID) == null)
				{
					return BuyCarFromPurchaseResult.Error;
				}
				bool flag = false;
				if ((bool)V.CoreGameManager && (bool)V.CoreGameManager.dealerWindow)
				{
					flag = true;
					V.CoreGameManager.ManagerUI.ShowGarageWindow();
					V.CoreGameManager.UnloadCurrentSellCar();
					V.CoreGameManager.ResetCarId();
					V.CoreGameManager.dealerWindow.Close();
				}
				if ((bool)V.CoreGameManager.GarageController & flag)
				{
					V.CoreGameManager.GarageController.FocusOnCarOnSlot(V.SlotID).Forget();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				return BuyCarFromPurchaseResult.Error;
			}
			return BuyCarFromPurchaseResult.Success;
		}

		public static async UniTask<NewCarVisualData> GetDefaultVisualDataFromPreset(int carID)
		{
			ResourceRequest request = Resources.LoadAsync<VehicleStockTuningData>(SellCarValues.GetVehicleStockTuningDataPath(carID));
			await request.ToUniTask();
			VehicleStockTuningData vehicleStockTuningData = (VehicleStockTuningData)request.asset;
			if ((int)vehicleStockTuningData.DefaultStyle >= 0)
			{
				request = Resources.LoadAsync<AllVinylPreset>(SellCarValues.GetCarVinylPresetPath(carID));
				await request.ToUniTask();
				AllVinylPreset allVinylPreset = (AllVinylPreset)request.asset;
				if ((int)vehicleStockTuningData.DefaultStyle < allVinylPreset.allPresetList.Count)
				{
					NewCarVisualData newCarVisualData = new NewCarVisualData();
					OneVinylsPreset oneVinylsPreset = allVinylPreset.allPresetList[vehicleStockTuningData.DefaultStyle];
					newCarVisualData.VynilOneCar = oneVinylsPreset.vynilOneCarList;
					newCarVisualData.WindowVynilOneCar = oneVinylsPreset.windowVynilOneCarList;
					newCarVisualData.CarPaintColor = vehicleStockTuningData.CarInfoDefault?.Colors?.CarColor ?? CarPaintColor.Default;
					newCarVisualData.CarPaintColor.Color = oneVinylsPreset.carColor;
					return newCarVisualData;
				}
			}
			return null;
		}

		public static void RecordStyleForNewPurchasedCar(CarInfo carInfo, NewCarVisualData newCarVisualData)
		{
			if (newCarVisualData != null)
			{
				VynilOneCar vynilOneCar = newCarVisualData.VynilOneCar;
				vynilOneCar.CarInstanceID = carInfo.CarInstanceId;
				PlayerDatas.SaveVynilsFromDataBase(vynilOneCar);
				VynilOneCar windowVynilOneCar = newCarVisualData.WindowVynilOneCar;
				if (windowVynilOneCar?.allVynils != null && windowVynilOneCar.allVynils.Count > 0)
				{
					PlayerDatas.VinylsDataUtility.TrySaveCarVinyls(windowVynilOneCar.allVynils, DirectoryHelper.GetWindowVinylsPath(carInfo.CarInstanceId));
				}
				carInfo.Colors.CarColor = newCarVisualData.CarPaintColor;
				carInfo.Save();
			}
		}

		public static void RecordBodyKitsForNewPurchasedCar(CarInfo carInfo)
		{
			if (carInfo == null)
			{
				return;
			}
			SellCarValues sellCarValues = (V.CoreGameManager ? V.CoreGameManager.SellCarValues : null);
			if ((bool)sellCarValues && (bool)sellCarValues.BuyCarBodyKits)
			{
				sellCarValues.CarID.Get(out int value);
				if (value == carInfo.CarID)
				{
					sellCarValues.BuyCarBodyKits.ApplySelectedBodyKitsTo(carInfo.InstalledBodyKits);
				}
			}
			RecordBodyKitsForNewPurchasedCar(carInfo, carInfo.InstalledBodyKits);
		}

		public static void RecordBodyKitsForNewPurchasedCar(CarInfo carInfo, InstalledBodyKits installedBodies)
		{
			foreach (KeyValuePair<BodyKitType, int> item in new Dictionary<BodyKitType, int>
			{
				{
					BodyKitType.FrontBumper,
					installedBodies.FrontBumperId
				},
				{
					BodyKitType.RearBumper,
					installedBodies.RearBumperId
				},
				{
					BodyKitType.Spoiler,
					installedBodies.SpoilerId
				},
				{
					BodyKitType.Skirt,
					installedBodies.SkirtId
				},
				{
					BodyKitType.Fender,
					installedBodies.FenderId
				},
				{
					BodyKitType.Hood,
					installedBodies.HoodId
				},
				{
					BodyKitType.AirIntakeHood,
					installedBodies.HoodAirIntakeId
				},
				{
					BodyKitType.AirIntakeRoof,
					installedBodies.RoofAirIntakeId
				},
				{
					BodyKitType.Interior,
					installedBodies.InteriorId
				}
			})
			{
				carInfo.BoughtBodyKits.AddBoughtBodyKits(item.Key, item.Value);
				carInfo.InstalledBodyKits.GetInstalledBodyKitIndexRef(item.Key) = item.Value;
			}
			for (int i = 0; i < carInfo.InstalledBodyKits.Trims.Length; i++)
			{
				carInfo.BoughtBodyKits.AddBoughtBodyKits(BodyKitType.Trim, carInfo.InstalledBodyKits.Trims[i]);
			}
			carInfo.Save();
		}
	}
}
