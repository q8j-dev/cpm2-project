using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

public class PersonCustomizationWindow : WindowScript
{
	public void CreateClothItem()
	{
	}

	public void CreateClothType()
	{
		CreateClothItem();
	}

	public void OnCustomiazationCategorySelect()
	{
		CreateClothType();
	}
}
