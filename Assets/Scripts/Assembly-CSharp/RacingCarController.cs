using CPMEngine;
using UnityEngine;
using _Car_Parking.Scripts.Other.Helpers;

public class RacingCarController : MonoBehaviour
{
	private interface IGetLodDistance
	{
		float LODDistance { get; }
	}

	private class DragRacingLODDistance : IGetLodDistance
	{
		public float LODDistance => 30f;
	}

	private class CircuitRacingLODDistance : IGetLodDistance
	{
		public float LODDistance => Mathf.Clamp(V.DetailDistance, 5f, 30f);
	}

	private VehicleData _vehicleData;

	private Rigidbody rb;

	private Axles axles;

	public bool wheelOn;

	private float timeToDisable;

	public float distance;

	private IGetLodDistance _lodDistance;

	public bool DontSwitchLOD;

	private void Start()
	{
		_vehicleData = GetComponent<VehicleData>();
		rb = GetComponent<Rigidbody>();
		axles = GetComponent<Axles>();
		wheelOn = true;
		if (ScenesHelper.IsLapRacingSceneActive())
		{
			_lodDistance = new CircuitRacingLODDistance();
		}
		else
		{
			_lodDistance = new DragRacingLODDistance();
		}
	}

	private void Update()
	{
		if (Time.frameCount % 10 != 0 || _vehicleData.MainModel == null)
		{
			return;
		}
		distance = Vector3.Distance(V.CoreGameManager.transform.position, base.transform.position);
		if (distance > _lodDistance.LODDistance && !DontSwitchLOD)
		{
			_vehicleData.BodyLoadingComposition.ShowLod(val: true);
			_vehicleData.BodyLoadingComposition.SetLodWheel(showLODWheel: true);
		}
		else
		{
			_vehicleData.BodyLoadingComposition.ShowMainModelOnly();
		}
		if (_vehicleData.racingState == RacingState.onGarage)
		{
			if (wheelOn)
			{
				if (timeToDisable == 0f)
				{
					timeToDisable = Time.time;
				}
				else if (Time.time - timeToDisable > 2f)
				{
					TurnOffOnWheel(enable: false);
				}
			}
		}
		else if (!wheelOn)
		{
			TurnOffOnWheel(enable: true);
		}
	}

	private void TurnOffOnWheel(bool enable)
	{
		rb.isKinematic = !enable;
		foreach (WheelProvider allWheel in axles.AllWheels)
		{
			allWheel.enabled = enable;
		}
		wheelOn = enable;
		if (wheelOn)
		{
			timeToDisable = 0f;
		}
	}
}
