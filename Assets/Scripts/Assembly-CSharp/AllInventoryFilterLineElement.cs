using TMPro;
using UnityEngine;

public class AllInventoryFilterLineElement : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI TitleText;

	public void SetText(string text)
	{
		TitleText.text = text;
	}
}
