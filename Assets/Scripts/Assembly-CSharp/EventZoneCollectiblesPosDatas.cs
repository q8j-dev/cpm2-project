using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using _Car_Parking.Scripts.Free_drive;

[CreateAssetMenu(fileName = "EventZoneCollectiblesPosDataConfig", menuName = "Configs/EventZoneCollectiblesPosDatas")]
public class EventZoneCollectiblesPosDatas : ScriptableObject
{
	public List<EventLocationData> CollectibleZones;

	public List<ZoneConfigData> AllZoneConfigDatas { get; private set; }

	public void Init()
	{
		if (AllZoneConfigDatas != null)
		{
			return;
		}
		AllZoneConfigDatas = new List<ZoneConfigData>();
		foreach (EventLocationData collectibleZone in CollectibleZones)
		{
			foreach (ZoneConfigData collectiblePosition in collectibleZone.CollectiblePositions)
			{
				if (collectiblePosition != null)
				{
					AllZoneConfigDatas.Add(collectiblePosition);
				}
			}
		}
	}

	public ZoneConfigData GetPositions(int index, SceneType sceneType)
	{
		return CollectibleZones.FirstOrDefault((EventLocationData x) => x.SceneType == sceneType)?.CollectiblePositions.FirstOrDefault((ZoneConfigData x) => x.Index == index);
	}

	public void SetPosition(int indexForConfig, SceneType sceneTypeForConfig, Vector3 transformLocalPosition)
	{
		CollectibleZones.FirstOrDefault((EventLocationData x) => x.SceneType == sceneTypeForConfig).CollectiblePositions.FirstOrDefault((ZoneConfigData x) => x.Index == indexForConfig).ZonePosition = transformLocalPosition;
	}
}
