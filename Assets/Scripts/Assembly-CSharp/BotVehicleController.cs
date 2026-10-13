using UnityEngine;

public class BotVehicleController
{
	private const float MaxSteerRatePerSec = 6f;

	private readonly CircuitCarStearing _owner;

	private float _lastThrottle;

	private float _lastBrake;

	private float _lastSteering;

	public BotVehicleController(CircuitCarStearing owner)
	{
		_owner = owner;
	}

	public void Apply(CarAIController ai, in DriveCommand cmd, in RouteTarget route, bool reverse)
	{
		if (!(ai == null))
		{
			float num = Mathf.Clamp01(cmd.Throttle01);
			float num2 = Mathf.Clamp01(cmd.Brake01);
			if (num2 > 0.1f)
			{
				num = 0f;
			}
			float num3 = cmd.SteeringOverride ?? route.SteeringNormalized;
			if (reverse)
			{
				num3 = 0f - num3;
			}
			num3 = Mathf.MoveTowards(_lastSteering, num3, 6f * Time.fixedDeltaTime);
			ai.TargetThrottle = num;
			ai.TargetBrake = num2;
			ai.TargetSteering = num3;
			_lastThrottle = num;
			_lastBrake = num2;
			_lastSteering = num3;
		}
	}
}
