using System.Collections.Generic;
using UnityEngine;

namespace _Car_Parking.Configs
{
	[CreateAssetMenu(fileName = "InteriorInfoConfig", menuName = "Scriptable/InteriorInfoConfig")]
	public class InteriorInfoConfig : ScriptableObject
	{
		public List<InteriorInfo> InteriorsInfos = new List<InteriorInfo>();

		public Dictionary<int, InteriorInfo> InteriorsInfosDic = new Dictionary<int, InteriorInfo>();

		public void Init()
		{
			foreach (InteriorInfo interiorsInfo in InteriorsInfos)
			{
				InteriorsInfosDic.TryAdd(interiorsInfo.Number, interiorsInfo);
			}
		}
	}
}
