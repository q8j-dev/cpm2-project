namespace _Car_Parking.Scripts.GameInput.Rebinding
{
	public readonly struct AxisCaptureResult
	{
		public readonly int Index;

		public readonly bool Invert;

		public readonly bool NeedsRemap;

		public AxisCaptureResult(int index, bool invert, bool needsRemap)
		{
			Index = index;
			Invert = invert;
			NeedsRemap = needsRemap;
		}
	}
}
