using System;
using CPMEngine;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace _Car_Parking.Scripts.EngineTuning
{
	public abstract class MarketPart : ScriptableObject
	{
		public bool IsNotForSale;

		[SerializeField]
		[Header("Sell properties")]
		private string Name;

		[TextArea]
		public string Description;

		[HideInInspector]
		[SerializeField]
		private long _partValA;

		[SerializeField]
		[HideInInspector]
		private long _partValB;

		[SerializeField]
		[HideInInspector]
		private long _partValC;

		[HideInInspector]
		[SerializeField]
		private long _partValD;

		public virtual string GetName => Name;

		public virtual string GetDescription => Description;

		public virtual string GetMaxReliability
		{
			get
			{
				if (!(GetReliability < 0f))
				{
					return Utils.GetTorque(GetReliability);
				}
				return "∞";
			}
		}

		public float GetReliability => ResolveRel();

		public ObscuredInt GetPrice
		{
			get
			{
				ObscuredInt result = (int)ResolveVal();
				result.RandomizeCryptoKey();
				return result;
			}
		}

		public CostType GetPriceType => (CostType)(byte)(ResolveVal() >> 32);

		protected virtual int GetValRef()
		{
			return 0;
		}

		private ulong ResolveVal()
		{
			if (_partValB == 0L)
			{
				return Pack(536870911, CostType.Money);
			}
			ulong num = (ulong)_partValA ^ ValMask(GetValRef());
			if (_partValB != ValSign(num, GetValRef()))
			{
				return Pack(536870911, CostType.Money);
			}
			return num;
		}

		private static ulong Pack(int price, CostType type)
		{
			return (uint)price | ((ulong)(byte)type << 32);
		}

		private float ResolveRel()
		{
			if (_partValD == 0L)
			{
				return 1f;
			}
			int num = (int)(_partValC ^ (long)RelMask(GetValRef()));
			if (_partValD != ValSign((uint)num, GetValRef()))
			{
				return 1f;
			}
			return BitConverter.Int32BitsToSingle(num);
		}

		private static ulong ValMask(int refId)
		{
			return ((ulong)((-7046029254386353131L ^ (uint)(refId * 2654435761u)) * 1099511628211L) ^ ((ulong)(uint)refId << 27)) * 14029467366897019727uL;
		}

		private static ulong RelMask(int refId)
		{
			return ((ulong)((-2960836687051489901L ^ (uint)(refId * 3266489917u)) * 1099511628211L) ^ ((ulong)(uint)refId << 19)) * 11400714819323198485uL;
		}

		private static long ValSign(ulong value, int refId)
		{
			long num = ((((((-3750763034362895579L ^ (uint)value) * 1099511628211L) ^ (uint)(value >> 32)) * 1099511628211L) ^ (uint)(refId * 2246822519u)) * 1099511628211L) ^ 0x5851F42D4C957F2DL;
			if (num != 0L)
			{
				return num;
			}
			return 1L;
		}
	}
}
