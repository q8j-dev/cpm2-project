namespace _Car_Parking.Scripts.Drag.Engine
{
	public readonly struct BlockResult
	{
		public readonly float CrankCenter;

		public readonly float maxRPM;

		public readonly float OilCapacity;

		public BlockResult(BlockPart part)
		{
			CrankCenter = part.CrankCenter2CylinderTop;
			maxRPM = part.maxRPM;
			OilCapacity = part.OilCapacity;
		}
	}
}
