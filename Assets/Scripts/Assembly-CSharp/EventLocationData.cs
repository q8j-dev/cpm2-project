using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.EventSystem;
using _Car_Parking.Scripts.Free_drive;

[Serializable]
public class EventLocationData
{
	public SceneType SceneType;

	public List<ZoneConfigData> CollectiblePositions;

	public EventType EventType = EventType.None;
}
