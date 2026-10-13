using System;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public class ScrollViewDragAndDropItemContent : ScrollViewBase
	{
		public DragAndDropItemContentConfig DragAndDropItemContentConfig => (DragAndDropItemContentConfig)base.Config;

		public ItemContentDraggable ActiveItemScrollViewItem
		{
			get
			{
				if (!base.ActiveItem)
				{
					return null;
				}
				return (ItemContentDraggable)base.ActiveItem;
			}
		}

		public event Action OnDragStartedEvent;

		public event Action OnSwapEvent;

		protected override void InitStates()
		{
			AddState(new ScrollViewStateIdle(this));
			AddState(new ScrollViewStateItemDown(this));
			AddState(new ScrollViewStateScroll(this));
			ScrollViewStateDrag scrollViewStateDrag = new ScrollViewStateDrag(this);
			scrollViewStateDrag.OnSwapEvent += SwapItemsCallback;
			scrollViewStateDrag.SetConditionController(new DragStateConditionControllerHaveEmpty());
			AddState(scrollViewStateDrag);
		}

		public override void SetState(ScrollViewState.StateType stateType)
		{
			base.SetState(stateType);
			if (base.CurrentState != null && base.CurrentState.State == ScrollViewState.StateType.DragItem)
			{
				OnDragStartedEvent?.Invoke();
			}
		}

		private void SwapItemsCallback(ScrollViewStateDrag scrollViewStateDrag)
		{
			OnSwapEvent?.Invoke();
		}
	}
}
