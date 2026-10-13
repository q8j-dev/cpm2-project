using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.CustomDataTypes;
using _Car_Parking.Scripts.DataStorageService.PlayerData;

namespace _Car_Parking.Scripts.LapRaceMode.UI
{
	public class RaceGroupItem : MonoBehaviour
	{
		[SerializeField]
		private Button SelectionButton;

		[SerializeField]
		private TextMeshProUGUI BestTimeText;

		[SerializeField]
		private TextMeshProUGUI FirstPlaceCount;

		[SerializeField]
		private TextMeshProUGUI FirstPlaceReward;

		[SerializeField]
		private TextMeshProUGUI SecondPlaceCount;

		[SerializeField]
		private TextMeshProUGUI SecondPlaceReward;

		[SerializeField]
		private TextMeshProUGUI ThirdPlaceCount;

		[SerializeField]
		private TextMeshProUGUI ThirdPlaceReward;

		[SerializeField]
		private TextMeshProUGUI Class;

		[SerializeField]
		private GameObject DefaultBackground;

		[SerializeField]
		private GameObject BlockedBackground;

		[SerializeField]
		private Image GroupIcon;

		public void Init(CircuitRacingRecord record, int[] rewards, int id, CircuitRaceSelectMapWindow circuitRacingUIControl)
		{
			BestTimeText.text = record.BestTime.ToString();
			FirstPlaceCount.text = "x" + record.FirstPlace;
			SecondPlaceCount.text = "x" + record.SecondPlace;
			ThirdPlaceCount.text = "x" + record.ThirdPlace;
			FirstPlaceReward.text = rewards[0].ToString();
			SecondPlaceReward.text = rewards[1].ToString();
			ThirdPlaceReward.text = rewards[2].ToString();
			TextMeshProUGUI textMeshProUGUI = Class;
			VehicleClass vehicleClass = (VehicleClass)id;
			textMeshProUGUI.text = vehicleClass.ToString();
			SelectionButton.onClick.AddListener(() =>
			{
				circuitRacingUIControl.SelectClassIndex(new AdvancedInt(id));
			});
		}

		public void Block()
		{
			SelectionButton.interactable = false;
			DefaultBackground.SetActive(value: false);
			BlockedBackground.SetActive(value: true);
		}

		public Sprite GetGroupIcon()
		{
			return GroupIcon.sprite;
		}
	}
}
