namespace _Car_Parking.Scripts.WorldInteraction
{
	public static class PoseAudienceRules
	{
		public static bool Matches(PoseAudience audience, int gender)
		{
			return audience switch
			{
				PoseAudience.Male => gender == 1, 
				PoseAudience.Female => gender == 0, 
				_ => true, 
			};
		}
	}
}
