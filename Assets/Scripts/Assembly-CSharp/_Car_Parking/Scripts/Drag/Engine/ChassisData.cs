using System;
using CPMEngine;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag.Engine
{
	[Serializable]
	public struct ChassisData
	{
		[HideInInspector]
		[SerializeField]
		private long[] _chassisKeys;

		[SerializeField]
		[HideInInspector]
		private long _chassisStamp;

		private const int SlotCount = 15;

		private bool ProfileValid
		{
			get
			{
				if (_chassisStamp != 0L)
				{
					long[] chassisKeys = _chassisKeys;
					if (chassisKeys != null && chassisKeys.Length == 15)
					{
						return ComputeStamp(_chassisKeys) == _chassisStamp;
					}
				}
				return false;
			}
		}

		public float Cx => ReadFloat(0, 2f);

		public float FrontDownforce => ReadFloat(1, 0f);

		public float RearDownforce => ReadFloat(2, 0f);

		public float Length => ReadFloat(3, 4f);

		public float Width => ReadFloat(4, 1.8f);

		public float Height => ReadFloat(5, 1.5f);

		public int FuelTankCapacity => ReadInt(6, 0);

		public float BrakeBalance => ReadFloat(7, 0.5f);

		public float Weight => ReadFloat(8, 100000f);

		public float WeightDistribution => ReadFloat(9, 0.5f);

		public float WeightDistributionVertical => ReadFloat(10, 0.5f);

		public EngineType GetEngineType => (EngineType)ReadEngineTypeRaw();

		public GearboxType GetGearboxType => (GearboxType)ReadGearboxTypeRaw();

		public EngineOrientation Orientation => (EngineOrientation)ReadInt(13, 0);

		public string GetTypeTrans => ReadGearboxTypeRaw() switch
		{
			0 => "N/A", 
			1 => "RWD", 
			2 => "FWD", 
			3 => "FWD, RWD", 
			4 => "AWD", 
			5 => "RWD, AWD", 
			6 => "FWD, AWD", 
			7 => "FWD, RWD, AWD", 
			8 => "4WD", 
			9 => "RWD, 4WD", 
			10 => "FWD, 4WD", 
			11 => "FWD, RWD, 4WD", 
			12 => "AWD, 4WD", 
			13 => "RWD, AWD, 4WD", 
			14 => "FWD, AWD, 4WD", 
			15 => "FWD, RWD, AWD, 4WD", 
			_ => "N/A", 
		};

		private static ulong SlotMask(int idx)
		{
			return (ulong)((((-3750763034362895579L ^ (uint)((idx + 11) * 2654435761u)) * 1099511628211L) ^ 0x6E2A4C8F13B759D1L) * 1099511628211L);
		}

		private static long PackSlot(uint bits, int idx)
		{
			return (long)(bits ^ SlotMask(idx));
		}

		private static uint UnpackSlot(long stored, int idx)
		{
			return (uint)((ulong)stored ^ SlotMask(idx));
		}

		private static long ComputeStamp(long[] keys)
		{
			long num = -2960836687051489901L;
			for (int i = 0; i < keys.Length; i++)
			{
				num = (num * 31) ^ keys[i];
			}
			num ^= keys.Length * 2654435761u;
			if (num == 0L)
			{
				num = 1L;
			}
			return num;
		}

		private float ReadFloat(int idx, float fallback)
		{
			if (!ProfileValid)
			{
				return fallback;
			}
			return BitConverter.Int32BitsToSingle((int)UnpackSlot(_chassisKeys[idx], idx));
		}

		private int ReadInt(int idx, int fallback)
		{
			if (!ProfileValid)
			{
				return fallback;
			}
			return (int)UnpackSlot(_chassisKeys[idx], idx);
		}

		private int ReadEngineTypeRaw()
		{
			return ReadInt(11, 0);
		}

		private int ReadGearboxTypeRaw()
		{
			return ReadInt(12, 0);
		}

		public bool MatchesRef(int ownerId)
		{
			return ReadInt(14, -1) == ownerId;
		}
	}
}
