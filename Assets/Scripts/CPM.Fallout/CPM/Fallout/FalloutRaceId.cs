using _Car_Parking.Scripts.Backend;

namespace CPM.Fallout
{
	public static class FalloutRaceId
	{
		public static Result<string> Build(int activityType, string raceUid)
		{
			if (string.IsNullOrEmpty(raceUid))
			{
				return Result<string>.Fail();
			}
			return Result<string>.Success($"{activityType}_{raceUid}");
		}
	}
}
