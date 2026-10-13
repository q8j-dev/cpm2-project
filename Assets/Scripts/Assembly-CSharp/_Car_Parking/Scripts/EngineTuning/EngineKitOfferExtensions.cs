using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.Drag.Engine;

namespace _Car_Parking.Scripts.EngineTuning
{
	public static class EngineKitOfferExtensions
	{
		internal static readonly Dictionary<PartType, string> PartTypeToSlot;

		internal static readonly FieldInfo[] EngineItemFields;

		internal static readonly FieldInfo[] EngineKitDataFields;

		internal static readonly Dictionary<string, FieldInfo> EngineKitDataFieldBySlot;

		internal static readonly Dictionary<string, FieldInfo> EngineKitDataFieldByName;

		private static readonly HashSet<string> _slotsOverriddenScratch;

		static EngineKitOfferExtensions()
		{
			PartTypeToSlot = new Dictionary<PartType, string>
			{
				{
					PartType.TurboCharger,
					"Charger"
				},
				{
					PartType.SuperCharger,
					"Charger"
				},
				{
					PartType.Intercooler,
					"Intercooler"
				},
				{
					PartType.Radiator,
					"Radiator"
				},
				{
					PartType.IntakeManifold,
					"IntakeManifold"
				},
				{
					PartType.AirFilter,
					"AirFilter"
				},
				{
					PartType.ExhaustHeader,
					"ExhaustHeader"
				},
				{
					PartType.ExhaustPipe,
					"ExhaustPipe"
				},
				{
					PartType.FuelRail,
					"FuelRail"
				},
				{
					PartType.FuelInjectors,
					"FuelInjectors"
				},
				{
					PartType.FuelPump,
					"FuelPump"
				},
				{
					PartType.ThrottleValve,
					"ThrottleValve"
				},
				{
					PartType.Clutch,
					"Clutch"
				},
				{
					PartType.Block_Inline,
					"Block"
				},
				{
					PartType.Block_Vee,
					"Block"
				},
				{
					PartType.Crankshaft,
					"Crankshaft"
				},
				{
					PartType.ConnectionRod,
					"ConnectionRod"
				},
				{
					PartType.Pistons,
					"Pistons"
				},
				{
					PartType.Flywheel,
					"Flywheel"
				},
				{
					PartType.SOHC_CylinderHead,
					"CylinderHead"
				},
				{
					PartType.DOHC_CylinderHead,
					"CylinderHead"
				},
				{
					PartType.OHV_CylinderHead,
					"CylinderHead"
				},
				{
					PartType.SOHC_Camshaft,
					"Camshaft1"
				},
				{
					PartType.DOHC_Camshaft_Intake,
					"Camshaft1"
				},
				{
					PartType.OHV_Camshaft,
					"Camshaft1"
				},
				{
					PartType.DOHC_Camshaft_Exhaust,
					"Camshaft2"
				},
				{
					PartType.RotorHousing,
					"RotorHousing"
				},
				{
					PartType.Rotor,
					"Rotors"
				},
				{
					PartType.RotorTrackingGear,
					"RotorTrackingGear"
				},
				{
					PartType.EccentricShaft,
					"EccentricShaft"
				}
			};
			EngineItemFields = typeof(EngineItem).GetFields(BindingFlags.Instance | BindingFlags.Public);
			EngineKitDataFields = typeof(EngineKitData).GetFields(BindingFlags.Instance | BindingFlags.Public);
			_slotsOverriddenScratch = new HashSet<string>();
			EngineKitDataFieldByName = new Dictionary<string, FieldInfo>(EngineKitDataFields.Length);
			FieldInfo[] engineKitDataFields = EngineKitDataFields;
			foreach (FieldInfo fieldInfo in engineKitDataFields)
			{
				EngineKitDataFieldByName[fieldInfo.Name] = fieldInfo;
			}
			EngineKitDataFieldBySlot = new Dictionary<string, FieldInfo>();
			foreach (string value2 in PartTypeToSlot.Values)
			{
				if (typeof(EngineItem).GetField(value2) == null)
				{
					Debug.LogError("[EngineKitOfferExtensions] EngineItem has no field '" + value2 + "'. PartTypeToSlot is out of sync — GetEffectiveParts will miss this slot and the shop price/mass will silently diverge from the assembled kit.");
				}
				if (!EngineKitDataFieldByName.TryGetValue(value2, out var value))
				{
					Debug.LogError("[EngineKitOfferExtensions] EngineKitData has no field '" + value2 + "'. PartTypeToSlot is out of sync — ApplyStockExtras will silently drop extras for any PartType mapped to this slot, so bought kits will be missing parts.");
				}
				else
				{
					EngineKitDataFieldBySlot[value2] = value;
				}
			}
		}

		public static ECUPart GetEffectiveECU(this EngineKitOffer offer, AllParts allParts, int carId)
		{
			if (offer == null)
			{
				return null;
			}
			if (offer.ECU != null)
			{
				return offer.ECU;
			}
			if (!(offer.Base != null))
			{
				return null;
			}
			return offer.Base.GetCompatibleECU(allParts, carId);
		}

		public static void GetFullPrice(this EngineKitOffer offer, ECUPart ecu, out AdvancedInt total)
		{
			int num = 0;
			if (offer != null && offer.Base != null)
			{
				HashSet<string> slotsOverriddenScratch = _slotsOverriddenScratch;
				slotsOverriddenScratch.Clear();
				if (offer.Extras != null)
				{
					PartItem[] extras = offer.Extras;
					foreach (PartItem partItem in extras)
					{
						if ((bool)partItem && PartTypeToSlot.TryGetValue(partItem.TypeId, out var value))
						{
							slotsOverriddenScratch.Add(value);
							num += (int)partItem.GetPrice;
						}
					}
				}
				FieldInfo[] engineItemFields = EngineItemFields;
				foreach (FieldInfo fieldInfo in engineItemFields)
				{
					if (!slotsOverriddenScratch.Contains(fieldInfo.Name) && fieldInfo.GetValue(offer.Base) is PartItem partItem2 && partItem2 != null)
					{
						num += (int)partItem2.GetPrice;
					}
				}
			}
			int num2 = ((ecu != null) ? Mathf.RoundToInt((float)(int)ecu.GetPrice * 0.8f) : 0);
			int num3 = Mathf.RoundToInt((float)(num + num2) / 1000f) * 1000;
			total = new AdvancedInt(num3);
		}

		public static void GetFullPrice(this EngineKitOffer offer, AllParts allParts, int carId, out AdvancedInt total)
		{
			offer.GetFullPrice(offer.GetEffectiveECU(allParts, carId), out total);
		}

		public static float GetMass(this EngineKitOffer offer)
		{
			if (offer == null || offer.Base == null)
			{
				return 0f;
			}
			HashSet<string> slotsOverriddenScratch = _slotsOverriddenScratch;
			slotsOverriddenScratch.Clear();
			float num = 0f;
			if (offer.Extras != null)
			{
				PartItem[] extras = offer.Extras;
				foreach (PartItem partItem in extras)
				{
					if ((bool)partItem && PartTypeToSlot.TryGetValue(partItem.TypeId, out var value))
					{
						slotsOverriddenScratch.Add(value);
						num += partItem.Mass;
					}
				}
			}
			FieldInfo[] engineItemFields = EngineItemFields;
			foreach (FieldInfo fieldInfo in engineItemFields)
			{
				if (!slotsOverriddenScratch.Contains(fieldInfo.Name) && fieldInfo.GetValue(offer.Base) is PartItem partItem2 && partItem2 != null)
				{
					num += partItem2.Mass;
				}
			}
			return num;
		}

		public static float GetTotalMileage(this EngineKitOffer offer)
		{
			if (offer == null || offer.Base == null)
			{
				return 0f;
			}
			HashSet<string> slotsOverriddenScratch = _slotsOverriddenScratch;
			slotsOverriddenScratch.Clear();
			float num = 0f;
			int num2 = 0;
			if (offer.Extras != null)
			{
				PartItem[] extras = offer.Extras;
				foreach (PartItem partItem in extras)
				{
					if ((bool)partItem && PartTypeToSlot.TryGetValue(partItem.TypeId, out var value))
					{
						slotsOverriddenScratch.Add(value);
						num += partItem.MaxMileage;
						num2++;
					}
				}
			}
			FieldInfo[] engineItemFields = EngineItemFields;
			foreach (FieldInfo fieldInfo in engineItemFields)
			{
				if (!slotsOverriddenScratch.Contains(fieldInfo.Name) && fieldInfo.GetValue(offer.Base) is PartItem partItem2 && partItem2 != null)
				{
					num += partItem2.MaxMileage;
					num2++;
				}
			}
			if (num2 != 0)
			{
				return Mathf.Round(num / (float)num2);
			}
			return 0f;
		}

		public static string GetDisplayName(this EngineKitOffer offer)
		{
			if (offer == null || offer.Base == null)
			{
				return string.Empty;
			}
			if (!string.IsNullOrEmpty(offer.Name))
			{
				return offer.Base.FormatName(offer.Name);
			}
			return offer.Base.GetName;
		}

		public static string GetDisplayDescription(this EngineKitOffer offer)
		{
			object obj;
			if (!(offer != null) || string.IsNullOrEmpty(offer.Description))
			{
				obj = offer?.Base?.Description;
				if (obj == null)
				{
					return string.Empty;
				}
			}
			else
			{
				obj = offer.Description;
			}
			return (string)obj;
		}
	}
}
