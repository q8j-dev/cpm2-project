using UnityEngine;
using _Car_Parking.Scripts.UIElement.ScrollViews;

namespace _Car_Parking.Scripts.UIElement.ScrollViewDragAndDropContent
{
	public abstract class ItemContentDraggable : ScrollViewItemInteractiveBase
	{
		[SerializeField]
		protected AddItemViewController AddItemViewController;

		[SerializeField]
		protected RectTransform RectTransformContent;

		[SerializeField]
		protected ItemContentDraggableContent ItemContentDraggableContent;

		private Transform _contentParent;

		public bool CantSwap;

		public abstract bool IsEmpty { get; }

		public RectTransform ContentRectTransform => RectTransformContent;

		public AddItemViewController AddItemView => AddItemViewController;

		public ItemContentDraggableContent Content => ItemContentDraggableContent;

		protected override void Awake()
		{
			base.Awake();
			_contentParent = RectTransformContent.transform.parent;
		}

		public virtual void DestroyAnimation()
		{
			if ((bool)ItemContentDraggableContent)
			{
				ItemContentDraggableContent.DestroyAnimation();
			}
			Object.Destroy(base.gameObject);
		}

		public virtual void NewItemAnimation()
		{
		}

		public void RevertContentParent()
		{
			if (!_contentParent)
			{
				_contentParent = base.transform;
			}
			RectTransformContent.SetParent(_contentParent);
		}

		public virtual void StartDrag()
		{
		}

		public virtual void StopDragBegin()
		{
		}

		public virtual void StopDragEnd()
		{
		}

		public virtual void CopyContent(ItemContentDraggable itemContentDraggable)
		{
			if ((bool)ItemContentDraggableContent)
			{
				ItemContentDraggableContent.CopyContent(itemContentDraggable.ItemContentDraggableContent);
			}
		}

		public virtual void SelectContentVisibleUpdate(bool visible)
		{
		}

		public virtual void EmptyVisibleUpdate(bool visible)
		{
		}

		public virtual void SelectVisibleUpdate(bool visible)
		{
		}

		public virtual void ContentVisibleUpdate(bool visible)
		{
		}
	}
}
