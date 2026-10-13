using UnityEngine;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public class ItemDownModuleSelectItem : StateModule
	{
		private float _timeNow;

		public ItemDownModuleSelectItem(ScrollViewBase scrollViewBase, ScrollViewState scrollViewState)
			: base(scrollViewBase, scrollViewState)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			_timeNow = Time.time;
		}

		public override void OnItemUp()
		{
			base.OnItemUp();
			if (base.ScrollViewConfig.ItemSelect.IsWorking && Time.time - _timeNow < base.ScrollViewConfig.ItemSelect.TimeToSelect && (bool)scrollViewBase && (bool)base.ActiveItem && base.ActiveItem.CanSelect)
			{
				base.ActiveItem.TryRevertSelect();
				scrollViewBase.OnItemSelectChanged(base.ActiveItem);
			}
		}
	}
}
