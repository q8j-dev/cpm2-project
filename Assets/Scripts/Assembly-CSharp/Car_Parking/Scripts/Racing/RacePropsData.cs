using System;
using UnityEngine;

namespace Car_Parking.Scripts.Racing
{
	[Serializable]
	public class RacePropsData
	{
		public int MissCheckpointMultiplierSecs = 15;

		public V.placeToSpawn Location = V.placeToSpawn.Racing;

		public int TrackIndex;

		public int TrajectoryIndex;

		public Texture MapTexture;

		public Transform[] ActivatableTriggers;

		public RaceFinishLineTrigger[] Triggers;

		public RaceStartTrigger[] RaceStartTriggers;

		public Transform[] ToRoadPoints;

		public Transform[] RestartPoints;
	}
}
