using System;
using CPM.Account;
using Cysharp.Threading.Tasks;
using Firebase;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Root;
using _Car_Parking.Scripts.Analytics.Root.Data;

namespace _Car_Parking.Scripts.Database
{
	public class FirebaseInitializer : IInitializable, IDisposable
	{
		private CPMAnalytics _analytics;

		private IAccountController _accountController;

		private static FirebaseApp _app;

		public static bool IsInitialized { get; private set; }

		public FirebaseInitializer(IAccountController accountController)
		{
			_accountController = accountController;
		}

		public void Initialize()
		{
			_analytics.Initialize(GetEnvironment()).Forget();
			InitCrashlytics(_accountController);
		}

		[Inject]
		public void Construct(CPMAnalytics analytics)
		{
			_analytics = analytics;
		}

		public void Dispose()
		{
		}

		public static void InitCrashlytics(IAccountController accountController)
		{
			InitAllFirebase(accountController).Forget();
		}

		public static async UniTask InitAllFirebase(IAccountController accountController)
		{
			try
			{
				if (await CheckFirebaseDependencies(accountController))
				{
					IsInitialized = true;
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Init firebase error: " + ex);
			}
		}

		private static async UniTask<bool> CheckFirebaseDependencies(IAccountController accountController)
		{
			ServiceAvailability serviceAvailability = await accountController.GetServiceAvailability();
			if (FlagsStorage.AnalyticsInitialized)
			{
				return true;
			}
			if (serviceAvailability == ServiceAvailability.Available)
			{
				FlagsStorage.AnalyticsInitialized = true;
				_app = FirebaseApp.DefaultInstance;
			}
			else
			{
				Debug.LogError($"Could not resolve all Firebase dependencies: {serviceAvailability}");
			}
			return serviceAvailability == ServiceAvailability.Available;
		}

		private static string GetEnvironmentStr()
		{
			return "prod";
		}

		private static Env GetEnvironment()
		{
			return Env.prod;
		}
	}
}
