using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.EngineTuning;

public class CylinderHeadUIContainer : MonoBehaviour
{
	public Transform[] cylinderHeadParents;

	public Transform[] camshaftsParent;

	public TextMeshProUGUI cylinderHeadSideText;

	public Button leftArrow;

	public Button rightArrow;

	public GameObject leftSide;

	public GameObject rightSide;

	public UIInventory uiInventory;

	private void Start()
	{
		leftArrow.onClick.AddListener(OnArrowClick);
		rightArrow.onClick.AddListener(OnArrowClick);
	}

	public void InitCylinderHeads(PartData cylinderHeads, UIElement buttonPrefab, IClickable clickable)
	{
		List<UIElement> list = new List<UIElement>();
		list.Add(uiInventory.CreateButtons(cylinderHeads, buttonPrefab, cylinderHeadParents[0], clickable));
		SetIndexes(list);
	}

	public List<UIElement> InitCamshafts(PartData cylinderHeads, List<PartData> camshafts, UIElement buttonPrefab, IClickable clickable)
	{
		List<UIElement> list = new List<UIElement>();
		switch (camshafts.Count)
		{
		case 1:
			list.Add(uiInventory.CreateButtons(camshafts[0], buttonPrefab, camshaftsParent[0], clickable));
			break;
		case 2:
		{
			for (int j = 0; j < 2; j++)
			{
			}
			break;
		}
		case 4:
		{
			for (int i = 0; i < 4; i++)
			{
				list.Add(uiInventory.CreateButtons(camshafts[i], buttonPrefab, camshaftsParent[(i >= 2) ? 1u : 0u], clickable));
			}
			break;
		}
		}
		SetIndexes(list);
		return list;
	}

	private void SetIndexes(List<UIElement> partDataArray)
	{
		for (int i = 0; i < partDataArray.Count; i++)
		{
			if (partDataArray[i] is InventoryUIElement inventoryUIElement)
			{
				inventoryUIElement.IndexForMarket = i;
			}
		}
	}

	private void OnArrowClick()
	{
		bool activeInHierarchy = leftSide.activeInHierarchy;
		cylinderHeadSideText.text = (activeInHierarchy ? "Right" : "Left");
		leftSide.gameObject.SetActive(!activeInHierarchy);
		rightSide.gameObject.SetActive(activeInHierarchy);
	}
}
