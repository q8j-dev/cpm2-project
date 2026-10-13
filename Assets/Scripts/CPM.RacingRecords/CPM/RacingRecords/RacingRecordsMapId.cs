using _Car_Parking.Scripts.Backend;

namespace CPM.RacingRecords
{
	public static class RacingRecordsMapId
	{
		public static Result<string> Build(string locationKey, int trajectoryIndex)
		{
			if (string.IsNullOrEmpty(locationKey))
			{
				return Result<string>.Fail();
			}
			return Result<string>.Success($"{locationKey}_{trajectoryIndex}");
		}
	}
}
