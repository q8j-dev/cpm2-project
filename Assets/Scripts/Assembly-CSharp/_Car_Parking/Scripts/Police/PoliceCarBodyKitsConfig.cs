using System;
using System.Collections.Generic;
using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.Police
{
	[CreateAssetMenu(menuName = "Configs/Police/CarItems")]
	public class PoliceCarBodyKitsConfig : ScriptableObject
	{
		[Serializable]
		public class BodyKitData
		{
			public PoliceBodyKitType BodyKitType;

			public int Index;

			public TransformData TransformData;

			public string EditorElementLabelName => $"{BodyKitType} {Index}";
		}

		[SerializeField]
		private BodyKitData[] BodyKits;

		public BodyKitData GetBodyKit(PoliceBodyKitType bodyKitType, int index)
		{
			for (int i = 0; i < BodyKits.Length; i++)
			{
				if (BodyKits[i].BodyKitType == bodyKitType && BodyKits[i].Index == index)
				{
					return BodyKits[i];
				}
			}
			return null;
		}

		public List<BodyKitData> GetBodyKits(PoliceBodyKitType bodyKitType)
		{
			List<BodyKitData> list = new List<BodyKitData>();
			for (int i = 0; i < BodyKits.Length; i++)
			{
				if (BodyKits[i].BodyKitType == bodyKitType)
				{
					list.Add(BodyKits[i]);
				}
			}
			return list;
		}
	}
}
