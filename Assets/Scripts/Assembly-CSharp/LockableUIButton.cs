using UnityEngine;
using UnityEngine.UI;

public class LockableUIButton : Button
{
	[Header("Lock")]
	[SerializeField]
	private float LockedAlphaValue;

	[SerializeField]
	private CanvasGroup PanelCanvasGroup;

	[SerializeField]
	private GameObject LockIcon;

	public void Lock(bool state)
	{
		base.interactable = !state;
		PanelCanvasGroup.alpha = (state ? LockedAlphaValue : 1f);
		LockIcon.SetActive(state);
	}
}
