using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Car_Parking.Scripts.Audio.Dummy
{
	[Serializable]
	public abstract class BaseTypeDummy
	{
		[HideInInspector]
		[SerializeField]
		[FormerlySerializedAs("ID")]
		private int idInternal;

		[HideInInspector]
		[SerializeField]
		[FormerlySerializedAs("valueGuid")]
		private byte[] valueGuidInternal;

		public abstract object ObjectReference { get; set; }

		public abstract object WwiseObjectType { get; }

		public virtual string Name => string.Empty;

		public uint Id => 0u;

		public static uint InvalidId => 0u;

		public int ID => (int)Id;

		public byte[] valueGuid => new byte[0];

		public static int CombineHashCodes(int[] hashCodes)
		{
			return 0;
		}

		public virtual bool IsValid()
		{
			return false;
		}

		public bool Validate()
		{
			return false;
		}

		protected void Verify(object result)
		{
		}

		public override string ToString()
		{
			return "Empty";
		}

		public override int GetHashCode()
		{
			return CombineHashCodes(new int[4]
			{
				ObjectReference.GetHashCode(),
				WwiseObjectType.GetHashCode(),
				Name.GetHashCode(),
				Id.GetHashCode()
			});
		}
	}
}
