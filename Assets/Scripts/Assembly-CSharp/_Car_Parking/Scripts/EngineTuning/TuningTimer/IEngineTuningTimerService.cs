using System;
using System.Collections.Generic;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	public interface IEngineTuningTimerService
	{
		int ObservingEngineId { get; set; }

		bool IsOnPause { get; set; }

		event Action<TimerEnums, float> ProgressEvent;

		event Action<bool> HideEvent;

		event Action<bool> SwitchToEngineProgressEvent;

		event Action<int> OnTimerCompleted;

		bool CheckEngineBlocked(int engineId);

		void CreateTimer(int engineId, params PartData[] partsToSet);

		List<PartData> GetPartsInTimers();

		bool TryGetTimerInfo(int engineId, out EngineTimerInfo info);

		List<TuningTimerEntry> GetTimerEntries(int engineId);

		bool TryApplyAdReduction(int engineId, string timerId, double seconds);

		bool TryFinishNow(int engineId, string timerId);
	}
}
