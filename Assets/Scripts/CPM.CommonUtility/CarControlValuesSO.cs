using UnityEngine;
using UnityEngine.Scripting;

[CreateAssetMenu(fileName = "CarControlValues", menuName = "Scriptable/CarControlValuesConfig", order = 15)]
[Preserve]
public class CarControlValuesSO : ScriptableObject
{
	public AnimationCurve steeringCurve;

	public AnimationCurve steeringCurveByInput;

	public float TimeToOnHandBrake = 0.45f;

	[Preserve]
	public CarControlValuesSO()
	{
	}
}
