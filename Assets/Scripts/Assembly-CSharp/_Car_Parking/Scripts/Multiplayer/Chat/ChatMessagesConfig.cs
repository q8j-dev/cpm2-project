using UnityEngine;

namespace _Car_Parking.Scripts.Multiplayer.Chat
{
	[CreateAssetMenu(fileName = "ChatMessagesConfig", menuName = "Scriptable/ChatMessagesConfig")]
	public class ChatMessagesConfig : ScriptableObject
	{
		[Header("World chat messages")]
		public GameObject messagePrefab;

		[Header("Char in line")]
		public int maxNextLineIndex = 20;

		[Header("Time to disappear message")]
		public float timeForMessage = 5f;

		[Header("Messages Fade")]
		public float worldMessageFadeTime = 0.5f;

		[Header("World message background color")]
		public Color globalMessageColor = Color.black;

		public Color friendsMessageColor = Color.blue;

		public Color personalMessageColor = Color.yellow;

		[Header("Chat show types")]
		public bool showMeMyMessages;

		public bool showGlobalMessages = true;

		public bool showFriendsMessages = true;

		public bool showPersonalMessages = true;

		[Header("Chat type sprites")]
		public Sprite globalChatSprite;

		public Sprite friendsChatSprite;

		public Sprite personalChatSprite;

		[Header("Height position of message")]
		public float highCarWorldMessageHeight = 1.75f;

		public float classicCarWorldMessageHeight = 0.75f;

		public float personCarWorldMessageHeight = 2.3f;

		public int[] highCarIds;

		[Header("Click on world message")]
		public bool openChatOnClickMessage = true;
	}
}
