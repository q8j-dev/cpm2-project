namespace _Car_Parking.Scripts.DataStorageService
{
	public static class PlayerDataExtensions
	{
		public static PlayerStorage Read(ES3File file)
		{
			return file.Load<PlayerStorage>("PlayerData");
		}
	}
}
