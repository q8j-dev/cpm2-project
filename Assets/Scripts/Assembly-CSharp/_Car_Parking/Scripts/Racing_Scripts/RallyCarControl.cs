using System;
using System.Collections.Generic;
using CPM.Database;
using CPM.Scripts.VinylDrawer;
using CPMEngine;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer;
using _Car_Parking.Scripts.Car.CarSetter;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;
using _Car_Parking.Scripts.Rally.Data;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	public class RallyCarControl : MonoBehaviour
	{
		private class RallyLiveryPerformer : IVinylDrawPerformer
		{
			private readonly VehicleData _vehicleData;

			private readonly OneVinylsPreset _preset;

			private readonly VinylDrawer _vinylDrawer;

			private readonly PlayerDatas.VinylsDataUtility _vinylsDataUtility;

			public MonoBehaviour ComponentToCheck => _vehicleData;

			public RallyLiveryPerformer(VehicleData vehicleData, OneVinylsPreset preset, VinylDrawer vinylDrawer, PlayerDatas.VinylsDataUtility vinylsDataUtility)
			{
				_vehicleData = vehicleData;
				_preset = preset;
				_vinylDrawer = vinylDrawer;
				_vinylsDataUtility = vinylsDataUtility;
			}

			public int GetVinylsCount(VinylsType type, bool isFrontPlate)
			{
				return type switch
				{
					VinylsType.CarVinyls => _preset.vynilOneCarList.allVynils.Count, 
					VinylsType.WindowVinyls => (_preset.windowVynilOneCarList?.allVynils?.Count).GetValueOrDefault(), 
					_ => 0, 
				};
			}

			public void RenderVinyls(VinylsType type, bool isFrontPlate)
			{
				switch (type)
				{
				case VinylsType.CarVinyls:
					try
					{
						Material carMat = _vehicleData.CarMat;
						RenderTexture mainTexture = (_vehicleData.VinylsComposition.RenderTexture = _vinylDrawer.GetRenderTexture(VinylsType.CarVinyls));
						carMat.mainTexture = mainTexture;
						_vinylsDataUtility.LoadAllVinyls(_preset.vynilOneCarList, VinylsType.CarVinyls);
						_vinylDrawer.FocusOnVinyls(VinylsType.CarVinyls);
						_vinylDrawer.TakeSnapshot();
						break;
					}
					catch (Exception message2)
					{
						Debug.LogError(message2);
						break;
					}
					finally
					{
						_vehicleData.CarMat.color = _preset.carColor;
						_vinylDrawer.DeleteVinylContext(VinylsType.CarVinyls);
					}
				case VinylsType.WindowVinyls:
					if (_vehicleData.WindowComposition.WindowMat == null)
					{
						break;
					}
					try
					{
						_vehicleData.WindowComposition.WindowMat.mainTexture = (_vehicleData.WindowComposition.WindowRT = _vinylDrawer.GetRenderTexture(VinylsType.WindowVinyls));
						_vinylsDataUtility.LoadAllVinyls(_preset.windowVynilOneCarList, VinylsType.WindowVinyls);
						_vinylDrawer.FocusOnVinyls(VinylsType.WindowVinyls);
						_vinylDrawer.TakeSnapshot();
						break;
					}
					catch (Exception message)
					{
						Debug.LogError(message);
						break;
					}
					finally
					{
						_vinylDrawer.DeleteVinylContext(VinylsType.WindowVinyls);
					}
				}
			}
		}

		[SerializeField]
		private RallyTriggersControl RallyTriggersControl;

		[SerializeField]
		private RallyController RallyController;

		private PlayerDatas.CarLoadingUtility _carLoadingUtility;

		private RallySequenceData _racingSequenceData;

		private RallyCar _rallyCar;

		private IPrefabFactory _prefabFactory;

		private RallyCarsDatasSO _carsDatasSo;

		private RallyCarData _currentRallyCarData;

		private DrawQueue _drawQueue;

		private VinylDrawer _vinylDrawer;

		private PlayerDatas.VinylsDataUtility _vinylsDataUtility;

		private QuickCarSettings _quickCarSettings;

		private AllCarsInfo _allCarsInfo;

		private CPMGraphicsSettingsManager _graphicSettings => CPMGraphicsSettingsManager.Instance;

		[UnityEngine.Scripting.Preserve]
		[Inject]
		public void Construct(PlayerDatas.CarLoadingUtility carLoadingUtility, IPrefabFactory prefabFactory, RallyCarsDatasSO carsDatasSo, DrawQueue drawQueue, VinylDrawer vinylDrawer, PlayerDatas.VinylsDataUtility vinylsDataUtility, QuickCarSettings quickCarSettings, AllCarsInfo allCarsInfo)
		{
			_carLoadingUtility = carLoadingUtility;
			_prefabFactory = prefabFactory;
			_carsDatasSo = carsDatasSo;
			_drawQueue = drawQueue;
			_vinylDrawer = vinylDrawer;
			_vinylsDataUtility = vinylsDataUtility;
			_quickCarSettings = quickCarSettings;
			_allCarsInfo = allCarsInfo;
		}

		private void OnGraphicsQualityChanged(int quality)
		{
			SetMirror();
		}

		public void Init(RallySequenceData racingSequenceData)
		{
			_racingSequenceData = racingSequenceData;
			_graphicSettings.OnQualityChanged -= OnGraphicsQualityChanged;
			_graphicSettings.OnQualityChanged += OnGraphicsQualityChanged;
		}

		public async UniTask SpawnCarToGaragePoint()
		{
			RallyMapDataSO rallyMapDataSO = ((RallyController != null) ? RallyController.RallyMapDataSO : null);
			_currentRallyCarData = ((rallyMapDataSO != null && rallyMapDataSO.TryGetConfigIds(RallyController.LocationIndex, RallyController.TrajectoryIndex, out var locationId, out var trajectoryId)) ? _carsDatasSo.GetRallyCarData(locationId, trajectoryId, _allCarsInfo) : null);
			if (_currentRallyCarData == null)
			{
				RallyController.ExitToMainMenu();
				return;
			}
			(CarInfo, GameObject) obj = await _carLoadingUtility.LoadDefaultCar(_currentRallyCarData.MainCarValues.CarID, _racingSequenceData.CarStartGaragePoint.Position, _racingSequenceData.CarStartGaragePoint.Rotation, tryUnloadCurrent: true, waitBodyLoad: false, _racingSequenceData.CarStartGaragePoint.Position.y + 2f);
			CarInfo item = obj.Item1;
			V.Car = obj.Item2;
			V.CarId = _currentRallyCarData.MainCarValues.CarID;
			V.Car.transform.rotation = _racingSequenceData.CarStartGaragePoint.Rotation;
			if (V.Car.TryGetComponent<Rigidbody>(out var carRigidbody))
			{
				carRigidbody.velocity = Vector3.zero;
				carRigidbody.angularVelocity = Vector3.zero;
				carRigidbody.isKinematic = true;
			}
			CarDefaultTuningDataConfig defaultTuningConfig = item?.GetDefaultTuningConfig();
			ApplyStockWheelData(defaultTuningConfig);
			await UniTask.Yield();
			if (V.Car.TryGetComponent<VehicleData>(out var carData))
			{
				if (carData.IsCarGraphicLoaded)
				{
					SetRallyCarInterior();
				}
				else
				{
					carData.OnCarGraphicLoaded += SetRallyCarInterior;
					this.GetCancellationTokenOnDestroy().Register(() =>
					{
						if ((bool)carData)
						{
							carData.OnCarGraphicLoaded -= SetRallyCarInterior;
						}
					});
				}
			}
			Powertrain dr = V.Car.GetComponent<Powertrain>();
			V.Car.GetComponent<VehicleEntityProvider>().IsRallyCar = true;
			ApplyRallyConfig();
			if ((bool)carData && (bool)carData.BodyKits)
			{
				carData.BodyKits.AllBodyKitsWasLoaded.Where((bool loaded) => loaded).First().Subscribe((bool _) =>
				{
					carData.BodyKits.LoadAllKits();
				})
					.AddTo(carData.gameObject);
			}
			SetStockRimColors(carData, defaultTuningConfig);
			SetLiveryPreset();
			V.LastUsedCar = V.Car;
			_rallyCar = _prefabFactory.InstantiateComponent<RallyCar>(V.Car);
			await UniTask.Yield();
			_rallyCar.SetPosition(_racingSequenceData.CarStartGaragePoint.Position, _racingSequenceData.CarStartGaragePoint.Rotation);
			if (carRigidbody != null)
			{
				carRigidbody.isKinematic = false;
			}
			_rallyCar.FixCar(state: true);
			SetMirror();
			void ApplyRallyConfig()
			{
				_carLoadingUtility.LoadCarConfig(V.Car, _currentRallyCarData.MainCarValues);
				dr.Automatic = dr.IsAutomaticGearbox;
				dr.GetDynoData(out var desc);
				Debug.Log($"[Rally] tyres applied — front:{desc.FrontTyre.TyreType} rear:{desc.RearTyre.TyreType}" + $" | fricMult F:{desc.FrontTyre.FrictionMultiplier} R:{desc.RearTyre.FrictionMultiplier}" + $" | config TyreType:{_currentRallyCarData.TyreType}");
			}
			void SetRallyCarInterior()
			{
				if ((bool)carData)
				{
					carData.OnCarGraphicLoaded -= SetRallyCarInterior;
					carData.InteriorsComposition.ForceSetSavedInterior(this.GetCancellationTokenOnDestroy()).Forget();
				}
			}
		}

		private void ApplyStockWheelData(CarDefaultTuningDataConfig defaultTuningConfig)
		{
			FrontWheelData frontWheelData = CloneWheelData(defaultTuningConfig?.FrontWheelData);
			RearWheelData rearWheelData = CloneWheelData(defaultTuningConfig?.RearWheelData);
			if (frontWheelData != null && rearWheelData != null)
			{
				frontWheelData.tyreType = _currentRallyCarData.TyreType;
				rearWheelData.tyreType = _currentRallyCarData.TyreType;
				_currentRallyCarData.MainCarValues.FrontWheelData = frontWheelData;
				_currentRallyCarData.MainCarValues.RearWheelData = rearWheelData;
			}
		}

		private static FrontWheelData CloneWheelData(FrontWheelData source)
		{
			if (source == null)
			{
				return null;
			}
			FrontWheelData obj = (FrontWheelData)source.Clone();
			obj.BrakesData = (BrakesData)(source.BrakesData?.Clone());
			obj.SuspensionData = (SteerSuspensionData)(source.SuspensionData?.Clone());
			return obj;
		}

		private static RearWheelData CloneWheelData(RearWheelData source)
		{
			if (source == null)
			{
				return null;
			}
			RearWheelData obj = (RearWheelData)source.Clone();
			obj.BrakesData = (BrakesData)(source.BrakesData?.Clone());
			obj.SuspensionData = (SuspensionData)(source.SuspensionData?.Clone());
			return obj;
		}

		private static void SetStockRimColors(VehicleData carData, CarDefaultTuningDataConfig defaultTuningConfig)
		{
			if ((bool)carData && defaultTuningConfig != null && carData.TryGetComponent<Axles>(out var component))
			{
				PaintAxleRims(component.FrontAxle, defaultTuningConfig.FrontWheelData);
				PaintAxleRims(component.RearAxle, defaultTuningConfig.RearWheelData);
				PaintAxleRims(component.Rear1Axle, defaultTuningConfig.RearWheelData);
				PaintAxleRims(component.Rear2Axle, defaultTuningConfig.RearWheelData);
			}
		}

		private static void PaintAxleRims(Axle axle, WheelData wheelData)
		{
			if (axle?.Wheels == null || wheelData == null)
			{
				return;
			}
			foreach (WheelProvider wheel in axle.Wheels)
			{
				WheelModel wheelModel = (wheel ? wheel.WheelModel : null);
				if ((bool)wheelModel)
				{
					PaintRim(wheelModel.Rim, wheelData.Color1);
					PaintRim(wheelModel.Rim2, wheelData.Color2);
					if ((bool)wheelModel.LodModel && wheelModel.LodModel.TryGetComponent<MeshRenderer>(out var component))
					{
						Color color = wheelData.Color1.Color;
						color.a = 1f;
						component.material.color = color;
					}
				}
			}
		}

		private static void PaintRim(GameObject rim, CarPaintColor color)
		{
			if ((bool)rim && rim.TryGetComponent<MeshRenderer>(out var component))
			{
				ColorsService.SetCarPaintColor(component.material, color);
			}
		}

		private void SetMirror()
		{
			bool mirror = (FlagsStorage.ShowCarMirror = _graphicSettings.Quality >= 2);
			if ((bool)_rallyCar && _rallyCar.TryGetComponent<VehicleData>(out var component) && component.CarMirrorController != null)
			{
				component.CarMirrorController.SetMirror(mirror);
			}
		}

		public void SetLights()
		{
			if (_rallyCar.TryGetComponent<LightController>(out var component))
			{
				if (RallyController.DayTimeType == DayTimeType.Night || RallyController.DayTimeType == DayTimeType.Sunset)
				{
					component.HState = 0;
					_quickCarSettings.TurnOnLights();
					_quickCarSettings.TurnOnLights();
				}
				else
				{
					component.HState = 2;
					_quickCarSettings.TurnOnLights();
				}
			}
		}

		private void SetLiveryPreset()
		{
			if (!(_currentRallyCarData.LiveryPreset == null) && _currentRallyCarData.LiveryPreset.allPresetList.Count != 0 && V.Car.TryGetComponent<VehicleData>(out var component))
			{
				if (!component.CarMat)
				{
					component.ColorsService.CreateFirstMat(component);
				}
				OneVinylsPreset preset = _currentRallyCarData.LiveryPreset.allPresetList[0];
				_drawQueue.Enqueue(new RallyLiveryPerformer(component, preset, _vinylDrawer, _vinylsDataUtility));
			}
		}

		public async UniTask MoveToStagePoint()
		{
			_rallyCar.SetPosition(_racingSequenceData.CarStagePoint.Position, _racingSequenceData.CarStagePoint.Rotation);
			_rallyCar.SetStagePoint(_racingSequenceData.CarStagePoint);
		}

		public void MoveToStartPoint(Transform startPoint)
		{
			V.Car.transform.SetPositionAndRotation(startPoint.position, startPoint.rotation);
		}

		public void MoveToStopPoint()
		{
			List<RallySequenceItem> rallySequenceItems = _racingSequenceData.RallySequenceItems;
			RallySequenceItem rallySequenceItem = ((rallySequenceItems.Count > 0) ? rallySequenceItems[rallySequenceItems.Count - 1] : null);
			if (rallySequenceItem != null)
			{
				Vector3 posByWheel = VectorExtensions.GetPosByWheel(V.Car, rallySequenceItem.Position);
				base.transform.SetPositionAndRotation(posByWheel, rallySequenceItem.Rotation);
			}
		}

		private void OnDestroy()
		{
			if ((bool)CPMGraphicsSettingsManager.Instance)
			{
				CPMGraphicsSettingsManager.Instance.OnQualityChanged -= OnGraphicsQualityChanged;
			}
			FlagsStorage.ShowCarMirror = PlayerPrefs.GetInt("showCarMirror", 0) == 1;
		}
	}
}
