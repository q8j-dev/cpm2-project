using System;
using CPMEngine;
using UnityEngine;

[Serializable]
public class CircuitRaceConfigItem
{
	public MainCarValues[] RaceCarsConfigs;

	public string Name;

	public VehicleClass Class;

	[Tooltip("Need to change coins value in backend")]
	public int[] Rewards;
}
