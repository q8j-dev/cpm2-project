using System;
using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

[Serializable]
public class oneProductShopData
{
	public Texture texture;

	public ObscuredInt coinPrice;

	public string description;

	public string idPrefs = "";

	public string inAppID;

	public string inAppPrice;

	public int carID;

	public bool isHome;

	public bool isHorn;

	public bool isForPerson;

	public int gender;

	public int type;

	public int idEquipment;

	public int width;

	public int height;

	public OneProductShopWindowUI uiCard;

	private string _productID = "";

	public string ProductID
	{
		get
		{
			return _productID;
		}
		set
		{
			_productID = value;
		}
	}
}
