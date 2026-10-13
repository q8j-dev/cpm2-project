using UnityEngine;

public class CircuitAiCarStateChanger : MonoBehaviour
{
	public CarSteeringState TargetState;

	public int TargetDirection;

	public bool DirectionCheck;

	private void OnEnable()
	{
		if (DirectionCheck)
		{
			CheckDirection();
		}
	}

	public void CheckDirection()
	{
		base.gameObject.SetActive(CircuitRaceControlller.Direction == TargetDirection);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.layer == 31 && (bool)other.gameObject.transform.parent && other.gameObject.transform.parent.TryGetComponent<LapRaceCar>(out var component))
		{
			OnLapRaceCarEnter(component);
		}
	}

	public virtual void OnLapRaceCarEnter(LapRaceCar lapRaceCar)
	{
		lapRaceCar.ChangeCarState(TargetState);
	}
}
