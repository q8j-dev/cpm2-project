using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.Drag.Engine
{
	public class ChargerPart : PartItem
	{
		[SerializeField]
		private float pressure;

		[SerializeField]
		private float MinRPM;

		[SerializeField]
		private float MaxRPM;

		[SerializeField]
		private TurboType ChargerType;

		[SerializeField]
		private ObscuredInt Pressure;

		public override string GetMaxReliability
		{
			get
			{
				ObscuredInt obscuredInt = Pressure;
				obscuredInt.RandomizeCryptoKey();
				return string.Format("{0:f2} {1}", (float)(int)obscuredInt / 100000f, new LocalizedString("Utils.bar"));
			}
		}

		public ObscuredInt GetPressure
		{
			get
			{
				ObscuredInt result = Pressure;
				result.RandomizeCryptoKey();
				return result;
			}
		}

		public float GetMin => MinRPM;

		public float GetMax => MaxRPM;

		public ObscuredInt GetMaxPressure => Utils.Bar2Pa(base.GetReliability);

		public TurboType GetChargerType => ChargerType;
	}
}
