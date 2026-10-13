using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using VContainer;
using VLB;
using _Car_Parking.Scripts.EngineTuning;
using _Car_Parking.Scripts.Infrastructure;

namespace _Car_Parking.Scripts.Drag
{
	public class UIInventory : MonoBehaviour
	{
		private List<UIElement> spawnedItems = new List<UIElement>();

		private const int FpsLimit = 5;

		[Header("For Inventory Stay Null If Not Use")]
		public Transform itemParentTransform;

		private IPrefabFactory _prefabFactory;

		[Inject]
		public void Construct(IPrefabFactory prefabFactory)
		{
			_prefabFactory = prefabFactory;
		}

		public void Init(CarTuningData carTuningData, IClickable clickable)
		{
		}

		public void ClearItemsList()
		{
			if (spawnedItems != null)
			{
				for (int i = 0; i < spawnedItems.Count; i++)
				{
					Object.Destroy(spawnedItems[i].gameObject);
				}
				spawnedItems.Clear();
			}
		}

		public List<UIElement> CreateButtons(List<PartData> partDatas, List<ButtonsSlots> buttonsSlotsList, UIElement buttonPrefab, IClickable clickable)
		{
			List<UIElement> list = new List<UIElement>();
			foreach (PartData part in partDatas)
			{
				ButtonsSlots buttonsSlots = buttonsSlotsList.Find((ButtonsSlots x) => x.PartType == part.PartType);
				if (buttonsSlots != null)
				{
					Transform parent = buttonsSlots.parent;
					UIElement uIElement = _prefabFactory.Instantiate(buttonPrefab, parent);
					SetAnimation(uIElement);
					uIElement.Init(part, clickable);
					if (part.ModelId != 0)
					{
						list.Add(uIElement);
					}
					else
					{
						spawnedItems.Add(uIElement);
					}
				}
			}
			spawnedItems.AddRange(list);
			return list;
		}

		public UIElement CreateButtons(PartData partData, UIElement buttonPrefab, Transform parentForButton, IClickable clickable)
		{
			UIElement uIElement = _prefabFactory.Instantiate(buttonPrefab, parentForButton);
			uIElement.Init(partData, clickable);
			SetAnimation(uIElement);
			return uIElement;
		}

		public UIElement CreateButtonsInInventory(PartData partData, InventoryPartUIElement buttonPrefab, Transform parentForButton, IClickable clickable, int carId, int engineId)
		{
			InventoryPartUIElement inventoryPartUIElement = _prefabFactory.Instantiate(buttonPrefab, parentForButton);
			inventoryPartUIElement.Init(partData, clickable, carId, engineId);
			SetAnimation(inventoryPartUIElement);
			return inventoryPartUIElement;
		}

		private static void SetAnimation(UIElement uiElement)
		{
			float duration = 0.2f;
			CanvasGroup orAddComponent = uiElement.GetOrAddComponent<CanvasGroup>();
			orAddComponent.alpha = 0f;
			orAddComponent.DOFade(1f, duration);
			orAddComponent.transform.localScale *= 0.85f;
			orAddComponent.transform.DOScale(1f, duration).SetEase(Ease.OutSine);
		}
	}
}
