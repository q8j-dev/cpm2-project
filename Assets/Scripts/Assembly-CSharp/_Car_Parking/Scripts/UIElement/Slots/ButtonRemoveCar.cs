using CPM.Database;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using _Car_Parking.Scripts.CarSlots;
using _Car_Parking.Scripts.Drag;
using _Car_Parking.Scripts.Localization;
using _Car_Parking.Scripts.Other.Helpers;
using _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class ButtonRemoveCar : SlotManagerUIButtonBase
	{
		private ISlotService _slotService;

		private ManagerUI _managerUI;

		private ICarInfoService _carInfoService;

		private WindowScript _windowScript;

		protected override int MinSelectedCountToWork => 1;

		[Inject]
		public void Construct(ISlotService slotService, ManagerUI managerUI, ICarInfoService carInfoService)
		{
			_slotService = slotService;
			_managerUI = managerUI;
			_carInfoService = carInfoService;
		}

		protected override void ButtonClickCallback(ButtonWithState button)
		{
			base.ButtonClickCallback(button);
			if (!_slotManagerSelectController.LastSelectedItem)
			{
				return;
			}
			ItemContentDraggableAnimation draggableAnimation = (ItemContentDraggableAnimation)_slotManagerSelectController.LastSelectedItem;
			SlotItemContent slotItemContent = _slotManagerSelectController.LastSelectedItem.GetComponent<SlotItemContent>();
			if (!slotItemContent || !draggableAnimation)
			{
				return;
			}
			CarSlot slot = _slotService.GetSlot(slotItemContent.CarInstanceId);
			if (slot == null)
			{
				return;
			}
			if (_slotService.OccupiedSlotCountInAllGarages == 1)
			{
				_managerUI.ShowNotificationWindow(new LocalizedString("GarageMenuWindow.Can'tremovelastcarfromgarage"), 3f).Forget();
				return;
			}
			Texture texture = Resources.Load(PathGetterHelper.GetCarPreviewPath(_carInfoService.GetFromSlot(slot.ID).CarID), typeof(Texture)) as Texture;
			ShowUI(state: false);
			ConfirmWindow.AdditionalData additionalData = new ConfirmWindow.AdditionalData(new Color(0f, 0f, 0f, 0.75f));
			_managerUI.ShowConfirmWindow(() =>
			{
				if (draggableAnimation.IsSelected)
				{
					ScrollViewBase componentInParent = draggableAnimation.GetComponentInParent<ScrollViewBase>();
					draggableAnimation.TryRevertSelect();
					componentInParent.OnItemSelectChanged(draggableAnimation);
				}
				draggableAnimation.ContentVisibleUpdate(visible: false);
				draggableAnimation.EmptyVisibleUpdate(visible: true);
				slotItemContent.ClearCarInstanceId();
				SlotsViewColumn componentInParent2 = slotItemContent.GetComponentInParent<SlotsViewColumn>();
				if ((bool)componentInParent2)
				{
					componentInParent2.UpdateOccupiedFromUI();
				}
				ShowUI(state: true);
			}, new LocalizedString("GarageMenuWindow.RemoveCar"), new LocalizedString("GarageMenuWindow.AreYouSureYouWantToRemoveThisC"), () =>
			{
				ShowUI(state: true);
			}, new LocalizedString("MarketInventoryPopup.Confirm"), texture, additionalData).Forget();
		}

		private void ShowUI(bool state)
		{
			if (!_windowScript)
			{
				_windowScript = GetComponentInParent<WindowScript>();
			}
			if ((bool)_windowScript)
			{
				_windowScript.ShowUI(state);
			}
		}
	}
}
