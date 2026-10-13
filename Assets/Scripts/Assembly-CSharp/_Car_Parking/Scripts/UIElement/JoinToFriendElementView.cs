using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Multiplayer.Arena;

namespace _Car_Parking.Scripts.UIElement
{
	public class JoinToFriendElementView : MonoBehaviour
	{
		[SerializeField]
		private Text PlayerName;

		[SerializeField]
		private TMP_Text PlayerCountText;

		[SerializeField]
		private Button JoinButton;

		[SerializeField]
		private TMP_InputField PasswordInput;

		private int _password;

		private bool _joinable;

		public string RoomName { get; private set; }

		public event Action<JoinToFriendElementView> OnJoinConfirmed;

		private void OnEnable()
		{
			JoinButton.onClick.AddListener(OnJoinButtonClicked);
		}

		private void OnDisable()
		{
			JoinButton.onClick.RemoveListener(OnJoinButtonClicked);
		}

		public void SetPresence(ArenaFriendPresence p)
		{
			RoomName = p.RoomName;
			_password = p.Password;
			if ((bool)PlayerName)
			{
				PlayerName.text = p.DisplayName;
			}
			ApplyStatus(p);
			if ((bool)PasswordInput)
			{
				PasswordInput.gameObject.SetActive(value: false);
				PasswordInput.text = "";
			}
		}

		public void RefreshStatus(ArenaFriendPresence p)
		{
			ApplyStatus(p);
		}

		private void ApplyStatus(ArenaFriendPresence p)
		{
			if ((bool)PlayerCountText)
			{
				PlayerCountText.text = string.Format(new LocalizedString("Lobby.Players"), p.PlayerCount, p.MaxPlayers);
			}
			bool flag = p.PlayerCount >= p.MaxPlayers;
			_joinable = p.IsOpen && !flag;
			JoinButton.interactable = _joinable;
		}

		private void OnJoinButtonClicked()
		{
			if (_joinable)
			{
				int result;
				if (_password == 0)
				{
					OnJoinConfirmed?.Invoke(this);
				}
				else if ((bool)PasswordInput && !PasswordInput.gameObject.activeSelf)
				{
					PasswordInput.gameObject.SetActive(value: true);
				}
				else if ((bool)PasswordInput && int.TryParse(PasswordInput.text, out result) && result == _password)
				{
					OnJoinConfirmed?.Invoke(this);
				}
				else if ((bool)PasswordInput)
				{
					PasswordInput.text = "";
				}
			}
		}
	}
}
