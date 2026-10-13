using System;

namespace _Car_Parking.Scripts.Car
{
	public abstract class VehicleDataExtraModule : IDisposable
	{
		protected VehicleData vehicleData;

		public VehicleDataExtraModule(VehicleData vehicleData)
		{
			this.vehicleData = vehicleData;
		}

		public abstract void Dispose();

		public virtual void EnableCarDisplay(bool isEngineOn)
		{
		}
	}
}
