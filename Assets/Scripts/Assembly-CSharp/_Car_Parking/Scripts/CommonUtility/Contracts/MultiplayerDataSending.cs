using System;
using CPM.Database;
using CPM.Database.Constructors;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Photon.Pun;
using Unity.Mathematics;
using UnityEngine;
using _Car_Parking.Scripts.Car;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Multiplayer.InterestGroupsImpl;

namespace _Car_Parking.Scripts.CommonUtility.Contracts
{
	public class MultiplayerDataSending
	{
		public enum Order
		{
			CarInstanceId = 0,
			CarId = 1,
			CarResult = 2,
			InstalledBodyKits = 3,
			Colors = 4,
			AdditionalData = 5,
			VinylsData = 6,
			FirebaseId = 7
		}

		[Serializable]
		public struct CarVinyls
		{
			public int FrontPlateIndex;

			public int RearPlateIndex;

			public object[] CarBodyVinyls;

			public object[] FrontPlateVinyls;

			public object[] RearPlateVinyls;

			public object[] WindowVinyls;

			public CarVinyls(int frontPlateIndex, int rearPlateIndex, object[] carBodyVinyls, object[] frontPlateVinyls, object[] rearPlateVinyls, object[] windowVinyls)
			{
				FrontPlateIndex = frontPlateIndex;
				RearPlateIndex = rearPlateIndex;
				CarBodyVinyls = carBodyVinyls;
				FrontPlateVinyls = frontPlateVinyls;
				RearPlateVinyls = rearPlateVinyls;
				WindowVinyls = windowVinyls;
			}

			public bool HasVinyls()
			{
				if (CarBodyVinyls == null && FrontPlateVinyls == null && RearPlateVinyls == null)
				{
					return WindowVinyls != null;
				}
				return true;
			}
		}

		private static readonly int DataLength = Enum.GetValues(typeof(Order)).Length;

		private static int _lastVisualApplyFrame = -1000;

		private const int VisualApplyCooldownFrames = 10;

		public static object[] SetDataForSend(int id, CarInfo carInfo, VehicleDataResult vehicleDataResult, string firebaseId)
		{
			Debug.Log("Set data for send");
			object[] array = new object[DataLength];
			string carInstanceId = carInfo.CarInstanceId;
			CarVinyls data = new CarVinyls(PlateDataUtility.TryGetPlateIdForCar(carInfo.CarInstanceId, isFrontPlate: true), PlateDataUtility.TryGetPlateIdForCar(carInfo.CarInstanceId), VinylsUtility.GetVinylsInstantiationData(carInfo, VinylsType.CarVinyls, isFrontPlate: false), VinylsUtility.GetVinylsInstantiationData(carInfo, VinylsType.PlateVinyls, isFrontPlate: true), VinylsUtility.GetVinylsInstantiationData(carInfo, VinylsType.PlateVinyls, isFrontPlate: false), VinylsUtility.GetVinylsInstantiationData(carInfo, VinylsType.WindowVinyls, isFrontPlate: false));
			array[0] = carInstanceId;
			array[1] = (carInfo.CarID << 8) | InterestGroupManager.CarInterestGroup;
			array[2] = CarResultConverter.ToArray(vehicleDataResult);
			array[3] = InstalledBodyKitsConverter.ToArray(carInfo.InstalledBodyKits);
			array[4] = ColorsConverter.ToArray(carInfo.Colors);
			array[5] = AdditionalDataConverter.ToArray(carInfo.AdditionalData);
			array[6] = CarVinylsConverter.ToArray(data);
			array[7] = firebaseId ?? string.Empty;
			Debug.LogWarning("set data for send:\n " + JsonConvert.SerializeObject(array.Length));
			long num = ObjectArraySizeCalculator.CalculateArraySize(array);
			Debug.LogWarning($"set data for send array size:\n {num}");
			return array;
		}

		public static void CarApplyData(MultiPlayerPosControl multiPlayerPosControl, AllCarsInfo allCarsInfo, BadCarConfig badCarConfig, bool isCarLegit = true)
		{
			object[] instantiationData = multiPlayerPosControl.gameObject.GetComponent<PhotonView>().InstantiationData;
			if (multiPlayerPosControl.gameObject == multiPlayerPosControl.FreeDriveProvider.FirstCarMultiPlayer)
			{
				return;
			}
			if (!multiPlayerPosControl.DataApplied)
			{
				CarInfo carInfo = new CarInfo();
				string text = (string)instantiationData[0];
				int num = (int)instantiationData[1];
				byte interestGroup = (byte)(num % 256);
				multiPlayerPosControl.InterestGroup = interestGroup;
				int num2 = num >> 8;
				VehicleDataResult vehicleDataResult = CarResultConverter.Get(instantiationData[2]);
				SuspensionData suspensionData = CarTuningFactory.GetDefaultCarTuningDataConfig(num2).Tuning.FrontWheelData.SuspensionData;
				SuspensionData suspensionData2 = CarTuningFactory.GetDefaultCarTuningDataConfig(num2).Tuning.RearWheelData.SuspensionData;
				SuspensionState suspensionState = Utils.GetSuspensionState(vehicleDataResult.FrontWheel.SuspensionData, vehicleDataResult.RearWheel.SuspensionData, suspensionData, suspensionData2);
				vehicleDataResult.FrontWheel.SuspensionData.SuspensionState = suspensionState;
				vehicleDataResult.FrontWheel.SuspensionData.SuspensionType = suspensionData.SuspensionType;
				vehicleDataResult.RearWheel.SuspensionData.SuspensionState = suspensionState;
				vehicleDataResult.RearWheel.SuspensionData.SuspensionType = suspensionData2.SuspensionType;
				carInfo.CarInstanceId = text;
				carInfo.InstalledBodyKits = InstalledBodyKitsConverter.Get(instantiationData[3]);
				carInfo.Colors = ColorsConverter.Get(instantiationData[4]);
				carInfo.AdditionalData = AdditionalDataConverter.Get(instantiationData[5]);
				CarVinyls vinylsData = CarVinylsConverter.Get(instantiationData[6]);
				carInfo.CarID = num2;
				carInfo.TuningData = new CarTuningData();
				carInfo.TuningData.FrontWheelData = new FrontWheelData();
				carInfo.TuningData.FrontWheelData = vehicleDataResult.FrontWheel;
				carInfo.TuningData.RearWheelData = new RearWheelData();
				carInfo.TuningData.RearWheelData = vehicleDataResult.RearWheel;
				CarInfoContainer carInfoContainer = multiPlayerPosControl.gameObject.AddComponent<CarInfoContainer>();
				carInfoContainer.CarInfo = carInfo;
				carInfoContainer.VinylsData = vinylsData;
				carInfoContainer.vehicleDataResult = vehicleDataResult;
				multiPlayerPosControl.DataApplied = true;
				multiPlayerPosControl.CreateSkidAfter(carInfo.Colors.SmokeColor).Forget();
				BadCarsChecker.CarInfoCheck(carInfo, multiPlayerPosControl, carInfoContainer, badCarConfig, isCarLegit);
				multiPlayerPosControl.VehicleData.CarInfoProvider.SetCarInfoContainer(carInfoContainer);
				GameObject gameObject = multiPlayerPosControl.gameObject;
				VehicleData component = gameObject.GetComponent<VehicleData>();
				multiPlayerPosControl.VehicleData.generatedID = text;
				if (gameObject.TryGetComponent<VehicleEntityProvider>(out var component2))
				{
					component2.InitOtherMultiplayerCar(vehicleDataResult);
					component2.CarEntity.CarInstanceId = text;
					multiPlayerPosControl.VehicleData.CarChildsReferences.FuelTank.CurrentFuel = vehicleDataResult.MaintenanceData.FuelInTank;
				}
				else
				{
					Debug.LogError("VehicleEntityProvider not found:" + gameObject, gameObject);
				}
				Rigidbody component3 = multiPlayerPosControl.GetComponent<Rigidbody>();
				if (FlagsStorage.RacingGameMode)
				{
					component3.isKinematic = true;
					multiPlayerPosControl.forceDisableCar = true;
					multiPlayerPosControl.FarFromPerson = true;
				}
				if ((bool)multiPlayerPosControl.VehicleData.Driver)
				{
					multiPlayerPosControl.updateLabel(multiPlayerPosControl.VehicleData.Driver, Show: true);
				}
				component3.isKinematic = true;
				if (Time.time - multiPlayerPosControl.createdTime < 5f)
				{
					component3.detectCollisions = false;
					multiPlayerPosControl.ResetIgnorCollisionDelayed(3f);
				}
				multiPlayerPosControl.VehicleData.ColorsService.CreateFirstMat(multiPlayerPosControl.VehicleData);
				if (math.all(allCarsInfo.CheckTradeCarNotAllowed(num2) == Utils.NotValid) || math.all(allCarsInfo.CheckTradeCarNotAllowed(text) == Utils.NotValid))
				{
					multiPlayerPosControl.VehicleData.BlockSellThisCar = true;
				}
				ColorsService.SetCarPaintColor(multiPlayerPosControl.VehicleData.CarMat, carInfo.Colors.CarColor);
				WheelConstructor.SetWheel(component, carInfo.TuningData, 200).Forget();
			}
			if ((bool)multiPlayerPosControl.VehicleData.CarMat && (bool)multiPlayerPosControl.VehicleData.MainModel)
			{
				BodyMarker component4 = multiPlayerPosControl.VehicleData.MainModel.GetComponent<BodyMarker>();
				if ((bool)component4)
				{
					if ((bool)component4.frontLightRight)
					{
						component4.frontLightRight.GetComponent<MeshRenderer>().material = multiPlayerPosControl.VehicleData.CarMat;
					}
					if ((bool)component4.frontLightLeft)
					{
						component4.frontLightLeft.GetComponent<MeshRenderer>().material = multiPlayerPosControl.VehicleData.CarMat;
					}
					if ((bool)component4.hood)
					{
						component4.hood.GetComponent<MeshRenderer>().material = multiPlayerPosControl.VehicleData.CarMat;
					}
					if ((bool)component4.spoiler)
					{
						component4.spoiler.GetComponent<MeshRenderer>().material = multiPlayerPosControl.VehicleData.CarMat;
					}
					if ((bool)component4.spoiler)
					{
						multiPlayerPosControl.VehicleData.BodyLoadingComposition.SetSpoiler(component4.spoiler);
					}
					GameObject[] additionalElements = component4.AdditionalElements;
					foreach (GameObject gameObject2 in additionalElements)
					{
						if (gameObject2.TryGetComponent<MeshRenderer>(out var component5))
						{
							component5.sharedMaterial = multiPlayerPosControl.VehicleData.CarMat;
						}
						if (gameObject2.transform.childCount <= 0)
						{
							continue;
						}
						foreach (Transform item in gameObject2.transform)
						{
							if (item.name.Contains("window") && item.TryGetComponent<MeshRenderer>(out var component6))
							{
								component6.sharedMaterial = multiPlayerPosControl.VehicleData.WindowComposition.WindowMat;
							}
						}
					}
					additionalElements = component4.AdditionalElementsWithoutVinyls;
					for (int i = 0; i < additionalElements.Length; i++)
					{
						if (additionalElements[i].TryGetComponent<MeshRenderer>(out var component7))
						{
							component7.sharedMaterial = multiPlayerPosControl.VehicleData.CarMat;
						}
					}
					additionalElements = component4.AdditionalWindowElements;
					foreach (GameObject gameObject3 in additionalElements)
					{
						if ((bool)gameObject3 && gameObject3.TryGetComponent<MeshRenderer>(out var component8))
						{
							component8.sharedMaterial = multiPlayerPosControl.VehicleData.WindowComposition.WindowMat;
						}
					}
				}
			}
			if (multiPlayerPosControl.TryGetComponent<LightController>(out var component9))
			{
				multiPlayerPosControl.SetHeadAndIndicator((int)component9.IndicatorState.Value, component9.HState, forceSet: true);
			}
		}

		public static void ApplyAllVisual(MultiPlayerPosControl multiPlayerPosControl, CarFactoryService carFactoryService)
		{
			if (!multiPlayerPosControl.isOnlineCar)
			{
				return;
			}
			if (!multiPlayerPosControl.DataApplied)
			{
				multiPlayerPosControl.ApplyDataAsync().Forget();
				return;
			}
			CarInfoContainer component = multiPlayerPosControl.GetComponent<CarInfoContainer>();
			if (component.CarBodyWasLoaded)
			{
				return;
			}
			if (multiPlayerPosControl.gameObject != V.Car)
			{
				if (Time.frameCount - _lastVisualApplyFrame < 10)
				{
					return;
				}
				_lastVisualApplyFrame = Time.frameCount;
			}
			component.CarBodyWasLoaded = true;
			GameObject gameObject = multiPlayerPosControl.gameObject;
			if (gameObject.TryGetComponent<VehicleData>(out var component2) && !multiPlayerPosControl.vinylLoaded)
			{
				CarInfo carInfo = component.CarInfo;
				_ = carFactoryService.SetVisual(carInfo, waitBodyLoad: false, gameObject, component2, isLocalCar: false);
				multiPlayerPosControl.vinylLoaded = true;
			}
		}

		public static void ApplyVinyls(MultiPlayerPosControl multiPlayerPosControl, CarVinyls vinylsData, Colors carInfoColors)
		{
			Debug.Log("ApplyVinyls");
			byte b = (byte)vinylsData.FrontPlateIndex;
			byte b2 = (byte)vinylsData.RearPlateIndex;
			if (b != 0)
			{
				PlateConstructor.InstantiatePlate(isFront: true, b, multiPlayerPosControl.VehicleData);
			}
			if (b2 != 0)
			{
				PlateConstructor.InstantiatePlate(isFront: false, b2, multiPlayerPosControl.VehicleData);
			}
			multiPlayerPosControl.CarVinyls = vinylsData;
			ColorsService.SetCarPaintColor(multiPlayerPosControl.VehicleData.CarMat, carInfoColors.CarColor);
			if (vinylsData.HasVinyls())
			{
				multiPlayerPosControl.DrawQueue.Enqueue(multiPlayerPosControl);
			}
		}
	}
}
