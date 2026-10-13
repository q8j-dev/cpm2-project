using System;
using CPM.Connection;
using CodeStage.AntiCheat.Storage;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Car_Parking.Scripts.Licensing
{
	public class LicenseCheckerService : IInitializable
	{
		private static class CachedSecureStorage
		{
			private const string key = "GooglePlayCachedLicenseResponse1";

			public static void SaveString(string str)
			{
				ObscuredPrefs.SetString("GooglePlayCachedLicenseResponse1", str);
				PlayerPrefs.Save();
			}

			public static string LoadString()
			{
				return ObscuredPrefs.GetString("GooglePlayCachedLicenseResponse1");
			}
		}

		private IConnectionManager _connectionManager;

		public event Action<bool> OnLicenseChecked;

		[Inject]
		public LicenseCheckerService(IConnectionManager connectionManager)
		{
			_connectionManager = connectionManager;
		}

		public void Initialize()
		{
			Check();
		}

		private void Check()
		{
		}

		private void SetBadHash()
		{
			_connectionManager.SetNotLicensedApp();
		}
	}
}
