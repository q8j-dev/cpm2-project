using UnityEngine;
using UnityEngine.Scripting;

[CreateAssetMenu(fileName = "BrakesTuningConfig", menuName = "Scriptable/BrakesTuningConfig", order = 15)]
public class BrakeTuningConfig : ScriptableObject
{
	public BrakeDiskItemData[] AllBrakes;

	public BrakeDiskItemData[] AllBrakesCaliperDatas;

	[Preserve]
	public BrakeTuningConfig()
	{
	}
}
