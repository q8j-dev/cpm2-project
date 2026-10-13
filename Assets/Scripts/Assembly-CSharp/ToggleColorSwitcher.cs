using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToggleColorSwitcher : MonoBehaviour
{
	[SerializeField]
	private Color SelectedBgColor;

	[SerializeField]
	private Color UnSelectedBgColor;

	[SerializeField]
	private Color SelectedTextColor;

	[SerializeField]
	private Color UnSelectedTextColor;

	[SerializeField]
	private Color SelectedSpriteColor;

	[SerializeField]
	private Color UnSelectedSpriteColor;

	[SerializeField]
	private TextMeshProUGUI TextComponent;

	[SerializeField]
	private MaskableGraphic ImageComponent;

	[SerializeField]
	private MaskableGraphic BackgroundImage;

	[SerializeField]
	private Toggle Toggle;

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
