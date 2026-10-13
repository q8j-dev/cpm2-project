using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.DataStorageService.PlayerData;

public class CircuitRaceMapElement : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI MapTitleText;

	[SerializeField]
	private Image MapImage;

	[SerializeField]
	private TextMeshProUGUI MapDescriptionText;

	[SerializeField]
	private TextMeshProUGUI DirectionText;

	[SerializeField]
	private Button MapButton;

	[SerializeField]
	private Button DirectionButton;

	private CircuitRacingUIControl _circuitRacingUIControl;

	private int _direction;

	private int _index;

	public void Init(int index, List<CircuitRacingRecord> circuitRacingRecords, CircuitRacingUIControl circuitRacingUIControl, int direction)
	{
		_index = index;
		_circuitRacingUIControl = circuitRacingUIControl;
		_direction = direction;
		MapTitleText.text = "Map #" + index;
		Sprite sprite = Resources.Load<Sprite>("CircuitMapUI/" + index);
		if (sprite != null)
		{
			MapImage.sprite = sprite;
		}
		string text = "";
		for (int i = 0; i < circuitRacingRecords.Count; i++)
		{
			text = text + $"\n Class {i} \n #1 place count:{circuitRacingRecords[i].FirstPlace} " + $" #2 place count:{circuitRacingRecords[i].SecondPlace} " + $" #3 place count:{circuitRacingRecords[i].ThirdPlace}";
		}
		MapDescriptionText.text = text;
		MapButton.onClick.RemoveAllListeners();
		SetImageDirection();
		DirectionButton.onClick.RemoveAllListeners();
		DirectionButton.onClick.AddListener(SetDirection);
	}

	private void SetImageDirection()
	{
		DirectionText.text = "Direction:" + _direction;
	}

	private void SetDirection()
	{
		_direction *= -1;
		_circuitRacingUIControl.SetDirection(_index, _direction);
		SetImageDirection();
	}
}
