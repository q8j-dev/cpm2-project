using UnityEngine;
using _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class DragStateConditionControllerSlots : DragStateConditionControllerHaveEmpty
	{
		private ScrollViewSlots _scrollViewSlots;

		private readonly ScrollViewSlots _originalScrollViewSlots;

		private readonly GameObject _swapCarTip;

		public DragStateConditionControllerSlots(ScrollViewSlots originalScrollViewSlots, GameObject swapCarTip)
		{
			_originalScrollViewSlots = originalScrollViewSlots;
			_swapCarTip = swapCarTip;
		}

		public override void Init(ScrollViewDragAndDropItemContent scrollViewDragAndDropItemContent)
		{
			if ((bool)_scrollViewSlots)
			{
				_scrollViewSlots.UpdateShowAddNewItemError(isOn: false);
			}
			base.Init(scrollViewDragAndDropItemContent);
			_scrollViewSlots = null;
			if ((bool)scrollView)
			{
				_scrollViewSlots = (ScrollViewSlots)scrollView;
			}
			OnDataUpdated();
		}

		public override void Exit()
		{
			base.Exit();
			_swapCarTip.SetActive(value: false);
		}

		public override void FailToDragNewPlace()
		{
			base.FailToDragNewPlace();
			if ((bool)_scrollViewSlots)
			{
				_scrollViewSlots.TryShowBuySlotMessage();
			}
		}

		public override void OnDataUpdated()
		{
			base.OnDataUpdated();
			UpdateSwapCarTip();
			if ((bool)_scrollViewSlots && (bool)_originalScrollViewSlots)
			{
				bool isOn = !CanAddNewItem;
				if (_originalScrollViewSlots.CurrentState != null && _originalScrollViewSlots.CurrentState is ScrollViewStateDrag && ((ScrollViewStateDrag)_originalScrollViewSlots.CurrentState).IsSwapItem)
				{
					isOn = false;
				}
				_scrollViewSlots.UpdateShowAddNewItemError(isOn);
			}
		}

		private void UpdateSwapCarTip()
		{
			_swapCarTip.SetActive(value: false);
			if (_originalScrollViewSlots.CurrentState != null && _originalScrollViewSlots.CurrentState is ScrollViewStateDrag && ((ScrollViewStateDrag)_originalScrollViewSlots.CurrentState).IsSwapItem)
			{
				_swapCarTip.SetActive(value: true);
			}
		}
	}
}
