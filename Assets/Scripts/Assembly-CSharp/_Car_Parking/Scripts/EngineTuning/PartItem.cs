using System.Collections.Generic;
using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Localization;

namespace _Car_Parking.Scripts.EngineTuning
{
	public abstract class PartItem : MarketPart
	{
		[Header("ID")]
		public PartType TypeId;

		public int ModelId;

		public string NoticeAboutPart;

		[Range(0.1f, 2000f)]
		public float Mass;

		[Header("Conditions values")]
		[Range(1f, 1000000f)]
		public float MaxMileage = 100f;

		public int VendorId;

		public int Rarity = 1;

		[HideInInspector]
		[SerializeField]
		private List<long> _partLinkKeysA = new List<long>();

		[SerializeField]
		[HideInInspector]
		private List<long> _partLinkKeysB = new List<long>();

		[SerializeField]
		[HideInInspector]
		private List<long> _partLinkKeysC = new List<long>();

		[HideInInspector]
		[SerializeField]
		private long _partLinkStamp;

		[SerializeField]
		[HideInInspector]
		private int _partSourceRef;

		public override string GetDescription
		{
			get
			{
				object[] array = new object[3]
				{
					this.GetEngineName(),
					GetMaxReliability,
					this.GetPartVendorName()
				};
				return string.Format(Utils.CorrectFormatString(new LocalizedString(Description), array.Length), array);
			}
		}

		public int EngineLinkCount => _partLinkKeysA.Count;

		public int CarLinkCount => _partLinkKeysB.Count;

		public int CarExcludeCount => _partLinkKeysC.Count;

		protected override int GetValRef()
		{
			return ModelId;
		}

		private static long GetLinkKey(int id, int listTag, int partId)
		{
			return (long)(((((((ulong)((((-3750763034362895579L ^ (uint)(id * 2654435761u)) * 1099511628211L) ^ (uint)(partId * 2246822519u)) * 1099511628211L) ^ ((ulong)(uint)listTag << 13)) * 1099511628211L) ^ ((ulong)(uint)id << 29)) * 1099511628211L) ^ 0x3D1B58BA6A2F09E5L) * 1099511628211L);
		}

		private static long GetLinkStamp(List<long> a, List<long> b, List<long> c)
		{
			long num = -4132994306676758123L;
			for (int i = 0; i < a.Count; i++)
			{
				num = (num * 31) ^ a[i];
			}
			num ^= a.Count * 2654435761u;
			for (int j = 0; j < b.Count; j++)
			{
				num = (num * 31) ^ b[j];
			}
			num ^= b.Count * 2246822519u;
			for (int k = 0; k < c.Count; k++)
			{
				num = (num * 31) ^ c[k];
			}
			num ^= c.Count * 3266489917u;
			if (num == 0L)
			{
				num = 1L;
			}
			return num;
		}

		public bool IsLinkProfileStale()
		{
			if (_partLinkStamp == 0L)
			{
				return true;
			}
			return GetLinkStamp(_partLinkKeysA, _partLinkKeysB, _partLinkKeysC) != _partLinkStamp;
		}

		public int GetSourceRef()
		{
			return _partSourceRef;
		}

		public bool IsEngineLinked(int engineId)
		{
			long linkKey = GetLinkKey(engineId, 1, ModelId);
			for (int i = 0; i < _partLinkKeysA.Count; i++)
			{
				if (_partLinkKeysA[i] == linkKey)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsCarLinked(int carId)
		{
			long linkKey = GetLinkKey(carId, 2, ModelId);
			for (int i = 0; i < _partLinkKeysB.Count; i++)
			{
				if (_partLinkKeysB[i] == linkKey)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsCarExcluded(int carId)
		{
			long linkKey = GetLinkKey(carId, 3, ModelId);
			for (int i = 0; i < _partLinkKeysC.Count; i++)
			{
				if (_partLinkKeysC[i] == linkKey)
				{
					return true;
				}
			}
			return false;
		}

		protected virtual void OnValidate()
		{
			if (ModelId != 0)
			{
				RecalculateNotice();
			}
		}

		public void RecalculateNotice()
		{
			NoticeAboutPart = CheckModelId();
		}

		private string CheckModelId()
		{
			string text = ModelId.ToString();
			int length = text.Length;
			if (length > 3 && length == 8)
			{
				int num = int.Parse(text.Substring(3, 2));
				int.Parse(text.Substring(5, 3));
				return (num != (int)TypeId) ? "NOT CORRECT MODEL ID" : "Good model id";
			}
			return "Unknown id????";
		}
	}
}
