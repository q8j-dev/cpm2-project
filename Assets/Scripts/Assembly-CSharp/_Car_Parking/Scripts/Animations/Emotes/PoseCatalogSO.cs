using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.WorldInteraction;

namespace _Car_Parking.Scripts.Animations.Emotes
{
	[CreateAssetMenu(fileName = "PoseCatalog", menuName = "Scriptable/PoseCatalog")]
	public class PoseCatalogSO : ScriptableObject
	{
		[Serializable]
		public class PoseEntry
		{
			public int IKPoseId;

			public Sprite Sprite;

			public PoseAudience Audience;

			public int BundleId = -1;

			public bool IsFree;
		}

		public const string ResourcesPath = "Configs/PoseCatalog";

		public const int PoseSlotIdOffset = 1000;

		public const int AudienceSlotStride = 100;

		public const int MaxIkPoseId = 100;

		[SerializeField]
		private List<PoseEntry> Poses = new List<PoseEntry>();

		public IReadOnlyList<PoseEntry> Entries => Poses;

		public static int SlotOffsetFor(PoseAudience audience)
		{
			return audience switch
			{
				PoseAudience.Female => 1100, 
				PoseAudience.Male => 1200, 
				_ => 1000, 
			};
		}

		public List<int> CollectIdsForBundle(int? bundleId)
		{
			List<int> list = new List<int>();
			foreach (PoseEntry pose in Poses)
			{
				if (pose != null && (!bundleId.HasValue || pose.BundleId == bundleId.Value) && !list.Contains(pose.IKPoseId))
				{
					list.Add(pose.IKPoseId);
				}
			}
			return list;
		}

		private void OnValidate()
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (PoseEntry pose in Poses)
			{
				if (pose == null)
				{
					continue;
				}
				if (pose.IKPoseId < 0 || pose.IKPoseId >= 100)
				{
					Debug.LogError($"PoseCatalogSO: IKPoseId = {pose.IKPoseId} is outside [0, {100}). One audience partition is {100} wide.");
					continue;
				}
				int item = SlotOffsetFor(pose.Audience) + pose.IKPoseId;
				if (!hashSet.Add(item))
				{
					Debug.LogError($"PoseCatalogSO: duplicate (Audience = {pose.Audience}, IKPoseId = {pose.IKPoseId}). The pair keys the slot code and must be unique.");
				}
			}
		}
	}
}
