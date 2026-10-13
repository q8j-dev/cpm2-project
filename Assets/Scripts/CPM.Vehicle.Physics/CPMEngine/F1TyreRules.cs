using System.Collections.Generic;

namespace CPMEngine
{
	public static class F1TyreRules
	{
		public const int F1Id = 155;

		public const int SuperFormulaId = 165;

		public static readonly IReadOnlyList<int> F1CarIds = new int[2] { 155, 165 };

		private static readonly HashSet<int> Ids = new HashSet<int> { 155, 165 };

		public static bool IsF1(int carId)
		{
			return Ids.Contains(carId);
		}

		public static bool IsTyreAllowed(TyreType type, int carId)
		{
			if (IsF1(carId))
			{
				if (type != TyreType.Slick && type != TyreType.SoftSlick)
				{
					return type == TyreType.HardSlick;
				}
				return true;
			}
			return true;
		}

		public static TyreType Sanitize(TyreType type, int carId)
		{
			if (!IsTyreAllowed(type, carId))
			{
				return TyreType.Slick;
			}
			return type;
		}
	}
}
