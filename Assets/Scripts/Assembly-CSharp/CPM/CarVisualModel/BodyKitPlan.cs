using System.Collections.Generic;
using CPM.Database;
using _Car_Parking.Scripts.Drag;

namespace CPM.CarVisualModel
{
	public static class BodyKitPlan
	{
		private const int NotSetSentinel = -2;

		public static List<BodyKitRequest> BuildLoadList(InstalledBodyKits kits)
		{
			List<BodyKitRequest> list = new List<BodyKitRequest>();
			if (kits == null)
			{
				return list;
			}
			AddSimpleStockOnNegative(list, BodyKitType.Hood, kits.HoodId);
			AddSimpleStockOnNegative(list, BodyKitType.FrontBumper, kits.FrontBumperId);
			AddSimpleStockOnNegative(list, BodyKitType.RearBumper, kits.RearBumperId);
			AddSimpleRemovable(list, BodyKitType.Skirt, kits.SkirtId);
			AddSimpleRemovable(list, BodyKitType.Fender, kits.FenderId);
			AddComplex(list, BodyKitType.Spoiler, kits.SpoilerId);
			AddComplex(list, BodyKitType.AirIntakeHood, kits.HoodAirIntakeId);
			AddComplex(list, BodyKitType.AirIntakeRoof, kits.RoofAirIntakeId);
			if (kits.Trims != null)
			{
				for (int i = 0; i < kits.Trims.Length; i++)
				{
					AddComplex(list, BodyKitType.Trim, kits.Trims[i]);
				}
			}
			return list;
		}

		public static bool TryGetMount(BodyKitMount[] mounts, BodyKitType type, int id, out BodyKitMount mount)
		{
			if (mounts != null)
			{
				for (int i = 0; i < mounts.Length; i++)
				{
					if (mounts[i].Type == type && mounts[i].Id == id)
					{
						mount = mounts[i];
						return true;
					}
				}
			}
			mount = default;
			return false;
		}

		private static void AddSimpleStockOnNegative(List<BodyKitRequest> list, BodyKitType type, int id)
		{
			if (id < 0)
			{
				id = 0;
			}
			list.Add(new BodyKitRequest(type, id, BodyKitClass.Simple));
		}

		private static void AddSimpleRemovable(List<BodyKitRequest> list, BodyKitType type, int id)
		{
			if (id == -2)
			{
				id = 0;
			}
			if (id >= 0)
			{
				list.Add(new BodyKitRequest(type, id, BodyKitClass.Simple));
			}
		}

		private static void AddComplex(List<BodyKitRequest> list, BodyKitType type, int id)
		{
			if (id > 0)
			{
				list.Add(new BodyKitRequest(type, id, BodyKitClass.Complex));
			}
		}
	}
}
