using System.Linq;
using System.Reflection;
using CPMEngine;
using Unity.Mathematics;
using UnityEngine;
using _Car_Parking.Scripts.Drag.Engine;

namespace _Car_Parking.Scripts.EngineTuning
{
	[CreateAssetMenu(fileName = "EngineDefaultConfig", menuName = "CarConfigs/EngineDefaultConfig", order = 1)]
	public class EngineItem : PartItem
	{
		[SerializeField]
		[HideInInspector]
		private long _engineArchKey;

		[SerializeField]
		[HideInInspector]
		private long _engineArchStamp;

		[Header("Stock parts")]
		public EngineId EngineId;

		public BlockPart Block;

		public CrankshaftPart Crankshaft;

		public ConnectionRodsPart ConnectionRod;

		public PistonsPart Pistons;

		public FlywheelPart Flywheel;

		public CylinderHeadPart CylinderHead;

		public CamshaftPart Camshaft1;

		public CamshaftPart Camshaft2;

		public IntakeManifoldPart IntakeManifold;

		public AirFilterPart AirFilter;

		public ExhaustHeaderPart ExhaustHeader;

		public ExhaustPipePart ExhaustPipe;

		public ChargerPart Charger;

		public FuelRailPart FuelRail;

		public FuelInjectorsPart FuelInjectors;

		public ThrottleValvePart ThrottleValve;

		public FuelPumpPart FuelPump;

		public ClutchPart Clutch;

		public RadiatorPart Radiator;

		public IntercoolerPart Intercooler;

		[Header("Wankel parts")]
		public RotorHousingPart RotorHousing;

		public RotorPart Rotors;

		public RotorTrackingGearPart RotorTrackingGear;

		public EccentricShaftPart EccentricShaft;

		public EngineType GetTypeEngine => (EngineType)ReadArch();

		public override string GetMaxReliability => base.GetMaxReliability + " " + Utils.GetTorquePostfix.ToUpper();

		public override string GetName => FormatName(base.GetName);

		protected override void OnValidate()
		{
			base.OnValidate();
		}

		public EngineItem()
		{
			TypeId = PartType.Engine;
		}

		private static ulong ArchMask(int partId)
		{
			return (ulong)((((-3750763034362895579L ^ (uint)(partId * 2654435761u)) * 1099511628211L) ^ 0x51C64B7E29A8D3F5L) * 1099511628211L);
		}

		private static long ArchStamp(long key, int partId)
		{
			long num = -5435081209227447693L;
			num = (num * 31) ^ key;
			num = (num * 31) ^ (uint)(partId * 3266489917u);
			if (num == 0L)
			{
				num = 1L;
			}
			return num;
		}

		private int ReadArch()
		{
			if (ArchStamp(_engineArchKey, ModelId) != _engineArchStamp)
			{
				return 0;
			}
			return (int)(_engineArchKey ^ (long)ArchMask(ModelId));
		}

		public bool HasFlag(EngineType flag)
		{
			return ((uint)ReadArch() & (uint)flag) != 0;
		}

		public float GetMass()
		{
			float num = 0f;
			FieldInfo[] fields = typeof(EngineItem).GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				if (fields[i].GetValue(this) is PartItem partItem)
				{
					num += partItem.Mass;
				}
			}
			return num;
		}

		public float GetTotalMileage()
		{
			float num = 0f;
			int num2 = 0;
			FieldInfo[] fields = typeof(EngineItem).GetFields();
			for (int i = 0; i < fields.Length; i++)
			{
				object value = fields[i].GetValue(this);
				if (value != null)
				{
					if (value is PartItem partItem)
					{
						num += partItem.MaxMileage;
					}
					num2++;
				}
			}
			return math.round(num / (float)num2);
		}

		public string FormatName(string displayName)
		{
			if (RotorHousing != null)
			{
				if (EccentricShaft == null)
				{
					Debug.LogError("[EngineItem] '" + base.name + "' has RotorHousing but no EccentricShaft — cannot compute Wankel displacement. Showing name without (cc) suffix.", this);
					return $"({GetTypeEngine}) {displayName}";
				}
				float num = Utils.RoundTo(Utils.Mm2Meter(3f * Mathf.Sqrt(3f) * RotorHousing.GeneratingRadius * EccentricShaft.Eccentricity * RotorHousing.HousingWidth) * (float)RotorHousing.Rotors, 0);
				return $"({GetTypeEngine}) {displayName} ({num} cc)";
			}
			if (!Pistons || !Crankshaft || !Block)
			{
				string text = ((Pistons == null) ? "Pistons" : ((!Crankshaft) ? "Crankshaft" : "Block"));
				Debug.LogError("[EngineItem] '" + base.name + "' is missing geometry part '" + text + "' — cannot compute displacement. Showing name without (cc) suffix.", this);
				return $"({GetTypeEngine}) {displayName}";
			}
			float num2 = Utils.RoundTo(Pistons.GetBore * Pistons.GetBore * 0.7853975f * Crankshaft.GetStroke * 0.001f * (float)Block.Cylinders, 0);
			return $"({GetTypeEngine}) {displayName} ({num2} cc)";
		}

		public ECUPart GetCompatibleECU(AllParts allParts, int carId)
		{
			ECUPart eCUPart = (from x in allParts.GetParts(PartType.ECU)
				select (ECUPart)x).FirstOrDefault((ECUPart x) => x.IsCarLinked(carId) && !x.CheckNotCompatibleFull(carId, EngineId.ModelId));
			if (!eCUPart)
			{
				eCUPart = (from x in allParts.GetParts(PartType.ECU)
					select (ECUPart)x).FirstOrDefault((ECUPart x) => x.Rarity == 1 && !x.CheckNotCompatibleFull(carId, EngineId.ModelId));
			}
			return eCUPart;
		}
	}
}
