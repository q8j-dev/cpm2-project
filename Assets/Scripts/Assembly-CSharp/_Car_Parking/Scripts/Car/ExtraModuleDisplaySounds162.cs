namespace _Car_Parking.Scripts.Car
{
	public class ExtraModuleDisplaySounds162 : ExtraModuleDisplaySounds
	{
		protected override float DelaySlideOpenDashboard => -1f;

		protected override float DelayFoldOpenDashboard => 0.45f;

		protected override float DelaySlideCloseDashboard => 0.3f;

		protected override float DelayFoldCloseDashboard => 0.3f;

		public ExtraModuleDisplaySounds162(VehicleData vehicleData)
			: base(vehicleData)
		{
		}
	}
}
