using System;
using CPMEngine;
using UnityEngine;

namespace CPM.CarVisualModel
{
	[Serializable]
	public struct WheelMount
	{
		[Tooltip("Wheel position relative to root (from CarChildsReferences.Wheel* of the car{id} prefab)")]
		public Vector3 LocalPosition;

		[Tooltip("Wheel mount rotation (usually 0)")]
		public Vector3 LocalEuler;

		[Tooltip("Wheel lossyScale. NOT used by the builder (mirroring is done via IsLeft + InstallSideDisk) — safe to ignore")]
		public Vector3 LocalScale;

		[Tooltip("Which wheel: FrontLeft=0, FrontRight=1, RearLeft=2, RearRight=3 (+Rear1/Rear2). Drives left/right (mirror) and front/rear (which WheelData)")]
		public WheelIndex WheelIndex;
	}
}
