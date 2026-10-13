using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Car_Parking.Scripts.Audio.Dummy
{
	[Serializable]
	public abstract class BaseGroupTypeDummy : BaseTypeDummy
	{
		public object GroupWwiseObjectReference => null;

		public abstract object WwiseObjectGroupType { get; }

		public uint GroupId => 0u;

		public int groupID => (int)GroupId;

		public byte[] groupGuid => new byte[0];

		public override bool IsValid()
		{
			return false;
		}
	}
}
