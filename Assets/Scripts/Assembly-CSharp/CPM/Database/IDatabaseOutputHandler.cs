namespace CPM.Database
{
	public interface IDatabaseOutputHandler
	{
		void OnProgress(float progress);

		void OnMessage(string message);
	}
}
