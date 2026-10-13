namespace _Car_Parking.Scripts.GameInput
{
	public static class GamepadEcho
	{
		public const float LatencyWindowSeconds = 0.06f;

		public static bool? Compare(GamepadSignatureHistory xinput, GamepadSignatureHistory playStation)
		{
			if (!xinput.HasAny || !playStation.HasAny)
			{
				return null;
			}
			GamepadSignature signature = xinput.Latest;
			GamepadSignature signature2 = playStation.Latest;
			if (signature.IsActive)
			{
				return playStation.MatchesAnyWithin(in signature, 0.06f);
			}
			if (!signature2.IsActive)
			{
				return null;
			}
			if (xinput.MatchesAnyWithin(in signature2, 0.06f))
			{
				return true;
			}
			if (!playStation.MatchesAnyWithin(in signature, 0.06f))
			{
				return false;
			}
			return null;
		}
	}
}
