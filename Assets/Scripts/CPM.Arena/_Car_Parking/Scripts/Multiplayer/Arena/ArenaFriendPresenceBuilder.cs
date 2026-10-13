using System.Collections.Generic;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaFriendPresenceBuilder
	{
		public static List<ArenaFriendPresence> Build(IEnumerable<ArenaFriendRoomInput> friendsInRooms, IReadOnlyDictionary<string, ArenaRoomStatus> roomsByName, IReadOnlyDictionary<string, string> nameByUserId, ArenaId targetArena)
		{
			List<ArenaFriendPresence> list = new List<ArenaFriendPresence>();
			if (friendsInRooms == null)
			{
				return list;
			}
			foreach (ArenaFriendRoomInput friendsInRoom in friendsInRooms)
			{
				if (ArenaRoomNames.TryParseArenaId(friendsInRoom.RoomName, out var arenaId) && arenaId == targetArena && roomsByName != null && roomsByName.TryGetValue(friendsInRoom.RoomName, out var value))
				{
					string value2 = null;
					nameByUserId?.TryGetValue(friendsInRoom.UserId, out value2);
					if (string.IsNullOrEmpty(value2))
					{
						value2 = friendsInRoom.UserId;
					}
					int password = ArenaRoomNames.ParseFriendRoomPassword(friendsInRoom.RoomName);
					list.Add(new ArenaFriendPresence(friendsInRoom.UserId, value2, friendsInRoom.RoomName, value.PlayerCount, value.MaxPlayers, value.IsOpen, password));
				}
			}
			return list;
		}
	}
}
