using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleColorSwitcher : MonoBehaviour
{
	[SerializeField]
	private Color SelectedBgColor = default;

	[SerializeField]
	private Color UnSelectedBgColor = default;

	[SerializeField]
	private Color SelectedTextColor = default;

	[SerializeField]
	private Color UnSelectedTextColor = default;

	[SerializeField]
	private Color SelectedSpriteColor = default;

	[SerializeField]
	private Color UnSelectedSpriteColor = default;

	[SerializeField]
	private TextMeshProUGUI TextComponent = default;

	[SerializeField]
	private MaskableGraphic ImageComponent = default;

	[SerializeField]
	private MaskableGraphic BackgroundImage = default;

	[SerializeField]
	private Toggle Toggle = default;

	private void OnEnable()
	{
		OnSelected(Toggle.isOn);
	}

	public void OnSelected(bool state)
	{
		if ((bool)TextComponent)
		{
			TextComponent.color = (state ? SelectedTextColor : UnSelectedTextColor);
		}
		if ((bool)ImageComponent)
		{
			ImageComponent.color = (state ? SelectedSpriteColor : UnSelectedSpriteColor);
		}
		if ((bool)BackgroundImage)
		{
			BackgroundImage.color = (state ? SelectedBgColor : UnSelectedBgColor);
		}
	}
}
