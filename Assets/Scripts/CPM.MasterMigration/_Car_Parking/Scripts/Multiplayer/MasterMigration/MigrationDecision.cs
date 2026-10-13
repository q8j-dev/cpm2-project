namespace _Car_Parking.Scripts.Multiplayer.MasterMigration
{
	public readonly struct MigrationDecision
	{
		public readonly bool ShouldMigrate;

		public readonly PlayerKey Target;

		public readonly string Reason;

		private MigrationDecision(bool shouldMigrate, PlayerKey target, string reason)
		{
			ShouldMigrate = shouldMigrate;
			Target = target;
			Reason = reason;
		}

		public static MigrationDecision None(string reason)
		{
			return new MigrationDecision(shouldMigrate: false, default, reason);
		}

		public static MigrationDecision MigrateTo(PlayerKey target, string reason)
		{
			return new MigrationDecision(shouldMigrate: true, target, reason);
		}
	}
}
