using System;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.EngineTuning.Instances;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public class ChargerInstance : PartInstance<ChargerPart>
	{
		private ObscuredInt _factoryPressure;

		private ObscuredInt _stamina;

		private readonly float _minRange;

		private readonly float _maxRange;

		private readonly TurboType _type;

		private readonly int _size;

		public AdvancedInt GetActualPressure
		{
			get
			{
				ObscuredInt factoryPressure = _factoryPressure;
				factoryPressure.RandomizeCryptoKey();
				return new AdvancedInt(Mathf.RoundToInt((float)(int)factoryPressure * GetActualCondition));
			}
		}

		public float GetActualCondition => GetEfficiency(base.Condition, 0.7f);

		public AdvancedInt GetPressure
		{
			get
			{
				ObscuredInt factoryPressure = _factoryPressure;
				factoryPressure.RandomizeCryptoKey();
				return new AdvancedInt(factoryPressure);
			}
		}

		public AdvancedInt GetMaxSize
		{
			get
			{
				ObscuredInt stamina = _stamina;
				stamina.RandomizeCryptoKey();
				return new AdvancedInt(Mathf.RoundToInt((float)(int)stamina * 1.15f));
			}
		}

		public AdvancedInt GetMaxPressure
		{
			get
			{
				ObscuredInt stamina = _stamina;
				stamina.RandomizeCryptoKey();
				return new AdvancedInt(stamina);
			}
		}

		public float GetMinRange => _minRange;

		public float GetMaxRange => _maxRange;

		public TurboType GetTurboType => _type;

		public ChargerInstance(ChargerPart part)
		{
			Part = part;
			_factoryPressure = part.GetPressure;
			_minRange = part.GetMin;
			_maxRange = part.GetMax;
			_type = part.GetChargerType;
			_stamina = part.GetMaxPressure;
		}

		public override void UpdateWear(Pv value)
		{
			if (!(Mileage > Part.MaxMileage))
			{
				float num = value.D;
				GetActualPressure.Get(out int value2);
				float num2 = (((int)_stamina > 0) ? (num / (float)(int)_stamina) : 0f);
				float num3 = ((value2 > 0) ? (num / (float)value2) : 0f);
				float num4 = 1f;
				if (num3 > 1f)
				{
					num4 += 250f * num3;
				}
				if (num2 > 1f)
				{
					num4 = 1000f * num2;
				}
				Mileage += value.P * num4;
			}
		}

		public override void UpdateLoad(Pv value)
		{
			float num = value.D;
			GetMaxSize.Get(out int value2);
			if (num > (float)value2 || Utils.Pa2Bar(value2) > 3.6f)
			{
				Mileage += Part.MaxMileage;
			}
		}
	}
}
