using UnityEngine;

[CreateAssetMenu(fileName = "BotTuning", menuName = "Scriptable/Racing/Bot Tuning", order = 9)]
public class BotTuningSO : ScriptableObject
{
	[Tooltip("Forward acceleration cap (m/s²) at full throttle.")]
	[Header("Default bot physics profile (used when no per-class BotRacePhysicsProfile is assigned)")]
	public float DefaultBotMaxAccel = 12f;

	[Tooltip("Braking deceleration cap (m/s²).")]
	public float DefaultBotMaxBrake = 14f;

	[Tooltip("Mover lateral-force headroom (m/s²). MUST equal BotSpeedPlanner.GripMaxMps2 (22) — any desync brings sharp-corner sliding back. 2.2g is safely under the ~2.5g mover breakaway.")]
	public float DefaultBotMaxLateral = 22f;

	[Tooltip("Front-axle slip→yaw torque gain.")]
	public float DefaultBotSteerGain = 60f;

	[Tooltip("Hard clamp on per-axle yaw torque coefficient.")]
	public float DefaultBotMaxSteerTorque = 30f;

	[Tooltip("±fraction per-car accel/top-speed trim so same-class bots aren't clones.")]
	public float DefaultBotPaceSpread = 0.13f;

	[Range(0.3f, 1.2f)]
	[Tooltip("Global multiplier on straight-line top speed (mover cap + planner cruise). 1 = full, 0.8 = 20% slower on straights. Corners (grip/lateral) are untouched, so lowering this only helps the no-fly-off invariant.")]
	public float StraightSpeedScale = 1f;

	[Tooltip("Scale applied to per-car Min/Max turn-speed coefficients when baking the speed profile.")]
	public float BakedCoeffScale = 1f;

	[Tooltip("Below this speed (km/h) in Race the bot always full-throttles (personality scales it).")]
	[Header("Following / cruise (BotDriveStateMachine)")]
	public float MinSpeedThreshold = 150f;

	[Tooltip("Base overtake-completion distance unit (× multipliers).")]
	public float MinOvertakeDistance = 6f;

	[Tooltip("Fastest side-avoidance check interval (s) at high speed.")]
	public float MinCheckInterval = 0.1f;

	[Tooltip("Slowest side-avoidance check interval (s) at low speed.")]
	public float MaxCheckInterval = 0.2f;

	[Tooltip("Cruise light-brake band as a fraction of target speed.")]
	public float BrakeSpeedInPercent = 0.1f;

	[Tooltip("Multiplier on the cruise light-brake.")]
	public float AppliedBrakeMultiply = 1f;

	[Tooltip("Above this speed (km/h) the bot brakes in-lane instead of swerving into traffic.")]
	public float HighSpeedSwerveCutoffKmh = 150f;

	[Tooltip("Above this speed (km/h) overtakes require a long, confirmed-clear corridor.")]
	public float HighSpeedOvertakeGateKmh = 145f;

	[Tooltip("Player-ahead distance (m) within which the Rival rams / cuts across.")]
	[Header("Rival dirty sabotage (BotDriveStateMachine)")]
	public float RamRange = 16f;

	[Tooltip("Player-behind distance (m) within which the Rival stabs the brakes.")]
	public float BrakeCheckRange = 11f;

	[Tooltip("Brake-on part of the brake-check stab cycle (s).")]
	public float BrakeCheckPulse = 0.45f;

	[Tooltip("Coast part of the brake-check stab cycle (s).")]
	public float BrakeCheckGap = 0.75f;

	[Tooltip("Player within this many lane-widths sideways → lean & push to the edge.")]
	public float PushLateralRange = 2.2f;

	[Tooltip("\"Alongside\" forward tolerance in car-lengths for the push.")]
	public float PushForwardLen = 1.6f;

	[Header("Rear-end safety backstop (BotDriveStateMachine)")]
	[Tooltip("Universal anti-rear-end brake. Time-to-contact (s) with a car directly ahead below which the bot slams the brakes — works in ALL states, including overtaking. Higher = reacts earlier / more cautious.")]
	public float RearEndSafetyTtc = 1.7f;

	[Tooltip("Bumper-to-bumper gap (m) below which the bot brakes regardless of closing speed. Higher = keeps more space.")]
	public float RearEndSafetyGap = 2.5f;

	[Tooltip("Half-width of the in-path corridor (m). A car ahead whose sideways offset is within this is treated as 'in my lane'. Higher = brakes for cars further to the side. Cars are ~1.8-2 m wide, so below ~1.8 partial overlaps are missed.")]
	public float RearEndPathHalfWidth = 1.9f;

	[Tooltip("Reaction lag (s) budgeted into the physics-based braking distance. Higher = starts braking earlier at any closing speed.")]
	public float RearEndReactionSec = 0.35f;

	[Tooltip("Fraction of the bot's brake capability assumed achievable when computing the required stopping distance. Lower = more conservative (earlier braking).")]
	[Range(0.4f, 1f)]
	public float RearEndBrakeFrac = 0.8f;

	[Range(0f, 1.6f)]
	[Tooltip("Emergency swerve strength in lane-widths when braking distance is no longer enough and a side corridor is free. 0 disables swerving (brake only).")]
	public float RearEndSwerveStrength = 1f;
}
