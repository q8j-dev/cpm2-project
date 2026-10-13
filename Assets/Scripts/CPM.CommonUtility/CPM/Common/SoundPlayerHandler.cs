using UnityEngine;
using UnityEngine.Scripting;

namespace CPM.Common
{
	[Preserve]
	public class SoundPlayerHandler : ISoundPlayerHandler
	{
		private GameObject gameObject;

		public SoundPlayerHandler()
		{
			gameObject = Object.Instantiate(new GameObject("SoundHandlerObject"));
			Object.DontDestroyOnLoad(gameObject);
		}

		public void MessageReceived()
		{
			PlaySound("Chat_Massege");
		}

		public void MessageSent()
		{
			PlaySound("Chat_Received");
		}

		public void RequestReceived()
		{
			PlaySound("Chat_FriendRequest");
		}

		private void PlaySound(string switchState)
		{
			Debug.Log("Play Sound: " + switchState);
			AkSoundEngine.SetSwitch("UiSwitch", switchState, gameObject);
			AkSoundEngine.PostEvent("Ui_Event", gameObject);
		}
	}
}
