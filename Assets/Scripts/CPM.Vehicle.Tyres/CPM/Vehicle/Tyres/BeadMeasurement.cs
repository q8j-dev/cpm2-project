namespace CPM.Vehicle.Tyres
{
	public readonly struct BeadMeasurement
	{
		public readonly float Diameter;

		public readonly float Width;

		public BeadMeasurement(float diameter, float width)
		{
			Diameter = diameter;
			Width = width;
		}
	}
}
