using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.CustomDataTypes;

namespace _Car_Parking.Scripts.EngineTuning.TuningTimer
{
	public interface ITimerTuningService
	{
		bool IsPause { get; set; }

		event Action<int, List<PartData>> TimerCompletedEvent;

		event Action<TimerEnums, float> ProgressEvent;

		event Action<bool> HideEvent;

		AdvancedBoolean TryCreateTimer(int slotId, params PartData[] partsToSet);

		AdvancedBoolean IsCarBlocked(int slotId);

		TimerInstallResult TryInstallPart(int slotId, PartData part);

		AdvancedBoolean SkipTimer(int slotId, out List<PartData> newPartDatas);

		AdvancedBoolean TryCreateTimer(int slotId, EngineKitData engineKit, PartData ecuToSet);

		AdvancedBoolean TryCreateTimer(int slotId, EngineKitData engine);

		List<EngineKitData> GetEnginesInTimers();

		List<PartData> GetPartsInTimers();

		bool TryGetEngineTimerInfo(int slotId, out EngineTimerInfo info);

		List<TuningTimerEntry> GetTimerEntries(int slotId);

		bool TryApplyAdReduction(int slotId, string timerId, double seconds);

		bool TryFinishNow(int slotId, string timerId);
	}
}
