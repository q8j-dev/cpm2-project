using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public abstract class ResultRowElement<TRow> : MonoBehaviour where TRow : IResultRow
	{
		[SerializeField]
		private TMP_Text PlaceText = default;

		[SerializeField]
		private Text NickNameText = default;

		[SerializeField]
		private Image BGImage = default;

		[SerializeField]
		private Image PlaceImage = default;

		[Header("Palette")]
		[SerializeField]
		private ResultRowColors FirstPlaceColor = default;

		[SerializeField]
		private ResultRowColors SecondPlaceColor = default;

		[SerializeField]
		private ResultRowColors ThirdPlaceColor = default;

		[SerializeField]
		private ResultRowColors YourPlaceColor = default;

		[SerializeField]
		private ResultRowColors DefaultColor = default;

		public void Bind(int place, in TRow row)
		{
			if ((bool)PlaceText)
			{
				PlaceText.text = place.ToString();
			}
			if ((bool)NickNameText)
			{
				NickNameText.text = row.UserName;
			}
			ApplyPalette(place, row.IsLocal);
			OnBind(in row);
		}

		public void Show(bool visible)
		{
			base.gameObject.SetActive(visible);
		}

		protected abstract void OnBind(in TRow row);

		protected void SetNickName(string value)
		{
			if ((bool)NickNameText)
			{
				NickNameText.text = value;
			}
		}

		private void ApplyPalette(int place, bool isLocal)
		{
			ResultRowColors resultRowColors = (isLocal ? YourPlaceColor : DefaultColor);
			switch (place)
			{
			case 1:
				resultRowColors = FirstPlaceColor;
				break;
			case 2:
				resultRowColors = SecondPlaceColor;
				break;
			case 3:
				resultRowColors = ThirdPlaceColor;
				break;
			}
			if ((bool)BGImage)
			{
				BGImage.color = resultRowColors.BG;
			}
			if ((bool)PlaceImage)
			{
				PlaceImage.color = resultRowColors.Place;
			}
		}
	}
}
