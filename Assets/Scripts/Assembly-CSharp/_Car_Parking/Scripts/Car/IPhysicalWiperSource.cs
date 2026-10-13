namespace _Car_Parking.Scripts.Car
{
	public interface IPhysicalWiperSource
	{
		CarBodyReferences.WiperBlade[] WiperBlades { get; set; }

		bool HasPhysicalWipers { get; }

		void ApplyWiperPose(float value);
	}
}
