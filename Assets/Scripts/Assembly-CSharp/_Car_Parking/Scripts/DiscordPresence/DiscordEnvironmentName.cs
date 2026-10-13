using _Car_Parking.Scripts.Free_drive;

namespace _Car_Parking.Scripts.DiscordPresence
{
	public static class DiscordEnvironmentName
	{
		public static string Current()
		{
			SceneType sceneType = SceneTypeExtensions.FromPlaceToSpawn(V.PlaceToSpawn);
			if (sceneType != SceneType.None)
			{
				return sceneType.GetEnumMemberValue();
			}
			return string.Empty;
		}
	}
}
