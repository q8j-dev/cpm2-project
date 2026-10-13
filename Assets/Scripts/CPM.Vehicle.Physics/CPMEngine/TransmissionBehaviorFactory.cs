using CPM.Transmission;
using UnityEngine;

namespace CPMEngine
{
	public static class TransmissionBehaviorFactory
	{
		public static ITransmissionBehavior Create(GearboxKind kind, in GearboxProfile profile)
		{
			switch (kind)
			{
			case GearboxKind.MT:
				return new MtBehavior();
			case GearboxKind.AT:
				return new AtBehavior();
			case GearboxKind.AMT:
				return new AmtBehavior();
			case GearboxKind.DCT:
				return new DctBehavior();
			case GearboxKind.CVT:
				return new CvtBehavior();
			case GearboxKind.DirectDrive:
				return new DirectDriveBehavior();
			case GearboxKind.Sequential:
				return new SequentialBehavior();
			default:
				Debug.LogWarning($"[Transmission] Поведение для {kind} ещё не реализовано (фазы 5–6). Fallback => AT.");
				return new AtBehavior();
			}
		}
	}
}
