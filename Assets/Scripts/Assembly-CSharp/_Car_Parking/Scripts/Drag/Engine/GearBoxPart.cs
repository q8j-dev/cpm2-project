using System;
using CPM.Shifter.Logic;
using CPM.Transmission;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[CreateAssetMenu(fileName = "GearBox", menuName = "CarPartsTuning/EngineParts/GearBox", order = 5)]
	public class GearBoxPart : ReciprocatingEnginePart
	{
		[Header("GearBox values")]
		[SerializeField]
		private GearboxType gearboxType = GearboxType.GearRwd;

		[SerializeField]
		private GearboxKind kind = GearboxKind.AT;

		[Tooltip("Опциональные авто-режимы шифтера (P/R/N/D всегда). Дефолт Manual — как у большинства АКПП.")]
		[SerializeField]
		private AutoShifterModes autoModes = AutoShifterModes.Manual;

		[Tooltip("L-режим (low gear lock): передача, на которой машина едет и трогается в L (1-based: 1=первая, 2=вторая, и т.д.). Дефолт 2 — смысл L в том, чтобы тронуться в натяг на скользком, а первая для этого слишком резвая. Внедорожнику, которому нужна именно первая для враскачку, ставь 1. Действует только если autoModes содержит Low.")]
		[SerializeField]
		[Range(1f, 9f)]
		private int lowMaxGear = 2;

		[Range(0.1f, 0.9f)]
		[SerializeField]
		private float TorqueSplit = 0.5f;

		[SerializeField]
		private float[] ratio = new float[8];

		[SerializeField]
		private float end_ratio = 4.4f;

		[SerializeField]
		private int adjustableGears;

		[HideInInspector]
		[SerializeField]
		private long _partLayoutKey;

		[HideInInspector]
		[SerializeField]
		private long _partLayoutStamp;

		[Header("Launch")]
		[SerializeField]
		private bool hasLaunchControl;

		[SerializeField]
		private float LaunchControlRpm = 4000f;

		[SerializeField]
		private float LaunchControlTime = 5000f;

		[Header("Shifter")]
		[Tooltip("Optional layout for the manual H-pattern shifter. If null, defaults are used per NumGears.")]
		[SerializeField]
		private ShifterLayoutAsset shifterLayout;

		public ShifterLayoutAsset ShifterLayout => shifterLayout;

		public override string GetDescription
		{
			get
			{
				string text = string.Format("{0}{1}", new LocalizedString("MarketInventoryWindow.GearboxDescriptionHeader"), Environment.NewLine);
				for (int i = 2; i < ratio.Length; i++)
				{
					float num = Utils.RoundTo(0.9f * ratio[i], 3);
					float num2 = Utils.RoundTo(1.1f * ratio[i], 3);
					text += string.Format("{0} {1} : {2}:1 ({3} {4}, {5} {6}){7}", i - 1, new LocalizedString("MarketInventoryWindow.GearboxGear"), ratio[i], new LocalizedString("MarketInventoryWindow.GearboxMin"), num, new LocalizedString("MarketInventoryWindow.GearboxMax"), num2, Environment.NewLine);
				}
				float num3 = Utils.RoundTo(0.85f * end_ratio, 3);
				float num4 = Utils.RoundTo(1.15f * end_ratio, 3);
				return text + string.Format("{0} {1} ({2} {3}, {4} {5})", new LocalizedString("MarketInventoryWindow.GearboxDescriptionFooter"), end_ratio, new LocalizedString("MarketInventoryWindow.GearboxMin"), num3, new LocalizedString("MarketInventoryWindow.GearboxMax"), num4);
			}
		}

		public override string GetName => base.GetName + " (" + GetKindLabel + " " + GetTypeTrans + ")";

		private string GetKindLabel => kind switch
		{
			GearboxKind.MT => "M/T", 
			GearboxKind.AMT => "AMT", 
			GearboxKind.DCT => "DCT", 
			GearboxKind.CVT => "CVT", 
			GearboxKind.Sequential => "SEQ", 
			_ => "A/T", 
		};

		private bool HasLayoutProfile => _partLayoutStamp != 0;

		public TransmissionValues GetParams
		{
			get
			{
				TransmissionValues result = new TransmissionValues
				{
					TorqueSplit = TorqueSplit
				};
				ResolveLayout(out var driveType, out var kindResult);
				result.GearRatio = ratio.Clone() as float[];
				result.FinalDrive = end_ratio;
				result.Type = driveType;
				result.LaunchControl = hasLaunchControl;
				result.LaunchRpm = LaunchControlRpm;
				result.LaunchTime = LaunchControlTime;
				result.Inertia = GetInertia();
				result.Kind = kindResult;
				result.AutoModes = autoModes;
				result.LowMaxGear = lowMaxGear;
				return result;
			}
		}

		private string GetTypeTrans => gearboxType switch
		{
			GearboxType.GearFwd => "FWD", 
			GearboxType.GearRwd => "RWD", 
			GearboxType.GearAwd => "AWD", 
			GearboxType.Gear4Wd => "4WD", 
			GearboxType.Gear4Wdr1 => "4WD", 
			GearboxType.Gear4Wdr2 => "4WD", 
			GearboxType.Gear6Wd => "6WD", 
			GearboxType.Gear6Wdr2 => "6WD", 
			GearboxType.Gear8Wd => "8WD", 
			_ => throw new ArgumentOutOfRangeException(), 
		};

		protected override void OnValidate()
		{
			base.OnValidate();
			adjustableGears = (int)gearboxType;
		}

		private static long GetLayoutKey(int driveType, int kindId, int partId)
		{
			return (long)(((((((ulong)((((((-3750763034362895579L ^ (uint)(driveType * 2246822519u)) * 1099511628211L) ^ (uint)(kindId * 2654435761u)) * 1099511628211L) ^ (uint)(partId * 3266489917u)) * 1099511628211L) ^ ((ulong)(uint)driveType << 23)) * 1099511628211L) ^ ((ulong)(uint)kindId << 41)) * 1099511628211L) ^ 0x7C15D3A29B44E871L) * 1099511628211L);
		}

		private static long GetLayoutStamp(long key)
		{
			return 0x28B7BD766A05068BL ^ key ^ 0x2545F4914F6CDD1DL;
		}

		private void ResolveLayout(out int driveType, out GearboxKind kindResult)
		{
			driveType = adjustableGears;
			kindResult = kind;
			if (!HasLayoutProfile)
			{
				driveType = 512;
				kindResult = GearboxKind.AT;
			}
			else if (GetLayoutStamp(_partLayoutKey) != _partLayoutStamp || GetLayoutKey(driveType, (int)kindResult, ModelId) != _partLayoutKey)
			{
				driveType = 512;
				kindResult = GearboxKind.AT;
			}
		}
	}
}
