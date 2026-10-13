using System;
using UnityEngine;

namespace Gley.TrafficSystem.Internal
{
	[Serializable]
	public struct IntersectionDataType
	{
		[SerializeField]
		private IntersectionType _type;

		[SerializeField]
		private int _index;

		[SerializeField]
		private string _name;

		public readonly IntersectionType Type => _type;

		public readonly int OtherListIndex => _index;

		public IntersectionDataType(IntersectionType type, int index, string name)
		{
			_type = type;
			_index = index;
			_name = name;
		}
	}
}
