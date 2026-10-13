using System;
using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	[Serializable]
	public class PersonalChatData
	{
		public ChatMessageType ChatMessageType;

		public string ServiceName;

		public Sprite ServiceIcon;

		public Color ServiceColor;
	}
}
