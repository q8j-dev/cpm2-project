using CPMEngine;
using _Car_Parking.Scripts.Car.Sounds;

namespace DataEntities.Car
{
	public class BoughtCarData
	{
		public string PackedData { get; private set; }

		public VehicleData VehicleData { get; private set; }

		public Powertrain Powertrain { get; private set; }

		public Axles Axles { get; private set; }

		public MultiPlayerPosControl Mpcc { get; private set; }

		public SoundController SoundController { get; private set; }

		public BodyKits BodyKits { get; private set; }

		public BoughtCarData(string packedData, VehicleData vehicleData, Powertrain powertrain, Axles axles, MultiPlayerPosControl mpcc, SoundController soundController, BodyKits bodyKits)
		{
			PackedData = packedData;
			VehicleData = vehicleData;
			Powertrain = powertrain;
			Axles = axles;
			Mpcc = mpcc;
			SoundController = soundController;
			BodyKits = bodyKits;
		}
	}
}
