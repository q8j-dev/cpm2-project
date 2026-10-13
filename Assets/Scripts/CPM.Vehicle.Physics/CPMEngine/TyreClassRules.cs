namespace CPMEngine
{
	public static class TyreClassRules
	{
		public static bool IsTyreAllowed(TyreType type, int carId)
		{
			if (BuggyTyreRules.IsTyreAllowed(type, carId))
			{
				return F1TyreRules.IsTyreAllowed(type, carId);
			}
			return false;
		}

		public static TyreType Sanitize(TyreType type, int carId)
		{
			if (IsTyreAllowed(type, carId))
			{
				return type;
			}
			if (!F1TyreRules.IsF1(carId))
			{
				return BuggyTyreRules.Sanitize(type, carId);
			}
			return F1TyreRules.Sanitize(type, carId);
		}
	}
}
