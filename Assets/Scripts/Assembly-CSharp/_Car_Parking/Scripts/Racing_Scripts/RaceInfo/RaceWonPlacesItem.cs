using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Car_Parking.Scripts.Racing_Scripts.RaceInfo
{
	public class RaceWonPlacesItem : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI FirstPlacesCount = default;

		[SerializeField]
		private TextMeshProUGUI SecondPlacesCount = default;

		[SerializeField]
		private TextMeshProUGUI ThirdPlacesCount = default;

		[SerializeField]
		private bool ShowStatusIcon = true;

		[SerializeField]
		private Image StatusIconImage = default;

		[SerializeField]
		private Sprite DoneIcon = default;

		[SerializeField]
		private Sprite DefaultIcon = default;

		[field: SerializeField]
		public int TimeClass { get; private set; }

		public void SetPlacesCount(int firstPlacesCount, int secondPlacesCount, int thirdPlacesCount)
		{
			FirstPlacesCount.text = ((firstPlacesCount > 0) ? firstPlacesCount.ToString() : "0");
			SecondPlacesCount.text = ((secondPlacesCount > 0) ? secondPlacesCount.ToString() : "0");
			ThirdPlacesCount.text = ((thirdPlacesCount > 0) ? thirdPlacesCount.ToString() : "0");
			if (ShowStatusIcon)
			{
				StatusIconImage.sprite = ((firstPlacesCount > 0) ? DoneIcon : DefaultIcon);
			}
		}
	}
}
