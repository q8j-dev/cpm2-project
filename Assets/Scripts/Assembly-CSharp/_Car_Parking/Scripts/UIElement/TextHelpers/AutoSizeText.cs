using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.UIElement.TextHelpers
{
	[RequireComponent(typeof(Text))]
	public class AutoSizeText : MonoBehaviour
	{
		[SerializeField]
		private RectTransform RectTransform;

		[SerializeField]
		private Text UIText;

		[SerializeField]
		private int MinFontSize = 10;

		[SerializeField]
		private int MaxFontSize = 40;

		private void OnValidate()
		{
			GetNullComponents();
		}

		private void Start()
		{
			GetNullComponents();
			AdjustFontSize();
		}

		private void GetNullComponents()
		{
			if (!UIText)
			{
				UIText = GetComponent<Text>();
			}
			if (!RectTransform)
			{
				RectTransform = GetComponent<RectTransform>();
			}
		}

		private void AdjustFontSize()
		{
			int fontSize = MaxFontSize;
			UIText.resizeTextForBestFit = false;
			for (int num = MaxFontSize; num >= MinFontSize; num--)
			{
				UIText.fontSize = num;
				if (UIText.preferredHeight <= RectTransform.rect.height && UIText.preferredWidth <= RectTransform.rect.width)
				{
					fontSize = num;
					break;
				}
			}
			UIText.fontSize = fontSize;
		}
	}
}
