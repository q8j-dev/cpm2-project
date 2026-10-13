using System;

namespace _Car_Parking.Scripts.Multiplayer.Arena
{
	public static class ArenaRoomNames
	{
		private const char Separator = '_';

		private const char ArenaMarker = 'a';

		public static string AppendArenaSuffix(string roomName, ArenaId arenaId)
		{
			if (arenaId != ArenaId.Free)
			{
				int num = (int)arenaId;
				return roomName + "_a" + num;
			}
			return roomName;
		}

		public static string BuildFriendRoomName(string ownerId, int password, string instance, ArenaId arenaId)
		{
			string text = ownerId + "_" + password;
			if (!string.IsNullOrEmpty(instance))
			{
				text = text + "_" + instance;
			}
			return AppendArenaSuffix(text, arenaId);
		}

		public static string GenerateInstance()
		{
			return BitConverter.ToUInt32(Guid.NewGuid().ToByteArray(), 0).ToString();
		}

		public static string BuildRematchRoomName(string roomName, ArenaId arenaId)
		{
			if (string.IsNullOrEmpty(roomName))
			{
				return null;
			}
			string[] array = roomName.Split('_');
			if (array.Length < 2)
			{
				return null;
			}
			if (!int.TryParse(array[1], out var result))
			{
				return null;
			}
			return BuildFriendRoomName(array[0], result, StableInstance(roomName), arenaId);
		}

		private static string StableInstance(string source)
		{
			uint num = 2166136261u;
			foreach (char c in source)
			{
				num ^= c;
				num *= 16777619;
			}
			return num.ToString();
		}

		public static bool TryParseArenaId(string roomName, out ArenaId arenaId)
		{
			arenaId = ArenaId.Free;
			if (string.IsNullOrEmpty(roomName))
			{
				return false;
			}
			string[] array = roomName.Split('_');
			if (array.Length < 3)
			{
				return false;
			}
			string text = array[array.Length - 1];
			if (text.Length < 2 || text[0] != 'a')
			{
				return false;
			}
			if (!int.TryParse(text.Substring(1), out var result))
			{
				return false;
			}
			arenaId = (ArenaId)result;
			return arenaId != ArenaId.Free;
		}

		public static int ParseFriendRoomPassword(string roomName)
		{
			if (string.IsNullOrEmpty(roomName))
			{
				return 0;
			}
			string[] array = roomName.Split('_');
			if (array.Length < 2)
			{
				return 0;
			}
			if (!int.TryParse(array[1], out var result))
			{
				return 0;
			}
			if (result >= 0 && result <= 10000)
			{
				return result;
			}
			return 0;
		}
	}
}
