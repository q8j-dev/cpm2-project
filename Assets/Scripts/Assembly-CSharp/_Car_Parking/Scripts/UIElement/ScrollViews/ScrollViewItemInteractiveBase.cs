using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace _Car_Parking.Scripts.UIElement.ScrollViews
{
	public abstract class ScrollViewItemInteractiveBase : ScrollViewItemBase, IPointerDownHandler, IEventSystemHandler, IPointerUpHandler
	{
		public Action<ScrollViewItemInteractiveBase, bool> OnSelectChangedEvent;

		[SerializeField]
		private RectTransform RectTransformSelf = default;

		protected ScrollViewBase _scrollView;

		public RectTransform SelfRectTransform => RectTransformSelf;

		public virtual bool CanDrag => true;

		public virtual bool CanSelect => true;

		public virtual bool IsSelected => true;

		public virtual void TryRevertSelect()
		{
			OnSelectChangedEvent?.Invoke(this, IsSelected);
		}

		public void OnPointerDown(PointerEventData eventData)
		{
			if ((bool)_scrollView)
			{
				_scrollView.OnItemDown(this);
			}
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			if ((bool)_scrollView)
			{
				_scrollView.OnItemUp(this);
			}
		}

		public void OnNewParent(ScrollViewBase scrollView)
		{
			_scrollView = scrollView;
			NewScrollViewInit();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			if (!_scrollView)
			{
				_scrollView = GetComponentInParent<ScrollViewBase>();
				NewScrollViewInit();
			}
		}

		protected virtual void NewScrollViewInit()
		{
		}
	}
}
