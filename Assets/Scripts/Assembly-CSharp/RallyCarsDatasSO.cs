using System.Linq;
using CPM.Database;
using UnityEngine;

[CreateAssetMenu(menuName = "Rally/RallyCarsDatasSO")]
public class RallyCarsDatasSO : ScriptableObject
{
	public RallyCarData[] RallyCarsDatas;

	public RallyCarData DefaultRallyCarData;

	public RallyCarData GetRallyCarData(int locationId, int trajectoryId, AllCarsInfo allCarsInfo)
	{
		if (RallyCarsDatas == null || RallyCarsDatas.Length == 0)
		{
			return null;
		}
		RallyCarData rallyCarData = RallyCarsDatas.FirstOrDefault((RallyCarData x) => x.LocationId == locationId && x.TrajectoryId == trajectoryId);
		if (IsValid(rallyCarData))
		{
			return rallyCarData;
		}
		if (IsValid(DefaultRallyCarData))
		{
			return DefaultRallyCarData;
		}
		return RallyCarsDatas.FirstOrDefault(IsValid);
		bool IsValid(RallyCarData d)
		{
			if (d != null && d.MainCarValues != null)
			{
				if (!(allCarsInfo == null))
				{
					return !allCarsInfo.IsBannedCar(d.MainCarValues.CarID);
				}
				return true;
			}
			return false;
		}
	}
}
