using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
	[SerializeField]
	private Image img;

	[SerializeField]
	private Sprite activeSprite;

	[SerializeField]
	private Sprite deactivateSprite;

	[SerializeField]
	private TextMeshProUGUI label;

	[SerializeField]
	private Color highlightedColor;

	[SerializeField]
	private Color fadedColor;

	[SerializeField]
	private float activePixelsPerUnit = 1f;

	[SerializeField]
	private float deactivatePixelsPerUnit = 3f;

	private bool _state;

	public void ChangeState()
	{
		_state = !_state;
		ChangeState(_state);
	}

	public void ChangeState(bool state)
	{
		if ((bool)img)
		{
			img.sprite = (state ? activeSprite : deactivateSprite);
			img.pixelsPerUnitMultiplier = (state ? activePixelsPerUnit : deactivatePixelsPerUnit);
		}
		label.color = (state ? highlightedColor : fadedColor);
	}
}
