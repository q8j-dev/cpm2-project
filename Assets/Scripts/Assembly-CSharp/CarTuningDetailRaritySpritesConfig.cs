using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
[CreateAssetMenu(fileName = "CarTuningDetailRaritySpritesConfig", menuName = "Scriptable/CarTuningDetailRaritySpritesConfig", order = 15)]
public class CarTuningDetailRaritySpritesConfig : ScriptableObject
{
	public CarTuningDetailRarityData[] CarTuningDetailRarityDatas;

	[Preserve]
	public CarTuningDetailRaritySpritesConfig()
	{
	}
}
