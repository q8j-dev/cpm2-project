using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Car_Parking.Scripts.Rally.Data;

namespace _Car_Parking.Scripts.Racing_Scripts
{
	[CreateAssetMenu(fileName = "RallyMapDataSO", menuName = "Configs/RallyMapDataSO")]
	public class RallyMapDataSO : ScriptableObject
	{
		public List<RallyLocationData> RallyLocationDatas;

		public RallyTrajectoryWeatherDataSO TrajectoryWeatherData;

		public RallySequenceData GetRallySequenceData(int locationId, int trajectoryId)
		{
			return RallyLocationDatas.FirstOrDefault((RallyLocationData l) => l.Id == locationId)?.TrajectoryData?.RallySequenceDataList.FirstOrDefault((RallySequenceData t) => t.Index == trajectoryId);
		}

		public bool TryGetConfigIds(int locationPosition, int trajectoryPosition, out int locationId, out int trajectoryId)
		{
			locationId = -1;
			trajectoryId = -1;
			if (RallyLocationDatas == null || locationPosition < 0 || locationPosition >= RallyLocationDatas.Count)
			{
				return false;
			}
			RallyLocationData rallyLocationData = RallyLocationDatas[locationPosition];
			List<RallySequenceData> list = rallyLocationData?.TrajectoryData?.RallySequenceDataList;
			if (list == null || trajectoryPosition < 0 || trajectoryPosition >= list.Count)
			{
				return false;
			}
			locationId = rallyLocationData.Id;
			trajectoryId = list[trajectoryPosition].Index;
			return true;
		}
	}
}
