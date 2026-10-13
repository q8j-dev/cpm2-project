using UnityEngine;

public class DrawLine : MonoBehaviour
{
	private Vector2 meshPoint;

	private Vector2 uiImage;

	private bool isInit;

	public void InitLine(Vector2 meshPoint, Vector2 ImagePoint)
	{
		this.meshPoint = meshPoint;
		uiImage = ImagePoint;
		isInit = true;
		Vector2[] transforms = new Vector2[2] { this.meshPoint, ImagePoint };
		DrawLines(transforms);
	}

	private void DrawLines(Vector2[] transforms)
	{
		base.gameObject.SetActive(value: false);
		base.gameObject.SetActive(value: true);
	}
}
