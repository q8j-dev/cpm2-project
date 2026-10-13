using System.Collections.Generic;

namespace CPMEngine
{
	public static class BuggyTyreRules
	{
		public const int ArielNomadId = 164;

		public const int CanAmMaverickId = 274;

		public const int PolarisRzrId = 276;

		private static readonly HashSet<int> BuggyCarIds = new HashSet<int> { 164, 274, 276 };

		public static bool IsBuggy(int carId)
		{
			return BuggyCarIds.Contains(carId);
		}

		public static bool IsTyreAllowed(TyreType type, int carId)
		{
			if (!IsBuggy(carId))
			{
				return type != TyreType.Buggy;
			}
			if (type != TyreType.Buggy)
			{
				return type == TyreType.Offroad;
			}
			return true;
		}

		public static TyreType Sanitize(TyreType type, int carId)
		{
			if (!IsTyreAllowed(type, carId))
			{
				if (!IsBuggy(carId))
				{
					return TyreType.Offroad;
				}
				return TyreType.Buggy;
			}
			return type;
		}
	}
}
