public struct BotPersonality
{
	public string ArchetypeName;

	public float BrakeEarliness;

	public float CornerConfidence;

	public float TurnSpeedScale;

	public float PaceBias;

	public float Aggression;

	public float PlayerAggression;

	public float PlayerDefense;

	public float PlayerSabotage;

	public float Consistency;

	public float ApexAggression;

	public float HighSpeedOvertakeGateKmh;

	public static BotPersonality Neutral => new BotPersonality
	{
		ArchetypeName = "Neutral",
		BrakeEarliness = 1f,
		CornerConfidence = 1f,
		TurnSpeedScale = 1f,
		PaceBias = 1f,
		Aggression = 0.3f,
		PlayerAggression = 0f,
		PlayerDefense = 0f,
		PlayerSabotage = 0f,
		Consistency = 1f,
		ApexAggression = 1f,
		HighSpeedOvertakeGateKmh = 145f
	};
}
