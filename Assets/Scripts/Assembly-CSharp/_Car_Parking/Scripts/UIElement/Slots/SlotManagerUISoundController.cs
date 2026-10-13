using System;
using AK.Wwise;
using UnityEngine;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.Slots
{
	public class SlotManagerUISoundController : MonoBehaviour
	{
		[Serializable]
		private class SoundData
		{
			public AK.Wwise.Event UIEvent = default;

			public Switch WwiseSound = default;

			public void Play(GameObject gameObject)
			{
				WwiseSound?.SetValue(gameObject);
				UIEvent?.Post(gameObject);
			}
		}

		[SerializeField]
		private SlotManagerWindow SlotManagerWindow = default;

		[SerializeField]
		private SoundData SwapCars = default;

		[SerializeField]
		private SoundData StartDrag = default;

		[SerializeField]
		private SoundData StopDrag = default;

		private void OnEnable()
		{
			SlotManagerWindow.ScrollViewColumnCurrent.ScrollViewSlots.OnDragStartedEvent += StartDragCallback;
			SlotManagerWindow.ScrollViewColumnOther.ScrollViewSlots.OnDragStartedEvent += StartDragCallback;
			SlotManagerWindow.ScrollViewColumnCurrent.ScrollViewSlots.OnSwapEvent += SwapItemsCallback;
			SlotManagerWindow.ScrollViewColumnOther.ScrollViewSlots.OnSwapEvent += SwapItemsCallback;
			ScrollViewSlots scrollViewSlots = SlotManagerWindow.ScrollViewColumnCurrent.ScrollViewSlots;
			scrollViewSlots.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Combine(scrollViewSlots.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(StateChangedCallback));
			ScrollViewSlots scrollViewSlots2 = SlotManagerWindow.ScrollViewColumnOther.ScrollViewSlots;
			scrollViewSlots2.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Combine(scrollViewSlots2.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(StateChangedCallback));
		}

		private void OnDisable()
		{
			SlotManagerWindow.ScrollViewColumnCurrent.ScrollViewSlots.OnDragStartedEvent -= StartDragCallback;
			SlotManagerWindow.ScrollViewColumnOther.ScrollViewSlots.OnDragStartedEvent -= StartDragCallback;
			SlotManagerWindow.ScrollViewColumnCurrent.ScrollViewSlots.OnSwapEvent -= SwapItemsCallback;
			SlotManagerWindow.ScrollViewColumnOther.ScrollViewSlots.OnSwapEvent -= SwapItemsCallback;
			ScrollViewSlots scrollViewSlots = SlotManagerWindow.ScrollViewColumnCurrent.ScrollViewSlots;
			scrollViewSlots.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Remove(scrollViewSlots.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(StateChangedCallback));
			ScrollViewSlots scrollViewSlots2 = SlotManagerWindow.ScrollViewColumnOther.ScrollViewSlots;
			scrollViewSlots2.OnStateChangedEvent = (Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>)Delegate.Remove(scrollViewSlots2.OnStateChangedEvent, new Action<ScrollViewBase, ScrollViewBase.ScrollViewStateChangeData>(StateChangedCallback));
		}

		private void StartDragCallback()
		{
			StartDrag.Play(base.gameObject);
		}

		private void SwapItemsCallback()
		{
			SwapCars.Play(base.gameObject);
		}

		private void StateChangedCallback(ScrollViewBase scrollViewBase, ScrollViewBase.ScrollViewStateChangeData data)
		{
			if (data.OldState != null && data.OldState.State == ScrollViewState.StateType.DragItem)
			{
				StopDrag.Play(base.gameObject);
			}
		}
	}
}
