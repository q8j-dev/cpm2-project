using System;
using Firebase.Functions;
using UnityEngine.Scripting;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Database.Functions;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[Preserve]
	public class BackendErrorAnalytic : AnalyticProviderBase, IInitializable, IDisposable
	{
		[Preserve]
		public BackendErrorAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void Initialize()
		{
			FirebaseWrappedFunction.OnErrorCalled += OnFunctionErrorCalled;
		}

		public void Dispose()
		{
			FirebaseWrappedFunction.OnErrorCalled -= OnFunctionErrorCalled;
		}

		private void OnFunctionErrorCalled(string functionName, FunctionStatus functionStatus)
		{
			base.AnalyticEventFactory.CreateEvent<BackendErrorEvent, BackendErrorContext>(new BackendErrorContext
			{
				FunctionId = FirebaseFunctionIdMap.GetId(functionName),
				FunctionStatus = (int)functionStatus,
				WindowId = GetWindowId()
			}).Send();
		}

		private string GetWindowId()
		{
			if (!V.CoreGameManager || !V.CoreGameManager.ManagerUI)
			{
				return "";
			}
			return V.CoreGameManager.ManagerUI.GetCurrentWindow();
		}
	}
}
