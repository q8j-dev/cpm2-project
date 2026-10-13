namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public sealed class ArenaReturnContext
	{
		public ArenaReturnTarget Target;

		public int PreviousSpawnPlace;

		public string PreviousRoomName = "";

		public static ArenaReturnContext FromMainMenu()
		{
			return new ArenaReturnContext
			{
				Target = ArenaReturnTarget.MainMenu
			};
		}

		public static ArenaReturnContext FromFreeDriveRoom(int prevSpawn)
		{
			return new ArenaReturnContext
			{
				Target = ArenaReturnTarget.FreeDriveRoom,
				PreviousSpawnPlace = prevSpawn
			};
		}
	}
}
