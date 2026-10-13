namespace CPM.Ghost
{
	public interface IGhostLogger
	{
		void Log(string message);

		void Warn(string message);

		void Error(string message);
	}
}
