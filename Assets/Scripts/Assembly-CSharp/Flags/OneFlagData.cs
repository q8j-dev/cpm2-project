using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Flags
{
	[Serializable]
	public class OneFlagData
	{
		public int index;

		public int order;

		public ObscuredInt price = 1000;

		public Texture texture;
	}
}
