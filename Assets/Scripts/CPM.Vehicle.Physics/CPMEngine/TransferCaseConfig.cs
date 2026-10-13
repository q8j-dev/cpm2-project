using System;
using UnityEngine;

namespace CPMEngine
{
	[Serializable]
	public class TransferCaseConfig
	{
		public bool Enabled;

		public bool HasLowRange = true;

		[Range(1.5f, 4f)]
		public float LowRangeRatio = 2.5f;

		public TransferCaseMode DefaultMode = TransferCaseMode.H4;

		public bool HasFrontLock;

		public bool HasRearLock;

		public bool IsFullTime;

		public bool HasCenterLock;
	}
}
