using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TyreTypeUIElement : MonoBehaviour
{
	[SerializeField]
	private Toggle WheelToggle;

	[SerializeField]
	private RacingCarChangeTyreWindow RacingCarChangeTyreWindow;

	[SerializeField]
	private TextMeshProUGUI TyreTypeText;

	public TyreType TyreType;

	private void Start()
	{
		WheelToggle.onValueChanged.AddListener(OnValueChanged);
		TyreTypeText.text = TyreType.ToString();
	}

	public void Select(bool state)
	{
		WheelToggle.SetIsOnWithoutNotify(state);
	}

	private void OnValueChanged(bool state)
	{
		if (state)
		{
			RacingCarChangeTyreWindow.ChangeTyre(TyreType);
		}
	}
}
