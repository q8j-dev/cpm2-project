using UnityEngine;

[CreateAssetMenu(fileName = "BotPersonality", menuName = "Scriptable/Racing/Bot Personality", order = 7)]
public class BotPersonalitySO : ScriptableObject
{
	[Header("Identity")]
	[Tooltip("Shown in editor telemetry so you can tell who is who in Play. Empty → asset name.")]
	public string ArchetypeName = "Custom";

	[Header("Braking / cornering style")]
	[Tooltip(">1 brakes earlier and farther from the corner; <1 brakes later / dives in. Extremes <0.78 overshoot corner entry (run wide) — that's the amplified reckless character.")]
	[Range(0.7f, 1.45f)]
	public float BrakeEarliness = 1f;

	[Tooltip("Multiplier on the planner's CornerMarginCoef. >1 = more committed, but the cornering factor is clamped to ≤0.92 downstream (grip limit) so the fast side saturates; the timid side (<1) makes a bot visibly slower through corners.")]
	[Range(0.72f, 1.2f)]
	public float CornerConfidence = 1f;

	[Tooltip("Apex bravery — multiplier on the planner's global TurnSpeedScale. Up-side saturates at the 0.92 grip cap; down-side slows the bot through corners.")]
	[Range(0.82f, 1.15f)]
	public float TurnSpeedScale = 1f;

	[Tooltip("Base accel/top-speed multiplier — the clearest way to make bots visibly separate over a lap. PaceSpreadFrac still jitters on top so clones differ.")]
	[Range(0.68f, 1.2f)]
	public float PaceBias = 1f;

	[Tooltip("Aggression toward OTHER BOTS (traffic): tighter following, lower overtake threshold.")]
	[Range(0f, 1f)]
	[Header("Aggression (used from the state-machine iteration)")]
	public float Aggression = 0.3f;

	[Tooltip("Extra aggression specifically toward the human player (attack: hound, dive-bomb).")]
	[Range(0f, 1f)]
	public float PlayerAggression;

	[Range(0f, 1f)]
	[Tooltip("Willingness/strength to block the player defensively (hold the line in front).")]
	public float PlayerDefense;

	[Tooltip("DIRTY nemesis behaviour toward the player: brake-check (stab the brakes) when directly in front of the player, ram them when behind, push them to the edge when alongside. 0 = clean.")]
	[Range(0f, 1f)]
	public float PlayerSabotage;

	[Header("Skill / racing line")]
	[Tooltip("1 = flawless. Below 1 injects wandering corner-speed errors — occasional too-hot (runs wide) or too-slow moments. Low = amateur/beatable; high = machine-precise.")]
	[Range(0.4f, 1f)]
	public float Consistency = 1f;

	[Range(0.6f, 1.4f)]
	[Tooltip("Racing-line tightness. >1 cuts the apex tighter (aggressive line), <1 takes a wider, safer line. Visible as different trajectories through corners.")]
	public float ApexAggression = 1f;

	[Range(120f, 200f)]
	[Tooltip("Per-bot high-speed overtake caution gate (km/h). Below this, overtakes are less cautious.")]
	public float HighSpeedOvertakeGateKmh = 145f;

	public BotPersonality Resolve()
	{
		return new BotPersonality
		{
			ArchetypeName = (string.IsNullOrEmpty(ArchetypeName) ? base.name : ArchetypeName),
			BrakeEarliness = Mathf.Clamp(BrakeEarliness, 0.68f, 1.48f),
			CornerConfidence = Mathf.Clamp(CornerConfidence, 0.7f, 1.22f),
			TurnSpeedScale = Mathf.Clamp(TurnSpeedScale, 0.8f, 1.18f),
			PaceBias = Mathf.Clamp(PaceBias, 0.66f, 1.22f),
			Aggression = Mathf.Clamp01(Aggression),
			PlayerAggression = Mathf.Clamp01(PlayerAggression),
			PlayerDefense = Mathf.Clamp01(PlayerDefense),
			PlayerSabotage = Mathf.Clamp01(PlayerSabotage),
			Consistency = Mathf.Clamp(Consistency, 0.35f, 1f),
			ApexAggression = Mathf.Clamp(ApexAggression, 0.55f, 1.45f),
			HighSpeedOvertakeGateKmh = Mathf.Clamp(HighSpeedOvertakeGateKmh, 90f, 230f)
		};
	}
}
