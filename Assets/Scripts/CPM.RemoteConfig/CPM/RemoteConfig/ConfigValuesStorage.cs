using System;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.RemoteConfig;

namespace CPM.RemoteConfig
{
	[Serializable]
	[Preserve]
	public class ConfigValuesStorage
	{
		public int needToUpdateGame;

		public bool needHoldRootDevices;

		public int photonGameVersion;

		public int maxPlayers;

		public int multiPlayerStatus;

		public int worldSaleStatus;

		public int adsCount;

		public bool autoSaveFromMainMenu;

		public bool autoSaveFromMainMenuOneTime;

		public bool autoSaveFromMainOnlinePlayerMenu;

		public bool autoSaveAllCarsAfterGetAllCars;

		public bool autoSaveCarAfterBuy;

		public bool saveCarsInAutoSave;

		public float delayInAutoSaveInSec;

		public int blockAppStatus;

		public int blockAppStatus2;

		public string messageFromDevelopers;

		public string nfsCarIds = "[]";

		public string nfsCarInstances = "";

		public string banCarsIds = "[]";

		public float unsafeImagesThreshold = 0.5f;

		public int textureSize = 224;

		public int modelVersion;

		public int parentalControlStatus;

		public bool appCheckLogs;

		public int raceRewardCoinsLimit;

		public int raceRewardMoneyLimit;

		public bool legacyRating;

		public AnalyticsRemoteConfig analytics = new AnalyticsRemoteConfig();

		public MasterMigrationRemoteConfig masterMigration = new MasterMigrationRemoteConfig();

		public int coinsDiscount;

		public int funcRegion = 1;

		public bool legacyGarage;

		public bool multiplayerCarLegitCheckEnabled;

		public bool useNewRatingBackend;

		public int adsAudienceTarget;
	}
}
