using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.Settings
{
	public class InputBindingsGroupSection : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI TitleLabel = default;

		[field: SerializeField]
		public RectTransform Content { get; private set; }

		public void SetTitle(string title)
		{
			if ((bool)TitleLabel)
			{
				TitleLabel.text = title;
			}
		}
	}
}
