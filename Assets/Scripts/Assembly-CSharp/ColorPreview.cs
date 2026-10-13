using UnityEngine;
using UnityEngine.UI;

public class ColorPreview : MonoBehaviour
{
	public Graphic previewGraphic;

	public ColorPicker colorPicker;

	private void Start()
	{
		previewGraphic.color = colorPicker.color;
	}

	public void OnColorChanged(Color c)
	{
		previewGraphic.color = c;
	}

	private void OnDestroy()
	{
	}
}
