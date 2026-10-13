using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Offers;
using _Car_Parking.Scripts.Racing_Scripts;
using _Car_Parking.Scripts.Racing_Scripts.RallyCrowd;
using _Car_Parking.Scripts.Racing_Scripts.RallyProps;

namespace _Car_Parking.Scripts.Rally.Data
{
	[Serializable]
	public class RallySequenceData
	{
		[Serializable]
		public class RewardData
		{
			public Sprite Icon;

			[SerializeReference]
			public RewardItemData[] RewardItems;

			public bool HaveReward
			{
				get
				{
					if (RewardItems != null)
					{
						return RewardItems.Length != 0;
					}
					return false;
				}
			}
		}

		public Sprite TrajectoryLogo;

		public RaceGaragePoint RaceGaragePoint;

		public RaceGaragePoint CarStartGaragePoint;

		public RaceGaragePoint CarStagePoint;

		public int Index;

		public V.placeToSpawn PlaceToSpawn;

		public List<RallySequenceItem> RallySequenceItems;

		public GameObject TrajectoryPrefab;

		public CPMCrowdDataSO CrowdData;

		public RallyPropDataSO PropData;

		public RallyTimes[] RallyTimes;

		public List<Vector3> RoadCenterline;

		public float RoadHalfWidth = 20f;

		public bool IsSpecial;

		public RewardData Reward;

		public bool HaveReward
		{
			get
			{
				if (Reward != null)
				{
					return Reward.HaveReward;
				}
				return false;
			}
		}

		public RallySequenceData()
		{
		}

		private void SetRallySequenceData(List<RallySequenceItem> rallySequenceItems)
		{
			RallySequenceItems = new List<RallySequenceItem>();
			for (int i = 0; i < rallySequenceItems.Count; i++)
			{
				RallySequenceItem rallySequenceItem = new RallySequenceItem();
				rallySequenceItem.Index = i;
				rallySequenceItem.Distance = rallySequenceItems[i].Distance;
				rallySequenceItem.Degree = rallySequenceItems[i].Degree;
				rallySequenceItem.SequenceTurnType = rallySequenceItems[i].SequenceTurnType;
				rallySequenceItem.Link = rallySequenceItems[i].Link;
				rallySequenceItem.Mark = rallySequenceItems[i].Mark;
				rallySequenceItem.Position = rallySequenceItems[i].Position;
				rallySequenceItem.Rotation = rallySequenceItems[i].Rotation;
				rallySequenceItem.IgnoreRaycastOnStart = rallySequenceItems[i].IgnoreRaycastOnStart;
				rallySequenceItem.TriggerSize = rallySequenceItems[i].TriggerSize;
				RallySequenceItems.Add(rallySequenceItem);
			}
			ParseChain();
		}

		[ContextMenu("ParseChain")]
		public void ParseChain()
		{
			for (int i = 0; i < RallySequenceItems.Count; i++)
			{
				int num = i + 1;
				if (num < RallySequenceItems.Count)
				{
					RallySequenceItems[i].NextItem = RallySequenceItems[num];
				}
			}
		}

		public RallySequenceData(V.placeToSpawn placeToSpawn, List<RallySequenceItem> rallySequenceItems)
		{
			PlaceToSpawn = placeToSpawn;
			SetRallySequenceData(rallySequenceItems);
		}
	}
}
