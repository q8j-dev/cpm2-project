using System;
using CPM.Scripts.Garage.GarageBuyer;
using TMPro;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent;
using _Car_Parking.Scripts.WindowSystem;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class SlotsViewColumn : MonoBehaviour
	{
		public enum PositionType
		{
			Left = 0,
			Right = 1
		}

		[SerializeField]
		private TMP_Text Title = default;

		[SerializeField]
		private ScrollViewSlots ScrollView = default;

		[SerializeField]
		private GameObject GarageIsNotAvailable = default;

		[SerializeField]
		private PositionType Position = default;

		[SerializeField]
		private ButtonBuyGarage ButtonBuyGarage = default;

		[SerializeField]
		private OccupiedCarSlotsUI OccupiedCarSlotsUI = default;

		private IGarageBuyService _garageBuyService;

		public PositionType TypePosition => Position;

		public ScrollViewSlots ScrollViewSlots => ScrollView;

		public OccupiedCarSlotsUI OccupiedCarSlots => OccupiedCarSlotsUI;

		[Inject]
		public void Construct(IGarageBuyService garageBuyService)
		{
			_garageBuyService = garageBuyService;
		}

		public void Init(int garageIndex, Action<int> clickButtonBuyGarageCallback)
		{
			ButtonBuyGarage.Init(garageIndex, clickButtonBuyGarageCallback);
			ScrollView.Init(garageIndex, Position);
			Title.text = string.Format(new LocalizedString("SlotManager.GarageID"), garageIndex + 1);
			OccupiedCarSlotsUI.Init(garageIndex);
			_garageBuyService.GetGarageStatus(garageIndex).Get(out var value);
			ScrollView.gameObject.SetActive(value);
			GarageIsNotAvailable.SetActive(!ScrollView.gameObject.activeSelf);
		}

		public void UpdateOccupiedFromUI()
		{
			int num = 0;
			RectTransform contentRectTransform = ScrollView.ContentRectTransform;
			for (int i = 0; i < contentRectTransform.childCount; i++)
			{
				if (contentRectTransform.GetChild(i).TryGetComponent<ItemContentDraggable>(out var component) && component.Content is SlotItemContent slotItemContent && !string.IsNullOrEmpty(slotItemContent.CarInstanceId))
				{
					num++;
				}
			}
			OccupiedCarSlotsUI.SetOccupiedCount(num);
		}
	}
}
