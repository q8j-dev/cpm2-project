using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class CopyAndFollowRect : MonoBehaviour
{
	public MaskableGraphic Origin;

	private MaskableGraphic self;

	private void Awake()
	{
		self = GetComponent<MaskableGraphic>();
	}

	private void LateUpdate()
	{
		if ((bool)Origin && (bool)self)
		{
			self.transform.position = Origin.transform.position;
			self.rectTransform.sizeDelta = new Vector2(Origin.rectTransform.rect.width, Origin.rectTransform.rect.height);
		}
	}
}
