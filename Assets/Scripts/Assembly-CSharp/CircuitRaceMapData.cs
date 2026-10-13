using System;
using UnityEngine.Serialization;

[Serializable]
public class CircuitRaceMapData
{
	public int Id;

	[FormerlySerializedAs("CircuitRaceConfigItems")]
	public CircuitRaceConfigItem[] Groups;
}
