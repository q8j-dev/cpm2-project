using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Scripting;

namespace _Car_Parking.Scripts
{
	[Preserve]
	[CreateAssetMenu(menuName = "Configs/BodyKitsTrimConfigs")]
	public class BodyKitsTrimConfigs : ScriptableObject
	{
		public enum TrimType
		{
			None = 0,
			Grill = 1,
			Headlight = 2
		}

		[Serializable]
		public class TrimData
		{
			public int ID;

			public TrimType Type;
		}

		[SerializeField]
		private TrimType[] RequiredInstall;

		[SerializeField]
		private TrimData[] TrimDatas;

		[Preserve]
		public BodyKitsTrimConfigs()
		{
		}

		public bool ThisTrimRequiredToInstall(TrimType trimType)
		{
			return Enumerable.Contains(RequiredInstall, trimType);
		}

		public TrimType GetTrimType(int id)
		{
			return GetTrimData(id)?.Type ?? TrimType.None;
		}

		public TrimData GetTrimData(int id)
		{
			for (int i = 0; i < TrimDatas.Length; i++)
			{
				if (TrimDatas[i].ID == id)
				{
					return TrimDatas[i];
				}
			}
			return null;
		}
	}
}
