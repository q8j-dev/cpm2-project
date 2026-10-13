namespace _Car_Parking.Scripts.Drag
{
	public static class BodyKitForcedDefaults
	{
		public static bool ShouldForceNonDefault(BodyKitType type)
		{
			if (type != BodyKitType.FrontBumper && type != BodyKitType.RearBumper && type != BodyKitType.Skirt && type != BodyKitType.Hood && type != BodyKitType.Fender)
			{
				return type == BodyKitType.Trim;
			}
			return true;
		}
	}
}
