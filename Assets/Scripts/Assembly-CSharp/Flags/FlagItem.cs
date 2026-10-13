using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Flags
{
	[Serializable]
	public class FlagItem
	{
		public Texture texture;

		public ObscuredInt price;

		public FlagData data;

		public FlagItem(int id, Texture texture)
		{
			this.texture = texture;
			price = 100;
			data = new FlagData(id);
		}
	}
}
