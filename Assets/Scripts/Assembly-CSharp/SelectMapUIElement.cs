using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SelectMapUIElement : MonoBehaviour
{
	[SerializeField]
	private Toggle SelectToggle;

	[SerializeField]
	private Image CupImage = default;

	[SerializeField]
	private int MapIndex;

	[SerializeField]
	private TextMeshProUGUI TrackName = default;

	private CircuitRaceSelectMapWindow _сircuitRaceSelectMapWindow;

	private void Start()
	{
		SelectToggle = GetComponent<Toggle>();
		if ((bool)SelectToggle)
		{
			SelectToggle.onValueChanged.AddListener(OnMapElementSelected);
			SelectToggle.group = base.gameObject.transform.parent.GetComponent<ToggleGroup>();
		}
	}

	public void Init(int mapIndex, string trackName, CircuitRaceSelectMapWindow circuitRaceSelectMapWindow)
	{
		MapIndex = mapIndex;
		if ((bool)TrackName)
		{
			TrackName.text = trackName;
		}
		_сircuitRaceSelectMapWindow = circuitRaceSelectMapWindow;
	}

	public void SetCupImageColor(Color color)
	{
		CupImage.color = color;
	}

	public void SetOnWithoutNotify(bool isOn)
	{
		SelectToggle.SetIsOnWithoutNotify(isOn);
	}

	private void OnMapElementSelected(bool state)
	{
		if (state)
		{
			_сircuitRaceSelectMapWindow.OnMapSelected(MapIndex);
		}
	}
}
