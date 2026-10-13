using UnityEngine;
using UnityEngine.Scripting;

[CreateAssetMenu(fileName = "CircuitRacingConfig", menuName = "Configs/CircuitRaceConfigCircuitRacingConfig")]
[Preserve]
public class CircuitRacingConfig : ScriptableObject
{
	public CircuitRaceMapData[] MapDatas;

	public CircuitRaceConfigItem[] CircuitRacingConfigItems;

	[Preserve]
	public CircuitRacingConfig()
	{
	}
}
