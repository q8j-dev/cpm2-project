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
		private Button SelectionButton = default;

		[SerializeField]
		private TextMeshProUGUI BestTimeText = default;

		[SerializeField]
		private TextMeshProUGUI FirstPlaceCount = default;

		[SerializeField]
		private TextMeshProUGUI FirstPlaceReward = default;

		[SerializeField]
		private TextMeshProUGUI SecondPlaceCount = default;

		[SerializeField]
		private TextMeshProUGUI SecondPlaceReward = default;

		[SerializeField]
		private TextMeshProUGUI ThirdPlaceCount = default;

		[SerializeField]
		private TextMeshProUGUI ThirdPlaceReward = default;

		[SerializeField]
		private TextMeshProUGUI Class = default;

		[SerializeField]
		private GameObject DefaultBackground = default;

		[SerializeField]
		private GameObject BlockedBackground = default;

		[SerializeField]
		private Image GroupIcon = default;

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
