using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement
{
	public class ChartListElementView : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI PlaceNumberText = default;

		[SerializeField]
		private Text UserNameText = default;

		[SerializeField]
		private TextMeshProUGUI ValueText = default;

		[SerializeField]
		private RawImage RankImage = default;

		[Header("Player highlight")]
		[SerializeField]
		private GameObject[] PlayerGraphics = default;

		[SerializeField]
		private GameObject[] NormalGraphics = default;

		[SerializeField]
		private Color PlayerTextColor = Color.black;

		[SerializeField]
		private Color DefaultTextColor = Color.white;

		public void Bind(in ChartListRowData data)
		{
			TextMeshProUGUI placeNumberText = PlaceNumberText;
			int place = data.Place;
			placeNumberText.text = place.ToString();
			UserNameText.text = data.UserName;
			if ((bool)RankImage)
			{
				RankImage.texture = data.RankIcon;
			}
			if ((bool)ValueText)
			{
				ValueText.text = data.ValueText;
			}
			ApplyPlayerState(data.IsUser);
		}

		public void Show(bool visible)
		{
			base.gameObject.SetActive(visible);
		}

		private void ApplyPlayerState(bool isUser)
		{
			PlayerGraphics.SetActiveStatusAll(isUser);
			NormalGraphics.SetActiveStatusAll(!isUser);
			Color color = (isUser ? PlayerTextColor : DefaultTextColor);
			PlaceNumberText.color = color;
			UserNameText.color = color;
			if ((bool)ValueText)
			{
				ValueText.color = color;
			}
		}
	}
}
