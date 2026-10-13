namespace CarInteractionAnimationsScripts
{
	public class CarSoundCondition
	{
		public enum Type
		{
			None = 0,
			HaveBodyKit = 1
		}

		public virtual bool CanPlaySound(VehicleData vehicleData)
		{
			return true;
		}
	}
}
