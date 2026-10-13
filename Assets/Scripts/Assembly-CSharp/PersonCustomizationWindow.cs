using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;

public class PersonCustomizationWindow : WindowScript
{
	[SerializeField]
	private ScrollRect clothItemsScrollRect;

	[SerializeField]
	private ScrollRect clothCategoryItemsScrollRect;

	[SerializeField]
	private ScrollRect clothTypeItemsScrollRect;

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
