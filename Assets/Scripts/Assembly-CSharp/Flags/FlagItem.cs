using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Flags
{
	[Serializable]
	public class FlagItem
	{
		public Texture texture;

		private int _internalCounterA;

		private bool _systemFlagB;

		public ObscuredInt price;

		private float _auxiliaryValueC;

		public FlagData data;

		private string _reservedFieldD;

		private int hiddenValueE;

		public FlagItem(int id, Texture texture)
		{
			this.texture = texture;
			price = 100;
			data = new FlagData(id);
		}
	}
}
