using CPMEngine;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TyreTypeUIElement : MonoBehaviour
{
	[SerializeField]
	private Toggle WheelToggle = default;

	[SerializeField]
	private RacingCarChangeTyreWindow RacingCarChangeTyreWindow = default;

	[SerializeField]
	private TextMeshProUGUI TyreTypeText = default;

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
