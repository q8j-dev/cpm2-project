namespace CPM.Ghost
{
	public sealed class NullGhostLogger : IGhostLogger
	{
		public static readonly NullGhostLogger Instance = new NullGhostLogger();

		private NullGhostLogger()
		{
		}

		public void Log(string message)
		{
		}

		public void Warn(string message)
		{
		}

		public void Error(string message)
		{
		}
	}
}
