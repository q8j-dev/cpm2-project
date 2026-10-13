using TMPro;
using UnityEngine;

namespace _Car_Parking.Scripts.Drag
{
	public class TuningTimerElement : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI DetailTypeText = default;

		[SerializeField]
		private TextMeshProUGUI DetailNameText = default;

		[SerializeField]
		private TextMeshProUGUI TimeText = default;

		[SerializeField]
		private GameObject DoneIcon = default;

		public void Init(string detailType, string detailName, string detailTime, bool isReady = false)
		{
			DetailTypeText.text = detailType;
			DetailNameText.text = detailName;
			TimeText.text = detailTime;
			if (isReady)
			{
				TimeText.gameObject.SetActive(value: false);
				ElementDone();
			}
		}

		private void ElementDone()
		{
			DoneIcon.SetActive(value: true);
		}
	}
}
