namespace Service.Taxi
{
	public static class ClientStateTransitions
	{
		public static bool IsAllowed(CLIENT_STATE current, CLIENT_STATE next)
		{
			if (current == CLIENT_STATE.SIT_IN)
			{
				if (next != CLIENT_STATE.GO_TO_CAR)
				{
					return next != CLIENT_STATE.WAIT;
				}
				return false;
			}
			return true;
		}
	}
}
