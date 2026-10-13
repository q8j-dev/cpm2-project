using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Connection
{
	public class BlockAppStatusUtil
	{
		private ObscuredInt _blockAppStatus;

		public BlockAppStatusUtil(ObscuredInt blockAppStatus)
		{
			_blockAppStatus = blockAppStatus;
		}

		public bool BlockBadHashApp()
		{
			if ((int)_blockAppStatus != 1)
			{
				return (int)_blockAppStatus == 3;
			}
			return true;
		}

		public bool BlockNotLicensedApp()
		{
			if ((int)_blockAppStatus != 2)
			{
				return (int)_blockAppStatus == 3;
			}
			return true;
		}

		public static void CheckNeedsToUpdateGame(bool needsToUpdateGame)
		{
			if (needsToUpdateGame)
			{
				Debug.LogError("Game needs update");
				ScenesHelper.LoadNotificationScene(SceneNotificationReason.AppUpdate);
			}
		}
	}
}
