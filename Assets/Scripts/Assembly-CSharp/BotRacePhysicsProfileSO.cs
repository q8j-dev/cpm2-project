using UnityEngine;

[CreateAssetMenu(fileName = "BotRacePhysicsProfile", menuName = "Scriptable/Racing/Bot Race Physics Profile", order = 5)]
public class BotRacePhysicsProfileSO : ScriptableObject
{
	[Header("Longitudinal")]
	[Tooltip("Top speed governor (km/h). If > 0 it is FROZEN (fully physics-independent). If <= 0 (default) it is derived ONCE from Powertrain.CalcTopSpeed() at spawn — gives per-class top speed automatically while keeping all handling frozen. Straight-line top speed is not a breakage symptom, so deriving it once is safe; set a value to fully freeze.")]
	public float TopSpeedKmh;

	[Tooltip("Forward acceleration cap (m/s²) at full throttle. Replaces the engine torque path.")]
	public float MaxAccelMps2 = 12f;

	[Tooltip("Braking deceleration cap (m/s²) at full brake. Replaces Powertrain.MaxBrakeAccel.")]
	public float MaxBrakeMps2 = 14f;

	[Header("Launch / ICE torque curve")]
	[Tooltip("Traction multiplier the moment the car pulls away (0..1). Lower = softer, slower launch. Actual launch accel = MaxAccelMps2 × this. 1 = instant full torque (electric-car feel).")]
	[Range(0.05f, 1f)]
	public float LaunchTraction = 0.256f;

	[Range(0.02f, 0.6f)]
	[Tooltip("Length of the soft-launch phase as a FRACTION of TopSpeedKmh: traction ramps from LaunchTraction up to full over this band. NOTE: if TopSpeedKmh is a placeholder like 500, this stretches far past a real launch (0.22 × 500 ≈ 110 km/h) — set a realistic top speed.")]
	public float LaunchBandFrac = 0.22f;

	[Range(0f, 0.25f)]
	[Tooltip("Per-car pace spread (±fraction) on accel + top speed so same-class bots aren't clones. 0 = identical, 0.06 = ±6%. Deterministic by spawn index; does NOT touch grip, so cornering stays invariant-safe.")]
	[Header("Per-car variation")]
	public float PaceSpreadFrac = 0.06f;

	[Header("Lateral grip (shared by mover AND planner)")]
	[Tooltip("Lateral acceleration the chassis can produce (m/s²). The planner uses sqrt(this * R) for corner speed; the mover caps lateral force at this * mass. Same number = no slide/roll.")]
	public float MaxLateralAccelMps2 = 15f;

	[Header("Steering authority (mover)")]
	[Tooltip("Gain converting front-axle slip angle into yaw torque. Higher = sharper turn-in.")]
	public float SteerTorqueGain = 60f;

	[Tooltip("Hard clamp on the per-axle yaw torque coefficient (m/s² equivalent before * mass * dt).")]
	public float MaxSteerTorque = 30f;

	[Tooltip("Vertical velocity damping — kills bounce. Traffic uses 6.")]
	[Header("Stability damping (mover)")]
	public float VerticalDamp = 6f;

	[Tooltip("Yaw/roll angular damping — kills spin and keeps the car planted. Traffic uses 5.")]
	public float AngularDamp = 5f;

	public static BotRacePhysicsProfileSO CreateDefault()
	{
		return ScriptableObject.CreateInstance<BotRacePhysicsProfileSO>();
	}
}
