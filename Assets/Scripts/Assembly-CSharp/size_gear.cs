using UnityEngine;

public class size_gear : MonoBehaviour
{
	public new GameObject camera;

	public Color rear;

	public Color neutral;

	public Color other;

	public bool update_car;

	private void Start()
	{
		base.transform.gameObject.SetActive(value: false);
	}

	private int scale_widht(int input)
	{
		return (int)(float)(Screen.width * input / 1366);
	}

	private int scale_height(int input)
	{
		return (int)(float)(Screen.height * input / 730);
	}
}
