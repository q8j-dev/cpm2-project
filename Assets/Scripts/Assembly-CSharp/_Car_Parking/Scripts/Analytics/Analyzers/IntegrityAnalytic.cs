using System;
using Persistence.Systems;
using UnityEngine.Scripting;
using VContainer;
using VContainer.Unity;
using _Car_Parking.Scripts.Analytics.Event;
using _Car_Parking.Scripts.Analytics.Event.Contexts;
using _Car_Parking.Scripts.Analytics.Event.CustomEvent;
using _Car_Parking.Scripts.Analytics.Types;
using _Car_Parking.Scripts.Infrastructure.Start;

namespace _Car_Parking.Scripts.Analytics.Analyzers
{
	[UnityEngine.Scripting.Preserve]
	public class IntegrityAnalytic : AnalyticProviderBase, IInitializable, IDisposable
	{
		[Inject]
		public IntegrityAnalytic(AnalyticEventFactory analyticEventFactory)
			: base(analyticEventFactory)
		{
		}

		public void Initialize()
		{
			AntiCheatDetectors.CheatingDetected += OnMemoryTamperingDetected;
			SaveIntegrity.ViolationDetected += OnSaveTamperingDetected;
		}

		public void Dispose()
		{
			AntiCheatDetectors.CheatingDetected -= OnMemoryTamperingDetected;
			SaveIntegrity.ViolationDetected -= OnSaveTamperingDetected;
		}

		private void OnMemoryTamperingDetected()
		{
			Send(IntegrityDetectionType.MEMORY);
		}

		private void OnSaveTamperingDetected()
		{
			Send(IntegrityDetectionType.SAVE_FILE);
		}

		private void Send(IntegrityDetectionType type)
		{
			base.AnalyticEventFactory.CreateEvent<IntegrityCheckEvent, IntegrityCheckContext>(new IntegrityCheckContext
			{
				DetectionType = type.ToString()
			}).Send();
		}
	}
}
