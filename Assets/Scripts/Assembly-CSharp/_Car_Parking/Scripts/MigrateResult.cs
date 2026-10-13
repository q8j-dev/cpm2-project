namespace _Car_Parking.Scripts
{
	public class MigrateResult
	{
		public readonly bool IsMigrated;

		public readonly string Message;

		public readonly MigrationMessageType MigrationMessageType;

		public bool IsCorrectAnswer => MigrationMessageType == MigrationMessageType.Success;

		public MigrateResult(MigrationMessageType migrationMessageType, bool isMigrated = false, string message = "")
		{
			MigrationMessageType = migrationMessageType;
			IsMigrated = isMigrated;
			string message2;
			if (string.IsNullOrEmpty(message))
			{
				int num = (int)migrationMessageType;
				message2 = num.ToString();
			}
			else
			{
				message2 = message;
			}
			Message = message2;
		}
	}
}
