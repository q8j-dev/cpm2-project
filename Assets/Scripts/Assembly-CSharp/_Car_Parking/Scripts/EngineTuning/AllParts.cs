using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.EngineTuning
{
	[Preserve]
	[CreateAssetMenu(fileName = "AllParts", menuName = "CarPartsTuning/AllParts", order = 0)]
	public class AllParts : ScriptableObject, IInitializable
	{
		public static AllParts Instance;

		private readonly Dictionary<PartType, List<PartItem>> _partsByTypeCache = new Dictionary<PartType, List<PartItem>>();

		public int TotalParts;

		[Header("Car")]
		public PartsData<EngineItem> EngineKits = new PartsData<EngineItem>();

		public List<EngineKitOffer> EngineKitOffers = new List<EngineKitOffer>();

		public PartsData<EngineId> EngineIds = new PartsData<EngineId>();

		[Header("Engine parts")]
		public PartsData<Block_Inline> BlocksInline = new PartsData<Block_Inline>();

		public PartsData<Block_Vee> BlocksVee = new PartsData<Block_Vee>();

		public PartsData<CrankshaftPart> Crankshafts = new PartsData<CrankshaftPart>();

		public PartsData<ConnectionRodsPart> ConnectionRods = new PartsData<ConnectionRodsPart>();

		public PartsData<PistonsPart> Pistons = new PartsData<PistonsPart>();

		public PartsData<FlywheelPart> Flywheels = new PartsData<FlywheelPart>();

		public PartsData<CylinderHeadPart> CylinderHeads = new PartsData<CylinderHeadPart>();

		public PartsData<SOHC_Camshaft> SOHCCamshafts = new PartsData<SOHC_Camshaft>();

		public PartsData<DOHC_Camshaft_Intake> DOHCCamshaftsIn = new PartsData<DOHC_Camshaft_Intake>();

		public PartsData<DOHC_Camshaft_Exhaust> DOHCCamshaftsEx = new PartsData<DOHC_Camshaft_Exhaust>();

		public PartsData<OHV_Camshaft> OHVCamshafts = new PartsData<OHV_Camshaft>();

		public PartsData<IntakeManifoldPart> IntakeManifolds = new PartsData<IntakeManifoldPart>();

		public PartsData<ThrottleValvePart> ThrottleValves = new PartsData<ThrottleValvePart>();

		public PartsData<FuelRailPart> FuelRails = new PartsData<FuelRailPart>();

		public PartsData<FuelInjectorsPart> FuelInjectors = new PartsData<FuelInjectorsPart>();

		public PartsData<ExhaustHeaderPart> ExhaustHeaders = new PartsData<ExhaustHeaderPart>();

		public PartsData<ExhaustPipePart> ExhaustPipes = new PartsData<ExhaustPipePart>();

		public PartsData<GearBoxPart> GearBoxes = new PartsData<GearBoxPart>();

		public PartsData<ClutchPart> Clutches = new PartsData<ClutchPart>();

		public PartsData<TurboChargerPart> TurboChargers = new PartsData<TurboChargerPart>();

		public PartsData<SuperChargerPart> SuperChargers = new PartsData<SuperChargerPart>();

		public PartsData<ECUPart> ECUs = new PartsData<ECUPart>();

		public PartsData<FuelPumpPart> FuelPumps = new PartsData<FuelPumpPart>();

		public PartsData<AirFilterPart> AirFilters = new PartsData<AirFilterPart>();

		public PartsData<RadiatorPart> Radiators = new PartsData<RadiatorPart>();

		public PartsData<IntercoolerPart> Intercoolers = new PartsData<IntercoolerPart>();

		public PartsData<FrontLSDPart> FrontLSD = new PartsData<FrontLSDPart>();

		public PartsData<RearLSDPart> RearLSD = new PartsData<RearLSDPart>();

		public PartsData<RotorHousingPart> RotorHousings = new PartsData<RotorHousingPart>();

		public PartsData<EccentricShaftPart> EccentricShafts = new PartsData<EccentricShaftPart>();

		public PartsData<RotorPart> Rotors = new PartsData<RotorPart>();

		public PartsData<RotorTrackingGearPart> RotorTrackingGears = new PartsData<RotorTrackingGearPart>();

		[Header("Vendors")]
		public DetailVendorsConfigs DetailVendorsConfigs;

		[Preserve]
		public AllParts()
		{
		}

		public void Initialize()
		{
			if (Instance == null)
			{
				Instance = this;
			}
		}

		public TPart GetPart<TPart>(PartData partData) where TPart : PartItem
		{
			partData = CheckCompactPartDataFromDataBase(partData);
			if (partData == null || partData.ModelId == 0)
			{
				return null;
			}
			return GetPart<TPart>(partData.PartType, partData.ModelId);
		}

		public TPart GetPartWithoutCheck<TPart>(PartData partData) where TPart : PartItem
		{
			return GetPart<TPart>(partData.PartType, partData.ModelId);
		}

		public PartItem GetPart(PartData partData)
		{
			if (partData == null)
			{
				return null;
			}
			partData = CheckCompactPartDataFromDataBase(partData);
			if (partData == null || partData.ModelId == 0)
			{
				return null;
			}
			return GetPart<PartItem>(partData.PartType, partData.ModelId);
		}

		public TPartInstance CreateCustomInstance<TPartInstance, TPart>(PartData partData) where TPartInstance : PartInstance<TPart>, new() where TPart : PartItem
		{
			if (partData == null || partData.ModelId == 0)
			{
				return null;
			}
			TPartInstance val = new TPartInstance();
			TPart part = (val.Part = GetPart<TPart>(partData.CheckPartData()));
			val.InstanceId = partData.InstanceID;
			val.Mileage = partData.GetMileage(part);
			val.UpdateVariables();
			return val;
		}

		public BlockPart GetCompatibleBlockFromEngine(int engineId)
		{
			BlockPart blockPart = GetParts(PartType.Block_Inline).OfType<BlockPart>().FirstOrDefault((BlockPart x) => x.IsEngineLinked(engineId));
			if (!blockPart)
			{
				blockPart = GetParts(PartType.Block_Vee).OfType<BlockPart>().FirstOrDefault((BlockPart x) => x.IsEngineLinked(engineId));
			}
			return blockPart;
		}

		public ECUPart GetCompatibleEcuFromEngine(int engineId, int carId)
		{
			ECUPart eCUPart = GetParts(PartType.ECU).OfType<ECUPart>().FirstOrDefault((ECUPart x) => x.IsEngineLinked(engineId) && x.IsCarLinked(carId));
			if (!eCUPart)
			{
				eCUPart = (from x in GetParts(PartType.ECU).OfType<ECUPart>()
					where x.Rarity == 1
					select x).FirstOrDefault((ECUPart x) => x.IsEngineLinked(engineId));
			}
			return eCUPart;
		}

		public CylinderHeadPart GetCompatibleCylinderHeadFromEngine(int engineId)
		{
			CylinderHeadPart cylinderHeadPart = GetParts(PartType.DOHC_CylinderHead).OfType<CylinderHeadPart>().FirstOrDefault((CylinderHeadPart x) => x.IsEngineLinked(engineId));
			if (!cylinderHeadPart)
			{
				cylinderHeadPart = GetParts(PartType.SOHC_CylinderHead).OfType<CylinderHeadPart>().FirstOrDefault((CylinderHeadPart x) => x.IsEngineLinked(engineId));
				if (!cylinderHeadPart)
				{
					cylinderHeadPart = GetParts(PartType.OHV_CylinderHead).OfType<CylinderHeadPart>().FirstOrDefault((CylinderHeadPart x) => x.IsEngineLinked(engineId));
				}
			}
			return cylinderHeadPart;
		}

		public CamshaftPart GetCompatibleCamshaft(int engineId, PartType partType)
		{
			CamshaftPart result = null;
			switch (partType)
			{
			case PartType.DOHC_Camshaft_Intake:
				result = GetParts(PartType.DOHC_Camshaft_Intake).OfType<DOHC_Camshaft_Intake>().FirstOrDefault((DOHC_Camshaft_Intake x) => x.IsEngineLinked(engineId));
				break;
			case PartType.DOHC_Camshaft_Exhaust:
				result = GetParts(PartType.DOHC_Camshaft_Exhaust).OfType<DOHC_Camshaft_Exhaust>().FirstOrDefault((DOHC_Camshaft_Exhaust x) => x.IsEngineLinked(engineId));
				break;
			case PartType.SOHC_Camshaft:
				result = GetParts(PartType.SOHC_Camshaft).OfType<SOHC_Camshaft>().FirstOrDefault((SOHC_Camshaft x) => x.IsEngineLinked(engineId));
				break;
			case PartType.OHV_Camshaft:
				result = GetParts(PartType.OHV_Camshaft).OfType<OHV_Camshaft>().FirstOrDefault((OHV_Camshaft x) => x.IsEngineLinked(engineId));
				break;
			}
			return result;
		}

		public T GetCompatiblePartFromEngine<T>(int engineId, PartType partType) where T : PartItem
		{
			return GetParts(partType).OfType<T>().FirstOrDefault((T x) => x.IsEngineLinked(engineId));
		}

		public PartInstance<TPart> GetPartInstance<TPart>(PartData partData) where TPart : PartItem
		{
			if (partData == null || partData.ModelId == 0)
			{
				return null;
			}
			partData = CheckCompactPartDataFromDataBase(partData);
			TPart part = GetPart<TPart>(partData.PartType, partData.ModelId);
			if (!part)
			{
				return null;
			}
			PartInstance<TPart> partInstance = new PartInstance<TPart>();
			partInstance.Part = part;
			partInstance.InstanceId = partData.InstanceID;
			partInstance.Mileage = partData.GetMileage(part);
			partInstance.UpdateVariables();
			return partInstance;
		}

		public int GetCompatiblePartsCount(int engineId, PartData partDataOnCar, int carId)
		{
			List<PartItem> parts = GetParts(partDataOnCar.PartType);
			Debug.Log($"All {partDataOnCar.PartType} parts: " + parts.Count);
			int num = 0;
			foreach (PartItem item in parts)
			{
				if (item.CheckNotCompatibleFull(carId, engineId))
				{
					num++;
				}
			}
			return num;
		}

		public PartData CheckCompactPartDataFromDataBase(PartData partData)
		{
			partData = partData.CheckPartData();
			return partData;
		}

		private static bool IsInductionType(PartType partType)
		{
			if (partType != PartType.TurboCharger && partType != PartType.SuperCharger)
			{
				return partType == PartType.Intercooler;
			}
			return true;
		}

		private EngineItem StockEngineOf(int modelId)
		{
			int num = modelId / 100000;
			List<EngineItem> list = EngineKits?.Models;
			if (list == null)
			{
				return null;
			}
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] != null && list[i].ModelId / 100000 == num)
				{
					return list[i];
				}
			}
			return null;
		}

		private TStock ExactOrStock<TStock>(PartsData<TStock> catalog, int modelId, Func<EngineItem, PartItem> stockOf) where TStock : PartItem
		{
			if (catalog.TryGetExact(modelId, out var part))
			{
				return part;
			}
			TStock val = stockOf(StockEngineOf(modelId)) as TStock;
			if (val != null)
			{
				Debug.LogWarning($"Наддув {modelId} не найден — подставлен стоковый для этого " + $"двигателя: {val.ModelId}.");
				return val;
			}
			Debug.LogWarning($"Наддув {modelId} не найден, штатного у двигателя нет — слот пуст.");
			return null;
		}

		public TPart GetPart<TPart>(PartType partType, int i) where TPart : PartItem
		{
			if (partType != PartType.TurboCharger)
			{
				if (partType != PartType.Intercooler)
				{
					if (partType == PartType.SuperCharger)
					{
						return ExactOrStock(SuperChargers, i, (EngineItem engine) => engine?.Charger) as TPart;
					}
					return partType switch
					{
						PartType.Engine => EngineKits[i] as TPart, 
						PartType.EngineId => EngineIds[i] as TPart, 
						PartType.Block_Inline => BlocksInline[i] as TPart, 
						PartType.Block_Vee => BlocksVee[i] as TPart, 
						PartType.Crankshaft => Crankshafts[i] as TPart, 
						PartType.ConnectionRod => ConnectionRods[i] as TPart, 
						PartType.Pistons => Pistons[i] as TPart, 
						PartType.Flywheel => Flywheels[i] as TPart, 
						PartType.SOHC_CylinderHead => CylinderHeads[i] as TPart, 
						PartType.DOHC_CylinderHead => CylinderHeads[i] as TPart, 
						PartType.OHV_CylinderHead => CylinderHeads[i] as TPart, 
						PartType.SOHC_Camshaft => SOHCCamshafts[i] as TPart, 
						PartType.DOHC_Camshaft_Intake => DOHCCamshaftsIn[i] as TPart, 
						PartType.DOHC_Camshaft_Exhaust => DOHCCamshaftsEx[i] as TPart, 
						PartType.OHV_Camshaft => OHVCamshafts[i] as TPart, 
						PartType.IntakeManifold => IntakeManifolds[i] as TPart, 
						PartType.ThrottleValve => ThrottleValves[i] as TPart, 
						PartType.FuelRail => FuelRails[i] as TPart, 
						PartType.FuelInjectors => FuelInjectors[i] as TPart, 
						PartType.ExhaustHeader => ExhaustHeaders[i] as TPart, 
						PartType.ExhaustPipe => ExhaustPipes[i] as TPart, 
						PartType.GearBox => GearBoxes[i] as TPart, 
						PartType.Clutch => Clutches[i] as TPart, 
						PartType.TurboCharger => TurboChargers[i] as TPart, 
						PartType.SuperCharger => SuperChargers[i] as TPart, 
						PartType.ECU => ECUs[i] as TPart, 
						PartType.FuelPump => FuelPumps[i] as TPart, 
						PartType.AirFilter => AirFilters[i] as TPart, 
						PartType.Radiator => Radiators[i] as TPart, 
						PartType.Intercooler => Intercoolers[i] as TPart, 
						PartType.FrontLSD => FrontLSD[i] as TPart, 
						PartType.RearLSD => RearLSD[i] as TPart, 
						PartType.RotorHousing => RotorHousings[i] as TPart, 
						PartType.EccentricShaft => EccentricShafts[i] as TPart, 
						PartType.RotorTrackingGear => RotorTrackingGears[i] as TPart, 
						PartType.Rotor => Rotors[i] as TPart, 
						_ => EngineIds[i] as TPart, 
					};
				}
				return ExactOrStock(Intercoolers, i, (EngineItem engine) => engine?.Intercooler) as TPart;
			}
			return ExactOrStock(TurboChargers, i, (EngineItem engine) => engine?.Charger) as TPart;
		}

		public List<PartItem> GetParts(PartType partType)
		{
			if (_partsByTypeCache.TryGetValue(partType, out var value))
			{
				return value;
			}
			List<PartItem> list = BuildPartsList(partType);
			_partsByTypeCache[partType] = list;
			return list;
		}

		private List<PartItem> BuildPartsList(PartType partType)
		{
			return partType switch
			{
				PartType.Engine => ((IEnumerable<EngineId>)EngineIds.Models).Select((Func<EngineId, PartItem>)((EngineId x) => x)).ToList(), 
				PartType.Block_Inline => ((IEnumerable<Block_Inline>)BlocksInline.Models).Select((Func<Block_Inline, PartItem>)((Block_Inline x) => x)).ToList(), 
				PartType.Block_Vee => ((IEnumerable<Block_Vee>)BlocksVee.Models).Select((Func<Block_Vee, PartItem>)((Block_Vee x) => x)).ToList(), 
				PartType.Crankshaft => ((IEnumerable<CrankshaftPart>)Crankshafts.Models).Select((Func<CrankshaftPart, PartItem>)((CrankshaftPart x) => x)).ToList(), 
				PartType.ConnectionRod => ((IEnumerable<ConnectionRodsPart>)ConnectionRods.Models).Select((Func<ConnectionRodsPart, PartItem>)((ConnectionRodsPart x) => x)).ToList(), 
				PartType.Pistons => ((IEnumerable<PistonsPart>)Pistons.Models).Select((Func<PistonsPart, PartItem>)((PistonsPart x) => x)).ToList(), 
				PartType.Flywheel => ((IEnumerable<FlywheelPart>)Flywheels.Models).Select((Func<FlywheelPart, PartItem>)((FlywheelPart x) => x)).ToList(), 
				PartType.SOHC_CylinderHead => CylinderHeads.Models.Where((CylinderHeadPart x) => x.CylinderHeadType == CylinderHeadPart.CylinderHeadTypeEnum.SOHC).Select((Func<CylinderHeadPart, PartItem>)((CylinderHeadPart x) => x)).ToList(), 
				PartType.DOHC_CylinderHead => CylinderHeads.Models.Where((CylinderHeadPart x) => x.CylinderHeadType == CylinderHeadPart.CylinderHeadTypeEnum.DOHC).Select((Func<CylinderHeadPart, PartItem>)((CylinderHeadPart x) => x)).ToList(), 
				PartType.OHV_CylinderHead => CylinderHeads.Models.Where((CylinderHeadPart x) => x.CylinderHeadType == CylinderHeadPart.CylinderHeadTypeEnum.OHV).Select((Func<CylinderHeadPart, PartItem>)((CylinderHeadPart x) => x)).ToList(), 
				PartType.SOHC_Camshaft => ((IEnumerable<SOHC_Camshaft>)SOHCCamshafts.Models).Select((Func<SOHC_Camshaft, PartItem>)((SOHC_Camshaft x) => x)).ToList(), 
				PartType.DOHC_Camshaft_Intake => ((IEnumerable<DOHC_Camshaft_Intake>)DOHCCamshaftsIn.Models).Select((Func<DOHC_Camshaft_Intake, PartItem>)((DOHC_Camshaft_Intake x) => x)).ToList(), 
				PartType.DOHC_Camshaft_Exhaust => ((IEnumerable<DOHC_Camshaft_Exhaust>)DOHCCamshaftsEx.Models).Select((Func<DOHC_Camshaft_Exhaust, PartItem>)((DOHC_Camshaft_Exhaust x) => x)).ToList(), 
				PartType.OHV_Camshaft => ((IEnumerable<OHV_Camshaft>)OHVCamshafts.Models).Select((Func<OHV_Camshaft, PartItem>)((OHV_Camshaft x) => x)).ToList(), 
				PartType.IntakeManifold => ((IEnumerable<IntakeManifoldPart>)IntakeManifolds.Models).Select((Func<IntakeManifoldPart, PartItem>)((IntakeManifoldPart x) => x)).ToList(), 
				PartType.AirFilter => ((IEnumerable<AirFilterPart>)AirFilters.Models).Select((Func<AirFilterPart, PartItem>)((AirFilterPart x) => x)).ToList(), 
				PartType.ThrottleValve => ((IEnumerable<ThrottleValvePart>)ThrottleValves.Models).Select((Func<ThrottleValvePart, PartItem>)((ThrottleValvePart x) => x)).ToList(), 
				PartType.FuelRail => ((IEnumerable<FuelRailPart>)FuelRails.Models).Select((Func<FuelRailPart, PartItem>)((FuelRailPart x) => x)).ToList(), 
				PartType.FuelInjectors => ((IEnumerable<FuelInjectorsPart>)FuelInjectors.Models).Select((Func<FuelInjectorsPart, PartItem>)((FuelInjectorsPart x) => x)).ToList(), 
				PartType.ExhaustHeader => ((IEnumerable<ExhaustHeaderPart>)ExhaustHeaders.Models).Select((Func<ExhaustHeaderPart, PartItem>)((ExhaustHeaderPart x) => x)).ToList(), 
				PartType.ExhaustPipe => ((IEnumerable<ExhaustPipePart>)ExhaustPipes.Models).Select((Func<ExhaustPipePart, PartItem>)((ExhaustPipePart x) => x)).ToList(), 
				PartType.GearBox => ((IEnumerable<GearBoxPart>)GearBoxes.Models).Select((Func<GearBoxPart, PartItem>)((GearBoxPart x) => x)).ToList(), 
				PartType.Clutch => ((IEnumerable<ClutchPart>)Clutches.Models).Select((Func<ClutchPart, PartItem>)((ClutchPart x) => x)).ToList(), 
				PartType.TurboCharger => ((IEnumerable<TurboChargerPart>)TurboChargers.Models).Select((Func<TurboChargerPart, PartItem>)((TurboChargerPart x) => x)).ToList(), 
				PartType.SuperCharger => ((IEnumerable<SuperChargerPart>)SuperChargers.Models).Select((Func<SuperChargerPart, PartItem>)((SuperChargerPart x) => x)).ToList(), 
				PartType.ECU => ((IEnumerable<ECUPart>)ECUs.Models).Select((Func<ECUPart, PartItem>)((ECUPart x) => x)).ToList(), 
				PartType.FuelPump => ((IEnumerable<FuelPumpPart>)FuelPumps.Models).Select((Func<FuelPumpPart, PartItem>)((FuelPumpPart x) => x)).ToList(), 
				PartType.Radiator => ((IEnumerable<RadiatorPart>)Radiators.Models).Select((Func<RadiatorPart, PartItem>)((RadiatorPart x) => x)).ToList(), 
				PartType.Intercooler => ((IEnumerable<IntercoolerPart>)Intercoolers.Models).Select((Func<IntercoolerPart, PartItem>)((IntercoolerPart x) => x)).ToList(), 
				PartType.FrontLSD => ((IEnumerable<FrontLSDPart>)FrontLSD.Models).Select((Func<FrontLSDPart, PartItem>)((FrontLSDPart x) => x)).ToList(), 
				PartType.RearLSD => ((IEnumerable<RearLSDPart>)RearLSD.Models).Select((Func<RearLSDPart, PartItem>)((RearLSDPart x) => x)).ToList(), 
				PartType.RotorHousing => ((IEnumerable<RotorHousingPart>)RotorHousings.Models).Select((Func<RotorHousingPart, PartItem>)((RotorHousingPart x) => x)).ToList(), 
				PartType.EccentricShaft => ((IEnumerable<EccentricShaftPart>)EccentricShafts.Models).Select((Func<EccentricShaftPart, PartItem>)((EccentricShaftPart x) => x)).ToList(), 
				PartType.Rotor => ((IEnumerable<RotorPart>)Rotors.Models).Select((Func<RotorPart, PartItem>)((RotorPart x) => x)).ToList(), 
				PartType.RotorTrackingGear => ((IEnumerable<RotorTrackingGearPart>)RotorTrackingGears.Models).Select((Func<RotorTrackingGearPart, PartItem>)((RotorTrackingGearPart x) => x)).ToList(), 
				_ => throw new ArgumentOutOfRangeException(), 
			};
		}
	}
}
