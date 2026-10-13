using UnityEngine;

public class MarketPurchaseWindow : MonoBehaviour
{
	[SerializeField]
	private GameObject LoadingIcon;

	public virtual void Reload()
	{
	}

	public void ShowLoadingIcon(bool show)
	{
		LoadingIcon.SetActive(show);
	}
}
