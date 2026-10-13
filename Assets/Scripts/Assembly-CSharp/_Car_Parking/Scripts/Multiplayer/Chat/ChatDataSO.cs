using System.Linq;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	[CreateAssetMenu(fileName = "ChatData", menuName = "Chat/ChatData", order = 0)]
	public class ChatDataSO : ScriptableObject
	{
		public PersonalChatData[] PersonalChatDataCollection;

		public SmileData[] SmileDataCollection;

		public PersonalChatData GetPersonalChatData(ChatMessageType chatMessageType)
		{
			return PersonalChatDataCollection.FirstOrDefault((PersonalChatData data) => data.ChatMessageType == chatMessageType);
		}

		public Sprite GetSmileById(int id)
		{
			if (id < 0 || SmileDataCollection.Length == 0)
			{
				return null;
			}
			return (from data in SmileDataCollection
				where data.Id == id
				select data.SmileSprite).FirstOrDefault();
		}
	}
}
