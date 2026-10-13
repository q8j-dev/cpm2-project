using System.Collections.Generic;
using CPM.Shifter.Logic;
using UnityEngine;

namespace CPM.Shifter.Contracts
{
	public interface IManualHShifterView
	{
		bool IsLayoutReady { get; }

		Vector2 KnobAnchoredPos { get; }

		IReadOnlyList<Rect> AllowedZones { get; }

		void ApplyLayout(ShifterLayoutAsset layout);

		Vector2 GetSlotAnchoredPos(int gear);

		bool IsSlotActive(int gear);

		void SetKnobAnchoredPos(Vector2 pos);

		void MoveKnobTo(Vector2 anchoredPos, float duration);

		void SetGearTraceAlpha(float a);
	}
}
