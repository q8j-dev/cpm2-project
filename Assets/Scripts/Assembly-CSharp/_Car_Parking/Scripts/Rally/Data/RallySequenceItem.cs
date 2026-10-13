using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Rally.Data
{
	[Serializable]
	public class RallySequenceItem
	{
		public Vector3 Position;

		public Quaternion Rotation;

		public int Index;

		public float Distance;

		public float Degree;

		public RallySequenceTurnType SequenceTurnType;

		public RallySequenceLink Link;

		public RallySequenceMark[] Mark;

		public RallySequenceItem NextItem;

		public bool IgnoreRaycastOnStart;

		public TriggerSize TriggerSize;
	}
}
