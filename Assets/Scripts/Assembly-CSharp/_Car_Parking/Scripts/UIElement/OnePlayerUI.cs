using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using _Car_Parking.Scripts.Other.Helpers;

namespace _Car_Parking.Scripts.UIElement
{
	public class OnePlayerUI : MonoBehaviour
	{
		[SerializeField]
		private Text NameLabel;

		[SerializeField]
		private Image ImageRank;

		[SerializeField]
		private Button PlayerButton;

		public void Init(OnePlayerInListData onePlayer, UnityAction onPlayerClicked = null)
		{
			base.transform.localScale = Vector3.one;
			NameLabel.text = onePlayer.PlayerName;
			int rankType = onePlayer.MultiPersonControl.rank.GetValueOrDefault();
			ImageRank.sprite = UtilsRank.LoadRankSpriteFromResources((UtilsRank.RankType)rankType);
			if (onPlayerClicked != null)
			{
				PlayerButton.onClick.AddListener(onPlayerClicked);
			}
		}
	}
}
