using System.Collections.Generic;
using CPM.Database;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;
using _Car_Parking.Scripts.Drag.Engine;
using _Car_Parking.Scripts.EngineTuning;

[CreateAssetMenu(fileName = "carPreset0", menuName = "CarConfigs/CarPresetTuningData")]
public class VehiclePresetTuningData : ScriptableObject
{
	public ObscuredInt Id;

	public ObscuredInt Style = -1;

	[Header("Engine")]
	public EngineItem EngineDefaultConfig;

	[Header("Gearbox")]
	public GearBoxPart GearBox;

	[Header("Differentials")]
	public LSDPart FrontDifferential;

	public LSDPart RearDifferential;

	[Header("Parts")]
	public List<PartItem> Parts;

	[Header("Front Wheel Data")]
	public FrontWheelData FrontWheelData;

	[Header("Rear Wheel Data")]
	public RearWheelData RearWheelData;

	[Header("Other car data")]
	public CarInfoMain CarInfoMain;
}
