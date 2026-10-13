using System;
using System.Collections.Generic;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class SlotManagerSelectController : IDisposable
	{
		private List<ScrollViewItemInteractiveBase> _selectedItems = new List<ScrollViewItemInteractiveBase>();

		private ScrollViewBase[] _scrollViews;

		public int SelectedCount => _selectedItems.Count;

		public ScrollViewItemInteractiveBase LastSelectedItem
		{
			get
			{
				if (_selectedItems.Count <= 0)
				{
					return null;
				}
				List<ScrollViewItemInteractiveBase> selectedItems = _selectedItems;
				return selectedItems[selectedItems.Count - 1];
			}
		}

		public List<ScrollViewItemInteractiveBase> SelectedItems => _selectedItems;

		public event Action<int> OnSelectCountUpdatedEvent;

		public SlotManagerSelectController(ScrollViewBase[] scrollViews)
		{
			_scrollViews = scrollViews;
			if (_scrollViews == null)
			{
				return;
			}
			for (int i = 0; i < _scrollViews.Length; i++)
			{
				if ((bool)_scrollViews[i])
				{
					ScrollViewBase obj = _scrollViews[i];
					obj.OnItemSelectChangedEvent = (Action<ScrollViewBase, ScrollViewItemInteractiveBase, bool>)Delegate.Combine(obj.OnItemSelectChangedEvent, new Action<ScrollViewBase, ScrollViewItemInteractiveBase, bool>(ItemSelectedChangedCallback));
				}
			}
		}

		public void Dispose()
		{
			if (_scrollViews == null)
			{
				return;
			}
			for (int i = 0; i < _scrollViews.Length; i++)
			{
				if ((bool)_scrollViews[i])
				{
					ScrollViewBase obj = _scrollViews[i];
					obj.OnItemSelectChangedEvent = (Action<ScrollViewBase, ScrollViewItemInteractiveBase, bool>)Delegate.Remove(obj.OnItemSelectChangedEvent, new Action<ScrollViewBase, ScrollViewItemInteractiveBase, bool>(ItemSelectedChangedCallback));
				}
			}
		}

		private void ItemSelectedChangedCallback(ScrollViewBase scrollView, ScrollViewItemInteractiveBase item, bool isSelected)
		{
			if (!item)
			{
				return;
			}
			for (int i = 0; i < _selectedItems.Count; i++)
			{
				if (!_selectedItems[i])
				{
					_selectedItems.RemoveAt(i);
					i--;
				}
			}
			if (isSelected)
			{
				_selectedItems.Add(item);
				if (_selectedItems.Count > 2)
				{
					_selectedItems[0].TryRevertSelect();
					_selectedItems.RemoveAt(0);
				}
			}
			else
			{
				_selectedItems.Remove(item);
			}
			OnSelectCountUpdatedEvent?.Invoke(SelectedCount);
		}
	}
}
