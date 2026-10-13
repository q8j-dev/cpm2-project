using UnityEngine;

public class RealtimeAnalyze : MonoBehaviour
{
	public bool yourBool;

	public TextMesh textLabel;

	public CircuitCarStearing Stearing;

	private void Start()
	{
		if (textLabel == null)
		{
			GameObject gameObject = new GameObject("TextLabel");
			textLabel = gameObject.AddComponent<TextMesh>();
			gameObject.transform.SetParent(base.transform);
			gameObject.transform.localPosition = new Vector3(0f, 0.8f, 0f);
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
			textLabel.alignment = TextAlignment.Center;
			textLabel.anchor = TextAnchor.MiddleCenter;
		}
		if (Stearing == null)
		{
			Stearing = GetComponent<CircuitCarStearing>();
		}
		UpdateText();
	}

	private void Update()
	{
		if (Stearing != null)
		{
			UpdateText();
		}
	}

	private void UpdateText()
	{
		textLabel.text = "";
		Color color = Color.white;
		switch (Stearing.StateToCheck)
		{
		case 1:
			textLabel.text = "brake";
			color = Color.red;
			break;
		case 2:
			textLabel.text = "reverse";
			color = Color.yellow;
			break;
		case 3:
			textLabel.text = "full";
			break;
		case 4:
			textLabel.text = $"steering, no throttle: {Stearing.AiController.TargetSteering:F1}";
			break;
		default:
			textLabel.text = "coasting";
			break;
		}
		textLabel.color = color;
		textLabel.text = $" {textLabel.text} {Stearing.AdjustedTargetSpeed:N0}";
		textLabel.text = $"{textLabel.text} __ {Stearing.FollowOffset:F2}";
		if (Stearing.StateByCollider != 0)
		{
			switch (Stearing.StateByCollider)
			{
			case 1:
				textLabel.text = "brake? " + textLabel.text;
				break;
			case 2:
				textLabel.text = "<<< " + textLabel.text;
				break;
			case 3:
				textLabel.text = ">>> " + textLabel.text;
				break;
			case 4:
				textLabel.text = "brake by collider  " + textLabel.text;
				textLabel.color = Color.cyan;
				break;
			}
		}
	}
}
