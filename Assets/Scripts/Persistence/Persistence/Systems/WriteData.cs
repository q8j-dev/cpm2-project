namespace Persistence.Systems
{
	public struct WriteData
	{
		public string path;

		public string json;

		public WriteData(string json, string path)
		{
			this.json = json;
			this.path = path;
		}
	}
}
