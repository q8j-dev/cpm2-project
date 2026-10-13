using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.PhotoMode.UI
{
	public class PhotoModeScrollItemWithText : PhotoModeScrollItem
	{
		[SerializeField]
		private TextMeshProUGUI Text;

		private void Start()
		{
			Text.text = $"{TextBefore}{Value}{TextAfter}";
		}

		public void SetTextColorAlpha(float alpha)
		{
			Color color = Text.color;
			color.a = alpha;
			Text.color = color;
		}
	}
}
