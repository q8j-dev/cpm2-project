using TMPro;
using UnityEngine;

public class AllInventoryFilterLineElement : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI TitleText = default;

	public void SetText(string text)
	{
		TitleText.text = text;
	}
}
