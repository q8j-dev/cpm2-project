using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SmoothFill : MonoBehaviour
{
	[SerializeField]
	private Image sprite;

	[SerializeField]
	private float targetFill;

	[SerializeField]
	private float speed = 1f;

	[SerializeField]
	private TextMeshProUGUI CenterText;

	private void Start()
	{
	}

	public void SetValue(float fillVal, string text = "")
	{
		targetFill = fillVal;
		if ((bool)CenterText)
		{
			CenterText.text = text;
		}
	}

	private void Update()
	{
		if ((bool)sprite && sprite.fillAmount != targetFill)
		{
			sprite.fillAmount = Mathf.MoveTowards(sprite.fillAmount, targetFill, Time.deltaTime * speed);
		}
	}
}
