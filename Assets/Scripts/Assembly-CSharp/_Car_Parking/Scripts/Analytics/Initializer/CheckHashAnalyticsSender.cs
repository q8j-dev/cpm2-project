using System;
using CPM.Connection;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Connection;

namespace _Car_Parking.Scripts.Analytics.Initializer
{
	public class CheckHashAnalyticsSender : IInitializable, IDisposable
	{
		[Inject]
		private AnalyticEventFactory _analyticEventFactory;

		[Inject]
		private IConnectionManager _connectionManager;

		private FirebaseConnectionService _firebaseConnectionService;

		public void Initialize()
		{
			if (_connectionManager != null && _connectionManager is ConnectionHandler connectionHandler && connectionHandler.GetConnectionService() is FirebaseConnectionService firebaseConnectionService)
			{
				_firebaseConnectionService = firebaseConnectionService;
				_firebaseConnectionService.OnCheckHashResult += SendCheckHashResultAnalytic;
			}
		}

		private void SendCheckHashResultAnalytic(CheckHashResultForAnalytics result)
		{
			CheckHashContext context = new CheckHashContext
			{
				Source = result.Source,
				AmountRetry = result.AmountRetry,
				Result = result.Result,
				IsValidated = result.IsValidated,
				Error = result.Error
			};
			_analyticEventFactory.CreateEvent<CheckHashEvent, CheckHashContext>(context).Send();
		}

		public void Dispose()
		{
			if (_firebaseConnectionService != null)
			{
				_firebaseConnectionService.OnCheckHashResult -= SendCheckHashResultAnalytic;
			}
		}
	}
}
