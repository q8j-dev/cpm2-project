using System;
using UnityEngine;

public class OneProductShopWindowUI : MonoBehaviour
{
	public oneProductShopData data;

	public ShopWindowUI shopWindowUI;

	public AudioClip clip;

	public void FocusOnThisProduct()
	{
		if ((bool)shopWindowUI)
		{
			shopWindowUI.onProductSelected(data);
		}
	}

	public void SetUIData(oneProductShopData one)
	{
		try
		{
			data = one;
			data.uiCard = this;
			Texture texture = one.texture;
			if (one.carID != 0)
			{
				texture = Resources.Load("UI/CarImages/b" + one.carID, typeof(Texture)) as Texture;
				one.texture = texture;
			}
			if (!(one.inAppID.Trim() == ""))
			{
				if (V.StoreController != null)
				{
					string inAppID = data.inAppID;
					data.ProductID = inAppID;
				}
				else
				{
					Debug.LogError("Shop controller is null!");
				}
			}
		}
		catch (Exception message)
		{
			Debug.LogError(message);
		}
	}
}
