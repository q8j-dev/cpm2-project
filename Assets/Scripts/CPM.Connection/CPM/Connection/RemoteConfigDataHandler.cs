using CPM.RemoteConfig;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using UnityEngine.Scripting;
using _Car_Parking.Scripts.CPMWorldSale;

namespace CPM.Connection
{
	[Preserve]
	public class RemoteConfigDataHandler
	{
		private IWorldSaleActivityDependant worldSaleActivityDependant;

		public int MultiplayerStatus { get; private set; }

		public void SubscribeForWwStatus(IWorldSaleActivityDependant worldSaleActivityDependant)
		{
			this.worldSaleActivityDependant = worldSaleActivityDependant;
		}

		public void ProcessRemoteConfigData(ConfigValuesStorage configValuesStorage, bool correctHash)
		{
			MultiplayerStatus = configValuesStorage.multiPlayerStatus;
			worldSaleActivityDependant?.SetWorldSaleActivity(configValuesStorage.worldSaleStatus);
			ObscuredPrefs.SetInt("adsInterstit", configValuesStorage.adsCount);
			int multiPlayerStatus = configValuesStorage.multiPlayerStatus;
			if (multiPlayerStatus == 1)
			{
				ObscuredPrefs.SetInt("checkMonyCoin", 0);
				if (!correctHash)
				{
					ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "7_" + Application.version, 1);
				}
			}
			if (multiPlayerStatus == 2)
			{
				ObscuredPrefs.SetInt("checkMonyCoin", 0);
			}
			if (multiPlayerStatus == 3)
			{
				if (correctHash)
				{
					ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "7_" + Application.version, 0);
				}
				ObscuredPrefs.SetInt("checkMonyCoin", 0);
			}
			if (multiPlayerStatus == 4)
			{
				if (!correctHash)
				{
					Debug.Log("Hash is not Correct");
					ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "7_" + Application.version, 1);
				}
				ObscuredPrefs.SetInt("checkMonyCoin", 1);
			}
			if (multiPlayerStatus == 5)
			{
				ObscuredPrefs.SetInt("checkMonyCoin", 1);
			}
			if (multiPlayerStatus == 6)
			{
				ObscuredPrefs.SetInt("checkMonyCoin", 0);
				ObscuredPrefs.SetInt(SystemInfo.deviceUniqueIdentifier + "7_" + Application.version, (!correctHash) ? 1 : 0);
			}
		}
	}
}
