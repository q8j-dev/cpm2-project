using System;
using UnityEngine;

namespace CPMEngine
{
	[Serializable]
	public class TCurveTyre
	{
		public AnimationCurve Curve;

		public TyreType Type;

		public float TreadThickness = 0.01f;
	}
}
