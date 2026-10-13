using System;
using CodeStage.AntiCheat.ObscuredTypes;
using _Car_Parking.Scripts.Drag;

namespace CPMEngine
{
	[Serializable]
	public struct PaidExteriorPart
	{
		public BodyKitType PartType;

		public ObscuredInt Id;

		public ObscuredInt Cost;
	}
}
