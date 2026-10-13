namespace _Car_Parking.Scripts.Free_drive
{
	public static class SceneTypeExtensions
	{
		public static SceneType FromPlaceToSpawn(V.placeToSpawn place)
		{
			return place switch
			{
				V.placeToSpawn.MainCity => SceneType.MainCity, 
				V.placeToSpawn.Highway => SceneType.Highway2, 
				V.placeToSpawn.Desert => SceneType.Desert, 
				V.placeToSpawn.Mountain => SceneType.Mountain, 
				V.placeToSpawn.Racing => SceneType.RaceTrack, 
				V.placeToSpawn.City2 => SceneType.City2, 
				V.placeToSpawn.Offroad => SceneType.OffRoad, 
				V.placeToSpawn.JapanSerpentine => SceneType.Japan, 
				V.placeToSpawn.JapanSuburban => SceneType.JapanSuburban, 
				V.placeToSpawn.Island => SceneType.Island, 
				V.placeToSpawn.TropicalIsland => SceneType.TropicalIsland, 
				V.placeToSpawn.Nurburgring => SceneType.Nurburgring, 
				V.placeToSpawn.Italy => SceneType.Italy, 
				V.placeToSpawn.StraightRoad => SceneType.StraightRoad, 
				_ => SceneType.None, 
			};
		}

		public static int ToBuildIndex(this SceneType sceneType)
		{
			return sceneType switch
			{
				SceneType.MainCity => 14, 
				SceneType.Highway2 => 27, 
				SceneType.Desert => 15, 
				SceneType.Mountain => 16, 
				SceneType.RaceTrack => 23, 
				SceneType.City2 => 26, 
				SceneType.Island => 17, 
				SceneType.Road1 => 19, 
				SceneType.Road2 => 18, 
				SceneType.StraightRoad => 24, 
				SceneType.Home2 => 28, 
				SceneType.OffRoad => 29, 
				SceneType.Richmond => 31, 
				SceneType.Japan => 35, 
				SceneType.JapanSuburban => 36, 
				SceneType.RichIsland => 37, 
				SceneType.CityMainRaceTrack => 38, 
				SceneType.TropicalIsland => 43, 
				SceneType.Nurburgring => 44, 
				SceneType.JunctionNurburgring => 45, 
				SceneType.RichIslandRoad => 46, 
				SceneType.Italy => 47, 
				_ => -1, 
			};
		}
	}
}
