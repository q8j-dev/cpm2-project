using UnityEngine;
using UnityEngine.UI;

public class AllInventoryFilterGridElement : MonoBehaviour
{
	public GridLayoutGroup GridLayout;

	public ContentSizeFitter ContentSizeFitter;

	public RectTransform RectTransform;

	private void OnEnable()
	{
		GridLayout.enabled = true;
		ContentSizeFitter.enabled = true;
	}

	public void RebuildGrid()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
		GridLayout.enabled = false;
		ContentSizeFitter.enabled = false;
	}
}
