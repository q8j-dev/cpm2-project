using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace _Car_Parking.Scripts.Other.Helpers
{
	public class ScenesHelper
	{
		public const int LoadingSceneIndex = 0;

		public const int MainMenuSceneIndex = 1;

		public const int LevelsVillage2SceneIndex = 3;

		public const int TiyCar5SceneIndex = 6;

		public const int Highway6SceneIndex = 7;

		public const int DesertLevel10SceneIndex = 11;

		public const int FreeDriveIndex = 13;

		public const int CityMainSceneIndex = 14;

		public const int DesertSceneIndex = 15;

		public const int MountainsSceneIndex = 16;

		public const int IslandSceneIndex = 17;

		public const int Road2SceneIndex = 18;

		public const int Road1SceneIndex = 19;

		public const int RaceTrackSceneIndex = 23;

		public const int StraightRoadSceneIndex = 24;

		public const int ParkingSceneIndex = 25;

		public const int City2SceneIndex = 26;

		public const int HighwaySceneIndex = 27;

		public const int Home2SceneIndex = 28;

		public const int OffRoadSceneIndex = 29;

		public const int ApartmentSceneIndex = 30;

		public const int RichmondSceneIndex = 31;

		public const int RacingSceneIndex = 32;

		public const int HangarGarageSceneIndex = 33;

		public const int LoadingSceneSecondIndex = 34;

		public const int JapanSerpantineSceneIndex = 35;

		public const int JapanSuburbanSceneIndex = 36;

		public const int RichIslandSceneIndex = 37;

		public const int RaceTrackCityMainSceneIndex = 38;

		public const int NotificationsSceneIndex = 39;

		public const int RacingFree = 41;

		public const int DesertGarageSceneIndex = 42;

		public const int TropicalIslandSceneIndex = 43;

		public const int NurburgringIndex = 44;

		public const int JunctionNurburgringIndex = 45;

		public const int RichIslandRoadSceneIndex = 46;

		public const int ItalySceneIndex = 47;

		public const int TransitionSceneIndex = 48;

		public static bool IsLoadingSceneActive()
		{
			if (!IsSceneActive(0))
			{
				return IsSceneActive(34);
			}
			return true;
		}

		public static bool IsParkingLevelActive()
		{
			int buildIndex = SceneManager.GetActiveScene().buildIndex;
			if (buildIndex != 1 && buildIndex != 13 && buildIndex != 32 && buildIndex != 33 && buildIndex != 41)
			{
				return buildIndex != 42;
			}
			return false;
		}

		public static bool IsLapRacingSceneActive()
		{
			return SceneManager.GetActiveScene().buildIndex == 41;
		}

		public static bool IsDragRacingSceneActive()
		{
			return SceneManager.GetActiveScene().buildIndex == 32;
		}

		public static bool IsFreeDriveSceneActive()
		{
			return SceneManager.GetActiveScene().buildIndex == 13;
		}

		public static bool IsMenuSceneActive()
		{
			int buildIndex = SceneManager.GetActiveScene().buildIndex;
			if (buildIndex != 1 && buildIndex != 33)
			{
				return buildIndex == 42;
			}
			return true;
		}

		public static bool IsNotificationActive()
		{
			return IsSceneActive(39);
		}

		public static bool GarageIsActive()
		{
			if (!IsSceneActive(33))
			{
				return IsSceneActive(42);
			}
			return true;
		}

		public static bool IsSceneActive(int sceneIndex)
		{
			for (int i = 0; i < SceneManager.sceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.buildIndex == sceneIndex && sceneAt.isLoaded)
				{
					return true;
				}
			}
			return false;
		}

		public static int GetFreeFromGarageIndex(int garageIndex)
		{
			return garageIndex switch
			{
				33 => 14, 
				42 => 15, 
				_ => -1, 
			};
		}

		public static Scene[] GetAllLoadedScenes()
		{
			int loadedSceneCount = SceneManager.loadedSceneCount;
			Scene[] array = new Scene[loadedSceneCount];
			for (int i = 0; i < loadedSceneCount; i++)
			{
				array[i] = SceneManager.GetSceneAt(i);
			}
			return array;
		}

		public static bool AllScenesLoaded(List<int> sceneIndexes)
		{
			foreach (int sceneIndex in sceneIndexes)
			{
				if (!TryGetLoadedScene(sceneIndex, out var _))
				{
					return false;
				}
			}
			return true;
		}

		public static bool TryGetLoadedScene(int sceneIndex, out Scene scene)
		{
			int loadedSceneCount = SceneManager.loadedSceneCount;
			for (int i = 0; i < loadedSceneCount; i++)
			{
				Scene sceneAt = SceneManager.GetSceneAt(i);
				if (sceneAt.buildIndex == sceneIndex)
				{
					scene = sceneAt;
					return true;
				}
			}
			scene = default;
			return false;
		}

		public static void LoadNotificationScene(SceneNotificationReason reason)
		{
			SceneNotificationContext.Reason = reason;
			SceneManager.LoadScene(39);
		}
	}
}
