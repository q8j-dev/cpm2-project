using CPM.PlayerData;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.UIElement
{
	public class PlayerListWindowPlayerUserView : MonoBehaviour
	{
		private const int MaxNameTextLength = 20;

		[SerializeField]
		private Image RankIcon = default;

		[SerializeField]
		private TMP_Text RankText = default;

		[SerializeField]
		private Text NameText = default;

		[SerializeField]
		private TMP_Text IDText = default;

		private PlayerDataStorage _playerDataStorage;

		[Inject]
		public void Construct(PlayerDataStorage playerDataStorage)
		{
			_playerDataStorage = playerDataStorage;
		}

		public void UpdateView()
		{
			RankIcon.sprite = UtilsRank.LoadMyRankSpriteFromResources();
			RankText.text = string.Format("{0}:{1}", new LocalizedString("PlayerList.Rating"), UtilsRank.GetMyRankName());
			string text = _playerDataStorage.PlayerName;
			if (text.Length > 20)
			{
				text = text.Substring(0, 20);
				text += "...";
			}
			NameText.text = text;
			IDText.text = $"ID:{_playerDataStorage.PlayerID}";
		}

		private void OnEnable()
		{
			UpdateView();
		}
	}
}
