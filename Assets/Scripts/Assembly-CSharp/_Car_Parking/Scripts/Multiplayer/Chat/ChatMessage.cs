using System;
using System.Text;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	public class ChatMessage
	{
		private const string Separator = "&&";

		private readonly string[] _mainDataSeparatorArray = new string[1] { "&&" };

		private const string AddDataSeparator = "@@";

		private readonly string[] _addDataSeparatorArray = new string[1] { "@@" };

		public readonly ChatType ChatType;

		public readonly ChatMessageType MessageType;

		public readonly string EnteredText;

		public readonly string AdditionData;

		public string FromPlayerId;

		public string ToPlayerId;

		public string FromPlayerName;

		public string ToPlayerName;

		public ChatMessage(ChatType chatType, ChatMessageType messageType, string enteredText, string additionData = "")
		{
			ChatType = chatType;
			MessageType = messageType;
			EnteredText = enteredText;
			AdditionData = additionData;
		}

		public ChatMessage(string messageString)
		{
			if (messageString.StartsWith("&&"))
			{
				string[] array = messageString.Split(_mainDataSeparatorArray, StringSplitOptions.None);
				if (array.Length >= 5)
				{
					if (int.TryParse(array[1], out var result))
					{
						ChatType = (ChatType)result;
					}
					if (int.TryParse(array[2], out var result2))
					{
						MessageType = (ChatMessageType)result2;
					}
					EnteredText = array[3];
					switch (ChatType)
					{
					case ChatType.FullChat:
						AdditionData = array[4];
						GetAdditionalDataForFullAndFriendChat(AdditionData);
						break;
					case ChatType.FriendsChat:
						AdditionData = array[4];
						GetAdditionalDataForFullAndFriendChat(AdditionData);
						break;
					case ChatType.Personal:
						AdditionData = array[4];
						GetAdditionalDataForPersonalChat(AdditionData);
						break;
					default:
						AdditionData = array[4];
						break;
					}
				}
			}
			else
			{
				ChatType = ChatType.FullChat;
				AdditionData = "";
				EnteredText = messageString;
			}
		}

		public override string ToString()
		{
			string[] obj = new string[8] { "&&", null, null, null, null, null, null, null };
			int chatType = (int)ChatType;
			obj[1] = chatType.ToString();
			obj[2] = "&&";
			chatType = (int)MessageType;
			obj[3] = chatType.ToString();
			obj[4] = "&&";
			obj[5] = EnteredText;
			obj[6] = "&&";
			obj[7] = AdditionData;
			return string.Concat(obj);
		}

		public static string SanitizeNameField(string name)
		{
			if (string.IsNullOrEmpty(name))
			{
				return name;
			}
			StringBuilder stringBuilder = new StringBuilder(name.Length);
			foreach (char c in name)
			{
				if (stringBuilder.Length <= 0 || stringBuilder[stringBuilder.Length - 1] != c || (c != '@' && c != '&'))
				{
					stringBuilder.Append(c);
				}
			}
			while (stringBuilder.Length > 0 && stringBuilder[stringBuilder.Length - 1] == '@')
			{
				stringBuilder.Length--;
			}
			return stringBuilder.ToString();
		}

		public static string AdditionalDataForPersonalChat(string fromPlayerId, string fromPlayerName, string toPlayerId, string toPlayerName)
		{
			return fromPlayerId + "@@" + SanitizeNameField(fromPlayerName) + "@@" + toPlayerId + "@@" + SanitizeNameField(toPlayerName) + "@@";
		}

		private void GetAdditionalDataForPersonalChat(string additionalData)
		{
			string[] array = additionalData.Split(_addDataSeparatorArray, StringSplitOptions.None);
			if (array.Length >= 4)
			{
				FromPlayerId = array[0];
				FromPlayerName = array[1];
				ToPlayerId = array[2];
				ToPlayerName = array[3];
			}
		}

		public static string AdditionalDataForFullAndFriendChat(string fromPlayerId, string fromPlayerName)
		{
			return fromPlayerId + "@@" + SanitizeNameField(fromPlayerName);
		}

		private void GetAdditionalDataForFullAndFriendChat(string additionalData)
		{
			string[] array = additionalData.Split(_addDataSeparatorArray, StringSplitOptions.None);
			if (array.Length >= 2)
			{
				FromPlayerId = array[0];
				FromPlayerName = array[1];
			}
		}

		public static string RemoveSeparators(string str)
		{
			str = ChatHelper.RemoveBBCode(str, "&&");
			str = ChatHelper.RemoveBBCode(str, "@@");
			return str;
		}
	}
}
