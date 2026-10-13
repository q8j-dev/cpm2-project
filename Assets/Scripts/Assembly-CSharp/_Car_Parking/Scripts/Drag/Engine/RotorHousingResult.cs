namespace _Car_Parking.Scripts.Drag.Engine
{
	public readonly struct RotorHousingResult
	{
		public readonly int Rotors;

		public readonly float GeneratingRadius;

		public readonly float HousingWidth;

		public readonly float IntakeOpenDuration;

		public readonly float IntakeCloseDuration;

		public readonly float IntakeDuration;

		public readonly float ExhaustOpenDuration;

		public readonly float ExhaustCloseDuration;

		public readonly float ExhaustDuration;

		public readonly float IntakePortDiameter;

		public readonly float ExhaustPortDiameter;

		public readonly RotorHousingPart.PortType IntakePortType;

		public readonly RotorHousingPart.PortType ExhaustPortType;

		public readonly float MaxRPM;

		public readonly float OilCapacity;

		public RotorHousingResult(RotorHousingPart part)
		{
			Rotors = part.Rotors;
			GeneratingRadius = part.GeneratingRadius;
			HousingWidth = part.HousingWidth;
			IntakeOpenDuration = part.IntakeOpenDuration;
			IntakeCloseDuration = part.IntakeCloseDuration;
			IntakeDuration = part.GetIntakeDuration();
			ExhaustOpenDuration = part.ExhaustOpenDuration;
			ExhaustCloseDuration = part.ExhaustCloseDuration;
			ExhaustDuration = part.GetExhaustDuration();
			IntakePortDiameter = part.IntakePortDiameter;
			ExhaustPortDiameter = part.ExhaustPortDiameter;
			IntakePortType = part.IntakePortType;
			ExhaustPortType = part.ExhaustPortType;
			MaxRPM = part.maxRPM;
			OilCapacity = part.OilCapacity;
		}
	}
}
