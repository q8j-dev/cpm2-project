using System;
using UnityEngine;

namespace GPS
{
	[Serializable]
	public struct NodePosition
	{
		public short x;

		public short y;

		public short z;

		public NodePosition(Vector3 v)
		{
			x = (short)v.x;
			y = (short)v.y;
			z = (short)v.z;
		}

		public static implicit operator Vector3(NodePosition nodePosition)
		{
			return new Vector3(nodePosition.x, nodePosition.y, nodePosition.z);
		}
	}
}
