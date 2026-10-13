using System;
using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Scripts.WorldInteraction
{
	[CreateAssetMenu(menuName = "WorldInteraction/Config", fileName = "WorldInteractionConfig")]
	public class WorldInteractionConfig : ScriptableObject
	{
		[Serializable]
		public class TypeSettings
		{
			public int Id;

			public bool IsInPlace;

			public bool IsVertical;

			public bool DisableAnimation;

			public float PreviewGroundLift;

			public float MinHeight = 0.2f;

			public float MaxHeight = 1.2f;

			public float MaxNormalAngle = 25f;

			public float MaxFlatnessSpread = 0.08f;

			public bool IncludeTriggerSurfaces = true;

			public WorldPoseData DefaultPose = new WorldPoseData();

			public string ElementLabel => PoseLabel(Id);
		}

		[SerializeField]
		private List<TypeSettings> _perType = new List<TypeSettings>();

		public IReadOnlyList<TypeSettings> Entries => _perType;

		public static string PoseLabel(int id)
		{
			return id switch
			{
				1 => "1 Squat", 
				2 => "2 Lean", 
				3 => "3 LeaningSit", 
				10 => "10 Ninja", 
				_ => "Pose " + id, 
			};
		}

		public TypeSettings GetSettings(int id)
		{
			for (int i = 0; i < _perType.Count; i++)
			{
				if (_perType[i].Id == id)
				{
					if (_perType[i].DefaultPose != null)
					{
						_perType[i].DefaultPose.Id = id;
					}
					return _perType[i];
				}
			}
			return CreateDefault(id);
		}

		public static TypeSettings CreateDefault(int id)
		{
			TypeSettings typeSettings = new TypeSettings
			{
				Id = id,
				DefaultPose = new WorldPoseData
				{
					Id = id
				}
			};
			switch (id)
			{
			case 1:
				typeSettings.IsInPlace = true;
				break;
			case 2:
				typeSettings.IsVertical = true;
				break;
			case 3:
				typeSettings.IsVertical = true;
				break;
			case 10:
				typeSettings.IsInPlace = true;
				break;
			}
			return typeSettings;
		}
	}
}
