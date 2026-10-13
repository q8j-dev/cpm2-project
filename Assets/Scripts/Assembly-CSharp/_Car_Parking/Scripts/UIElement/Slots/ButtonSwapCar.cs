using _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class ButtonSwapCar : SlotManagerUIButtonBase
	{
		protected override int MinSelectedCountToWork => 2;

		protected override void ButtonClickCallback(ButtonWithState button)
		{
			base.ButtonClickCallback(button);
			if (_slotManagerSelectController.SelectedCount >= MinSelectedCountToWork)
			{
				SlotItem slotItem = (SlotItem)_slotManagerSelectController.SelectedItems[0];
				SlotItemContent component = _slotManagerSelectController.SelectedItems[0].GetComponent<SlotItemContent>();
				SlotItem slotItem2 = (SlotItem)_slotManagerSelectController.SelectedItems[1];
				SlotItemContent component2 = _slotManagerSelectController.SelectedItems[1].GetComponent<SlotItemContent>();
				if ((bool)slotItem && (bool)component && (bool)slotItem2 && (bool)component2 && slotItem.CanDrag && slotItem2.CanDrag)
				{
					ScrollViewDragAndDropItemContentHelper.SwapItems(slotItem, slotItem2);
				}
			}
		}
	}
}
