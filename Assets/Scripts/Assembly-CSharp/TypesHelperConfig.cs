using UnityEngine;

[CreateAssetMenu(fileName = "Engine", menuName = "CarPartsTuning/UI/TypesHelperConfig", order = 1)]
public class TypesHelperConfig : ScriptableObject
{
	public string MainFolder = "Assets/_Car Parking/Scripts/";

	public string ConfigsFolder = "Assets/_Car Parking/Configs/";

	private string CircuitCarsConfigsFolder = "Assets/_Car Parking/Configs/_RacingCars/CircuitCars/";
}
