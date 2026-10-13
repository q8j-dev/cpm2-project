using TMPro;
using UnityEngine;

public class QualificationResultPanel : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI messageText;

	public void Init(string text)
	{
		messageText.text = text;
	}
}
