using UnityEngine;

public class highlightTest : MonoBehaviour
{
	public PointHighlight Highlight;

	public int count;

	private float time = 1f;

	private void Start()
	{
	}

	private void Update()
	{
		time -= Time.deltaTime;
		if (time < 0f)
		{
			time = 2f;
			Highlight.HighlightPoint("Player 1", Random.Range(0, count));
		}
	}
}
